using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaodeReembolsos.Migrations
{
    /// <inheritdoc />
    public partial class RenomearColunasParaPortugues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Usuarios",
                newName: "PerfilUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_User_Email",
                table: "Usuarios",
                newName: "IX_Usuario_Email");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "SolicitacoesReembolso",
                newName: "Versao");

            migrationBuilder.RenameIndex(
                name: "IX_ReimbursementRequest_RequestNumber",
                table: "SolicitacoesReembolso",
                newName: "IX_SolicitacaoReembolso_NumeroSolicitacao");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "Pagamentos",
                newName: "DataPagamento");

            migrationBuilder.RenameColumn(
                name: "PaidAmount",
                table: "Pagamentos",
                newName: "ValorPago");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "HistoricosStatusSolicitacao",
                newName: "Motivo");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Departamentos",
                newName: "Nome");

            migrationBuilder.RenameIndex(
                name: "IX_Department_Name",
                table: "Departamentos",
                newName: "IX_Departamento_Nome");

            migrationBuilder.RenameIndex(
                name: "IX_Department_CostCenterCode",
                table: "Departamentos",
                newName: "IX_Departamento_CodigoCentroCusto");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CategoriasDespesa",
                newName: "Nome");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseCategory_Name",
                table: "CategoriasDespesa",
                newName: "IX_CategoriaDespesa_Nome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PerfilUsuario",
                table: "Usuarios",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_Email",
                table: "Usuarios",
                newName: "IX_User_Email");

            migrationBuilder.RenameColumn(
                name: "Versao",
                table: "SolicitacoesReembolso",
                newName: "Version");

            migrationBuilder.RenameIndex(
                name: "IX_SolicitacaoReembolso_NumeroSolicitacao",
                table: "SolicitacoesReembolso",
                newName: "IX_ReimbursementRequest_RequestNumber");

            migrationBuilder.RenameColumn(
                name: "ValorPago",
                table: "Pagamentos",
                newName: "PaidAmount");

            migrationBuilder.RenameColumn(
                name: "DataPagamento",
                table: "Pagamentos",
                newName: "PaymentDate");

            migrationBuilder.RenameColumn(
                name: "Motivo",
                table: "HistoricosStatusSolicitacao",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Departamentos",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Departamento_Nome",
                table: "Departamentos",
                newName: "IX_Department_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Departamento_CodigoCentroCusto",
                table: "Departamentos",
                newName: "IX_Department_CostCenterCode");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "CategoriasDespesa",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_CategoriaDespesa_Nome",
                table: "CategoriasDespesa",
                newName: "IX_ExpenseCategory_Name");
        }
    }
}
