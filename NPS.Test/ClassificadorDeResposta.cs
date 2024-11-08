namespace NPS.Test;

// TODO: Implementar serviço para categorizar resposta em Promotor, Neutro ou Detrator.

public class ClassificadorDeResposta
{
    [Fact]
    public void ClassificaResposta()
    {
        var atualRespostaUsuario = 1;
        string resultado = "Inválido";
        string resultadoEsperado = Classificacao.Detrator.ToString();

        switch (atualRespostaUsuario)
        {
            case (int) Classificacao.Detrator:
                resultado = Classificacao.Detrator.ToString();
                break;
            case (int) Classificacao.Neutro:
                resultado = Classificacao.Neutro.ToString();
                break;
            case (int) Classificacao.Promotor:
                resultado = Classificacao.Promotor.ToString();
                break;
        }

        Assert.Equal(resultadoEsperado, resultado);
    }
}

enum Classificacao
{
    Detrator = 1,
    Neutro = 2,
    Promotor = 3,
}
