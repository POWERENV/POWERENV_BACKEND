using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        //============================PGRID DATA HANDLING METHODS============================//

        public STRUCT_PGRID_FULL_INFO DBGetPGridFullInfo(int _targetPgridID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PGRID_FULL_INFO({_targetPgridID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PGRID_ACCESSPOLICIES({_targetPgridID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PGRID_ACCESSAUDITS({_targetPgridID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PGRID_PNODES_LOGINAUDITS({_targetPgridID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PGRID_ERROR_LOGS({_targetPgridID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PGRID_ATTENTIONLED_PNODES({_targetPgridID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PGRIDS_PPOOLS_LIST({_targetPgridID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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