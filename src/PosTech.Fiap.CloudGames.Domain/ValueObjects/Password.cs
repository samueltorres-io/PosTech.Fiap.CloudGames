using PosTech.Fiap.CloudGames.Domain.Exceptions;

namespace PosTech.Fiap.CloudGames.Domain.ValueObjects;

/// <summary>
/// Requisito (Desafio Fase 1 · RF-02): "senha segura (mínimo de 8 caracteres com
/// números, letras e caracteres especiais)".
/// Senha em texto puro validada pela política de segurança — objeto transitório:
/// nunca é persistido, apenas o hash resultante é armazenado.
/// </summary>
public sealed class Password
{
    public const int MinLength = 8;

    public string Value { get; }

    private Password(string value) => Value = value;

    public static Password Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Senha é obrigatória.");

        if (value.Length < MinLength)
            throw new DomainException($"A senha deve ter no mínimo {MinLength} caracteres.");

        if (!value.Any(char.IsLetter))
            throw new DomainException("A senha deve conter ao menos uma letra.");

        if (!value.Any(char.IsDigit))
            throw new DomainException("A senha deve conter ao menos um número.");

        if (!value.Any(c => !char.IsLetterOrDigit(c)))
            throw new DomainException("A senha deve conter ao menos um caractere especial.");

        return new Password(value);
    }

    public override string ToString() => "********";
}
