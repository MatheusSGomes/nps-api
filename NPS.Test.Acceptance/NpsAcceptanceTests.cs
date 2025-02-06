using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace NPS.Test.Acceptance;

public class NpsAcceptanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public NpsAcceptanceTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.BaseAddress = new Uri("http://localhost:5115");
    }

    [Theory]
    [InlineData("/swagger")]
    [InlineData("/swagger/index.html")]
    public async Task Get_SwaggerIndex_DeveRetornarSucesso_QuandoContentTypeEstiverCorreto(string uri)
    {
        // Arrange & Act
        var response = await _client.GetAsync(uri);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }

    [Fact]
    public async Task Post_AuthLogin_DeveRetornarUnauthorized_CasoUsernameEstejaIncorreto()
    {
        // Arrange
        string uri = "/api/Auth/login";
        string mediaType = "application/json";
        string usernameInexistente = "USERNAME_INEXISTENTE";
        var serializeObject = JsonConvert.SerializeObject(new { username = usernameInexistente, password = "password" });

        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        Assert.False(response.IsSuccessStatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("Unauthorized", responseBody);
        Assert.Contains("401", responseBody);
    }

    [Fact]
    public async Task Post_AuthLogin_DeveRetornarUnauthorized_CasoPasswordEstejaIncorreto()
    {
        // Arrange
        string uri = "/api/Auth/login";
        string mediaType = "application/json";
        string wrongPassword = "WRONG_PASSWORD";
        var serializeObject = JsonConvert.SerializeObject(new { username = "admin", password = wrongPassword });

        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        Assert.False(response.IsSuccessStatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("Unauthorized", responseBody);
        Assert.Contains("401", responseBody);
    }

    [Fact]
    public async Task Post_AuthLogin_DeveRetornarToken_QuandoCredenciaisEstejamCorretas()
    {
        // Arrange
        string uri = "/api/Auth/login";
        string mediaType = "application/json";
        var serializeObject = JsonConvert.SerializeObject(new { username = "admin", password = "password" });

        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se a resposta foi 2xx
        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", responseBody);
    }

    // Cenários:
    // Cenário 1: Passar Score menor que 0 (número negativo) -> Erro
    // Cenário 2: Passar Score maior que 10 -> Erro
    // Cenário 3: Não passar CustomerName -> Erro
    // Cenário 4: Todos os parâmetros corretos -> Sucesso

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public async Task Post_NpsResponses_DeveRetornarErro_QuandoScoreForMenorOuIgualZero(int expectedScore)
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        var serializedObject = JsonConvert.SerializeObject(
            new { score = expectedScore, customerName = "Gerado pelo Teste de Aceitação", comment = "Gerado pelo Teste de Aceitação" });
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        string expectedContent = "Invalid Score";
        Assert.Contains(expectedContent, responseBody);
    }

    [Theory]
    [InlineData(11)]
    [InlineData(20)]
    [InlineData(30)]
    public async Task Post_NpsResponses_DeveRetornarUmErro_QuandoScoreForMaiorQue10(int expectedScore)
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        var serializedObject = JsonConvert.SerializeObject(
            new { score = expectedScore, customerName = "Gerado pelo Teste de Aceitação", comment = "Gerado pelo Teste de Aceitação" });
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        string expectedContent = "Invalid Score";
        Assert.Contains(expectedContent, responseBody);
    }

    // [Fact]
    // public async Task SubmitNps_ReturnsSuccess()
    // {
    //     // Arrange
    //     var content = new StringContent(JsonConvert.SerializeObject(new { UserId = 1, Score = 9 }), Encoding.UTF8, "application/json");
    //
    //     // Act
    //     var response = await _client.PostAsync("/api/nps/submit", content); // Supondo que existe um endpoint POST em "/api/nps/submit"
    //
    //     // Assert
    //     response.EnsureSuccessStatusCode(); // Verifica se a resposta foi 2xx
    //     var responseBody = await response.Content.ReadAsStringAsync();
    //     Assert.Contains("Success", responseBody); // Verifica se a resposta contém "Success" (exemplo de resposta)
    // }
}
