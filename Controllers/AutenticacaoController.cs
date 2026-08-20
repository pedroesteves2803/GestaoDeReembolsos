using GestaodeReembolsos.Data;
using GestaodeReembolsos.Dtos;
using GestaodeReembolsos.Dtos.Autenticacao;
using GestaodeReembolsos.Dtos.Shared;
using GestaodeReembolsos.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaodeReembolsos.Controllers;

[ApiController]
public class AutenticacaoController : ControllerBase
{
    [HttpPost("api/v1/autenticacao/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto dto,
        [FromServices] GestaoDeReembolsoContext context,
        [FromServices] ServicoToken tokenService
        )
    {
        var usuario = await context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (usuario == null)
            return StatusCode(401, new ApiResponseDto<LoginResponseDto>(
                false,
                "Usuário ou senha inválidos"
            ));
        
        if(!BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.HashSenha))
            return StatusCode(401, new ApiResponseDto<LoginResponseDto>(
                false,
                "Usuário ou senha inválidos"
            ));
        
        try
        {
            var token = tokenService.GerarToken(usuario);
            
            return StatusCode(200, new ApiResponseDto<LoginResponseDto>(
                true,
                "Autenticado com sucesso!",
                new LoginResponseDto(token)
                ));
        }
        catch
        {
            return StatusCode(500, new ApiResponseDto<LoginResponseDto>(
                false,
                "Erro interno do servidor"
            ));
        }
    }
}
