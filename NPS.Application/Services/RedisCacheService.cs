using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace NPS.Application.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<T> GetFromCacheAsync<T>(string key)
    {
        var data = await _distributedCache.GetStringAsync(key);
        return data == null ? default : JsonConvert.DeserializeObject<T>(data);
    }

    public async Task SetToCacheAsync<T>(string key, T value)
    {
        var data = JsonConvert.SerializeObject(value);
        await _distributedCache.SetStringAsync(key, data);
    }
}
