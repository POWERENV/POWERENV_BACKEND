using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    //============================PPOOL DATA HANDLING METHODS============================//

    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        #region READ

        public List<PNodesBasicInfo> DBGetPGPPoolPNodesList(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL sp_get_ppool_pnodes_list({_targetPPoolID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<PNodesBasicInfo> pnodesInfoList = new List<PNodesBasicInfo>();

            while (connectionInfo.reader.Read())
            {
                PNodesBasicInfo pnodeInfo = new PNodesBasicInfo
                {
                    pnodeID = connectionInfo.reader.GetInt32(0),
                    pnodeName = connectionInfo.reader.GetString(1),
                    pnodeLparsCount = connectionInfo.reader.GetInt32(2)
                };

                pnodesInfoList.Add(pnodeInfo);
            }

            connectionInfo.conn.Close();

            return pnodesInfoList;
        }

        public PPoolFullInfo DBGetPPoolFullInfo(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_FULL_INFO({_targetPPoolID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            PPoolFullInfo pgridFullInfo = new PPoolFullInfo();

            while (connectionInfo.reader.Read())
            {
                pgridFullInfo = new PPoolFullInfo()
                {
                    ppool_id = connectionInfo.reader.GetInt32(0),
                    ppool_name = connectionInfo.reader.GetString(1),
                    ppool_tag = connectionInfo.reader.GetString(2),
                    ppool_parent_pgrid_name = connectionInfo.reader.GetString(3),
                    ppool_creation_datetime = connectionInfo.reader.GetDateTime(4).ToString(),
                    ppool_last_update_datetime = connectionInfo.reader.GetDateTime(5).ToString(),
                    ppool_readme_text = connectionInfo.reader.GetString(6),
                    ppool_pnodes_count = connectionInfo.reader.GetInt32(7),
                    ppool_active_pnodes_count = connectionInfo.reader.GetInt32(8)
                };
            }

            connectionInfo.conn.Close();

            return pgridFullInfo;
        }

        public List<NodesLoginAudits> DBGetPPoolsLoginAudits(int _targetPpool)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_LOGIN_AUDITS({_targetPpool}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<NodesLoginAudits> pgridPnodesLoginAudits = new List<NodesLoginAudits>();

            while (connectionInfo.reader.Read())
            {
                NodesLoginAudits pgridPnodeLoginAudit = new NodesLoginAudits
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

        public List<AttentionLEDPNodesInfo> DBGetPPoolAttentionLEDPNodes(int _targetPpool)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_ATTENTIONLED_PNODES({_targetPpool}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<AttentionLEDPNodesInfo> attentionLEDMarkedPNodesInfo = new List<AttentionLEDPNodesInfo>();

            while (connectionInfo.reader.Read())
            {
                AttentionLEDPNodesInfo attentionLEDMarkedPNodeInfo = new AttentionLEDPNodesInfo
                {
                    pnode_nickname = connectionInfo.reader.GetString(0),
                    ppool_name = connectionInfo.reader.GetString(1)
                };

                attentionLEDMarkedPNodesInfo.Add(attentionLEDMarkedPNodeInfo);
            }

            connectionInfo.conn.Close();

            return attentionLEDMarkedPNodesInfo;
        }

        public List<FSPErrorLogInfo> DBGetPPoolsErrorLogs(int _targetPpool)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_ERROR_LOGS({_targetPpool}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<FSPErrorLogInfo> pgridPnodesErrorLogs = new List<FSPErrorLogInfo>();

            while (connectionInfo.reader.Read())
            {
                List<string> actionFlags = new List<string>();
                string[] a = connectionInfo.reader.GetString(6).Split("g");

                for (int i = 0; i < a.Length; i++)
                {
                    actionFlags.Add(a[i]);
                }

                string[] logDateNTime = connectionInfo.reader.GetDateTime(1).ToString().Split(" ");

                FSPErrorLogInfo pgridPnodeErrorLog = new FSPErrorLogInfo()
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

        private List<PNodesSingleOperationHistory> DBGetPPoolPNodesSingleOperationLogs(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_PNODES_SINGLE_OPERATION_LOGS({_targetPPoolID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

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

        private List<PPoolsBatchOperationHistory> DBGetPPoolBatchOperationLogs(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_BATCH_OPERATION_LOGS({_targetPPoolID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<PPoolsBatchOperationHistory> ppoolBatchOperationHistory = new List<PPoolsBatchOperationHistory>();

            while (connectionInfo.reader.Read())
            {
                PPoolsBatchOperationHistory ppoolBatchOperationLog = new PPoolsBatchOperationHistory()
                {
                    batchOperationID = connectionInfo.reader.GetInt32(0),
                    batchOperationCatName = connectionInfo.reader.GetString(1),
                    batchOperationSourcePPoolID = connectionInfo.reader.GetInt32(2),
                    batchOperationSourcePPoolName = connectionInfo.reader.GetString(3),
                    batchOperationAction = connectionInfo.reader.GetString(4),
                    batchOperationDateTime = connectionInfo.reader.GetDateTime(5).ToString(),
                    batchOperationSourceUserName = connectionInfo.reader.GetString(6),
                };

                ppoolBatchOperationHistory.Add(ppoolBatchOperationLog);
            }

            connectionInfo.conn.Close();

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
            string sqlCommandText = $"BEGIN TRANSACTION;" +
                $"CALL SP_PPOOL_EDIT_README({ppoolID}, '{newReadmeText}', NULL);" +
                $"COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}