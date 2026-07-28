using Microsoft.AspNetCore.Mvc;
using POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER;
using System.Security.Claims;
using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.USER_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/backend/user/notifications")]
    public class USER_NOTIFICATIONS_CONTROLLER : Controller
    {
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        public USER_NOTIFICATIONS_CONTROLLER()
        {
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("getUserNotifications")]
        public IActionResult DBGetUserNotificationsBatch()
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                string? strUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int userId = -1;

                if(!int.TryParse(strUserId, out userId)) throw new Exception("Can't parse UserId authentication cookie string to integer.");

                List<NotificationInfo> NotificationsListInfo = DB_HANDLER.USER_DATA_HANDLER.DBGetUserNotifications(userId);
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

        [HttpGet("resolveNotification_{notificationID}")]
        public IActionResult DBMarkNotificationAsResolved([FromRoute] int notificationID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                int notificationResolutionRowsAffected = DB_HANDLER.USER_DATA_HANDLER.DBMarkNotificationAsResolved(notificationID);
                response.operationStatus = true;
                response.statusMessage = "Notification successfuly marked as resolved!";
                response.packetData = notificationResolutionRowsAffected;
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