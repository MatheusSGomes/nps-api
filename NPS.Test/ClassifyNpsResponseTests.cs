using Bogus;
using NPS.Core;
using NPS.Core.Entities;
using NPS.Core.Enums;

namespace NPS.Test;

public class ClassifyNpsResponseTests
{
    private readonly Faker _faker;

    public ClassifyNpsResponseTests()
    {
        _faker = new Faker();
    }

    [Theory]
    [InlineData(1, Category.Detractor)]
    [InlineData(2, Category.Detractor)]
    [InlineData(3, Category.Detractor)]
    [InlineData(4, Category.Detractor)]
    [InlineData(5, Category.Detractor)]
    [InlineData(6, Category.Detractor)]
    [InlineData(7, Category.Neutral)]
    [InlineData(8, Category.Neutral)]
    [InlineData(9, Category.Promoter)]
    [InlineData(10, Category.Promoter)]
    public void DeveClassificarRespostasSegundoNotaAtribuida(int respostaUsuario, Category classificacaoEsperada)
    {
        var resposta = Nps.CreateResponse(respostaUsuario, _faker.Person.FullName, _faker.Lorem.Sentence());
        Assert.True(resposta.Category.Equals(classificacaoEsperada));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(11)]
    public void NaoDeveAtribuirNotaComClassificacaoInvalida(int respostaInvalidaUsuario)
    {
        var msg = Assert.Throws<Exception>(() =>
                Nps.CreateResponse(respostaInvalidaUsuario,
                    _faker.Person.FullName,
                    _faker.Lorem.Sentence()))
            .Message;

        Assert.Equal(NpsErrorMessage.InvalidScore, msg);
    }
}
