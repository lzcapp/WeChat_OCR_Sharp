namespace WeChat_OCR_Lib {
    internal static class SystemHandling {
        internal static string GetMmmojoName() {
            var mmmojoName = Is64BitOperatingSystem() ? "mmmojo_64.dll" : "mmmojo.dll";
            return mmmojoName;
        }

        internal static bool Is64BitOperatingSystem() {
            var oConn = new ConnectionOptions();
            var oMs = new ManagementScope(@"\\localhost", oConn);
            var oQuery = new ObjectQuery("select AddressWidth from Win32_Processor");
            var oSearcher = new ManagementObjectSearcher(oMs, oQuery);
            ManagementObjectCollection oReturnCollection = oSearcher.Get();
            var addressWidth = string.Empty;
            foreach (ManagementBaseObject? o in oReturnCollection) {
                var oReturn = (ManagementObject)o;
                addressWidth = oReturn["AddressWidth"].ToString();
            }
            return addressWidth is "64";
        }
    }
}
