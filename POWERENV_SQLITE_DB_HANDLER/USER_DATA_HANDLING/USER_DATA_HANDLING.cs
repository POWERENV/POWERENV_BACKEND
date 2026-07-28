using static POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER.POWERDB_PGSQL_DATA_HANDLING;

namespace POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER
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

        public record NotificationInfo
        {
            public int NotificationId { get; set; }
            public string SeverityLevel { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string TriggeredAt { get; set; }
            public string NotificationTargetUsername { get; set; }
            public string NotificationAcknowledgementDatetime { get; set; }
            public string NotificationResolvedDatetime { get; set; }
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

        public List<NotificationInfo> DBGetUserNotifications(int userID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                "CALL SP_GET_USER_NOTIFICATIONS(@userID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER { Name = "userID", Value = userID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<NotificationInfo> notificationInfoList = new List<NotificationInfo>();

            while (connectionInfo.reader.Read())
            {
                NotificationInfo notificationInfo = new NotificationInfo() {
                    NotificationId = connectionInfo.reader.GetInt32(0),
                    SeverityLevel = connectionInfo.reader.GetString(1),
                    Title = connectionInfo.reader.GetString(2),
                    Description = connectionInfo.reader.GetString(3),
                    TriggeredAt = connectionInfo.reader.GetDateTime(4).ToString(),
                    NotificationTargetUsername = connectionInfo.reader.GetString(5),
                    NotificationAcknowledgementDatetime = connectionInfo.reader.GetDateTime(6).ToString(),
                    NotificationResolvedDatetime = connectionInfo.reader.GetDateTime(7).ToString()
                };

                notificationInfoList.Add(notificationInfo);
            }

            connectionInfo.conn.Close();

            return notificationInfoList;
        }

        public int DBMarkNotificationAsResolved(int notificationID)
        {
            string sqlCommandText = "BEGIN TRANSACTION;" +
                "CALL SP_MARK_EVENT_AS_RESOLVED(@NotificationID, NULL);" +
                "COMMIT;";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER { Name = "NotificationID", Value = notificationID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }
    }
}