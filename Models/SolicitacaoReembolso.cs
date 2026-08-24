using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class SolicitacaoReembolso
{
    public Guid Id { get; set; }

    public string NumeroSolicitacao { get; set; } = string.Empty;

    public Guid ColaboradorId { get; set; }

    public Usuario Colaborador { get; set; } = null!;
    
    public Guid DepartamentoId { get; set; }

    public Departamento Departamento { get; set; } = null!;

    public DateOnly MesReferencia { get; set; }

    public StatusSolicitacaoReembolso Status { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime EnviadaEmUtc { get; set; }
    
    public DateTime DecididaPeloGestorEmUtc { get; set; }

    public DateTime? DecididaPeloFinanceiroEmUtc { get; set; }

    public DateTime? PagaEmUtc { get; set; }
    
    public DateTime CriadaEmUtc { get; set; }
    
    public DateTime? AtualizadaEmUtc { get; set; }

    public ICollection<ItemDespesa> ItensDespesa { get; set; } = new List<ItemDespesa>();
    
    public ICollection<DecisaoAprovacao> DecisoesAprovacao { get; set; } = new List<DecisaoAprovacao>();
    
    public ICollection<HistoricoStatusSolicitacao> HistoricosStatusSolicitacao { get; set; } = new List<HistoricoStatusSolicitacao>();
    
    public Pagamento? Pagamento { get; set; } = null!;
    
    public byte[] Versao { get; set; }  = [];
}
