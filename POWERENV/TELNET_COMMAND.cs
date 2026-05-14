namespace XTELNET
{
    public class XTELNET_COMMAND
    {
        string prompt;
        string s;

        int MyConnectionimeOut;
        string MyHost;
        int MyRetryCount;
        int MyTimeOut;
        string MyLoginName;
        string MyPassword;

        int sessionID = -1;

        XTelnetConnection tc;

        public string MYHOST { get => MyHost; set => MyHost = value; }
        public int SessionID { get => sessionID; set => sessionID = value; }

        public XTELNET_COMMAND(int _connectionTimeOut, string _host, int _timeOut, int _retryCount, int _sessionID = -1)
        {
            MyConnectionimeOut = _connectionTimeOut;
            MyHost = _host;
            MyRetryCount = _retryCount;
            MyTimeOut = _timeOut;
            s = "";
            prompt = "";
            sessionID = _sessionID;
        }

        public void Login(string _username, string _password)
        {
            MyLoginName = _username;
            MyPassword = _password;

            if (MyConnectionimeOut == 0 || MyHost == "" || MyTimeOut == 0 || MyRetryCount == 0 || MyLoginName == "" || MyPassword == "")
            {
                Ajuda();
                System.Environment.Exit(1);
            }

            //create a new telnet connection to hostname "MyHost" on port 23

            Console.Write("A fazer a ligação ao servidor '" + MyHost + "'...\n");

            try
            {
                tc = new XTelnetConnection(MyHost, 23);
            }
            catch
            {
                Console.Write("\nHost '" + MyHost + "' inacessivel.\n");
                return;
            }
            //login with user "root",password "rootpassword", using a timeout of 100ms, and show server output
            try
            {
                s = tc.Login(MyLoginName, MyPassword, MyConnectionimeOut, MyTimeOut, MyRetryCount);
            }
            catch
            {
                Console.Write(s + "\nSaindo\n\n"); //"Ligação ao servidor falhou.\nSaindo...\n\n\n");
                Environment.Exit(1);
            }

            Console.Write(s);

            // server output should end with "$" or ">", otherwise the connection failed
            prompt = s.TrimEnd();
            prompt = s.Substring(prompt.Length - 1, 1);

            if (prompt != "$" && prompt != ">" && prompt != "#")
            {
                throw new Exception("Falha na ligação ao servidor.");
            }

            prompt = "";
        }

        public void Logout()
        {
            if(tc != null)
            {
                if (tc.IsConnected)
                {
                    tc.WriteLine("exit");
                    Console.Write(s);
                }
                else Console.WriteLine("Error! Terminal connection not established!");
            }
            else Console.WriteLine("Error! Terminal connection object null!");
        }

        public void ExecuteCommands(List<string> _commands)
        {
            if (tc.IsConnected)
            {
                for (int i = 0; i < _commands.Count; i++)
                {
                    Console.Write(tc.Read());
                    tc.WriteLine(_commands[i]);
                    s = tc.Read();
                    Console.Write(s);
                }
            }
        }

        public string ExecuteCommandsAndReturn(List<string> _commands)
        {
            string output = string.Empty;
            if (tc.IsConnected)
            {
                for (int i = 0; i < _commands.Count; i++)
                {
                    Console.Write(tc.Read());
                    tc.WriteLine(_commands[i]);
                    s = tc.Read();
                    Console.Write(s);
                    output += s;
                }
            }

            return output;
        }

        private static void Ajuda()
        {
            Console.Write("Syntax: MyTelnet -ct ConnectionTimeout -h host -r RetryCount -tr TimeoutRetray(miliseconds) -l LoginName -p Password -c MyRemoteCMDs\n");
            Console.Write("Example: MyTelnet -ct 100 -h 192.168.0.1 -r 2 -tr 1000 -l root -p pass -c ls -la;pwd\n");
            Console.Write("Example: MyTelnet -ct 100 -h 192.168.0.1 -r 2 -tr 1000 -l root -p pass -c \"cmd1;cmd2;cmd3\"\n");
        }
    }
}