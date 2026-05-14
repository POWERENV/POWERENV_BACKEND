using POWERENV_PGSQL_DB_HANDLER;

namespace POWERENV_DB_HANDLER
{
    /// <summary>
    /// Generic database connection info interface.
    /// </summary>
    public interface ICONNECTION_INFO
    {
        public int rowsAffected { get; set; }
    }

    /// <summary>
    /// Generic database connection and handling interface.
    /// </summary>
    public interface IDB_DATA_HANDLING
    {
        public PSYSTEMS_HARDWARE_DATA_HANDLING HARDWARE_DATA_HANDLER { get; set; }
        public ICONNECTION_INFO intReadQueryFromDB(string _connectionString, string _sqlCommandText);
        public ICONNECTION_INFO intWriteDataOnDB(string _connectionString, string _sqlCommandText);
    }
}