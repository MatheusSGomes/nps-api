using NPS.Core.Enums;

namespace NPS.Core.Entities;

public class NpsResponse
{
    // A pontuação do NPS, que varia de -100 a +100
    private int _score { get; set; }

    // Nome do cliente que respondeu à pesquisa (opcional)
    private string _customerName { get; set; }

    // Comentário do cliente, fornecido como feedback adicional (opcional)
    private string _comment { get; set; }

    // Categoria da pesquisa (ex.: "Atendimento ao Cliente", "Produto", etc.)
    private string _category { get; set; }

    public NpsResponse(int score, string customerName, string comment, string category)
    {
        // Validation
        if (score <= 0 || score >= 11)
            throw new Exception(NpsErrorMessage.InvalidScore);

        _score = score;
        _customerName = customerName;
        _comment = comment;
        _category = category;
    }

    public string Classify()
    {
        if (_score > 0 && _score < 7)
        {
            return Classificacao.Detrator.ToString();
        }

        if (_score >= 7 && _score <= 8)
        {
            return Classificacao.Neutro.ToString();
        }

        if (_score > 8 && _score <= 10)
        {
            return Classificacao.Promotor.ToString();
        }

        throw new Exception(NpsErrorMessage.InvalidScore);
    }
}
