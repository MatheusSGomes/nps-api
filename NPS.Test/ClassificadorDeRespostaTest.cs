using NPS.Core;
using NPS.Core.Entities;

namespace NPS.Test;

public class ClassificadorDeRespostaTest
{
    [Theory]
    [InlineData(1, "Detrator")]
    [InlineData(2, "Detrator")]
    [InlineData(3, "Detrator")]
    [InlineData(4, "Detrator")]
    [InlineData(5, "Detrator")]
    [InlineData(6, "Detrator")]
    [InlineData(7, "Neutro")]
    [InlineData(8, "Neutro")]
    [InlineData(9, "Promotor")]
    [InlineData(10, "Promotor")]
    public void DeveClassificarRespostasSegundoNotaAtribuida(int respostaUsuario, string classificacaoEsperada)
    {
        var resposta = new NpsResponse(
            respostaUsuario, "Customer Name", "Customer Comment", "Customer Category");
        Assert.Equal(classificacaoEsperada, resposta.Classify());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(11)]
    public void NaoDeveAtribuirNotaComClassificacaoInvalida(int respostaInvalidaUsuario)
    {
        var response = new NpsResponse(respostaInvalidaUsuario,
            "Customer Name", "Customer Comment", "Customer Category");

        var msg = Assert.Throws<Exception>(() => response.Classify()).Message;

        Assert.Equal(NpsErrorMessage.InvalidScore, msg);
    }
}
