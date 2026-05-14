using Microsoft.AspNetCore.Mvc;
using POWER_ENV;
using POWERENV_PGSQL_DB_HANDLER;
using StackExchange.Redis;
using System.Text.Json;
using XTELNET;
using static POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/os")]
    public class MACHINE_OS_CONTROLLER : Controller
    {
        XTELNET_COMMAND XTelnet_Instance;
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        public MACHINE_OS_CONTROLLER()
        {
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("{pnode_id}/openOSSession")]
        public async Task<IActionResult> openOSSession([FromRoute] int pnode_id, IConnectionMultiplexer redis)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            
            try
            {
                IDatabase db = redis.GetDatabase();

                Random rndEngine = new Random();
                int newToken;
                PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_LPAR_FULL_INFO targetLPARID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeMainOSLPARInfo(pnode_id);
                bool canProceed = false;

                do {
                    // Generate a random 6-digit token
                    newToken = rndEngine.Next(100000, 999999);

                    RedisValue[] currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);

                    // If the generated token is not already in the queue, we can proceed
                    if (!currDBQueueList.Contains(newToken)) canProceed = true;
                } while (!canProceed);

                PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO newSessionInfo = new PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO()
                {
                    session_id = newToken,
                    WSSListenerConnectionID = "-1",
                    sessionTargetLPARInfo = targetLPARID,
                    pendingCommand = ""
                };

                updateUserCredentials(targetLPARID.lpar_target_pnode_id);

                string jsonSessionInfo = JsonSerializer.Serialize(newSessionInfo);

                await db.ListRightPushAsync("osSessionQueue", jsonSessionInfo);

                STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                {
                    operationCatName = "REMOTE_ACCESS",
                    operationSourcePNodeID = pnode_id,
                    operationAction = $"NodeAccessOSConsole",
                    operationCompletionStatus = "SUCCESS",
                    operationSourceUserName = "Alice Wonder"
                };

                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                if (pnodePowerOnOperationRegistryRowsAffected > 0)
                {
                    response.operationStatus = true;
                    response.statusMessage = "OS session established with success!";
                    response.packetData = $"{newSessionInfo.session_id}";
                }
                else
                {
                    response.operationStatus = false;
                    response.statusMessage = "OS session established, but database not updated (database operation registry write fail)!";
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{sessionID}/closeOSSession")]
        public async Task<IActionResult> closeOSSession([FromRoute] int sessionID, IConnectionMultiplexer redis)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                IDatabase db = redis.GetDatabase();

                RedisValue[] currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);

                if(currDBQueueList.Length > 0)
                {
                    PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO targetSession = new PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO();
                    string targetSessionSerialized = string.Empty;

                    int c = 0;
                    for (int i = 0; i < currDBQueueList.Length; i++)
                    {
                        targetSession = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(currDBQueueList[i]);
                        targetSessionSerialized = currDBQueueList[i];

                        if (targetSession.session_id == sessionID) break;
                        c++;
                    }

                    if (c < currDBQueueList.Length)
                    {
                        await db.ListRemoveAsync("osSessionQueue", targetSessionSerialized);
                    }

                    response.operationStatus = true;
                    response.statusMessage = "OS session closed with success!";
                }
                else
                {
                    response.statusMessage = "No active OS sessions found!";
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost("{sessionID}/OSSendCommand")]
        public async Task<IActionResult> OSSendCommand([FromBody] string command, [FromRoute] int sessionID, IConnectionMultiplexer redis)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                IDatabase db = redis.GetDatabase();

                RedisValue[] currDBQueueList = await db.ListRangeAsync("osSessionQueue", 0, -1);

                if (currDBQueueList.Length > 0)
                {
                    PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO targetSession = new PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO();
                    string targetSessionSerialized = string.Empty;

                    int c = 0;
                    for (int i = 0; i < currDBQueueList.Length; i++)
                    {
                        targetSession = JsonSerializer.Deserialize<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_OSCONN_SESSION_INFO>(currDBQueueList[i]);
                        targetSessionSerialized = currDBQueueList[i];

                        if (targetSession.session_id == sessionID) break;
                        c++;
                    }

                    if (c < currDBQueueList.Length)
                    {
                        targetSession.pendingCommand = command;
                        await db.ListSetByIndexAsync("osSessionQueue", c, JsonSerializer.Serialize(targetSession));
                    }

                    response.operationStatus = true;
                    response.statusMessage = "OS command successfully sent and queued!";
                    response.packetData = "OS command successfully sent and queued!";
                }
                else
                {
                    response.statusMessage = "No active OS sessions found!";
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

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
    }
}