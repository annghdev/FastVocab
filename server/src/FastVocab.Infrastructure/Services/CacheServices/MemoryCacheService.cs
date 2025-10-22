using FastVocab.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace FastVocab.Infrastructure.Services.CacheServices;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (_memoryCache.TryGetValue(key, out var cachedValue))
        {
            return Task.FromResult((T?)cachedValue);
        }

        return Task.FromResult<T?>(null);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var options = new MemoryCacheEntryOptions();

        if (slidingExpiration.HasValue)
        {
            options.SlidingExpiration = slidingExpiration.Value;
        }
        else
        {
            // Default sliding expiration: 5 minutes
            options.SlidingExpiration = TimeSpan.FromMinutes(5);
        }

        _memoryCache.Set(key, value, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }
}
