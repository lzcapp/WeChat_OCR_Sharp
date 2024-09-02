using System.Runtime.InteropServices;

namespace WeChat_OCR_Lib {
    [StructLayout(LayoutKind.Sequential)]
    public class XPluginManager : IDisposable {
        private bool isMmmojoEnvInited = false;
        private IntPtr intPtrMmmojoEnv = IntPtr.Zero;
        private IntPtr intPtrUserData;
        private IntPtr intPtrOcrExePath;
        private List<string> cmdLines = [];
        private readonly DefaultCallbacks defaultCallbacks = new();
        private readonly Dictionary<string, Delegate> callbacks = [];
        private readonly Dictionary<string, string> m_switch_native = [];

        public XPluginManager() {
            //var wechatPath = AppDomain.CurrentDomain.BaseDirectory;
            //var dllName = Environment.Is64BitProcess ? "mmmojo_64.dll" : "mmmojo.dll";
            //var Mmmojo_WrapperPath = Path.Combine(wechatPath, dllName);
            //if (!File.Exists(Mmmojo_WrapperPath)) throw new Exception("给定的微信路径不存在 mmmojo.dll");
        }

        ~XPluginManager() {
            if (isMmmojoEnvInited) {
                StopMMMojoEnv();
            }
        }

        public void SetExePath(string exePath) {
            intPtrOcrExePath = Marshal.StringToHGlobalUni(exePath);
        }

        public void SetExePath() {
            var exePath = FileHandling.GetWeChatOCRExePath();
            intPtrOcrExePath = Marshal.StringToHGlobalUni(exePath);
        }

        public void AppendSwitchNativeCmdLine(string arg, string value) {
            m_switch_native[arg] = value;
        }

        public void SetCommandLine(List<string> cmdline) {
            cmdLines = cmdline;
        }

        public void SetOneCallback(string name, Delegate func) {
            callbacks[name] = func;
        }

        public void SetCallbacks(Dictionary<string, Delegate> callbacks) {
            foreach (var callback in callbacks) {
                this.callbacks[callback.Key] = callback.Value;
            }
        }

        public void SetCallbackUsrData(IntPtr cbUsrData) {
            intPtrUserData = cbUsrData;
        }

        public void InitMMMojoEnv() {
            if (intPtrOcrExePath != null) {
                var exePath = Marshal.PtrToStringUni(this.intPtrOcrExePath);
                if (!File.Exists(exePath))
                    throw new Exception($"给定的 WeChatOcr.exe 路径错误 (m_exe_path): {exePath}");
            }
            if (isMmmojoEnvInited && intPtrMmmojoEnv != IntPtr.Zero)
                return;

            Mmmojo_Wrapper.InitializeMMMojo(0, IntPtr.Zero);
            intPtrMmmojoEnv = Mmmojo_Wrapper.CreateMMMojoEnvironment();
            if (intPtrMmmojoEnv == IntPtr.Zero)
                throw new Exception("CreateMMMojoEnvironment 失败!");
            Mmmojo_Wrapper.SetMMMojoEnvironmentCallbacks(intPtrMmmojoEnv, (int)MMMojoCallbackType.kMMUserData, intPtrUserData);
            SetDefaultCallbacks();
            Mmmojo_Wrapper.SetMMMojoEnvironmentInitParams(intPtrMmmojoEnv, (int)MMMojoEnvironmentInitParamType.kMMHostProcess, new IntPtr(1));
            Mmmojo_Wrapper.SetMMMojoEnvironmentInitParams(intPtrMmmojoEnv, (int)MMMojoEnvironmentInitParamType.kMMExePath, intPtrOcrExePath);

            foreach (var item in m_switch_native) {
                var keyPtr = Marshal.StringToHGlobalAnsi(item.Key);
                var valuePtr = Marshal.StringToHGlobalUni(item.Value);
                Mmmojo_Wrapper.AppendMMSubProcessSwitchNative(intPtrMmmojoEnv, keyPtr, valuePtr);
            }
            Mmmojo_Wrapper.StartMMMojoEnvironment(intPtrMmmojoEnv);
            isMmmojoEnvInited = true;
        }

        private void SetDefaultCallbacks() {
            foreach (MMMojoCallbackType type in Enum.GetValues(typeof(MMMojoCallbackType))) {
                if (type == MMMojoCallbackType.kMMUserData)
                    continue;
                try {
                    string fname = type.ToString();
                    Delegate callback = defaultCallbacks.callbacks[fname];
                    if (callbacks.ContainsKey(fname))
                        callback = callbacks[fname];
                    IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
                    Mmmojo_Wrapper.SetMMMojoEnvironmentCallbacks(intPtrMmmojoEnv, (int)type, callbackPtr);
                } catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public void StopMMMojoEnv() {
            if (isMmmojoEnvInited && intPtrMmmojoEnv != IntPtr.Zero) {
                Mmmojo_Wrapper.StopMMMojoEnvironment(intPtrMmmojoEnv);
                Mmmojo_Wrapper.RemoveMMMojoEnvironment(intPtrMmmojoEnv);
                intPtrMmmojoEnv = IntPtr.Zero;
                isMmmojoEnvInited = false;
            }
        }

        public void SendPbSerializedData(byte[] pbData, int pbSize, int method, int sync, uint requestId) {
            IntPtr writeInfo = Mmmojo_Wrapper.CreateMMMojoWriteInfo(method, sync, requestId);
            IntPtr request = Mmmojo_Wrapper.GetMMMojoWriteInfoRequest(writeInfo, (uint)pbSize);
            Marshal.Copy(pbData, 0, request, pbSize);
            Mmmojo_Wrapper.SendMMMojoWriteInfo(intPtrMmmojoEnv, writeInfo);
        }

        public IntPtr GetPbSerializedData(IntPtr requestInfo, ref uint dataSize) => Mmmojo_Wrapper.GetMMMojoReadInfoRequest(requestInfo, ref dataSize);

        public IntPtr GetReadInfoAttachData(IntPtr requestInfo, ref uint dataSize) => Mmmojo_Wrapper.GetMMMojoReadInfoAttach(requestInfo, ref dataSize);

        public void RemoveReadInfo(IntPtr requestInfo) => Mmmojo_Wrapper.RemoveMMMojoReadInfo(requestInfo);

        public void Dispose() {
            if (isMmmojoEnvInited)
                StopMMMojoEnv();
        }
    }
}