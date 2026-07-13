// ============================================================================
//  FIAP Cloud Games (FCG) — Tech Challenge Fase 1
//  Composição da aplicação (enunciado em docs/desafio.md; rastreabilidade em
//  docs/requisitos-atendidos.md). Requisitos técnicos atendidos aqui:
//   - RT-05: API .NET 8 no padrão Minimal API (endpoints em MapFcgEndpoints).
//   - RT-06: middleware de erros + logs estruturados (Serilog abaixo).
//   - RT-07: documentação Swagger (AddApiServices / UseApiPipeline).
//   - RT-08: GraphQL (AddApiServices) · RF-03/RF-04: autenticação e papéis JWT.
// ============================================================================

using PosTech.Fiap.CloudGames.Api.Extensions;
using PosTech.Fiap.CloudGames.Application;
using PosTech.Fiap.CloudGames.Infrastructure;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Requisito (Desafio Fase 1 · RT-06): logs estruturados com Serilog (console + arquivo diário).
builder.Host.UseSerilog((context, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/cloudgames-.log", rollingInterval: RollingInterval.Day));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApiPipeline();
app.MapFcgEndpoints();

if (!app.Environment.IsEnvironment("Testing"))
{
    await app.ApplyMigrationsAndSeedAsync();
}

await app.RunAsync();

// Necessário para testes de integração baseados em WebApplicationFactory.
public partial class Program;
