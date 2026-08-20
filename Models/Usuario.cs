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

    public Usuario? Manager { get; set; }
    
    public bool Ativo { get; set; } = true;

    public DateTime CriadaEmUtc { get; set; }
    
    public DateTime? AtualizadaEmUtc { get; set; }

    public ICollection<Usuario> Subordinados { get; set; } = new List<Usuario>();

    public ICollection<SolicitacaoReembolso> Requests { get; set; } = new  List<SolicitacaoReembolso>();
    
    public ICollection<DecisaoAprovacao> ApprovalDecisions { get; set; } = new List<DecisaoAprovacao>();
    public ICollection<HistoricoStatusSolicitacao> RequestStatusHistories { get; set; } = new List<HistoricoStatusSolicitacao>();
    
    public ICollection<Pagamento> Payments { get; set; } = new List<Pagamento>();

    
}