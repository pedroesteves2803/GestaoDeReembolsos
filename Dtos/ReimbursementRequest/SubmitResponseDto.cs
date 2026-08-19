using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.ReimbursementRequest;

public class SubmitResponseDto(
    Guid id,
    string requestNumber,
    ReimbursementRequestEnum status,
    DateTime submittedAtUtc)
{
    public Guid Id { get; set; } = id;
    
    public string RequestNumber { get; set; } = requestNumber;
    
    public ReimbursementRequestEnum Status { get; set; } = status;
    
    public DateTime SubmittedAtUtc { get; set; } = submittedAtUtc;
    
}