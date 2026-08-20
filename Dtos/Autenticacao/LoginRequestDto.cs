using System.ComponentModel.DataAnnotations;

namespace GestaodeReembolsos.Dtos.Autenticacao;

public class LoginRequestDto
{
    [Required(ErrorMessage = "o E-mail é obrigatorio")]
    [EmailAddress(ErrorMessage ="o E-mail é invalido!")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a senha")]
    public string Senha { get; set; } = string.Empty;
}