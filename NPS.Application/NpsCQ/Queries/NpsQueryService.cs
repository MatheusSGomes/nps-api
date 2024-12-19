using NPS.Application.NpsCQ.ViewModels;
using NPS.Infrastructure.Data.Queries;

namespace NPS.Application.NpsCQ.Queries;

public class NpsQueryService : INpsQueryService
{
    private readonly INpsQuery _npsQuery;

    public NpsQueryService(INpsQuery npsQuery)
    {
        _npsQuery = npsQuery;
    }

    public async Task<NpsScoreViewModel> GetNpsScore()
    {
        var score = await _npsQuery.GetNpsScore();
        return new NpsScoreViewModel(score);
    }
}
