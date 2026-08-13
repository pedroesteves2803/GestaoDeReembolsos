namespace GestaodeReembolsos.Models;

public class Payment
{
    public Guid Id { get; set; }

    public Guid ReimbursementRequestId { get; set; }

    public ReimbursementRequest ReimbursementRequest { get; set; } = null!;

    public string PaymentReference { get; set; } = string.Empty;

    public decimal PaidAmount { get; set; }

    public DateOnly PaymentDate { get; set; }

    public Guid ProcessedByUserId { get; set; }

    public User ProcessedByUser { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}