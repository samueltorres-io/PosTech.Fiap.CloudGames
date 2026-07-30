using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging.Abstractions;
using PosTech.Fiap.CloudGames.Api.Middleware;
using PosTech.Fiap.CloudGames.Domain.Exceptions;

namespace PosTech.Fiap.CloudGames.Api.Tests.Middleware;

/// <summary>
/// Requisito (Desafio Fase 1 · RT-06): "middleware para tratamento de erros".
/// </summary>
public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Deve_converter_excecao_de_dominio_em_problem_details_400()
    {
        var context = CreateContext();

        await Invoke(context, new DomainException("O título do jogo é obrigatório."));

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.ContentType.Should().StartWith("application/problem+json");

        var problem = ReadBody(context);
        problem.GetProperty("title").GetString().Should().Be("Regra de negócio violada.");
        problem.GetProperty("detail").GetString().Should().Be("O título do jogo é obrigatório.");
        problem.GetProperty("traceId").GetString().Should().Be(context.TraceIdentifier);
    }

    [Fact]
    public async Task Deve_ocultar_detalhes_de_excecao_inesperada_no_500()
    {
        var context = CreateContext();

        await Invoke(context, new InvalidOperationException("connection string secreta"));

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var problem = ReadBody(context);
        problem.GetProperty("detail").GetString().Should().Be("Ocorreu um erro inesperado.");
        problem.GetProperty("detail").GetString().Should().NotContain("secreta");
    }

    [Fact]
    public async Task Deve_abortar_a_conexao_quando_a_resposta_ja_comecou()
    {
        var lifetime = new RecordingLifetimeFeature();
        var context = CreateContext(responseStarted: true, lifetime: lifetime);

        await Invoke(context, new DomainException("falha no meio da serialização"));

        lifetime.Aborted.Should().BeTrue();
        BodyAsString(context).Should().BeEmpty();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK, "o status já foi enviado e não pode ser reescrito");
    }

    private static Task Invoke(HttpContext context, Exception exception)
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        return middleware.InvokeAsync(context);
    }

    private static DefaultHttpContext CreateContext(
        bool responseStarted = false,
        IHttpRequestLifetimeFeature? lifetime = null)
    {
        var context = new DefaultHttpContext();

        if (responseStarted)
            context.Features.Set<IHttpResponseFeature>(new StartedResponseFeature());

        if (lifetime is not null)
            context.Features.Set(lifetime);

        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/api/v1/games";
        context.Response.Body = new MemoryStream();

        return context;
    }

    private static JsonElement ReadBody(HttpContext context)
        => JsonDocument.Parse(BodyAsString(context)).RootElement;

    private static string BodyAsString(HttpContext context)
    {
        var body = (MemoryStream)context.Response.Body;
        return Encoding.UTF8.GetString(body.ToArray());
    }

    private sealed class StartedResponseFeature : HttpResponseFeature
    {
        public override bool HasStarted => true;
    }

    private sealed class RecordingLifetimeFeature : IHttpRequestLifetimeFeature
    {
        public bool Aborted { get; private set; }

        public CancellationToken RequestAborted { get; set; }

        public void Abort() => Aborted = true;
    }
}