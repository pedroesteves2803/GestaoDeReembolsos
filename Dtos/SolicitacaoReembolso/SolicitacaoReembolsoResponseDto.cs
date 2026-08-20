namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class ReimbursementRequestResponseDto(
    Guid id,
    string requestNumber)
{
    public Guid Id { get; set; } = id;
    public string NumeroSolicitacao { get; set; } = requestNumber;
}   