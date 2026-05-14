using Microsoft.AspNetCore.Mvc;
using POWER_ENV;
using POWERENV_PGSQL_DB_HANDLER;
using static POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/power")]
    public class MACHINE_POWER_CONTROLER : Controller
    {
        private POWERENV POWERENVEngine;
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        /// <summary>
        /// Controler Class Constructor Method
        /// </summary>
        public MACHINE_POWER_CONTROLER()
        {
            POWERENVEngine = new POWERENV();
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }
        
        [HttpGet("{_systemID}/poweron")]
        public IActionResult SysPowerOn([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWERENV.PowerManagementLib.DEVICE_POWERON();
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                int pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.updatePNodeActivenessState(_systemID, 1);

                STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY() {
                    operationCatName = "POWER",
                    operationSourcePNodeID = _systemID,
                    operationAction = "NodePowerOn",
                    operationCompletionStatus = pnodeActivenessStateUpdateRowsAffected > 0 ? "SUCCESS" : "FAILURE",
                    operationSourceUserName = "Alice Wonder"
                };
                
                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                if (pnodeActivenessStateUpdateRowsAffected >= 1 && pnodePowerOnOperationRegistryRowsAffected >= 1)
                {
                    response.operationStatus = true;
                    response.statusMessage = "System powered on successfully.";
                }
                else throw new Exception("System powered on, but database not updated (database write fail)!");
            }
            catch(Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/poweroff")]
        public IActionResult SysPowerOff([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                updateUserCredentials(_systemID);
                POWERENV.PowerManagementLib.DEVICE_POWEROFF();
                POWERENVEngine.CloseSerialConnection();

                int pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.updatePNodeActivenessState(_systemID, 2);

                STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                {
                    operationCatName = "POWER",
                    operationSourcePNodeID = _systemID,
                    operationAction = "NodePowerOff",
                    operationCompletionStatus = pnodeActivenessStateUpdateRowsAffected > 0 ? "SUCCESS" : "FAILURE",
                    operationSourceUserName = "Alice Wonder"
                };

                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                if (pnodeActivenessStateUpdateRowsAffected >= 1 && pnodePowerOnOperationRegistryRowsAffected >= 1)
                {
                    response.operationStatus = true;
                    response.statusMessage = "System powered off successfully.";
                }
                else throw new Exception("System powered off, but database not updated (database write fail)!");
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/restart")]
        public IActionResult SysRestart([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                updateUserCredentials(_systemID);
                POWERENV.PowerManagementLib.DEVICE_REBOOT();
                POWERENVEngine.CloseSerialConnection();

                int pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.updatePNodeActivenessState(_systemID, 1);

                STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                {
                    operationCatName = "POWER",
                    operationSourcePNodeID = _systemID,
                    operationAction = "NodeRestart",
                    operationCompletionStatus = pnodeActivenessStateUpdateRowsAffected > 0 ? "SUCCESS" : "FAILURE",
                    operationSourceUserName = "Alice Wonder"
                };

                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                if (pnodeActivenessStateUpdateRowsAffected >= 1 && pnodePowerOnOperationRegistryRowsAffected >= 1)
                {
                    response.operationStatus = true;
                    response.statusMessage = "System restarted successfully.";
                }
                else throw new Exception("System restarted, but database not updated (database write fail)!");
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/atentionLedOff")]
        public IActionResult SysPowerOffAtentionLed([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWERENV.PowerManagementLib.DEVICE_ATENTION_LED_OFF();
                Thread.Sleep(500); // Wait for 0.5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                int pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.updatePNodeAttentionLEDState(_systemID, "OFF");

                STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                {
                    operationCatName = "POWER",
                    operationSourcePNodeID = _systemID,
                    operationAction = "NodePowerOffAttentionLed",
                    operationCompletionStatus = pnodeActivenessStateUpdateRowsAffected > 0 ? "SUCCESS" : "FAILURE",
                    operationSourceUserName = "Alice Wonder"
                };

                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                if (pnodeActivenessStateUpdateRowsAffected >= 1 && pnodePowerOnOperationRegistryRowsAffected >= 1)
                {
                    response.operationStatus = true;
                    response.statusMessage = "Atention LED powered off.";
                }
                else throw new Exception("Attention LED powered off, but database not updated (database write fail)!");
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