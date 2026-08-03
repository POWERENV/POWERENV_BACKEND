using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;
using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER
{
    //============================PPOOL DATA HANDLING METHODS============================//

    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        #region READ

        public List<PNodesBasicInfo> DBGetPGPPoolPNodesList(int _targetPPoolID)
        {
            string sqlCommandText = $"CALL sp_get_ppool_pnodes_list(@targetPPoolID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPPoolID", Value = _targetPPoolID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodesBasicInfo> pnodesInfoList = new List<PNodesBasicInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PNodesBasicInfo pnodeInfo = new PNodesBasicInfo
                {
                    pnodeID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    pnodeName = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    pnodeLparsCount = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][2])
                };

                pnodesInfoList.Add(pnodeInfo);
            }

            return pnodesInfoList;
        }

        public PPoolFullInfo DBGetPPoolFullInfo(int _targetPPoolID)
        {
            string sqlCommandText = $"CALL SP_GET_PPOOL_FULL_INFO(@targetPPoolID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPPoolID", Value = _targetPPoolID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            PPoolFullInfo pgridFullInfo = new PPoolFullInfo();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                pgridFullInfo = new PPoolFullInfo()
                {
                    ppool_id = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    ppool_name = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    ppool_tag = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    ppool_parent_pgrid_name = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    ppool_creation_datetime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][4])).ToString(),
                    ppool_last_update_datetime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][5])).ToString(),
                    ppool_readme_text = (string)(connectionInfo.resultsDataTable.Rows[i][6]),
                    ppool_pnodes_count = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][7]),
                    ppool_active_pnodes_count = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][8])
                };
            }

            return pgridFullInfo;
        }

        public List<NodesLoginAudits> DBGetPPoolsLoginAudits(int _targetPpool)
        {
            string sqlCommandText = $"CALL SP_GET_PPOOL_LOGIN_AUDITS(@targetPPoolID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPPoolID", Value = _targetPpool }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<NodesLoginAudits> pgridPnodesLoginAudits = new List<NodesLoginAudits>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                NodesLoginAudits pgridPnodeLoginAudit = new NodesLoginAudits
                {
                    login_audit_id = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    login_audit_fsp_user = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    login_audit_datetime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][2])).ToString(),
                    login_audit_login_status = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    login_audit_location = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    login_audit_pnode_nickname = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    login_audit_pnode_ppool_name = (string)(connectionInfo.resultsDataTable.Rows[i][6])
                };

                pgridPnodesLoginAudits.Add(pgridPnodeLoginAudit);
            }

            return pgridPnodesLoginAudits;
        }

        public List<AttentionLEDPNodesInfo> DBGetPPoolAttentionLEDPNodes(int _targetPpool)
        {
            string sqlCommandText = $"CALL SP_GET_PPOOL_ATTENTIONLED_PNODES(@targetPPoolID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPPoolID", Value = _targetPpool }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<AttentionLEDPNodesInfo> attentionLEDMarkedPNodesInfo = new List<AttentionLEDPNodesInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                AttentionLEDPNodesInfo attentionLEDMarkedPNodeInfo = new AttentionLEDPNodesInfo
                {
                    pnode_nickname = (string)(connectionInfo.resultsDataTable.Rows[i][0]),
                    ppool_name = (string)(connectionInfo.resultsDataTable.Rows[i][1])
                };

                attentionLEDMarkedPNodesInfo.Add(attentionLEDMarkedPNodeInfo);
            }

            return attentionLEDMarkedPNodesInfo;
        }

        public List<FSPErrorLogInfo> DBGetPPoolsErrorLogs(int _targetPpool)
        {
            string sqlCommandText = $"CALL SP_GET_PPOOL_ERROR_LOGS(@targetPPoolID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPPoolID", Value = _targetPpool }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);
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

        private List<PNodesSingleOperationHistory> DBGetPPoolPNodesSingleOperationLogs(int _targetPPoolID)
        {
            string sqlCommandText = $"CALL SP_GET_PPOOL_PNODES_SINGLE_OPERATION_LOGS(@targetPPoolID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPPoolID", Value = _targetPPoolID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PNodesSingleOperationHistory> ppoolPNodesSingleOperationHistory = new List<PNodesSingleOperationHistory>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PNodesSingleOperationHistory ppoolPNodesSingleOperationLog = new PNodesSingleOperationHistory()
                {
                    operationID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    operationCatName = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    operationSourcePNodeName = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    operationBatchOperationID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][3]),
                    operationBatchOperationName = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    operationAction = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    operationCompletionStatus = (string)(connectionInfo.resultsDataTable.Rows[i][6]),
                    operationDateTime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][7])).ToString(),
                    operationSourceUserName = (string)(connectionInfo.resultsDataTable.Rows[i][8])
                };

                ppoolPNodesSingleOperationHistory.Add(ppoolPNodesSingleOperationLog);
            }

            return ppoolPNodesSingleOperationHistory;
        }

        private List<PPoolsBatchOperationHistory> DBGetPPoolBatchOperationLogs(int _targetPPoolID)
        {
            string sqlCommandText = $"CALL SP_GET_PPOOL_BATCH_OPERATION_LOGS(@targetPPoolID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetPPoolID", Value = _targetPPoolID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PPoolsBatchOperationHistory> ppoolBatchOperationHistory = new List<PPoolsBatchOperationHistory>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PPoolsBatchOperationHistory ppoolBatchOperationLog = new PPoolsBatchOperationHistory()
                {
                    batchOperationID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    batchOperationCatName = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    batchOperationSourcePPoolID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][2]),
                    batchOperationSourcePPoolName = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    batchOperationAction = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    batchOperationDateTime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][5])).ToString(),
                    batchOperationSourceUserName = (string)(connectionInfo.resultsDataTable.Rows[i][6]),
                };

                ppoolBatchOperationHistory.Add(ppoolBatchOperationLog);
            }

            return ppoolBatchOperationHistory;
        }

        public List<PPoolsBatchOperationHistory> DBGetUserBatchOperationLogs(int _targetUserID)
        {
            string sqlCommandText = $"CALL SP_GET_USER_BATCH_OPERATION_LOGS(@targetUserID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "targetUserID", Value = _targetUserID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<PPoolsBatchOperationHistory> ppoolBatchOperationHistory = new List<PPoolsBatchOperationHistory>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                PPoolsBatchOperationHistory ppoolBatchOperationLog = new PPoolsBatchOperationHistory()
                {
                    batchOperationID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][0]),
                    batchOperationCatName = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    batchOperationSourcePPoolID = Convert.ToInt32(connectionInfo.resultsDataTable.Rows[i][2]),
                    batchOperationSourcePPoolName = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    batchOperationAction = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    batchOperationDateTime = ((DateTime)(connectionInfo.resultsDataTable.Rows[i][5])).ToString(),
                    batchOperationSourceUserName = (string)(connectionInfo.resultsDataTable.Rows[i][6]),
                };

                ppoolBatchOperationHistory.Add(ppoolBatchOperationLog);
            }

            return ppoolBatchOperationHistory;
        }

        public PPoolsOperationHistory DBGetPPoolsOperationLogs(int _targetPpool)
        {
            PPoolsOperationHistory ppoolOperationLogs = new PPoolsOperationHistory
            {
                pnodesSingleOperationHistory = DBGetPPoolPNodesSingleOperationLogs(_targetPpool),
                ppoolsBatchOperationHistory = DBGetPPoolBatchOperationLogs(_targetPpool)
            };

            return ppoolOperationLogs;
        }

        #endregion READ

        #region WRITE

        public int DBPPoolEditReadme(int ppoolID, string newReadmeText)
        {
            string sqlCommandText = $"CALL SP_PPOOL_EDIT_README(" +
                $"@ppoolID," +
                $"@newReadmeText," +
                $"NULL" +
                $");";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = {
                new SQL_QUERY_PARAMETER { Name = "ppoolID", Value = ppoolID },
                new SQL_QUERY_PARAMETER { Name = "newReadmeText", Value = newReadmeText }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters);
            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}