
using global::FoodOrderingServices.Core.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace FoodOrderingServices.Infrastructure.Repositories
{
    /// <summary>
    /// Redis implementation of cache repository.
    /// </summary>
    public class RedisRepository : ICacheRepository
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisRepository> _logger;
        private readonly TimeSpan _defaultExpiration;

        public RedisRepository(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisRepository> logger)
        {
            _connectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultExpiration = TimeSpan.FromMinutes(5);
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Cache key cannot be null or empty.");
                return null;
            }

            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                var value = await db.StringGetAsync(key);

                if (!value.HasValue)
                {
                    return null;
                }

                return JsonSerializer.Deserialize<T>(value.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve cache key '{CacheKey}'. Falling back to database.", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T? value, TimeSpan? expiration = null) where T : class
        {
            if (value == null || string.IsNullOrEmpty(key))
            {
                return;
            }

            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                var serialized = JsonSerializer.Serialize(value);
                var ttl = expiration ?? _defaultExpiration;

                await db.StringSetAsync(key, serialized, ttl);

                _logger.LogDebug("Cache key '{CacheKey}' set successfully with TTL of {TtlMinutes} minutes.", key, ttl.TotalMinutes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to set cache key '{CacheKey}'. Continuing without cache.", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                await db.KeyDeleteAsync(key);

                _logger.LogDebug("Cache key '{CacheKey}' removed successfully.", key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove cache key '{CacheKey}'.", key);
            }
        }

        public async Task RefreshAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                var ttl = await db.KeyTimeToLiveAsync(key);

                if (ttl.HasValue && ttl.Value > TimeSpan.Zero)
                {
                    await db.KeyExpireAsync(key, _defaultExpiration);
                    _logger.LogDebug("Cache key '{CacheKey}' refreshed successfully.", key);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to refresh cache key '{CacheKey}'.", key);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                return await db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to check if cache key '{CacheKey}' exists.", key);
                return false;
            }
        }
    }
}

