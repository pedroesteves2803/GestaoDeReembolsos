using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class ItemDespesaDetalheResponseDto
{
    public Guid Id { get; set; }
    public string Categoria { get; set; } = null!;
    public DateOnly DataDespesa { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string NomeEstabelecimento { get; set; } = string.Empty;
    public string NomeArquivoComprovante { get; set; } = string.Empty;
}