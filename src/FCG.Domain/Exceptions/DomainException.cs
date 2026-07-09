namespace FCG.Domain.Exceptions;

/// <summary>Lançada quando uma invariante de domínio é violada.</summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
