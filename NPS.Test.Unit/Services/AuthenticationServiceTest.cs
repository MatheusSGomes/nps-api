using System.IdentityModel.Tokens.Jwt;
using Bogus;
using Microsoft.Extensions.Configuration;
using NPS.Application.Services;

namespace NPS.Test.Services;

public class AuthenticationServiceTest
{
    private readonly Faker _faker;

    public AuthenticationServiceTest()
    {
        _faker = new Faker();
    }

    [Fact]
    public void AuthenticationService_DeveImplementarUmaInterface()
    {
        // Arrange
        var authenticationService = new AuthenticationService();

        // Act & Assert
        Assert.IsAssignableFrom<IAuthenticationService>(authenticationService);
    }

    [Fact]
    public void GenerateJwtToken_DeveLancarExcecao_QuandoSecretSettingsForNula()
    {
        // Arrange
        var username = _faker.Person.UserName;
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:SecretKey", null },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new AuthenticationService().SetUsername(username).GenerateToken(configuration));
        Assert.Equal("Chave secreta inválida ou vazia", exception.ParamName);
    }

    [Fact]
    public void GenerateJwtToken_DeveLancarExcecao_QuandoSecretKeyForMenorQue256Bits()
    {
        // Arrange
        var username = _faker.Person.UserName;
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Authentication:SecretKey", "MINHA_CHAVE_SECRETA_INVALIDA" },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new AuthenticationService().SetUsername(username).GenerateToken(configuration));
        Assert.Equal("Chave secreta menor que 256 bits (32 bytes/caracteres)", exception.ParamName);
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
            { "Authentication:SecretKey", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", expireTimeInMinutes.ToString() },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var generatedToken = authenticationService.SetUsername(username).GenerateToken(configuration);

        // Act
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
            { "Authentication:SecretKey", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var generatedToken = authenticationService.SetUsername(username).GenerateToken(configuration);
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
            { "Authentication:SecretKey", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var generateJwtToken = authenticationService.SetUsername(username).GenerateToken(configuration);
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(generateJwtToken);
        var iss = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iss).Value;

        // Assert
        Assert.Equal(configuration["Authentication:Issuer"], iss);
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
            { "Authentication:SecretKey", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var generateJwtToken = authenticationService.SetUsername(username).GenerateToken(configuration);
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(generateJwtToken);
        var audience = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud).Value;

        // Assert
        Assert.Equal(configuration["Authentication:Audience"], audience);
    }

    [Fact]
    public void GenerateJwtToken_DeveIncluirClaimJtiUnico_QuandoTokenForGerado()
    {
        // Arrange
        var authenticationService = new AuthenticationService();
        var username = _faker.Person.UserName;
        var inMemorySettings = new Dictionary<string, string>()
        {
            { "Authentication:SecretKey", "MINHA_CHAVE_SECRETA_A53D39BF-80CF-46F3-BFBB-7A3B69F33D17" },
            { "Authentication:Issuer", _faker.Random.Hash() },
            { "Authentication:Audience", _faker.Random.Hash() },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var token = authenticationService.SetUsername(username).GenerateToken(configuration);

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
        var newToken = authenticationService.SetUsername(username).GenerateToken(configuration);
        var newJwtToken = handler.ReadJwtToken(newToken);
        var newJtiClaim = newJwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
        Assert.NotEqual(jtiValue, newJtiClaim?.Value);
    }
}
