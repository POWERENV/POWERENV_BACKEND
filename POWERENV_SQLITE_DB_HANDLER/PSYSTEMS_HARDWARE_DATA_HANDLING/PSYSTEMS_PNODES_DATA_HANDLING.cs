using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        //============================PNODE DATA HANDLING METHODS============================//

        #region READ
        public STRUCT_PNODE_FULL_INFO DBGetPNodeFullInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_FULL_INFO({_targetPNodeID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            STRUCT_PNODE_FULL_INFO pnodeFullInfo = new STRUCT_PNODE_FULL_INFO();

            while (connectionInfo.reader.Read())
            {
                pnodeFullInfo = new STRUCT_PNODE_FULL_INFO()
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
                    pnodeSerialCOMPortId = connectionInfo.reader.GetInt32(13)
                };
            }

            connectionInfo.conn.Close();

            return pnodeFullInfo;
        }

        public STRUCT_PNODE_FSP_INFO DBGetPNodeFSPInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_FSP_INFO({_targetPNodeID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            STRUCT_PNODE_FSP_INFO pnodeFSPInfo = new STRUCT_PNODE_FSP_INFO();

            while (connectionInfo.reader.Read())
            {
                string passphrase = "";

                for (int i = 0; i < connectionInfo.reader.GetString(3).Length; i++)
                {
                    passphrase += "*";
                }

                pnodeFSPInfo = new STRUCT_PNODE_FSP_INFO()
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

        public STRUCT_PNODE_MACHINE_INFO DBGetPNodeMachineInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_MACHINE_INFO({_targetPNodeID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            STRUCT_PNODE_MACHINE_INFO pnodeMachineInfo = new STRUCT_PNODE_MACHINE_INFO();

            while (connectionInfo.reader.Read())
            {
                pnodeMachineInfo = new STRUCT_PNODE_MACHINE_INFO()
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

        public List<STRUCT_PNODE_NIC_INFO> DBGetPNodeNICsInfo(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_NICS_INFO({_targetPNodeID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<STRUCT_PNODE_NIC_INFO> pnodeNICsInfo = new List<STRUCT_PNODE_NIC_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_PNODE_NIC_INFO pnodeIndividualNICInfo = new STRUCT_PNODE_NIC_INFO()
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

        public List<STRUCT_PNODE_ETH_ACCESS_POLICY_INFO> DBGetPNodeETHAccessPolicies(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_ETH_ACCESS_POLICIES({_targetPNodeID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<STRUCT_PNODE_ETH_ACCESS_POLICY_INFO> pnodeETHAccessPoliciesInfo = new List<STRUCT_PNODE_ETH_ACCESS_POLICY_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_PNODE_ETH_ACCESS_POLICY_INFO pnodeIndividualNICInfo = new STRUCT_PNODE_ETH_ACCESS_POLICY_INFO()
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

        public List<STRUCT_NODES_LOGIN_AUDITS> DBGetPNodesLoginAudits(int _targetPNode)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODES_LOGIN_AUDITS({_targetPNode}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);
            List<STRUCT_NODES_LOGIN_AUDITS> pnodesLoginAudits = new List<STRUCT_NODES_LOGIN_AUDITS>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_NODES_LOGIN_AUDITS pnodeLoginAudit = new STRUCT_NODES_LOGIN_AUDITS()
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

        public List<STRUCT_PNODES_SINGLE_OPERATION_HISTORY> DBGetPNodeOperationLogs(int _targetPNodeID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_OPERATION_LOGS({_targetPNodeID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);
            List<STRUCT_PNODES_SINGLE_OPERATION_HISTORY> ppoolPNodesSingleOperationHistory = new List<STRUCT_PNODES_SINGLE_OPERATION_HISTORY>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_PNODES_SINGLE_OPERATION_HISTORY ppoolPNodesSingleOperationLog = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
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

        public List<STRUCT_FSP_ERROR_LOG_INFO> DBGetPNodesErrorLogs(int _targetPNode)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODES_ERROR_LOGS({_targetPNode}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<STRUCT_FSP_ERROR_LOG_INFO> pnodeErrorLogs = new List<STRUCT_FSP_ERROR_LOG_INFO>();

            while (connectionInfo.reader.Read())
            {
                List<string> actionFlags = new List<string>();
                string[] a = connectionInfo.reader.GetString(6).Split(", ");

                for (int i = 0; i < a.Length; i++)
                {
                    actionFlags.Add(a[i]);
                }

                string[] logDateNTime = connectionInfo.reader.GetDateTime(1).ToString().Split(" ");

                STRUCT_FSP_ERROR_LOG_INFO pnodeErrorLog = new STRUCT_FSP_ERROR_LOG_INFO()
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

        public List<STRUCT_LPAR_BASIC_INFO> DBGetPNodeLPARS(int PNode_ID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_LPARS({PNode_ID}, 'CURSOR');" +
                $"FETCH ALL FROM \"CURSOR\";" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<STRUCT_LPAR_BASIC_INFO> lparsInfo = new List<STRUCT_LPAR_BASIC_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_LPAR_BASIC_INFO lparInfo = new STRUCT_LPAR_BASIC_INFO()
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

        public STRUCT_LPAR_FULL_INFO DBGetPNodeMainOSLPARInfo(int PNode_ID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_GET_PNODE_MAIN_OS_LPAR_INFO({PNode_ID}, 'CURSOR');" +
                $"FETCH ALL FROM \"CURSOR\";" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            connectionInfo.reader.Read();

            STRUCT_AUTH_INFO osAuthInfo = new STRUCT_AUTH_INFO()
            {
                username = connectionInfo.reader.GetString(1),
                password = connectionInfo.reader.GetString(2)
            };

            STRUCT_LPAR_FULL_INFO osInfo = new STRUCT_LPAR_FULL_INFO() {
                os_id = connectionInfo.reader.GetInt32(0),
                osAuthInfo = osAuthInfo,
                os_ip_address = connectionInfo.reader.GetString(3),
                os_family = connectionInfo.reader.GetString(4),
                is_main_os_host = true,
                lpar_id = connectionInfo.reader.GetInt32(5),
                lpar_name = connectionInfo.reader.GetString(6),
                lpar_storage_size = connectionInfo.reader.GetInt32(7),
                lpar_target_pnode_id = PNode_ID
            };

            connectionInfo.conn.Close();

            return osInfo;
        }

        #endregion READ
        #region WRITE

        public int updatePNodeActivenessState(int pnodeID, int newActivenessStateID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODE_ACTIVENESS_STATE({pnodeID}, {newActivenessStateID}, NULL);" +
                $"COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int updatePNodeAttentionLEDState(int pnodeID, string newLEDState)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODES_ATTENTIONLED_STATE({pnodeID}, '{newLEDState}', NULL);" +
                $"COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int updatePNodeNICsInfo(STRUCT_PNODE_NIC_INFO _newNICInfo)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODE_NICS_INFO({ _newNICInfo.pnode_id }," +
                $"'{ _newNICInfo.pnode_nic_mac_address }'," +
                $"'{ _newNICInfo.pnode_nic_ip_address }'," +
                $"'{ _newNICInfo.pnode_nic_ip_address_type }'," +
                $"'{ _newNICInfo.pnode_nic_subnet_mask }'," +
                $"'{ _newNICInfo.pnode_nic_default_gateway }'," +
                $"'{ _newNICInfo.pnode_nic_hostname }'," +
                $"'{ _newNICInfo.pnode_nic_domain_name }'," +
                $"'{ _newNICInfo.pnode_nic_first_dns_ip_address }'," +
                $"'{ _newNICInfo.pnode_nic_second_dns_ip_address }'," +
                $"'{ _newNICInfo.pnode_nic_third_dns_ip_address }'," +
                $"'{ _newNICInfo.pnode_nic_type }'," +
                $"{ _newNICInfo.pnode_id }," +
                $"NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int insertPNodeETHAccessPolicy(STRUCT_PNODE_ETH_ACCESS_POLICY_INFO newETHAccessPolicy)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_ETH_ACCESS_POLICY({newETHAccessPolicy.access_policy_pnode_id}," +
                $"{newETHAccessPolicy.access_policy_index_id}," +
                $"'{newETHAccessPolicy.access_policy_ip_address}'," +
                $"{int.Parse(newETHAccessPolicy.access_policy_type)}," +
                $"NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int updatePNodeETHAccessPolicies(STRUCT_PNODE_ETH_ACCESS_POLICY_INFO _updatedPolicy)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_UPDATE_PNODE_ETH_ACCESS_POLICIES({ _updatedPolicy.access_policy_index_id }," +
                $"'{ _updatedPolicy.access_policy_ip_address }'," +
                $"{ int.Parse(_updatedPolicy.access_policy_type) }," +
                $"{ _updatedPolicy.access_policy_id }," +
                $"NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int deletePNodeETHAccessPolicy(STRUCT_PNODE_ETH_ACCESS_POLICY_INFO ETHAccessPolicy)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_DELETE_PNODE_ETH_ACCESS_POLICY({ETHAccessPolicy.access_policy_index_id}," +
                $"{ETHAccessPolicy.access_policy_type}," +
                $"{ETHAccessPolicy.access_policy_pnode_id}," +
                $"NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodeSingleOperation(STRUCT_PNODES_SINGLE_OPERATION_HISTORY OperationData)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_SINGLE_OPERATION('{OperationData.operationCatName}'," +
                $"'{OperationData.operationCompletionStatus}'," +
                $"'{OperationData.operationSourceUserName}'," +
                $"{OperationData.operationSourcePNodeID}," +
                $"'{OperationData.operationAction}'," +
                $"NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, true);
            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditReadme(int pnodeID, string newReadmeText)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_PNODE_EDIT_README({pnodeID}, '{newReadmeText}', NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditDateTime(int _pnodeID, string _date, string _time)
        {
            string tempDATE = null;
            string tempTIME = _time;

            if (_date != null) tempDATE = $"{_date.Split("-")[2]}-{_date.Split("-")[0]}-{_date.Split("-")[1]}";

            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_PNODE_EDIT_DATETIME({_pnodeID}, '{tempDATE}', '{tempTIME}', NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodeErrorLog(int _PNodeID, PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_INFO _currErrorLog)
        {
            string actionFlags = "";

            for(int i = 0; i < _currErrorLog.ActionFlags.Count - 1; i++)
            {
                actionFlags += $"{_currErrorLog.ActionFlags[i]}, ";
            }

            actionFlags += $"{_currErrorLog.ActionFlags[_currErrorLog.ActionFlags.Count - 1]}";

            _currErrorLog.RawData = _currErrorLog.RawData.Replace("'", "''");

            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_ERROR_LOG(" +
                $"'{_currErrorLog.ErrorLogID}'," +
                $"'{_currErrorLog.LogDate}'," +
                $"'{_currErrorLog.LogTime}'," +
                $"'{_currErrorLog.DriverName}'," +
                $"'{_currErrorLog.Subsystem}'," +
                $"'{_currErrorLog.RawData}'," +
                $"'{_currErrorLog.EventSeverity}'," +
                $"'{_currErrorLog.ActionFlags}'," +
                $"'{_currErrorLog.ActionStatus}'," +
                $"'{_currErrorLog.ReferenceCode}'," +
                $"{_PNodeID}," +
                $"{_currErrorLog.NormalHardwareFRU}" +
                $"NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);

            return connectionInfo.rowsAffected;
        }

        private int DBInsertPNodeErrorLogNHFRURecord(PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_FRU_INFO NHFRURecord, int errorLogDBID)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODE_ERROR_LOG_NHFRU_RECORD({NHFRURecord}, {errorLogDBID}, NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodesLoginAudits(int _targetPNode, PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS loginAudit)
        {
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_INSERT_PNODES_LOGIN_AUDITS(" +
                $"{_targetPNode}," +
                $"'{loginAudit.login_audit_fsp_user}'," +
                $"'{loginAudit.login_audit_datetime}'," +
                $"'{loginAudit.login_audit_login_status}'," +
                $"'{loginAudit.login_audit_location}'," +
                $"NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}