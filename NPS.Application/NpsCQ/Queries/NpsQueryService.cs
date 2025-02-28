using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NPS.Core.Entities;
using NPS.Core.Nps.Filters;
using NPS.Core.Nps.ViewModels;
using NPS.Infrastructure.Data.Queries;

namespace NPS.Application.NpsCQ.Queries;

public class NpsQueryService : INpsQueryService
{
    private readonly INpsQuery _npsQuery;
    private readonly IDistributedCache _distributedCache;

    public NpsQueryService(INpsQuery npsQuery, IDistributedCache distributedCache)
    {
        _npsQuery = npsQuery;
        _distributedCache = distributedCache;
    }

    public async Task<NpsScoreViewModel> GetNpsScore()
    {
        int resScore;
        string cacheKey = "nps";
        string serializedNps;

        var redisNps = await _distributedCache.GetAsync(cacheKey);

        if (redisNps is not null)
        {
            serializedNps = Encoding.UTF8.GetString(redisNps);
            resScore = JsonConvert.DeserializeObject<int>(serializedNps)!;
        }
        else
        {
            resScore = await _npsQuery.GetNpsScore();

            // Set Cache
            serializedNps = JsonConvert.SerializeObject(resScore);
            redisNps = Encoding.UTF8.GetBytes(serializedNps);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            await _distributedCache.SetAsync(cacheKey, redisNps, options);
        }

        return new NpsScoreViewModel(resScore);
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
