namespace POWERENV_DB_HANDLER.POWERENV_PGSQL_DB_HANDLER
{
    public class USER_DATA_HANDLING
    {
        #region VARIABLE DEFINITION

        private string connectionString;
        private POWERDB_PGSQL_DATA_HANDLING PARENT_DB_HANDLER;

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
            public string? TriggeredAt { get; set; }
            public string NotificationTargetUsername { get; set; }
            public string? NotificationAcknowledgementDatetime { get; set; }
            public string? NotificationResolvedDatetime { get; set; }
        }

        #endregion

        public USER_DATA_HANDLING(string dataSourceDirPath, POWERDB_PGSQL_DATA_HANDLING _parentDBHandler)
        {
            PARENT_DB_HANDLER = _parentDBHandler;
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

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters);

            UserProfileInfo userProfile = new UserProfileInfo();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                userProfile = new UserProfileInfo
                {
                    user_id = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    user_first_name = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    user_last_name = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    user_email = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    user_password_hash = (string)(connectionInfo.resultsDataTable.Rows[i][4]),
                    user_profile_picture = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    user_signup_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][6]).ToString(),
                    user_last_login_datetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][7]).ToString()
                };
            }

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

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }

        public List<NotificationInfo> DBGetUserNotifications(int userID)
        {
            string sqlCommandText = "CALL SP_GET_USER_NOTIFICATIONS(@userID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER { Name = "userID", Value = userID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<NotificationInfo> notificationInfoList = new List<NotificationInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                NotificationInfo notificationInfo = new NotificationInfo()
                {
                    NotificationId = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    SeverityLevel = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    Title = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    Description = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    TriggeredAt = ((DateTime)connectionInfo.resultsDataTable.Rows[i][4]).ToString(),
                    NotificationTargetUsername = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    NotificationAcknowledgementDatetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][6]).ToString(),
                    NotificationResolvedDatetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][7]).ToString()
                };

                notificationInfoList.Add(notificationInfo);
            }

            return notificationInfoList;
        }

        public List<NotificationInfo> DBGetAllUserNotifications(int userID)
        {
            string sqlCommandText = "CALL SP_GET_ALL_USER_NOTIFICATIONS(@userID, 'CURSOR');" +
                "FETCH ALL FROM \"CURSOR\";";

            SQL_QUERY_PARAMETER[] SQLQueryParameters = new SQL_QUERY_PARAMETER[]
            {
                new SQL_QUERY_PARAMETER { Name = "userID", Value = userID }
            };

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.readQueryFromDB(connectionString, sqlCommandText, SQLQueryParameters, true);

            List<NotificationInfo> notificationInfoList = new List<NotificationInfo>();

            for (int i = 0; i < connectionInfo.resultsDataTable.Rows.Count; i++)
            {
                NotificationInfo notificationInfo = new NotificationInfo()
                {
                    NotificationId = (int)(connectionInfo.resultsDataTable.Rows[i][0]),
                    SeverityLevel = (string)(connectionInfo.resultsDataTable.Rows[i][1]),
                    Title = (string)(connectionInfo.resultsDataTable.Rows[i][2]),
                    Description = (string)(connectionInfo.resultsDataTable.Rows[i][3]),
                    TriggeredAt = ((DateTime)connectionInfo.resultsDataTable.Rows[i][4]).ToString(),
                    NotificationTargetUsername = (string)(connectionInfo.resultsDataTable.Rows[i][5]),
                    NotificationAcknowledgementDatetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][6]).ToString(),
                    NotificationResolvedDatetime = ((DateTime)connectionInfo.resultsDataTable.Rows[i][7]).ToString()
                };

                notificationInfoList.Add(notificationInfo);
            }

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

            PGSQL_DB_CONNECTION_INFO connectionInfo = PARENT_DB_HANDLER.writeDataOnDB(connectionString, sqlCommandText, SQLQueryParameters, true);
            return connectionInfo.rowsAffected;
        }
    }
}