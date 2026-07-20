using POWERENV_DB_HANDLER;
using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        //============================PNODE DATA HANDLING METHODS============================//

        #region READ
        public PNodeFullInfo DBGetPNodeFullInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_FULL_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PNodeFullInfo pnodeFullInfo = new PNodeFullInfo();

            while (connectionInfo.reader.Read())
            {
                pnodeFullInfo = new PNodeFullInfo()
                {
                    pnode_id = connectionInfo.reader.GetInt32(0),
                    pnode_nickname = connectionInfo.reader.GetString(1),
                    pnode_parent_ppool_name = connectionInfo.reader.GetString(2),
                    pnode_config_datetime = connectionInfo.reader.GetDateTime(3).ToString(),
                    pnode_last_update_datetime = connectionInfo.reader.GetDateTime(4).ToString(),
                    pnode_last_heartbeat_datetime = connectionInfo.reader.GetDateTime(9).ToString(),
                    pnode_attention_led_state = connectionInfo.reader.GetString(10),
                    pnode_readme_text = connectionInfo.reader.GetString(11),
                    pnodeActivenessState = connectionInfo.reader.GetString(12) == "ACTIVE" ? true : false,
                    pnodeSerialCOMPortId = connectionInfo.reader.GetString(13)
                };
            }

            connectionInfo.conn.Close();

            return pnodeFullInfo;
        }

        public PNodeFSPInfo DBGetPNodeFSPInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_FSP_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PNodeFSPInfo pnodeFSPInfo = new PNodeFSPInfo();

            while (connectionInfo.reader.Read())
            {
                string passphrase = "";

                for (int i = 0; i < connectionInfo.reader.GetString(3).Length; i++)
                {
                    passphrase += "*";
                }

                pnodeFSPInfo = new PNodeFSPInfo()
                {
                    pnode_fsp_id = connectionInfo.reader.GetInt32(0),
                    pnode_fsp_asmi_version = connectionInfo.reader.GetString(1),
                    pnode_fsp_asmi_username = connectionInfo.reader.GetString(2),
                    pnode_fsp_asmi_password_hash = passphrase,
                    pnode_fsp_asmi_local_datetime = connectionInfo.reader.GetDateTime(4).ToString()
                };
            }

            connectionInfo.conn.Close();

            return pnodeFSPInfo;
        }

        public PNodeMachineInfo DBGetPNodeMachineInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_MACHINE_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PNodeMachineInfo pnodeMachineInfo = new PNodeMachineInfo();

            while (connectionInfo.reader.Read())
            {
                pnodeMachineInfo = new PNodeMachineInfo()
                {
                    pnode_system_model_name = connectionInfo.reader.GetString(0),
                    pnode_machine_type_model = connectionInfo.reader.GetString(1),
                    pnode_machine_serial_number = connectionInfo.reader.GetString(2),
                    pnode_system_pseries = connectionInfo.reader.GetString(3)
                };
            }

            connectionInfo.conn.Close();

            return pnodeMachineInfo;
        }

        public List<PNodeNICInfo> DBGetPNodeNICsInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_NICS_INFO(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodeNICInfo> pnodeNICsInfo = new List<PNodeNICInfo>();

            while (connectionInfo.reader.Read())
            {
                PNodeNICInfo pnodeIndividualNICInfo = new PNodeNICInfo
                {
                    pnode_nic_id = connectionInfo.reader.GetInt32(0),
                    pnode_nic_name = connectionInfo.reader.GetString(1),
                    pnode_nic_mac_address = connectionInfo.reader.GetString(2),
                    pnode_nic_ip_address = connectionInfo.reader.GetString(3),
                    pnode_nic_ip_address_type = connectionInfo.reader.GetString(4),
                    pnode_nic_subnet_mask = connectionInfo.reader.GetString(5),
                    pnode_nic_default_gateway = connectionInfo.reader.GetString(6),
                    pnode_nic_hostname = connectionInfo.reader.GetString(7),
                    pnode_nic_domain_name = connectionInfo.reader.GetString(8),
                    pnode_nic_first_dns_ip_address = connectionInfo.reader.GetString(9),
                    pnode_nic_second_dns_ip_address = connectionInfo.reader.GetString(10),
                    pnode_nic_third_dns_ip_address = connectionInfo.reader.GetString(11),
                    pnode_nic_type = connectionInfo.reader.GetString(12)
                };

                pnodeNICsInfo.Add(pnodeIndividualNICInfo);
            }

            connectionInfo.conn.Close();

            return pnodeNICsInfo;
        }

        public List<PNodeETHAccessPolicyInfo> DBGetPNodeETHAccessPolicies(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_ETH_ACCESS_POLICIES(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodeETHAccessPolicyInfo> pnodeETHAccessPoliciesInfo = new List<PNodeETHAccessPolicyInfo>();

            while (connectionInfo.reader.Read())
            {
                PNodeETHAccessPolicyInfo pnodeIndividualNICInfo = new PNodeETHAccessPolicyInfo()
                {
                    access_policy_id = connectionInfo.reader.GetInt32(0),
                    access_policy_index_id = connectionInfo.reader.GetInt32(1),
                    access_policy_ip_address = connectionInfo.reader.GetString(2),
                    access_policy_type = connectionInfo.reader.GetString(3)
                };

                pnodeETHAccessPoliciesInfo.Add(pnodeIndividualNICInfo);
            }

            connectionInfo.conn.Close();

            return pnodeETHAccessPoliciesInfo;
        }

        public List<NodesLoginAudits> DBGetPNodesLoginAudits(int _targetPNode)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODES_LOGIN_AUDITS(@targetPNode, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNode", Value = _targetPNode }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<NodesLoginAudits> pnodesLoginAudits = new List<NodesLoginAudits>();

            while (connectionInfo.reader.Read())
            {
                NodesLoginAudits pnodeLoginAudit = new NodesLoginAudits
                {
                    login_audit_id = connectionInfo.reader.GetInt32(0),
                    login_audit_fsp_user = connectionInfo.reader.GetString(1),
                    login_audit_datetime = connectionInfo.reader.GetDateTime(2).ToString(),
                    login_audit_login_status = connectionInfo.reader.GetString(3),
                    login_audit_location = connectionInfo.reader.GetString(4),
                    login_audit_pnode_nickname = connectionInfo.reader.GetString(5)
                };

                pnodesLoginAudits.Add(pnodeLoginAudit);
            }

            connectionInfo.conn.Close();

            return pnodesLoginAudits;
        }

        public List<PNodesSingleOperationHistory> DBGetPNodeOperationLogs(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_OPERATION_LOGS(@targetPNodeID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNodeID", Value = _targetPNodeID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodesSingleOperationHistory> ppoolPNodesSingleOperationHistory = new List<PNodesSingleOperationHistory>();

            while (connectionInfo.reader.Read())
            {
                PNodesSingleOperationHistory ppoolPNodesSingleOperationLog = new PNodesSingleOperationHistory()
                {
                    operationID = connectionInfo.reader.GetInt32(0),
                    operationCatName = connectionInfo.reader.GetString(1),
                    operationSourcePNodeName = connectionInfo.reader.GetString(2),
                    operationBatchOperationID = connectionInfo.reader.GetInt32(3),
                    operationBatchOperationName = connectionInfo.reader.GetString(4),
                    operationAction = connectionInfo.reader.GetString(5),
                    operationCompletionStatus = connectionInfo.reader.GetString(6),
                    operationDateTime = connectionInfo.reader.GetDateTime(7).ToString(),
                    operationSourceUserName = connectionInfo.reader.GetString(8)
                };

                ppoolPNodesSingleOperationHistory.Add(ppoolPNodesSingleOperationLog);
            }

            connectionInfo.conn.Close();

            return ppoolPNodesSingleOperationHistory;
        }

        public List<FSPErrorLogInfo> DBGetPNodesErrorLogs(int _targetPNode)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODES_ERROR_LOGS(@targetPNode, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNode", Value = _targetPNode }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<FSPErrorLogInfo> pnodeErrorLogs = new List<FSPErrorLogInfo>();

            while (connectionInfo.reader.Read())
            {
                List<string> actionFlags = new List<string>();
                string[] a = connectionInfo.reader.GetString(6).Split(", ");

                for (int i = 0; i < a.Length; i++)
                {
                    actionFlags.Add(a[i]);
                }

                string[] logDateNTime = connectionInfo.reader.GetDateTime(1).ToString().Split(" ");

                FSPErrorLogInfo pnodeErrorLog = new FSPErrorLogInfo()
                {
                    ErrorLogID = connectionInfo.reader.GetString(0),
                    LogDate = logDateNTime[0],
                    LogTime = logDateNTime[1],
                    DriverName = connectionInfo.reader.GetString(2),
                    Subsystem = connectionInfo.reader.GetString(3),
                    RawData = connectionInfo.reader.GetString(4),
                    EventSeverity = connectionInfo.reader.GetString(5),
                    ActionFlags = actionFlags,
                    ActionStatus = connectionInfo.reader.GetString(7),
                    ReferenceCode = connectionInfo.reader.GetString(8),
                    PNodeNickname = connectionInfo.reader.GetString(9)
                };

                pnodeErrorLogs.Add(pnodeErrorLog);
            }

            connectionInfo.conn.Close();

            return pnodeErrorLogs;
        }

        public List<LPARBasicInfo> DBGetPNodeLPARS(int PNode_ID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_LPARS(@PNode_ID, 'CURSOR');" +
                $"FETCH ALL FROM \"CURSOR\";" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "PNode_ID", Value = PNode_ID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<LPARBasicInfo> lparsInfo = new List<LPARBasicInfo>();

            while (connectionInfo.reader.Read())
            {
                LPARBasicInfo lparInfo = new LPARBasicInfo
                {
                    lpar_id = connectionInfo.reader.GetInt32(0),
                    lpar_name = connectionInfo.reader.GetString(1),
                    lpar_os_instance = connectionInfo.reader.GetInt32(2),
                    is_main_os_host = connectionInfo.reader.GetInt32(3) == 1 ? true : false,
                    lpar_storage_size = connectionInfo.reader.GetInt32(4)
                };

                lparsInfo.Add(lparInfo);
            }

            connectionInfo.conn.Close();

            return lparsInfo;
        }

        public LPARFullInfo DBGetPNodeMainOSLPARInfo(int PNode_ID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_MAIN_OS_LPAR_INFO(@PNode_ID, 'CURSOR');" +
                $"FETCH ALL FROM \"CURSOR\";" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "PNode_ID", Value = PNode_ID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            connectionInfo.reader.Read();

            AuthInfo osAuthInfo = new AuthInfo(
                connectionInfo.reader.GetString(1),
                connectionInfo.reader.GetString(2)
            );

            LPARFullInfo osInfo = new LPARFullInfo
            {
                lpar_id = connectionInfo.reader.GetInt32(5),
                lpar_name = connectionInfo.reader.GetString(6),
                is_main_os_host = true,
                lpar_storage_size = connectionInfo.reader.GetInt32(7),
                lpar_target_pnode_id = PNode_ID,
                os_id = connectionInfo.reader.GetInt32(0),
                osAuthInfo = osAuthInfo,
                os_ip_address = connectionInfo.reader.GetString(3),
                os_family = connectionInfo.reader.GetString(4)
            };

            connectionInfo.conn.Close();

            return osInfo;
        }

        #endregion READ
        #region WRITE

        public int updatePNodeActivenessState(int pnodeID, int newActivenessStateID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODE_ACTIVENESS_STATE(@pnodeID, @newActivenessStateID, NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = pnodeID },
                new SQL_QUERY_PARAMETER { Name = "newActivenessStateID", Value = newActivenessStateID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters);

            return connectionInfo.rowsAffected;
        }

        public int updatePNodeAttentionLEDState(int pnodeID, string newLEDState)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODES_ATTENTIONLED_STATE(@pnodeID, @newLEDState, NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = pnodeID },
                new SQL_QUERY_PARAMETER { Name = "newLEDState", Value = newLEDState }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters);

            return connectionInfo.rowsAffected;
        }

        public int updatePNodeNICsInfo(PNodeNICInfo _newNICInfo)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODE_NICS_INFO(@pnode_id," +
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
                $"NULL);" +
                $"COMMIT;";

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

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int insertPNodeETHAccessPolicy(PNodeETHAccessPolicyInfo newETHAccessPolicy)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_ETH_ACCESS_POLICY(@access_policy_pnode_id," +
                $"@access_policy_index_id," +
                $"@access_policy_ip_address," +
                $"@access_policy_type," +
                $"NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "access_policy_pnode_id", Value = newETHAccessPolicy.access_policy_pnode_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_index_id", Value = newETHAccessPolicy.access_policy_index_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_ip_address", Value = newETHAccessPolicy.access_policy_ip_address },
                new SQL_QUERY_PARAMETER { Name = "access_policy_type", Value = newETHAccessPolicy.access_policy_type }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int updatePNodeETHAccessPolicies(PNodeETHAccessPolicyInfo _updatedPolicy)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODE_ETH_ACCESS_POLICIES(@access_policy_index_id," +
                $"@access_policy_ip_address," +
                $"@access_policy_type," +
                $"@access_policy_id," +
                $"NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "access_policy_index_id", Value = _updatedPolicy.access_policy_index_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_ip_address", Value = _updatedPolicy.access_policy_ip_address },
                new SQL_QUERY_PARAMETER { Name = "access_policy_type", Value = int.Parse(_updatedPolicy.access_policy_type) },
                new SQL_QUERY_PARAMETER { Name = "access_policy_id", Value = _updatedPolicy.access_policy_id }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int deletePNodeETHAccessPolicy(PNodeETHAccessPolicyInfo ETHAccessPolicy)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_DELETE_PNODE_ETH_ACCESS_POLICY(@access_policy_index_id," +
                $"@access_policy_type," +
                $"@access_policy_pnode_id," +
                $"NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "access_policy_index_id", Value = ETHAccessPolicy.access_policy_index_id },
                new SQL_QUERY_PARAMETER { Name = "access_policy_type", Value = ETHAccessPolicy.access_policy_type },
                new SQL_QUERY_PARAMETER { Name = "access_policy_pnode_id", Value = ETHAccessPolicy.access_policy_pnode_id }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodeSingleOperation(PNodesSingleOperationHistory OperationData)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_SINGLE_OPERATION(@operationCatName," +
                $"@operationCompletionStatus," +
                $"@operationSourceUserName," +
                $"@operationSourcePNodeID," +
                $"@operationAction," +
                $"NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "operationCatName", Value = OperationData.operationCatName },
                new SQL_QUERY_PARAMETER { Name = "operationCompletionStatus", Value = OperationData.operationCompletionStatus },
                new SQL_QUERY_PARAMETER { Name = "operationSourceUserName", Value = OperationData.operationSourceUserName },
                new SQL_QUERY_PARAMETER { Name = "operationSourcePNodeID", Value = OperationData.operationSourcePNodeID },
                new SQL_QUERY_PARAMETER { Name = "operationAction", Value = OperationData.operationAction }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditReadme(int pnodeID, string newReadmeText)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_PNODE_EDIT_README(@pnodeID, @newReadmeText, NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = pnodeID },
                new SQL_QUERY_PARAMETER { Name = "newReadmeText", Value = newReadmeText }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditDateTime(int _pnodeID, string _date, string _time)
        {
            string tempDATE = null;
            string tempTIME = _time;

            if (_date != null) tempDATE = $"{_date.Split("-")[2]}-{_date.Split("-")[0]}-{_date.Split("-")[1]}";

            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_PNODE_EDIT_DATETIME(@pnodeID, @date, @time, NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pnodeID", Value = _pnodeID },
                new SQL_QUERY_PARAMETER { Name = "date", Value = tempDATE },
                new SQL_QUERY_PARAMETER { Name = "time", Value = tempTIME }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodeErrorLog(int _PNodeID, PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogInfo _currErrorLog)
        {
            string actionFlags = "";

            for(int i = 0; i < _currErrorLog.ActionFlags.Count - 1; i++)
            {
                actionFlags += $"{_currErrorLog.ActionFlags[i]}, ";
            }

            actionFlags += $"{_currErrorLog.ActionFlags[_currErrorLog.ActionFlags.Count - 1]}";

            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_ERROR_LOG(" +
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
                $"NULL);" +
                $"COMMIT;";

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

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        private int DBInsertPNodeErrorLogNHFRURecord(PSYSTEMS_HARDWARE_DATA_HANDLING.FSPErrorLogFRUInfo NHFRURecord, int errorLogDBID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_ERROR_LOG_NHFRU_RECORD(@NHFRURecord, @errorLogDBID, NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "NHFRURecord", Value = NHFRURecord },
                new SQL_QUERY_PARAMETER { Name = "errorLogDBID", Value = errorLogDBID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodesLoginAudits(int _targetPNode, PSYSTEMS_HARDWARE_DATA_HANDLING.NodesLoginAudits loginAudit)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODES_LOGIN_AUDITS(" +
                $"@targetPNode," +
                $"@login_audit_fsp_user," +
                $"@login_audit_datetime," +
                $"@login_audit_login_status," +
                $"@login_audit_location," +
                $"NULL);" +
                $"COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPNode", Value = _targetPNode },
                new SQL_QUERY_PARAMETER { Name = "login_audit_fsp_user", Value = loginAudit.login_audit_fsp_user },
                new SQL_QUERY_PARAMETER { Name = "login_audit_datetime", Value = loginAudit.login_audit_datetime },
                new SQL_QUERY_PARAMETER { Name = "login_audit_login_status", Value = loginAudit.login_audit_login_status },
                new SQL_QUERY_PARAMETER { Name = "login_audit_location", Value = loginAudit.login_audit_location }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}