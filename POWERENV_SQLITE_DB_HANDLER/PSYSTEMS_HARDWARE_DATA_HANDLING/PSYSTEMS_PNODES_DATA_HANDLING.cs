using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        //============================PNODE DATA HANDLING METHODS============================//

        #region READ
        public STRUCT_PNODE_FULL_INFO DBGetPNodeFullInfo(int _targetPNodeID)
        {
            string sqlCommandText = "SELECT " +
                "PNODES.PNODE_ID, " +
                "PNODES.PNODE_NICKNAME, " +
                "PPOOLS.PPOOL_NAME, " +
                "PNODES.PNODE_CREATION_DATETIME, " +
                "PNODES.PNODE_LAST_UPDATE_DATETIME, " +
                "PNODES.PNODE_SYSTEM_MODEL_NAME, " +
                "PNODES.PNODE_MACHINE_TYPE_MODEL, " +
                "PNODES.PNODE_MACHINE_SERIAL_NUMBER, " +
                "PNODES.PNODE_SYSTEM_PSERIES, " +
                "PNODES.PNODE_LAST_HEARTBEAT_DATETIME, " +
                "PNODES.PANODE_ATTENTION_LED_STATE, " +
                "PNODES.PNODE_README_TEXT, " +
                "PNODE_STATUS.PNODE_STATUS_NAME," +
                "PNODES.PNODE_SERIAL_COM_PORT_ID " +
                "FROM PNODES " +
                "INNER JOIN PPOOLS ON PPOOLS.PPOOL_ID = PNODES.PNODE_ASSOCIATED_PPOOL_ID " +
                "INNER JOIN PNODE_STATUS ON PNODE_STATUS.PNODE_STATUS_ID = PNODES.PNODE_STATUS_ID " +
                "INNER JOIN PNODES_FSP_INFO ON PNODES_FSP_INFO.PNODE_FSP_ID = PNODES.PNODE_FSP_ID " +
                $"WHERE PNODES.PNODE_ID = {_targetPNodeID};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "SELECT " +
                "PNODES_FSP_INFO.PNODE_FSP_ID, " +
                "PNODES_FSP_INFO.PNODE_FSP_ASMI_VERSION, " +
                "PNODES_FSP_INFO.PNODE_FSP_ASMI_USERNAME, " +
                "PNODES_FSP_INFO.PNODE_FSP_ASMI_PASSWORD_HASH, " +
                "PNODES_FSP_INFO.PNODE_FSP_ASMI_LOCAL_DATETIME " +
                "FROM PNODES " +
                "INNER JOIN PNODES_FSP_INFO ON PNODES_FSP_INFO.PNODE_FSP_ID = PNODES.PNODE_FSP_ID " +
                $"WHERE PNODES.PNODE_ID = {_targetPNodeID};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "SELECT " +
                "PNODES.PNODE_SYSTEM_MODEL_NAME, " +
                "PNODES.PNODE_MACHINE_TYPE_MODEL, " +
                "PNODES.PNODE_MACHINE_SERIAL_NUMBER, " +
                "PNODES.PNODE_SYSTEM_PSERIES " +
                "FROM PNODES " +
                $"WHERE PNODES.PNODE_ID = {_targetPNodeID};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "SELECT " +
                "PNODES_NIC_INFO.PNODE_NIC_ID, " +
                "PNODES_NIC_INFO.PNODE_NIC_NAME, " +
                "PNODES_NIC_INFO.PNODE_NIC_MAC_ADDRESS, " +
                "PNODES_NIC_INFO.PNODE_NIC_IP_ADDRESS, " +
                "PNODES_NIC_INFO.PNODE_NIC_IP_ADDRESS_TYPE, " +
                "PNODES_NIC_INFO.PNODE_NIC_SUBNETMASK, " +
                "PNODES_NIC_INFO.PNODE_NIC_DEFAULT_GATEWAY, " +
                "PNODES_NIC_INFO.PNODE_NIC_HOSTNAME, " +
                "COALESCE(PNODES_NIC_INFO.PNODE_NIC_DOMAIN_NAME, 'NONE'), " +
                "COALESCE(PNODES_NIC_INFO.PNODE_NIC_FIRST_DNS_SERVER_IP_ADDRESS, 'NONE'), " +
                "COALESCE(PNODES_NIC_INFO.PNODE_NIC_SECOND_DNS_SERVER_IP_ADDRESS, 'NONE'), " +
                "COALESCE(PNODES_NIC_INFO.PNODE_NIC_THIRD_DNS_SERVER_IP_ADDRESS, 'NONE'), " +
                "PNODES_NIC_INFO.PNODE_NIC_TYPE " +
                "FROM PNODES_NIC_INFO " +
                $"WHERE PNODES_NIC_INFO.PNODE_NICE_TARGET_PNODE_ID = {_targetPNodeID};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "SELECT " +
                "PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_ID, " +
                "PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_IP_INDEX, " +
                "PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_IP_ADDRESS, " +
                "ACCESS_POLICY_TYPE.TYPE_NAME " +
                "FROM PNODES_ETH_ACCESS_POLICIES " +
                "INNER JOIN ACCESS_POLICY_TYPE ON ACCESS_POLICY_TYPE.TYPE_ID = PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_TYPE " +
                $"WHERE PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_PNODE_ID = {_targetPNodeID};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "SELECT " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_ID, " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_FSP_USER, " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_DATETIME, " +
                "PNODE_LOGIN_STATUS.PNODE_LOGIN_STATUS_NAME, " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_LOCATION, " +
                "PNODES.PNODE_NICKNAME " +
                "FROM NODES_LOGIN_AUDITS " +
                "INNER JOIN PNODE_LOGIN_STATUS ON NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_STATUS_ID = PNODE_LOGIN_STATUS.PNODE_LOGIN_STATUS_ID " +
                "INNER JOIN PNODES ON NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_TARGET_PNODE_ID = PNODES.PNODE_ID " +
                $"WHERE PNODES.PNODE_ID = {_targetPNode} " +
                $"ORDER BY NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_DATETIME DESC;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "SELECT " +
                "PNODE_OPERATIONS.PNODE_OPERATION_ID, " +
                "PNODE_OPERATION_CATEGORIES.OPERATION_CAT_NAME, " +
                "PNODES.PNODE_NICKNAME, " +
                "COALESCE(PNODE_OPERATIONS.OPERATION_BATCH_OPERATION_ID, -1), " +
                "COALESCE(PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_ACTION, '-1'), " +
                "PNODE_OPERATIONS.OPERATION_ACTION, " +
                "PNODE_LOGIN_STATUS.PNODE_LOGIN_STATUS_NAME, " +
                "PNODE_OPERATIONS.OPERATION_DATETIME, " +
                "USERS.USER_FIRST_NAME || ' ' || USERS.USER_LAST_NAME " +
                "FROM PNODE_OPERATIONS " +
                "INNER JOIN PNODE_OPERATION_CATEGORIES ON PNODE_OPERATION_CATEGORIES.OPERATION_CAT_ID = PNODE_OPERATIONS.OPERATION_CAT_ID " +
                "INNER JOIN PNODES ON PNODES.PNODE_ID = PNODE_OPERATIONS.OPERATION_SOURCE_PNODE_ID " +
                "LEFT JOIN PPOOLS_BATCH_OPERATIONS ON PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_ID = PNODE_OPERATIONS.OPERATION_BATCH_OPERATION_ID " +
                "INNER JOIN PNODE_LOGIN_STATUS ON PNODE_LOGIN_STATUS.PNODE_LOGIN_STATUS_ID = PNODE_OPERATIONS.OPERATION_COMPLETION_STATUS_ID " +
                "INNER JOIN USERS ON USERS.USER_ID = PNODE_OPERATIONS.OPERATION_SOURCE_USER_ID " +
                $"WHERE PNODES.PNODE_ID = {_targetPNodeID} " +
                $"ORDER BY PNODE_OPERATIONS.OPERATION_DATETIME DESC;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "SELECT " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_FSP_ID, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_DATETIME, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_DRIVER_NAME, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_SUBSYSTEM, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_RAW_DATA, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_EVENT_SEVERITY, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_ACTION_FLAGS, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_ACTION_STATUS, " +
                "PNODES_FSP_ERROR_LOGS.ERROR_LOG_REFERENCE_CODE, " +
                "PNODES.PNODE_NICKNAME " +
                "FROM PNODES_FSP_ERROR_LOGS " +
                "INNER JOIN PNODES ON PNODES_FSP_ERROR_LOGS.ERROR_LOG_SOURCE_PNODE_ID = PNODES.PNODE_ID " +
                $"WHERE PNODES.PNODE_ID = {_targetPNode};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = $"SELECT " +
                $"LPAR_ID, " +
                $"LPAR_NAME, " +
                $"LPAR_ASSOCIATED_OS_INSTANCE_ID, " +
                $"LPAR_IS_MAIN_PNODE_OS, " +
                $"LPAR_STORAGE_SIZE " +
                $"FROM LPARS " +
                $"WHERE LPAR_ASSOCIATED_PNODE_ID = {PNode_ID};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = $"SELECT " +
                $"PNODE_OS_ID, " +
                $"PNODE_OS_USERNAME, " +
                $"PNODE_OS_PASSWORD_HASH, " +
                $"PNODE_OS_IP_ADDRESS, " +
                $"PNODE_OS_FAMILY, " +
                $"LPARS.LPAR_ID, " +
                $"LPARS.LPAR_NAME, " +
                $"LPARS.LPAR_STORAGE_SIZE " +
                $"FROM PNODE_OS_USER_INFO " +
                $"INNER JOIN LPARS ON LPARS.LPAR_ASSOCIATED_OS_INSTANCE_ID = PNODE_OS_USER_INFO.PNODE_OS_ID " +
                $"WHERE LPARS.LPAR_ASSOCIATED_PNODE_ID = {PNode_ID} " +
                $"AND LPARS.LPAR_IS_MAIN_PNODE_OS = TRUE;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = $"UPDATE PNODES " +
                $"SET PNODE_STATUS_ID = {newActivenessStateID} " +
                $"WHERE PNODES.PNODE_ID = {pnodeID};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int updatePNodeAttentionLEDState(int pnodeID, string newLEDStateID)
        {
            string sqlCommandText = $"UPDATE PNODES " +
                $"SET PANODE_ATTENTION_LED_STATE = '{newLEDStateID}' " +
                $"WHERE PNODES.PNODE_ID = {pnodeID};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int updatePNodeNICsInfo(STRUCT_PNODE_NIC_INFO _newNICInfo)
        {
            string sqlCommandText = $"UPDATE PNODES_NIC_INFO " +
                $"SET PNODE_NIC_MAC_ADDRESS = '{_newNICInfo.pnode_nic_mac_address}', " +
                $"PNODE_NIC_IP_ADDRESS = '{_newNICInfo.pnode_nic_ip_address}', " +
                $"PNODE_NIC_IP_ADDRESS_TYPE = '{_newNICInfo.pnode_nic_ip_address_type}', " +
                $"PNODE_NIC_SUBNETMASK = '{_newNICInfo.pnode_nic_subnet_mask}', " +
                $"PNODE_NIC_DEFAULT_GATEWAY = '{_newNICInfo.pnode_nic_default_gateway}', " +
                $"PNODE_NIC_HOSTNAME = '{_newNICInfo.pnode_nic_hostname}', " +
                $"PNODE_NIC_DOMAIN_NAME = '{_newNICInfo.pnode_nic_domain_name}', " +
                $"PNODE_NIC_FIRST_DNS_SERVER_IP_ADDRESS = '{_newNICInfo.pnode_nic_first_dns_ip_address}', " +
                $"PNODE_NIC_SECOND_DNS_SERVER_IP_ADDRESS = '{_newNICInfo.pnode_nic_second_dns_ip_address}', " +
                $"PNODE_NIC_THIRD_DNS_SERVER_IP_ADDRESS = '{_newNICInfo.pnode_nic_third_dns_ip_address}', " +
                $"PNODE_NIC_TYPE = '{_newNICInfo.pnode_nic_type}', " +
                $"PNODE_NICE_TARGET_PNODE_ID = {_newNICInfo.pnode_id} " +
                $"WHERE PNODE_NIC_ID = {_newNICInfo.pnode_nic_id};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int insertPNodeETHAccessPolicy(STRUCT_PNODE_ETH_ACCESS_POLICY_INFO newETHAccessPolicy)
        {
            string sqlCommandText = $"INSERT INTO PNODES_ETH_ACCESS_POLICIES" +
                $"(" +
                $"ACCESS_POLICY_PNODE_ID," +
                $"ACCESS_POLICY_IP_INDEX," +
                $"ACCESS_POLICY_IP_ADDRESS," +
                $"ACCESS_POLICY_TYPE" +
                $") " +
                $"VALUES (" +
                $"{newETHAccessPolicy.access_policy_pnode_id}," +
                $"{newETHAccessPolicy.access_policy_index_id}," +
                $"'{newETHAccessPolicy.access_policy_ip_address}'," +
                $"{newETHAccessPolicy.access_policy_type}" +
                $");";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int updatePNodeETHAccessPolicies(STRUCT_PNODE_ETH_ACCESS_POLICY_INFO _updatedPolicy)
        {
            string sqlCommandText = $"UPDATE PNODES_ETH_ACCESS_POLICIES " +
                $"SET ACCESS_POLICY_IP_INDEX = {_updatedPolicy.access_policy_index_id}, " +
                $"ACCESS_POLICY_IP_ADDRESS = '{_updatedPolicy.access_policy_ip_address}', " +
                $"ACCESS_POLICY_TYPE = {int.Parse(_updatedPolicy.access_policy_type)} " +
                $"WHERE ACCESS_POLICY_ID = {_updatedPolicy.access_policy_id};";
            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int deletePNodeETHAccessPolicy(STRUCT_PNODE_ETH_ACCESS_POLICY_INFO ETHAccessPolicy)
        {
            string sqlCommandText = $"DELETE " +
                $"FROM PNODES_ETH_ACCESS_POLICIES " +
                $"WHERE PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_IP_INDEX = {ETHAccessPolicy.access_policy_index_id} " +
                $"AND PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_TYPE = {int.Parse(ETHAccessPolicy.access_policy_type)} " +
                $"AND PNODES_ETH_ACCESS_POLICIES.ACCESS_POLICY_PNODE_ID = {ETHAccessPolicy.access_policy_pnode_id};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodeSingleOperation(STRUCT_PNODES_SINGLE_OPERATION_HISTORY OperationData)
        {
            string sqlCommandText = $"WITH operationCatID_CTE AS " +
                $"(" +
                $"SELECT OPERATION_CAT_ID AS CAT_ID " +
                $"FROM PNODE_OPERATION_CATEGORIES " +
                $"WHERE OPERATION_CAT_NAME = '{OperationData.operationCatName}'" +
                $"), " +
                $"operationCompletionStatusID_CTE AS " +
                $"(" +
                $"SELECT PNODE_LOGIN_STATUS_ID AS STATUS_ID " +
                $"FROM PNODE_LOGIN_STATUS " +
                $"WHERE PNODE_LOGIN_STATUS_NAME = '{OperationData.operationCompletionStatus}'" +
                $"), " +
                $"userID_CTE AS " +
                $"(" +
                $"SELECT USER_ID AS USERID " +
                $"FROM USERS " +
                $"WHERE (USER_FIRST_NAME || ' ' || USER_LAST_NAME) = '{OperationData.operationSourceUserName}'" +
                $") " +
                $"INSERT INTO PNODE_OPERATIONS" +
                $"(" +
                $"OPERATION_CAT_ID, " +
                $"OPERATION_SOURCE_PNODE_ID, " +
                $"OPERATION_BATCH_OPERATION_ID, " +
                $"OPERATION_ACTION, " +
                $"OPERATION_COMPLETION_STATUS_ID, " +
                $"OPERATION_DATETIME, " +
                $"OPERATION_SOURCE_USER_ID" +
                $") " +
                $"VALUES (" +
                $"(SELECT operationCatID_CTE.CAT_ID FROM operationCatID_CTE), " +
                $"{OperationData.operationSourcePNodeID}, " +
                $"NULL, " +
                $"'{OperationData.operationAction}', " +
                $"(SELECT operationCompletionStatusID_CTE.STATUS_ID FROM operationCompletionStatusID_CTE), " +
                $"NOW(), " +
                $"(SELECT userID_CTE.USERID FROM userID_CTE)" +
                $")";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditReadme(int pnodeID, string newReadmeText)
        {
            string sqlCommandText = $"UPDATE PNODES " +
                $"SET PNODE_README_TEXT = '{newReadmeText}' " +
                $"WHERE PNODE_ID = {pnodeID};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBPNodeEditDateTime(int _pnodeID, string _date, string _time)
        {
            string tempDATE = null;
            string tempTIME = _time;

            if (_date != null)
            {
                tempDATE = $"{_date.Split("-")[2]}-{_date.Split("-")[0]}-{_date.Split("-")[1]}";
            }

            string date = tempDATE != null ? $"(SELECT '{tempDATE}')" : "(SELECT CURRENT_DATETIME_CTE.CURRENT_DATE FROM CURRENT_DATETIME_CTE)";
            string time = tempTIME != null ? $"(SELECT '{tempTIME}')" : "(SELECT CURRENT_DATETIME_CTE.CURRENT_TIME FROM CURRENT_DATETIME_CTE)";
            string pnode_fsp_id = "(SELECT CURRENT_DATETIME_CTE.PNODE_FSP_ID FROM CURRENT_DATETIME_CTE)";

            string sqlCommandText = $"WITH CURRENT_DATETIME_CTE AS (" +
                $"SELECT " +
                $"PNODES_FSP_INFO.PNODE_FSP_ASMI_LOCAL_DATETIME::DATE AS CURRENT_DATE, " +
                $"PNODES_FSP_INFO.PNODE_FSP_ASMI_LOCAL_DATETIME::TIME AS CURRENT_TIME, " +
                $"PNODES_FSP_INFO.PNODE_FSP_ASMI_LOCAL_DATETIME AS DATETIME, " +
                $"PNODES_FSP_INFO.PNODE_FSP_ID " +
                $"FROM PNODES_FSP_INFO " +
                $"INNER JOIN PNODES ON PNODES.PNODE_FSP_ID = PNODES_FSP_INFO.PNODE_FSP_ID " +
                $"WHERE PNODES.PNODE_ID = {_pnodeID}" +
                $") " +
                $"UPDATE PNODES_FSP_INFO " +
                $"SET PNODE_FSP_ASMI_LOCAL_DATETIME = ({date} || ' ' || {time})::TIMESTAMP " +
                $"WHERE PNODE_FSP_ID = {pnode_fsp_id};";

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

            string sqlCommandText = $"INSERT INTO PNODES_FSP_ERROR_LOGS (" +
                $"ERROR_LOG_FSP_ID, " +
                $"ERROR_LOG_DATETIME, " +
                $"ERROR_LOG_DRIVER_NAME, " +
                $"ERROR_LOG_SUBSYSTEM, " +
                $"ERROR_LOG_RAW_DATA, " +
                $"ERROR_LOG_EVENT_SEVERITY, " +
                $"ERROR_LOG_ACTION_FLAGS, " +
                $"ERROR_LOG_ACTION_STATUS, " +
                $"ERROR_LOG_REFERENCE_CODE, " +
                $"ERROR_LOG_SOURCE_PNODE_ID" +
                $") " +
                $"VALUES (" +
                $"'{_currErrorLog.ErrorLogID}', " +
                $"'{_currErrorLog.LogDate + " " + _currErrorLog.LogTime}'::TIMESTAMP, " +
                $"'{_currErrorLog.EventSeverity}', " +
                $"'{_currErrorLog.Subsystem}', " +
                $"'{_currErrorLog.RawData}', " +
                $"'{_currErrorLog.EventSeverity}', " +
                $"'{actionFlags}', " +
                $"'{_currErrorLog.ActionStatus}', " +
                $"'{_currErrorLog.ReferenceCode}', " +
                $"{_PNodeID}" +
                $");";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);

            //Get the last inserted error log ID (the current one)
            string lastInsertedErrorLogIDsqlCommandText = "SELECT PNODES_FSP_ERROR_LOGS.ERROR_LOG_ID FROM PNODES_FSP_ERROR_LOGS ORDER BY ERROR_LOG_ID DESC LIMIT 1;";
            PGSQL_DB_CONNECTION_INFO connectionInfo2 = readQueryFromDB(connectionString, lastInsertedErrorLogIDsqlCommandText);
            int latestErrorLogDBID = 0;
            connectionInfo2.reader.Read();
            latestErrorLogDBID = connectionInfo2.reader.GetInt32(0);
            connectionInfo2.conn.Close();

            //Insert Normal Hardware FRU Records on its own table, and connect them with the latest error log ID
            for (int i = 0; i < _currErrorLog.NormalHardwareFRU.Count; i++)
            {
                DBInsertPNodeErrorLogNHFRURecord(_currErrorLog.NormalHardwareFRU[i], latestErrorLogDBID);
            }

            return connectionInfo.rowsAffected;
        }

        private int DBInsertPNodeErrorLogNHFRURecord(PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_FSP_ERROR_LOG_FRU_INFO NHFRURecord, int errorLogDBID)
        {
            string sqlCommandText = $"INSERT INTO PNODES_FSP_ERROR_LOG_NORM_HARDWARE_FRU_RECORDS (" +
                $"ERROR_LOG_NHFRU_PRIORITY, " +
                $"ERROR_LOG_NHFRU_LOCATION_CODE, " +
                $"ERROR_LOG_NHFRU_PART_NUMBER, " +
                $"ERROR_LOG_NHFRU_SERIAL_NUMBER, " +
                $"ERROR_LOG_NHFRU_CCIN, " +
                $"ERROR_LOG_NHFRU_ERROR_LOG_ID" +
                $") " +
                $"VALUES (" +
                $"'{NHFRURecord.Priority}', " +
                $"'{NHFRURecord.LocationCode}', " +
                $"'{NHFRURecord.PartNumber}', " +
                $"'{NHFRURecord.SerialNumber}', " +
                $"'{NHFRURecord.CCIN}', " +
                $"{errorLogDBID}" +
                $");";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        public int DBInsertPNodesLoginAudits(int _targetPNode, PSYSTEMS_HARDWARE_DATA_HANDLING.STRUCT_NODES_LOGIN_AUDITS loginAudit)
        {
            string sqlCommandText = $"WITH LOGIN_AUDIT_STATUS_CTE AS (" +
                $"SELECT PNODE_LOGIN_STATUS_ID " +
                $"FROM PNODE_LOGIN_STATUS " +
                $"WHERE PNODE_LOGIN_STATUS_NAME = '{loginAudit.login_audit_login_status}'" +
                $") " +
                $"INSERT INTO NODES_LOGIN_AUDITS (" +
                $"PNODE_LOGIN_AUDIT_FSP_USER, " +
                $"PNODE_LOGIN_AUDIT_DATETIME, " +
                $"PNODE_LOGIN_AUDIT_STATUS_ID, " +
                $"PNODE_LOGIN_AUDIT_LOCATION, " +
                $"PNODE_LOGIN_AUDIT_TARGET_PNODE_ID" +
                $") " +
                $"VALUES (" +
                $"'{loginAudit.login_audit_fsp_user}', " +
                $"'{loginAudit.login_audit_datetime}', " +
                $"(SELECT PNODE_LOGIN_STATUS_ID FROM LOGIN_AUDIT_STATUS_CTE), " +
                $"'{loginAudit.login_audit_location}', " +
                $"{_targetPNode}" +
                $");";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}