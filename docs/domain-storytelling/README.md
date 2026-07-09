# Domain Storytelling — "Usuário adquire um jogo"

Narrativa de um cenário de interação com a plataforma, no estilo **Domain Storytelling**
(atores, objetos de trabalho e atividades numeradas na ordem em que acontecem).

## História

> **Cenário:** a jogadora *Alice* já cadastrada quer adquirir o jogo *Elden Ring*, que está em promoção.

```mermaid
flowchart LR
    alice([👤 Alice<br/>Usuária]):::actor
    api[[Plataforma FCG]]:::system
    catalogo[(Catálogo)]:::obj
    biblioteca[(Biblioteca de Alice)]:::obj
    promo[(Promoção ativa)]:::obj

    alice -- "1 - autentica-se e recebe token JWT" --> api
    api -- "2 - lista jogos e preços efetivos" --> catalogo
    alice -- "3 - escolhe 'Elden Ring' e solicita a compra" --> api
    api -- "4 - verifica se já não possui o jogo" --> biblioteca
    api -- "5 - consulta desconto vigente" --> promo
    api -- "6 - registra o jogo com o preço promocional" --> biblioteca
    api -- "7 - confirma a aquisição" --> alice

    classDef actor fill:#FFF59D,stroke:#F9A825,color:#000
    classDef system fill:#1E88E5,stroke:#1565C0,color:#fff
    classDef obj fill:#43A047,stroke:#2E7D32,color:#fff
```

## Passo a passo

1. **Alice se autentica** (`POST /api/v1/auth/login`) e recebe um **token JWT**.
2. A plataforma apresenta o **catálogo** com o **preço efetivo** de cada jogo (consulta Dapper que já aplica a promoção vigente).
3. Alice **solicita a compra** de *Elden Ring* (`POST /api/v1/library/{gameId}`).
4. A plataforma verifica que Alice **ainda não possui** o jogo (invariante do agregado `User`).
5. Consulta se há **promoção ativa** para o jogo no momento.
6. **Registra o jogo na biblioteca** de Alice com o **preço já descontado** (evento `GameAcquired`).
7. Retorna a **confirmação** da aquisição, com o valor pago e o desconto aplicado.

## Regras evidenciadas

- Aquisição exige **usuário autenticado**.
- **Não é possível adquirir o mesmo jogo duas vezes** → retorna `409 Conflict`.
- O **preço pago** reflete a promoção ativa no instante da compra.
