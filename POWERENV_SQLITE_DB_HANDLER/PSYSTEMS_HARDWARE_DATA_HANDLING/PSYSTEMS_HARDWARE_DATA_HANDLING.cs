using Npgsql;
using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        #region VARIABLE_DEFINITION

        string connectionString;

        public record AccessPolicyInfo
        {
            public int access_policy_id { get; set; }
            public string access_policy_name { get; set; }
            public string access_policy_pgrid_name { get; set; }
            public string access_policy_target_username { get; set; }
            public string access_policy_creation_datetime { get; set; }
            public string access_policy_last_update_datetime { get; set; }
            public string access_policy_permission_level { get; set; }
        };

        public record AccessAuditInfo
        {
            public int access_audit_id { get; set; }
            public string access_audit_datetime { get; set; }
            public string access_audit_performed_by_username { get; set; }
            public string access_audit_target_pgrid_name { get; set; }
        };

        public record NodesLoginAudits
        {
            public int login_audit_id { get; set; }
            public string login_audit_fsp_user { get; set; }
            public string login_audit_datetime { get; set; }
            public string login_audit_login_status { get; set; }
            public string login_audit_location { get; set; }
            public string login_audit_pnode_nickname { get; set; }
            public string login_audit_pnode_ppool_name { get; set; }
        }

        public record FSPErrorLogFRUInfo
        {
            public string Priority { get; set; }
            public string LocationCode { get; set; }
            public string PartNumber { get; set; }
            public string CCIN { get; set; }
            public string SerialNumber { get; set; }
        };

        public record FSPErrorLogInfo
        {
            public string? ErrorLogID { get; set; }
            public string? LogDate { get; set; }
            public string? LogTime { get; set; }
            public string? DriverName { get; set; }
            public string? Subsystem { get; set; }
            public string? EventSeverity { get; set; }
            public List<string>? ActionFlags { get; set; }
            public string? ActionStatus { get; set; }
            public string? ReferenceCode { get; set; } //Primary System Reference Code
            public List<FSPErrorLogFRUInfo>? NormalHardwareFRU { get; set; } // Normal Hardware FRU
            public string? RawData { get; set; } // Raw data (for detailed report visualization)
            public string? PNodeNickname { get; set; }
            public string? PPoolName { get; set; }
        };

        public record AttentionLEDPNodesInfo
        {
            public string pnode_nickname { get; set; }
            public string ppool_name { get; set; }
        };

        public record PPoolsList
        {
            public int ppoolID { get; set; }
            public string ppool_name { get; set; }
            public int ppoolPnodesCount { get; set; }
            public List<PNodesBasicInfo> pnodesList { get; set; }
        };
        public record PNodesBasicInfo
        {
            public int pnodeID { get; set; }
            public string pnodeName { get; set; }
            public int pnodeLparsCount { get; set; }
        };

        public record PGridFullInfo
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
        };

        public record PGridBasicInfo
        (
            int pgrid_id,
            string pgrid_name,
            int pgrid_ppools_count,
            int pgrid_pnodes_count
        );

        public record PPoolFullInfo
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
        };

        public record PNodesSingleOperationHistory
        {
            public int? operationID { get; set; }
            public string? operationCatName { get; set; }
            public int? operationSourcePNodeID { get; set; }
            public string? operationSourcePNodeName { get; set; }
            public int? operationBatchOperationID { get; set; }
            public string? operationBatchOperationName { get; set; }
            public string? operationAction { get; set; }
            public string? operationCompletionStatus { get; set; }
            public string? operationDateTime { get; set; }
            public string? operationSourceUserName { get; set; }
        };

        public record PPoolsBatchOperationHistory
        {
            public int batchOperationID { get; set; }
            public string batchOperationCatName { get; set; }
            public int batchOperationSourcePPoolID { get; set; }
            public string batchOperationSourcePPoolName { get; set; }
            public string batchOperationAction { get; set; }
            public string batchOperationDateTime { get; set; }
            public string batchOperationSourceUserName { get; set; }
        };

        public record PPoolsOperationHistory
        {
            public List<PNodesSingleOperationHistory> pnodesSingleOperationHistory { get; set; }
            public List<PPoolsBatchOperationHistory> ppoolsBatchOperationHistory { get; set; }
        };

        public record PNodeMachineInfo
        {
            public string pnode_system_model_name { get; set; }
            public string pnode_machine_type_model { get; set; }
            public string pnode_machine_serial_number { get; set; }
            public string pnode_system_pseries { get; set; }
        };

        public record PNodeFSPInfo
        {
            public int pnode_fsp_id { get; set; }
            public string pnode_fsp_asmi_version { get; set; }
            public string pnode_fsp_asmi_username { get; set; }
            public string pnode_fsp_asmi_password_hash { get; set; }
            public string pnode_fsp_asmi_local_datetime { get; set; }
        };

        public record PNodeNICInfo
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
        };

        public record PNodeETHAccessPolicyInfo
        {
            public int access_policy_id { get; set; }
            public int access_policy_pnode_id { get; set; }
            public int access_policy_index_id { get; set; }
            public string access_policy_ip_address { get; set; }
            public string access_policy_type { get; set; }
        };

        public record PNodeFullInfo
        {
            public int pnode_id { get; set; }
            public string pnode_nickname { get; set; }
            public string pnode_parent_ppool_name { get; set; }
            public string pnode_config_datetime { get; set; }
            public string pnode_last_update_datetime { get; set; }
            public List<PNodeNICInfo> pnode_nics_info { get; set; }
            public string pnode_last_heartbeat_datetime { get; set; }
            public string pnode_attention_led_state { get; set; }
            public string pnode_readme_text { get; set; }
            public bool pnodeActivenessState { get; set; }
            public string pnodeSerialCOMPortId { get; set; }
        }

        public record LPARBasicInfo
        {
            public int lpar_id { get; set; }
            public string lpar_name { get; set; }
            public int lpar_os_instance { get; set; }
            public bool is_main_os_host { get; set; }
            public int lpar_storage_size { get; set; }
        };

        public record LPARFullInfo
        {
            public int lpar_id { get; set; }
            public string lpar_name { get; set; }
            public bool is_main_os_host { get; set; }
            public int lpar_storage_size { get; set; }
            public int lpar_target_pnode_id { get; set; }
            public int os_id { get; set; }
            public AuthInfo osAuthInfo { get; set; }
            public string os_ip_address { get; set; }
            public string os_family { get; set; }
        };

        public record AuthInfo
        (
            string username,
            string password
        );

        public record OSUserInfo
        (
            int os_id,
            AuthInfo osAuthInfo,
            string os_ip_address,
            string os_family,
            int os_lpar_id
        );

        public record OSConnSessionInfo
        {
            public int? session_id { get; set; }
            public string? WSSListenerConnectionID { get; set; }
            public LPARFullInfo? sessionTargetLPARInfo { get; set; }
            public string? pendingCommand { get; set; }
        };

        #endregion VARIABLE_DEFINITION

        public PSYSTEMS_HARDWARE_DATA_HANDLING(string dataSourceDirPath) {
            string DBPassword = Environment.GetEnvironmentVariable("POWERENV_DB_PASSWORD");
            string DBIPAddress = Environment.GetEnvironmentVariable("POWERENV_DB_IPADDRESS");
            string DBPort = Environment.GetEnvironmentVariable("POWERENV_DB_PORT");

            if (DBPassword != null)
            {
                connectionString = $"Host={DBIPAddress};Port={DBPort};Username=postgres;Password={DBPassword};Database=POWERENV-POWERDB";
            }
            else throw new Exception("FATAL ERROR: DATABASE KEYS NOT FOUND!");

            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapComposite<FSPErrorLogFRUInfo>("ERROR_LOG_NHFRU_RECORD_LIST_TYPE");
            NpgsqlDataSource dataSource = dataSourceBuilder.Build();
        }

        public List<PGridBasicInfo> DBGetPGrids()
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                "CALL SP_GET_PGRIDS_LIST('CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<PGridBasicInfo> pgridsBasicInfoList = new List<PGridBasicInfo>();

            while (connectionInfo.reader.Read())
            {
                PGridBasicInfo pgridBasicInfo = new PGridBasicInfo
                (
                    connectionInfo.reader.GetInt32(0),
                    connectionInfo.reader.GetString(1),
                    connectionInfo.reader.GetInt32(2),
                    connectionInfo.reader.GetInt32(3)
                );
                pgridsBasicInfoList.Add(pgridBasicInfo);
            }

            connectionInfo.conn.Close();

            return pgridsBasicInfoList;
        }
    }
}