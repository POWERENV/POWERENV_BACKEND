using Microsoft.AspNetCore.Mvc;
using POWERENV_PGSQL_DB_HANDLER;
using POWER_ENV;
using POWER_ENV.GLOBAL.NETWORK;
using static POWERENV_BACKEND_API.Program;
using static POWERENV_PGSQL_DB_HANDLER.PSYSTEMS_HARDWARE_DATA_HANDLING;

namespace POWERENV_BACKEND_API.Controllers
{
    [ApiController]
    [Route("psystems/network")]
    public class MACHINE_NETWORK_CONTROLLER : Controller
    {
        private POWERENV POWERENVEngine;
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;

        /// <summary>
        /// Controler Class Constructor Method
        /// </summary>
        public MACHINE_NETWORK_CONTROLLER()
        {
            POWERENVEngine = new POWERENV();
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING(AppContext.BaseDirectory);
        }

        [HttpGet("{_systemID}/getNetworkIntInfo/{_eth_index}")]
        public IActionResult SysGetNetworkInfo([FromRoute] int _systemID, [FromRoute] int _eth_index)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                POWERENVEngine.Main(_systemID);
                STRUCT_NETWORK_INTERFACE networkInfo = POWERENV.NetworkMgmt.GetNetworkInterfaceConfigs(_eth_index);
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "Network information retrieved successfully.";
                response.packetData = networkInfo; // Assuming networkInfo is serializable
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/resetNetworkConfigs")]
        public IActionResult SysResetNetworkConfigs([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                POWERENVEngine.Main(_systemID);
                POWERENV.NetworkMgmt.ResetNetworkConfigs();
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "Network information retrieved successfully.";
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost("{_systemID}/editNetworkInterfaceConfigs/bulkData")]
        public IActionResult SysChangeNetworkConfigs([FromBody] STRUCT_NETWORK_INTERFACE_CONFIG_PACKED_DATA networkInterfaceConfigChangeData, [FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWERENV.NetworkMgmt.EditNetworkInterfaceConfigs(networkInterfaceConfigChangeData.eth_index, networkInterfaceConfigChangeData.changedProperties, networkInterfaceConfigChangeData.newValues, networkInterfaceConfigChangeData.IPAddressType);
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                List<STRUCT_PNODE_NIC_INFO> pnodeNICS = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeNICsInfo(_systemID);
                STRUCT_PNODE_NIC_INFO newNicInfo = new STRUCT_PNODE_NIC_INFO();

                for (int i = 0; i < pnodeNICS.Count; i++)
                {
                    if (pnodeNICS[i].pnode_nic_name == $"eth{networkInterfaceConfigChangeData.eth_index}")
                    {
                        newNicInfo = new STRUCT_PNODE_NIC_INFO()
                        {
                            pnode_nic_id = pnodeNICS[i].pnode_nic_id,
                            pnode_nic_name = pnodeNICS[i].pnode_nic_name,
                            pnode_nic_mac_address = pnodeNICS[i].pnode_nic_mac_address,
                            pnode_nic_hostname = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.Hostname) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.Hostname)] : pnodeNICS[i].pnode_nic_hostname,
                            pnode_nic_ip_address = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.IPAddress) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.IPAddress)] : pnodeNICS[i].pnode_nic_ip_address,
                            pnode_nic_subnet_mask = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.SubnetMask) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.SubnetMask)] : pnodeNICS[i].pnode_nic_subnet_mask,
                            pnode_nic_default_gateway = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.DefaultGateway) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.DefaultGateway)] : pnodeNICS[i].pnode_nic_default_gateway,
                            pnode_nic_domain_name = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.DomainName) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.DomainName)] : pnodeNICS[i].pnode_nic_domain_name,
                            pnode_nic_first_dns_ip_address = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.IP1DNSSERVER) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.IP1DNSSERVER)] : pnodeNICS[i].pnode_nic_first_dns_ip_address,
                            pnode_nic_second_dns_ip_address = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.IP2DNSSERVER) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.IP2DNSSERVER)] : pnodeNICS[i].pnode_nic_second_dns_ip_address,
                            pnode_nic_third_dns_ip_address = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.IP3DNSSERVER) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.IP3DNSSERVER)] : pnodeNICS[i].pnode_nic_third_dns_ip_address,
                            pnode_nic_ip_address_type = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.IPAddressType) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.IPAddressType)] : pnodeNICS[i].pnode_nic_ip_address_type,
                            pnode_nic_type = networkInterfaceConfigChangeData.changedProperties.Contains<ENUM_STATIC_NETWORK_PROPERTIES>(ENUM_STATIC_NETWORK_PROPERTIES.ETHType) ? networkInterfaceConfigChangeData.newValues[networkInterfaceConfigChangeData.changedProperties.IndexOf(ENUM_STATIC_NETWORK_PROPERTIES.ETHType)] : pnodeNICS[i].pnode_nic_type,
                            pnode_id = _systemID
                        };
                        break;
                    }
                }

                int pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.updatePNodeNICsInfo(newNicInfo);

                STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                {
                    operationCatName = "NETWORK",
                    operationSourcePNodeID = _systemID,
                    operationAction = $"NodeChangeNIC{networkInterfaceConfigChangeData.eth_index}Configs",
                    operationCompletionStatus = pnodeActivenessStateUpdateRowsAffected > 0 ? "SUCCESS" : "FAILURE",
                    operationSourceUserName = "Alice Wonder"
                };

                int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                response = checkDBInsertionSuccessState(new int[] { pnodeActivenessStateUpdateRowsAffected, pnodePowerOnOperationRegistryRowsAffected }, new string[] { "Network information", "Network information" });
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/getAllowedIPAddresses")]
        public IActionResult SysGetAllowedIPAddresses([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            List<string> AllowedIPs = new List<string>();
            try
            {
                POWERENVEngine.Main(_systemID);
                AllowedIPs = POWERENV.NetworkMgmt.GetAllowedIPAddresses();
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "Allowed IP addresses list retrieved successfully.";
                response.packetData = AllowedIPs; // Echo back the sent data
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpGet("{_systemID}/getDeniedIPAddresses")]
        public IActionResult SysGetDeniedIPAddresses([FromRoute] int _systemID)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            List<string> DeniedIPs = new List<string>();
            try
            {
                POWERENVEngine.Main(_systemID);
                DeniedIPs = POWERENV.NetworkMgmt.GetDeniedIPAddresses();
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();
                response.operationStatus = true;
                response.statusMessage = "Denied IP addresses list retrieved successfully.";
                response.packetData = DeniedIPs; // Echo back the sent data
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost("{_systemID}/editAllowedIPAddresses")]
        public IActionResult SysEditAllowedIPAddresses([FromRoute] int _systemID, [FromBody] STRUCT_IP_ADDRESSES_PERMITIONS_DATA IPAddressesChangeData)
        {
            Program.STRUCT_REQUEST_DATA response = new STRUCT_REQUEST_DATA();
            try
            {
                // Change Data on the Machine
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWERENV.NetworkMgmt.EditAllowedIPAddresses(IPAddressesChangeData.indexes, IPAddressesChangeData.IPAddresses);
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                // Update Data on the Database
                for (int i = 0; i < IPAddressesChangeData.indexes.Count; i++)
                {
                    STRUCT_PNODE_ETH_ACCESS_POLICY_INFO newETHAccessPolicy = new STRUCT_PNODE_ETH_ACCESS_POLICY_INFO()
                    {
                        access_policy_index_id = IPAddressesChangeData.indexes[i],
                        access_policy_pnode_id = _systemID,
                        access_policy_ip_address = IPAddressesChangeData.IPAddresses[i],
                        access_policy_type = "1",
                    };

                    int pnodeActivenessStateUpdateRowsAffected = 0;

                    if (newETHAccessPolicy.access_policy_ip_address != "")
                    {
                        pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.insertPNodeETHAccessPolicy(newETHAccessPolicy);
                    }
                    else
                    {
                        pnodeActivenessStateUpdateRowsAffected = UpdateAccessPoliciesIndexDB(newETHAccessPolicy);
                    }

                    STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                    {
                        operationCatName = "NETWORK",
                        operationSourcePNodeID = _systemID,
                        operationAction = $"NodeEditAllowedIPAddresses",
                        operationCompletionStatus = pnodeActivenessStateUpdateRowsAffected > 0 ? "SUCCESS" : "FAILURE",
                        operationSourceUserName = "Alice Wonder"
                    };

                    int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                    response = checkDBInsertionSuccessState(new int[] { pnodeActivenessStateUpdateRowsAffected, pnodePowerOnOperationRegistryRowsAffected }, new string[] { "Allowed IP addresses", "Allowed IP addresses" });
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        [HttpPost("{_systemID}/editDeniedIPAddresses")]
        public IActionResult SysEditDeniedIPAddresses([FromRoute] int _systemID, [FromBody] STRUCT_IP_ADDRESSES_PERMITIONS_DATA IPAddressesChangeData)
        {
            Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
            try
            {
                int pnodeCOMPortID = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeFullInfo(_systemID).pnodeSerialCOMPortId;
                POWERENVEngine.Main(pnodeCOMPortID);
                POWERENV.NetworkMgmt.EditDeniedIPAddresses(IPAddressesChangeData.indexes, IPAddressesChangeData.IPAddresses);
                Thread.Sleep(2000); // Wait for 5 seconds to ensure the command is processed
                POWERENVEngine.CloseSerialConnection();

                // Update Data on the Database
                for (int i = 0; i < IPAddressesChangeData.indexes.Count; i++)
                {
                    STRUCT_PNODE_ETH_ACCESS_POLICY_INFO newETHAccessPolicy = new STRUCT_PNODE_ETH_ACCESS_POLICY_INFO()
                    {
                        access_policy_index_id = IPAddressesChangeData.indexes[i],
                        access_policy_pnode_id = _systemID,
                        access_policy_ip_address = IPAddressesChangeData.IPAddresses[i],
                        access_policy_type = "2",
                    };

                    int pnodeActivenessStateUpdateRowsAffected = 0;

                    if (newETHAccessPolicy.access_policy_ip_address != "")
                    {
                        pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.insertPNodeETHAccessPolicy(newETHAccessPolicy);
                    }
                    else
                    {
                        pnodeActivenessStateUpdateRowsAffected = UpdateAccessPoliciesIndexDB(newETHAccessPolicy);
                    }

                    STRUCT_PNODES_SINGLE_OPERATION_HISTORY PowerOnOperationData = new STRUCT_PNODES_SINGLE_OPERATION_HISTORY()
                    {
                        operationCatName = "NETWORK",
                        operationSourcePNodeID = _systemID,
                        operationAction = $"NodeEditDeniedIPAddresses",
                        operationCompletionStatus = pnodeActivenessStateUpdateRowsAffected > 0 ? "SUCCESS" : "FAILURE",
                        operationSourceUserName = "Alice Wonder"
                    };

                    int pnodePowerOnOperationRegistryRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.DBInsertPNodeSingleOperation(PowerOnOperationData);

                    response = checkDBInsertionSuccessState(new int[] { pnodeActivenessStateUpdateRowsAffected, pnodePowerOnOperationRegistryRowsAffected }, new string[] { "Denied IP addresses", "Denied IP addresses" });
                }
            }
            catch (Exception ex)
            {
                response.operationStatus = false;
                response.statusMessage = ex.Message;
            }

            return Ok(response);
        }

        #region Appendix Methods

        private STRUCT_REQUEST_DATA checkDBInsertionSuccessState(int[] rowsAffected, string[] messagePrefix)
        {
            List<Program.STRUCT_REQUEST_DATA> responsesList = new List<STRUCT_REQUEST_DATA>();

            for(int i = 0; i < rowsAffected.Length; i++)
            {
                Program.STRUCT_REQUEST_DATA response = new Program.STRUCT_REQUEST_DATA();
                if (rowsAffected[i] >= 1)
                {
                    response.operationStatus = true;
                    response.statusMessage = $"{messagePrefix[i]} edited successfully.";
                    responsesList.Add(response);
                }
                else
                {
                    response.operationStatus = false;
                    response.statusMessage = $"{messagePrefix[i]} edited on the machine, but the database was not updated!";
                    responsesList.Add(response);
                    break;
                }
            }

            return responsesList.Last();
        }

        private int UpdateAccessPoliciesIndexDB(STRUCT_PNODE_ETH_ACCESS_POLICY_INFO newETHAccessPolicy)
        {
            int pnodeActivenessStateUpdateRowsAffected = DB_HANDLER.HARDWARE_DATA_HANDLER.deletePNodeETHAccessPolicy(newETHAccessPolicy);
            List<STRUCT_PNODE_ETH_ACCESS_POLICY_INFO> accessPolicies = DB_HANDLER.HARDWARE_DATA_HANDLER.DBGetPNodeETHAccessPolicies(newETHAccessPolicy.access_policy_pnode_id);

            for (int j = 0; j < accessPolicies.Count; j++)
            {
                if (accessPolicies[j].access_policy_index_id > newETHAccessPolicy.access_policy_index_id && accessPolicies[j].access_policy_type == "ALLOW")
                {
                    STRUCT_PNODE_ETH_ACCESS_POLICY_INFO updatedPolicy = new STRUCT_PNODE_ETH_ACCESS_POLICY_INFO()
                    {
                        access_policy_id = accessPolicies[j].access_policy_id,
                        access_policy_index_id = accessPolicies[j].access_policy_index_id - 1,
                        access_policy_ip_address = accessPolicies[j].access_policy_ip_address,
                        access_policy_type = "1",
                    };
                    DB_HANDLER.HARDWARE_DATA_HANDLER.updatePNodeETHAccessPolicies(updatedPolicy);
                }
            }

            return pnodeActivenessStateUpdateRowsAffected;
        }

        #endregion
    }
}