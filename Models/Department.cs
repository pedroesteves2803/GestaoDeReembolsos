namespace GestaodeReembolsos.Models;

public class Department
{
    public Guid Id { get; set; } =  Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string CostCenterCode { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    
    public ICollection<ReimbursementRequest> Requests { get; set; } = new List<ReimbursementRequest>();
    
}