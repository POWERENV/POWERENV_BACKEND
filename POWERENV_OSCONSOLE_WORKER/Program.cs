using StackExchange.Redis;

namespace POWERENV_OSCONSOLE_WORKER
{
    public class Program
    {
        private static int maxReddisConnectionAttempts = 5;

        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();


            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();

                ConnectionMultiplexer redisCache = null;

                // Asynchronously connect to Redis cache with retry mechanism, and wait for the connection to be established before proceeding with the application startup. This ensures that the worker service has a valid Redis connection when it starts executing.
                Task.Run(() => { redisCache = connectToRedisCache(config, 1).Result; }).Wait();

                return redisCache;
            });

            var host = builder.Build();
            host.Run();
        }

        /// <summary>
        /// This recursive method tries to connect to Redis Cache, and if it fails, it retries up to 5 times (by default) before giving up and returning a null ConnectionMultiplexer object.
        /// </summary>
        /// <param name="config">IServiceProvider config object, necessary for Redis connection with connection string.</param>
        /// <param name="numTries">Current connection attempts count.</param>
        /// <returns></returns>
        private static async Task<ConnectionMultiplexer> connectToRedisCache(IConfiguration config, int numTries)
        {
            ConnectionMultiplexer redisCache;
            try
            {
                redisCache = ConnectionMultiplexer.Connect(config.GetConnectionString("RedisCache"));
                Console.WriteLine("Connected to Redis cache successfully.");
            }
            catch (Exception ex)
            {
                if(numTries < maxReddisConnectionAttempts)
                {
                    Console.WriteLine($"Failed to connect to Redis cache. Attempt {numTries}/{maxReddisConnectionAttempts}. Retrying...");
                    await Task.Delay(1000); // Wait for 2 seconds before retrying
                    redisCache = connectToRedisCache(config, numTries + 1).Result;
                }
                else
                {
                    Console.WriteLine($"Error connecting to Redis cache: {ex.Message}");
                    return null;
                }
            }

            return redisCache;
        }
    }
}