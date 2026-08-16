using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace InsightHub.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache? _distributedCache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(
        IMemoryCache memoryCache,
        ILogger<CacheService> logger,
        IDistributedCache? distributedCache = null)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_memoryCache.TryGetValue(key, out T? memoryValue))
            {
                _logger.LogInformation("⚡ [CACHE HIT - MEMORY] Key: {Key}", key);
                return memoryValue;
            }

            if (_distributedCache != null)
            {
                var cachedData = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogInformation("⚡ [CACHE HIT - REDIS] Key: {Key}", key);
                    var deserialized = JsonSerializer.Deserialize<T>(cachedData);
                    if (deserialized != null)
                    {
                        _memoryCache.Set(key, deserialized, TimeSpan.FromMinutes(5));
                    }
                    return deserialized;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache okuma hatası: {Key}", key);
        }

        _logger.LogInformation("❌ [CACHE MISS] Key: {Key}", key);
        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, CancellationToken cancellationToken = default)
    {
        if (value == null) return;

        var expiration = absoluteExpireTime ?? TimeSpan.FromMinutes(10);

        try
        {
            _memoryCache.Set(key, value, expiration);

            if (_distributedCache != null)
            {
                var json = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration
                };
                await _distributedCache.SetStringAsync(key, json, options, cancellationToken);
            }

            _logger.LogInformation("💾 [CACHE SET] Key: {Key} (Süre: {Expiry} dk)", key, expiration.TotalMinutes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache yazma hatası: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _memoryCache.Remove(key);
            if (_distributedCache != null)
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }
            _logger.LogInformation("🗑️ [CACHE REMOVE] Key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache silme hatası: {Key}", key);
        }
    }
}
