# Relatório de Entrega — Tech Challenge Fase 1

> Preencha os campos abaixo e poste este arquivo (PDF ou TXT) na data da entrega.

## Grupo

- **Nome do grupo:** _[preencher]_

## Participantes

| Nome | Username no Discord |
|---|---|
| _[preencher]_ | _[preencher]_ |
| _[preencher]_ | _[preencher]_ |

## Links

- **Documentação (Miro / equivalente):** _[preencher]_
  - Também versionada no repositório em [`docs/`](../docs) (Event Storming, Domain Storytelling e arquitetura).
- **Repositório:** _[preencher]_
- **Vídeo (YouTube ou preferência):** _[preencher]_

## Checklist dos requisitos

### Obrigatórios
- [x] Cadastro de usuários (nome, e-mail, senha) com validação de e-mail e senha segura
- [x] Autenticação/autorização via JWT com dois níveis (Usuário / Administrador)
- [x] Arquitetura monolítica (MVP)
- [x] Persistência com EF Core + migrations
- [x] Middleware de tratamento de erros e logs estruturados
- [x] Documentação Swagger
- [x] Testes unitários das regras de negócio
- [x] TDD/BDD em pelo menos um módulo (BDD no módulo de autenticação)
- [x] Modelagem em DDD (entidades, VOs, agregados) + Event Storming

### Opcionais implementados
- [x] Dapper (consultas de alta performance)
- [x] GraphQL (consulta dinâmica de jogos)
- [x] Domain Storytelling

## Como executar (resumo)

```bash
docker compose up -d cloudgames-postgres     # PostgreSQL (porta 5433)
dotnet run --project src/PosTech.Fiap.CloudGames.Api      # aplica migrations + seed e sobe em http://localhost:5080
dotnet test                           # 62 testes (unitários + BDD)
```

Detalhes completos no [README.md](../README.md).
