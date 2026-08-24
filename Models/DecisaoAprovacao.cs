using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class DecisaoAprovacao
{
    public Guid Id { get; set; }

    public Guid SolicitacaoReembolsoId { get; set; }

    public SolicitacaoReembolso SolicitacaoReembolso { get; set; } = null!;

    public Guid DecididaPorUsuarioId { get; set; }

    public Usuario DecididaPorUsuario { get; set; } = null!;

    public NivelDecisao NivelDecisao { get; set; }

    public Decisao Decisao { get; set; }

    public string? Comentario { get; set; }

    public DateTime CriadaEmUtc { get; set; }
    
}
