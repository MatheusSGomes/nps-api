namespace NPS.Application.Services;

public interface ICacheService
{
    Task<T> GetFromCacheAsync<T>(string key);
    Task SetToCacheAsync<T>(string key, T value);
}
