using Npgsql;
using NpgsqlTypes;
using System.Data;
using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        //============================PNODE DATA HANDLING METHODS============================//

        #region READ
        public PNodeFullInfo DBGetPNodeFullInfo(int _targetPNodeID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_FULL_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PNodeFullInfo pnodeFullInfo = new PNodeFullInfo();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                pnodeFullInfo = new PNodeFullInfo()
                {
                    pnode_id = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    pnode_nickname = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    pnode_parent_ppool_name = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    pnode_config_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][3]).ToString(),
                    pnode_last_update_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][4]).ToString(),
                    pnode_last_heartbeat_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][9]).ToString(),
                    pnode_attention_led_state = (string)(connectionInfo.resultsDataTable.Rows[i][10]),
                    pnode_readme_text = (string)(connectionInfo.resultsDataTable.Rows[i][11]),
                    pnodeActivenessState = (string)(connectionInfo.resultsDataTable.Rows[i][12]) == "ACTIVE" ? true : false,
                    pnodeSerialCOMPortId = (string)(connectionInfo.resultsDataTable.Rows[i][13])
                };
            }

            return pnodeFullInfo;
        }

        public PNodeFSPInfo DBGetPNodeFSPInfo(int _targetPNodeID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_FSP_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PNodeFSPInfo pnodeFSPInfo = new PNodeFSPInfo();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                string passphrase = "";

                for (int j = 0; j < ((string)(connectionInfo.resultsDataTable.Rows[i][3])).Length; j++)
                {
                    passphrase += "*";
                }

                pnodeFSPInfo = new PNodeFSPInfo()
                {
                    FSPID = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    FSPASMIVersion = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    FSPASMIUsername = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    FSPASMIPasswordHash = passphrase,
                    FSPASMILocalTime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][4]).ToString()
                };
            }

            return pnodeFSPInfo;
        }

        public PNodeMachineInfo DBGetPNodeMachineInfo(int _targetPNodeID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_MACHINE_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PNodeMachineInfo pnodeMachineInfo = new PNodeMachineInfo();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                pnodeMachineInfo = new PNodeMachineInfo()
                {
                    pnode_system_model_name = (string)(connectionInfo.resultsDataTable.Rows[i][0]),
                    pnode_machine_type_model = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    pnode_machine_serial_number = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    pnode_system_pseries = (string)(connectionInfo.resultsDataTable.Rows[i][3])
                };
            }

            return pnodeMachineInfo;
        }

        public List<PNodeNICInfo> DBGetPNodeNICsInfo(int _targetPNodeID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_NICS_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodeNICInfo> pnodeNICsInfo = new List<PNodeNICInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PNodeNICInfo pnodeIndividualNICInfo = new PNodeNICInfo
                {
                    pnode_nic_id = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    pnode_nic_name = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    pnode_nic_mac_address = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    pnode_nic_ip_address = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    pnode_nic_ip_address_type = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    pnode_nic_subnet_mask = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    pnode_nic_default_gateway = (string)(connectionInfo.resultsDataTable.Rows[i][6]),
                    pnode_nic_hostname = (string)(connectionInfo.resultsDataTable.Rows[i][7]),
                    pnode_nic_domain_name = (string)(connectionInfo.resultsDataTable.Rows[i][8]),
                    pnode_nic_first_dns_ip_address = (string)(connectionInfo.resultsDataTable.Rows[i][9]),
                    pnode_nic_second_dns_ip_address = (string)(connectionInfo.resultsDataTable.Rows[i][10]),
                    pnode_nic_third_dns_ip_address = (string)(connectionInfo.resultsDataTable.Rows[i][11]),
                    pnode_nic_type = (string)(connectionInfo.resultsDataTable.Rows[i][12])
                };

                pnodeNICsInfo.Add(pnodeIndividualNICInfo);
            }

            return pnodeNICsInfo;
        }

        public List<PNodeETHAccessPolicyInfo> DBGetPNodeETHAccessPolicies(int _targetPNodeID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_ETH_ACCESS_POLICIES(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodeETHAccessPolicyInfo> pnodeETHAccessPoliciesInfo = new List<PNodeETHAccessPolicyInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PNodeETHAccessPolicyInfo pnodeIndividualNICInfo = new PNodeETHAccessPolicyInfo()
                {
                    access_policy_id = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    access_policy_index_id = (int)(connectionInfo.resultsDataTable.Rows[i][1]),
                    access_policy_ip_address = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    access_policy_type = (string)(connectionInfo.resultsDataTable.Rows[i][3])
                };

                pnodeETHAccessPoliciesInfo.Add(pnodeIndividualNICInfo);
            }

            return pnodeETHAccessPoliciesInfo;
        }

        public List<NodesLoginAudits> DBGetPNodesLoginAudits(int _targetPNode)
        {
            string sqlCommandText = $"CALL SP_GET_PNODES_LOGIN_AUDITS(@targetPNode, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNode", Value = _targetPNode }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<NodesLoginAudits> pnodesLoginAudits = new List<NodesLoginAudits>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                NodesLoginAudits pnodeLoginAudit = new NodesLoginAudits
                {
                    login_audit_id = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    login_audit_fsp_user = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    login_audit_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][2]).ToString(),
                    login_audit_login_status = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    login_audit_location = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    login_audit_pnode_nickname = (string)(connectionInfo.resultsDataTable.Rows[i][5])
                };

                pnodesLoginAudits.Add(pnodeLoginAudit);
            }

            return pnodesLoginAudits;
        }

        public List<PNodesSingleOperationHistory> DBGetPNodeOperationLogs(int _targetPNodeID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_OPERATION_LOGS(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodesSingleOperationHistory> ppoolPNodesSingleOperationHistory = new List<PNodesSingleOperationHistory>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PNodesSingleOperationHistory ppoolPNodesSingleOperationLog = new PNodesSingleOperationHistory()
                {
                    operationID = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    operationCatName = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    operationSourcePNodeName = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    operationBatchOperationID = (int)(connectionInfo.resultsDataTable.Rows[i][3]),
                    operationBatchOperationName = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    operationAction = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    operationCompletionStatus = (string)(connectionInfo.resultsDataTable.Rows[i][6]),
                    operationDateTime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][7]).ToString(),
                    operationSourceUserName = (string)(connectionInfo.resultsDataTable.Rows[i][8])
                };

                ppoolPNodesSingleOperationHistory.Add(ppoolPNodesSingleOperationLog);
            }

            return ppoolPNodesSingleOperationHistory;
        }

        public List<FSPErrorLogInfo> DBGetPNodesErrorLogs(int _targetPNode)
        {
            string sqlCommandText = $"CALL SP_GET_PNODES_ERROR_LOGS(@targetPNode, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNode", Value = _targetPNode }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<FSPErrorLogInfo> pnodeErrorLogs = new List<FSPErrorLogInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                List<string> actionFlags = new List<string>();
                string[] a = ((string)(connectionInfo.resultsDataTable.Rows[i][6])).Split(", ");

                for (int j = 0; j < a.Length; j++)
                {
                    actionFlags.Add(a[j]);
                }

                string[] logDateNTime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][1])).ToString().Split(" ");

                FSPErrorLogInfo pnodeErrorLog = new FSPErrorLogInfo()
                {
                    ErrorLogID = (string)(connectionInfo.resultsDataTable.Rows[i][0]),
                    LogDate = logDateNTime[0],
                    LogTime = logDateNTime[1],
                    DriverName = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    Subsystem = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    RawData = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    EventSeverity = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    ActionFlags = actionFlags,
                    ActionStatus = (string)(connectionInfo.resultsDataTable.Rows[i][7]),
                    ReferenceCode = (string)(connectionInfo.resultsDataTable.Rows[i][8]),
                    PNodeNickname = (string)(connectionInfo.resultsDataTable.Rows[i][9])
                };

                pnodeErrorLogs.Add(pnodeErrorLog);
            }

            return pnodeErrorLogs;
        }

        public List<LPARBasicInfo> DBGetPNodeLPARS(int PNode_ID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_LPARS(@PNode_ID, 'CURSOR');" +
                $"FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "PNode_ID", Value = PNode_ID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<LPARBasicInfo> lparsInfo = new List<LPARBasicInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                LPARBasicInfo lparInfo = new LPARBasicInfo
                {
                    lpar_id = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    lpar_name = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    lpar_os_instance = (int)(connectionInfo.resultsDataTable.Rows[i][2]),
                    is_main_os_host = (int)(connectionInfo.resultsDataTable.Rows[i][3]) == 1 ? true : false,
                    lpar_storage_size = (int)(connectionInfo.resultsDataTable.Rows[i][4])
                };

                lparsInfo.Add(lparInfo);
            }

            return lparsInfo;
        }

        public LPARFullInfo DBGetPNodeMainOSLPARInfo(int PNode_ID)
        {
            string sqlCommandText = $"CALL SP_GET_PNODE_MAIN_OS_LPAR_INFO(@PNode_ID, 'CURSOR');" +
                $"FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "PNode_ID", Value = PNode_ID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            AuthInfo osAuthInfo = new AuthInfo(
                (string)(connectionInfo.resultsDataTable.Rows[0][1]),
                (string)(connectionInfo.resultsDataTable.Rows[0][2])
            );

            LPARFullInfo osInfo = new LPARFullInfo
            {
                lpar_id = (int)(connectionInfo.resultsDataTable.Rows[0][5]),
                lpar_name = (string)(connectionInfo.resultsDataTable.Rows[0][6]),
                is_main_os_host = true,
                lpar_storage_size = (int)(connectionInfo.resultsDataTable.Rows[0][7]),
                lpar_target_pnode_id = PNode_ID,
                os_id = (int)(connectionInfo.resultsDataTable.Rows[0][0]),
                osAuthInfo = osAuthInfo,
                os_ip_address = (string)(connectionInfo.resultsDataTable.Rows[0][3]),
                os_family = (string)(connectionInfo.resultsDataTable.Rows[0][4])
            };

            return osInfo;
        }

        #endregion READ
        #region WRITE

        public int updatePNodeActivenessState(int pnodeID, int newActivenessStateID)
        {
            string sqlCommandText = $"CALL SP_UPDATE_PNODE_ACTIVENESS_STATE(" +
                $"@pnodeID," +
                $"@newActivenessStateID," +
                $"NULL" +
                $");";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = pnodeID },
                new SQL_QUERY_PARAMETER { Name = "newActivenessStateID", Value = newActivenessStateID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters);

            return connectionInfo.rowsAffected;
        }

        public int updatePNodeAttentionLEDState(int pnodeID, string newLEDState)
        {
            string sqlCommandText = $"CALL SP_UPDATE_PNODES_ATTENTIONLED_STATE(" +
                $"@pnodeID," +
                $"@newLEDState," +
                $"NULL" +
                $");";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = pnodeID },
                new SQL_QUERY_PARAMETER { Name = "newLEDState", Value = newLEDState }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters);

            return connectionInfo.rowsAffected;
        }

        public int updatePNodeNICsInfo(PNodeNICInfo _newNICInfo)
        {
            string sqlCommandText = $"CALL SP_UPDATE_PNODE_NICS_INFO(@pnode_id," +
                $"@pnode_nic_mac_address," +
                $"@pnode_nic_ip_address," +
                $"@pnode_nic_ip_address_type," +
                $"@pnode_nic_subnet_mask," +
                $"@pnode_nic_default_gateway," +
                $"@pnode_nic_hostname," +
                $"@pnode_nic_domain_name," +
                $"@pnode_nic_first_dns_ip_address," +
                $"@pnode_nic_second_dns_ip_address," +
                $"@pnode_nic_third_dns_ip_address," +
                $"@pnode_nic_type," +
                $"@pnode_id," +
                $"NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnode_id", Value = _newNICInfo.pnode_id },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_mac_address", Value = _newNICInfo.pnode_nic_mac_address },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_ip_address", Value = _newNICInfo.pnode_nic_ip_address },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_ip_address_type", Value = _newNICInfo.pnode_nic_ip_address_type },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_subnet_mask", Value = _newNICInfo.pnode_nic_subnet_mask },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_default_gateway", Value = _newNICInfo.pnode_nic_default_gateway },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_hostname", Value = _newNICInfo.pnode_nic_hostname },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_domain_name", Value = _newNICInfo.pnode_nic_domain_name },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_first_dns_ip_address", Value = _newNICInfo.pnode_nic_first_dns_ip_address },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_second_dns_ip_address", Value = _newNICInfo.pnode_nic_second_dns_ip_address },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_third_dns_ip_address", Value = _newNICInfo.pnode_nic_third_dns_ip_address },
                new SQL_QUERY_PARAMETER { Name = "pnode_nic_type", Value = _newNICInfo.pnode_nic_type }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int insertPNodeETHAccessPolicy(PNodeETHAccessPolicyInfo newETHAccessPolicy)
        {
            string sqlCommandText = $"CALL SP_INSERT_PNODE_ETH_ACCESS_POLICY(@access_policy_pnode_id," +
                $"@access_policy_index_id," +
                $"@access_policy_ip_address," +
                $"@access_policy_type," +
                $"NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "access_policy_pnode_id", Value = newETHAccessPolicy.access_policy_pnode_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_index_id", Value = newETHAccessPolicy.access_policy_index_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_ip_address", Value = newETHAccessPolicy.access_policy_ip_address },
                new SQL_QUERY_PARAMETER { Name = "access_policy_type", Value = newETHAccessPolicy.access_policy_type }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int updatePNodeETHAccessPolicies(PNodeETHAccessPolicyInfo _updatedPolicy)
        {
            string sqlCommandText = $"CALL SP_UPDATE_PNODE_ETH_ACCESS_POLICIES(@access_policy_index_id," +
                $"@access_policy_ip_address," +
                $"@access_policy_type," +
                $"@access_policy_id," +
                $"NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "access_policy_index_id", Value = _updatedPolicy.access_policy_index_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_ip_address", Value = _updatedPolicy.access_policy_ip_address },
                new SQL_QUERY_PARAMETER { Name = "access_policy_type", Value = int.Parse(_updatedPolicy.access_policy_type) },
                new SQL_QUERY_PARAMETER { Name = "access_policy_id", Value = _updatedPolicy.access_policy_id }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int deletePNodeETHAccessPolicy(PNodeETHAccessPolicyInfo ETHAccessPolicy)
        {
            string sqlCommandText = $"CALL SP_DELETE_PNODE_ETH_ACCESS_POLICY(@access_policy_index_id," +
                $"@access_policy_type," +
                $"@access_policy_pnode_id," +
                $"NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "access_policy_index_id", Value = ETHAccessPolicy.access_policy_index_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_type", Value = ETHAccessPolicy.access_policy_type },
                new SQL_QUERY_PARAMETER { Name = "access_policy_pnode_id", Value = ETHAccessPolicy.access_policy_pnode_id }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodeSingleOperation(PNodesSingleOperationHistory OperationData)
        {
            string sqlCommandText = $"CALL SP_INSERT_PNODE_SINGLE_OPERATION(@operationCatName," +
                $"@operationCompletionStatus," +
                $"@operationSourceUserName," +
                $"@operationSourcePNodeID," +
                $"@operationAction," +
                $"@operationDescription," +
                $"@operationSeverityLevelID," +
                $"NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "operationCatName", Value = OperationData.operationCatName ?? "" },
                new SQL_QUERY_PARAMETER { Name = "operationCompletionStatus", Value = OperationData.operationCompletionStatus ?? "" },
                new SQL_QUERY_PARAMETER { Name = "operationSourceUserName", Value = OperationData.operationSourceUserName ?? "" },
                new SQL_QUERY_PARAMETER { Name = "operationSourcePNodeID", Value = OperationData.operationSourcePNodeID ?? 1 },
                new SQL_QUERY_PARAMETER { Name = "operationAction", Value = OperationData.operationAction ?? "" },
                new SQL_QUERY_PARAMETER { Name = "operationDescription", Value = OperationData.operationDescription ?? "" },
                new SQL_QUERY_PARAMETER { Name = "operationSeverityLevelID", Value = OperationData.operationSeverityLevelID ?? 1 }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditReadme(int pnodeID, string newReadmeText)
        {
            string sqlCommandText = $"CALL SP_PNODE_EDIT_README(" +
                $"@pnodeID," +
                $"@newReadmeText," +
                $"NULL" +
                $");";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = pnodeID },
                new SQL_QUERY_PARAMETER { Name = "newReadmeText", Value = newReadmeText }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditDateTime(int _pnodeID, string _date, string _time)
        {
            string tempDATE = null;
            string tempTIME = _time;

            if (_date != null) tempDATE = $"{_date.Split("-")[2]}-{_date.Split("-")[0]}-{_date.Split("-")[1]}";

            string sqlCommandText = $"CALL SP_PNODE_EDIT_DATETIME(" +
                $"@pnodeID," +
                $"@date," +
                $"@time," +
                $"NULL" +
                $");";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = _pnodeID },
                new SQL_QUERY_PARAMETER { Name = "date", Value = tempDATE },
                new SQL_QUERY_PARAMETER { Name = "time", Value = tempTIME }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodeErrorLog(int _PNodeID, PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo _currErrorLog)
        {
            string actionFlags = "";

            for (int i = 0; i < _currErrorLog.ActionFlags.Count - 1; i++)
            {
                actionFlags += $"{_currErrorLog.ActionFlags[i]}, ";
            }

            actionFlags += $"{_currErrorLog.ActionFlags[_currErrorLog.ActionFlags.Count - 1]}";

            string sqlCommandText = $"CALL SP_INSERT_PNODE_ERROR_LOG(" +
                $"@ErrorLogID," +
                $"@LogDate," +
                $"@LogTime," +
                $"@DriverName," +
                $"@Subsystem," +
                $"@RawData," +
                $"@EventSeverity," +
                $"@ActionFlags," +
                $"@ActionStatus," +
                $"@ReferenceCode," +
                $"@PNodeID," +
                $"@NormalHardwareFRU," +
                $"NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "ErrorLogID", Value = _currErrorLog.ErrorLogID },
                new SQL_QUERY_PARAMETER { Name = "LogDate", Value = _currErrorLog.LogDate },
                new SQL_QUERY_PARAMETER { Name = "LogTime", Value = _currErrorLog.LogTime },
                new SQL_QUERY_PARAMETER { Name = "DriverName", Value = _currErrorLog.DriverName },
                new SQL_QUERY_PARAMETER { Name = "Subsystem", Value = _currErrorLog.Subsystem },
                new SQL_QUERY_PARAMETER { Name = "RawData", Value = _currErrorLog.RawData },
                new SQL_QUERY_PARAMETER { Name = "EventSeverity", Value = _currErrorLog.EventSeverity },
                new SQL_QUERY_PARAMETER { Name = "ActionFlags", Value = actionFlags },
                new SQL_QUERY_PARAMETER { Name = "ActionStatus", Value = _currErrorLog.ActionStatus },
                new SQL_QUERY_PARAMETER { Name = "ReferenceCode", Value = _currErrorLog.ReferenceCode },
                new SQL_QUERY_PARAMETER { Name = "PNodeID", Value = _PNodeID },
                new SQL_QUERY_PARAMETER { Name = "NormalHardwareFRU", Value = _currErrorLog.NormalHardwareFRU }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        private int DBInsertPNodeErrorLogNHFRURecord(PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogFRUInfo NHFRURecord, int errorLogDBID)
        {
            string sqlCommandText = $"CALL SP_INSERT_PNODE_ERROR_LOG_NHFRU_RECORD(@NHFRURecord, @errorLogDBID, NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "NHFRURecord", Value = NHFRURecord },
                new SQL_QUERY_PARAMETER { Name = "errorLogDBID", Value = errorLogDBID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodesLoginAudits(int _targetPNode, PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits loginAudit)
        {
            string sqlCommandText = $"CALL SP_INSERT_PNODES_LOGIN_AUDITS(" +
                $"@targetPNode," +
                $"@login_audit_fsp_user," +
                $"@login_audit_datetime," +
                $"@login_audit_login_status," +
                $"@login_audit_location," +
                $"NULL);";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNode", Value = _targetPNode },
                new SQL_QUERY_PARAMETER { Name = "login_audit_fsp_user", Value = loginAudit.login_audit_fsp_user },
                new SQL_QUERY_PARAMETER { Name = "login_audit_datetime", Value = loginAudit.login_audit_datetime },
                new SQL_QUERY_PARAMETER { Name = "login_audit_login_status", Value = loginAudit.login_audit_login_status },
                new SQL_QUERY_PARAMETER { Name = "login_audit_location", Value = loginAudit.login_audit_location }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }

        public int DBCreateNewPNode(int userID, PNodesBasicInfoPGSQLCompositeType pnodeBasicInfo, PNodeFSPInfo pnodeFSPInfo, OSUserInfoPGSQLCompositeType pnodeOSUserInfoType)
        {
            string sqlCommandText = $"CALL SP_CREATE_PNODE(" +
                $"@pnodeBasicInfo," +
                $"@pnodeFSPInfo," +
                $"@pnodeOSUserInfoType," +
                $"NULL" +
                $");";

            pnodeFSPInfo.FSPASMILocalTime = $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeBasicInfo", Value = pnodeBasicInfo, SQLType = "public.pnode_basic_info_type" },
                new SQL_QUERY_PARAMETER { Name = "pnodeFSPInfo", Value = pnodeFSPInfo, SQLType = "public.pnode_fsp_info_type" },
                new SQL_QUERY_PARAMETER { Name = "pnodeOSUserInfoType", Value = pnodeOSUserInfoType, SQLType = "public.pnode_os_user_info_type" }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters);
            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}