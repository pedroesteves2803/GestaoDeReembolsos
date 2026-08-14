using System.ComponentModel.DataAnnotations;

namespace GestaodeReembolsos.Validations;

public class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute()
    {
        ErrorMessage = "O campo {0} deve conter um Guid válido.";
    }
    
    public override bool IsValid(object? value)
    {
        return value is Guid guid && guid != Guid.Empty;
    }
}