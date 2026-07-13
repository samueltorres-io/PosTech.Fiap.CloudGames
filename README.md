# 🎮 PosTech.Fiap.CloudGames — API

> **Tech Challenge · Fase 1 — FIAP POSTECH** · produto: **FIAP Cloud Games (FCG)**
> API REST em **.NET 8** para cadastro de usuários e biblioteca de jogos adquiridos, base para as próximas fases da plataforma (matchmaking e gestão de servidores).

A FCG é o MVP de uma plataforma de venda de jogos digitais. Esta primeira fase entrega um **monólito** organizado com **DDD / Clean Architecture**, persistência com **Entity Framework Core + PostgreSQL**, autenticação **JWT** com dois níveis de acesso, documentação **Swagger**, consultas de alta performance com **Dapper**, consultas dinâmicas via **GraphQL** e cobertura de testes **unitários + BDD**.

> 📚 **Projeto didático.** O enunciado do desafio está transcrito em **[docs/desafio.md](docs/desafio.md)**, cada requisito recebe um identificador (`RF-xx`/`RT-xx`) e a **[matriz de rastreabilidade](docs/requisitos-atendidos.md)** mostra onde cada um foi atendido. Esses mesmos identificadores aparecem em comentários `Requisito (Desafio Fase 1): ...` ao longo do código.

---

## 📋 Índice

- [Funcionalidades](#-funcionalidades)
- [Arquitetura](#-arquitetura)
- [Stack](#-stack)
- [Como executar](#-como-executar)
- [Endpoints](#-endpoints)
- [Autenticação e perfis](#-autenticação-e-perfis)
- [GraphQL](#-graphql)
- [Testes](#-testes)
- [Documentação de DDD](#-documentação-de-ddd)
- [Estrutura do projeto](#-estrutura-do-projeto)

---

## ✨ Funcionalidades

**Obrigatórias (desafio):**

- ✅ **Cadastro de usuários** com nome, e-mail e senha — validação de formato de e-mail e **senha segura** (mín. 8 caracteres, com letras, números e caracteres especiais).
- ✅ **Autenticação/autorização via JWT** com dois níveis: **Usuário** (plataforma + biblioteca) e **Administrador** (cadastro de jogos, administração de usuários e criação de promoções).
- ✅ Arquitetura **monolítica** (MVP).
- ✅ Persistência com **EF Core + migrations**.
- ✅ **Middleware** de tratamento de erros (ProblemDetails / RFC 7807) e **logs estruturados** (Serilog).
- ✅ Documentação **Swagger** com esquema Bearer JWT.
- ✅ **Testes unitários** das regras de negócio + **BDD** no módulo de autenticação.
- ✅ **DDD**: entidades, value objects e regras organizadas por domínio + Event Storming documentado.

**Opcionais implementados:**

- ✅ **Dapper** para consultas de alta performance (catálogo com preço promocional efetivo e biblioteca).
- ✅ **GraphQL** (HotChocolate) para consulta de jogos com filtragem e ordenação dinâmicas.
- ✅ **Domain Storytelling** ([docs/domain-storytelling](docs/domain-storytelling/README.md)).

---

## 🏛 Arquitetura

Monólito em camadas seguindo **Clean Architecture** e princípios de **DDD**. A regra de dependência aponta sempre para o domínio:

```
PosTech.Fiap.CloudGames.Api  ──►  PosTech.Fiap.CloudGames.Application  ──►  PosTech.Fiap.CloudGames.Domain
   │                    ▲                ▲
   └──►  PosTech.Fiap.CloudGames.Infrastructure  ───────────┘
        (implementa as abstrações do domínio/aplicação)
```

| Camada | Responsabilidade |
|---|---|
| **PosTech.Fiap.CloudGames.Domain** | Entidades, agregados, value objects, eventos, exceções e **interfaces de repositório**. Núcleo puro, sem dependências. |
| **PosTech.Fiap.CloudGames.Application** | Casos de uso (command/query services), DTOs, validações (FluentValidation) e interfaces de serviços (`IJwtTokenGenerator`, `IPasswordHasher`…). |
| **PosTech.Fiap.CloudGames.Infrastructure** | EF Core (`DbContext`, configurações, migrations, repositórios), Dapper, JWT, hashing BCrypt, seed. |
| **PosTech.Fiap.CloudGames.Api** | Minimal API (endpoints por feature), middleware, Swagger, GraphQL, autenticação e composição de DI. |

Detalhes em [docs/arquitetura.md](docs/arquitetura.md).

---

## 🧰 Stack

- **.NET 8** · Minimal API · C# 12
- **Entity Framework Core 8** + **Npgsql** (PostgreSQL) + migrations · convenção `snake_case`
- **Dapper** (consultas de leitura de alta performance)
- **JWT** (`Microsoft.AspNetCore.Authentication.JwtBearer`) + **BCrypt** (hash de senha)
- **FluentValidation** · **Serilog** (console + arquivo) · **Swashbuckle** (Swagger)
- **HotChocolate** (GraphQL)
- **xUnit** + **FluentAssertions** + **Moq** + **Bogus** + **coverlet** · **Reqnroll** (BDD)
- **Docker Compose** (PostgreSQL 16)

---

## 🚀 Como executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (para o PostgreSQL)

### 1. Suba o banco de dados

```bash
docker compose up -d cloudgames-postgres
```

> O PostgreSQL sobe na porta **5433** do host (mapeada para 5432 no container) para não conflitar com outros bancos locais.
> Opcional: `docker compose --profile tools up -d` também sobe o **pgAdmin** em http://localhost:5050.

### 2. Execute a API

```bash
dotnet run --project src/PosTech.Fiap.CloudGames.Api
```

As **migrations** e o **seed** (admin + jogos de exemplo) são aplicados automaticamente no startup (ambiente Development).

A API sobe em **http://localhost:5080** e o Swagger abre em **http://localhost:5080/swagger**.

### 3. Explore

- **Swagger UI:** http://localhost:5080/swagger
- **GraphQL (Banana Cake Pop):** http://localhost:5080/graphql
- **Health check:** http://localhost:5080/health

### Fluxo rápido no Swagger

1. `POST /api/v1/auth/login` com o admin do seed → copie o `accessToken`.
2. Clique em **Authorize** e cole o token.
3. `POST /api/v1/games` para cadastrar um jogo (perfil admin).
4. `POST /api/v1/auth/register` para criar um usuário comum, faça login com ele.
5. `POST /api/v1/library/{gameId}` para adquirir o jogo e `GET /api/v1/library` para vê-lo.

### Migrations (manual)

```bash
# ferramenta local já fixada no manifesto (.config/dotnet-tools.json)
dotnet tool restore
dotnet dotnet-ef database update --project src/PosTech.Fiap.CloudGames.Infrastructure --startup-project src/PosTech.Fiap.CloudGames.Api
```

---

## 🔌 Endpoints

Base: `http://localhost:5080`

### Autenticação
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `POST` | `/api/v1/auth/register` | Público | Cadastra um novo usuário |
| `POST` | `/api/v1/auth/login` | Público | Autentica e retorna o token JWT |

### Usuários
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `GET` | `/api/v1/users/me` | Autenticado | Perfil do usuário logado |
| `GET` | `/api/v1/users` | Admin | Lista usuários |
| `GET` | `/api/v1/users/{id}` | Admin | Detalha um usuário |
| `PUT` | `/api/v1/users/{id}` | Admin | Atualiza o nome |
| `PUT` | `/api/v1/users/{id}/role` | Admin | Altera o nível de acesso |
| `DELETE` | `/api/v1/users/{id}` | Admin | Desativa o usuário |

### Jogos
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `GET` | `/api/v1/games` | Público | Catálogo (filtros: `title`, `genre`, `minPrice`, `maxPrice`, `page`, `pageSize`) — preço promocional efetivo (Dapper) |
| `GET` | `/api/v1/games/{id}` | Público | Detalha um jogo |
| `POST` | `/api/v1/games` | Admin | Cadastra um jogo |
| `PUT` | `/api/v1/games/{id}` | Admin | Atualiza um jogo |
| `DELETE` | `/api/v1/games/{id}` | Admin | Desativa um jogo |

### Biblioteca
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `GET` | `/api/v1/library` | Autenticado | Biblioteca do usuário logado (Dapper) |
| `POST` | `/api/v1/library/{gameId}` | Autenticado | Adquire um jogo (aplica promoção ativa) |

### Promoções
| Método | Rota | Acesso | Descrição |
|---|---|---|---|
| `GET` | `/api/v1/promotions` | Admin | Lista promoções |
| `POST` | `/api/v1/promotions` | Admin | Cria uma promoção de desconto |
| `DELETE` | `/api/v1/promotions/{id}` | Admin | Desativa uma promoção |

### Outros
| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/graphql` | Consulta de jogos via GraphQL |
| `GET` | `/health` | Health check |

---

## 🔐 Autenticação e perfis

Autenticação via **JWT Bearer**. O token carrega os claims `sub` (id), `email`, `name` e `role`.

**Dois níveis de acesso:**

| Perfil | Permissões |
|---|---|
| **Usuário** (`User`) | Acessa a plataforma, o catálogo e a própria biblioteca |
| **Administrador** (`Administrator`) | Cadastra jogos, administra usuários e cria promoções |

**Credenciais do administrador (seed):**

```
e-mail: admin@cloudgames.com
senha:  Admin@123
```

Configuração do JWT em [`appsettings.json`](src/PosTech.Fiap.CloudGames.Api/appsettings.json) (`Jwt:SecretKey`, `Issuer`, `Audience`, `ExpirationMinutes`).

---

## 🔎 GraphQL

Endpoint `POST /graphql`. Consulta o catálogo de jogos ativos com **filtragem e ordenação dinâmicas** (HotChocolate).

```graphql
{
  games(where: { genre: { eq: "RPG" } }, order: [{ price: ASC }]) {
    title
    genre
    price
    releaseDate
  }
}
```

---

## 🧪 Testes

```bash
dotnet test
```

| Projeto | Tipo | Cobertura |
|---|---|---|
| `PosTech.Fiap.CloudGames.Domain.Tests` | Unitário (TDD) | Value objects (Email, senha, Money) e regras de entidade (biblioteca sem duplicatas, faixa de desconto…) |
| `PosTech.Fiap.CloudGames.Application.Tests` | Unitário (Moq) | Casos de uso: cadastro, login, criação de jogo, aquisição com/sem promoção |
| `PosTech.Fiap.CloudGames.Bdd.Tests` | **BDD** (Reqnroll) | Módulo de autenticação — cadastro + login em cenários Gherkin |

> **62 testes** no total. O módulo de autenticação é validado também por **BDD** (arquivo [`Autenticacao.feature`](test/PosTech.Fiap.CloudGames.Bdd.Tests/Features/Autenticacao.feature)).

---

## 📐 Documentação

- **[O desafio](docs/desafio.md)** — enunciado transcrito do PDF, com os requisitos identificados (`RF-xx`/`RT-xx`).
- **[Requisitos atendidos](docs/requisitos-atendidos.md)** — matriz de rastreabilidade requisito → código.
- **[Event Storming](docs/event-storming/README.md)** — fluxos de **criação de usuários** e **criação de jogos** (comandos, eventos, agregados, políticas e read models).
- **[Domain Storytelling](docs/domain-storytelling/README.md)** — cenário "usuário adquire um jogo".
- **[Arquitetura](docs/arquitetura.md)** — contexto delimitado, agregados e camadas.

---

## 📁 Estrutura do projeto

```
postech-fiap-cloudgames/
├── src/
│   ├── PosTech.Fiap.CloudGames.Domain/          # entidades, VOs, eventos, interfaces de repositório
│   ├── PosTech.Fiap.CloudGames.Application/     # casos de uso, DTOs, validações, abstrações
│   ├── PosTech.Fiap.CloudGames.Infrastructure/  # EF Core, Dapper, JWT, BCrypt, seed, migrations
│   └── PosTech.Fiap.CloudGames.Api/             # Minimal API, middleware, Swagger, GraphQL
├── test/
│   ├── PosTech.Fiap.CloudGames.Domain.Tests/
│   ├── PosTech.Fiap.CloudGames.Application.Tests/
│   └── PosTech.Fiap.CloudGames.Bdd.Tests/       # Reqnroll (BDD)
├── docs/                    # DDD (Event Storming, Domain Storytelling, arquitetura)
├── docker-compose.yml       # PostgreSQL + pgAdmin
└── Fcg.sln
```

---

## 👥 Entrega

Consulte o modelo de relatório em [docs/relatorio-entrega.md](docs/relatorio-entrega.md).
