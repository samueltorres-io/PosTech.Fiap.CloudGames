# ✅ Requisitos Atendidos — Rastreabilidade

Mapa de cada requisito do [desafio](desafio.md) para **onde** e **como** foi atendido no código.
Os identificadores (`RF-xx`, `RT-xx`) também aparecem em comentários `Requisito (Desafio Fase 1): ...`
espalhados pelo código-fonte.

## Funcionalidades obrigatórias

| ID | Requisito | Onde está | Como foi atendido |
|---|---|---|---|
| **RF-01** | Identificação por nome, e-mail e senha | `src/PosTech.Fiap.CloudGames.Domain/Entities/User.cs`, `src/PosTech.Fiap.CloudGames.Application/Users/Dtos/UserDtos.cs` | Entidade `User` (nome + `Email` + `PasswordHash`); DTO `RegisterUserRequest` |
| **RF-02** | Validar e-mail e senha segura (8+, letras, números, especiais) | `src/PosTech.Fiap.CloudGames.Domain/ValueObjects/Email.cs`, `Password.cs`, `src/PosTech.Fiap.CloudGames.Application/Users/UserValidators.cs` | VOs `Email` e `Password` (invariantes de domínio) + `RegisterUserRequestValidator` (validação de borda) |
| **RF-03** | Autenticação via JWT | `src/PosTech.Fiap.CloudGames.Infrastructure/Security/JwtTokenGenerator.cs`, `src/PosTech.Fiap.CloudGames.Api/Extensions/ApiServiceCollectionExtensions.cs` | Emissão do token (claims `sub`/`email`/`role`) + `AddJwtBearer` validando emissor/audiência/assinatura |
| **RF-04** | Dois níveis de acesso (Usuário/Administrador) | `src/PosTech.Fiap.CloudGames.Domain/Enums/UserRole.cs`, `src/PosTech.Fiap.CloudGames.Api/Endpoints/AuthorizationPolicies.cs` + `RequireAuthorization` nos endpoints | Enum `UserRole`; policy `Admin` (`RequireRole`) protegendo jogos/usuários/promoções |
| **RF-05** | Arquitetura monolítica (MVP) | `Fcg.sln`, [`docs/arquitetura.md`](arquitetura.md) | Solução única com camadas DDD/Clean Architecture |

## Requisitos técnicos

| ID | Requisito | Onde está | Como foi atendido |
|---|---|---|---|
| **RT-01** | Entity Framework Core | `src/PosTech.Fiap.CloudGames.Infrastructure/Persistence/CloudGamesDbContext.cs` + `Configurations/` | `DbContext` + `IEntityTypeConfiguration` para User/Game/Promotion (VOs convertidos em colunas) |
| **RT-02** | Migrations | `src/PosTech.Fiap.CloudGames.Infrastructure/Persistence/Migrations/` | Migration `InitialCreate` aplicada no startup (`MigrateAsync`) |
| **RT-03** | *(Opcional)* MongoDB | — | Não utilizado; persistência principal é PostgreSQL |
| **RT-04** | *(Opcional)* Dapper | `src/PosTech.Fiap.CloudGames.Infrastructure/Persistence/Dapper/GameReadQueries.cs` | Catálogo (com preço promocional efetivo) e biblioteca via SQL + Dapper |
| **RT-05** | Minimal API ou MVC | `src/PosTech.Fiap.CloudGames.Api/Endpoints/*.cs`, `Program.cs` | **Minimal API** com grupos por feature |
| **RT-06** | Middleware de erros + logs estruturados | `src/PosTech.Fiap.CloudGames.Api/Middleware/ExceptionHandlingMiddleware.cs`, `Program.cs` | Exceções → `ProblemDetails` (RFC 7807); **Serilog** (console + arquivo) + request logging |
| **RT-07** | Swagger | `src/PosTech.Fiap.CloudGames.Api/Extensions/ApiServiceCollectionExtensions.cs`, `WebApplicationExtensions.cs` | `AddSwaggerGen` com esquema Bearer + `UseSwaggerUI` |
| **RT-08** | *(Opcional)* GraphQL | `src/PosTech.Fiap.CloudGames.Api/GraphQL/Query.cs` | HotChocolate: `games(where/order)` com filtragem/ordenação dinâmicas |
| **RT-09** | Testes unitários | `test/PosTech.Fiap.CloudGames.Domain.Tests/`, `test/PosTech.Fiap.CloudGames.Application.Tests/` | 59 testes de VOs, entidades e casos de uso (xUnit + FluentAssertions + Moq) |
| **RT-10** | TDD ou **BDD** em um módulo | `test/PosTech.Fiap.CloudGames.Bdd.Tests/Features/Autenticacao.feature` | **BDD** (Reqnroll) no módulo de autenticação (cadastro + login) |
| **RT-11** | Event Storming | [`docs/event-storming/README.md`](event-storming/README.md) | Fluxos de criação de usuários e de jogos (+ aquisição) |
| **RT-12** | *(Opcional)* Domain Storytelling | [`docs/domain-storytelling/README.md`](domain-storytelling/README.md) | Cenário "usuário adquire um jogo" |
| **RT-13** | Princípios de DDD | `src/PosTech.Fiap.CloudGames.Domain/`, [`docs/arquitetura.md`](arquitetura.md) | Entidades/agregados, Value Objects, eventos de domínio, repositórios |

## Entregáveis

| Entregável | Status |
|---|---|
| Código-fonte (APIs + testes) | ✅ neste repositório |
| README.md completo | ✅ [`README.md`](../README.md) |
| Documentação DDD (Event Storming + diagramas) | ✅ [`docs/`](.) |
| Vídeo (até 15 min) | ⬜ a gravar |
| Relatório de entrega | ⬜ modelo em [`relatorio-entrega.md`](relatorio-entrega.md) |
