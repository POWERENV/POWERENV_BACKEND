using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using System.Text.Json;
using POWERENV_PGSQL_DB_HANDLER;

namespace POWERENV_BACKEND_API.SignalR
{
    public class OS_TERMINAL_WSS_HUB : Hub
    {
        //Redis
        private IConnectionMultiplexer redisCache;
        private IDatabase redisDb;

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("WSS Connection established!!!!!");
            var httpContext = Context.GetHttpContext();
            string strSessionId = httpContext.Request.Query["sessionId"];
            int sessionID = -1;

            if(int.TryParse(strSessionId, out sessionID))
            {
                redisCache = (IConnectionMultiplexer)httpContext.RequestServices.GetService(typeof(IConnectionMultiplexer));

                redisDb = redisCache.GetDatabase();

                // Map telnet session to WSS connection
                RedisValue[] queueList = await redisDb.ListRangeAsync("osSessionQueue", 0, -1);

                for (int i = 0; i < queueList.Length; i++)
                {
                    if (!queueList[i].IsNull)
                    {
                        PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO sessionInfo = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(queueList[i]); //queueList[i] is always non-null in this scope.

                        if (sessionInfo.session_id == sessionID && sessionInfo.WSSListenerConnectionID == "-1")
                        {
                            sessionInfo.WSSListenerConnectionID = Context.ConnectionId;
                            redisDb.ListSetByIndex("osSessionQueue", i, JsonSerializer.Serialize(sessionInfo));
                            break;
                        }
                    }
                }

                await base.OnConnectedAsync();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        // Called by client (optional)
        public async Task SendCommand(string sessionId, string command)
        {
            // Forward to your worker (queue, Redis, etc.)
        }
    }
}