using GestaodeReembolsos.Enums;
using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class ListaSolicitacaoReembolsoResponseDto()
{
    public Guid Id { get; set; }
    public string NumeroSolicitacao { get; set; } = string.Empty;
    public DateOnly MesReferencia { get; set; }
    public StatusSolicitacaoReembolso Status { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime CriadaEmUtc { get; set; }
    public DateTime? EnviadaEmUtc { get; set; }
    public DateTime? AtualizadaEmUtc { get; set; }
}