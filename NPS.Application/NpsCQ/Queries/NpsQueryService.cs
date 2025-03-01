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
        var cachedEntity = await _cacheService.GetFromCacheAsync<NpsScoreViewModel>(cacheKey);

        if (cachedEntity != null)
            return cachedEntity;

        var score = await _npsQuery.GetNpsScore();

        if (score != null)
            await _cacheService.SetToCacheAsync(cacheKey, cachedEntity);

        return new NpsScoreViewModel(score);
    }

    public async Task<IEnumerable<NpsFullResponseViewModel>> GetNpsResponses(NpsFilters filters)
    {
        return await _npsQuery.GetNpsResponses(filters);
    }

    public async Task<NpsSummaryViewModel> GetNpsSummary()
    {
        return await _npsQuery.GetNpsSummary();
    }
}
