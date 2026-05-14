namespace POWER_ENV.GLOBAL.NETWORK
{
    public enum ENUM_STATIC_NETWORK_PROPERTIES
    {
        Hostname,
        DomainName,
        IPAddress,
        SubnetMask,
        DefaultGateway,
        IP1DNSSERVER,
        IP2DNSSERVER,
        IP3DNSSERVER,
        IPAddressType,
        ETHType
    }

    public struct STRUCT_NETWORK_INTERFACE
    {
        public string MACAddress { get; set; }
        public string IPAddress { get; set; }
        public string IPAddressType { get; set; }
        public string Hostname { get; set; }
        public string DomainName { get; set; }
        public string SubnetMask { get; set; }
        public string DefaultGateway { get; set; }
        public string IP1DNSSERVER { get; set; }
        public string IP2DNSSERVER { get; set; }
        public string IP3DNSSERVER { get; set; }
    }

    internal interface INETWORK_MANAGEMENT
    {
        /// <summary>
        /// METHOD TO GET CONFIGURATIONS FROM A SPECIFIC NETWORK INTERFACE CARD IN THE POWER SERVER ENVIRONMENT.
        /// </summary>
        /// <param name="eth_index">NIC INDEX (NUMBER)</param>
        /// <returns></returns>
        STRUCT_NETWORK_INTERFACE GetNetworkInterfaceConfigs(int eth_index);

        List<string> FetchDynamicNetworkInterfaceConfigs(string _receivedData);

        List<string> FetchStaticNetworkInterfaceConfigs(string _receivedData);

        /// <summary>
        /// METHOD TO RESET ALL NETWORK CONFIGURATIONS IN THE POWER SERVER ENVIRONMENT.
        /// </summary>
        void ResetNetworkConfigs();

        void EditNetworkInterfaceConfigs(int _eth_index, List<ENUM_STATIC_NETWORK_PROPERTIES> _changedProperties, List<string> _newValues, string _IPAddressType);

        void EditDynamicNetworkInterfaceConfigs(List<ENUM_STATIC_NETWORK_PROPERTIES> _changedProperties, List<string> _newValues);

        void EditStaticNetworkInterfaceConfigs(List<ENUM_STATIC_NETWORK_PROPERTIES> _changedProperties, List<string> _newValues);

        /// <summary>
        /// METHOD TO GET THE LIST OF IP ADDRESSES THAT HAVE ACCESS RIGHTS TO ASMI WEB PORTAL.
        /// </summary>
        /// <returns>A STRING LIST WITH ASMI ALLOWED IP ADDRESSES.</returns>
        List<string> GetAllowedIPAddresses();

        /// <summary>
        /// METHOD TO GET THE LIST OF IP ADDRESSES THAT AREN'T ALLOWED TO ACCESS THE ASMI WEB PORTAL.
        /// </summary>
        /// <returns>A STRING LIST WITH ASMI DENIED IP ADDRESSES.</returns>
        List<string> GetDeniedIPAddresses();

        /// <summary>
        /// METHOD TO EDIT THE LIST OF ALLOWED IP ADDRESSES IN THE ASMI WEB PORTAL.
        /// </summary>
        void EditAllowedIPAddresses(List<int> indexes, List<string> allowedIPs);

        /// <summary>
        /// METHOD TO EDIT THE LIST OF DENIED IP ADDRESSES IN THE ASMI WEB PORTAL.
        /// </summary>
        void EditDeniedIPAddresses(List<int> indexes, List<string> deniedIPs);
    }
}