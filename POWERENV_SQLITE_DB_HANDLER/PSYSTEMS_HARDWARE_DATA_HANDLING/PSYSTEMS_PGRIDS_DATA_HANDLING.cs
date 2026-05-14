using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        //============================PGRID DATA HANDLING METHODS============================//

        public STRUCT_PGRID_FULL_INFO DBGetPGridFullInfo(int _targetPgridID)
        {
            string sqlCommandText = "WITH PPOOL_COUNT_CTE AS ( " +
                "SELECT " +
                "PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID, " +
                "COUNT(PPOOL_ID) AS PPOOL_COUNT " +
                "FROM PPOOLS " +
                "GROUP BY PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID " +
                "), " +
                "PNODE_COUNT_CTE AS ( " +
                "SELECT " +
                "PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID, " +
                "COUNT(PNODES.PNODE_ID) AS PNODE_COUNT " +
                "FROM PNODES " +
                "RIGHT JOIN PPOOLS ON PPOOLS.PPOOL_ID = PNODES.PNODE_ASSOCIATED_PPOOL_ID " + // THE RIGHT JOIN IS USED TO FORCE THE QUERY TO COUNT THE NUMBER OF PNODES FOR ALL PPOOLS, EVEN THOSE WITH 0 PNODES (IF WE USED INNER JOIN, IT WOULD DISCARD THE PPOOLS WITHOUT ANY PNODE => THEY DO NOT MAKE PART OF THE INTERSECTION OF THE TABLES)
                "GROUP BY PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID " +
                "), " +
                "ACTIVE_PNODE_COUNT_CTE AS ( " +
                "SELECT " +
                "PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID, " +
                "COUNT(PNODES.PNODE_ID) AS ACTIVE_PNODE_COUNT " +
                "FROM PNODES " +
                "INNER JOIN PNODE_STATUS ON PNODE_STATUS.PNODE_STATUS_ID = PNODES.PNODE_STATUS_ID " +
                "RIGHT JOIN PPOOLS ON PPOOLS.PPOOL_ID = PNODES.PNODE_ASSOCIATED_PPOOL_ID " + // THE RIGHT JOIN IS USED TO FORCE THE QUERY TO COUNT THE NUMBER OF PNODES FOR ALL PPOOLS, EVEN THOSE WITH 0 PNODES (IF WE USED INNER JOIN, IT WOULD DISCARD THE PPOOLS WITHOUT ANY PNODE => THEY DO NOT MAKE PART OF THE INTERSECTION OF THE TABLES)
                "WHERE PNODE_STATUS.PNODE_STATUS_NAME = 'ACTIVE' " +
                "GROUP BY PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID " +
                ") " +
                "SELECT " +
                "PGRIDS.PGRID_ID, " +
                "PGRIDS.PGRID_NAME, " +
                "PGRIDS.PGRID_CREATION_DATETIME, " +
                "PGRIDS.PGRID_LAST_UPDATE_DATETIME, " +
                "(USERS.USER_FIRST_NAME || ' ' || USERS.USER_LAST_NAME) AS PGRID_OWNER_FULL_NAME, " +
                "PGRIDS.PGRID_README_TEXT, " +
                "COALESCE(PPOOL_COUNT_CTE.PPOOL_COUNT, 0), " +
                "COALESCE(PNODE_COUNT_CTE.PNODE_COUNT, 0), " +
                "COALESCE(ACTIVE_PNODE_COUNT_CTE.ACTIVE_PNODE_COUNT, 0) " +
                "FROM PGRIDS " +
                "INNER JOIN PPOOL_COUNT_CTE ON PPOOL_COUNT_CTE.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                "INNER JOIN PNODE_COUNT_CTE ON PNODE_COUNT_CTE.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                "LEFT JOIN ACTIVE_PNODE_COUNT_CTE ON ACTIVE_PNODE_COUNT_CTE.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                "INNER JOIN USERS ON USERS.USER_ID = PGRIDS.PGRID_OWNER_ID " +
                $"WHERE PGRIDS.PGRID_ID = {_targetPgridID}";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            STRUCT_PGRID_FULL_INFO pgridFullInfo = new STRUCT_PGRID_FULL_INFO();

            while (connectionInfo.reader.Read())
            {
                pgridFullInfo = new STRUCT_PGRID_FULL_INFO()
                {
                    pgrid_id = $"PG-{connectionInfo.reader.GetInt32(0)}",
                    pgrid_name = connectionInfo.reader.GetString(1),
                    pgrid_creation_datetime = connectionInfo.reader.GetDateTime(2).ToString(),
                    pgrid_last_update_datetime = connectionInfo.reader.GetDateTime(3).ToString(),
                    pgrid_owner = connectionInfo.reader.GetString(4),
                    pgrid_readme_text = connectionInfo.reader.GetString(5),
                    pgrid_ppools_count = connectionInfo.reader.GetInt32(6),
                    pgrid_pnodes_count = connectionInfo.reader.GetInt32(7),
                    pgrid_active_pnodes_count = connectionInfo.reader.GetInt32(8)
                };
            }

            connectionInfo.conn.Close();

            return pgridFullInfo;
        }

        public List<STRUCT_ACCESS_POLICY_INFO> DBGetPGAccessPolicies(int _targetPgridID)
        {
            string sqlCommandText = "SELECT " +
                "PGRID_ACCESS_POLICIES.ACCESS_POLICY_ID, " +
                "PGRID_ACCESS_POLICIES.ACCESS_POLICY_NAME, " +
                "PGRIDS.PGRID_NAME, " +
                "USERS.USER_FIRST_NAME, " +
                "USERS.USER_LAST_NAME, " +
                "PGRID_ACCESS_POLICIES.ACCESS_POLICY_CREATION_DATETIME, " +
                "PGRID_ACCESS_POLICIES.ACCESS_POLICY_LAST_UPDATE_DATETIME, " +
                "PERMISSION_LEVELS.PERMISSION_LEVEL_NAME " +
                "FROM PGRID_ACCESS_POLICIES " +
                "INNER JOIN PGRIDS ON PGRIDS.PGRID_ID = PGRID_ACCESS_POLICIES.ACCESS_POLICY_PGRID_ID " +
                "INNER JOIN USERS ON USERS.USER_ID = PGRID_ACCESS_POLICIES.ACCESS_POLICY_TARGET_USER_ID " +
                "INNER JOIN PERMISSION_LEVELS ON PERMISSION_LEVELS.PERMISSION_LEVEL_ID = PGRID_ACCESS_POLICIES.ACCESS_POLICY_PERMISSION_LEVEL_ID " +
                $"WHERE PGRID_ACCESS_POLICIES.ACCESS_POLICY_PGRID_ID = {_targetPgridID} " +
                "LIMIT 20;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            List<STRUCT_ACCESS_POLICY_INFO> accessPolicies = new List<STRUCT_ACCESS_POLICY_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_ACCESS_POLICY_INFO accessPolicy = new STRUCT_ACCESS_POLICY_INFO()
                {
                    access_policy_id = connectionInfo.reader.GetInt32(0),
                    access_policy_name = connectionInfo.reader.GetString(1),
                    access_policy_pgrid_name = connectionInfo.reader.GetString(2),
                    access_policy_target_username = $"{connectionInfo.reader.GetString(3)} {connectionInfo.reader.GetString(4)}",
                    access_policy_creation_datetime = connectionInfo.reader.GetDateTime(5).ToString(),
                    access_policy_last_update_datetime = connectionInfo.reader.GetDateTime(6).ToString(),
                    access_policy_permission_level = connectionInfo.reader.GetString(7)
                };

                accessPolicies.Add(accessPolicy);
            }

            connectionInfo.conn.Close();

            return accessPolicies;
        }

        public List<STRUCT_ACCESS_AUDIT_INFO> DBGetPGAccessAudits(int _targetPgridID)
        {
            string sqlCommandText = "SELECT " +
                "PGRID_ACCESS_AUDITS.PGRID_AUDIT_ID, " +
                "PGRID_ACCESS_AUDITS.PGRID_AUDIT_DATETIME, " +
                "USERS.USER_FIRST_NAME, " +
                "USERS.USER_LAST_NAME, " +
                "PGRIDS.PGRID_NAME " +
                "FROM PGRID_ACCESS_AUDITS " +
                "INNER JOIN USERS ON PGRID_ACCESS_AUDITS.PGRID_AUDIT_ACTION_USER_ID = USERS.USER_ID " +
                "INNER JOIN PGRIDS ON PGRID_ACCESS_AUDITS.PGRID_AUDIT_TARGET_PGRID_ID = PGRIDS.PGRID_ID " +
                $"WHERE PGRID_ACCESS_AUDITS.PGRID_AUDIT_TARGET_PGRID_ID = {_targetPgridID} " +
                $"LIMIT 10;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            List<STRUCT_ACCESS_AUDIT_INFO> pgridAccessAudits = new List<STRUCT_ACCESS_AUDIT_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_ACCESS_AUDIT_INFO accessAudit = new STRUCT_ACCESS_AUDIT_INFO()
                {
                    access_audit_id = connectionInfo.reader.GetInt32(0),
                    access_audit_datetime = connectionInfo.reader.GetDateTime(1).ToString(),
                    access_audit_performed_by_username = $"{connectionInfo.reader.GetString(2)} {connectionInfo.reader.GetString(3)}",
                    access_audit_target_pgrid_name = connectionInfo.reader.GetString(4)
                };

                pgridAccessAudits.Add(accessAudit);
            }

            connectionInfo.conn.Close();

            return pgridAccessAudits;
        }

        public List<STRUCT_NODES_LOGIN_AUDITS> DBGetPGPNLoginAudits(int _targetPgridID)
        {
            string sqlCommandText = "SELECT " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_ID, " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_FSP_USER, " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_DATETIME, " +
                "PNODE_LOGIN_STATUS.PNODE_LOGIN_STATUS_NAME, " +
                "NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_LOCATION, " +
                "PNODES.PNODE_NICKNAME, " +
                "PPOOLS.PPOOL_NAME " +
                "FROM NODES_LOGIN_AUDITS " +
                "INNER JOIN PNODE_LOGIN_STATUS ON NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_STATUS_ID = PNODE_LOGIN_STATUS.PNODE_LOGIN_STATUS_ID " +
                "INNER JOIN PNODES ON NODES_LOGIN_AUDITS.PNODE_LOGIN_AUDIT_TARGET_PNODE_ID = PNODES.PNODE_ID " +
                "INNER JOIN PPOOLS ON PNODES.PNODE_ASSOCIATED_PPOOL_ID = PPOOLS.PPOOL_ID " +
                "INNER JOIN PGRIDS ON PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                $"WHERE PGRIDS.PGRID_ID = {_targetPgridID} " +
                $"LIMIT 10;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            List<STRUCT_NODES_LOGIN_AUDITS> pgridPnodesLoginAudits = new List<STRUCT_NODES_LOGIN_AUDITS>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_NODES_LOGIN_AUDITS pgridPnodeLoginAudit = new STRUCT_NODES_LOGIN_AUDITS()
                {
                    login_audit_id = connectionInfo.reader.GetInt32(0),
                    login_audit_fsp_user = connectionInfo.reader.GetString(1),
                    login_audit_datetime = connectionInfo.reader.GetDateTime(2).ToString(),
                    login_audit_login_status = connectionInfo.reader.GetString(3),
                    login_audit_location = connectionInfo.reader.GetString(4),
                    login_audit_pnode_nickname = connectionInfo.reader.GetString(5),
                    login_audit_pnode_ppool_name = connectionInfo.reader.GetString(6)
                };

                pgridPnodesLoginAudits.Add(pgridPnodeLoginAudit);
            }

            connectionInfo.conn.Close();

            return pgridPnodesLoginAudits;
        }

        public List<STRUCT_FSP_ERROR_LOG_INFO> DBGetPGErrorLogs(int _targetPgridID)
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
                "PNODES.PNODE_NICKNAME, " +
                "PPOOLS.PPOOL_NAME " +
                "FROM PNODES_FSP_ERROR_LOGS " +
                "INNER JOIN PNODES ON PNODES_FSP_ERROR_LOGS.ERROR_LOG_SOURCE_PNODE_ID = PNODES.PNODE_ID " +
                "INNER JOIN PPOOLS ON PNODES.PNODE_ASSOCIATED_PPOOL_ID = PPOOLS.PPOOL_ID " +
                "INNER JOIN PGRIDS ON PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                $"WHERE PGRIDS.PGRID_ID = {_targetPgridID} " +
                $"LIMIT 10;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            List<STRUCT_FSP_ERROR_LOG_INFO> pgridPnodesErrorLogs = new List<STRUCT_FSP_ERROR_LOG_INFO>();

            while (connectionInfo.reader.Read())
            {
                List<string> actionFlags = new List<string>();
                string[] a = connectionInfo.reader.GetString(6).Split("g");

                for (int i = 0; i < a.Length; i++)
                {
                    actionFlags.Add(a[i]);
                }

                string[] logDateNTime = connectionInfo.reader.GetDateTime(1).ToString().Split(" ");

                STRUCT_FSP_ERROR_LOG_INFO pgridPnodeErrorLog = new STRUCT_FSP_ERROR_LOG_INFO()
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
                    PNodeNickname = connectionInfo.reader.GetString(9),
                    PPoolName = connectionInfo.reader.GetString(10)
                };

                pgridPnodesErrorLogs.Add(pgridPnodeErrorLog);
            }

            connectionInfo.conn.Close();

            return pgridPnodesErrorLogs;
        }

        public List<STRUCT_ATTENTION_LED_PNODES_INFO> DBGetAttentionLEDPNodes(int _targetPgridID)
        {
            string sqlCommandText = "SELECT " +
                "PNODES.PNODE_NICKNAME, " +
                "PPOOLS.PPOOL_NAME " +
                "FROM PNODES " +
                "INNER JOIN PPOOLS ON PNODES.PNODE_ASSOCIATED_PPOOL_ID = PPOOLS.PPOOL_ID " +
                "INNER JOIN PGRIDS ON PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID = PGRIDS.PGRID_ID " +
                $"WHERE PGRIDS.PGRID_ID = {_targetPgridID} AND PNODES.PANODE_ATTENTION_LED_STATE = 'ON' " +
                $"LIMIT 20;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            List<STRUCT_ATTENTION_LED_PNODES_INFO> attentionLEDMarkedPNodesInfo = new List<STRUCT_ATTENTION_LED_PNODES_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_ATTENTION_LED_PNODES_INFO attentionLEDMarkedPNodeInfo = new STRUCT_ATTENTION_LED_PNODES_INFO()
                {
                    pnode_nickname = connectionInfo.reader.GetString(0),
                    ppool_name = connectionInfo.reader.GetString(1)
                };

                attentionLEDMarkedPNodesInfo.Add(attentionLEDMarkedPNodeInfo);
            }

            connectionInfo.conn.Close();

            return attentionLEDMarkedPNodesInfo;
        }

        public List<STRUCT_PPOOLS_LIST> DBGetPGPPoolsList(int _targetPgridID)
        {
            string sqlCommandText = "WITH PNODES_COUNT_CTE AS ( " +
                "SELECT " +
                "PNODE_ASSOCIATED_PPOOL_ID, " +
                "COUNT(PNODE_ID) AS PNODES_COUNT " +
                "FROM PNODES " +
                "GROUP BY PNODE_ASSOCIATED_PPOOL_ID " +
                ") " +
                "SELECT " +
                "PPOOLS.PPOOL_ID, " +
                "PPOOLS.PPOOL_NAME, " +
                "COALESCE(PNODES_COUNT_CTE.PNODES_COUNT, 0) AS PNODE_COUNT " +
                "FROM PPOOLS " +
                "LEFT JOIN PNODES_COUNT_CTE ON PNODES_COUNT_CTE.PNODE_ASSOCIATED_PPOOL_ID = PPOOLS.PPOOL_ID " +
                $"WHERE PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID = {_targetPgridID};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

            List<STRUCT_PPOOLS_LIST> ppoolsInfoList = new List<STRUCT_PPOOLS_LIST>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_PPOOLS_LIST ppoolInfo = new STRUCT_PPOOLS_LIST()
                {
                    ppoolID = connectionInfo.reader.GetInt32(0),
                    ppool_name = connectionInfo.reader.GetString(1),
                    ppoolPnodesCount = connectionInfo.reader.GetInt32(2),
                    pnodesList = DBGetPGPPoolPNodesList(connectionInfo.reader.GetInt32(0))
                };

                ppoolsInfoList.Add(ppoolInfo);
            }

            connectionInfo.conn.Close();

            return ppoolsInfoList;
        }
    }
}