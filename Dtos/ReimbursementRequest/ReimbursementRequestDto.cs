using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Validations;

namespace GestaodeReembolsos.Dtos.ReimbursementRequest;

public class ReimbursementRequestDto
{
    [NotEmptyGuid]
    [Required(ErrorMessage = "Informe o departamento")]
    public Guid DepartmentId { get; set; }

    [Required(ErrorMessage = "Informe o mes de referencia")]
    public DateOnly ReferenceMonth { get; set; }
}