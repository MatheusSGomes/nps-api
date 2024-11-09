namespace NPS.Core.Entities;

public class NpsResponse
{
    // A pontuação do NPS, que varia de -100 a +100
    public int Score { get; set; }
    // Nome do cliente que respondeu à pesquisa (opcional)
    public string CustomerName { get; set; }
    // Comentário do cliente, fornecido como feedback adicional (opcional)
    public string Comment { get; set; }
    // Categoria da pesquisa (ex.: "Atendimento ao Cliente", "Produto", etc.)
    public string Category { get; set; }

    public NpsResponse(int score, string customerName, string comment, string category)
    {
        Score = score;
        CustomerName = customerName;
        Comment = comment;
        Category = category;
    }
}