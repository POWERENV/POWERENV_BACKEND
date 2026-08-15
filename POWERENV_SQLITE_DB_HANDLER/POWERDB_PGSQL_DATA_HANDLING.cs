using Npgsql;
using System.Data;

namespace POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER
{
    /// <summary>
    /// PostgreSQL database connection info class, derived from the generic ICONNECTION_INFO database connection info interface.
    /// </summary>
    public class PGSQL_DB_CONNECTION_INFO : ICONNECTION_INFO
    {
        public required NpgsqlConnection conn { get; set; }
        public NpgsqlDataReader? reader { get; set; }
        public DataTable? resultsDataTable { get; set; }
        public int rowsAffected { get; set; }
    }

    /// <summary>
    /// PostgreSQL database handling class, derived from the generic database handling interface.
    /// </summary>
    public class POWERDB_PGSQL_DATA_HANDLING : IDB_DATA_HANDLING
    {
        //private static POWERDB_PGSQL_DATA_HANDLING autoInstance;
        private NpgsqlDataSource connectionDataSource;
        private PSYSTEMS_HARDWARE_DATA_HANDLING hardwareDataHandler;
        private USER_DATA_HANDLING userDataHandler;

        /// <summary>
        /// Property for the actual POWERENV data interaction methods class.
        /// </summary>
        public PSYSTEMS_HARDWARE_DATA_HANDLING HARDWARE_DATA_HANDLER
        {
            get => hardwareDataHandler;
            set => hardwareDataHandler = value;
        }

        /// <summary>
        /// Property for the actual POWERENV data interaction methods class.
        /// </summary>
        public USER_DATA_HANDLING USER_DATA_HANDLER
        {
            get => userDataHandler;
            set => userDataHandler = value;
        }
        /*public static NpgsqlDataSource ConnectionDataSource {
            get {
                if(autoInstance != null) return autoInstance.connectionDataSource;
                return null;
            }
            set {
                if (autoInstance != null) autoInstance.connectionDataSource = value;
            }
        }*/

        public NpgsqlDataSource ConnectionDataSource
        {
            get
            {
                return connectionDataSource;
            }
            set
            {
                connectionDataSource = value;
            }
        }

        /// <summary>
        /// POWERDB_PGSQL_DATA_HANDLING class constructor
        /// </summary>
        /// <param name="dataSourceDirPath"></param>
        public POWERDB_PGSQL_DATA_HANDLING(string dataSourceDirPath, bool initializeAutoInstance = true)
        {
            //if (initializeAutoInstance) autoInstance = new POWERDB_PGSQL_DATA_HANDLING(dataSourceDirPath, false);
            HARDWARE_DATA_HANDLER = new PSYSTEMS_HARDWARE_DATA_HANDLING(dataSourceDirPath, this);
            USER_DATA_HANDLER = new USER_DATA_HANDLING(dataSourceDirPath, this);
        }

        //###########################################################################################
        //###########################################################################################

        /// <summary>
        /// Method to read data from PostgreSQL database, returning a PGSQL_DB_CONNECTION_INFO object containing the connection and reader objects.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <param name="parameters"></param>
        /// <returns>ICONNECTION_INFO packet object.</returns>
        public ICONNECTION_INFO intReadQueryFromDB(string _connectionString, string _sqlCommandText, SQL_QUERY_PARAMETER[] parameters, bool hasCursor)
        {
            DataTable resultDataTable = new DataTable();
            PGSQL_DB_CONNECTION_INFO connectionInfo = new PGSQL_DB_CONNECTION_INFO()
            {
                conn = ConnectionDataSource.CreateConnection()
            };
            connectionInfo.conn.Open();

            using NpgsqlTransaction transaction = connectionInfo.conn.BeginTransaction();

            try
            {
                var cmd = new NpgsqlCommand(_sqlCommandText, connectionInfo.conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    cmd.Parameters.AddWithValue(parameters[i].Name, parameters[i].Value);
                }

                connectionInfo.reader = cmd.ExecuteReader();

                if (hasCursor) connectionInfo.reader.NextResult();

                resultDataTable.Load(connectionInfo.reader);
                connectionInfo.resultsDataTable = resultDataTable;

                transaction.Commit();
                connectionInfo.conn.Close();
            }
            catch (Exception ex)
            {
                if (connectionInfo.reader != null && !connectionInfo.reader.IsClosed) connectionInfo.reader.Close();

                try
                {
                    transaction.Rollback();
                }
                catch (InvalidOperationException) { }

                connectionInfo.conn.Close();
                throw;
            }

            return connectionInfo;
        }

        /// <summary>
        /// Static reference method to read data from PostgreSQL database, returning a PGSQL_DB_CONNECTION_INFO object containing the connection and reader objects.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <returns>PGSQL_DB_CONNECTION_INFO packet object.</returns>
        internal PGSQL_DB_CONNECTION_INFO readQueryFromDB(string _connectionString, string _sqlCommandText, bool hasCursor = false)
        {
            return (PGSQL_DB_CONNECTION_INFO)intReadQueryFromDB(_connectionString, _sqlCommandText, Array.Empty<SQL_QUERY_PARAMETER>(), hasCursor);
        }

        /// <summary>
        /// Static reference method to read data from PostgreSQL database, replacing query parameters by the indicated values, and returning a PGSQL_DB_CONNECTION_INFO object containing the connection and reader objects.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <param name="parameters"></param>
        /// <returns>PGSQL_DB_CONNECTION_INFO packet object.</returns>
        internal PGSQL_DB_CONNECTION_INFO readQueryFromDB(string _connectionString, string _sqlCommandText, SQL_QUERY_PARAMETER[] parameters, bool hasCursor = false)
        {
            return (PGSQL_DB_CONNECTION_INFO)intReadQueryFromDB(_connectionString, _sqlCommandText, parameters, hasCursor);
        }

        /// <summary>
        /// Method to write data on PostgreSQL database, returning a PGSQL_DB_CONNECTION_INFO object containing the connection object and the number of rows affected by the command.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <param name="parameters"></param>
        /// <returns>ICONNECTION_INFO packet object.</returns>
        public ICONNECTION_INFO intWriteDataOnDB(string _connectionString, string _sqlCommandText, SQL_QUERY_PARAMETER[] parameters, bool isStoredProcedure)
        {
            PGSQL_DB_CONNECTION_INFO connectionInfo = new PGSQL_DB_CONNECTION_INFO()
            {
                conn = ConnectionDataSource.CreateConnection()
            };
            connectionInfo.conn.Open();

            using NpgsqlTransaction transaction = connectionInfo.conn.BeginTransaction();

            try
            {
                var cmd = new NpgsqlCommand(_sqlCommandText, connectionInfo.conn);

                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = cmd.Parameters.AddWithValue(parameters[i].Name, parameters[i].Value);
                    if (parameters[i].SQLType != null) param.DataTypeName = parameters[i].SQLType;
                }

                // "DataTypes" returns a DataTable containing all types cached by the driver
                DataTable dataTypesTable = connectionInfo.conn.GetSchema("DataTypes");

                foreach (DataRow row in dataTypesTable.Rows)
                {
                    Console.WriteLine($"Type Name: {row["TypeName"]} | OID: {row["ProviderDbType"]}");
                }

                if (isStoredProcedure)
                {
                    object? nonQueryResult = cmd.ExecuteScalar();

                    try
                    {
                        connectionInfo.rowsAffected = Convert.ToInt32(nonQueryResult);
                    }
                    catch (Exception ex)
                    {
                        connectionInfo.rowsAffected = -1;
                        Console.WriteLine($"Error converting result to int: {ex.Message}");
                    }
                }
                else
                {
                    int nonQueryResult = cmd.ExecuteNonQuery();
                    connectionInfo.rowsAffected = nonQueryResult;
                }

                transaction.Commit();
                connectionInfo.conn.Close();
            }
            catch (Exception ex)
            {
                if (connectionInfo.reader != null && !connectionInfo.reader.IsClosed) connectionInfo.reader.Close();

                try
                {
                    transaction.Rollback();
                }
                catch (InvalidOperationException) { }

                connectionInfo.conn.Close();
                throw;
            }

            return connectionInfo;
        }

        /// <summary>
        /// Static reference method to write data on PostgreSQL database, returning a PGSQL_DB_CONNECTION_INFO object containing the connection object and the number of rows affected by the command.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <returns>PGSQL_DB_CONNECTION_INFO packet object.</returns>
        internal PGSQL_DB_CONNECTION_INFO writeDataOnDB(string _connectionString, string _sqlCommandText, bool isStoredProcedure = true)
        {
            return (PGSQL_DB_CONNECTION_INFO)intWriteDataOnDB(_connectionString, _sqlCommandText, Array.Empty<SQL_QUERY_PARAMETER>(), isStoredProcedure);
        }

        /// <summary>
        /// Static reference method to write data on PostgreSQL database, returning a PGSQL_DB_CONNECTION_INFO object containing the connection object and the number of rows affected by the command.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <param name="parameters"></param>
        /// <returns>PGSQL_DB_CONNECTION_INFO packet object.</returns>
        internal PGSQL_DB_CONNECTION_INFO writeDataOnDB(string _connectionString, string _sqlCommandText, SQL_QUERY_PARAMETER[] parameters, bool isStoredProcedure = true)
        {
            return (PGSQL_DB_CONNECTION_INFO)intWriteDataOnDB(_connectionString, _sqlCommandText, parameters, isStoredProcedure);
        }
    }
}