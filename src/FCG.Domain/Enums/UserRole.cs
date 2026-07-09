namespace FCG.Domain.Enums;

/// <summary>Níveis de acesso da plataforma.</summary>
public enum UserRole
{
    /// <summary>Acesso à plataforma e à biblioteca de jogos.</summary>
    User = 1,

    /// <summary>Cadastra jogos, administra usuários e cria promoções.</summary>
    Administrator = 2
}
