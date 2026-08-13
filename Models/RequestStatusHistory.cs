using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class RequestStatusHistory
{
    public Guid Id { get; set; }

    public ReimbursementRequestEnum? PreviousStatus { get; set; }
    
    public ReimbursementRequestEnum NewStatus { get; set; }

    public Guid ReimbursementRequestId { get; set; }

    public ReimbursementRequest ReimbursementRequest { get; set; } = null!;

    public Guid ChangedByUserId { get; set; }

    public User ChangedByUser { get; set; } = null!;

    public string? Reason { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}