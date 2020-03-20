using StackExchange.Redis;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisConnectionSample
{
    class Program
    {
        const string OUR_NODE = "YOUR NODE HERE";
        const string PRIMARY_ACCESS_KEY = "YOUR PASSWORD HERE";
        const string testKey = "testKey";
        const string testValue = "testValue";
        const string REDIS_CONNECTION_STRING = "YOUR CONNECTION STRING HERE";

        #region Properties and Fields
        private static Lazy<ConnectionMultiplexer> redisConnection = GetNewRedisConnection();
        public static LazyThreadSafetyMode LazyThreadSafetyMode = LazyThreadSafetyMode.None;
        private static Lazy<ConnectionMultiplexer> GetNewRedisConnection()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(REDIS_CONNECTION_STRING);
            }, LazyThreadSafetyMode);
        }

        public static IDatabase Cache
        {
            get
            {
                return Connection.GetDatabase();
            }
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return redisConnection.Value;
            }
            set
            {
                if (value == null)
                    redisConnection = GetNewRedisConnection();
            }
        }
        #endregion

        async static Task Main()
        {
            await ConnectionMethod1();
            await ConnectionMethod2();
        }

        private static async Task ConnectionMethod1()
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { $"{OUR_NODE}.redis.cache.windows.net" },
                Password = PRIMARY_ACCESS_KEY,
                Ssl = true
            };
            Console.WriteLine(config); // equivalent if using the string overload
                                       // looks like: "REDACTED.redis.cache.windows.net,password=REDACTED,ssl=True"

            using var muxer = await ConnectionMultiplexer.ConnectAsync(config);
            var db = muxer.GetDatabase();

            RedisKey key = "abc";
            await db.KeyDeleteAsync(key);
            await db.StringIncrementAsync(key);
            await db.StringIncrementAsync(key, 42);
            var val = (int)await db.StringGetAsync(key);

            Console.WriteLine(val); // 43
        }

        private static async Task ConnectionMethod2()
        {
            Connection.GetDatabase().Ping();
            Cache.StringSet(testKey, testValue);
            var result = Cache.GetBytes(testKey);
            Console.WriteLine(Encoding.UTF8.GetString(result));
        }
    }
}
