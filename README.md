# Gestão de Reembolsos Corporativos

API para colaboradores registrarem despesas corporativas e acompanharem a aprovação pelo gestor e pelo financeiro.

Este repositório está em construção. O objetivo deste README é servir como um roteiro de estudo e implementação para quem está começando em C# e ASP.NET Core.

## O que você vai construir

O sistema terá quatro perfis:

- **Employee**: cria e acompanha os próprios pedidos de reembolso.
- **Manager**: aprova, rejeita ou devolve pedidos dos seus subordinados diretos.
- **Finance**: valida pedidos aprovados pelo gestor e registra pagamentos.
- **Admin**: administra cadastros e também pode executar ações do financeiro.

O fluxo principal será:

```text
Rascunho → aprovação do gestor → validação do financeiro → pagamento
```

Um pedido também pode ser devolvido para correção, rejeitado ou cancelado.

## Tecnologias do projeto

- C# e .NET 10 (LTS)
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger/OpenAPI para testar a API no navegador
- JWT para login e autorização
- FluentValidation para validar entradas
- Serilog para logs
- xUnit, Moq e FluentAssertions para testes

## Pré-requisitos

Instale antes de começar:

1. [.NET SDK 10](https://dotnet.microsoft.com/download)
2. [Docker Desktop](https://www.docker.com/products/docker-desktop/) **ou** SQL Server local
3. Um editor: Visual Studio, Visual Studio Code ou Rider
4. Opcional, mas recomendado: Postman ou Insomnia para testar endpoints

Confira se o .NET está instalado:

```bash
dotnet --version
```

## Como executar o projeto pela primeira vez

Abra o terminal na pasta do projeto:

```bash
cd /Users/pedroesteves/code/GestaodeReembolsos
```

Restaure os pacotes e inicie a API:

```bash
dotnet restore
dotnet run
```

Quando Swagger for configurado, abra no navegador a URL mostrada pelo terminal, normalmente:

```text
https://localhost:xxxx/swagger
```

> Neste momento o projeto ainda está na fase inicial. Swagger, controllers e o banco serão adicionados nas etapas abaixo.

## Banco de dados local com Docker

Se você não possui SQL Server instalado, pode executar um banco local com Docker:

```bash
docker run --name gestao-reembolsos-sqlserver \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD='Senha@123456' \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Para parar e iniciar novamente o banco depois:

```bash
docker stop gestao-reembolsos-sqlserver
docker start gestao-reembolsos-sqlserver
```

O arquivo `appsettings.Development.json` contém a conexão local usada durante o desenvolvimento. **Não envie senhas reais ao Git.** Mais adiante, crie um `appsettings.example.json` com valores fictícios.

## Roteiro de implementação

Faça uma etapa por vez. Ao final de cada uma, execute `dotnet build` e corrija os erros antes de avançar.

### Etapa 0 — Entender a estrutura atual

Arquivos que já existem:

- `Program.cs`: ponto de entrada da aplicação e configuração de serviços.
- `Data/GestaoDeReembolsoContext.cs`: classe que liga o C# ao banco pelo Entity Framework Core.
- `Models/User.cs`: entidade inicial de usuário.
- `Enums/Role.cs`: perfis de acesso do sistema.
- `appsettings*.json`: configurações, como a conexão com o banco.

Conceitos importantes:

- **Entidade/Model**: representa uma tabela do banco, como `User`.
- **DTO**: representa os dados recebidos ou devolvidos pela API; não exponha entidades diretamente.
- **Controller**: recebe a requisição HTTP e devolve a resposta.
- **Service**: concentra regras de negócio.
- **Repository**: faz consultas e alterações no banco.
- **Migration**: histórico versionado da estrutura do banco de dados.

### Etapa 1 — Configurar a aplicação base

- [ ] Registrar `GestaoDeReembolsoContext` no `Program.cs`.
- [ ] Adicionar controllers e Swagger/OpenAPI.
- [ ] Configurar o tratamento global de erros com `ProblemDetails`.
- [ ] Confirmar que a API inicia com `dotnet run`.
- [ ] Confirmar que `dotnet build` termina sem erros.

### Etapa 2 — Criar o domínio e os relacionamentos

- [ ] Criar `Department`.
- [ ] Completar `User` com relacionamentos: departamento, gestor e subordinados.
- [ ] Criar `ExpenseCategory`.
- [ ] Criar os enums: status do pedido, nível de decisão e decisão.
- [ ] Criar `ReimbursementRequest`, `ExpenseItem`, `ApprovalDecision`, `Payment` e `RequestStatusHistory`.
- [ ] Adicionar todos os `DbSet`s ao contexto.
- [ ] Criar mapeamentos Fluent API para chaves, tamanhos, índices únicos e relacionamentos.

### Etapa 3 — Criar o banco com migrations

- [ ] Instalar a ferramenta, se necessário: `dotnet tool install --global dotnet-ef`.
- [ ] Criar a primeira migration:

```bash
dotnet ef migrations add InitialCreate
```

- [ ] Aplicar a migration:

```bash
dotnet ef database update
```

- [ ] Conferir se o banco e as tabelas foram criados.

### Etapa 4 — Inserir dados de desenvolvimento

- [ ] Criar os departamentos Tecnologia, Comercial e Financeiro.
- [ ] Criar os seis usuários do enunciado, incluindo gestores.
- [ ] Criar as seis categorias de despesa e seus limites.
- [ ] Usar hash seguro para a senha; nunca salvar senha em texto puro.

### Etapa 5 — Login e segurança

- [ ] Criar DTOs de login e resposta de autenticação.
- [ ] Implementar hash e validação de senha.
- [ ] Configurar JWT com identificador, e-mail, perfil e departamento.
- [ ] Criar `POST /api/auth/login` e `GET /api/auth/me`.
- [ ] Implementar refresh token com validade de 7 dias.
- [ ] Proteger endpoints com `[Authorize]` e perfis adequados.

### Etapa 6 — Solicitações e despesas

- [ ] Criar DTOs e validators para solicitações e itens de despesa.
- [ ] Implementar criação, edição, consulta, envio e cancelamento de solicitações.
- [ ] Implementar inclusão, edição e exclusão de despesas.
- [ ] Calcular o total sempre pela soma dos itens.
- [ ] Validar mês de referência, datas, valores, descrição e comprovante.
- [ ] Implementar upload seguro de PDF, JPG e PNG com até 5 MB.

### Etapa 7 — Aprovações e pagamento

- [ ] Permitir que gestor decida somente pedidos de subordinados diretos.
- [ ] Registrar toda decisão em `ApprovalDecision`.
- [ ] Permitir que Finance/Admin valide somente pedidos pendentes do financeiro.
- [ ] Registrar pagamento somente para pedidos aprovados para pagamento.
- [ ] Criar histórico para cada alteração de status.
- [ ] Aplicar concorrência otimista no pedido para evitar atualizações conflitantes.

### Etapa 8 — Administração, filtros e qualidade

- [ ] Criar CRUD de departamentos, usuários e categorias para Admin.
- [ ] Implementar paginação, ordenação e filtros nas listagens.
- [ ] Adicionar logs de login, decisões, pagamentos, erros e acessos negados.
- [ ] Criar testes unitários dos services.
- [ ] Criar testes de integração dos endpoints principais.
- [ ] Atualizar este README com exemplos de uso do Swagger.

## Regras de negócio que não podem ser esquecidas

- O pedido nasce como `Draft` (rascunho).
- Somente o dono pode editar ou cancelar o próprio pedido.
- Só é possível enviar um pedido com ao menos uma despesa e gestor ativo.
- O pedido só pode ser editado em rascunho ou quando devolvido.
- O total do pedido é a soma das despesas.
- Gestor não pode aprovar o próprio pedido.
- Rejeições, devoluções e cancelamentos exigem uma justificativa de 10 a 500 caracteres.
- Um pedido pago não pode mais ser alterado.
- Cada mudança de status precisa ficar registrada no histórico.

## Organização sugerida de pastas

Quando o projeto crescer, use esta estrutura:

```text
Controllers/       Endpoints HTTP
Data/              DbContext e configurações EF Core
DTOs/              Contratos de entrada e saída da API
Enums/             Enumerações do domínio
Exceptions/        Exceções de negócio
Models/            Entidades do banco
Repositories/      Consultas e persistência
Services/          Regras de negócio
Validators/        Validações FluentValidation
Tests/             Testes unitários e de integração
```

## Comandos que você usará com frequência

```bash
dotnet restore                 # baixa dependências
dotnet build                   # compila e encontra erros
dotnet run                     # inicia a API
dotnet test                    # executa testes
dotnet ef migrations add Nome  # cria uma migration
dotnet ef database update      # atualiza o banco
```

## Como pedir ajuda durante o desenvolvimento

Trabalhe em blocos pequenos. Exemplos de boas próximas solicitações:

- “Explique o que é DbContext e revise o meu `Program.cs`.”
- “Implemente a etapa 1 e explique cada alteração.”
- “Revise a entidade `Department`: primeiro erros estruturais, depois regras ausentes.”
- “Crie a migration inicial e me explique como verificar o banco.”

## Convenção de commits

Use commits pequenos e descritivos, em português:

```text
feat(auth): adiciona autenticação JWT
feat(requests): cria fluxo de solicitação de reembolso
fix(requests): impede envio sem gestor ativo
docs(readme): adiciona roteiro inicial do projeto
```

Antes de um commit, confira o que será enviado:

```bash
git status
git diff
```

Nunca envie arquivos com segredos, senhas reais, tokens ou chaves de API.
