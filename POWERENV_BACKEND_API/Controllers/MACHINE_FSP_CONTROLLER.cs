using Microsoft.AspNetCore.Mvc;
using POWER_ENV;
using POWERENV_PGSQL_DB_HANDLER;
using System.Collections.Generic;
using static POWER_ENV.FSP_MGMT;
using static POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/fsp")]
    public class MACHINE_FSP_CONTROLLER : Controller
    {
        private POWERENV POWERENVEngine;
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        /// <summary>
        /// Controler Class Constructor Method
        /// </summary>
        public MACHINE_FSP_CONTROLLER()
        {
            POWERENVEngine = new POWERENV();
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("{_systemID}/resetFsp")]
        public IActionResult ResetFSP([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                POWERENVEngine.Main(_systemID);
                POWERENV.FspMgmt.FSP_RESET();
                Thread.Sleep(2000); // Wait for 2 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "FSP reset task has started successfully.";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/getFSPErrorLogs")]
        public IActionResult GetFSPErrorLogs([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            List<POWER_ENV.FSP_MGMT.STRUCT_FSP_ERROR_LOG_INFO> errorLogInfo = new List<FSP_MGMT.STRUCT_FSP_ERROR_LOG_INFO>();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                errorLogInfo = POWERENV.FspMgmt.getFSPErrorLogs();
                Thread.Sleep(2000); // Wait for 2 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> DBErrorLogInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodesErrorLogs(_systemID);

                for (int i = 0; i < errorLogInfo.Count; i++)
                {
                    List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_FRU_INFO> fruInfoList = new List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_FRU_INFO>();

                    for (int j = 0; j < errorLogInfo[i].NormalHardwareFRU.Count; j++)
                    {
                        PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_FRU_INFO _FSP_ERROR_LOG_FRU_INFO = new PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_FRU_INFO()
                        {
                            CCIN = errorLogInfo[i].NormalHardwareFRU[j].CCIN,
                            LocationCode = errorLogInfo[i].NormalHardwareFRU[j].LocationCode,
                            PartNumber = errorLogInfo[i].NormalHardwareFRU[j].PartNumber,
                            SerialNumber = errorLogInfo[i].NormalHardwareFRU[j].SerialNumber,
                            Priority = errorLogInfo[i].NormalHardwareFRU[j].Priority
                        };

                        fruInfoList.Add(_FSP_ERROR_LOG_FRU_INFO);
                    }

                    PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO currentErrorLog = new PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO() {
                        ErrorLogID = errorLogInfo[i].ErrorLogID,
                        LogDate = POWERENV.DatetimeMgmt.changeDateStringFormat(errorLogInfo[i].LogDate.Replace("/", "-"), new int[] { 2, 0, 1 }), // Convert date format from MM/DD/YYYY to YYYY-MM-DD
                        LogTime = errorLogInfo[i].LogTime,
                        EventSeverity = errorLogInfo[i].EventSeverity,
                        DriverName = errorLogInfo[i].DriverName,
                        RawData = errorLogInfo[i].RawData,
                        Subsystem = errorLogInfo[i].Subsystem,
                        ReferenceCode = errorLogInfo[i].ReferenceCode,
                        ActionFlags = errorLogInfo[i].ActionFlags,
                        ActionStatus = errorLogInfo[i].ActionStatus,
                        NormalHardwareFRU = fruInfoList
                    };
                    if (!listContainsErrorLog(DBErrorLogInfo, currentErrorLog))
                    {
                        int errorLogInsertRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeErrorLog(_systemID, currentErrorLog);

                        if (errorLogInsertRowsAffected == 0)
                        {
                            response.operationStatus = false;
                            response.statusMessage = "FSP Errors retrieved, but the database was not updated!";
                            break;
                        }
                    }
                }

                if(response.statusMessage == null)
                {
                    response.operationStatus = true;
                    response.statusMessage = "FSP Errors successfully retrieved.";
                    response.packetData = errorLogInfo;
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = $"{ex.Message} | {ex.StackTrace}";
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/getSystemInfo")]
        public IActionResult GetMachineInfo([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            STRUCT_MACHINE_INFO systemInfo = new STRUCT_MACHINE_INFO();
            try
            {
                POWERENVEngine.Main(_systemID);
                systemInfo = POWERENV.FspMgmt.GetMachineInfo();
                Thread.Sleep(2000); // Wait for 2 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "System information successfully retrieved.";
                response.packetData = systemInfo;
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/openASMISession")]
        public IActionResult openASMISession([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWER_ENV.POWERENV.SendCommand("\n", 500);
                string receivedData = POWER_ENV.POWERENV.GetReceivedData();

                STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                {
                    operationCatName = "REMOTE_ACCESS",
                    operationSourcePNodeID = _systemID,
                    operationAction = $"NodeAccessASMIConsole",
                    operationCompletionStatus = "SUCCESS",
                    operationSourceUserName = "Alice Wonder"
                };

                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                if(pnodePowerOnOperationRegistryRowsAffected > 0)
                {
                    response.operationStatus = true;
                    response.statusMessage = "ASMI Session Started!!!";
                }
                else
                {
                    response.operationStatus = false;
                    response.statusMessage = "ASMI Session started, but database not updated (database write fail)!";
                }
                response.packetData = receivedData;
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/closeASMISession")]
        public IActionResult closeASMISession([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "ASMI Session Ended Successfully!!!";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost("{_systemID}/sendASMICommand")]
        public IActionResult SendASMICommand([FromRoute] int _systemID, [FromBody] string command)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                POWER_ENV.POWERENV.SendCommand(command, 1000);
                string commandResult = POWER_ENV.POWERENV.GetReceivedData();
                response.operationStatus = true;
                response.statusMessage = "Command Executed Successfully!!!";
                response.packetData = new { commandResult };
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        private bool listContainsErrorLog(List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> dbErrorLogs, PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO errorLog)
        {
            for(int i = 0; i < dbErrorLogs.Count; i++)
            {
                if (dbErrorLogs[i].ErrorLogID == errorLog.ErrorLogID)
                {
                    return true;
                }
            }

            return false;
        }
    }
}