using StackExchange.Redis;

namespace RedisConnectionSample
{
    public static class CustomExtensions
    {
        public static byte[] GetBytes(this IDatabase cache, string key)
        {
            return cache.StringGet(key);
        }
    }
}
