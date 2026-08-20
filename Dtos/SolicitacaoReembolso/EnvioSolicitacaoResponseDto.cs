using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class EnvioSolicitacaoResponseDto(
    Guid id,
    string numeroSolicitacao,
    StatusSolicitacaoReembolso status,
    DateTime enviadaEmUtc)
{
    public Guid Id { get; set; } = id;
    
    public string NumeroSolicitacao { get; set; } = numeroSolicitacao;
    
    public StatusSolicitacaoReembolso Status { get; set; } = status;
    
    public DateTime EnviadaEmUtc { get; set; } = enviadaEmUtc;
    
}