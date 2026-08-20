using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestaodeReembolsos.Migrations
{
    /// <inheritdoc />
    public partial class CriarBancoEmPortugues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasDespesa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LimiteMensal = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ExigeComprovante = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasDespesa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoCentroCusto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(SYSUTCDATETIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    HashSenha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GestorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(SYSUTCDATETIME())"),
                    AtualizadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Usuarios_GestorId",
                        column: x => x.GestorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitacoesReembolso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroSolicitacao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ColaboradorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MesReferencia = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    EnviadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecididaPeloGestorEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DecididaPeloFinanceiroEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PagaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(SYSUTCDATETIME())"),
                    AtualizadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesReembolso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitacoesReembolso_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitacoesReembolso_Usuarios_ColaboradorId",
                        column: x => x.ColaboradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DecisoesAprovacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitacaoReembolsoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecididaPorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NivelDecisao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Decisao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(SYSUTCDATETIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisoesAprovacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DecisoesAprovacao_SolicitacoesReembolso_SolicitacaoReembolsoId",
                        column: x => x.SolicitacaoReembolsoId,
                        principalTable: "SolicitacoesReembolso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DecisoesAprovacao_Usuarios_DecididaPorUsuarioId",
                        column: x => x.DecididaPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistoricosStatusSolicitacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NovoStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SolicitacaoReembolsoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlteradoPorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(SYSUTCDATETIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosStatusSolicitacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricosStatusSolicitacao_SolicitacoesReembolso_SolicitacaoReembolsoId",
                        column: x => x.SolicitacaoReembolsoId,
                        principalTable: "SolicitacoesReembolso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistoricosStatusSolicitacao_Usuarios_AlteradoPorUsuarioId",
                        column: x => x.AlteradoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItensDespesa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitacaoReembolsoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoriaDespesaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataDespesa = table.Column<DateOnly>(type: "date", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    NomeEstabelecimento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NomeArquivoComprovante = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChaveArmazenamentoComprovante = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(SYSUTCDATETIME())"),
                    AtualizadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensDespesa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensDespesa_CategoriasDespesa_CategoriaDespesaId",
                        column: x => x.CategoriaDespesaId,
                        principalTable: "CategoriasDespesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItensDespesa_SolicitacoesReembolso_SolicitacaoReembolsoId",
                        column: x => x.SolicitacaoReembolsoId,
                        principalTable: "SolicitacoesReembolso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pagamentos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitacaoReembolsoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenciaPagamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PaymentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProcessadoPorUsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(SYSUTCDATETIME())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagamentos_SolicitacoesReembolso_SolicitacaoReembolsoId",
                        column: x => x.SolicitacaoReembolsoId,
                        principalTable: "SolicitacoesReembolso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pagamentos_Usuarios_ProcessadoPorUsuarioId",
                        column: x => x.ProcessadoPorUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CategoriasDespesa",
                columns: new[] { "Id", "Ativo", "ExigeComprovante", "LimiteMensal", "Name" },
                values: new object[,]
                {
                    { new Guid("1d7d9f1c-cba5-488d-a718-5102401ee001"), true, true, 800m, "Alimentação" },
                    { new Guid("413b783c-2030-40a8-8a09-a03ea12cd002"), true, true, 600m, "Transporte" },
                    { new Guid("59f8b4b9-e72b-4dbd-955b-cb67e940e003"), true, true, 3000m, "Hospedagem" }
                });

            migrationBuilder.InsertData(
                table: "CategoriasDespesa",
                columns: new[] { "Id", "Ativo", "LimiteMensal", "Name" },
                values: new object[] { new Guid("a80f04df-615f-4f0c-8efc-44f39543f005"), true, 1000m, "Quilometragem" });

            migrationBuilder.InsertData(
                table: "CategoriasDespesa",
                columns: new[] { "Id", "Ativo", "ExigeComprovante", "LimiteMensal", "Name" },
                values: new object[,]
                {
                    { new Guid("bff56b5b-29d8-40f4-8e32-f9a4590d3006"), true, true, 300m, "Outros" },
                    { new Guid("f741aa00-84e1-4a02-a5e4-249268fee004"), true, true, 500m, "Material de escritório" }
                });

            migrationBuilder.InsertData(
                table: "Departamentos",
                columns: new[] { "Id", "Ativo", "CodigoCentroCusto", "CriadaEmUtc", "Name" },
                values: new object[,]
                {
                    { new Guid("6f468dfa-4cb8-46a9-a6fd-4e333787e202"), true, "COM-200", new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Comercial" },
                    { new Guid("c3a3b4d8-28c3-4d72-a479-55c39a5bd101"), true, "TI-100", new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Tecnologia" },
                    { new Guid("e154d747-f905-4bea-a32d-13bcc991c303"), true, "FIN-300", new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Financeiro" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "AtualizadaEmUtc", "CriadaEmUtc", "DepartamentoId", "Email", "GestorId", "HashSenha", "NomeCompleto", "Role" },
                values: new object[,]
                {
                    { new Guid("00bf84cc-22dc-401b-a1ec-a8d523687105"), true, null, new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("e154d747-f905-4bea-a32d-13bcc991c303"), "fernanda.rocha@empresa.test", null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Fernanda Rocha", "Finance" },
                    { new Guid("9f586795-fc0f-4b35-b251-48cbab902401"), true, null, new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("c3a3b4d8-28c3-4d72-a479-55c39a5bd101"), "carlos.lima@empresa.test", null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Carlos Lima", "Manager" },
                    { new Guid("ad9bf52d-1a99-4f03-a64e-65aee12f2502"), true, null, new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6f468dfa-4cb8-46a9-a6fd-4e333787e202"), "marina.costa@empresa.test", null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Marina Costa", "Manager" },
                    { new Guid("c95ab38b-5c3e-45a4-a087-c930bea18e06"), true, null, new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("e154d747-f905-4bea-a32d-13bcc991c303"), "admin@empresa.test", null, "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Admin Sistema", "Admin" },
                    { new Guid("d62bdf0d-2268-4c5f-b6ef-d7d9f62d3603"), true, null, new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("c3a3b4d8-28c3-4d72-a479-55c39a5bd101"), "ana.souza@empresa.test", new Guid("9f586795-fc0f-4b35-b251-48cbab902401"), "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Ana Souza", "Employee" },
                    { new Guid("fbb62499-5f39-4e04-98fe-5762a2c15704"), true, null, new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6f468dfa-4cb8-46a9-a6fd-4e333787e202"), "bruno.alves@empresa.test", new Guid("ad9bf52d-1a99-4f03-a64e-65aee12f2502"), "$2y$10$GtYsJ4hK2nFZcmlVkht08uEg4/iiM.XuDj3oFODONcwxqeowXch4W", "Bruno Alves", "Employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategory_Name",
                table: "CategoriasDespesa",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DecisoesAprovacao_DecididaPorUsuarioId",
                table: "DecisoesAprovacao",
                column: "DecididaPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisoesAprovacao_SolicitacaoReembolsoId",
                table: "DecisoesAprovacao",
                column: "SolicitacaoReembolsoId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CostCenterCode",
                table: "Departamentos",
                column: "CodigoCentroCusto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_Name",
                table: "Departamentos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosStatusSolicitacao_AlteradoPorUsuarioId",
                table: "HistoricosStatusSolicitacao",
                column: "AlteradoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosStatusSolicitacao_SolicitacaoReembolsoId",
                table: "HistoricosStatusSolicitacao",
                column: "SolicitacaoReembolsoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensDespesa_CategoriaDespesaId",
                table: "ItensDespesa",
                column: "CategoriaDespesaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensDespesa_SolicitacaoReembolsoId",
                table: "ItensDespesa",
                column: "SolicitacaoReembolsoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ProcessadoPorUsuarioId",
                table: "Pagamentos",
                column: "ProcessadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_ReferenciaPagamento",
                table: "Pagamentos",
                column: "ReferenciaPagamento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pagamentos_SolicitacaoReembolsoId",
                table: "Pagamentos",
                column: "SolicitacaoReembolsoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReimbursementRequest_RequestNumber",
                table: "SolicitacoesReembolso",
                column: "NumeroSolicitacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesReembolso_ColaboradorId",
                table: "SolicitacoesReembolso",
                column: "ColaboradorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitacoesReembolso_DepartamentoId",
                table: "SolicitacoesReembolso",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DepartamentoId",
                table: "Usuarios",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_GestorId",
                table: "Usuarios",
                column: "GestorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DecisoesAprovacao");

            migrationBuilder.DropTable(
                name: "HistoricosStatusSolicitacao");

            migrationBuilder.DropTable(
                name: "ItensDespesa");

            migrationBuilder.DropTable(
                name: "Pagamentos");

            migrationBuilder.DropTable(
                name: "CategoriasDespesa");

            migrationBuilder.DropTable(
                name: "SolicitacoesReembolso");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Departamentos");
        }
    }
}
