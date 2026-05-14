using XTELNET;

namespace POWER_ENV
{
    public class POWER_MGMT
    {
        public void DEVICE_POWERON()
        {
            POWERENV.CheckSerialSessionOpen();

            POWERENV.AuthManagementLib.ASMIAuthenticate();
            
            POWERENV.SendCommand("1", 200);
            POWERENV.SendCommand("1", 200);
            POWERENV.SendCommand("8", 200);
            POWERENV.SendCommand("\n", 500);

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        public void DEVICE_POWEROFF()
        {
            POWERENV.CheckSerialSessionOpen();

            XTELNET_COMMAND XTelnet_Instance = new XTELNET.XTELNET_COMMAND(100, POWERENV.AuthManagementLib.OS_INFO.os_ip_address, 1, 1000);
            POWERENV.AuthManagementLib.OSAuthenticate(XTelnet_Instance);
            List<string> commands = new List<string> { "shutdown -F" };
            XTelnet_Instance.ExecuteCommands(commands);
            XTelnet_Instance.Logout();
        }

        public void DEVICE_REBOOT()
        {
            POWERENV.CheckSerialSessionOpen();

            XTELNET_COMMAND XTelnet_Instance = new XTELNET.XTELNET_COMMAND(100, POWERENV.AuthManagementLib.OS_INFO.os_ip_address, 1, 1000);
            POWERENV.AuthManagementLib.OSAuthenticate(XTelnet_Instance);
            List<string> commands = new List<string> { "shutdown -F -r" };
            XTelnet_Instance.ExecuteCommands(commands);
            XTelnet_Instance.Logout();
        }

        public void DEVICE_ATENTION_LED_OFF()
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();
            
            POWERENV.SendCommand("4", 500);
            POWERENV.SendCommand("13", 200);
            POWERENV.SendCommand("1", 200);

            if (!POWERENV.GetReceivedData().Contains("System attention indicator is already off."))
            {
                POWERENV.SendCommand("1", 200);
                POWERENV.SendCommand("\n", 500);
                Console.WriteLine("Attention LED is now off.");
            }
            else Console.WriteLine("Attention LED is already off.");

            POWERENV.SendCommand("\n", 500);
            POWERENV.GetReceivedData();
            POWERENV.AuthManagementLib.ASMISignOut();
        }
    }
}