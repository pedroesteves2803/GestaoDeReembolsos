namespace GestaodeReembolsos.Models;

public class CategoriaDespesa
{
    public Guid Id { get; set; }  = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public Decimal? LimiteMensal { get; set; }

    public bool ExigeComprovante { get; set; }

    public bool Ativo { get; set; } = true;
    
    public ICollection<ItemDespesa> ExpenseItems { get; set; } = new List<ItemDespesa>();
}