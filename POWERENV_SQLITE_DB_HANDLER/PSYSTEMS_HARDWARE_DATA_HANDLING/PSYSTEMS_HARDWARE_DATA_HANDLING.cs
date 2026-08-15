using Npgsql;
using NpgsqlTypes;

namespace POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        #region VARIABLE_DEFINITION

        private string connectionString;
        private POWERDB_PGSQL_DATA_HANDLING PARENT_DB_HANDLER;

        public record AccessPolicyInfo
        {
            public required int access_policy_id { get; set; }
            public required string access_policy_name { get; set; }
            public required string access_policy_pgrid_name { get; set; }
            public required string access_policy_target_username { get; set; }
            public required string access_policy_creation_datetime { get; set; }
            public required string access_policy_last_update_datetime { get; set; }
            public required string access_policy_permission_level { get; set; }
        };

        public record AccessAuditInfo
        {
            public required int access_audit_id { get; set; }
            public required string access_audit_datetime { get; set; }
            public required string access_audit_performed_by_username { get; set; }
            public required string access_audit_target_pgrid_name { get; set; }
        };

        public record NodesLoginAudits
        {
            public int login_audit_id { get; set; }
            public string? login_audit_fsp_user { get; set; }
            public string? login_audit_datetime { get; set; }
            public string? login_audit_login_status { get; set; }
            public string? login_audit_location { get; set; }
            public string? login_audit_pnode_nickname { get; set; }
            public string? login_audit_pnode_ppool_name { get; set; }
        }

        public record FSPErrorLogFRUInfo
        {
            [PgName("priority")] public required string Priority { get; set; }
            [PgName("locationcode")] public required string LocationCode { get; set; }
            [PgName("partnumber")] public required string PartNumber { get; set; }
            [PgName("serialnumber")]  public required string SerialNumber { get; set; }
            [PgName("ccin")] public required string CCIN { get; set; }
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
            public required string pnode_nickname { get; set; }
            public required string ppool_name { get; set; }
        };

        public record PPoolsList
        {
            public required int ppoolID { get; set; }
            public required string ppool_name { get; set; }
            public required int ppoolPnodesCount { get; set; }
            public required List<PNodesBasicInfo> pnodesList { get; set; }
        };

        public record PNodesBasicInfo
        {
            public required int pnodeID { get; set; }
            public required string pnodeName { get; set; }
            public required int pnodeLparsCount { get; set; }
        };

        public record PNodesBasicInfoPGSQLCompositeType
        {
            [PgName("pnodeid")] public required int PNodeID { get; set; }
            [PgName("nickname")] public required string NickName { get; set; }
            [PgName("systemmodelname")] public required string SystemModelName { get; set; }
            [PgName("systemmachinetypemodel")] public required string SystemMachineTypeModel { get; set; }
            [PgName("systemmachineserialnumber")] public required string SystemMachineSerialNumber { get; set; }
            [PgName("systempseries")]  public required string SystemPSeries { get; set; }
            [PgName("parentppoolid")] public required int ParentPPoolID { get; set; }
            [PgName("readmetext")]  public required string ReadmeText { get; set; }
            [PgName("serialcomport")] public required string SerialCOMPort { get; set; }
        };

        public record PGridFullInfo
        {
            public string? pgrid_id { get; set; }
            public string? pgrid_name { get; set; }
            public string? pgrid_creation_datetime { get; set; }
            public string? pgrid_last_update_datetime { get; set; }
            public string? pgrid_owner { get; set; }
            public string? pgrid_readme_text { get; set; }
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
            public string? ppool_name { get; set; }
            public string? ppool_tag { get; set; }
            public int ppool_parent_pgrid_id { get; set; }
            public string? ppool_parent_pgrid_name { get; set; }
            public string? ppool_creation_datetime { get; set; }
            public string? ppool_last_update_datetime { get; set; }
            public string? ppool_readme_text { get; set; }
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
            public string? operationDescription { get; set; }
            public int? operationSeverityLevelID { get; set; }
            public string? operationCompletionStatus { get; set; }
            public string? operationDateTime { get; set; }
            public string? operationSourceUserName { get; set; }
        };

        public record PPoolsBatchOperationHistory
        {
            public int batchOperationID { get; set; }
            public string? batchOperationCatName { get; set; }
            public int batchOperationSourcePPoolID { get; set; }
            public string? batchOperationSourcePPoolName { get; set; }
            public string? batchOperationAction { get; set; }
            public string? batchOperationDateTime { get; set; }
            public string? batchOperationSourceUserName { get; set; }
        };

        public record PPoolsOperationHistory
        {
            public required List<PNodesSingleOperationHistory> pnodesSingleOperationHistory { get; set; }
            public required List<PPoolsBatchOperationHistory> ppoolsBatchOperationHistory { get; set; }
        };

        public record PNodeMachineInfo
        {
            public required string pnode_system_model_name { get; set; }
            public required string pnode_machine_type_model { get; set; }
            public required string pnode_machine_serial_number { get; set; }
            public required string pnode_system_pseries { get; set; }
        };

        public record PNodeFSPInfo
        {
            [PgName("fspid")] public int FSPID { get; set; }
            [PgName("fspasmiusername")] public required string FSPASMIUsername { get; set; }
            [PgName("fspasmipasswordhash")] public required string FSPASMIPasswordHash { get; set; }
            [PgName("fspasmiversion")] public required string FSPASMIVersion { get; set; }
            [PgName("fspasmilocaltime")] public required string FSPASMILocalTime { get; set; }
        };

        public record PNodeNICInfo
        {
            public int pnode_nic_id { get; set; }
            public string? pnode_nic_name { get; set; }
            public string? pnode_nic_mac_address { get; set; }
            public string? pnode_nic_ip_address { get; set; }
            public string? pnode_nic_ip_address_type { get; set; }
            public string? pnode_nic_subnet_mask { get; set; }
            public string? pnode_nic_default_gateway { get; set; }
            public string? pnode_nic_hostname { get; set; }
            public string? pnode_nic_domain_name { get; set; }
            public string? pnode_nic_first_dns_ip_address { get; set; }
            public string? pnode_nic_second_dns_ip_address { get; set; }
            public string? pnode_nic_third_dns_ip_address { get; set; }
            public string? pnode_nic_type { get; set; }
            public int pnode_id { get; set; }
        };

        public record PNodeETHAccessPolicyInfo
        {
            public int access_policy_id { get; set; }
            public int access_policy_pnode_id { get; set; }
            public int access_policy_index_id { get; set; }
            public required string access_policy_ip_address { get; set; }
            public required string access_policy_type { get; set; }
        };

        public record PNodeFullInfo
        {
            public int pnode_id { get; set; }
            public string? pnode_nickname { get; set; }
            public string? pnode_parent_ppool_name { get; set; }
            public string? pnode_config_datetime { get; set; }
            public string? pnode_last_update_datetime { get; set; }
            public List<PNodeNICInfo>? pnode_nics_info { get; set; }
            public string? pnode_last_heartbeat_datetime { get; set; }
            public string? pnode_attention_led_state { get; set; }
            public string? pnode_readme_text { get; set; }
            public bool pnodeActivenessState { get; set; }
            public string? pnodeSerialCOMPortId { get; set; }
        }

        public record LPARBasicInfo
        {
            public required int lpar_id { get; set; }
            public required string lpar_name { get; set; }
            public required int lpar_os_instance { get; set; }
            public required bool is_main_os_host { get; set; }
            public required int lpar_storage_size { get; set; }
        };

        public record LPARFullInfo
        {
            public int lpar_id { get; set; }
            public required string lpar_name { get; set; }
            public bool is_main_os_host { get; set; }
            public int lpar_storage_size { get; set; }
            public int lpar_target_pnode_id { get; set; }
            public int os_id { get; set; }
            public AuthInfo? osAuthInfo { get; set; }
            public string? os_ip_address { get; set; }
            public string? os_family { get; set; }
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

        public record OSUserInfoPGSQLCompositeType
        {
            [PgName("osid")] public required int OSID { get; set; }
            [PgName("osusername")] public required string OSUsername { get; set; }
            [PgName("ospasswordhash")] public required string OSPasswordHash { get; set; }
            [PgName("osipaddress")] public required string OSIPAddress { get; set; }
            [PgName("osfamily")] public required string OSFamily { get; set; }
        };

        public record OSConnSessionInfo
        {
            public int? session_id { get; set; }
            public string? WSSListenerConnectionID { get; set; }
            public LPARFullInfo? sessionTargetLPARInfo { get; set; }
            public string? pendingCommand { get; set; }
        };

        public record GlobalEvent
        {
            public int GlobalEventId { get; set; }
            public string? GlobalEventSeverityLevel { get; set; }
            public string? GlobalEventTitle { get; set; }
            public string? GlobalEventDescription { get; set; }
            public DateTime GlobalEventTriggeredAt { get; set; }
            public string? NotificationTargetUsername { get; set; }
            public DateTime NotificationAcknowledgementTimestamp { get; set; }
            public DateTime NotificationResolvedTimestamp { get; set; }
        }

        public record GlobalEventTypesDistribution
        {
            public int informationalEventsCount { get; set; }
            public int warningEventsCount { get; set; }
            public int highImpactEventsCount { get; set; }
            public int criticalEventsCount { get; set; }
        }

        public record GlobalEventCadenceRegistry
        {
            public DateTime hourlyIntervalTimestamp { get; set; }
            public int eventCadence { get; set; }
        }

        public enum enum_timeScaleUnit
        {
            day,
            hour
        }

        public record CreatePNodeDataBundle
        {
            public required PNodesBasicInfoPGSQLCompositeType pnodeBasicInfo { get; set; }
            public required PNodeFSPInfo pnodeFSPInfo { get; set; }
            public required OSUserInfoPGSQLCompositeType pnodeOSUserInfoType { get; set; }
        }

        #endregion VARIABLE_DEFINITION

        public PSYSTEMS_HARDWARE_DATA_HANDLING(string dataSourceDirPath, POWERDB_PGSQL_DATA_HANDLING _parentDBHandler)
        {
            PARENT_DB_HANDLER = _parentDBHandler;
            string? DBPassword = Environment.GetEnvironmentVariable("POWERENV_DB_PASSWORD");
            string? DBIPAddress = Environment.GetEnvironmentVariable("POWERENV_DB_IPADDRESS");
            string? DBPort = Environment.GetEnvironmentVariable("POWERENV_DB_PORT");

            if (DBPassword != null)
            {
                connectionString = $"Host={DBIPAddress};Port={DBPort};Username=postgres;Password={DBPassword};Database=POWERENV-POWERDB;Timezone=UTC;";
            }
            else throw new Exception("FATAL ERROR: DATABASE KEYS NOT FOUND!");

            NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.MapComposite<FSPErrorLogFRUInfo>("public.error_log_nhfru_record_list_type");
            dataSourceBuilder.MapComposite<PNodesBasicInfoPGSQLCompositeType>("public.pnode_basic_info_type");
            dataSourceBuilder.MapComposite<PNodeFSPInfo>("public.pnode_fsp_info_type");
            dataSourceBuilder.MapComposite<OSUserInfoPGSQLCompositeType>("public.pnode_os_user_info_type");
            NpgsqlDataSource dataSource = dataSourceBuilder.Build();

            PARENT_DB_HANDLER.ConnectionDataSource = dataSource;
        }

        public List<GlobalEvent> DBGetRecentActivity(int userID)
        {
            string sqlCommandText = "CALL SP_GET_USER_LATEST_EVENTS(@userID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER() {Name = "userID", Value = userID}
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<GlobalEvent> recentActivityList = new List<GlobalEvent>();

            for (int i = 0; i < connectionInfo.resultsDataTable!.Rows.Count; i++)
            {
                GlobalEvent recentEventInfo = new GlobalEvent
                {
                    GlobalEventId = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    GlobalEventSeverityLevel = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    GlobalEventTitle = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    GlobalEventDescription = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    GlobalEventTriggeredAt = (DateTime)(connectionInfo.resultsDataTable.Rows[i][4]),
                    NotificationTargetUsername = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    NotificationAcknowledgementTimestamp = (DateTime)(connectionInfo.resultsDataTable.Rows[i][6]),
                    NotificationResolvedTimestamp = (DateTime)(connectionInfo.resultsDataTable.Rows[i][7]),
                };

                recentActivityList.Add(recentEventInfo);
            }

            return recentActivityList;
        }

        public List<GlobalEvent> DBGetGlobalEventsActivity(int userID)
        {
            string sqlCommandText = "CALL SP_GET_USER_EVENTS(@userID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER() {Name = "userID", Value = userID}
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<GlobalEvent> globalEventsActivityList = new List<GlobalEvent>();

            for (int i = 0; i < connectionInfo.resultsDataTable!.Rows.Count; i++)
            {
                GlobalEvent globalEventInfo = new GlobalEvent
                {
                    GlobalEventId = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    GlobalEventSeverityLevel = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    GlobalEventTitle = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    GlobalEventDescription = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    GlobalEventTriggeredAt = (DateTime)(connectionInfo.resultsDataTable.Rows[i][4]),
                    NotificationTargetUsername = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    NotificationAcknowledgementTimestamp = (DateTime)(connectionInfo.resultsDataTable.Rows[i][6]),
                    NotificationResolvedTimestamp = (DateTime)(connectionInfo.resultsDataTable.Rows[i][7]),
                };

                globalEventsActivityList.Add(globalEventInfo);
            }

            return globalEventsActivityList;
        }

        public GlobalEventTypesDistribution DBGetGlobalEventTypesDistribution(int userID)
        {
            string sqlCommandText = "CALL SP_GET_USER_EVENT_TYPES_DISTRIBUTION(@userID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER() {Name = "userID", Value = userID}
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            GlobalEventTypesDistribution globalEventTypesDistribution = new GlobalEventTypesDistribution()
            {
                informationalEventsCount = Convert.ToInt32(connectionInfo.resultsDataTable!.Rows[0][0]),
                warningEventsCount = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[1][0]),
                highImpactEventsCount = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[2][0]),
                criticalEventsCount = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[3][0])
            };

            return globalEventTypesDistribution;
        }

        public List<GlobalEventCadenceRegistry> DBGetGlobalEventCadenceStats(int userID, DateTime startDate, enum_timeScaleUnit timeScaleUnit)
        {
            string sqlCommandText = "CALL SP_GET_USER_EVENT_LOGGING_CADENCE_STATS(@userID, @startDate, @timeScaleUnit, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER() {Name = "userID", Value = userID},
                new SQL_QUERY_PARAMETER() {Name = "startDate", Value = startDate.Date},
                new SQL_QUERY_PARAMETER() {Name = "timeScaleUnit", Value = timeScaleUnit == enum_timeScaleUnit.day ? "day" : (timeScaleUnit == enum_timeScaleUnit.hour ? "hour" : "")}
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<GlobalEventCadenceRegistry> globalEventCadenceStats = new List<GlobalEventCadenceRegistry>();

            for (int i = 0; i < connectionInfo.resultsDataTable!.Rows.Count; i++)
            {
                GlobalEventCadenceRegistry globalEventCadenceRecord = new GlobalEventCadenceRegistry
                {
                    hourlyIntervalTimestamp = Convert.ToDateTime(connectionInfo.resultsDataTable.Rows[i][0]).ToUniversalTime(),
                    eventCadence = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][1])
                };

                globalEventCadenceStats.Add(globalEventCadenceRecord);
            }

            return globalEventCadenceStats;
        }

        public List<PGridBasicInfo> DBGetPGrids(int userID)
        {
            string sqlCommandText = "CALL SP_GET_PGRIDS_LIST(@userID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER() {Name = "userID", Value = userID}
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PGridBasicInfo> pgridsBasicInfoList = new List<PGridBasicInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable!.Rows.Count; i++)
            {
                PGridBasicInfo pgridBasicInfo = new PGridBasicInfo
                (
                    Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][2]),
                    Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][3])
                );

                pgridsBasicInfoList.Add(pgridBasicInfo);
            }

            return pgridsBasicInfoList;
        }
    }
}