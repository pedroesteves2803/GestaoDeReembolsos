using GestaodeReembolsos.Data;
using GestaodeReembolsos.Dtos;
using GestaodeReembolsos.Dtos.Authentication;
using GestaodeReembolsos.Dtos.Shared;
using GestaodeReembolsos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost("api/v1/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto requestDto,
        [FromServices] GestaoDeReembolsoContext context,
        [FromServices] TokenService tokenService
        )
    {
        var user = await context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == requestDto.Email);

        if (user == null)
            return StatusCode(401, new ApiResponseDto<LoginResponseDto>(
                true,
                "Usuário ou senha inválidos"
            ));
        
        if(!BCrypt.Net.BCrypt.Verify(requestDto.Password, user.PasswordHash))
            return StatusCode(401, new ApiResponseDto<LoginResponseDto>(
                true,
                "Usuário ou senha inválidos"
            ));
        
        try
        {
            var token = tokenService.Generatetoken(user);
            
            return StatusCode(200, new ApiResponseDto<LoginResponseDto>(
                true,
                "Autenticado com sucesso!",
                new LoginResponseDto("Usuário logado!", token)
                ));
        }
        catch
        {
            return StatusCode(500, new ApiResponseDto<LoginResponseDto>(
                true,
                "Erro interno do servidor"
            ));
        }
    }
}