using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestaodeReembolsos.Extensions;
using GestaodeReembolsos.Models;
using Microsoft.IdentityModel.Tokens;

namespace GestaodeReembolsos.Services;

public class ServicoToken
{
    public string GerarToken(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        var claims = usuario.ObterClaims();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}