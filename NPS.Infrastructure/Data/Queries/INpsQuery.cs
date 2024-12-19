namespace NPS.Infrastructure.Data.Queries;

public interface INpsQuery
{
    Task<int> GetNpsScore();
}
