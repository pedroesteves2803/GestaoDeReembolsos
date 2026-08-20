namespace GestaodeReembolsos.Dtos.Autenticacao;

public class LoginResponseDto
{
    public string? Token { get; set; }
    
    public LoginResponseDto(string? token = null)
    {
        Token = token;
    }
}