# Desafio Técnico - API de Reservas de Salas de Reunião

Este projeto é uma implementação de uma API de Reservas de Salas de Reunião em .NET 8, seguindo os princípios de **Clean Architecture** e utilizando o padrão **CQRS (Command Query Responsibility Segregation)** com a biblioteca **MediatR**.

O objetivo principal é demonstrar a organização do código, a aplicação de padrões de arquitetura e a implementação de uma regra de negócio crucial: a **validação de conflito de horários**.

## 1. Arquitetura e Organização do Código (Clean Architecture)

A **Clean Architecture** (Arquitetura Limpa) é um princípio de design de software que visa a separação de preocupações em camadas concêntricas, onde as dependências fluem apenas de fora para dentro. Isso garante que as regras de negócio centrais (o *Domain*) sejam independentes de frameworks, bancos de dados ou interfaces de usuário.

### Estrutura de Camadas

| Camada | Descrição | Dependências |
| :--- | :--- | :--- |
| `ReservaSalas.Domain` | Contém as **regras de negócio** e as **entidades** centrais. É o coração da aplicação. | Nenhuma (independente) |
| `ReservaSalas.Application` | Contém a **lógica de aplicação** e a orquestração das regras de negócio. Implementa o CQRS. | `Domain` |
| `ReservaSalas.Infrastructure` | Contém a **implementação de detalhes externos**, como acesso a dados (EF Core) e serviços de terceiros. | `Domain`, `Application` |
| `ReservaSalas.Api` | É a **interface de apresentação** (Web API). Responsável por receber requisições HTTP e enviar comandos/queries. | `Application`, `Infrastructure` |

**Ponto de Entrevista:** A principal vantagem da Clean Architecture é a **testabilidade** e a **manutenibilidade**. Ao isolar o Domain e o Application, podemos testar a lógica de negócio sem a necessidade de um banco de dados real ou de um servidor web.

## 2. CQRS (Command Query Responsibility Segregation) com MediatR

O CQRS é um padrão que separa o modelo de dados e as operações de **leitura (Queries)** das operações de **escrita (Commands)**.

### Implementação com MediatR

A biblioteca **MediatR** facilita a implementação do CQRS e do padrão Mediator.

*   **Commands (Escrita):** São classes que representam uma intenção de **modificar** o estado da aplicação (ex: `CreateReservaCommand`, `UpdateReservaCommand`).
*   **Queries (Leitura):** São classes que representam uma intenção de **consultar** o estado da aplicação (ex: `GetReservaByIdQuery`, `GetReservasByRoomQuery`).
*   **Handlers:** São classes que implementam a interface `IRequestHandler<TRequest, TResponse>` e contêm a lógica para processar um Command ou Query específico.

**Ponto de Entrevista:**
*   **Vantagem do CQRS:** Permite otimizar as operações de leitura e escrita de forma independente. Por exemplo, as Queries podem usar um modelo de dados mais simples (DTOs) e ser otimizadas para leitura (ex: `AsNoTracking()` do EF Core), enquanto os Commands se concentram na consistência transacional.
*   **MediatR:** Atua como um **despachante** (Mediator), desacoplando o emissor (Controller) do receptor (Handler). O Controller apenas envia o Command/Query, sem saber quem o processará.

## 3. Logging como Cross-Cutting Concern (Pipeline Behavior)

O requisito de registrar logs para todas as ações foi implementado usando o recurso **Pipeline Behavior** do MediatR.

*   **Pipeline Behavior:** É um interceptor que permite executar lógica **antes** e **depois** do processamento de qualquer Command ou Query. É ideal para implementar *cross-cutting concerns* (preocupações transversais) como Logging, Validação, Autorização e Cache.
*   **Implementação:** A classe `LoggingBehavior<TRequest, TResponse>` intercepta todas as requisições, registra a data/hora, o nome da requisição e os dados de entrada/saída (serializados em JSON), conforme solicitado.

**Ponto de Entrevista:** O uso de Pipeline Behavior demonstra um entendimento de como aplicar o **Princípio da Responsabilidade Única (SRP)** e o **Princípio Aberto/Fechado (OCP)**. O Handler se concentra apenas na lógica de negócio, enquanto o Logging é injetado de forma não intrusiva.

## 4. Regra de Negócio: Conflito de Horários

A regra de negócio central é garantir que não haja sobreposição de horários para a mesma sala.

*   **Lógica de Validação:** A validação é realizada no serviço `ReservaValidator`, que verifica se existe alguma reserva no banco de dados para a mesma `Room` onde:
    *   `reservaExistente.StartDate < novaReserva.EndDate`
    *   **E** `reservaExistente.EndDate > novaReserva.StartDate`
*   **Exceção Customizada:** Em caso de conflito, é lançada uma exceção customizada (`ReservaConflictException`), que é capturada no Controller e retorna o `Status Code 409 Conflict` (padrão HTTP para conflito de recurso).

## 5. Entity Framework Core (EF Core)

O EF Core é o ORM utilizado para o acesso a dados.

*   **`ApplicationDbContext`:** Implementa a interface `IApplicationDbContext` (definida na camada Application), permitindo que a camada Application dependa de uma abstração, e não da implementação concreta do EF Core.
*   **Migrations:** Utilizadas para gerenciar o esquema do banco de dados (SQLite, neste caso).
*   **`AsNoTracking()`:** Utilizado nas Queries para otimizar a leitura, informando ao EF Core que os objetos retornados não precisam ser rastreados para alterações.

## 6. Testes Automatizados

Foi implementado um teste de integração (`ReservaConflictTests`) usando o **InMemory Database** do EF Core para validar a regra de negócio de conflito de horários.

*   **Foco:** O teste valida o comportamento do `ReservaValidator` em cenários de conflito, não-conflito e atualização (excluindo a própria reserva da validação).
*   **Vantagem do InMemory:** Permite testar a lógica de acesso a dados e a regra de negócio de forma rápida e isolada, sem depender de um banco de dados externo.

## Instruções para Rodar o Projeto

### Pré-requisitos

*   .NET 8 SDK
*   Git

### Setup

1.  **Clonar o repositório:**
    \`\`\`bash
    git clone [LINK_DO_REPOSITORIO]
    cd ReservaSalasAPI
    \`\`\`

2.  **Restaurar dependências e compilar:**
    \`\`\`bash
    dotnet restore
    dotnet build
    \`\`\`

3.  **Executar os testes:**
    \`\`\`bash
    dotnet test tests/ReservaSalas.Tests/ReservaSalas.Tests.csproj
    \`\`\`

4.  **Executar a API:**
    \`\`\`bash
    dotnet run --project src/ReservaSalas.Api/ReservaSalas.Api.csproj
    \`\`\`

A API estará disponível em `https://localhost:7000` (ou porta similar). O Swagger UI estará acessível em `/swagger`.

## Justificativa das Escolhas de Arquitetura

| Escolha | Justificativa para a Entrevista |
| :--- | :--- |
| **Clean Architecture** | **Separação de Preocupações** e **Testabilidade**. Garante que a lógica de negócio seja o centro, independente de tecnologia de banco de dados ou framework web. |
| **CQRS com MediatR** | **Desacoplamento** e **Manutenibilidade**. Separa as responsabilidades de leitura e escrita, facilitando a evolução e otimização de cada fluxo de forma isolada. O MediatR simplifica a comunicação entre as camadas. |
| **Pipeline Behavior** | Implementação elegante de **Cross-Cutting Concerns** (como Logging), seguindo o OCP. Mantém os Handlers limpos e focados na lógica de negócio. |
| **EF Core + SQLite** | **Produtividade** e **Simplicidade**. O EF Core é o padrão de mercado para .NET, e o SQLite é ideal para o ambiente de desafio técnico, sendo um banco de dados leve e sem necessidade de setup externo. |
| **Testes com InMemory DB** | **Confiabilidade** e **Velocidade**. Garante que a regra de negócio mais crítica (conflito) funcione corretamente, usando um ambiente de teste rápido e isolado. |

---
## Referências

[1] Clean Architecture. Robert C. Martin (Uncle Bob).
[2] CQRS (Command Query Responsibility Segregation). Martin Fowler.
[3] MediatR. Jimmy Bogard.
[4] Entity Framework Core. Microsoft Documentation.
[5] HTTP Status Code 409 Conflict. Mozilla Developer Network.
