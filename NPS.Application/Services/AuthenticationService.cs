using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace NPS.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    public string GenerateJwtToken(string username, Dictionary<string, string> settings)
    {
        if (settings.IsNullOrEmpty())
            throw new ArgumentNullException("Settings precisa ser configurado");

        var secret = settings["Secret"];

        if (secret is null)
            throw new ArgumentNullException("Chave de autenticação inválida ou vazia");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        double.TryParse(settings["Expires"], out var expiresAt);

        var token = new JwtSecurityToken(
            issuer: settings["Issuer"],
            audience: settings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiresAt),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
