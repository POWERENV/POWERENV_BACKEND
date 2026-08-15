namespace POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER
{
    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        //============================PGRID DATA HANDLING METHODS============================//

        #region READ

        public PGridFullInfo DBGetPGridFullInfo(int _targetPgridID)
        {
            string sqlCommandText = $"CALL SP_GET_PGRID_FULL_INFO(@targetPgridID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPgridID", Value = _targetPgridID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PGridFullInfo pgridFullInfo = new PGridFullInfo { };

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                pgridFullInfo = new PGridFullInfo
                {
                    pgrid_id = $"PG-{Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0])}",
                    pgrid_name = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    pgrid_creation_datetime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][2])).ToString(),
                    pgrid_last_update_datetime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][3])).ToString(),
                    pgrid_owner = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    pgrid_readme_text = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    pgrid_ppools_count = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][6]),
                    pgrid_pnodes_count = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][7]),
                    pgrid_active_pnodes_count = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][8])
                };
            }

            return pgridFullInfo;
        }

        public List<AccessPolicyInfo> DBGetPGAccessPolicies(int _targetPgridID)
        {
            string sqlCommandText = $"CALL SP_GET_PGRID_ACCESSPOLICIES(@targetPgridID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPgridID", Value = _targetPgridID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<AccessPolicyInfo> accessPolicies = new List<AccessPolicyInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                AccessPolicyInfo accessPolicy = new AccessPolicyInfo
                {
                    access_policy_id = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    access_policy_name = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    access_policy_pgrid_name = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    access_policy_target_username = $"{(string)(connectionInfo.resultsDataTable.Rows[i][3])} {(string)(connectionInfo.resultsDataTable.Rows[i][4])}",
                    access_policy_creation_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][5]).ToString(),
                    access_policy_last_update_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][6]).ToString(),
                    access_policy_permission_level = (string)connectionInfo.resultsDataTable.Rows[i][7]
                };

                accessPolicies.Add(accessPolicy);
            }

            return accessPolicies;
        }

        public List<AccessAuditInfo> DBGetPGAccessAudits(int _targetPgridID)
        {
            string sqlCommandText = $"CALL SP_GET_PGRID_ACCESSAUDITS(@targetPgridID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPgridID", Value = _targetPgridID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<AccessAuditInfo> pgridAccessAudits = new List<AccessAuditInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                AccessAuditInfo accessAudit = new AccessAuditInfo
                {
                    access_audit_id = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    access_audit_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][1]).ToString(),
                    access_audit_performed_by_username = $"{(string)(connectionInfo.resultsDataTable.Rows[i][2])} {(string)(connectionInfo.resultsDataTable.Rows[i][3])}",
                    access_audit_target_pgrid_name = (string)(connectionInfo.resultsDataTable.Rows[i][4])
                };

                pgridAccessAudits.Add(accessAudit);
            }

            return pgridAccessAudits;
        }

        public List<NodesLoginAudits> DBGetPGPNLoginAudits(int _targetPgridID)
        {
            string sqlCommandText = $"CALL SP_GET_PGRID_PNODES_LOGINAUDITS(@targetPgridID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPgridID", Value = _targetPgridID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<NodesLoginAudits> pgridPnodesLoginAudits = new List<NodesLoginAudits>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                NodesLoginAudits pgridPnodeLoginAudit = new NodesLoginAudits()
                {
                    login_audit_id = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    login_audit_fsp_user = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    login_audit_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][2]).ToString(),
                    login_audit_login_status = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    login_audit_location = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    login_audit_pnode_nickname = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    login_audit_pnode_ppool_name = (string)(connectionInfo.resultsDataTable.Rows[i][6])
                };

                pgridPnodesLoginAudits.Add(pgridPnodeLoginAudit);
            }

            return pgridPnodesLoginAudits;
        }

        public List<FSPErrorLogInfo> DBGetPGErrorLogs(int _targetPgridID)
        {
            string sqlCommandText = $"CALL SP_GET_PGRID_ERROR_LOGS(@targetPgridID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPgridID", Value = _targetPgridID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<FSPErrorLogInfo> pgridPnodesErrorLogs = new List<FSPErrorLogInfo>();

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
                    PNodeNickname = (string)(connectionInfo.resultsDataTable.Rows[i][9]),
                    PPoolName = (string)(connectionInfo.resultsDataTable.Rows[i][10])
                };

                pgridPnodesErrorLogs.Add(pnodeErrorLog);
            }

            return pgridPnodesErrorLogs;
        }

        public List<AttentionLEDPNodesInfo> DBGetAttentionLEDPNodes(int _targetPgridID)
        {
            string sqlCommandText = $"CALL SP_GET_PGRID_ATTENTIONLED_PNODES(@targetPgridID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPgridID", Value = _targetPgridID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<AttentionLEDPNodesInfo> attentionLEDMarkedPNodesInfo = new List<AttentionLEDPNodesInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                AttentionLEDPNodesInfo attentionLEDMarkedPNodeInfo = new AttentionLEDPNodesInfo()
                {
                    pnode_nickname = (string)(connectionInfo.resultsDataTable.Rows[i][0]),
                    ppool_name = (string)(connectionInfo.resultsDataTable.Rows[i][1])
                };

                attentionLEDMarkedPNodesInfo.Add(attentionLEDMarkedPNodeInfo);
            }

            return attentionLEDMarkedPNodesInfo;
        }

        public List<PPoolsList> DBGetPGPPoolsList(int _targetPgridID)
        {
            string sqlCommandText = $"CALL SP_GET_PGRIDS_PPOOLS_LIST(@targetPgridID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPgridID", Value = _targetPgridID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PPoolsList> ppoolsInfoList = new List<PPoolsList>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PPoolsList ppoolInfo = new PPoolsList
                {
                    ppoolID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    ppool_name = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    ppoolPnodesCount = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][2]),
                    pnodesList = DBGetPGPPoolPNodesList(Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]))
                };

                ppoolsInfoList.Add(ppoolInfo);
            }

            return ppoolsInfoList;
        }

        #endregion READ

        #region WRITE

        public int DBCreateNewPGrid(int userID, PGridFullInfo newPGridInfo)
        {
            string sqlCommandText = $"CALL SP_CREATE_PGRID(" +
                $"@pgridName," +
                $"@pgridReadmeText," +
                $"@pgridOwnerId," +
                $"NULL" +
                $");";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pgridName", Value = newPGridInfo.pgrid_name},
                new SQL_QUERY_PARAMETER { Name = "pgridReadmeText", Value = newPGridInfo.pgrid_readme_text},
                new SQL_QUERY_PARAMETER { Name = "pgridOwnerId", Value = userID}
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        public int DBDeletePGrid(int userID, int pgrid_id)
        {
            string sqlCommandText = $"CALL SP_DELETE_PGRID(" +
                $"@pgridID," +
                $"NULL" +
                $");";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "pgridID", Value = pgrid_id}
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}