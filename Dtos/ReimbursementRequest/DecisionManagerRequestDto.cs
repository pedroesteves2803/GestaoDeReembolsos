using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.ReimbursementRequest;

public class DecisionManagerRequestDto
{
    [Required(ErrorMessage = "Informe uma decisão.")]
    public Decision Decision { get; set; }

    [Required(ErrorMessage = "Informe um comentário.")]
    public string Comment { get; set; } = string.Empty;
    
}