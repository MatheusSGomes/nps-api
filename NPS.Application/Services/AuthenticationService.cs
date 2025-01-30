using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        byte[] encodedSecret = Encoding.UTF8.GetBytes(secret);

        // 1 byte = 8 bits
        // 32 bytes = 256 bits
        if (encodedSecret.Length < 32)
            throw new ArgumentOutOfRangeException("Chave de autenticação inválida ou vazia, menor que 256 bits (32 bytes/caracteres)");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(encodedSecret);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        double.TryParse(settings["Expires"], out double expiresAt);

        var token = new JwtSecurityToken(
            issuer: settings["Issuer"],
            audience: settings["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiresAt),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
