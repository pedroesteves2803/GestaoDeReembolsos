using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.ReimbursementRequest;

public class DecisionManagerResponseDto(
    Guid id,
    string requestNumber,
    Decision decision,
    ReimbursementRequestEnum status,
    DateTime managerDecisionAtUtc
    )
{
    public Guid Id { get; set; } = id;
    
    public string RequestNumber { get; set; } = requestNumber;

    public Decision Decision { get; set; } = decision;
    
    public ReimbursementRequestEnum Status { get; set; } = status;
    
    public DateTime ManagerDecisionAtUtc { get; set; } = managerDecisionAtUtc;
    
}