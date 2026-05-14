using XTELNET;

namespace POWER_ENV
{
    public class AUTH_MGMT
    {
        public enum ENUM_LOGIN_AUDIT_TYPE
        {
            SuccessfullLogin,
            FailedLogin
        }

        // This class handles authentication procedures for ASMI and OS operations.
        public struct STRUCT_AUTH_INFO
        {
            public string username;
            public string password;
        }

        public struct STRUCT_OS_USER_INFO
        {
            public int os_id { get; set; }
            public STRUCT_AUTH_INFO osAuthInfo { get; set; }
            public string os_ip_address { get; set; }
            public string os_family { get; set; }
        }

        private STRUCT_AUTH_INFO machine_auth_info = new STRUCT_AUTH_INFO() {
            username = "admin",
            password = "Password123"
        };

        private STRUCT_AUTH_INFO os_auth_info = new STRUCT_AUTH_INFO()
        {
            username = "root",
            password = "trapa"
        };

        private STRUCT_OS_USER_INFO os_user_info = new STRUCT_OS_USER_INFO();

        public struct STRUCT_LOGIN_AUDIT_INFO
        {
            public string user { get; set; }
            public string date { get; set; }
            public string time { get; set; }
            public string location { get; set; }
        };

        public STRUCT_AUTH_INFO MACHINE_AUTH_INFO { get => machine_auth_info; set => machine_auth_info = value; }

        public STRUCT_OS_USER_INFO OS_INFO { get => os_user_info; set => os_user_info = value; }

        public void ASMIAuthenticate()
        {
            // At the beggining, the console must print the text "You have logged out"
            POWERENV.SendCommand("\n", 200);
            POWERENV.SendCommand(MACHINE_AUTH_INFO.username, 200);
            POWERENV.SendCommand(MACHINE_AUTH_INFO.password, 200);
            POWERENV.SendCommand("\n", 200);
            POWERENV.SendCommand("\n", 200);
            POWERENV.SendCommand("\n", 700);
            Console.WriteLine("LOGGED INTO ASMI");
        }

        public void ASMISignOut()
        {
            POWERENV.SendCommand("99", 200);
            POWERENV.SendCommand("\n", 500);
            POWERENV.GetReceivedData();
        }

        public void ASMIChangePassword(string newPassword)
        {
            POWERENV.CheckSerialSessionOpen();
            ASMIAuthenticate();
            POWERENV.SendCommand("9", 200);
            POWERENV.SendCommand("1", 200);
            POWERENV.SendCommand(machine_auth_info.username, 200);
            POWERENV.SendCommand(machine_auth_info.password, 200);
            POWERENV.SendCommand(newPassword, 200);
            POWERENV.SendCommand(newPassword, 1000);
            POWERENV.SendCommand("\n", 200);
            machine_auth_info.password = newPassword;
            ASMISignOut();
        }

        public void OSAuthenticate(XTELNET_COMMAND xtelnet_session)
        {
            xtelnet_session.Login(OS_INFO.osAuthInfo.username, OS_INFO.osAuthInfo.password);
        }
        public List<STRUCT_LOGIN_AUDIT_INFO> getASMILoginAudits(ENUM_LOGIN_AUDIT_TYPE loginAuditType)
        {
            POWERENV.CheckSerialSessionOpen();
            ASMIAuthenticate();

            POWERENV.SendCommand("9", 200);
            POWERENV.SendCommand("2", 700);
            POWERENV.GetReceivedData(); // Clear the buffer

            switch (loginAuditType)
            {
                case ENUM_LOGIN_AUDIT_TYPE.SuccessfullLogin:
                    POWERENV.SendCommand("1", 5000);
                    Console.WriteLine("ASMISuccessfullLoginAudits:");
                    break;
                case ENUM_LOGIN_AUDIT_TYPE.FailedLogin:
                    POWERENV.SendCommand("2", 5000);
                    Console.WriteLine("ASMIFailedLoginAudits:");
                    break;
            }

            string response = POWERENV.GetReceivedData();

            Console.WriteLine(response);

            string[] lines = response.Split("\n", StringSplitOptions.None);
            List<STRUCT_LOGIN_AUDIT_INFO> loginAuditsInfo = new List<STRUCT_LOGIN_AUDIT_INFO>();

            for (int i = 5; i < lines.Length - 2; i++)
            {
                string[] splitedLine = lines[i].Split(" ", StringSplitOptions.None);
                List<string> LogProps = new List<string>();

                for (int j = 0; j < splitedLine.Length; j++)
                {
                    if (splitedLine[j] != string.Empty)
                    {
                        LogProps.Add(splitedLine[j]);
                    }
                }

                loginAuditsInfo.Add(new STRUCT_LOGIN_AUDIT_INFO()
                {
                    user = LogProps[0].Substring(1),
                    date = LogProps[1],
                    time = LogProps[2],
                    location = LogProps[3].Trim()
                });
            }

            POWERENV.SendCommand("\n", 200);
            ASMISignOut();

            return loginAuditsInfo;
        }
    }
}