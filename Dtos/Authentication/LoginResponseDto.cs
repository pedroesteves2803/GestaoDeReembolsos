namespace GestaodeReembolsos.Dtos.Authentication;

public class LoginResponseDto
{
    public string? Token { get; set; }
    
    public LoginResponseDto(string message,  string? token = null)
    {
        Token = token;
    }
}