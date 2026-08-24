using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string NomeCompleto { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public string HashSenha { get; set; } = string.Empty;

    public PerfilUsuario PerfilUsuario { get; set; }

    public Guid DepartamentoId { get; set; }
    
    public Departamento Departamento { get; set; } = null!;
    
    public Guid? GestorId { get; set; }

    public Usuario? Gestor { get; set; }
    
    public bool Ativo { get; set; } = true;

    public DateTime CriadaEmUtc { get; set; }
    
    public DateTime? AtualizadaEmUtc { get; set; }

    public ICollection<Usuario> Subordinados { get; set; } = new List<Usuario>();

    public ICollection<SolicitacaoReembolso> SolicitacoesReembolso { get; set; } = new List<SolicitacaoReembolso>();
    
    public ICollection<DecisaoAprovacao> DecisoesAprovacao { get; set; } = new List<DecisaoAprovacao>();
    public ICollection<HistoricoStatusSolicitacao> HistoricosStatusSolicitacao { get; set; } = new List<HistoricoStatusSolicitacao>();
    
    public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();

    
}
