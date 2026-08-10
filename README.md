# Gestão de Reembolsos Corporativos

Sistema interno para empresas controlarem solicitações de reembolso de despesas corporativas, comprovantes, aprovação gerencial, validação financeira e pagamento.

## 1. Descrição do projeto

Um colaborador registra despesas pagas com recursos próprios, como transporte, hospedagem, alimentação ou material de escritório. Cada despesa pertence a uma solicitação de reembolso.

```text
Solicitação → aprovação do gestor direto → validação financeira → pagamento
```

| Perfil | Função |
| --- | --- |
| Employee | Cria e acompanha suas solicitações. |
| Manager | Aprova, rejeita ou devolve solicitações de subordinados. |
| Finance | Valida, devolve, rejeita e registra pagamentos. |
| Admin | Administra usuários, departamentos e categorias de despesa. |

## 2. Entidades e modelos de dados

### Department

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| Name | `string(100)` | Obrigatório e único |
| CostCenterCode | `string(20)` | Obrigatório e único |
| IsActive | `bool` | Padrão: `true` |
| CreatedAtUtc | `DateTime` | Obrigatório |

Um departamento possui vários usuários.

### User

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| FullName | `string(150)` | Obrigatório |
| Email | `string(150)` | Obrigatório, único e formato válido |
| PasswordHash | `string` | Obrigatório |
| Role | `Role` | Employee, Manager, Finance ou Admin |
| DepartmentId | `Guid` | Obrigatório |
| ManagerId | `Guid?` | Gestor direto; obrigatório para Employee |
| IsActive | `bool` | Padrão: `true` |
| CreatedAtUtc | `DateTime` | Obrigatório |
| UpdatedAtUtc | `DateTime?` | Opcional |

Um usuário pertence a um departamento, pode ter gestor e pode possuir subordinados.

### ExpenseCategory

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| Name | `string(80)` | Obrigatório e único |
| MonthlyLimit | `decimal(12,2)?` | Limite mensal opcional por colaborador |
| RequiresReceipt | `bool` | Define se comprovante é obrigatório |
| IsActive | `bool` | Padrão: `true` |

Categorias iniciais: Alimentação, Transporte, Hospedagem, Material de escritório, Quilometragem e Outros.

### ReimbursementRequest

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| RequestNumber | `string(20)` | Único e gerado pelo sistema |
| EmployeeId / DepartmentId | `Guid` | Obrigatórios |
| ReferenceMonth | `DateOnly` | Primeiro dia do mês de referência |
| Status | `RequestStatus` | Ver fluxo abaixo |
| TotalAmount | `decimal(12,2)` | Calculado pelo sistema |
| SubmittedAtUtc / ManagerDecisionAtUtc / FinanceDecisionAtUtc / PaidAtUtc | `DateTime?` | Datas das etapas |
| CreatedAtUtc / UpdatedAtUtc | `DateTime` / `DateTime?` | Auditoria |
| Version | `int` | Concorrência otimista |

Status: `Draft`, `PendingManagerApproval`, `RejectedByManager`, `ReturnedByManager`, `PendingFinanceValidation`, `RejectedByFinance`, `ReturnedByFinance`, `ApprovedForPayment`, `Paid` e `Cancelled`.

### ExpenseItem

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| ReimbursementRequestId / ExpenseCategoryId | `Guid` | Obrigatórios |
| ExpenseDate | `DateOnly` | Obrigatório |
| Description | `string(300)` | Obrigatório |
| Amount | `decimal(12,2)` | Maior que zero |
| MerchantName | `string(150)` | Obrigatório |
| ReceiptFileName / ReceiptStorageKey | `string?` | Conforme a categoria |
| CreatedAtUtc / UpdatedAtUtc | `DateTime` / `DateTime?` | Auditoria |

### ApprovalDecision, Payment e histórico

- `ApprovalDecision`: registra pedido, decisor, nível (Manager/Finance), decisão (Approved/Rejected/Returned), comentário e data.
- `Payment`: possui solicitação única, referência única, valor pago, data, processador, observação e data de criação.
- `RequestStatusHistory`: registra status anterior, novo status, usuário, motivo e data; o anterior é nulo somente na criação.

## 3. Regras de negócio explícitas

### Criação e edição

- Apenas o colaborador cria solicitações para si; toda solicitação começa em `Draft`.
- É necessário pelo menos um item antes do envio.
- As despesas pertencem ao mês de referência, não são futuras e não podem ser anteriores a 90 dias da criação.
- Item: valor entre 0,01 e 50.000,00; descrição entre 10 e 300 caracteres; estabelecimento obrigatório.
- Categoria que exige comprovante não pode ser salva sem arquivo PDF, JPG ou PNG de até 5 MB.
- Apenas o dono edita solicitações `Draft`, `ReturnedByManager` ou `ReturnedByFinance`.
- O total é sempre a soma dos itens ativos; nunca informado manualmente.
- Não se exclui o último item de pedido já enviado: o pedido deve ser cancelado.

### Envio, decisão e pagamento

- Enviar exige itens válidos, comprovantes exigidos, total maior que zero e gestor direto ativo; muda para `PendingManagerApproval` e cria histórico.
- Gestor decide apenas pedidos pendentes de seus subordinados diretos; não decide o próprio pedido.
- Gestor: aprovar → `PendingFinanceValidation`; rejeitar → `RejectedByManager`; devolver → `ReturnedByManager`.
- Finance/Admin atuam somente em `PendingFinanceValidation`: aprovar → `ApprovedForPayment`; rejeitar → `RejectedByFinance`; devolver → `ReturnedByFinance`.
- Rejeição e devolução exigem comentário de 10 a 500 caracteres. Toda decisão registra `ApprovalDecision` e histórico.
- Finance/Admin registram pagamento somente em `ApprovedForPayment`; valor deve ser igual ao total, data não pode ser futura e referência é única. O pagamento muda o status para `Paid`.
- Pedido pago é imutável. Cancelamento é exclusivo do proprietário em `Draft` ou status devolvidos, com motivo de 10 a 500 caracteres.

### Limite mensal por categoria

Se existir `MonthlyLimit`, bloquear o envio caso o total do colaborador, categoria e mês ultrapasse o limite. Considerar: `PendingManagerApproval`, `PendingFinanceValidation`, `ApprovedForPayment` e `Paid`. Não considerar rascunhos, devolvidas, rejeitadas ou canceladas.

## 4. Casos de uso principais

| Caso | Descrição |
| --- | --- |
| UC01 — Registrar solicitação | Colaborador cria rascunho, inclui despesas e sistema recalcula total. |
| UC02 — Enviar para aprovação | Sistema valida regras e limite, cria histórico e envia ao gestor. |
| UC03 — Decisão do gestor | Gestor consulta pendências e aprova, rejeita ou devolve. |
| UC04 — Validação financeira | Finance confere itens, comprovantes e total antes de decidir. |
| UC05 — Registrar pagamento | Finance informa referência, data e valor; sistema paga e registra histórico. |

## 5. Endpoints e funcionalidades

API REST sob o prefixo `/api`.

| Controller | Rotas | Acesso |
| --- | --- | --- |
| Auth | `POST /auth/login`, `POST /auth/refresh`, `GET /auth/me` | Público / autenticado |
| ReimbursementRequests | `POST/GET /reimbursement-requests`, `GET/PUT /reimbursement-requests/{id}`, `/submit`, `/cancel`, `/history` | Employee / proprietário / autorizado |
| ExpenseItems | `/reimbursement-requests/{requestId}/expenses` e `/{expenseId}/receipt` | Proprietário / autorizado |
| ManagerApprovals | `/manager/approvals/pending`, `/{requestId}/approve`, `/reject`, `/return` | Manager |
| Finance | `/finance/requests/pending`, `/{requestId}/approve`, `/reject`, `/return`, `/payment` | Finance/Admin |
| Administration | `/admin/departments`, `/admin/users`, `/admin/expense-categories` | Admin |

Listagens aceitam `status`, `referenceMonth`, `page`, `pageSize`, `sortBy` e `sortDirection`. Finance/Admin também filtram `employeeId` e `departmentId`.

## 6. Fluxo entre camadas

```text
Cliente HTTP → Controller → Request DTO + validação → Application Service
→ autorização e regras → Repositories / Unit of Work → EF Core → SQL Server
→ Response DTO → Cliente HTTP
```

- **Controller**: HTTP, usuário autenticado e código adequado.
- **DTO**: contrato de entrada/saída; não expor entidades diretamente.
- **Service**: negócio, autorização contextual, status e transações.
- **Repository**: consultas e persistência, sem regra de negócio.
- **Unit of Work**: `SaveChangesAsync` e transações.

## 7. Requisitos técnicos

- ASP.NET Core Web API, .NET LTS, EF Core e SQL Server.
- JWT Bearer, FluentValidation, Serilog, Swagger/OpenAPI, xUnit, Moq e FluentAssertions.
- JWT contém Id, e-mail, perfil e departamento; access token: 60 minutos; refresh: 7 dias.
- Todas as rotas, exceto login, exigem autenticação; roles e contexto são validados no serviço.
- Usar `ProblemDetails`: 400, 401, 403, 404, 409 e 500; sem detalhes internos em produção.
- Registrar login, alterações, decisões, pagamentos, erros e acessos negados; nunca senha, token ou comprovante.
- Usar concorrência otimista em `ReimbursementRequest`; conflito retorna 409.

## 8. Repositories

Interfaces: `IUserRepository`, `IDepartmentRepository`, `IExpenseCategoryRepository`, `IReimbursementRequestRepository`, `IExpenseItemRepository`, `IApprovalDecisionRepository`, `IPaymentRepository` e `IUnitOfWork`.

Consultas suportam paginação e usam `AsNoTracking` quando apropriado. A consulta detalhada carrega colaborador, departamento, despesas, categorias, decisões, pagamento e histórico. Alterações de status ocorrem em transação.

## 9. Dados de teste

| Departamento | Centro de custo |
| --- | --- |
| Tecnologia | TI-100 |
| Comercial | COM-200 |
| Financeiro | FIN-300 |

| Nome | E-mail | Perfil | Departamento | Gestor |
| --- | --- | --- | --- | --- |
| Ana Souza | ana.souza@empresa.test | Employee | Tecnologia | Carlos Lima |
| Bruno Alves | bruno.alves@empresa.test | Employee | Comercial | Marina Costa |
| Carlos Lima | carlos.lima@empresa.test | Manager | Tecnologia | — |
| Marina Costa | marina.costa@empresa.test | Manager | Comercial | — |
| Fernanda Rocha | fernanda.rocha@empresa.test | Finance | Financeiro | — |
| Admin Sistema | admin@empresa.test | Admin | Financeiro | — |

Senha de desenvolvimento: `SenhaTeste@123`.

| Categoria | Limite mensal | Comprovante obrigatório |
| --- | ---: | --- |
| Alimentação | 800,00 | Sim |
| Transporte | 600,00 | Sim |
| Hospedagem | 3.000,00 | Sim |
| Material de escritório | 500,00 | Sim |
| Quilometragem | 1.000,00 | Não |
| Outros | 300,00 | Sim |

Exemplo de solicitação: `RR-2026-000001`, Ana Souza, agosto/2026, `PendingManagerApproval`, total **303,40**.

## 10. Estrutura Git recomendada

```text
src/
  Reimbursement.Api/            Controllers, Middleware, Filters e Configuration
  Reimbursement.Application/    DTOs, Interfaces, Services, Validators e Mappings
  Reimbursement.Domain/         Entities, Enums, Exceptions e Rules
  Reimbursement.Infrastructure/ Persistence, Repositories, Authentication, Storage e Logging
  Reimbursement.Tests/          Unit e Integration
docs/                           Regras de negócio, API, banco e decisões
```

Branches: `main`, `develop`, `feature/authentication`, `feature/request-management`, `feature/expense-items`, `feature/manager-approval`, `feature/finance-validation`, `feature/payment`, `feature/admin-management` e `feature/tests`.

Cada feature abre pull request para `develop` com descrição, regras atendidas, endpoints, testes e evidência de execução.
