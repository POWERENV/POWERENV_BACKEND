using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using POWERENV_BACKEND_API.SignalR;
using POWERENV_PGSQL_DB_HANDLER;

namespace POWERENV_BACKEND_API
{
    public class TaskGatewayBKGService : BackgroundService
    {
        //Request Context
        private readonly IHubContext<OS_TERMINAL_WSS_HUB> _hubContext;

        //Redis
        private readonly IConnectionMultiplexer _redis;
        private IDatabase redisDB;

        public TaskGatewayBKGService(IConnectionMultiplexer redis, IHubContext<OS_TERMINAL_WSS_HUB> hubContext)
        {
            _hubContext = hubContext;
            _redis = redis;
            redisDB = _redis.GetDatabase();

            redisDB.StreamCreateConsumerGroup("terminal-output", "wss-api-reader-group", "$"); // start from latest messages
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
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
                            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO sessionData = System.Text.Json.JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(activeSessions[j]);
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

                await Task.Delay(100, stoppingToken);
            }
        }
    }
}