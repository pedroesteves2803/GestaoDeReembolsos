using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class ApprovalDecision
{
    public Guid Id { get; set; }

    public Guid ReimbursementRequestId { get; set; }

    public ReimbursementRequest ReimbursementRequest { get; set; } = null!;

    public Guid DecidedByUserId { get; set; }

    public User DecidedByUser { get; set; } = null!;

    public DecisionLevel DecisionLevel { get; set; }

    public Decision Decision { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    
}