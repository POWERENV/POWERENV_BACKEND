using Npgsql;
using POWERENV_PGSQL_DB_HANDLER;
using static POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_DB_HANDLER.USER_DATA_HANDLING
{
    public class USER_DATA_HANDLING
    {
        #region VARIABLE DEFINITION

        string connectionString;

        public record UserProfileInfo
        {
            public int? user_id { get; set; }
            public string? user_first_name { get; set; } = string.Empty;
            public string? user_last_name { get; set; } = string.Empty;
            public string? user_email { get; set; } = string.Empty;
            public string? user_password_hash { get; set; } = string.Empty;
            public string? user_profile_picture { get; set; } = string.Empty;
            public string? user_signup_datetime { get; set; } = string.Empty;
            public string? user_last_login_datetime { get; set; } = string.Empty;
        }

        public record LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public record SignupRequest
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        #endregion

        public USER_DATA_HANDLING(string dataSourceDirPath)
        {
            string DBPassword = Environment.GetEnvironmentVariable("POWERENV_DB_PASSWORD");
            string DBIPAddress = Environment.GetEnvironmentVariable("POWERENV_DB_IPADDRESS");
            string DBPort = Environment.GetEnvironmentVariable("POWERENV_DB_PORT");

            if (DBPassword != null)
            {
                connectionString = $"Host={DBIPAddress};Port={DBPort};Username=postgres;Password={DBPassword};Database=POWERENV-POWERDB";
            }
            else throw new Exception("FATAL ERROR: DATABASE KEYS NOT FOUND!");
        }

        public UserProfileInfo DBValidateUsername(string _userEmail)
        {
            string sqlCommandText = "SELECT * FROM FN_VALIDATE_LOGIN(@user_email)";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER { Name = "user_email", Value = _userEmail }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters);

            UserProfileInfo userProfile = new UserProfileInfo();

            while (connectionInfo.reader.Read())
            {
                userProfile = new UserProfileInfo
                {
                    user_id = connectionInfo.reader.GetInt32(0),
                    user_first_name = connectionInfo.reader.GetString(1),
                    user_last_name = connectionInfo.reader.GetString(2),
                    user_email = connectionInfo.reader.GetString(3),
                    user_password_hash = connectionInfo.reader.GetString(4),
                    user_profile_picture = connectionInfo.reader.GetString(5),
                    user_signup_datetime = connectionInfo.reader.GetDateTime(6).ToString(),
                    user_last_login_datetime = connectionInfo.reader.GetDateTime(7).ToString()
                };
            }

            connectionInfo.conn.Close();

            return userProfile;
        }

        public int DBCreateUser(SignupRequest newUserFormData)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                "CALL SP_CREATE_USER(@user_first_name," +
                "@user_last_name," +
                "@user_email," +
                "@user_password_hash," +
                "NULL);" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER { Name = "user_first_name", Value = newUserFormData.FirstName },
                new SQL_QUERY_PARAMETER { Name = "user_last_name", Value = newUserFormData.LastName },
                new SQL_QUERY_PARAMETER { Name = "user_email", Value = newUserFormData.Email },
                new SQL_QUERY_PARAMETER { Name = "user_password_hash", Value = newUserFormData.Password }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }
    }
}