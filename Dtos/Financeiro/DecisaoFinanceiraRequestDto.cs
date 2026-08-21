using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.Financeiro;

public class DecisaoFinanceiraRequestDto
{
    [Required(ErrorMessage = "A decisão é obrigatoria.")]
    public Decisao Decisao { get; set; }

    public string? Comentario { get; set; } = string.Empty;
}