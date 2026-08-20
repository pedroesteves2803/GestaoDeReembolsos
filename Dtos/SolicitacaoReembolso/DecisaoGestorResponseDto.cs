using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class DecisaoGestorResponseDto(
    Guid id,
    string numeroSolicitacao,
    Decisao decisao,
    StatusSolicitacaoReembolso status,
    DateTime decididaPeloGestorEmUtc
    )
{
    public Guid Id { get; set; } = id;
    
    public string NumeroSolicitacao { get; set; } = numeroSolicitacao;

    public Decisao Decisao { get; set; } = decisao;
    
    public StatusSolicitacaoReembolso Status { get; set; } = status;
    
    public DateTime DecididaPeloGestorEmUtc { get; set; } = decididaPeloGestorEmUtc;
    
}