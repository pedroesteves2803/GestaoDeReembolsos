using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public RoleEnum RoleEnum { get; set; }

    public Guid DepartmentId { get; set; }
    
    public Department Department { get; set; } = null!;
    
    public Guid? ManagerId { get; set; }

    public User? Manager { get; set; }
    
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<User> Subordinates { get; set; } = new List<User>();

    public ICollection<ReimbursementRequest> Requests { get; set; } = new  List<ReimbursementRequest>();
    
    public ICollection<ApprovalDecision> ApprovalDecisions { get; set; } = new List<ApprovalDecision>();
    public ICollection<RequestStatusHistory> RequestStatusHistories { get; set; } = new List<RequestStatusHistory>();
    
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    
}