namespace GestaodeReembolsos.Models;

public class Pagamento
{
    public Guid Id { get; set; }

    public Guid SolicitacaoReembolsoId { get; set; }

    public SolicitacaoReembolso SolicitacaoReembolso { get; set; } = null!;

    public string ReferenciaPagamento { get; set; } = string.Empty;

    public decimal ValorPago { get; set; }

    public DateOnly DataPagamento { get; set; }

    public Guid ProcessadoPorUsuarioId { get; set; }

    public Usuario ProcessadoPorUsuario { get; set; } = null!;

    public string? Observacoes { get; set; }

    public DateTime CriadaEmUtc { get; set; }
}
