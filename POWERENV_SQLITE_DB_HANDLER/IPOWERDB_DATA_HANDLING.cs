using POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER;

namespace POWERENV_DB_HANDLER
{
    /// <summary>
    /// Generic database connection info interface.
    /// </summary>
    public interface ICONNECTION_INFO
    {
        public int rowsAffected { get; set; }
    }

    public class SQL_QUERY_PARAMETER
    {
        public required string Name { get; set; }
        public string? SQLType { get; set; }
        public required object? Value { get; set; }
    }

    /// <summary>
    /// Generic database connection and handling interface.
    /// </summary>
    public interface IDB_DATA_HANDLING
    {
        public ICONNECTION_INFO intReadQueryFromDB(string _connectionString, string _sqlCommandText, SQL_QUERY_PARAMETER[] parameters, bool hasCursor);
        public ICONNECTION_INFO intWriteDataOnDB(string _connectionString, string _sqlCommandText, SQL_QUERY_PARAMETER[] parameters, bool isStoredProcedure);
    }
}