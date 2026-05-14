using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Linq;

namespace POWER_ENV
{
    public class FSP_MGMT
    {
        public struct STRUCT_FSP_ERROR_LOG_FRU_INFO
        {
            public string Priority { get; set; }
            public string LocationCode { get; set; }
            public string PartNumber { get; set; }
            public string CCIN { get; set; }
            public string SerialNumber { get; set; }
        }

        public struct STRUCT_FSP_ERROR_LOG_INFO
        {
            // BASIC INFO
            public string ErrorLogID { get; set; }
            public string LogDate { get; set; }
            public string LogTime { get; set; }
            public string DriverName { get; set; }
            public string Subsystem { get; set; }
            public string EventSeverity { get; set; }
            public List<string> ActionFlags { get; set; }
            public string ActionStatus { get; set; }
            public string ReferenceCode { get; set; } //Primary System Reference Code
            public List<STRUCT_FSP_ERROR_LOG_FRU_INFO> NormalHardwareFRU { get; set; } // Normal Hardware FRU
            public string RawData { get; set; } // Raw data (for detailed report visualization)
        }

        public struct STRUCT_MACHINE_INFO
        {
            public string MachineTypeModel { get; set; }
            public string SerialNumber { get; set; }
            public string SystemName { get; set; }
            public string ASMIVersion { get; set; }
        }

        /// <summary>
        /// METHOD TO RESTART (RESET) THE FSP (FIRMWARE SERVICE PROCESSOR) ON THE POWER ENVIRONMENT.
        /// </summary>
        public void FSP_RESET()
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("2", 200);
            POWERENV.SendCommand("10", 200);
            POWERENV.SendCommand("1", 200);
            POWERENV.SendCommand("\n", 500);

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        /// <summary>
        /// METHOD TO GET FSP ERROR LOGS FROM THE POWER ENVIRONMENT.
        /// </summary>
        /// <returns>THE ERROR INFO, STRUCTURED IN A SPECIALIZED DATA STRUCTURE.</returns>
        public List<STRUCT_FSP_ERROR_LOG_INFO> getFSPErrorLogs()
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("2", 200);
            POWERENV.SendCommand("1", 200);

            if(!POWERENV.GetReceivedData().Contains("No error/event logs to display."))
            {
                POWERENV.SendCommand("1", 200);
                POWERENV.GetReceivedData(); //Clear the buffer
                POWERENV.SendCommand("1", 1000);

                string receivedData = POWERENV.GetReceivedData();
                string[] lines = receivedData.Split("\n", StringSplitOptions.None);
                List<STRUCT_FSP_ERROR_LOG_INFO> errorLogsInfo = new List<STRUCT_FSP_ERROR_LOG_INFO>();

                int i = 4;
                while (!lines[i].Contains("98. Return to previous menu")) // Loop to read all error logs listed in ASMI console
                {
                    string errorIndex = lines[i].Substring(1, 2).Trim();
                    POWERENV.GetReceivedData(); //Clear the buffer
                    POWERENV.SendCommand(errorIndex, 1200);
                    string errorRawData = POWERENV.GetReceivedData();
                    if(errorIndex == "97")
                    {
                        errorIndex = "1";
                        POWERENV.SendCommand(errorIndex, 1200);
                        errorRawData = POWERENV.GetReceivedData();
                    }
                    while (!errorRawData.Contains("|-----------------------------------")) // Keep reading more error data until the end of the log entry
                    {
                        POWERENV.SendCommand("\n", 1200);
                        errorRawData += POWERENV.GetReceivedData();
                    }

                    int firstUsefulIndex = errorRawData.IndexOf("|                        Platform Event Log");
                    errorRawData = errorRawData.Substring(firstUsefulIndex, errorRawData.Length - 1 - firstUsefulIndex);

                    STRUCT_FSP_ERROR_LOG_INFO fspErrorLogInfo = ExtractFSPErrorLogInfo(errorRawData);

                    errorLogsInfo.Add(fspErrorLogInfo);

                    Console.WriteLine(errorRawData);
                    POWERENV.GetReceivedData(); //Clear the buffer
                    POWERENV.SendCommand("\n", 200);
                    POWERENV.SendCommand("1", 200);
                    POWERENV.SendCommand("1", 700);
                    i++;
                }

                POWERENV.AuthManagementLib.ASMISignOut();

                return errorLogsInfo;
            }
            else
            {
                POWERENV.SendCommand("\n", 200); // Exit from empty error logs warning
                POWERENV.AuthManagementLib.ASMISignOut();
                Console.WriteLine("No FSP error logs available.");
                return new List<STRUCT_FSP_ERROR_LOG_INFO>(); // Return an empty list if no error logs are available
            }
        }

        /// <summary>
        /// SUBMETHOD TO EXTRACT ALL IMPORTANT FSP ERROR LOG INFO FROM THE RAW DATA, PROVIDED BY THE MAIN METHOD (getFSPErrorLogs()) METHOD, SAVING AND ORGANIZING IN A SPECIALIZED DATA STRUCTURE.
        /// </summary>
        /// <param name="_errorRawData">RAW DATA RETURNED BY THE MAIN METHOD (getFSPErrorLogs()).</param>
        /// <returns>A STRUCT (STRUCT_FSP_ERROR_LOG_INFO) OBJECT WITH ALL THE COLLECTED DATA PACKAGED FOR FURTHER USE.</returns>
        static private STRUCT_FSP_ERROR_LOG_INFO ExtractFSPErrorLogInfo(string _errorRawData)
        {
            string[] first2Lines = new string[] { " ", " " };
            string[] temp = _errorRawData.Split("\n", StringSplitOptions.None);
            string[] errorRawDataRows = (first2Lines.Concat(temp)).ToArray();

            STRUCT_FSP_ERROR_LOG_INFO fspErrorLogInfo = new STRUCT_FSP_ERROR_LOG_INFO();
            fspErrorLogInfo.RawData = _errorRawData;
            fspErrorLogInfo.ErrorLogID = errorRawDataRows[2].Substring(2, errorRawDataRows[2].Length - 4).Trim().Split(" ")[4];
            fspErrorLogInfo.LogDate = errorRawDataRows[3].Substring(13, errorRawDataRows[3].Length - 15).Trim().Split(" ")[1];
            fspErrorLogInfo.LogTime = errorRawDataRows[3].Substring(13, errorRawDataRows[3].Length - 15).Trim().Split(" ")[2];
            fspErrorLogInfo.DriverName = errorRawDataRows[4].Substring(14, errorRawDataRows[4].Length - 16).Trim().Split(" ")[1];
            fspErrorLogInfo.Subsystem = errorRawDataRows[5].Substring(errorRawDataRows[5].IndexOf(":") + 2, errorRawDataRows[5].Length - (errorRawDataRows[5].IndexOf(":") + 2) - 2).Trim();
            fspErrorLogInfo.EventSeverity = errorRawDataRows[6].Substring(errorRawDataRows[6].IndexOf(":") + 2, errorRawDataRows[6].Length - (errorRawDataRows[6].IndexOf(":") + 2) - 2).Trim();
            fspErrorLogInfo.ActionFlags = new List<string>();

            int z = 7;
            while (!errorRawDataRows[z].Contains("| Action Status"))
            {
                string actionFlag = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();
                fspErrorLogInfo.ActionFlags.Add(actionFlag);
                z++;
            }

            fspErrorLogInfo.ActionStatus = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();
            z += 4; //Navigate to the Reference Code field
            fspErrorLogInfo.ReferenceCode = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();

            fspErrorLogInfo.NormalHardwareFRU = new List<STRUCT_FSP_ERROR_LOG_FRU_INFO>();
            while (true)
            {
                z++;
                if (errorRawDataRows[z].Contains("FRU"))
                {
                    z++;
                    STRUCT_FSP_ERROR_LOG_FRU_INFO FRU = new STRUCT_FSP_ERROR_LOG_FRU_INFO();
                    while (FRU.SerialNumber == null)
                    {
                        if (errorRawDataRows[z].Contains("Priority"))
                        {
                            FRU.Priority = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();
                        }
                        else if (errorRawDataRows[z].Contains("Location Code"))
                        {
                            FRU.LocationCode = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();
                        }
                        else if (errorRawDataRows[z].Contains("Part Number"))
                        {
                            FRU.PartNumber = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();
                        }
                        else if (errorRawDataRows[z].Contains("CCIN"))
                        {
                            FRU.CCIN = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();
                        }
                        else if (errorRawDataRows[z].Contains("Serial Number"))
                        {
                            FRU.SerialNumber = errorRawDataRows[z].Substring(errorRawDataRows[z].IndexOf(":") + 2, errorRawDataRows[z].Length - (errorRawDataRows[z].IndexOf(":") + 2) - 2).Trim();
                        }
                        z++;
                    }
                    fspErrorLogInfo.NormalHardwareFRU.Add(FRU);
                }
                if (errorRawDataRows[z].Contains("Log Hex Dump"))
                {
                    break; //Exit the loop when we reach the end of the FRU section
                }
            }

            // The rest of the data is the raw data, which is already stored in fspErrorLogInfo.RawData
            return fspErrorLogInfo;
        }

        public STRUCT_MACHINE_INFO GetMachineInfo()
        {
            POWERENV.CheckSerialSessionOpen();
            POWERENV.SendCommand("\n", 200);
            string response = POWERENV.GetReceivedData();
            string[] splitedResponse = response.Split("\n");

            STRUCT_MACHINE_INFO machineInfo = new STRUCT_MACHINE_INFO();


            for (int i = 0; i< splitedResponse.Length; i++)
            {
                if (splitedResponse[i].Contains("Machine type-model"))
                {
                    int startIndex = splitedResponse[i].IndexOf(":") + 2;
                    int length = splitedResponse[i].Length - startIndex - 2;
                    machineInfo.MachineTypeModel = splitedResponse[i].Substring(startIndex, length).Trim();
                }
                else if (splitedResponse[i].Contains("Serial number"))
                {
                    int startIndex = splitedResponse[i].IndexOf(":") + 2;
                    int length = splitedResponse[i].Length - startIndex - 2;
                    machineInfo.SerialNumber = splitedResponse[i].Substring(startIndex, length).Trim();
                }
            }

            POWERENV.SendCommand("\n", 500);
            POWERENV.GetReceivedData(); // Clear the buffer
            POWERENV.AuthManagementLib.ASMIAuthenticate();
            Thread.Sleep(700);

            response = POWERENV.GetReceivedData();
            splitedResponse = response.Split("\n");

            for(int i = 0; i < splitedResponse.Length; i++)
            {
                if (splitedResponse[i].Contains("Version"))
                {
                    int startIndex = splitedResponse[i].IndexOf(":") + 2;
                    int length = splitedResponse[i].Length - startIndex - 1;
                    machineInfo.ASMIVersion = splitedResponse[i].Substring(startIndex, length).Trim();
                }
                else if (splitedResponse[i].Contains("System name"))
                {
                    int startIndex = splitedResponse[i].IndexOf(":") + 2;
                    int length = splitedResponse[i].Length - startIndex - 1;
                    machineInfo.SystemName = splitedResponse[i].Substring(startIndex, length).Trim();
                }
            }

            POWERENV.AuthManagementLib.ASMISignOut();
            return machineInfo;
        }
    }
}
