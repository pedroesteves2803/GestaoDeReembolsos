using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class DecisionManagerRequestDto
{
    [Required(ErrorMessage = "Informe uma decisão.")]
    public Decisao Decisao { get; set; }

    [Required(ErrorMessage = "Informe um comentário.")]
    public string Comentario { get; set; } = string.Empty;
    
}