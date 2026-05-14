using Microsoft.AspNetCore.Mvc;
using POWERENV_PGSQL_DB_HANDLER;
using System.Collections.Generic;
using static POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/backend/data")]
    public class SQLITE_DATA_CONTROLLER : Controller
    {
        private struct STRUCT_PGRID_INSIGHTS
        {
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ACCESS_POLICY_INFO> accessPolicies { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ACCESS_AUDIT_INFO> accessAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> pnodesLoginAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> pnodesErrorLogs { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ATTENTION_LED_PNODES_INFO> attentionLEDMarkedPNodes { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PPOOLS_LIST> ppoolsInfoList { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PGRID_FULL_INFO pgridFullInfo { get; set; }
        }
        
        private struct STRUCT_PPOOL_INSIGHTS
        {
            public PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PPOOL_FULL_INFO ppoolFullInfo { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODES_BASIC_INFO> ppoolPNodesFullList { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> ppoolLoginAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ATTENTION_LED_PNODES_INFO> ppoolAttentionLEDMarkedPNodes { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> ppoolErrorLogs { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PPOOLS_OPERATION_LOGS ppoolOperationLogs { get; set; }
        }

        private struct STRUCT_PNODE_INSIGHTS
        {
            public PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_FULL_INFO pnode_full_info { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_FSP_INFO pnodeFSPInfo { get; set; }
            public PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_MACHINE_INFO pnodeMachineInfo { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_NIC_INFO> pnodeNICInfo { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> pnodeLoginAudits { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODES_SINGLE_OPERATION_HISTORY> pnodeSingleOperationHistory { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> pnodeErrorLogs { get; set; }
            public List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_ETH_ACCESS_POLICY_INFO> pnodeETHAccessPolicies { get; set; }
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

            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PGRID_BASIC_INFO> pgridsInfoList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGrids();

            response.operationStatus = true;
            response.statusMessage = "PGrid Dashboard data successfully received!";
            response.packetData = pgridsInfoList;

            return Ok(response);
        }

        [HttpGet("pgrid{_pgridID}/")]
        public IActionResult DBGetPgridInsights([FromRoute] int _pgridID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ACCESS_POLICY_INFO> accessPolicies = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGAccessPolicies(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ACCESS_AUDIT_INFO> accessAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGAccessAudits(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> pnodesLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGPNLoginAudits(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> pnodesErrorLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGErrorLogs(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ATTENTION_LED_PNODES_INFO> attentionLEDMarkedPNodes = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetAttentionLEDPNodes(_pgridID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PPOOLS_LIST> ppoolsInfoList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGPPoolsList(_pgridID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PGRID_FULL_INFO pgridFullInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGridFullInfo(_pgridID);

            STRUCT_PGRID_INSIGHTS pgridInsights = new STRUCT_PGRID_INSIGHTS()
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

            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODES_BASIC_INFO> ppoolPNodesList = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPGPPoolPNodesList(_ppoolID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PPOOL_FULL_INFO ppoolFullInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolFullInfo(_ppoolID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> ppoolLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsLoginAudits(_ppoolID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_ATTENTION_LED_PNODES_INFO> ppoolAttentionLEDMarkedPNodes = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolAttentionLEDPNodes(_ppoolID);
            List <PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> ppoolErrorLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsErrorLogs(_ppoolID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PPOOLS_OPERATION_LOGS ppoolOperationLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPPoolsOperationLogs(_ppoolID);

            STRUCT_PPOOL_INSIGHTS ppoolInsights = new STRUCT_PPOOL_INSIGHTS()
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

            /*STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
            {
                operationCatName = "DOCUMENTATION",
                operationSourcePNodeID = _ppoolID,
                operationAction = $"PoolEditReadme",
                operationCompletionStatus = "SUCCESS",
                operationSourceUserName = "Alice Wonder"
            };

            int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);*/

            response.operationStatus = true;
            response.statusMessage = "PPool Readme successfully received!";

            return Ok(response);
        }

        [HttpGet("pgrid{_pgridID}/ppool{_ppoolID}/pnode{_pnodeID}")]
        public IActionResult DBGetPNodeInsights([FromRoute] int _pnodeID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();

            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_FULL_INFO pnodeFullInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_pnodeID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_FSP_INFO pnodeFSPInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFSPInfo(_pnodeID);
            PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_MACHINE_INFO pnodeMachineInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeMachineInfo(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_NIC_INFO> pnodeNICInfo = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeNICsInfo(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS> pnodeLoginAudits = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodesLoginAudits(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODES_SINGLE_OPERATION_HISTORY> pnodeSingleOperationHistory = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeOperationLogs(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO> pnodeErrorLogs = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodesErrorLogs(_pnodeID);
            List<PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_PNODE_ETH_ACCESS_POLICY_INFO> pnodeETHAccessPolicies = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeETHAccessPolicies(_pnodeID);

            STRUCT_PNODE_INSIGHTS ppoolInsights = new STRUCT_PNODE_INSIGHTS()
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

            STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
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