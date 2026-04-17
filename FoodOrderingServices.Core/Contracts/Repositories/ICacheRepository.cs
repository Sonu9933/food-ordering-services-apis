namespace FoodOrderingServices.Core.Contracts.Repositories
{

    /// <summary>
    /// Interface for cache repository operations.
    /// </summary>
    public interface ICacheRepository
    {
        /// <summary>
        /// Retrieves a cached object by key with automatic deserialization.
        /// Returns null if the key is not found or cache is unavailable.
        /// </summary>
        Task<T?> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// Stores an object in cache with automatic serialization and default TTL.
        /// Silently fails if cache is unavailable.
        /// </summary>
        Task SetAsync<T>(string key, T? value, TimeSpan? expiration = null) where T : class;

        /// <summary>
        /// Removes an entry from cache. Silently fails if cache is unavailable.
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Refreshes the cache entry expiration time. Silently fails if key doesn't exist or cache is unavailable.
        /// </summary>
        Task RefreshAsync(string key);

        /// <summary>
        /// Checks if a key exists in the cache.
        /// </summary>
        Task<bool> ExistsAsync(string key);
    }
}
