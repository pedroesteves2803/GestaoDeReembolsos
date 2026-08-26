using System.ComponentModel.DataAnnotations;

namespace GestaodeReembolsos.Dtos.SolicitacaoReembolso;

public class CancelarRequestDto
{
    [StringLength(500, MinimumLength = 10,
        ErrorMessage = "O comentário deve ter entre 10 e 500 caracteres.")]
    
    [Required(ErrorMessage = "Comentário é obrigatório.")]
    public string Comentario { get; set; } = string.Empty;
}
