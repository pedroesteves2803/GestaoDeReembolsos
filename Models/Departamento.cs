namespace GestaodeReembolsos.Models;

public class Departamento
{
    public Guid Id { get; set; } =  Guid.NewGuid();

    public string Nome { get; set; } = string.Empty;

    public string CodigoCentroCusto { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
    
    public DateTime CriadaEmUtc { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    
    public ICollection<SolicitacaoReembolso> SolicitacoesReembolso { get; set; } = new List<SolicitacaoReembolso>();
    
}
