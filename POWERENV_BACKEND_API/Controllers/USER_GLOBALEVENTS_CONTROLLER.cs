using Microsoft.AspNetCore.Mvc;
using POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER;
using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.USER_DATA_HANDLING;
using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/backend/user/globalEvents")]
    public class USER_GLOBALEVENTS_CONTROLLER : Controller
    {
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        public USER_GLOBALEVENTS_CONTROLLER()
        {
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("getUserGlobalEvents")]
        public IActionResult DBGetUserGlobalEvents([FromRoute] int userID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                List<GlobalEvent> NotificationsListInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetGlobalEventsActivity(userID);
                response.operationStatus = true;
                response.statusMessage = "Notifications retrieved successfully!";
                response.packetData = NotificationsListInfo;
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }
    }
}
