using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using NPS.Core.Nps.ViewModels;

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

    [Fact]
    public async Task Post_NpsResponses_DeveRetornarUmErro_QuandoCustomerNameForVazio()
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        string emptyExpectedUsername = "";
        var serializedObject = JsonConvert.SerializeObject(
            new { score = 5, customerName = emptyExpectedUsername, comment = "Gerado pelo Teste de Aceitação" });
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        string expectedContent = "Customer name is null or empty";
        Assert.Contains(expectedContent, responseBody);
    }

    [Fact]
    public async Task Post_NpsResponses_DeveRetornarSucesso_QuandoTodosOsDadosEstiveremOk()
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        string mediaType = "application/json";
        var objToSerialize = new { score = 5, customerName = "Customer Name", comment = "Gerado pelo Teste de Aceitação" };

        var serializedObject = JsonConvert.SerializeObject(objToSerialize);
        var content = new StringContent(serializedObject, Encoding.UTF8, mediaType);

        // Act
        HttpResponseMessage response = await _client.PostAsync(uri, content);
        string responseBody = await response.Content.ReadAsStringAsync();
        var npsResponse = JsonConvert.DeserializeObject<NpsResponseViewModel>(responseBody);

        // Assert
        Assert.Equal(objToSerialize.score, npsResponse.Score);
        Assert.Equal(objToSerialize.customerName, npsResponse.CustomerName);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUnauthorized_QuandoNenhumTokenForEnviado()
    {
        // Arrange
        var uri = "/v1/Nps/Responses";

        // Act
        var response = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // TODAS PENDENTES DE TESTE:
    // GET - /v1/Nps/Responses
    // Cenário 0: Caso o token de acesso seja inválido, deve retornar um 401 e a mensagem de Unauthorized
    // Cenário 1: Ao preencher o parâmetro "Data" errado, é retornado o resultado algum erro
    // Cenário 2: Ao preencher o parâmetro "Data" corretamente, é retornado o resultado correto
    // Cenário 3: Ao preencher o parâmetro "CustomerName" errado, é retornado o resultado algum erro
    // Cenário 4: Ao preencher o parâmetro "CustomerName" corretamente, é retornado o resultado correto
    // Cenário 5: Ao preencher o parâmetro "Category" errado, é retornado o resultado algum erro
    // Cenário 6: Ao preencher o parâmetro "Category" corretamente, é retornado o resultado correto
    // Cenário 7: Ao preencher todos os parâmetros Data e CustomerName, é retornado sucesso
    // Cenário 8: Ao preencher todos os parâmetros Data e Category, é retornado sucesso
    // Cenário 9: Ao preencher todos os parâmetros Category e CustomerName, é retornado sucesso
    // Cenário 10: Ao preencher todos os parâmetros corretamente, é retornado sucesso

    // GET - /v1/Nps/Score
    // Cenário 0: Caso o token de acesso seja inválido, deve retornar um 401 e a mensagem de Unauthorized
    // Cenário 1: Valida se o objeto retornado contém a chave "score".
    // {
    //   "score": 0
    // }

    // GET - /v1/Nps/Summary
    // Cenário 0: Caso o token de acesso seja inválido, deve retornar um 401 e a mensagem de Unauthorized
    // Cenário 1: Valida se o objeto retornado contém as chaves "promoters", "neutrals", "detractors" e "npsScore".
    // {
    //   "promoters": 0,
    //   "neutrals": 0,
    //   "detractors": 0,
    //   "npsScore": 0
    // }
}
