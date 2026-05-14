using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_PGSQL_DB_HANDLER
{
    //============================PPOOL DATA HANDLING METHODS============================//

    public partial class PSYSTEMS_HARDWARE_DATA_HANDLING
    {
        #region READ

        public List<STRUCT_PNODES_BASIC_INFO> DBGetPGPPoolPNodesList(int _targetPPoolID)
        {
            string sqlCommandText = "WITH LPARS_COUNT_CTE AS ( " +
                "SELECT " +
                "LPAR_ASSOCIATED_PNODE_ID, " +
                "COUNT(*) AS LPARS_COUNT " +
                "FROM LPARS " +
                "GROUP BY LPAR_ASSOCIATED_PNODE_ID ) " +
                "SELECT " +
                "PNODES.PNODE_ID, " +
                "PNODES.PNODE_NICKNAME, " +
                "COALESCE(LPARS_COUNT_CTE.LPARS_COUNT, 0) " +
                "FROM PNODES " +
                "LEFT JOIN LPARS_COUNT_CTE ON LPARS_COUNT_CTE.LPAR_ASSOCIATED_PNODE_ID = PNODES.PNODE_ID " +
                $"WHERE PNODES.PNODE_ASSOCIATED_PPOOL_ID = {_targetPPoolID};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = "WITH PNODE_COUNT_CTE AS ( " +
                "SELECT " +
                "PNODES.PNODE_ASSOCIATED_PPOOL_ID, " +
                "COUNT(*) AS PNODE_COUNT " +
                "FROM PNODES " +
                "GROUP BY PNODES.PNODE_ASSOCIATED_PPOOL_ID" +
                "), " +
                "ACTIVE_PNODE_COUNT_CTE AS ( " +
                "SELECT " +
                "PNODES.PNODE_ASSOCIATED_PPOOL_ID, " +
                "COUNT(*) AS ACTIVE_PNODE_COUNT " +
                "FROM PNODES " +
                "INNER JOIN PNODE_STATUS ON PNODE_STATUS.PNODE_STATUS_ID = PNODES.PNODE_STATUS_ID " +
                "WHERE PNODE_STATUS.PNODE_STATUS_NAME = 'ACTIVE' " +
                "GROUP BY PNODES.PNODE_ASSOCIATED_PPOOL_ID" +
                ") " +
                "SELECT " +
                "PPOOLS.PPOOL_ID, " +
                "PPOOLS.PPOOL_NAME, " +
                "PPOOLS.PPOOL_TAG, " +
                "PGRIDS.PGRID_NAME, " +
                "PPOOLS.PPOOL_CREATION_DATETIME, " +
                "PPOOLS.PPOOL_LAST_UPDATE_DATETIME, " +
                "PPOOLS.PPOOL_README_TEXT, " +
                "COALESCE(PNODE_COUNT_CTE.PNODE_COUNT, 0), " +
                "COALESCE(ACTIVE_PNODE_COUNT_CTE.ACTIVE_PNODE_COUNT, 0) " +
                "FROM PPOOLS " +
                "INNER JOIN PGRIDS ON PGRIDS.PGRID_ID = PPOOLS.PPOOL_ASSOCIATERD_PGRID_ID " +
                "LEFT JOIN PNODE_COUNT_CTE ON PNODE_COUNT_CTE.PNODE_ASSOCIATED_PPOOL_ID = PPOOLS.PPOOL_ID " +
                "LEFT JOIN ACTIVE_PNODE_COUNT_CTE ON ACTIVE_PNODE_COUNT_CTE.PNODE_ASSOCIATED_PPOOL_ID = PPOOLS.PPOOL_ID " +
                $"WHERE PPOOLS.PPOOL_ID = {_targetPPoolID}";
            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
                $"WHERE PPOOLS.PPOOL_ID = {_targetPpool} " +
                $"LIMIT 20;";
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

        public List<STRUCT_ATTENTION_LED_PNODES_INFO> DBGetPPoolAttentionLEDPNodes(int _targetPpool)
        {
            string sqlCommandText = "SELECT " +
                "PNODES.PNODE_NICKNAME, " +
                "PPOOLS.PPOOL_NAME " +
                "FROM PNODES " +
                "INNER JOIN PPOOLS ON PNODES.PNODE_ASSOCIATED_PPOOL_ID = PPOOLS.PPOOL_ID " +
                $"WHERE PPOOLS.PPOOL_ID = {_targetPpool} AND PNODES.PANODE_ATTENTION_LED_STATE = 'ON' " +
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

        public List<STRUCT_FSP_ERROR_LOG_INFO> DBGetPPoolsErrorLogs(int _targetPpool)
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
                $"WHERE PPOOLS.PPOOL_ID = {_targetPpool} " +
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

        private List<STRUCT_PNODES_SINGLE_OPERATION_HISTORY> DBGetPPoolPNodesSingleOperationLogs(int _targetPPoolID)
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
                $"WHERE PNODES.PNODE_ASSOCIATED_PPOOL_ID = {_targetPPoolID} " +
                $"ORDER BY PNODE_OPERATIONS.OPERATION_DATETIME DESC " +
                $"LIMIT 50;";

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

        private List<STRUCT_PPOOLS_BATCH_OPERATION_HISTORY> DBGetPPoolBatchOperationLogs(int _targetPPoolID)
        {
            string sqlCommandText = "SELECT " +
                "PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_ID, " +
                "PNODE_OPERATION_CATEGORIES.OPERATION_CAT_NAME, " +
                "PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_SOURCE_PPOOL_ID, " +
                "PPOOLS.PPOOL_NAME, " +
                "PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_ACTION, " +
                "PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_DATETIME, " +
                "USERS.USER_FIRST_NAME || ' ' || USERS.USER_LAST_NAME " +
                "FROM PPOOLS_BATCH_OPERATIONS " +
                "INNER JOIN PNODE_OPERATION_CATEGORIES ON PNODE_OPERATION_CATEGORIES.OPERATION_CAT_ID = PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_CAT_ID " +
                "INNER JOIN PPOOLS ON PPOOLS.PPOOL_ID = PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_SOURCE_PPOOL_ID " +
                "INNER JOIN USERS ON USERS.USER_ID = PPOOLS_BATCH_OPERATIONS.BATCH_OPERATION_SOURCE_USER_ID " +
                $"WHERE PPOOLS.PPOOL_ID = {_targetPPoolID}";

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText);

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
            string sqlCommandText = $"UPDATE PPOOLS " +
                $"SET PPOOL_README_TEXT = '{newReadmeText}' " +
                $"WHERE PPOOL_ID = {ppoolID};";

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText);
            return connectionInfo.rowsAffected;
        }

        #endregion WRITE
    }
}