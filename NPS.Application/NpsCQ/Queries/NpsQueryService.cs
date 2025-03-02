using NPS.Application.Services;
using NPS.Core.Nps.Filters;
using NPS.Core.Nps.ViewModels;
using NPS.Infrastructure.Data.Queries;

namespace NPS.Application.NpsCQ.Queries;

public class NpsQueryService : INpsQueryService
{
    private readonly INpsQuery _npsQuery;
    private readonly ICacheService _cacheService;

    public NpsQueryService(INpsQuery npsQuery, ICacheService cacheService)
    {
        _npsQuery = npsQuery;
        _cacheService = cacheService;
    }

    public async Task<NpsScoreViewModel> GetNpsScore()
    {
        var cacheKey = $"npsScore";
        var cachedEntity = await _cacheService.GetFromCacheAsync<string>(cacheKey);

        if (cachedEntity != null)
            return new NpsScoreViewModel(cachedEntity);

        string score = await _npsQuery.GetNpsScore();
        string scoreWithoutFloat = score.Split(".").FirstOrDefault();

        if (scoreWithoutFloat != null)
            await _cacheService.SetToCacheAsync(cacheKey, scoreWithoutFloat);

        return new NpsScoreViewModel(scoreWithoutFloat);
    }

    public async Task<IEnumerable<NpsFullResponseViewModel>> GetNpsResponses(NpsFilters filters)
    {
        return await _npsQuery.GetNpsResponses(filters);
    }

    public async Task<NpsSummaryViewModel> GetNpsSummary()
    {
        var cacheKey = $"npsSummary";
        var cachedSummary = await _cacheService.GetFromCacheAsync<NpsSummaryViewModel>(cacheKey);

        if (cachedSummary != null)
            return cachedSummary;

        var summary = await _npsQuery.GetNpsSummary();

        if (summary != null)
            await _cacheService.SetToCacheAsync(cacheKey, summary);

        return summary;
    }
}
