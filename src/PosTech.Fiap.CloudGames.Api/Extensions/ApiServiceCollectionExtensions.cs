using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PosTech.Fiap.CloudGames.Api.Endpoints;
using PosTech.Fiap.CloudGames.Api.GraphQL;
using PosTech.Fiap.CloudGames.Api.Identity;
using PosTech.Fiap.CloudGames.Application.Abstractions;
using PosTech.Fiap.CloudGames.Domain.Enums;
using PosTech.Fiap.CloudGames.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace PosTech.Fiap.CloudGames.Api.Extensions;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddAuthenticationAndAuthorization(configuration);
        services.AddOpenApiDocumentation();

        services.AddGraphQLServer()
            .AddQueryType<Query>()
            .AddFiltering()
            .AddSorting();

        return services;
    }

    // Requisito (Desafio Fase 1 · RF-03/RF-04): autenticação JWT e dois níveis de acesso
    // (policy "Admin" para cadastro de jogos, administração de usuários e promoções).
    private static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>() ?? new JwtSettings();

        // Mantém os claims com seus nomes originais (sub, email, role).
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                    ValidateLifetime = true,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.Admin, policy =>
                policy.RequireRole(nameof(UserRole.Administrator)));
        });

        return services;
    }

    private static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "FIAP Cloud Games API",
                Version = "v1",
                Description = "API REST para cadastro de usuários e biblioteca de jogos (Tech Challenge - Fase 1)."
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token JWT (sem o prefixo 'Bearer').",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };

            options.AddSecurityDefinition("Bearer", scheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
        });

        return services;
    }
}
