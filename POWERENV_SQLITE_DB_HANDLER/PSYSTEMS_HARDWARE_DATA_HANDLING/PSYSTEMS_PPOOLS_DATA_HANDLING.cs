using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    //============================PPOOL DATA HANDLING METHODS============================//

    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        #region READ

        public List<STRUCT_PNODES_BASIC_INFO> DBGetPGPPoolPNodesList(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL sp_get_ppool_pnodes_list({_targetPPoolID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<STRUCT_PNODES_BASIC_INFO> pnodesInfoList = new List<STRUCT_PNODES_BASIC_INFO>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_PNODES_BASIC_INFO pnodeInfo = new STRUCT_PNODES_BASIC_INFO()
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

        public STRUCT_PPOOL_FULL_INFO DBGetPPoolFullInfo(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_FULL_INFO({_targetPPoolID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            STRUCT_PPOOL_FULL_INFO pgridFullInfo = new STRUCT_PPOOL_FULL_INFO();

            while (connectionInfo.reader.Read())
            {
                pgridFullInfo = new STRUCT_PPOOL_FULL_INFO()
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

        public List<STRUCT_NODES_LOGIN_AUDITS> DBGetPPoolsLoginAudits(int _targetPpool)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_LOGIN_AUDITS({_targetPpool}, 'CURSOR');" +
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

        public List<STRUCT_ATTENTION_LED_PNODES_INFO> DBGetPPoolAttentionLEDPNodes(int _targetPpool)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_ATTENTIONLED_PNODES({_targetPpool}, 'CURSOR');" +
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

        public List<STRUCT_FSP_ERROR_LOG_INFO> DBGetPPoolsErrorLogs(int _targetPpool)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_ERROR_LOGS({_targetPpool}, 'CURSOR');" +
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

        private List<STRUCT_PNODES_SINGLE_OPERATION_HISTORY> DBGetPPoolPNodesSingleOperationLogs(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_PNODES_SINGLE_OPERATION_LOGS({_targetPPoolID}, 'CURSOR');" +
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

        private List<STRUCT_PPOOLS_BATCH_OPERATION_HISTORY> DBGetPPoolBatchOperationLogs(int _targetPPoolID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                $"CALL SP_GET_PPOOL_BATCH_OPERATION_LOGS({_targetPPoolID}, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, true);

            List<STRUCT_PPOOLS_BATCH_OPERATION_HISTORY> ppoolBatchOperationHistory = new List<STRUCT_PPOOLS_BATCH_OPERATION_HISTORY>();

            while (connectionInfo.reader.Read())
            {
                STRUCT_PPOOLS_BATCH_OPERATION_HISTORY ppoolBatchOperationLog = new STRUCT_PPOOLS_BATCH_OPERATION_HISTORY()
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

        public STRUCT_PPOOLS_OPERATION_LOGS DBGetPPoolsOperationLogs(int _targetPpool)
        {
            STRUCT_PPOOLS_OPERATION_LOGS ppoolOperationLogs = new STRUCT_PPOOLS_OPERATION_LOGS()
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