using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class HistoricoStatusSolicitacao
{
    public Guid Id { get; set; }

    public StatusSolicitacaoReembolso? StatusAnterior { get; set; }
    
    public StatusSolicitacaoReembolso NovoStatus { get; set; }

    public Guid SolicitacaoReembolsoId { get; set; }

    public SolicitacaoReembolso SolicitacaoReembolso { get; set; } = null!;

    public Guid AlteradoPorUsuarioId { get; set; }

    public Usuario AlteradoPorUsuario { get; set; } = null!;

    public string? Motivo { get; set; }

    public DateTime CriadaEmUtc { get; set; }
}
