namespace GestaodeReembolsos.Models;

public class CategoriaDespesa
{
    public Guid Id { get; set; }  = Guid.NewGuid();

    public string Nome { get; set; } = string.Empty;

    public Decimal? LimiteMensal { get; set; }

    public bool ExigeComprovante { get; set; }

    public bool Ativo { get; set; } = true;
    
    public ICollection<ItemDespesa> ItensDespesa { get; set; } = new List<ItemDespesa>();
}
