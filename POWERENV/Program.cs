#pragma warning disable 8600
#pragma warning disable 8604

using System.IO.Ports;
using POWER_ENV.GLOBAL.NETWORK;
using POWER_ENV._9111_285__SF240_202;

namespace POWER_ENV
{
    public class POWERENV
    {
        static private SerialPort serialCOMPort = new SerialPort();
        static private POWER_MGMT powerManagementLib = new POWER_MGMT();
        static private AUTH_MGMT authManagementLib = new AUTH_MGMT();
        static private DATETIME_MGMT datetimeMgmt = new DATETIME_MGMT();
        static private NETWORK_MGMT networkMgmt = new NETWORK_MGMT();
        static private FSP_MGMT fspMgmt = new FSP_MGMT();

        public static POWER_MGMT PowerManagementLib { get => powerManagementLib; }
        public static AUTH_MGMT AuthManagementLib { get => authManagementLib; }
        public static DATETIME_MGMT DatetimeMgmt { get => datetimeMgmt; }
        public static NETWORK_MGMT NetworkMgmt { get => networkMgmt; }
        public static SerialPort SerialCOMPort { get => serialCOMPort; }
        public static FSP_MGMT FspMgmt { get => fspMgmt; }

        public void Main(int comPortIndex)
        {
            //Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);

            powerManagementLib = new POWER_MGMT();
            authManagementLib = new AUTH_MGMT();
            datetimeMgmt = new DATETIME_MGMT();
            networkMgmt = new NETWORK_MGMT();
            fspMgmt = new FSP_MGMT();

            InitializeCOMPort(comPortIndex);
        }

        public void CloseSerialConnection()
        {
            if (serialCOMPort.IsOpen)
            {
                serialCOMPort.Close();
            }
        }

        public void TestExecutionPrompt()
        {
            string command = "";
            do
            {
                command = Console.ReadLine();

                switch (command)
                {
                    case "poweron":
                        PowerManagementLib.DEVICE_POWERON();
                        break;
                    case "poweroff":
                        PowerManagementLib.DEVICE_POWEROFF();
                        break;
                    case "reboot":
                        PowerManagementLib.DEVICE_REBOOT();
                        break;
                    case "setdate":
                        Console.WriteLine("Enter new date (DD-MM-YYYY): ");
                        string newDate = Console.ReadLine();
                        DatetimeMgmt.DEVICE_SET_DATE(newDate);
                        break;
                    case "settime":
                        Console.WriteLine("Enter new time (HH:MM:SS): ");
                        string newTime = Console.ReadLine();
                        DatetimeMgmt.DEVICE_SET_TIME(newTime);
                        break;
                    case "changepassword":
                        Console.WriteLine("Enter new password: ");
                        string newPassword = Console.ReadLine();
                        AuthManagementLib.ASMIChangePassword(newPassword);
                        break;
                    case "getSuccessfulLoginAudits":
                        List<AUTH_MGMT.STRUCT_LOGIN_AUDIT_INFO> successfullLoginAuditsList = AuthManagementLib.getASMILoginAudits(AUTH_MGMT.ENUM_LOGIN_AUDIT_TYPE.SuccessfullLogin);
                        break;
                    case "getFailedLoginAudits":
                        List<AUTH_MGMT.STRUCT_LOGIN_AUDIT_INFO> failedLoginAuditsList = AuthManagementLib.getASMILoginAudits(AUTH_MGMT.ENUM_LOGIN_AUDIT_TYPE.FailedLogin);
                        break;
                    case "getNetworkInterfaceInfo":
                        Console.WriteLine("Enter Ethernet index (0 for eth0, 1 for eth1, etc.): ");

                        int ethIndex = 0;
                        if (!int.TryParse(Console.ReadLine(), out ethIndex))
                        {
                            Console.WriteLine("Invalid index. Please enter a valid integer.");
                        }
                        STRUCT_NETWORK_INTERFACE NetworkInterfaceConfigs = NetworkMgmt.GetNetworkInterfaceConfigs(ethIndex);
                        /*Console.WriteLine($"Network Interface Configs for eth{ethIndex}:");
                        Console.WriteLine($"MAC Address: {NetworkInterfaceConfigs[0]}");
                        Console.WriteLine($"IP Address: {NetworkInterfaceConfigs[1]}");
                        Console.WriteLine($"IP Address Type: {NetworkInterfaceConfigs[2]}");
                        if (NetworkInterfaceConfigs[2] == "Dynamic")
                        {
                            Console.WriteLine($"DHCP Server: {NetworkInterfaceConfigs[3]}");
                            Console.WriteLine($"Lease Time: {NetworkInterfaceConfigs[4]}");
                        }
                        else if (NetworkInterfaceConfigs[2] == "Static")
                        {
                            Console.WriteLine($"Hostname: {NetworkInterfaceConfigs[3]}");
                            Console.WriteLine($"Domain Name: {NetworkInterfaceConfigs[4]}");
                            Console.WriteLine($"Subnet Mask: {NetworkInterfaceConfigs[5]}");
                            Console.WriteLine($"Default Gateway: {NetworkInterfaceConfigs[6]}");
                            Console.WriteLine($"1st DNS IP ADDRESS: {NetworkInterfaceConfigs[7]}");
                            Console.WriteLine($"2nd DNS IP ADDRESS: {NetworkInterfaceConfigs[8]}");
                            Console.WriteLine($"3rd DNS IP ADDRESS: {NetworkInterfaceConfigs[9]}");
                        }*/

                        break;
                    case "resetNetworkSettings":
                        NetworkMgmt.ResetNetworkConfigs();
                        break;
                    case "getAllowedIPAddresses":
                        List<string> allowedIPs = NetworkMgmt.GetAllowedIPAddresses();
                        Console.WriteLine("Allowed IP Addresses:");
                        for (int i = 0; i < allowedIPs.Count; i++)
                        {
                            Console.WriteLine(allowedIPs[i]);
                        }
                        break;
                    case "getDeniedIPAddresses":
                        List<string> deniedIPs = NetworkMgmt.GetDeniedIPAddresses();
                        Console.WriteLine("Denied IP Addresses:");
                        for (int i = 0; i < deniedIPs.Count; i++)
                        {
                            Console.WriteLine(deniedIPs[i]);
                        }
                        break;
                    case "editAllowedIPAddresses":
                        List<string> allowedIpsList = new List<string>();
                        List<int> allowedIpsIndexesList = new List<int>();

                        Console.WriteLine("Enter address index: ");
                        int index;
                        int.TryParse(Console.ReadLine(), out index);
                        allowedIpsIndexesList.Add(index);

                        Console.WriteLine("Enter new allowed IP address: ");
                        allowedIpsList.Add(Console.ReadLine());

                        NetworkMgmt.EditAllowedIPAddresses(allowedIpsIndexesList, allowedIpsList);
                        break;
                    case "editDeniedIPAddresses":
                        List<string> deniedIpsList = new List<string>();
                        List<int> deniedIpsIndexesList = new List<int>();

                        Console.WriteLine("Enter address index: ");
                        int index2;
                        int.TryParse(Console.ReadLine(), out index2);
                        deniedIpsIndexesList.Add(index2);

                        Console.WriteLine("Enter new denied IP address: ");
                        deniedIpsList.Add(Console.ReadLine());

                        NetworkMgmt.EditDeniedIPAddresses(deniedIpsIndexesList, deniedIpsList);
                        break;
                    case "editNetworkInterfaceConfigs":
                        Console.WriteLine("Enter Ethernet index (0 for eth0, 1 for eth1, etc.): ");
                        int ethIndexToEdit = 0;
                        if (!int.TryParse(Console.ReadLine(), out ethIndexToEdit))
                        {
                            Console.WriteLine("Invalid index. Please enter a valid integer.");
                        }

                        List<ENUM_STATIC_NETWORK_PROPERTIES> propertiesToEdit = new List<ENUM_STATIC_NETWORK_PROPERTIES>() { ENUM_STATIC_NETWORK_PROPERTIES.Hostname };
                        List<string> newValues = new List<string>() { "POWER9111285ETH1" };
                        NetworkMgmt.EditNetworkInterfaceConfigs(ethIndexToEdit, propertiesToEdit, newValues, "Static");
                        break;
                    case "resetFSP":
                        FspMgmt.FSP_RESET();
                        break;
                    case "getFSPErrorLogs":
                        List<FSP_MGMT.STRUCT_FSP_ERROR_LOG_INFO> ErrorLogsInfo = FspMgmt.getFSPErrorLogs();
                        break;
                    case "TurnOffAttentionLED":
                        PowerManagementLib.DEVICE_ATENTION_LED_OFF();
                        break;
                    case "getMachineInfo":
                        FSP_MGMT.STRUCT_MACHINE_INFO machine_info = fspMgmt.GetMachineInfo();
                        break;
                    case "exit":
                        Console.WriteLine("Exiting...");
                        break;
                }
            }
            while (command != "exit");
        }

        static private void InitializeCOMPort(int comPortIndex)
        {
            serialCOMPort = new SerialPort($"COM{comPortIndex}", 19200, Parity.None, 8, StopBits.One);
            serialCOMPort.Handshake = Handshake.None;
            serialCOMPort.NewLine = "\n";
            serialCOMPort.DataReceived += SerialDataReceived;
            serialCOMPort.Open();
        }

        static private void ChangeCOMPort(string portName)
        {
            if (serialCOMPort.IsOpen)
            {
                serialCOMPort.Close();
            }
            serialCOMPort.PortName = portName;
            serialCOMPort.Open();
        }

        static public void SendCommand(string _command, short _awaitTime)
        {
            serialCOMPort.WriteLine(_command);
            Thread.Sleep(_awaitTime);
        }

        static public string GetReceivedData()
        {
            return serialCOMPort.ReadExisting();
        }

        static public void PrintReceivedData()
        {
            string receivedData = serialCOMPort.ReadExisting();
            Console.WriteLine(receivedData);
        }

        static private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e) {}

        /// <summary>
        /// POWER_ENV: Static method to check if the serial session is open.
        /// </summary>
        static public void CheckSerialSessionOpen()
        {
            if (!serialCOMPort.IsOpen)
            {
                Console.WriteLine("Serial port is not open. Please open the port before sending commands.");
                return;
            }
        }
    }
}