# 📄 O Desafio — Tech Challenge · Fase 1 (FIAP POSTECH)

> Transcrição do enunciado fornecido (`TC NETT - Fase 1.pdf`), mantida no repositório para fins
> **didáticos e de rastreabilidade**. Cada requisito abaixo tem um identificador (ex.: `RF-01`, `RT-03`)
> referenciado nos comentários do código-fonte e na [matriz de requisitos atendidos](requisitos-atendidos.md).

## Contexto

> A **FIAP Cloud Games (FCG)** será uma plataforma de venda de jogos digitais e gestão de servidores para
> partidas online. Nesta primeira fase, você desenvolverá um serviço de **cadastro de usuários** e
> **biblioteca de jogos adquiridos** que servirá de base para as próximas fases do projeto.

O objetivo desta fase é criar uma **API REST em .NET 8** para gerenciar usuários e seus jogos adquiridos,
garantindo **persistência de dados**, **qualidade de software** e **boas práticas de desenvolvimento**,
preparando a base para funcionalidades futuras como *matchmaking* e gerenciamento de servidores.

---

## Funcionalidades obrigatórias

### Cadastro de usuários
- **RF-01** — Identificação do cliente por **nome, e-mail e senha**.
- **RF-02** — Validar **formato de e-mail** e **senha segura** (mínimo de 8 caracteres com números, letras e caracteres especiais).

### Autenticação e Autorização
- **RF-03** — Implementar autenticação via **token JWT**.
- **RF-04** — Ter **dois níveis de acesso**:
  - **Usuário** — acesso à plataforma e à biblioteca de jogos.
  - **Administrador** — pode cadastrar jogos, administrar usuários e criar promoções.

### Arquitetura
- **RF-05** — Como se trata de um MVP, utilizar um **monólito** para facilitar o desenvolvimento ágil.

---

## Requisitos técnicos

### Persistência de Dados (Entity Framework Core / MongoDB)
- **RT-01** — Utilizar **Entity Framework Core** para gerenciar os modelos de usuários e jogos.
- **RT-02** — Aplicar **migrations** para a criação do banco de dados.
- **RT-03** — *(Opcional)* Utilizar **MongoDB** para persistência dos dados.
- **RT-04** — *(Opcional)* Utilizar **Dapper** para consultas de alta performance, caso necessário.

### Desenvolvimento de API com .NET 8
- **RT-05** — Criar a API seguindo o padrão **Minimal API** ou **Controllers MVC**.
- **RT-06** — Implementar **Middleware** para tratamento de erros e **logs estruturados**.
- **RT-07** — Adicionar documentação com **Swagger** para expor os endpoints da API.
- **RT-08** — *(Opcional)* Utilizar **GraphQL** para consulta avançada de jogos, permitindo filtragens dinâmicas.

### Qualidade de Software
- **RT-09** — Criar **testes unitários** para validar as principais regras de negócio.
- **RT-10** — Aplicar **TDD** ou **BDD** em pelo menos um dos módulos do projeto.

### Domain-Driven Design (DDD)
- **RT-11** — Modelar o domínio utilizando **Event Storming** para mapear os fluxos de usuários e jogos.
- **RT-12** — *(Opcional)* Aplicar **Domain Storytelling** para representar cenários de interação com a API.
- **RT-13** — Seguir os princípios de **DDD** na organização das entidades e regras de negócio.

---

## Entregáveis da Fase 1

- **Vídeo de até 15 minutos** demonstrando todos os requisitos (pode rodar localmente).
- **Documentação DDD** (Miro ou equivalente):
  - Event Storming dos fluxos: **criação de jogos** e **criação de usuários**.
  - Diagramas conforme apresentado na disciplina de DDD.
- **Código-fonte no repositório** (público ou privado), incluindo:
  - APIs conforme requisitos;
  - Testes escritos;
  - **README.md** completo com instruções de uso e objetivos.
- **Relatório de entrega** (PDF ou TXT) contendo: nome do grupo; participantes e usernames no Discord;
  link da documentação; link do(s) repositório(s); link do vídeo.

> **Observação sobre opcionais:** requisitos marcados como *(Opcional)* não descontam pontos se ausentes.
> Neste projeto foram implementados **Dapper (RT-04)**, **GraphQL (RT-08)** e **Domain Storytelling (RT-12)**.
> O **MongoDB (RT-03)** não foi usado — a persistência principal é **PostgreSQL** com EF Core.

---

👉 Veja **onde cada requisito foi atendido** em [requisitos-atendidos.md](requisitos-atendidos.md).
