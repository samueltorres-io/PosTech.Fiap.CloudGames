namespace PosTech.Fiap.CloudGames.Domain.Enums;

/// <summary>
/// Requisito (Desafio Fase 1 · RF-04): "dois níveis de acesso" — Usuário e Administrador.
/// </summary>
public enum UserRole
{
    /// <summary>Acesso à plataforma e à biblioteca de jogos.</summary>
    User = 1,

    /// <summary>Cadastra jogos, administra usuários e cria promoções.</summary>
    Administrator = 2
}
