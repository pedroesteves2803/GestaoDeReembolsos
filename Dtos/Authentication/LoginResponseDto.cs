namespace GestaodeReembolsos.Dtos.Authentication;

public class LoginResponseDto
{
    public string? Message { get; set; }
    public string? Token { get; set; }
    
    public LoginResponseDto(string message,  string? token = null)
    {
        Message = message;
        Token = token;
    }
}