using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NPS.Application;

public class JwtTokenBuilder
{
    private string _username = string.Empty;
    private string _secret = string.Empty;
    private string _issuer = string.Empty;
    private string _audience = string.Empty;
    private double _expires = 30;

    public JwtTokenBuilder AddUsername(string username)
    {
        _username = username;
        return this;
    }

    public JwtTokenBuilder AddSecret(string secret)
    {
        _secret = secret;
        return this;
    }

    public JwtTokenBuilder AddIssuer(string issuer)
    {
        _issuer = issuer;
        return this;
    }

    public JwtTokenBuilder AddAudience(string audience)
    {
        _audience = audience;
        return this;
    }

    public JwtTokenBuilder AddExpires(double expires)
    {
        _expires = expires;
        return this;
    }

    public void Validations()
    {
        if (_secret is null)
            throw new ArgumentNullException("Chave secreta inválida ou vazia");

        if (_username is null)
            throw new ArgumentNullException("Username deve ser informato");

        if (_expires < 0)
            throw new ArgumentNullException("Tempo de expiração não pode ser negativo");
    }

    public string Build()
    {
        Validations();

        byte[] encodedSecret = Encoding.UTF8.GetBytes(_secret);

        // 1 byte = 8 bits
        // 32 bytes = 256 bits
        if (encodedSecret.Length < 32)
            throw new ArgumentOutOfRangeException("Chave secreta menor que 256 bits (32 bytes/caracteres)");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, _username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(encodedSecret);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenJwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_expires),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(tokenJwt);
    }
}
