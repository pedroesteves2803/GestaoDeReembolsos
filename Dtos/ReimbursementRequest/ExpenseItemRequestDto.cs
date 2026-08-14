using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Validations;

namespace GestaodeReembolsos.Dtos.ReimbursementRequest;
public class ExpenseItemRequestDto
{
    [NotEmptyGuid]
    public Guid ExpenseCategoryId { get; set; }

    public DateOnly ExpenseDate { get; set; }

    [Required(ErrorMessage = "Descrição é obrigatória.")]
    [StringLength(300, MinimumLength = 10,
        ErrorMessage = "A descrição deve ter entre 10 e 300 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Range(
        typeof(decimal),
        "0.01",
        "50000.00",
        ParseLimitsInInvariantCulture = true,
        ErrorMessage = "O valor deve estar entre 0,01 e 50.000,00."
    )]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Nome do estabelecimento é obrigatório.")]
    [StringLength(150)]
    public string MerchantName { get; set; } = string.Empty;
}