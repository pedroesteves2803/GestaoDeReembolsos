namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class SolicitacaoReembolsoResponseDto(
    Guid id,
    string numeroSolicitacao)
{
    public Guid Id { get; set; } = id;
    public string NumeroSolicitacao { get; set; } = numeroSolicitacao;
}   