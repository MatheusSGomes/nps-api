using System.IdentityModel.Tokens.Jwt;
using NPS.Application.Services;

namespace NPS.Test.Services;

public class AuthenticationServiceTest
{
    [Fact]
    public void AuthenticationService_DeveImplementarUmaInterface()
    {
        // Arrange
        var authenticationService = new AuthenticationService();

        // Act & Assert
        Assert.IsAssignableFrom<IAuthenticationService>(authenticationService);
    }

    [Fact]
    public void GenerateJwtToken_DeveLancarExcecao_QuandoSettingsDictionaryForVazio()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>();
        var authenticationService = new AuthenticationService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            authenticationService.GenerateJwtToken("user", inMemorySettings));
        Assert.Equal("Settings precisa ser configurado", exception.ParamName);
    }

    [Fact]
    public void GenerateJwtToken_DeveLancarExcecao_QuandoSecretSettingsForNula()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", null },
            { "Expires", "Authentication:Expires" },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        var authenticationService = new AuthenticationService();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            authenticationService.GenerateJwtToken("user", inMemorySettings));
        Assert.Equal("Chave de autenticação inválida ou vazia", exception.ParamName);
    }

    [Fact]
    public void GenerateJwtToken_DeveLancarExcecao_QuandoSecretKeyForMenorQue256Bits()
    {
        // Arrange
        var authenticationService = new AuthenticationService();
        var username = "username1";
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", "MINHA_CHAVE_SECRETA_INVALIDA" },
            { "Expires", "Authentication:Expires" },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            authenticationService.GenerateJwtToken(username, inMemorySettings));
        Assert.Equal("Chave de autenticação inválida ou vazia, menor que 256 bits (32 bytes/caracteres)", exception.ParamName);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(30)]
    [InlineData(60)]
    public void GenerateJwtToken_DeveRetornarOMesmoExpireTime_QuandoTokenForGerado(int expireTimeInMinutes)
    {
        // Arrange
        DateTime utcNow = DateTime.UtcNow.AddMinutes(expireTimeInMinutes);

        var username = "fakeUsername";
        var authenticationService = new AuthenticationService();
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Expires", expireTimeInMinutes.ToString() },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        // Act
        var generatedToken = authenticationService.GenerateJwtToken(username, inMemorySettings);
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(generatedToken);

        // Assert
        Assert.Equal(jwtToken.ValidTo.Date, utcNow.Date);
        Assert.Equal(jwtToken.ValidTo.Year, utcNow.Year);
        Assert.Equal(jwtToken.ValidTo.Month, utcNow.Month);
        Assert.Equal(jwtToken.ValidTo.Day, utcNow.Day);
        Assert.Equal(jwtToken.ValidTo.Hour, utcNow.Hour);
        Assert.Equal(jwtToken.ValidTo.Minute, utcNow.Minute);
        Assert.Equal(jwtToken.ValidTo.Second, utcNow.Second);
    }

    [Fact]
    public void GenerateJwtToken_DeveRetornarOMesmoUsername_QuandoTokenForGerado()
    {
        // Arrange
        var username = "fakeUsername";
        var authenticationService = new AuthenticationService();
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Expires", "30" },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        // Act
        var generatedToken = authenticationService.GenerateJwtToken(username, inMemorySettings);
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(generatedToken);
        var subValue = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;

        // Assert
        Assert.StrictEqual(subValue, username);
    }

    [Fact]
    public void GeneratedJwtToken_DeveRetornarOMesmoIssuer_QuandoOTokenForGerado()
    {
        // Arrange
        var username = "fakeUsername";
        var authenticationService = new AuthenticationService();
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Expires", "30" },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        // Act
        var generateJwtToken = authenticationService.GenerateJwtToken(username, inMemorySettings);
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(generateJwtToken);
        var iss = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iss).Value;

        // Assert
        Assert.Equal(inMemorySettings["Issuer"], iss);
    }

    // Cenário: Valida se o "aud" (audience) é o mesmo após gerado o token
    [Fact]
    public void GenerateJwtToken_DeveRetornarOMesmoAudience_QuandoOTokenForGerado()
    {
        // Arrange
        var username = "fakeUsername";
        var authenticationService = new AuthenticationService();
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Expires", "30" },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        // Act
        var generateJwtToken = authenticationService.GenerateJwtToken(username, inMemorySettings);
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(generateJwtToken);
        var audience = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud).Value;

        // Assert
        Assert.Equal(inMemorySettings["Audience"], audience);
    }

    [Fact]
    public void GenerateJwtToken_DeveIncluirClaimJtiUnico_QuandoTokenForGerado()
    {
        // Arrange
        var authenticationService = new AuthenticationService();
        var username = "username1";
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Secret", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Expires", "Authentication:Expires" },
            { "Issuer", "Authentication:Issuer" },
            { "Audience", "Authentication:Audience" }
        };

        // Act
        var token = authenticationService.GenerateJwtToken(username, inMemorySettings);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var jtiClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);

        // Verifica se o claim "jti" existe
        Assert.NotNull(jtiClaim);

        // Verifica se o valor do "jti" é único (geralmente, isso será verdadeiro se a geração do GUID estiver correta)
        var jtiValue = jtiClaim?.Value;
        Assert.False(string.IsNullOrEmpty(jtiValue));

        // Testa se o valor de "jti" muda em chamadas subsequentes
        var newToken = authenticationService.GenerateJwtToken(username, inMemorySettings);
        var newJwtToken = handler.ReadJwtToken(newToken);
        var newJtiClaim = newJwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        Assert.NotEqual(jtiValue, newJtiClaim?.Value);
    }
}
