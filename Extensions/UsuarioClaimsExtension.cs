using System.Security.Claims;
using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Extensions;

public static class UserClaimsExtension
{
    public static IEnumerable<Claim> ObterClaims(this Usuario usuario)
    {
        return new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.PerfilUsuario.ToString()),
            new Claim("departmentId", usuario.DepartamentoId.ToString())
        };
    }
    
    public static Guid ObterIdUsuario(this ClaimsPrincipal usuario)
    {
        var valorId = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(valorId, out var usuarioId))
            throw new UnauthorizedAccessException("Token inválido.");

        return usuarioId;
    }
}