using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Validations;

namespace GestaodeReembolsos.Dtos.ReimbursementRequest;

public partial class ExpenseItemResponseDto(
    Guid id
)
{
    public Guid Id { get; set; } = id;
}