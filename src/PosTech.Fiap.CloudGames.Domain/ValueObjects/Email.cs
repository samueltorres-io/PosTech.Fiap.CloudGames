using System.Text.RegularExpressions;
using PosTech.Fiap.CloudGames.Domain.Exceptions;

namespace PosTech.Fiap.CloudGames.Domain.ValueObjects;

/// <summary>
/// Requisito (Desafio Fase 1 · RF-02): "validar formato de e-mail".
/// E-mail válido e normalizado (minúsculas, sem espaços nas bordas).
/// </summary>
public sealed partial class Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("E-mail é obrigatório.");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > 320 || !EmailRegex().IsMatch(normalized))
            throw new DomainException($"E-mail '{value}' possui formato inválido.");

        return new Email(normalized);
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is Email other && other.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
