using Microsoft.AspNetCore.Mvc;
using POWER_ENV;
using POWERENV_PGSQL_DB_HANDLER;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/auth")]
    public class MACHINE_AUTH_CONTROLLER : Controller
    {
        private POWERENV POWERENVEngine;
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;
        /// <summary>
        /// /// Controler Class Constructor Method
        /// </summary>
        public MACHINE_AUTH_CONTROLLER()
        {
            POWERENVEngine = new POWERENV();
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        //Change from GET to POST (to increase security)
        [HttpPost("{_systemID}/changePassword")]
        public IActionResult ASMIChangePassword([FromRoute] int _systemID, [FromBody] string _newPassword)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                POWERENVEngine.Main(_systemID);
                POWERENV.AuthManagementLib.ASMIChangePassword(_newPassword);
                Thread.Sleep(3000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "ASMI password changed successfully.";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/getLoginAudits/{_loginAuditsType}")]
        public IActionResult ASMIGetLoginAudits([FromRoute] int _systemID, [FromRoute] string _loginAuditsType)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            List<AUTH_MGMT.STRUCT_LOGIN_AUDIT_INFO> authLogs = new List<AUTH_MGMT.STRUCT_LOGIN_AUDIT_INFO>();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);

                switch (_loginAuditsType)
                {
                    case "SuccessfullLogins":
                        authLogs = POWERENV.AuthManagementLib.getASMILoginAudits(AUTH_MGMT.ENUM_LOGIN_AUDIT_TYPE.SuccessfullLogin);
                        break;
                    case "FailedLogins":
                        authLogs = POWERENV.AuthManagementLib.getASMILoginAudits(AUTH_MGMT.ENUM_LOGIN_AUDIT_TYPE.FailedLogin);
                        break;
                    default:
                        throw new Exception("Invalid login audits type. Use 'SuccessfullLogins' or 'FailedLogins'.");
                }

                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed

                POWERENVEngine.CloseSerialConnection();

                List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> dbLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodesLoginAudits(_systemID);

                for (int i = 0; i < authLogs.Count; i++)
                {
                    PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS loginAudit = new PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS() {
                        login_audit_location = authLogs[i].location,
                        login_audit_datetime = $"{authLogs[i].date} {authLogs[i].time}",
                        login_audit_fsp_user = authLogs[i].user,
                        login_audit_login_status = _loginAuditsType == "SuccessfullLogins" ? "SUCCESS" : "FAILURE"
                    };

                    if (!listContainsLoginAudit(dbLoginAudits, loginAudit))
                    {
                        int rowsAffectedCount = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodesLoginAudits(_systemID, loginAudit);

                        if (rowsAffectedCount == 0)
                        {
                            response.operationStatus = false;
                            response.statusMessage = "Login Audits retrieved, but the database was not updated!";
                            break;
                        }
                    }
                }

                if(response.statusMessage == null)
                {
                    response.operationStatus = true;
                    response.statusMessage = "Login Audits retrieved successfully!";
                    response.packetData = authLogs;
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        private bool listContainsLoginAudit(List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> dbLoginAudits, PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS loginAudit)
        {
            for (int i = 0; i < dbLoginAudits.Count; i++)
            {
                if (dbLoginAudits[i].login_audit_datetime == loginAudit.login_audit_datetime)
                {
                    return true;
                }
            }

            return false;
        }
    }
}