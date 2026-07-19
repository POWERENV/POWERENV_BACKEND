using Microsoft.AspNetCore.Mvc;
using POWERENV_PGSQL_DB_HANDLER;
using static POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

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

        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        public SQLITE_DATA_CONTROLLER()
        {
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("getPGridsList")]
        public IActionResult DBGetPGridsList()
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            List<PSYSTEMS_HARDWARE_DATA_HANDLING.PGridBasicInfo> pgridsInfoList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGrids();

            response.operationStatus = true;
            response.statusMessage = "PGrid Dashboard data successfully received!";
            response.packetData = pgridsInfoList;

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

        [HttpGet("pgrid{_pgridID}/ppool{_ppoolID}")]
        public IActionResult DBGetPPoolInsights([FromRoute] int _ppoolID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            List<PSYSTEMS_HARDWARE_DATA_HANDLING.PNodesBasicInfo> ppoolPNodesList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGPPoolPNodesList(_ppoolID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolFullInfo ppoolFullInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolFullInfo(_ppoolID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits> ppoolLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsLoginAudits(_ppoolID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.AttentionLEDPNodesInfo> ppoolAttentionLEDMarkedPNodes = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolAttentionLEDPNodes(_ppoolID);
            List <PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo> ppoolErrorLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsErrorLogs(_ppoolID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.PPoolsOperationHistory ppoolOperationLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsOperationLogs(_ppoolID);

            PPoolInsights ppoolInsights = new PPoolInsights()
            {
                ppoolPNodesFullList = ppoolPNodesList,
                ppoolFullInfo = ppoolFullInfo,
                ppoolLoginAudits = ppoolLoginAudits,
                ppoolAttentionLEDMarkedPNodes = ppoolAttentionLEDMarkedPNodes,
                ppoolErrorLogs = ppoolErrorLogs,
                ppoolOperationLogs= ppoolOperationLogs
            };

            response.operationStatus = true;
            response.statusMessage = "PPool Dashboard data successfully received!";
            response.packetData = ppoolInsights;

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

        [HttpPost("pnode{_pnodeID}/changeReadme")]
        public IActionResult DBPNodeEditReadmeText([FromRoute] int _pnodeID, [FromBody] string newReadmeText)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            int readmeTextRowsChanged = DB_HANDLER.HARDWARE_DATA_HANDLER.DBPNodeEditReadme(_pnodeID, newReadmeText);

            PNodesSingleOperationHistory PowerOnOperationData = new PNodesSingleOperationHistory
            {
                operationCatName = "DOCUMENTATION",
                operationSourcePNodeID = _pnodeID,
                operationAction = $"NodeEditReadme",
                operationCompletionStatus = "SUCCESS",
                operationSourceUserName = "Alice Wonder"
            };

            int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

            response.operationStatus = true;
            response.statusMessage = "PNode Readme successfully received!";

            return Ok(response);
        }
    }
}