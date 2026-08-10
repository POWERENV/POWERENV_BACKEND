using Microsoft.AspNetCore.SignalR;
using POWERENV_BACKEND_API.SignalR;
using POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER;
using StackExchange.Redis;

namespace POWERENV_BACKEND_API
{
    public class TaskGatewayBKGService : BackgroundService
    {
        //Request Context
        private readonly IHubContext<OS_TERMINAL_WSS_HUB> _hubContext;

        //Redis
        private readonly IConnectionMultiplexer _redis;
        private IDatabase redisDB;
        string redisTerminalOutputStream;
        string redisAPIReaderConsumerGroup;

        /// <summary>
        /// TaskGatewayBKGService class constructor.
        /// </summary>
        /// <param name="redis">Orchestrated Redis connection linker object (IConnectionMultiplexer).</param>
        /// <param name="hubContext">SignalR terminal hub context reference.</param>
        public TaskGatewayBKGService(IConnectionMultiplexer redis, IHubContext<OS_TERMINAL_WSS_HUB> hubContext)
        {
            _hubContext = hubContext;
            _redis = redis;
            redisDB = _redis.GetDatabase();
            redisTerminalOutputStream = "terminal-output";
            redisAPIReaderConsumerGroup = "wss-api-reader-group";

            Task.Run(configureRedis);
        }

        /// <summary>
        /// Checks the existance of a certain Stream in Redis cache.
        /// </summary>
        /// <param name="streamName">The name of the stream.</param>
        /// <returns>A boolean. If the stream exists, returns true, otherwise, returns false.</returns>
        private bool checkRedisStreamExistance(string streamName)
        {
            return redisDB.KeyExists(streamName);
        }

        /// <summary>
        /// Checks the existance of a consumer group attached to a certain stream in the Redis cache.
        /// </summary>
        /// <param name="streamName">The name of the stream.</param>
        /// <param name="consumerGroupName">The name of the consumer group.</param>
        /// <returns>A boolean. If the consumer group already exists, returns true, otherwise, returns false.</returns>
        private bool checkRedisConsumerGroupExistance(string streamName, string consumerGroupName)
        {
            StreamGroupInfo[] groups = redisDB.StreamGroupInfo(streamName);
            return groups.Any(g => g.Name == consumerGroupName);
        }

        /// <summary>
        /// <para>Configures the connection to Redis cache.</para>
        /// <para>Also checks if the terminal-output stream and it's consumer group are already created, regenerating if the last one does.</para>
        /// </summary>
        /// <returns>A Task object (async method inherent).</returns>
        private async Task configureRedis()
        {
            if (checkRedisStreamExistance(redisTerminalOutputStream))
            {
                if (checkRedisConsumerGroupExistance(redisTerminalOutputStream, redisAPIReaderConsumerGroup))
                {
                    redisDB.StreamDeleteConsumerGroup(redisTerminalOutputStream, redisAPIReaderConsumerGroup);
                }
            }

            /*
             * If the stream wasn't created yet (the above condition returns false), it's inherently true that
             * the consumer group does not exist, so we should call StreamCreateConsumerGroup, which creates
             * the stream before creating the consumer group.
            */

            redisDB.StreamCreateConsumerGroup(redisTerminalOutputStream, redisAPIReaderConsumerGroup, "$");
        }

        /// <summary>
        /// Main asynchronous business logic method that loops over the background service lifetime.
        /// </summary>
        /// <param name="stoppingToken">Signals the method to stop it's execution when the background service process is about to terminate.</param>
        /// <returns>A Task object (async method inherent).</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!checkRedisStreamExistance(redisTerminalOutputStream))
                {
                    await Task.Delay(100, stoppingToken);
                    continue;
                }

                if (!checkRedisConsumerGroupExistance(redisTerminalOutputStream, redisAPIReaderConsumerGroup))
                {
                    await Task.Delay(100, stoppingToken);
                    continue;
                }

                StreamEntry[] streamEntries = await redisDB.StreamReadGroupAsync(
                    "terminal-output", // stream
                    "wss-api-reader-group",       // group
                    "apiTaskGateway",      // consumer name
                    "$"               // only new messages
                );

                for (int i = 0; i < streamEntries.Length; i++)
                {
                    RedisValue[] activeSessions = await redisDB.ListRangeAsync("osSessionQueue", 0, -1);

                    for (int j = 0; j < activeSessions.Length; j++)
                    {
                        if (!activeSessions[j].IsNull)
                        {
                            PSYSTEMS_HARDWARE_DATA_HANDLING.OSConnSessionInfo sessionData = System.Text.Json.JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.OSConnSessionInfo>(activeSessions[j]);
                            Console.WriteLine("I'M HERE!!!");
                            if (sessionData.WSSListenerConnectionID == streamEntries[i].Values[0].Value)
                            {
                                Console.WriteLine("Reached destination!!!");
                                await _hubContext.Clients.Client(sessionData.WSSListenerConnectionID).SendAsync("ReceiveTerminalOutput", streamEntries[i].Values[1].Value.ToString(), stoppingToken);
                                redisDB.StreamAcknowledge("terminal-output", "wss-api-reader-group", streamEntries[i].Id); // Acknowledge message as processed
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}