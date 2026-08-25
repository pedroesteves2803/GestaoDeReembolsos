using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class DetalheSolicitacaoReembolsoResponseDto
{
    public Guid Id { get; set; }
    public string NumeroSolicitacao { get; set; } = string.Empty;
    public StatusSolicitacaoReembolso Status { get; set; }
    public decimal ValorTotal { get; set; }

    public List<ItemDespesaDetalheResponseDto> Itens { get; set; } = [];
    public List<DecisaoAprovacaoResponseDto> Decisoes { get; set; } = [];
    public List<HistoricoStatusResponseDto> Historico { get; set; } = [];
    public PagamentoDetalheResponseDto? Pagamento { get; set; }
}