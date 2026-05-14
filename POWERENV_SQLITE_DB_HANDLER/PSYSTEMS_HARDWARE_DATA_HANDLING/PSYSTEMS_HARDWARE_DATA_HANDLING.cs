using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        #region VARIABLE_DEFINITION

        string connectionString;

        public struct STRUCT_ACCESS_POLICY_INFO
        {
            public int access_policy_id { get; set; }
            public string access_policy_name { get; set; }
            public string access_policy_pgrid_name { get; set; }
            public string access_policy_target_username { get; set; }
            public string access_policy_creation_datetime { get; set; }
            public string access_policy_last_update_datetime { get; set; }
            public string access_policy_permission_level { get; set; }
        }

        public struct STRUCT_ACCESS_AUDIT_INFO
        {
            public int access_audit_id { get; set; }
            public string access_audit_datetime { get; set; }
            public string access_audit_performed_by_username { get; set; }
            public string access_audit_target_pgrid_name { get; set; }
        }

        public struct STRUCT_NODES_LOGIN_AUDITS
        {
            public int login_audit_id { get; set; }
            public string login_audit_fsp_user { get; set; }
            public string login_audit_datetime { get; set; }
            public string login_audit_login_status { get; set; }
            public string login_audit_location { get; set; }
            public string login_audit_pnode_nickname { get; set; }
            public string login_audit_pnode_ppool_name { get; set; }
        }

        public struct STRUCT_FSP_ERROR_LOG_FRU_INFO
        {
            public string Priority { get; set; }
            public string LocationCode { get; set; }
            public string PartNumber { get; set; }
            public string CCIN { get; set; }
            public string SerialNumber { get; set; }
        }

        public struct STRUCT_FSP_ERROR_LOG_INFO
        {
            // BASIC INFO
            public string ErrorLogID { get; set; }
            public string LogDate { get; set; }
            public string LogTime { get; set; }
            public string DriverName { get; set; }
            public string Subsystem { get; set; }
            public string EventSeverity { get; set; }
            public List<string> ActionFlags { get; set; }
            public string ActionStatus { get; set; }
            public string ReferenceCode { get; set; } //Primary System Reference Code
            public List<STRUCT_FSP_ERROR_LOG_FRU_INFO> NormalHardwareFRU { get; set; } // Normal Hardware FRU
            public string RawData { get; set; } // Raw data (for detailed report visualization)
            public string PNodeNickname { get; set; }
            public string PPoolName { get; set; }
        }

        public struct STRUCT_ATTENTION_LED_PNODES_INFO
        {
            public string pnode_nickname { get; set; }
            public string ppool_name { get; set; }
        }

        public struct STRUCT_PPOOLS_LIST
        {
            public int ppoolID { get; set; }
            public string ppool_name { get; set; }
            public int ppoolPnodesCount { get; set; }
            public List<STRUCT_PNODES_BASIC_INFO> pnodesList { get; set; }
        }

        public struct STRUCT_PNODES_BASIC_INFO
        {
            public int pnodeID { get; set; }
            public string pnodeName { get; set; }
            public int pnodeLparsCount { get; set; }
        }

        public struct STRUCT_PGRID_FULL_INFO
        {
            public string pgrid_id { get; set; }
            public string pgrid_name { get; set; }
            public string pgrid_creation_datetime { get; set; }
            public string pgrid_last_update_datetime { get; set; }
            public string pgrid_owner { get; set; }
            public string pgrid_readme_text { get; set; }
            public int pgrid_ppools_count { get; set; }
            public int pgrid_pnodes_count { get; set; }
            public int pgrid_active_pnodes_count { get; set; }
        }

        public struct STRUCT_PGRID_BASIC_INFO
        {
            public int pgrid_id { get; set; }
            public string pgrid_name { get; set; }
            public int pgrid_ppools_count { get; set; }
            public int pgrid_pnodes_count { get; set; }
        }

        public struct STRUCT_PPOOL_FULL_INFO
        {
            public int ppool_id { get; set; }
            public string ppool_name { get; set; }
            public string ppool_tag { get; set; }
            public string ppool_parent_pgrid_name { get; set; }
            public string ppool_creation_datetime { get; set; }
            public string ppool_last_update_datetime { get; set; }
            public string ppool_readme_text { get; set; }
            public int ppool_pnodes_count { get; set; }
            public int ppool_active_pnodes_count { get; set; }
        }

        public struct STRUCT_PNODES_SINGLE_OPERATION_HISTORY
        {
            public int operationID {  get; set; }
            public string operationCatName { get; set; }
            public int operationSourcePNodeID { get; set; }
            public string operationSourcePNodeName { get; set; }
            public int operationBatchOperationID { get; set; }
            public string operationBatchOperationName { get; set; }
            public string operationAction { get; set; }
            public string operationCompletionStatus { get; set; }
            public string operationDateTime { get; set; }
            public string operationSourceUserName { get; set; }
        }

        public struct STRUCT_PPOOLS_BATCH_OPERATION_HISTORY
        {
            public int batchOperationID { get; set; }
            public string batchOperationCatName { get; set; }
            public int batchOperationSourcePPoolID { get; set; }
            public string batchOperationSourcePPoolName { get; set; }
            public string batchOperationAction { get; set; }
            public string batchOperationDateTime { get; set; }
            public string batchOperationSourceUserName { get; set; }
        }

        public struct STRUCT_PPOOLS_OPERATION_LOGS
        {
            public List<STRUCT_PNODES_SINGLE_OPERATION_HISTORY> pnodesSingleOperationHistory { get; set; }
            public List<STRUCT_PPOOLS_BATCH_OPERATION_HISTORY> ppoolsBatchOperationHistory { get; set; }
        }

        public struct STRUCT_PNODE_MACHINE_INFO
        {
            public string pnode_system_model_name { get; set; }
            public string pnode_machine_type_model { get; set; }
            public string pnode_machine_serial_number { get; set; }
            public string pnode_system_pseries { get; set; }
        }

        public struct STRUCT_PNODE_FSP_INFO
        {
            public int pnode_fsp_id { get; set; }
            public string pnode_fsp_asmi_version { get; set; }
            public string pnode_fsp_asmi_username { get; set; }
            public string pnode_fsp_asmi_password_hash { get; set; }
            public string pnode_fsp_asmi_local_datetime { get; set; }
        }

        public struct STRUCT_PNODE_NIC_INFO
        {
            public int pnode_nic_id { get; set; }
            public string pnode_nic_name { get; set; }
            public string pnode_nic_mac_address { get; set; }
            public string pnode_nic_ip_address { get; set; }
            public string pnode_nic_ip_address_type { get; set; }
            public string pnode_nic_subnet_mask { get; set; }
            public string pnode_nic_default_gateway { get; set; }
            public string pnode_nic_hostname { get; set; }
            public string pnode_nic_domain_name { get; set; }
            public string pnode_nic_first_dns_ip_address { get; set; }
            public string pnode_nic_second_dns_ip_address { get; set; }
            public string pnode_nic_third_dns_ip_address { get; set; }
            public string pnode_nic_type { get; set; }
            public int pnode_id { get; set; }
        }

        public struct STRUCT_PNODE_ETH_ACCESS_POLICY_INFO
        {
            public int access_policy_id { get; set; }
            public int access_policy_pnode_id { get; set; }
            public int access_policy_index_id { get; set; }
            public string access_policy_ip_address { get; set; }
            public string access_policy_type { get; set; }
        }

        public struct STRUCT_PNODE_FULL_INFO
        {
            public int pnode_id { get; set; }
            public string pnode_nickname { get; set; }
            public string pnode_parent_ppool_name { get; set; }
            public string pnode_config_datetime { get; set; }
            public string pnode_last_update_datetime { get; set; }
            public List<STRUCT_PNODE_NIC_INFO> pnode_nics_info { get; set; }
            public string pnode_last_heartbeat_datetime { get; set; }
            public string pnode_attention_led_state { get; set; }
            public string pnode_readme_text { get; set; }
            public bool pnodeActivenessState { get; set; }
            public int pnodeSerialCOMPortId { get; set; }
        }

        public struct STRUCT_LPAR_BASIC_INFO
        {
            public int lpar_id { get; set; }
            public string lpar_name { get; set; }
            public int lpar_os_instance { get; set; }
            public bool is_main_os_host { get; set; }
            public int lpar_storage_size { get; set; }
        }

        public struct STRUCT_LPAR_FULL_INFO
        {
            public int lpar_id { get; set; }
            public string lpar_name { get; set; }
            public bool is_main_os_host { get; set; }
            public int lpar_storage_size { get; set; }
            public int lpar_target_pnode_id { get; set; }
            public int os_id { get; set; }
            public STRUCT_AUTH_INFO osAuthInfo { get; set; }
            public string os_ip_address { get; set; }
            public string os_family { get; set; }
        }

        public struct STRUCT_AUTH_INFO
        {
            public string username;
            public string password;
        }

        public struct STRUCT_OS_USER_INFO
        {
            public int os_id { get; set; }
            public STRUCT_AUTH_INFO osAuthInfo { get; set; }
            public string os_ip_address { get; set; }
            public string os_family { get; set; }
            public int os_lpar_id { get; set; }
        }

        public struct STRUCT_OSCONN_SESSION_INFO
        {
            public int session_id { get; set; }
            public string WSSListenerConnectionID { get; set; }
            public STRUCT_LPAR_FULL_INFO sessionTargetLPARInfo { get; set; }
            public string pendingCommand { get; set; }
        }

        #endregion VARIABLE_DEFINITION

        public PSYSTEMS_HARDWARE_DATA_HANDLING(string dataSourceDirPath) {
            string DBPassword = Environment.GetEnvironmentVariable("POWERENV_DB_PASSWORD");
            string DBIPAddress = Environment.GetEnvironmentVariable("POWERENV_DB_IPADDRESS");
            string DBPort = Environment.GetEnvironmentVariable("POWERENV_DB_PORT");

            if (DBPassword != null)
            {
                //connectionString = $"Data Source={dataSourceDirPath}POWERDB.db";
                connectionString = $"Host={DBIPAddress};Port={DBPort};Username=postgres;Password={DBPassword};Database=POWERENV-POWERDB";
            }
            else throw new Exception("FATAL ERROR: DATABASE KEYS NOT FOUND!");
        }

        public List<STRUCT_PGRID_BASIC_INFO> DBGetPGrids()
        {
            string sqlCommandText = "WITH " +
                "PPOOL_COUNT_CTE AS ( " +
                "SELECT " +
                "PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID, " +
                "COUNT(PPOOL_ID) AS PPOOL_COUNT " +
                "FROM PPOOLS " +
                "GROUP BY PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID" +
                "), " +
                "PNODE_COUNT_CTE AS " +
                "(" +
                "SELECT " +
                "PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID, " +
                "COUNT(PNODES.PNODE_ID) AS PNODE_COUNT " +
                "FROM PNODES " +
                "RIGHT JOIN PPOOLS ON PPOOLS.PPOOL_ID = PNODES.PNODE_ASSOCIATED_PPOOL_ID " +
                "GROUP BY PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID" +
                ") " +
                "SELECT " +
                "PGRIDS.PGRID_ID, " +
                "PGRIDS.PGRID_NAME, " +
                "COALESCE(PPOOL_COUNT_CTE.PPOOL_COUNT, 0), " +
                "COALESCE(PNODE_COUNT_CTE.PNODE_COUNT, 0) " +
                "FROM PGRIDS " +
                "INNER JOIN PPOOL_COUNT_CTE ON PPOOL_COUNT_CTE.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                "INNER JOIN PNODE_COUNT_CTE ON PNODE_COUNT_CTE.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                "ORDER BY pgrids.PGRID_ID;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            List<STRUCT_PGRID_BASIC_INFO> pgridsBasicInfoList = new List<STRUCT_PGRID_BASIC_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_PGRID_BASIC_INFO pgridBasicInfo = new STRUCT_PGRID_BASIC_INFO();
                pgridBasicInfo = new STRUCT_PGRID_BASIC_INFO()
                {
                    pgrid_id = connectionInfo.reader.GetInt32(0),
                    pgrid_name = connectionInfo.reader.GetString(1),
                    pgrid_ppools_count = connectionInfo.reader.GetInt32(2),
                    pgrid_pnodes_count = connectionInfo.reader.GetInt32(3),
                };

                pgridsBasicInfoList.Add(pgridBasicInfo);
            }

            connectionInfo.conn.Close();

            return pgridsBasicInfoList;
        }
    }
}
