using StackExchange.Redis;
using System.Text.Json;
using POWER_ENV;
using POWERENV_PGSQL_DB_HANDLER;
using XTELNET;

namespace POWERENV_OSCONSOLE_WORKER
{
    internal class OSSESSION_HANDLING
    {
        // Internal Fields
        private readonly ILogger<Worker> logger;

        //Database Handling
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        //Redis Cache Handling
        private IConnectionMultiplexer redisCache;
        private RedisValue[] dbQueueList = new RedisValue[1];

        //Telnet Session Handling
        List<XTELNET_COMMAND> XTelnetSessions = new List<XTELNET_COMMAND>();

        public OSSESSION_HANDLING(ILogger<Worker> _logger, IConnectionMultiplexer _redis, POWERDB_PGSQL_DATA_HANDLING _dbHandler)
        {
            logger = _logger;
            redisCache = _redis;
            DB_HANDLER = _dbHandler;
        }

        /// <summary>
        /// This method checks for new queued OS sessions in the Redis cache, and if it finds any, it initiates a new telnet session with the target OS specified in the session's data.
        /// </summary>
        /// <returns>Async Task Object.</returns>
        internal async Task checkForNewQueuedOSSessions()
        {
            IDatabase db = redisCache.GetDatabase();

            RedisValue[] currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);

            if (currDBQueueList.Length > 0)
            {
                bool hasNewItems = false;
                int lastI = 0;

                for (int i = 0; i < currDBQueueList.Count(); i++)
                {
                    lastI = i;
                    PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO sessionInfo = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(currDBQueueList[i]); //currDBQueueList[i] is always non-null in this scope.
                    hasNewItems = !findSessionID(dbQueueList, sessionInfo.session_id);
                    if (hasNewItems) break;
                }

                if (hasNewItems && !currDBQueueList[lastI].IsNull)
                {
                    PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO newSessionInfo = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(currDBQueueList[lastI]); //currDBQueueList[lastI] is always non-null in this scope.

                    updateUserCredentials(newSessionInfo.sessionTargetLPARInfo.lpar_target_pnode_id);

                    XTELNET_COMMAND XTelnet_Instance = new XTELNET.XTELNET_COMMAND(100, POWERENV.AuthManagementLib.OS_INFO.os_ip_address, 1, 1000, _sessionID: newSessionInfo.session_id);
                    POWERENV.AuthManagementLib.OSAuthenticate(XTelnet_Instance);

                    XTelnetSessions.Add(XTelnet_Instance);
                }

                dbQueueList = currDBQueueList;
            }
        }

        internal async Task checkForCloseSessions()
        {
            IDatabase db = redisCache.GetDatabase();

            RedisValue[] currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);

            if (dbQueueList.Length > 0)
            {
                bool hasRemovedItems = false;
                PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO deletedSessionData = new PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO();

                for (int i = 0; i < dbQueueList.Count(); i++)
                {
                    if (!dbQueueList[i].IsNull)
                    {
                        PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO sessionInfo = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(dbQueueList[i]); //currDBQueueList[i] is always non-null in this scope.
                        hasRemovedItems = !findSessionID(currDBQueueList, sessionInfo.session_id);

                        if (hasRemovedItems)
                        {
                            deletedSessionData = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(dbQueueList[i]); //dbQueueList[i] is always non-null in this scope.
                            break;
                        }
                    }
                }

                if (hasRemovedItems)
                {
                    for (int i = 0; i < XTelnetSessions.Count; i++)
                    {
                        if (XTelnetSessions[i].SessionID == deletedSessionData.session_id)
                        {
                            XTelnetSessions[i].Logout();
                            XTelnetSessions.RemoveAt(i);
                            break;
                        }
                    }
                }

                currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);
                dbQueueList = currDBQueueList;
            }
        }

        internal async Task checkForSessionCommands()
        {
            IDatabase db = redisCache.GetDatabase();
            RedisValue[] currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);

            if (currDBQueueList.Length > 0)
            {
                for (int i = 0; i < currDBQueueList.Count(); i++)
                {
                    PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO sessionInfo = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(currDBQueueList[i]); //currDBQueueList[i] is always non-null in this scope.
                    if (sessionInfo.pendingCommand.Length > 0)
                    {
                        for (int j = 0; j < XTelnetSessions.Count; j++)
                        {
                            if (XTelnetSessions[j].SessionID == sessionInfo.session_id)
                            {
                                string output = XTelnetSessions[j].ExecuteCommandsAndReturn(new List<string>() {
                                    sessionInfo.pendingCommand
                                });

                                sessionInfo.pendingCommand = string.Empty;

                                await db.ListSetByIndexAsync("osSessionQueue", i, JsonSerializer.Serialize(sessionInfo));

                                await db.StreamAddAsync(
                                    "terminal-output",   // stream key
                                    new NameValueEntry[]
                                    {
                                        new("targetSession", sessionInfo.WSSListenerConnectionID),
                                        new("data", output),
                                        new("timestamp", DateTime.UtcNow.ToString("O"))
                                    }
                                );

                                break;
                            }
                        }
                    }
                }
            }

            currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);
            dbQueueList = currDBQueueList;
        }

        /// <summary>
        /// This method fetches user credentials from the database and assigns them to the AuthManagementLib object, inside POWERENV's internal communication engine.
        /// </summary>
        /// <param name="_systemID"></param>
        private void updateUserCredentials(int _systemID)
        {
            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_LPAR_FULL_INFO osInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeMainOSLPARInfo(_systemID);
            POWERENV.AuthManagementLib.OS_INFO = new AUTH_MGMT.STRUCT_OS_USER_INFO()
            {
                os_id = osInfo.os_id,
                os_ip_address = osInfo.os_ip_address,
                osAuthInfo = new AUTH_MGMT.STRUCT_AUTH_INFO()
                {
                    username = osInfo.osAuthInfo.username,
                    password = osInfo.osAuthInfo.password
                },
                os_family = osInfo.os_family
            };
        }

        private bool findSessionID(RedisValue[] sessionQueue, int sessionID)
        {
            for (int j = 0; j < sessionQueue.Length; j++)
            {
                PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO pastSessionInfo = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(sessionQueue[j]); //dbQueueList[j] is always non-null in this scope.
                if (pastSessionInfo.session_id == sessionID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}