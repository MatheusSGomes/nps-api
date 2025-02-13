using System.Net;
using System.Text;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPS.Application;
using NPS.Application.Services;
using NPS.Core.Nps.ViewModels;

namespace NPS.Test.Acceptance;

public class NpsAcceptanceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Faker _faker;

    public NpsAcceptanceTests(WebApplicationFactory<Program> factory)
    {
        _faker = new Faker();
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
        var serializeObject =
            JsonConvert.SerializeObject(new { username = usernameInexistente, password = "password" });

        var content = new StringContent(serializeObject, Encoding.UTF8, mediaType);

        // Act
        var response = await _client.PostAsync(uri, content);

        // Assert
        Assert.False(response.IsSuccessStatusCode);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
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
            new
            {
                score = expectedScore, customerName = "Gerado pelo Teste de Aceitação",
                comment = "Gerado pelo Teste de Aceitação"
            });
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
            new
            {
                score = expectedScore, customerName = "Gerado pelo Teste de Aceitação",
                comment = "Gerado pelo Teste de Aceitação"
            });
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
        var objToSerialize = new
            { score = 5, customerName = "Customer Name", comment = "Gerado pelo Teste de Aceitação" };

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

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUnauthorized_QuandoTokenEnviadoForInvalido()
    {
        // Arrange
        string uri = "/v1/Nps/Responses";
        var tokenSettings = new Dictionary<string, string>()
        {
            { "Secret", Guid.NewGuid().ToString() },
            { "Expires", "ALEATÓRIO" },
            { "Issuer", "ALEATÓRIO" },
            { "Audience", "ALEATÓRIO" }
        };

        var authService = new AuthenticationService();
        var randomToken = authService.GenerateJwtToken("USERNAME_ALEATÓRIO", tokenSettings);

        // Act
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + randomToken);
        var response = await _client.GetAsync(uri);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUmErro_QuandoQueryParamDataForInvalida()
    {
        // Arrange
        string uri = "v1/Nps/Responses";
        string mediaType = "application/json";

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:SecretKey", "my_super_secret_key_E918128D-9D28-4156-AB70-9A8ADD1CA8C8" },
            { "Authentication:Issuer", "nps.com.br" },
            { "Authentication:Audience", "nps.com.br" },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["Data"] = "VALOR_INVÁLIDO";

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();
        var responseDeserialized = JsonConvert.DeserializeObject<ErrorResponse>(responseSerialized);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseClient.StatusCode);
        Assert.Contains("errors", responseSerialized);
        Assert.True(responseDeserialized.Errors.Count >= 1);
        Assert.Equal(responseDeserialized.Errors["Data"][0], "The value 'VALOR_INVÁLIDO' is not valid.");
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarResultados_QuandoQueryParamDataForValida()
    {
        // Arrange
        string uri = "v1/Nps/Responses";
        string mediaType = "application/json";

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:SecretKey", "my_super_secret_key_E918128D-9D28-4156-AB70-9A8ADD1CA8C8" },
            { "Authentication:Issuer", "nps.com.br" },
            { "Authentication:Audience", "nps.com.br" },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["Data"] = _faker.Date.PastDateOnly().ToString("yyyy-MM-dd");

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseClient.StatusCode);
        Assert.DoesNotContain(responseSerialized, "error");
        Assert.True(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarResultados_QuandoQueryParamCustomerNameForValido()
    {
        // Arrange
        string uri = "v1/Nps/Responses";
        string mediaType = "application/json";

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:SecretKey", "my_super_secret_key_E918128D-9D28-4156-AB70-9A8ADD1CA8C8" },
            { "Authentication:Issuer", "nps.com.br" },
            { "Authentication:Audience", "nps.com.br" },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["CustomerName"] = _faker.Person.FullName;

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseClient.StatusCode);
        Assert.DoesNotContain(responseSerialized, "error");
        Assert.True(responseClient.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Get_NpsResponses_DeveRetornarUmErro_QuandoQueryParamCategoryForInvalida()
    {
        // Arrange
        string uri = "v1/Nps/Responses";
        string mediaType = "application/json";

        var inMemorySettings = new Dictionary<string, string>
        {
            { "Authentication:SecretKey", "my_super_secret_key_E918128D-9D28-4156-AB70-9A8ADD1CA8C8" },
            { "Authentication:Issuer", "nps.com.br" },
            { "Authentication:Audience", "nps.com.br" },
            { "Authentication:Expires", "30" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var tokenJwt = new AuthenticationService()
            .SetUsername(_faker.Person.UserName).GenerateToken(configuration);

        // Act
        // Definindo a URL base
        UriBuilder uriBuilder = new UriBuilder(_client.BaseAddress)
        {
            Path = "/v1/Nps/Responses"
        };

        var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
        query["Category"] = _faker.Random.String();

        // Atualizando a URL com os parâmetros de consulta
        uriBuilder.Query = query.ToString();

        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenJwt);
        var responseClient = await _client.GetAsync(uriBuilder.Uri.PathAndQuery);

        string responseSerialized = await responseClient.Content.ReadAsStringAsync();
        var responseDeserialized = JsonConvert.DeserializeObject<ErrorResponse>(responseSerialized);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, responseClient.StatusCode);
        Assert.Contains("errors", responseSerialized);
        Assert.True(responseDeserialized.Errors.Count >= 1);
        Assert.Equal(responseDeserialized.Title, "One or more validation errors occurred.");
    }
    
    // TODAS PENDENTES DE TESTE:
    // GET - /v1/Nps/Responses
    // Cenário 1: Caso não exista token, deve retornar um 401 e a mensagem de Unauthorized - OK
    // Cenário 2: Caso o token de acesso seja inválido, deve retornar um 401 e a mensagem de Unauthorized - OK
    // Cenário 3: Ao preencher o parâmetro "Data" errado, é retornado o resultado algum erro -OK
    // Cenário 4: Ao preencher o parâmetro "Data" corretamente, é retornado o resultado correto - OK
    // Cenário 5: Ao preencher o parâmetro "CustomerName" corretamente, é retornado o resultado correto - OK
    // Cenário 6: Ao preencher o parâmetro "Category" errado, é retornado o resultado algum erro
    // Cenário 7: Ao preencher o parâmetro "Category" corretamente, é retornado o resultado correto
    // Cenário 8: Ao preencher todos os parâmetros Data e CustomerName, é retornado sucesso
    // Cenário 9: Ao preencher todos os parâmetros Data e Category, é retornado sucesso
    // Cenário 10: Ao preencher todos os parâmetros Category e CustomerName, é retornado sucesso
    // Cenário 11: Ao preencher todos os parâmetros corretamente, é retornado sucesso

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

    // POST - /api/Auth/login
    // Cenário 1 - Email e Senha errada, deve retornar um erro genérico "usuário ou senha inválido" e status - PENDENTE
    // Cenário 2 - Email errado, deve retornar um erro genérico "usuário ou senha inválido" e status - OK
    // Cenário 3 - Senha errada, deve retornar um erro genérico "usuário ou senha inválido" e status - OK
    // Cenário 4 - Dados de acesso corretos - Deve retornar status de sucesso + Token - OK
}
