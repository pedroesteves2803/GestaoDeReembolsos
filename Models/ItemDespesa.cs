namespace GestaodeReembolsos.Models;

public class ItemDespesa
{
    public Guid Id { get; set; }

    public Guid SolicitacaoReembolsoId { get; set; }

    public SolicitacaoReembolso SolicitacaoReembolso { get; set; } = null!;
    
    public Guid CategoriaDespesaId { get; set; }

    public CategoriaDespesa CategoriaDespesa { get; set; } = null!;

    public DateOnly DataDespesa { get; set; }

    public string Descricao { get; set; } = string.Empty;
    
    public decimal Valor { get; set; }

    public string NomeEstabelecimento { get; set; } = string.Empty;

    public string? NomeArquivoComprovante { get; set; }

    public string? ChaveArmazenamentoComprovante { get; set; }

    public DateTime CriadaEmUtc { get; set; }
    
    public DateTime? AtualizadaEmUtc { get; set; }
}