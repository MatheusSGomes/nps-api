using NPS.Core.Enums;

namespace NPS.Core.Entities;

public class Nps : BaseEntity
{
    // A pontuação do NPS, que varia de -100 a +100
    public int Score { get; private set; }

    // Nome do cliente que respondeu à pesquisa (opcional)
    public string CustomerName { get; private set; }

    // Comentário do cliente, fornecido como feedback adicional (opcional)
    public string Comment { get; private set; }

    // Categoria da pesquisa (ex.: "Atendimento ao Cliente", "Produto", etc.)
    public Category Category { get; private set; }

    private Nps(int score, string customerName, string comment)
    {
        Score = score;
        CustomerName = customerName;
        Comment = comment;
        Category = Classify();
    }

    public static Nps CreateResponse(int score, string customerName, string comment)
    {
        if (string.IsNullOrEmpty(customerName))
            throw new Exception(NpsErrorMessage.CustomerNameIsNullOrEmpty);
            
        return new(score, customerName, comment);
    }

    private Category Classify()
    {
        if (Score > 0 && Score < 7)
        {
            Category = Category.Detractor;
            return Category;
        }

        if (Score >= 7 && Score <= 8)
        {
            Category = Category.Neutral;
            return Category;
        }

        if (Score > 8 && Score <= 10)
        {
            Category = Category.Promoter;
            return Category;
        }

        throw new Exception(NpsErrorMessage.InvalidScore);
    }
}
