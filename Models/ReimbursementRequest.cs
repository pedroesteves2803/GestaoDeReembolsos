using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class ReimbursementRequest
{
    public Guid Id { get; set; }

    public string RequestNumber { get; set; } = string.Empty;

    public Guid EmployeeId { get; set; }

    public User Employee { get; set; } = null!;
    
    public Guid DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public DateOnly ReferenceMonth { get; set; }

    public ReimbursementRequestEnum Status { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? SubmittedAtUtc { get; set; }
    
    public DateTime? ManagerDecisionAtUtc { get; set; }

    public DateTime? FinanceDecisionAtUtc { get; set; }

    public DateTime? PaidAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }

    public byte[] Version { get; set; }  = [];
}