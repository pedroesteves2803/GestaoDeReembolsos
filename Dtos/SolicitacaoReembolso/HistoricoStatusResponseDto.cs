using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class HistoricoStatusResponseDto
{
    public StatusSolicitacaoReembolso? StatusAnterior { get; set; }
    public StatusSolicitacaoReembolso NovoStatus { get; set; }
    public string? Motivo { get; set; } = string.Empty;
    public string AlteradoPor { get; set; } = string.Empty;
    public DateTime CriadaEmUtc { get; set; }
}