using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class DecisionManagerResponseDto(
    Guid id,
    string requestNumber,
    Decisao decision,
    StatusSolicitacaoReembolso status,
    DateTime managerDecisionAtUtc
    )
{
    public Guid Id { get; set; } = id;
    
    public string NumeroSolicitacao { get; set; } = requestNumber;

    public Decisao Decisao { get; set; } = decision;
    
    public StatusSolicitacaoReembolso Status { get; set; } = status;
    
    public DateTime DecididaPeloGestorEmUtc { get; set; } = managerDecisionAtUtc;
    
}