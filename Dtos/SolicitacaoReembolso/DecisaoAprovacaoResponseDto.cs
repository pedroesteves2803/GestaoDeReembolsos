using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class DecisaoAprovacaoResponseDto
{
    public NivelDecisao NivelDecisao { get; set; }
    public Decisao Decisao { get; set; }
    public string? Comentario { get; set; } = string.Empty;
    public string DecididaPor { get; set; } = string.Empty;
    public DateTime CriadaEmUtc { get; set; }
}