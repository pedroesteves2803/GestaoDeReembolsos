namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class PagamentoDetalheResponseDto
{
    public Guid Id { get; set; }
    public string ReferenciaPagamento { get; set; } = string.Empty;
    public decimal ValorPago { get; set; }
    public DateTime DataPagamento { get; set; }
    public string ProcessadoPor { get; set; } = string.Empty;
    public string Observacoes { get; set; } = string.Empty;
}