using System.Security.Claims;
using GestaodeReembolsos.Models;

namespace GestaodeReembolsos.Extensions;

public static class UserClaimsExtension
{
    public static IEnumerable<Claim> GetClaims(this User user)
    {
        return new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleEnum.ToString()),
            new Claim("departmentId", user.DepartmentId.ToString())
        };
    }
}