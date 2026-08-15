using Microsoft.AspNetCore.Mvc;
using POWER_ENV;
using POWER_ENV.GLOBAL.NETWORK;
using POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER;
using System.Security.Claims;
using static POWER_ENV.FSP_MGMT;
using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/backend/data")]
    public class SQLITE_DATA_CONTROLLER : Controller
    {
        private record PGridInsights
        {
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.AccessPolicyInfo> accessPolicies { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.AccessAuditInfo> accessAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits> pnodesLoginAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo> pnodesErrorLogs { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.AttentionLEDPNodesInfo> attentionLEDMarkedPNodes { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolsList> ppoolsInfoList { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.PGridFullInfo pgridFullInfo { get; set; }
        }

        private record PPoolInsights
        {
            public PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolFullInfo ppoolFullInfo { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodesBasicInfo> ppoolPNodesFullList { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits> ppoolLoginAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.AttentionLEDPNodesInfo> ppoolAttentionLEDMarkedPNodes { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo> ppoolErrorLogs { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolsOperationHistory ppoolOperationLogs { get; set; }
        }

        private record PNodeInsights
        {
            public PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeFullInfo pnode_full_info { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeFSPInfo pnodeFSPInfo { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeMachineInfo pnodeMachineInfo { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeNICInfo> pnodeNICInfo { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits> pnodeLoginAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodesSingleOperationHistory> pnodeSingleOperationHistory { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo> pnodeErrorLogs { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeETHAccessPolicyInfo> pnodeETHAccessPolicies { get; set; }
        }

        public record NewPNodeMachineSyncCredentials
        {
            public required string COMPort { get; set; }
            public required string username { get; set; }
            public required string password { get; set; }
        }

        private POWERENV POWERENVEngine;
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        public SQLITE_DATA_CONTROLLER()
        {
            POWERENVEngine = new POWERENV();
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("getRecentActivity")]
        public IActionResult DBGetRecentActivity()
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            int userId;

            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId))
            {
                List<PSYSTEMS_HARDWARE_DATA_HANDLING.GlobalEvent> recentActivityInfoList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetRecentActivity(userId);

                response.operationStatus = true;
                response.statusMessage = "Recent activity data successfully received!";
                response.packetData = recentActivityInfoList;
            }
            else
            {
                response.operationStatus = false;
                response.statusMessage = "Can't parse userID to integer!!!";
            }

            return Ok(response);
        }

        [HttpGet("getPGridsList")]
        public IActionResult DBGetPGridsList()
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            int userId;

            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out userId))
            {
                List<PSYSTEMS_HARDWARE_DATA_HANDLING.PGridBasicInfo> pgridsInfoList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGrids(userId);

                response.operationStatus = true;
                response.statusMessage = "PGrid Dashboard data successfully received!";
                response.packetData = pgridsInfoList;
            }
            else
            {
                response.operationStatus = false;
                response.statusMessage = "Can' parse userID to integer!!!";
            }

            return Ok(response);
        }

        [HttpGet("pgrid{_pgridID}/")]
        public IActionResult DBGetPgridInsights([FromRoute] int _pgridID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            List<PSYSTEMS_HARDWARE_DATA_HANDLING.AccessPolicyInfo> accessPolicies = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGAccessPolicies(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.AccessAuditInfo> accessAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGAccessAudits(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits> pnodesLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGPNLoginAudits(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo> pnodesErrorLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGErrorLogs(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.AttentionLEDPNodesInfo> attentionLEDMarkedPNodes = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetAttentionLEDPNodes(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolsList> ppoolsInfoList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGPPoolsList(_pgridID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.PGridFullInfo pgridFullInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGridFullInfo(_pgridID);

            PGridInsights pgridInsights = new PGridInsights()
            {
                accessPolicies = accessPolicies,
                accessAudits = accessAudits,
                pnodesLoginAudits = pnodesLoginAudits,
                pnodesErrorLogs = pnodesErrorLogs,
                attentionLEDMarkedPNodes = attentionLEDMarkedPNodes,
                ppoolsInfoList = ppoolsInfoList,
                pgridFullInfo = pgridFullInfo
            };

            response.operationStatus = true;
            response.statusMessage = "PGrid Dashboard data successfully received!";
            response.packetData = pgridInsights;

            return Ok(response);
        }

        [HttpPost("createNewPGrid")]
        public IActionResult DBCreateNewPGrid([FromBody] PGridFullInfo newPGridInfo)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                int userID = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int newPGridRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBCreateNewPGrid(userID, newPGridInfo);

                response.operationStatus = true;
                response.statusMessage = "New PGrid successfully created!";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = $"New PGrid creation operation failed! Error: {ex.Message}";
            }

            return Ok(response);
        }

        [HttpGet("deletePGrid_{pgrid_id}")]
        public IActionResult DBDeletePGrid([FromRoute] int pgrid_id)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                int userID = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string pgridName = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGridFullInfo(pgrid_id).pgrid_name;
                int newPGridRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBDeletePGrid(userID, pgrid_id);

                response.operationStatus = true;
                response.statusMessage = $"PGrid successfully deleted!";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = $"PGrid deletion operation failed! Error: {ex.Message}";
            }

            return Ok(response);
        }

        [HttpGet("pgrid{_pgridID}/ppool{_ppoolID}")]
        public IActionResult DBGetPPoolInsights([FromRoute] int _ppoolID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodesBasicInfo> ppoolPNodesList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGPPoolPNodesList(_ppoolID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolFullInfo ppoolFullInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolFullInfo(_ppoolID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits> ppoolLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsLoginAudits(_ppoolID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.AttentionLEDPNodesInfo> ppoolAttentionLEDMarkedPNodes = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolAttentionLEDPNodes(_ppoolID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo> ppoolErrorLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsErrorLogs(_ppoolID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolsOperationHistory ppoolOperationLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsOperationLogs(_ppoolID);

            PPoolInsights ppoolInsights = new PPoolInsights()
            {
                ppoolPNodesFullList = ppoolPNodesList,
                ppoolFullInfo = ppoolFullInfo,
                ppoolLoginAudits = ppoolLoginAudits,
                ppoolAttentionLEDMarkedPNodes = ppoolAttentionLEDMarkedPNodes,
                ppoolErrorLogs = ppoolErrorLogs,
                ppoolOperationLogs = ppoolOperationLogs
            };

            response.operationStatus = true;
            response.statusMessage = "PPool Dashboard data successfully received!";
            response.packetData = ppoolInsights;

            return Ok(response);
        }

        [HttpPost("createNewPPool")]
        public IActionResult DBCreateNewPPool([FromBody] PPoolFullInfo newPPoolInfo)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                int userID = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int newPGridRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBCreateNewPPool(userID, newPPoolInfo);

                response.operationStatus = true;
                response.statusMessage = "New PPool successfully created!";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = $"New PPool creation operation failed! Error: {ex.Message}";
            }

            return Ok(response);
        }

        [HttpGet("deletePPool_{ppool_id}")]
        public IActionResult DBDeletePPool([FromRoute] int ppool_id)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                int userID = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                string ppoolName = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolFullInfo(ppool_id).ppool_name;
                int newPPoolRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBDeletePPool(userID, ppool_id);

                response.operationStatus = true;
                response.statusMessage = $"PPool successfully deleted!";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = $"PPool deletion operation failed! Error: {ex.Message}";
            }

            return Ok(response);
        }

        [HttpPost("ppool{_ppoolID}/changeReadme")]
        public IActionResult DBPPoolEditReadmeText([FromRoute] int _ppoolID, [FromBody] string newReadmeText)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            int readmeTextRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBPPoolEditReadme(_ppoolID, newReadmeText);

            response.operationStatus = true;
            response.statusMessage = "PPool Readme successfully received!";

            return Ok(response);
        }

        [HttpGet("pgrid{_pgridID}/ppool{_ppoolID}/pnode{_pnodeID}")]
        public IActionResult DBGetPNodeInsights([FromRoute] int _pnodeID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeFullInfo pnodeFullInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_pnodeID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeFSPInfo pnodeFSPInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFSPInfo(_pnodeID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeMachineInfo pnodeMachineInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeMachineInfo(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeNICInfo> pnodeNICInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeNICsInfo(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits> pnodeLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodesLoginAudits(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodesSingleOperationHistory> pnodeSingleOperationHistory = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeOperationLogs(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo> pnodeErrorLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodesErrorLogs(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodeETHAccessPolicyInfo> pnodeETHAccessPolicies = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeETHAccessPolicies(_pnodeID);

            PNodeInsights ppoolInsights = new PNodeInsights()
            {
                pnode_full_info = pnodeFullInfo,
                pnodeFSPInfo = pnodeFSPInfo,
                pnodeMachineInfo = pnodeMachineInfo,
                pnodeNICInfo = pnodeNICInfo,
                pnodeLoginAudits = pnodeLoginAudits,
                pnodeSingleOperationHistory = pnodeSingleOperationHistory,
                pnodeErrorLogs = pnodeErrorLogs,
                pnodeETHAccessPolicies = pnodeETHAccessPolicies
            };

            response.operationStatus = true;
            response.statusMessage = "PNode Dashboard data successfully received!";
            response.packetData = ppoolInsights;

            return Ok(response);
        }

        [HttpPost("syncNewPNodeMachine")]
        public IActionResult DBSyncNewPNodeMachine([FromBody] NewPNodeMachineSyncCredentials credentialsBundle)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                STRUCT_MACHINE_INFO systemInfo = new STRUCT_MACHINE_INFO();
                int userID = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                POWERENVEngine.Main(credentialsBundle.COMPort);
                systemInfo = POWERENV.FspMgmt.GetMachineInfo();
                Thread.Sleep(2000); // Wait for 2 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                POWERENVEngine.Main(credentialsBundle.COMPort);
                STRUCT_NETWORK_INTERFACE networkInfo = POWERENV.NetworkMgmt.GetNetworkInterfaceConfigs(0);
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                response.operationStatus = true;
                response.statusMessage = "Machine Data Synchronized!";
                response.packetData = new
                {
                    systemInfo,
                    networkInfo
                };
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = $"Machine synchronization operation failed! Error: {ex.Message}";
            }

            return Ok(response);
        }

        [HttpPost("createNewPNode")]
        public IActionResult DBCreateNewPNode([FromBody] CreatePNodeDataBundle pnodeInfo)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            try
            {
                int userID = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int newPGridRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBCreateNewPNode(userID, pnodeInfo.pnodeBasicInfo, pnodeInfo.pnodeFSPInfo, pnodeInfo.pnodeOSUserInfoType);

                response.operationStatus = true;
                response.statusMessage = "New PNode successfully created!";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = $"New PNode creation operation failed! Error: {ex.Message}";
            }

            return Ok(response);
        }

        [HttpPost("pnode{_pnodeID}/changeReadme")]
        public IActionResult DBPNodeEditReadmeText([FromRoute] int _pnodeID, [FromBody] string newReadmeText)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            int readmeTextRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBPNodeEditReadme(_pnodeID, newReadmeText);

            try
            {
                string userName = User.FindFirst(ClaimTypes.Name)?.Value;
                string pnodeNickname = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_pnodeID).pnode_nickname;

                PNodesSingleOperationHistory PowerOnOperationData = new PNodesSingleOperationHistory
                {
                    operationCatName = "DOCUMENTATION",
                    operationSourcePNodeID = _pnodeID,
                    operationAction = $"NodeEditReadme",
                    operationDescription = $"PNode '{pnodeNickname}' readme text was edited by {userName}.",
                    operationSeverityLevelID = 1,
                    operationCompletionStatus = "SUCCESS",
                    operationSourceUserName = userName
                };

                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                response.operationStatus = true;
                response.statusMessage = "PNode Readme successfully received!";
            }
            catch (Exception error)
            {
                response.operationStatus = false;
                response.statusMessage = $"PNode Readme updated, but operation log creation failed!!! Error: ${error}";
            }

            return Ok(response);
        }
    }
}