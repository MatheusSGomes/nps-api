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
}
