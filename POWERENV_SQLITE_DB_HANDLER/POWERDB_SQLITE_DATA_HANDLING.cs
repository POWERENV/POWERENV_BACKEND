using POWERENV_DB_HANDLER;
using POWERENV_PGSQL_DB_HANDLER;
using Microsoft.Data.Sqlite;

namespace POWERDB_SQLITE_DATA_HANDLING
{
    /// <summary>
    /// SQLite database connection info class, derived from the generic ICONNECTION_INFO database connection info interface.
    /// </summary>
    public class SQLITE_DB_CONNECTION_INFO : ICONNECTION_INFO
    {
        public SqliteConnection conn { get; set; }
        public SqliteDataReader reader { get; set; }
        public int rowsAffected { get; set; }
    }

    /// <summary>
    /// SQLite database handling class, derived from the generic ICONNECTION_INFO database handling interface.
    /// </summary>
    public class POWERDB_SQLITE_DATA_HANDLING : IDB_DATA_HANDLING
    {
        private static POWERDB_SQLITE_DATA_HANDLING autoInstance;
        private PSYSTEMS_HARDWARE_DATA_HANDLING hardwareDataHandler;

        /// <summary>
        /// Property for the actual POWERENV data interaction methods class.
        /// </summary>
        public PSYSTEMS_HARDWARE_DATA_HANDLING HARDWARE_DATA_HANDLER {
            get => hardwareDataHandler;
            set => hardwareDataHandler = value;
        }

        /// <summary>
        /// POWERDB_SQLITE_DATA_HANDLING class constructor.
        /// </summary>
        public POWERDB_SQLITE_DATA_HANDLING(string dataSourceDirPath, bool initializeAutoInstance = true)
        {
            HARDWARE_DATA_HANDLER = new PSYSTEMS_HARDWARE_DATA_HANDLING(dataSourceDirPath);
            if(initializeAutoInstance) autoInstance = new POWERDB_SQLITE_DATA_HANDLING(dataSourceDirPath, false);
        }

        //########################################################################################
        //########################################################################################

        /// <summary>
        /// Method to read data from a SQLITE database. Derived from the generic IDB_DATA_HANDLING interface.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <returns>ICONNECTION_INFO packet object.</returns>
        public ICONNECTION_INFO intReadQueryFromDB(string _connectionString, string _sqlCommandText)
        {
            SQLITE_DB_CONNECTION_INFO connectionInfo = new SQLITE_DB_CONNECTION_INFO();
            connectionInfo.conn = new SqliteConnection(_connectionString);
            connectionInfo.conn.Open();

            var cmd = new SqliteCommand(_sqlCommandText, connectionInfo.conn);
            connectionInfo.reader = cmd.ExecuteReader();

            return connectionInfo;
        }

        /// <summary>
        /// Static reference to intReadQueryFromDB database reading method.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <returns>SQLITE_DB_CONNECTION_INFO packet object.</returns>
        static internal SQLITE_DB_CONNECTION_INFO readQueryFromDB(string _connectionString, string _sqlCommandText)
        {
            return (SQLITE_DB_CONNECTION_INFO)autoInstance.intReadQueryFromDB(_connectionString, _sqlCommandText);
        }

        /// <summary>
        /// Method to write data on a SQLITE database. Derived from the generic IDB_DATA_HANDLING interface.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <returns>ICONNECTION_INFO packet object.</returns>
        public ICONNECTION_INFO intWriteDataOnDB(string _connectionString, string _sqlCommandText)
        {
            SQLITE_DB_CONNECTION_INFO connectionInfo = new SQLITE_DB_CONNECTION_INFO();
            connectionInfo.conn = new SqliteConnection(_connectionString);
            connectionInfo.conn.Open();

            var cmd = new SqliteCommand(_sqlCommandText, connectionInfo.conn);
            connectionInfo.rowsAffected = cmd.ExecuteNonQuery();

            return connectionInfo;
        }

        /// <summary>
        /// Static reference to intWriteDataOnDB database writing method.
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <param name="_sqlCommandText"></param>
        /// <returns>SQLITE_DB_CONNECTION_INFO packet object.</returns>
        static internal SQLITE_DB_CONNECTION_INFO writeDataOnDB(string _connectionString, string _sqlCommandText)
        {
            return (SQLITE_DB_CONNECTION_INFO)autoInstance.intWriteDataOnDB(_connectionString, _sqlCommandText);
        }
    }
}