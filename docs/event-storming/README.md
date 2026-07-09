# Event Storming — FIAP Cloud Games (Fase 1)

Mapeamento dos fluxos de negócio no estilo **Event Storming**. Os diagramas abaixo estão em Mermaid
(renderizam no GitHub) e podem ser reproduzidos no Miro usando a mesma legenda de cores.

## 🎨 Legenda

| Cor | Elemento | Significado |
|---|---|---|
| 🟦 Azul | **Comando** | Intenção/ação disparada por um ator |
| 🟧 Laranja | **Evento de Domínio** | Fato relevante que ocorreu (no passado) |
| 🟨 Amarelo | **Agregado** | Consistência transacional; recebe o comando e emite o evento |
| 🟪 Lilás | **Política / Regra** | Reação automática ou invariante ("sempre que…") |
| 🟩 Verde | **Read Model** | Visão de leitura consumida pela UI/consulta |
| 🟡 Creme | **Ator** | Pessoa/papel que dispara o comando |

---

## 1. Criação de usuários

> Um visitante se cadastra na plataforma. A senha precisa respeitar a política de segurança e o e-mail deve ser único.

```mermaid
flowchart LR
    actor([👤 Visitante]):::actor
    cmd[Cadastrar Usuário]:::command
    pol1{{Política de Senha Forte}}:::policy
    pol2{{E-mail único}}:::policy
    agg[/User/]:::aggregate
    evt[Usuário Cadastrado]:::event
    rm[(Perfil do Usuário)]:::readmodel

    actor --> cmd
    pol1 -.valida.-> cmd
    pol2 -.valida.-> cmd
    cmd --> agg
    agg --> evt
    evt --> rm

    classDef actor fill:#FFF59D,stroke:#F9A825,color:#000
    classDef command fill:#1E88E5,stroke:#1565C0,color:#fff
    classDef aggregate fill:#FDD835,stroke:#F9A825,color:#000
    classDef event fill:#FB8C00,stroke:#EF6C00,color:#fff
    classDef readmodel fill:#43A047,stroke:#2E7D32,color:#fff
    classDef policy fill:#8E24AA,stroke:#6A1B9A,color:#fff
```

**Regras (invariantes):**
- E-mail com formato válido e **único** na base.
- Senha com **mínimo de 8 caracteres**, contendo letras, números e caracteres especiais (Value Object `Password`).
- A senha nunca é persistida em texto puro — apenas o **hash BCrypt**.
- Todo usuário é criado com o papel **Usuário** e ativo.

**Rastreabilidade no código:** `POST /api/v1/auth/register` → `UserCommandService.RegisterAsync` → `new User(...)` → evento `UserRegistered`.

---

## 2. Criação de jogos

> Um administrador cadastra um jogo no catálogo.

```mermaid
flowchart LR
    actor([🛡️ Administrador]):::actor
    cmd[Cadastrar Jogo]:::command
    pol1{{Somente Administrador}}:::policy
    pol2{{Título único · preço ≥ 0}}:::policy
    agg[/Game/]:::aggregate
    evt[Jogo Cadastrado]:::event
    rm[(Catálogo de Jogos)]:::readmodel

    actor --> cmd
    pol1 -.autoriza.-> cmd
    pol2 -.valida.-> cmd
    cmd --> agg
    agg --> evt
    evt --> rm

    classDef actor fill:#FFF59D,stroke:#F9A825,color:#000
    classDef command fill:#1E88E5,stroke:#1565C0,color:#fff
    classDef aggregate fill:#FDD835,stroke:#F9A825,color:#000
    classDef event fill:#FB8C00,stroke:#EF6C00,color:#fff
    classDef readmodel fill:#43A047,stroke:#2E7D32,color:#fff
    classDef policy fill:#8E24AA,stroke:#6A1B9A,color:#fff
```

**Regras (invariantes):**
- Apenas o papel **Administrador** pode cadastrar jogos (autorização JWT).
- Título e gênero são obrigatórios; **título único**; preço não-negativo (`Money`).

**Rastreabilidade no código:** `POST /api/v1/games` (policy `Admin`) → `GameCommandService.CreateAsync` → `new Game(...)` → evento `GameCreated`.

---

## 3. Aquisição de um jogo (bônus)

> Um usuário adquire um jogo; se houver promoção ativa, o desconto é aplicado ao preço pago.

```mermaid
flowchart LR
    actor([👤 Usuário]):::actor
    cmd[Adquirir Jogo]:::command
    pol1{{Sem duplicata na biblioteca}}:::policy
    pol2{{Aplica promoção ativa}}:::policy
    agg[/User · Biblioteca/]:::aggregate
    evt[Jogo Adquirido]:::event
    rm[(Biblioteca do Usuário)]:::readmodel

    actor --> cmd
    pol1 -.valida.-> cmd
    pol2 -.calcula preço.-> cmd
    cmd --> agg
    agg --> evt
    evt --> rm

    classDef actor fill:#FFF59D,stroke:#F9A825,color:#000
    classDef command fill:#1E88E5,stroke:#1565C0,color:#fff
    classDef aggregate fill:#FDD835,stroke:#F9A825,color:#000
    classDef event fill:#FB8C00,stroke:#EF6C00,color:#fff
    classDef readmodel fill:#43A047,stroke:#2E7D32,color:#fff
    classDef policy fill:#8E24AA,stroke:#6A1B9A,color:#fff
```

**Regras (invariantes):**
- O usuário **não pode possuir o mesmo jogo duas vezes** (invariante do agregado `User`).
- Se houver promoção ativa para o jogo no momento, o **preço pago recebe o desconto**.

**Rastreabilidade no código:** `POST /api/v1/library/{gameId}` → `LibraryService.AcquireAsync` → `User.AcquireGame(...)` → evento `GameAcquired`.
