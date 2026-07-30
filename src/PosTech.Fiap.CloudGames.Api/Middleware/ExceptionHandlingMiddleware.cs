using System.Net;
using PosTech.Fiap.CloudGames.Application.Common.Exceptions;
using PosTech.Fiap.CloudGames.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FluentValidation.ValidationException;

namespace PosTech.Fiap.CloudGames.Api.Middleware;

/// <summary>
/// Requisito (Desafio Fase 1 · RT-06): "middleware para tratamento de erros e logs estruturados".
/// Converte exceções não tratadas em respostas ProblemDetails (RFC 7807) com log estruturado.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private const string ProblemJsonContentType = "application/problem+json";

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (status, title) = Map(exception);

        if (status >= (int)HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Erro não tratado ao processar {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning("Requisição rejeitada ({Status}): {Message}", status, exception.Message);

        if (context.Response.HasStarted)
        {
            _logger.LogError(
                "Resposta já iniciada para {Method} {Path}; conexão abortada sem ProblemDetails.",
                context.Request.Method,
                context.Request.Path);

            context.Abort();
            return;
        }

        ProblemDetails problem = exception is ValidationException validation
            ? new ValidationProblemDetails(ToErrors(validation))
            {
                Status = status,
                Title = title,
                Type = $"https://httpstatuses.io/{status}"
            }
            : new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = status >= (int)HttpStatusCode.InternalServerError ? "Ocorreu um erro inesperado." : exception.Message,
                Type = $"https://httpstatuses.io/{status}"
            };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.Clear();
        context.Response.StatusCode = status;

        await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: ProblemJsonContentType);
    }

    private static (int Status, string Title) Map(Exception exception) => exception switch
    {
        ValidationException => ((int)HttpStatusCode.BadRequest, "Erro de validação."),
        BadHttpRequestException => ((int)HttpStatusCode.BadRequest, "Requisição malformada."),
        DomainException => ((int)HttpStatusCode.BadRequest, "Regra de negócio violada."),
        NotFoundException => ((int)HttpStatusCode.NotFound, "Recurso não encontrado."),
        ConflictException => ((int)HttpStatusCode.Conflict, "Conflito com o estado atual."),
        UnauthorizedException => ((int)HttpStatusCode.Unauthorized, "Não autenticado."),
        ForbiddenException => ((int)HttpStatusCode.Forbidden, "Acesso negado."),
        _ => ((int)HttpStatusCode.InternalServerError, "Erro interno do servidor.")
    };

    private static IDictionary<string, string[]> ToErrors(ValidationException exception)
        => exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
}
