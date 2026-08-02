using Microsoft.AspNetCore.Mvc;
using POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER;
using System.Security.Claims;
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
        public IActionResult DBGetUserGlobalEvents()
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                string? strUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = -1;

                if (!int.TryParse(strUserId, out userId)) throw new Exception("Can't parse UserId authentication cookie string to integer.");

                List<GlobalEvent> NotificationsListInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetGlobalEventsActivity(userId);
                GlobalEventTypesDistribution GlobalEventsDistribution = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetGlobalEventTypesDistribution(userId);
                List<GlobalEventCadenceRegistry> GlobalEventsCadenceStats = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetGlobalEventCadenceStats(userId, DateTime.Now.AddDays(-30), enum_timeScaleUnit.day);

                response.operationStatus = true;
                response.statusMessage = "Global Events retrieved successfully!";
                response.packetData = new {
                    NotificationsListInfo,
                    GlobalEventsDistribution,
                    GlobalEventsCadenceStats
                };
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("getUserGlobalEventsCadenceStats_{statsTimeRange}")]
        public IActionResult DBGetUserGlobalEventsCadenceStats([FromRoute] int statsTimeRange)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                string? strUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = -1;

                if (!int.TryParse(strUserId, out userId)) throw new Exception("Can't parse UserId authentication cookie string to integer.");

                enum_timeScaleUnit timeScaleUnit = statsTimeRange == 1 ? enum_timeScaleUnit.hour : enum_timeScaleUnit.day;

                List<GlobalEventCadenceRegistry> GlobalEventsCadenceStats = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetGlobalEventCadenceStats(userId, DateTime.Now.AddDays(-statsTimeRange), timeScaleUnit);

                response.operationStatus = true;
                response.statusMessage = "Global Events Cadence Statistics retrieved successfully!";
                response.packetData = GlobalEventsCadenceStats;
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