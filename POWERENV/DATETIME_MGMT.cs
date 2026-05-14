using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POWER_ENV
{
    public class DATETIME_MGMT
    {
        public void DEVICE_SET_DATE(string newDate)
        {
            POWERENV.CheckSerialSessionOpen();

            POWERENV.AuthManagementLib.ASMIAuthenticate();

            POWERENV.SendCommand("4", 200);
            POWERENV.SendCommand("4", 200);
            POWERENV.SendCommand("1", 200);
            POWERENV.SendCommand(newDate, 200);
            POWERENV.SendCommand("\n", 3000);

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        public void DEVICE_SET_TIME(string newTime)
        {
            POWERENV.CheckSerialSessionOpen();

            POWERENV.AuthManagementLib.ASMIAuthenticate();
            POWERENV.SendCommand("4", 200);
            POWERENV.SendCommand("4", 200);
            POWERENV.SendCommand("2", 200);
            POWERENV.SendCommand(newTime, 200);
            POWERENV.SendCommand("\n", 5000);

            POWERENV.AuthManagementLib.ASMISignOut();
        }

        public string changeDateStringFormat(string dt, int[] formatOrder)
        {
            string[] dateTimeParts = dt.Split('-');
            string invertedDateTime = $"{dateTimeParts[formatOrder[0]]}-{dateTimeParts[formatOrder[1]]}-{dateTimeParts[formatOrder[2]]}";
            return invertedDateTime;
        }
    }
}