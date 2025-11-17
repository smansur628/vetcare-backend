using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VetCare.Infrastructure.CrossCutting.Interfaces;
using VetCare.Infrastructure.CrossCutting.Options;

namespace VetCare.Infrastructure.CrossCutting.Services;

public class JwtService : IJwtService
{
    private readonly JwtOptions _opt;
    public JwtService(IOptions<JwtOptions> opt) => _opt = opt.Value;

    public string GenerateToken(int userId, string clinicCode, string role)
    {
        var claims = new[]
               {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("cli", clinicCode),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_opt.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_opt.Issuer, _opt.Audience,
                                         claims,
                                         expires: DateTime.UtcNow.AddMinutes(_opt.ExpirationMinutes),
                                         signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
