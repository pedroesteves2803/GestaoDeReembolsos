using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class SubmitResponseDto(
    Guid id,
    string requestNumber,
    StatusSolicitacaoReembolso status,
    DateTime submittedAtUtc)
{
    public Guid Id { get; set; } = id;
    
    public string NumeroSolicitacao { get; set; } = requestNumber;
    
    public StatusSolicitacaoReembolso Status { get; set; } = status;
    
    public DateTime EnviadaEmUtc { get; set; } = submittedAtUtc;
    
}