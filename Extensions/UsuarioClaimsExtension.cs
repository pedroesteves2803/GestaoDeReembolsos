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
}