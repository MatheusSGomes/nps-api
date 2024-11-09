namespace NPS.Test;

// TODO: Implementar serviço para categorizar resposta em Promotor, Neutro ou Detrator.

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
        var resposta = new Resposta(respostaUsuario);
        Assert.Equal(classificacaoEsperada, resposta.Classificar());
    }
}

public class Resposta
{
    private readonly int _respostaUsuario;

    public Resposta(int respostaUsuario)
    {
        _respostaUsuario = respostaUsuario;
    }

    public string Classificar()
    {
        if (_respostaUsuario > 0 && _respostaUsuario < 7)
        {
            return Classificacao.Detrator.ToString();
        }

        if (_respostaUsuario >= 7 && _respostaUsuario <= 8)
        {
            return Classificacao.Neutro.ToString();
        }

        if (_respostaUsuario > 8 && _respostaUsuario <= 10)
        {
            return Classificacao.Promotor.ToString();
        }

        return "Inválido";
    }
}

enum Classificacao
{
    Detrator = 1,
    Neutro = 2,
    Promotor = 3,
}
