# Arquitetura — FIAP Cloud Games (Fase 1)

## Visão geral

MVP entregue como **monólito** organizado segundo **Domain-Driven Design** e **Clean Architecture**.
O objetivo é isolar as regras de negócio (domínio) de detalhes de infraestrutura (banco, web, autenticação),
mantendo o núcleo testável e independente de frameworks.

## Contexto delimitado (Bounded Context)

Nesta fase há um único contexto — **Plataforma FCG** — que engloba:

- **Gestão de acesso**: cadastro, autenticação e perfis de usuário.
- **Catálogo**: jogos disponíveis para aquisição.
- **Biblioteca**: jogos adquiridos por cada usuário.
- **Promoções**: descontos temporários aplicados a jogos.

## Camadas e regra de dependência

```
┌─────────────────────────────────────────────────────────────┐
│                        PosTech.Fiap.CloudGames.Api                               │
│   Minimal API · Middleware · Swagger · GraphQL · JWT · DI    │
└───────────────┬─────────────────────────────┬───────────────┘
                │                             │
                ▼                             ▼
      ┌───────────────────┐        ┌────────────────────────┐
      │  PosTech.Fiap.CloudGames.Application  │        │   PosTech.Fiap.CloudGames.Infrastructure    │
      │  Casos de uso     │◄───────│  EF Core · Dapper · JWT │
      │  DTOs · Validação │        │  BCrypt · Repositórios  │
      └─────────┬─────────┘        └───────────┬────────────┘
                │                             │
                ▼                             ▼
              ┌───────────────────────────────────┐
              │            PosTech.Fiap.CloudGames.Domain             │
              │  Entidades · VOs · Eventos ·      │
              │  Regras · Interfaces de repo      │
              └───────────────────────────────────┘
```

**Regra de dependência:** o código-fonte só depende "para dentro". O domínio não conhece EF, ASP.NET ou JWT.
A infraestrutura implementa as **interfaces** declaradas no domínio (repositórios) e na aplicação
(`IPasswordHasher`, `IJwtTokenGenerator`, `IGameReadQueries`).

## Agregados e value objects

| Agregado (raiz) | Descrição | Invariantes |
|---|---|---|
| **User** | Usuário e sua biblioteca (`UserGame`) | E-mail único e válido; senha forte; sem jogos duplicados na biblioteca |
| **Game** | Jogo do catálogo | Título e gênero obrigatórios; preço ≥ 0 |
| **Promotion** | Promoção de desconto (`PromotionGame`) | Desconto entre 0 e 100%; período válido; ao menos um jogo |

**Value Objects:** `Email` (formato válido, normalizado), `Password` (política de segurança), `Money` (valor ≥ 0, arredondado).

Os VOs concentram as validações de formato/consistência, garantindo que estados inválidos não sejam representáveis.

## Padrões aplicados

- **Repository + Unit of Work** — abstração de persistência declarada no domínio, implementada com EF Core.
- **CQRS leve** — `*CommandService` (escrita, via EF/change-tracking) e `*QueryService` (leitura, via Dapper).
- **Value Objects** — encapsulam regras de formato e igualdade por valor.
- **Domain Events** — `UserRegistered`, `GameCreated`, `GameAcquired`, `PromotionCreated` registram fatos de negócio
  (base para reações futuras: e-mail de boas-vindas, notificações etc.).
- **Middleware de exceções** — traduz exceções de domínio/aplicação em respostas `ProblemDetails` (RFC 7807).

## Decisões técnicas

- **Minimal API** com endpoints agrupados por feature (extension methods).
- **EF Core + PostgreSQL** com convenção `snake_case` e conversão de Value Objects para colunas.
- **Dapper** para leituras de alta performance (catálogo com preço efetivo e biblioteca), sem o overhead do change-tracking.
- **JWT** com dois papéis (`User`, `Administrator`) via policies de autorização.
- **GraphQL** (HotChocolate) para consulta dinâmica de jogos.
