namespace NPS.Test;

// TODO: Implementar serviço para categorizar resposta em Promotor, Neutro ou Detrator.

public class ClassificadorDeRespostaTest
{
    [Fact]
    public void ClassificaResposta()
    {
        var atualRespostaUsuario = 1;
        string resultadoEsperado = Classificacao.Detrator.ToString();

        var resposta = new Resposta(atualRespostaUsuario);

        Assert.Equal(resultadoEsperado, resposta.Classificar());
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

        if (_respostaUsuario >= 7 && _respostaUsuario < 8)
        {
            return Classificacao.Neutro.ToString();
        }

        if (_respostaUsuario >= 8 && _respostaUsuario <= 10)
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
