namespace POWER_ENV
{
    internal class GLOBAL_METHODS
    {
        /// <summary>
        /// METHOD TO GET THE VALUE OF A SPECIFIC PROPERTY FROM THE RECEIVED DATA.
        /// </summary>
        /// <param name="_propertyName">Name of the property to search.</param>
        /// <param name="_receivedData">Data received by the serial connection.</param>
        /// <param name="startingChar">Character that marks the beggining of the value of the property.</param>
        /// <returns></returns>
        public static string GetPropertyValue(string _propertyName, string _receivedData, char startingChar)
        {
            string MACAddress = string.Empty;
            for (int i = 0; i < _receivedData.Length - _propertyName.Length; i++)
            {
                if (_receivedData.Substring(i, _propertyName.Length) == _propertyName)
                {
                    int j = i;
                    while (_receivedData[j] != '\n')
                    {
                        MACAddress += _receivedData[j];
                        j++;
                    }
                    break;
                }
            }
            if (MACAddress != string.Empty) MACAddress = MACAddress.Substring(MACAddress.IndexOf(startingChar) + 2).Trim();
            return MACAddress;
        }
    }
}