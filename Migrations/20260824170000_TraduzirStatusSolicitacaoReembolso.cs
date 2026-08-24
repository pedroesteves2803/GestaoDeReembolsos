using GestaodeReembolsos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaodeReembolsos.Migrations;

[DbContext(typeof(GestaoDeReembolsoContext))]
[Migration("20260824170000_TraduzirStatusSolicitacaoReembolso")]
public partial class TraduzirStatusSolicitacaoReembolso : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        AtualizarStatus(migrationBuilder, "RejectedByManager", "RejeitadaPeloGestor");
        AtualizarStatus(migrationBuilder, "ReturnedByManager", "DevolvidaPeloGestor");
        AtualizarStatus(migrationBuilder, "PendingFinanceValidation", "AguardandoValidacaoFinanceira");
        AtualizarStatus(migrationBuilder, "RejectedByFinance", "RejeitadaPeloFinanceiro");
        AtualizarStatus(migrationBuilder, "ReturnedByFinance", "DevolvidaPeloFinanceiro");
        AtualizarStatus(migrationBuilder, "ApprovedForPayment", "AprovadaParaPagamento");
        AtualizarStatus(migrationBuilder, "Paid", "Paga");
        AtualizarStatus(migrationBuilder, "Cancelled", "Cancelada");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        AtualizarStatus(migrationBuilder, "RejeitadaPeloGestor", "RejectedByManager");
        AtualizarStatus(migrationBuilder, "DevolvidaPeloGestor", "ReturnedByManager");
        AtualizarStatus(migrationBuilder, "AguardandoValidacaoFinanceira", "PendingFinanceValidation");
        AtualizarStatus(migrationBuilder, "RejeitadaPeloFinanceiro", "RejectedByFinance");
        AtualizarStatus(migrationBuilder, "DevolvidaPeloFinanceiro", "ReturnedByFinance");
        AtualizarStatus(migrationBuilder, "AprovadaParaPagamento", "ApprovedForPayment");
        AtualizarStatus(migrationBuilder, "Paga", "Paid");
        AtualizarStatus(migrationBuilder, "Cancelada", "Cancelled");
    }

    private static void AtualizarStatus(MigrationBuilder migrationBuilder, string statusAtual, string novoStatus)
    {
        migrationBuilder.Sql($"""
            UPDATE [SolicitacoesReembolso]
            SET [Status] = '{novoStatus}'
            WHERE [Status] = '{statusAtual}';

            UPDATE [HistoricosStatusSolicitacao]
            SET [StatusAnterior] = '{novoStatus}'
            WHERE [StatusAnterior] = '{statusAtual}';

            UPDATE [HistoricosStatusSolicitacao]
            SET [NovoStatus] = '{novoStatus}'
            WHERE [NovoStatus] = '{statusAtual}';
            """);
    }
}
