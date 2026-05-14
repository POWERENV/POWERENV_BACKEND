using Microsoft.AspNetCore.Mvc;
using POWER_ENV;
using POWERENV_PGSQL_DB_HANDLER;
using static POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/datetime")]
    public class MACHINE_DATETIME_CONTROLLER : Controller
    {
        private POWERENV POWERENVEngine;
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        /// <summary>
        /// Controler Class Constructor Method
        /// </summary>
        public MACHINE_DATETIME_CONTROLLER()
        {
            POWERENVEngine = new POWERENV();
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("{_systemID}/setdate/{_newDate}")]
        public IActionResult SetSystemDate([FromRoute] int _systemID, [FromRoute] string _newDate)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWERENV.DatetimeMgmt.DEVICE_SET_DATE(POWERENV.DatetimeMgmt.changeDateStringFormat(_newDate, new int[] { 2, 1, 0 }));
                Thread.Sleep(2000); // Wait for 2 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                int dateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBPNodeEditDateTime(_systemID, _newDate, null);

                if(dateUpdateRowsAffected > 0)
                {
                    response.operationStatus = true;
                    response.statusMessage = "System date set successfully.";
                }
                else
                {
                    response.operationStatus = false;
                    response.statusMessage = $"System date edited on the machine, but the database was not updated!";
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/settime/{_newTime}")]
        public IActionResult SetSystemTime([FromRoute] int _systemID, [FromRoute] string _newTime)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWERENV.DatetimeMgmt.DEVICE_SET_TIME(_newTime);
                Thread.Sleep(2000); // Wait for 2 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                int dateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBPNodeEditDateTime(_systemID, null, _newTime);

                if (dateUpdateRowsAffected > 0)
                {
                    response.operationStatus = true;
                    response.statusMessage = "System time set successfully.";
                }
                else
                {
                    response.operationStatus = false;
                    response.statusMessage = $"System time edited on the machine, but the database was not updated!";
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/setdatetime/{_newDateTime}")]
        public IActionResult SetSystemDateTime([FromRoute] int _systemID, [FromRoute] string _newDateTime)
        {
            IActionResult changeDateResponse = SetSystemDate(_systemID, _newDateTime.Split(' ')[0]);
            IActionResult changeTimeResponse = SetSystemTime(_systemID, _newDateTime.Split(' ')[1]);
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            if (((Program.STRUCT_REQUEST_DATA)((OkObjectResult)changeDateResponse).Value).operationStatus == false)
            {
                response = (Program.STRUCT_REQUEST_DATA)((OkObjectResult)changeDateResponse).Value;
            }
            else if (((Program.STRUCT_REQUEST_DATA)((OkObjectResult)changeTimeResponse).Value).operationStatus == false)
            {
                response = (Program.STRUCT_REQUEST_DATA)((OkObjectResult)changeTimeResponse).Value;
            }
            else
            {
                response = new Program.STRUCT_REQUEST_DATA()
                {
                    operationStatus = true,
                    statusMessage = "System date and time set successfully!"
                };
            }

            STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
            {
                operationCatName = "FSP",
                operationSourcePNodeID = _systemID,
                operationAction = $"NodeEditDateTime",
                operationCompletionStatus = response.operationStatus == true ? "SUCCESS" : "FAILURE",
                operationSourceUserName = "Alice Wonder"
            };

            DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

            return Ok(response);
        }
    }
}