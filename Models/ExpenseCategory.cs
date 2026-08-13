namespace GestaodeReembolsos.Models;

public class ExpenseCategory
{
    public Guid Id { get; set; }  = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public Decimal? MonthlyLimit { get; set; }

    public bool RequiresReceipt { get; set; }

    public bool IsActive { get; set; } = true;
    
    public ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();
}