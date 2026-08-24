using System.ComponentModel.DataAnnotations;

namespace GestaodeReembolsos.Dtos.Financeiro;

public class RegistrarPagamentoRequestDto
{
    [Required(ErrorMessage = "O valor pago é obrigatório.")]
    public decimal ValorPago { get; set; }

    [Required(ErrorMessage = "A data do pagamento é obrigatória.")]
    public DateOnly DataPagamento { get; set; }
    
    [Required(ErrorMessage = "A referência é obrigatória.")]
    public string Referencia { get; set; } = string.Empty;
    
    public string? Observacao { get; set; } = string.Empty;
}
