using NPS.Core.Entities;

namespace NPS.Test;

public class ClassificadorDeRespostaTest
{
    [Theory]
    [InlineData(0, "Inválido")]
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
    [InlineData(11, "Inválido")]
    public void ClassificaResposta(int respostaUsuario, string classificacaoEsperada)
    {
        var resposta = new NpsResponse(
            respostaUsuario, "Customer Name", "Customer Comment", "Customer Category");
        Assert.Equal(classificacaoEsperada, resposta.Classify());
    }
}
