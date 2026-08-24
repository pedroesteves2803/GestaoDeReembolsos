using System.ComponentModel.DataAnnotations;

namespace GestaodeReembolsos.Dtos.Autenticacao;

public class LoginRequestDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage ="O e-mail é inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha")]
    public string Senha { get; set; } = string.Empty;
}
