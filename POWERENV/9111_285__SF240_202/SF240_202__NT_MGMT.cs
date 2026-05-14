using POWER_ENV.GLOBAL.NETWORK;

namespace POWER_ENV._9111_285__SF240_202
{
    public class NETWORK_MGMT : INETWORK_MANAGEMENT
    {
        #region NETWORK CONFIGURATION
        #region GET

        /// <summary>
        /// METHOD TO GET CONFIGURATIONS FROM A SPECIFIC NETWORK INTERFACE CARD IN THE POWER SERVER ENVIRONMENT.
        /// </summary>
        /// <param name="eth_index">NIC INDEX (NUMBER)</param>
        /// <returns></returns>
        public STRUCT_NETWORK_INTERFACE GetNetworkInterfaceConfigs(int eth_index)
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("5", 200);
            POWERENV.SendCommand("1", 300);
            POWERENV.GetReceivedData();
            POWERENV.SendCommand($"{eth_index + 1}", 1000);
            string receivedData = POWERENV.GetReceivedData();

            STRUCT_NETWORK_INTERFACE networkInterfaceInfo = new STRUCT_NETWORK_INTERFACE();

            networkInterfaceInfo.MACAddress = GLOBAL_METHODS.GetPropertyValue("MAC address", receivedData, ':');
            networkInterfaceInfo.IPAddress = GLOBAL_METHODS.GetPropertyValue("IP address", receivedData, ':');
            networkInterfaceInfo.IPAddressType = GLOBAL_METHODS.GetPropertyValue("Currently", receivedData, ':');

            List<string> detailedConfigs = new List<string>();

            switch (networkInterfaceInfo.IPAddressType)
            {
                case "Dynamic":
                    detailedConfigs = ((INETWORK_MANAGEMENT)this).FetchDynamicNetworkInterfaceConfigs(receivedData);
                    networkInterfaceInfo.Hostname = detailedConfigs[0];
                    networkInterfaceInfo.DomainName = detailedConfigs[1];
                    break;
                case "Static":
                    detailedConfigs = ((INETWORK_MANAGEMENT)this).FetchStaticNetworkInterfaceConfigs(receivedData);
                    networkInterfaceInfo.Hostname = detailedConfigs[0];
                    networkInterfaceInfo.DomainName = detailedConfigs[1];
                    networkInterfaceInfo.SubnetMask = detailedConfigs[2];
                    networkInterfaceInfo.DefaultGateway = detailedConfigs[3];
                    networkInterfaceInfo.IP1DNSSERVER = detailedConfigs[4];
                    networkInterfaceInfo.IP2DNSSERVER = detailedConfigs[5];
                    networkInterfaceInfo.IP3DNSSERVER = detailedConfigs[6];
                    break;
            }

            POWERENV.AuthManagementLib.ASMISignOut();

            return networkInterfaceInfo;
        }

        List<string> INETWORK_MANAGEMENT.FetchDynamicNetworkInterfaceConfigs(string _receivedData)
        {
            POWERENV.SendCommand("1", 1000);

            _receivedData = POWERENV.GetReceivedData();

            string Hostname = GLOBAL_METHODS.GetPropertyValue("Currently", _receivedData, ':');
            string DomainName = GLOBAL_METHODS.GetPropertyValue("Domain name (Currently", _receivedData, ':');

            List<string> dynamicConfigs = new List<string>() {
                Hostname,
                DomainName
            };

            return dynamicConfigs;
        }

        List<string> INETWORK_MANAGEMENT.FetchStaticNetworkInterfaceConfigs(string _receivedData)
        {
            POWERENV.SendCommand("2", 1000);
            _receivedData = POWERENV.GetReceivedData();

            string Hostname = GLOBAL_METHODS.GetPropertyValue("Host name (Currently", _receivedData, ':');
            if (Hostname != string.Empty) Hostname = Hostname.Substring(0, Hostname.IndexOf(')')).Trim();

            string DomainName = GLOBAL_METHODS.GetPropertyValue("Domain name (Currently", _receivedData, ':');
            if (DomainName != string.Empty) DomainName = DomainName.Substring(0, DomainName.IndexOf(')')).Trim();

            string SubnetMask = GLOBAL_METHODS.GetPropertyValue("Subnet mask (Currently", _receivedData, ':');
            if (SubnetMask != string.Empty) SubnetMask = SubnetMask.Substring(0, SubnetMask.IndexOf(')')).Trim();

            string DefaultGateway = GLOBAL_METHODS.GetPropertyValue("Default gateway (Currently", _receivedData, ':');
            if (DefaultGateway != string.Empty) DefaultGateway = DefaultGateway.Substring(0, DefaultGateway.IndexOf(')')).Trim();

            string IP1DNSSERVER = GLOBAL_METHODS.GetPropertyValue("IP address of first DNS server (Currently", _receivedData, ':');
            if (IP1DNSSERVER != string.Empty) IP1DNSSERVER = IP1DNSSERVER.Substring(0, IP1DNSSERVER.IndexOf(')')).Trim();

            string IP2DNSSERVER = GLOBAL_METHODS.GetPropertyValue("IP address of second DNS server (Currently", _receivedData, ':');
            if (IP2DNSSERVER != string.Empty) IP2DNSSERVER = IP2DNSSERVER.Substring(0, IP2DNSSERVER.IndexOf(')')).Trim();

            string IP3DNSSERVER = GLOBAL_METHODS.GetPropertyValue("IP address of third DNS server (Currently", _receivedData, ':');
            if (IP3DNSSERVER != string.Empty) IP3DNSSERVER = IP3DNSSERVER.Substring(0, IP3DNSSERVER.IndexOf(')')).Trim();

            List<string> staticConfigs = new List<string>
            {
                Hostname,
                DomainName,
                SubnetMask,
                DefaultGateway,
                IP1DNSSERVER,
                IP2DNSSERVER,
                IP3DNSSERVER
            };

            return staticConfigs;
        }

        #endregion
        #region SET

        /// <summary>
        /// METHOD TO RESET ALL NETWORK CONFIGURATIONS IN THE POWER SERVER ENVIRONMENT.
        /// </summary>
        public void ResetNetworkConfigs()
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("5", 200);
            POWERENV.SendCommand("1", 1000);
            POWERENV.SendCommand("3", 1000);
            POWERENV.SendCommand("1", 1000);
            POWERENV.SendCommand("\n", 200);
            POWERENV.SendCommand("\n", 200);

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        public void EditNetworkInterfaceConfigs(int _eth_index, List<ENUM_STATIC_NETWORK_PROPERTIES> _changedProperties, List<string> _newValues, string _IPAddressType)
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("5", 200);
            POWERENV.SendCommand("1", 700);
            POWERENV.GetReceivedData(); //Run this function to clear the buffer
            POWERENV.SendCommand($"{_eth_index + 1}", 1000);

            switch (_IPAddressType)
            {
                case "Dynamic":
                    ((INETWORK_MANAGEMENT)this).EditDynamicNetworkInterfaceConfigs(_changedProperties, _newValues);
                    break;
                case "Static":
                    ((INETWORK_MANAGEMENT)this).EditStaticNetworkInterfaceConfigs(_changedProperties, _newValues);
                    break;
            }

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        void INETWORK_MANAGEMENT.EditDynamicNetworkInterfaceConfigs(List<ENUM_STATIC_NETWORK_PROPERTIES> _changedProperties, List<string> _newValues)
        {
            POWERENV.SendCommand("1", 500);
            for (int i = 0; i < _changedProperties.Count; i++)
            {
                int option = 0;
                switch (_changedProperties[i])
                {
                    case ENUM_STATIC_NETWORK_PROPERTIES.Hostname:
                        option = 1;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.DomainName:
                        option = 2;
                        break;
                }

                POWERENV.SendCommand($"{option}", 200);
                POWERENV.SendCommand(_newValues[i], 200);
                POWERENV.SendCommand("\n", 500);
            }

            POWERENV.SendCommand("3", 200);
            POWERENV.SendCommand("1", 200);
            POWERENV.SendCommand("\n", 9000);
        }

        void INETWORK_MANAGEMENT.EditStaticNetworkInterfaceConfigs(List<ENUM_STATIC_NETWORK_PROPERTIES> _changedProperties, List<string> _newValues)
        {
            POWERENV.SendCommand("2", 500);
            for (int i = 0; i < _changedProperties.Count; i++)
            {
                int option = 0;
                switch (_changedProperties[i])
                {
                    case ENUM_STATIC_NETWORK_PROPERTIES.Hostname:
                        option = 1;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.DomainName:
                        option = 2;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.IPAddress:
                        option = 3;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.SubnetMask:
                        option = 4;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.DefaultGateway:
                        option = 5;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.IP1DNSSERVER:
                        option = 6;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.IP2DNSSERVER:
                        option = 7;
                        break;
                    case ENUM_STATIC_NETWORK_PROPERTIES.IP3DNSSERVER:
                        option = 8;
                        break;
                    default:
                        continue;
                }

                POWERENV.SendCommand($"{option}", 200);
                POWERENV.SendCommand(_newValues[i], 200);
                POWERENV.SendCommand("\n", 500);
            }

            POWERENV.SendCommand("9", 200);
            POWERENV.SendCommand("1", 200);
            POWERENV.SendCommand("\n", 5000);
        }

        #endregion
        #endregion

        #region NETWORK ACCESS
        #region GET

        /// <summary>
        /// METHOD TO GET THE LIST OF IP ADDRESSES THAT HAVE ACCESS RIGHTS TO ASMI WEB PORTAL.
        /// </summary>
        /// <returns>A STRING LIST WITH ASMI ALLOWED IP ADDRESSES.</returns>
        public List<string> GetAllowedIPAddresses()
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("5", 500);
            POWERENV.SendCommand("2", 500);
            POWERENV.GetReceivedData();
            POWERENV.SendCommand("1", 1000);
            string receivedData = POWERENV.GetReceivedData();

            List<string> allowedIPs = new List<string>();
            for (int i = 1; i <= 16; i++)
            {
                string tmp_ip = GLOBAL_METHODS.GetPropertyValue($"{i}. ", receivedData, '.');
                string ip = "";
                for (int j = 0; j < tmp_ip.Length; j++)
                {
                    if (tmp_ip[j] != '\r' && tmp_ip[j] != '\n')
                    {
                        ip += tmp_ip[j];
                    }
                }
                allowedIPs.Add(ip);
            }

            POWERENV.AuthManagementLib.ASMISignOut();

            return allowedIPs;
        }

        /// <summary>
        /// METHOD TO GET THE LIST OF IP ADDRESSES THAT AREN'T ALLOWED TO ACCESS THE ASMI WEB PORTAL.
        /// </summary>
        /// <returns>A STRING LIST WITH ASMI DENIED IP ADDRESSES.</returns>
        public List<string> GetDeniedIPAddresses()
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("5", 500);
            POWERENV.SendCommand("2", 500);
            POWERENV.GetReceivedData();
            POWERENV.SendCommand("2", 1000);
            string receivedData = POWERENV.GetReceivedData();

            List<string> allowedIPs = new List<string>();
            for (int i = 1; i <= 16; i++)
            {
                string tmp_ip = GLOBAL_METHODS.GetPropertyValue($"{i}. ", receivedData, '.');
                string ip = "";
                for (int j = 0; j < tmp_ip.Length; j++)
                {
                    if (tmp_ip[j] != '\r' && tmp_ip[j] != '\n')
                    {
                        ip += tmp_ip[j];
                    }
                }
                allowedIPs.Add(ip);
            }

            POWERENV.AuthManagementLib.ASMISignOut();

            return allowedIPs;
        }

        #endregion
        #region SET

        /// <summary>
        /// METHOD TO EDIT THE LIST OF ALLOWED IP ADDRESSES IN THE ASMI WEB PORTAL.
        /// </summary>
        public void EditAllowedIPAddresses(List<int> indexes, List<string> allowedIPs)
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("5", 500);
            POWERENV.SendCommand("2", 500);
            POWERENV.GetReceivedData();
            POWERENV.SendCommand("1", 700);
            for (int i = 0; i < allowedIPs.Count; i++)
            {
                POWERENV.SendCommand($"{indexes[i]}", 200);
                POWERENV.SendCommand(allowedIPs[i], 300);
                POWERENV.SendCommand("\n", 500);
            }

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        /// <summary>
        /// METHOD TO EDIT THE LIST OF DENIED IP ADDRESSES IN THE ASMI WEB PORTAL.
        /// </summary>
        public void EditDeniedIPAddresses(List<int> indexes, List<string> deniedIPs)
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("5", 500);
            POWERENV.SendCommand("2", 500);
            POWERENV.GetReceivedData();
            POWERENV.SendCommand("2", 700);
            for (int i = 0; i < deniedIPs.Count; i++)
            {
                POWERENV.SendCommand($"{indexes[i]}", 200);
                POWERENV.SendCommand(deniedIPs[i], 300);
                POWERENV.SendCommand("\n", 500);
            }

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        #endregion
        #endregion
    }
}