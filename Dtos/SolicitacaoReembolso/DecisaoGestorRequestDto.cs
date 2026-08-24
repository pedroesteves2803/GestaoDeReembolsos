using System.ComponentModel.DataAnnotations;
using GestaodeReembolsos.Enums;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class DecisaoGestorRequestDto
{
    [Required(ErrorMessage = "Informe uma decisão.")]
    public Decisao Decisao { get; set; }

    public string Comentario { get; set; } = string.Empty;
}
