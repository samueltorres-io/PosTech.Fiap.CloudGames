using FCG.Api.Extensions;
using FCG.Application;
using FCG.Infrastructure;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Log estruturado com Serilog (console + arquivo diário).
builder.Host.UseSerilog((context, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/fcg-.log", rollingInterval: RollingInterval.Day));

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
