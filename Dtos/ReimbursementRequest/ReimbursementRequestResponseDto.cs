namespace GestaodeReembolsos.Dtos.ReimbursementRequest;

public class ReimbursementRequestResponseDto(
 Guid id,
    string requestNumber)
{
    public Guid Id { get; set; } = id;
    public string RequestNumber { get; set; } = requestNumber;
}   