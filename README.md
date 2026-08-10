# Gestão de Reembolsos Corporativos

Sistema interno para empresas controlarem solicitações de reembolso de despesas feitas por colaboradores, incluindo comprovantes, aprovação gerencial, validação financeira e pagamento.

## 1. Descrição do projeto

O colaborador registra despesas corporativas pagas com recursos próprios, como transporte, hospedagem, alimentação e material de escritório. As despesas pertencem a uma solicitação de reembolso, que percorre estas etapas:

```text
Criação → aprovação do gestor direto → validação financeira → pagamento
```

| Perfil | Responsabilidade |
| --- | --- |
| Employee | Cria e acompanha as próprias solicitações. |
| Manager | Aprova, rejeita ou devolve solicitações dos subordinados. |
| Finance | Valida, devolve, rejeita e registra pagamentos. |
| Admin | Administra usuários, departamentos e categorias. |

## 2. Entidades e modelo de dados

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
| Email | `string(150)` | Obrigatório, único e válido |
| PasswordHash | `string` | Obrigatório |
| Role | `Role` | Employee, Manager, Finance ou Admin |
| DepartmentId | `Guid` | Obrigatório |
| ManagerId | `Guid?` | Gestor direto; obrigatório para Employee |
| IsActive | `bool` | Padrão: `true` |
| CreatedAtUtc | `DateTime` | Obrigatório |
| UpdatedAtUtc | `DateTime?` | Opcional |

Um usuário pertence a um departamento, pode ter gestor, pode gerir subordinados, criar solicitações e tomar decisões de aprovação.

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

Representa a solicitação consolidada de reembolso.

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| RequestNumber | `string(20)` | Único e gerado pelo sistema |
| EmployeeId / DepartmentId | `Guid` | Obrigatórios |
| ReferenceMonth | `DateOnly` | Primeiro dia do mês de referência |
| Status | `RequestStatus` | Ver fluxo de status |
| TotalAmount | `decimal(12,2)` | Calculado pelo sistema |
| SubmittedAtUtc / ManagerDecisionAtUtc / FinanceDecisionAtUtc / PaidAtUtc | `DateTime?` | Datas das etapas |
| CreatedAtUtc / UpdatedAtUtc | `DateTime` / `DateTime?` | Auditoria |
| Version | `int` | Concorrência otimista |

Status: `Draft`, `PendingManagerApproval`, `RejectedByManager`, `ReturnedByManager`, `PendingFinanceValidation`, `RejectedByFinance`, `ReturnedByFinance`, `ApprovedForPayment`, `Paid` e `Cancelled`.

Uma solicitação pertence a um colaborador, possui despesas e histórico, e pode possuir um pagamento.

### ExpenseItem

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| ReimbursementRequestId / ExpenseCategoryId | `Guid` | Obrigatórios |
| ExpenseDate | `DateOnly` | Obrigatório |
| Description | `string(300)` | Obrigatório |
| Amount | `decimal(12,2)` | Maior que zero |
| MerchantName | `string(150)` | Obrigatório |
| ReceiptFileName | `string(255)?` | Conforme categoria |
| ReceiptStorageKey | `string(500)?` | Conforme categoria |
| CreatedAtUtc / UpdatedAtUtc | `DateTime` / `DateTime?` | Auditoria |

### ApprovalDecision

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| ReimbursementRequestId / DecidedByUserId | `Guid` | Obrigatórios |
| DecisionLevel | `DecisionLevel` | Manager ou Finance |
| Decision | `Decision` | Approved, Rejected ou Returned |
| Comment | `string(500)?` | Obrigatório em rejeição/devolução |
| CreatedAtUtc | `DateTime` | Obrigatório |

### Payment

| Campo | Tipo | Regra |
| --- | --- | --- |
| Id | `Guid` | Chave primária |
| ReimbursementRequestId | `Guid` | Obrigatório e único |
| PaymentReference | `string(100)` | Obrigatório e único |
| PaidAmount | `decimal(12,2)` | Igual ao total da solicitação |
| PaymentDate | `DateOnly` | Obrigatório |
| ProcessedByUserId | `Guid` | Obrigatório |
| Notes | `string(500)?` | Opcional |
| CreatedAtUtc | `DateTime` | Obrigatório |

### RequestStatusHistory

Registra todas as mudanças de status: `PreviousStatus`, `NewStatus`, `ChangedByUserId`, `Reason` e `CreatedAtUtc`. O status anterior só é nulo na criação. O motivo é obrigatório em rejeição, devolução e cancelamento.

## 3. Regras de negócio

### Criação e edição

- O colaborador cria solicitações apenas para si e elas começam em `Draft`.
- Antes do envio, uma solicitação exige pelo menos uma despesa válida.
- Todas as despesas devem pertencer ao mês de `ReferenceMonth`; não podem ser futuras nem anteriores a 90 dias da criação da solicitação.
- Cada despesa vale mais que zero e no máximo 50.000,00; descrição possui de 10 a 300 caracteres.
- Comprovantes obrigatórios devem ser PDF, JPG ou PNG, com no máximo 5 MB.
- Apenas o proprietário edita solicitações em `Draft`, `ReturnedByManager` ou `ReturnedByFinance`.
- O total é sempre a soma dos itens ativos; nunca é informado manualmente.
- Não é permitido excluir o último item de uma solicitação que já foi enviada; ela deve ser cancelada.

### Envio, aprovação e validação

- O envio é permitido em `Draft`, `ReturnedByManager` e `ReturnedByFinance`; exige gestor direto ativo, despesas válidas, comprovantes exigidos e total maior que zero.
- Enviar altera o status para `PendingManagerApproval` e cria histórico. O reenvio preserva decisões anteriores.
- Gestor age apenas em solicitações pendentes de seus subordinados diretos e nunca na própria solicitação.
- Aprovação do gestor muda para `PendingFinanceValidation`; rejeição para `RejectedByManager`; devolução para `ReturnedByManager`.
- Finance e Admin agem somente em `PendingFinanceValidation`. Aprovação muda para `ApprovedForPayment`; rejeição para `RejectedByFinance`; devolução para `ReturnedByFinance`.
- Rejeições e devoluções exigem comentário de 10 a 500 caracteres. Toda decisão cria `ApprovalDecision` e `RequestStatusHistory`.

### Pagamento, cancelamento e limite mensal

- Apenas Finance/Admin registra pagamento, somente em `ApprovedForPayment`.
- Um pedido possui no máximo um pagamento; valor pago deve igualar `TotalAmount`, a data não pode ser futura e a referência é única.
- O pagamento muda o status para `Paid`. Pedido pago é imutável.
- Somente o proprietário pode cancelar em `Draft`, `ReturnedByManager` ou `ReturnedByFinance`, com motivo de 10 a 500 caracteres. Rejeitados não podem ser cancelados.
- O limite mensal considera despesas da mesma categoria, colaborador e mês em solicitações `PendingManagerApproval`, `PendingFinanceValidation`, `ApprovedForPayment` e `Paid`. Rascunhos, devolvidas, rejeitadas e canceladas não entram no cálculo.

## 4. Casos de uso principais

| Caso | Resumo |
| --- | --- |
| UC01 — Registrar solicitação | Colaborador cria rascunho, adiciona despesas e o sistema calcula o total. |
| UC02 — Enviar para aprovação | Sistema valida regras, limite mensal, atualiza status e registra histórico. |
| UC03 — Decisão do gestor | Gestor consulta a fila, aprova, rejeita ou devolve pedidos autorizados. |
| UC04 — Validação financeira | Finance confere itens, comprovantes e totais antes de decidir. |
| UC05 — Registrar pagamento | Finance informa referência, data e valor; o sistema paga e registra histórico. |

## 5. Endpoints

Todas as rotas usam o prefixo `/api`.

| Controller | Rota | Acesso | Função |
| --- | --- | --- | --- |
| Auth | `POST /auth/login` | Público | Autentica e retorna JWT |
| Auth | `POST /auth/refresh`, `GET /auth/me` | Autenticado | Renova token / retorna usuário |
| Requests | `POST /reimbursement-requests` | Employee | Cria rascunho |
| Requests | `GET /reimbursement-requests`, `GET /reimbursement-requests/{id}` | Autorizado | Lista / detalha pedidos |
| Requests | `PUT /reimbursement-requests/{id}`, `/submit`, `/cancel`, `/history` | Proprietário/autorizado | Atualiza, envia, cancela e vê histórico |
| Expenses | `/reimbursement-requests/{requestId}/expenses` | Proprietário | Adiciona, altera ou remove item |
| Expenses | `/reimbursement-requests/{requestId}/expenses/{expenseId}/receipt` | Autorizado | Anexa ou baixa comprovante |
| Manager | `/manager/approvals/pending`, `/{requestId}/approve|reject|return` | Manager | Fila e decisões gerenciais |
| Finance | `/finance/requests/pending`, `/{requestId}/approve|reject|return|payment` | Finance/Admin | Fila, decisão e pagamento |
| Admin | `/admin/departments`, `/admin/users`, `/admin/expense-categories` | Admin | CRUD administrativo |

Listagens devem aceitar `status`, `referenceMonth`, `page`, `pageSize`, `sortBy` e `sortDirection`; Finance/Admin também filtram por `employeeId` e `departmentId`.

Validações ocorrem no DTO e obrigatoriamente no serviço, para proteger qualquer ponto de entrada futuro.

## 6. Arquitetura e fluxo de dados

```text
Cliente HTTP → Controller → DTO + validação → Application Service
→ autorização e regras → Repository / Unit of Work → EF Core → SQL Server
→ Response DTO → Cliente HTTP
```

| Camada | Responsabilidade |
| --- | --- |
| Controller | HTTP, usuário autenticado e código de resposta. |
| DTO | Contratos de entrada e saída; nunca expor entidade diretamente. |
| Service | Regras, autorização contextual, status e transações. |
| Repository | Busca e persistência, sem regras de negócio. |
| Unit of Work | `SaveChangesAsync` e transações. |
| DbContext | Mapeamentos, índices, chaves e relacionamentos. |

## 7. Requisitos técnicos

- ASP.NET Core Web API, .NET LTS, Entity Framework Core e SQL Server.
- JWT Bearer, FluentValidation, Serilog, Swagger/OpenAPI, xUnit, Moq e FluentAssertions.
- Senhas somente com hash seguro; access token de 60 minutos e refresh token de 7 dias.
- Toda rota, exceto login, exige autenticação; autorização combina roles e contexto no serviço.
- Usar `ProblemDetails`: 400 (dados inválidos), 401, 403, 404, 409 (conflito/concorrência) e 500.
- Nunca expor exceções internas em produção ou registrar senha, token, comprovante e dados pessoais além do necessário.
- Registrar logs de login, alterações, decisões, pagamentos, erros e acessos negados.
- `ReimbursementRequest` usa concorrência otimista; conflito retorna 409 e exige recarregar os dados.

## 8. Repositories e Unit of Work

Interfaces: `IUserRepository`, `IDepartmentRepository`, `IExpenseCategoryRepository`, `IReimbursementRequestRepository`, `IExpenseItemRepository`, `IApprovalDecisionRepository`, `IPaymentRepository` e `IUnitOfWork`.

Consultas devem permitir paginação e usar `AsNoTracking` quando forem somente leitura. A busca detalhada de pedido carrega colaborador, departamento, despesas/categorias, decisões, pagamento e histórico. Transições de status devem ocorrer em transação e somente o `IUnitOfWork` chama `SaveChangesAsync`.

## 9. Dados de teste

### Departamentos

| Nome | Centro de custo |
| --- | --- |
| Tecnologia | TI-100 |
| Comercial | COM-200 |
| Financeiro | FIN-300 |

### Usuários

| Nome | E-mail | Perfil | Departamento | Gestor |
| --- | --- | --- | --- | --- |
| Ana Souza | ana.souza@empresa.test | Employee | Tecnologia | Carlos Lima |
| Bruno Alves | bruno.alves@empresa.test | Employee | Comercial | Marina Costa |
| Carlos Lima | carlos.lima@empresa.test | Manager | Tecnologia | — |
| Marina Costa | marina.costa@empresa.test | Manager | Comercial | — |
| Fernanda Rocha | fernanda.rocha@empresa.test | Finance | Financeiro | — |
| Admin Sistema | admin@empresa.test | Admin | Financeiro | — |

Senha de desenvolvimento: `SenhaTeste@123`.

| Categoria | Limite mensal | Comprovante |
| --- | ---: | --- |
| Alimentação | 800,00 | Sim |
| Transporte | 600,00 | Sim |
| Hospedagem | 3.000,00 | Sim |
| Material de escritório | 500,00 | Sim |
| Quilometragem | 1.000,00 | Não |
| Outros | 300,00 | Sim |

Exemplo: `RR-2026-000001`, de Ana Souza, referência agosto/2026, `PendingManagerApproval`, com total de **303,40**.

## 10. Estrutura Git recomendada

```text
src/
  Reimbursement.Api/            Controllers, Middleware, Filters e Configuration
  Reimbursement.Application/    DTOs, Interfaces, Services, Validators e Mappings
  Reimbursement.Domain/         Entities, Enums, Exceptions e Rules
  Reimbursement.Infrastructure/ Persistence, Repositories, Authentication, Storage e Logging
  Reimbursement.Tests/          Unit e Integration
docs/                           Regras, contrato da API, modelo e decisões
```

Branches: `main` (estável), `develop` (integração), `feature/authentication`, `feature/request-management`, `feature/expense-items`, `feature/manager-approval`, `feature/finance-validation`, `feature/payment`, `feature/admin-management` e `feature/tests`.

Cada feature deve abrir pull request para `develop` com descrição da funcionalidade, regras atendidas, endpoints, testes e evidência de execução.

## Próximos passos

1. Configurar a base da API, `DbContext`, controllers e Swagger.
2. Modelar as entidades e criar a migration inicial.
3. Criar os dados de desenvolvimento e autenticação.
4. Implementar rascunhos, despesas, aprovações, validação e pagamento.
5. Cobrir regras e endpoints com testes.
