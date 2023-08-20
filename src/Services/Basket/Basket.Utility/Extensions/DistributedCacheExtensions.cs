using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.Utility.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            DistributedCacheEntryOptions options = new()
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime,
                SlidingExpiration = unusedExpireTime
            };
            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json, options);
        }

        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
        {
            var json = await cache.GetStringAsync(key);
            return json is null ? default : JsonSerializer.Deserialize<T>(json);
        }

    }
}
