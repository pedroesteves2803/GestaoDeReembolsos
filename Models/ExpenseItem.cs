namespace GestaodeReembolsos.Models;

public class ExpenseItem
{
    public Guid Id { get; set; }

    public Guid ReimbursementRequestId { get; set; }

    public ReimbursementRequest ReimbursementRequest { get; set; } = null!;
    
    public Guid ExpenseCategoryId { get; set; }

    public ExpenseCategory ExpenseCategory { get; set; } = null!;

    public DateOnly ExpenseDate { get; set; }

    public string Description { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }

    public string MerchantName { get; set; } = string.Empty;

    public string? ReceiptFileName { get; set; }

    public string? ReceiptStorageKey { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }
}