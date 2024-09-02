using Microsoft.Win32;

// ReSharper disable IdentifierTypo

namespace WeChat_OCR_Lib {
    public class FileHandling {
        public static string GetWeChatDir() {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(SystemHandling.Is64BitOperatingSystem() ? @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\WeChat" : @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\WeChat");
            if (key == null) {
                return string.Empty;
            }
            var dir = key.GetValue("DisplayVersion") is not string displayVersion ? string.Empty : Path.Combine(@"C:\Program Files\Tencent\WeChat", "[" + displayVersion + "]");
            return Directory.Exists(dir) ? dir : string.Empty;
        }

        public static string GetMmmojoDllDir() {
            var mmmojoDll = SystemHandling.Is64BitOperatingSystem() ? "mmmojo_64.dll" : "mmmojo.dll";
            var wechatDir = GetWeChatDir();
            if (!string.IsNullOrEmpty(wechatDir)) {
                var dllPath = Path.Combine(wechatDir, mmmojoDll);
                if (File.Exists(dllPath)) {
                    var fileInfo = new FileInfo(dllPath);
                    if (fileInfo.Length > 0) {
                        return wechatDir;
                    }
                }
            }
            wechatDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Tencent\WXWork\WeChatOCR\");
            var directories = Directory.GetDirectories(wechatDir);
            switch (directories.Length) {
                case 0:
                    break;
                default:
                    foreach (var directory in directories) {
                        var dllPath = Path.Combine(wechatDir, directory, "WeChatOCR", mmmojoDll);
                        if (File.Exists(dllPath)) {
                            var fileInfo = new FileInfo(dllPath);
                            if (fileInfo.Length > 0) {
                                return dllPath;
                            }
                        }
                    }
                    break;
            }
            return string.Empty;
        }

        public static string GetWeChatOCRExePath() {
            var ocrExePath = string.Empty;
            var searchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tencent");
            var searchPath1 = Path.Combine(searchPath, @"WeChat\XPlugin\Plugins\WeChatOCR");
            var directories = Directory.GetDirectories(searchPath1);
            switch (directories.Length) {
                case 0:
                    break;
                default:
                    foreach (var directory in directories) {
                        ocrExePath = Path.Combine(searchPath1, directory, @"extracted\WeChatOCR.exe");
                        if (File.Exists(ocrExePath)) {
                            var fileInfo = new FileInfo(ocrExePath);
                            if (fileInfo.Length > 0) {
                                return ocrExePath;
                            }
                        }
                    }
                    break;
            }
            var searchPath2 = Path.Combine(searchPath, @"WXWork\WeChatOCR");
            directories = Directory.GetDirectories(searchPath2);
            switch (directories.Length) {
                case 0:
                    break;
                default:
                    foreach (var directory in directories) {
                        ocrExePath = Path.Combine(searchPath2, directory, @"WeChatOCR\WeChatOCR.exe");
                        if (File.Exists(ocrExePath)) {
                            var fileInfo = new FileInfo(ocrExePath);
                            if (fileInfo.Length > 0) {
                                return ocrExePath;
                            }
                        }
                    }
                    break;
            }
            if (string.IsNullOrEmpty(ocrExePath)) {
                ocrExePath = Directory.EnumerateFiles(searchPath, "WeChatOCR.exe", SearchOption.AllDirectories).FirstOrDefault() ?? string.Empty;
                if (File.Exists(ocrExePath)) {
                    var fileInfo = new FileInfo(ocrExePath);
                    if (fileInfo.Length > 0) {
                        return ocrExePath;
                    }
                }
            }
            return string.Empty;
        }

        public static bool CopyMmmojoDll() {
            try {
                var dllDir = GetMmmojoDllDir();
                var mjName = SystemHandling.Is64BitOperatingSystem() ? "mmmojo_64.dll" : "mmmojo.dll";
                var source = Path.Combine(dllDir, mjName);
                var destin = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mjName);
                File.Copy(source, destin, false);
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}