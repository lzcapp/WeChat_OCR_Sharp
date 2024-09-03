// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace WeChat_OCR_Lib;

internal enum MMMojoInfoMethod {
    kMMNone = 0,
    kMMPush,
    kMMPullReq,
    kMMPullResp,
    kMMShared
}

internal enum MMMojoCallbackType {
    kMMUserData = 0,
    kMMReadPush,
    kMMReadPull,
    kMMReadShared,
    kMMRemoteConnect,
    kMMRemoteDisconnect,
    kMMRemoteProcessLaunched,
    kMMRemoteProcessLaunchFailed,
    kMMRemoteMojoError
}

internal enum MMMojoEnvironmentInitParamType {
    kMMHostProcess = 0,
    kMMLoopStartThread,
    kMMExePath,
    kMMLogPath,
    kMMLogToStderr,
    kMMAddNumMessagepipe,
    kMMSetDisconnectHandlers,
    kMMDisableDefaultPolicy = 1000,
    kMMElevated,
    kMMCompatible
}

internal class Mmmojo_Wrapper {
    public static void InitializeMMMojo(int argc, IntPtr argv) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.InitializeMMMojo(argc, argv);
        else
            Mmmojo_x86.InitializeMMMojo(argc, argv);
    }

    public static void ShutdownMMMojo() {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.ShutdownMMMojo();
        else
            Mmmojo_x86.ShutdownMMMojo();
    }

    public static IntPtr CreateMMMojoEnvironment() {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.CreateMMMojoEnvironment()
            : Mmmojo_x86.CreateMMMojoEnvironment();
    }

    public static void SetMMMojoEnvironmentCallbacks(IntPtr mmmojo_env, int type, IntPtr callback) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.SetMMMojoEnvironmentCallbacks(mmmojo_env, type, callback);
        else
            Mmmojo_x86.SetMMMojoEnvironmentCallbacks(mmmojo_env, type, callback);
    }

    public static void SetMMMojoEnvironmentInitParams(IntPtr mmmojo_env, int type, IntPtr param) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.SetMMMojoEnvironmentInitParams(mmmojo_env, type, param);
        else
            Mmmojo_x86.SetMMMojoEnvironmentInitParams(mmmojo_env, type, param);
    }

    public static void AppendMMSubProcessSwitchNative(IntPtr mmmojo_env, IntPtr switchStringPtr, IntPtr valuePtr) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.AppendMMSubProcessSwitchNative(mmmojo_env, switchStringPtr, valuePtr);
        else
            Mmmojo_x86.AppendMMSubProcessSwitchNative(mmmojo_env, switchStringPtr, valuePtr);
    }

    public static void StartMMMojoEnvironment(IntPtr mmmojo_env) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.StartMMMojoEnvironment(mmmojo_env);
        else
            Mmmojo_x86.StartMMMojoEnvironment(mmmojo_env);
    }

    public static void StopMMMojoEnvironment(IntPtr mmmojo_env) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.StopMMMojoEnvironment(mmmojo_env);
        else
            Mmmojo_x86.StopMMMojoEnvironment(mmmojo_env);
    }

    public static void RemoveMMMojoEnvironment(IntPtr mmmojo_env) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.RemoveMMMojoEnvironment(mmmojo_env);
        else
            Mmmojo_x86.RemoveMMMojoEnvironment(mmmojo_env);
    }

    public static IntPtr GetMMMojoReadInfoRequest(IntPtr mmmojo_readinfo, ref uint requestDataSize) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.GetMMMojoReadInfoRequest(mmmojo_readinfo, ref requestDataSize)
            : Mmmojo_x86.GetMMMojoReadInfoRequest(mmmojo_readinfo, ref requestDataSize);
    }

    public static IntPtr GetMMMojoReadInfoAttach(IntPtr mmmojo_readinfo, ref uint attachDataSize) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.GetMMMojoReadInfoAttach(mmmojo_readinfo, ref attachDataSize)
            : Mmmojo_x86.GetMMMojoReadInfoAttach(mmmojo_readinfo, ref attachDataSize);
    }

    public static void RemoveMMMojoReadInfo(IntPtr mmmojo_readinfo) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.RemoveMMMojoReadInfo(mmmojo_readinfo);
        else
            Mmmojo_x86.RemoveMMMojoReadInfo(mmmojo_readinfo);
    }

    public static int GetMMMojoReadInfoMethod(IntPtr mmmojo_readinfo) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.GetMMMojoReadInfoMethod(mmmojo_readinfo)
            : Mmmojo_x86.GetMMMojoReadInfoMethod(mmmojo_readinfo);
    }

    public static bool GetMMMojoReadInfoSync(IntPtr mmmojo_readinfo) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.GetMMMojoReadInfoSync(mmmojo_readinfo)
            : Mmmojo_x86.GetMMMojoReadInfoSync(mmmojo_readinfo);
    }

    public static IntPtr CreateMMMojoWriteInfo(int method, int sync, uint requestId) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.CreateMMMojoWriteInfo(method, sync, requestId)
            : Mmmojo_x86.CreateMMMojoWriteInfo(method, sync, requestId);
    }

    public static IntPtr GetMMMojoWriteInfoRequest(IntPtr mmmojo_writeinfo, uint requestDataSize) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.GetMMMojoWriteInfoRequest(mmmojo_writeinfo, requestDataSize)
            : Mmmojo_x86.GetMMMojoWriteInfoRequest(mmmojo_writeinfo, requestDataSize);
    }

    public static void RemoveMMMojoWriteInfo(IntPtr mmmojo_writeinfo) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.RemoveMMMojoWriteInfo(mmmojo_writeinfo);
        else
            Mmmojo_x86.RemoveMMMojoWriteInfo(mmmojo_writeinfo);
    }

    public static IntPtr GetMMMojoWriteInfoAttach(IntPtr mmmojo_writeinfo, uint attachDataSize) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.GetMMMojoWriteInfoAttach(mmmojo_writeinfo, attachDataSize)
            : Mmmojo_x86.GetMMMojoWriteInfoAttach(mmmojo_writeinfo, attachDataSize);
    }

    public static void SetMMMojoWriteInfoMessagePipe(IntPtr mmmojo_writeinfo, int numOfMessagePipe) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.SetMMMojoWriteInfoMessagePipe(mmmojo_writeinfo, numOfMessagePipe);
        else
            Mmmojo_x86.SetMMMojoWriteInfoMessagePipe(mmmojo_writeinfo, numOfMessagePipe);
    }

    public static void SetMMMojoWriteInfoResponseSync(IntPtr mmmojo_writeinfo, ref IntPtr mmmojo_readinfo) {
        if (SystemHandling.Is64BitOperatingSystem())
            Mmmojo_x64.SetMMMojoWriteInfoResponseSync(mmmojo_writeinfo, ref mmmojo_readinfo);
        else
            Mmmojo_x86.SetMMMojoWriteInfoResponseSync(mmmojo_writeinfo, ref mmmojo_readinfo);
    }

    public static bool SendMMMojoWriteInfo(IntPtr mmmojo_env, IntPtr mmmojo_writeinfo) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.SendMMMojoWriteInfo(mmmojo_env, mmmojo_writeinfo)
            : Mmmojo_x86.SendMMMojoWriteInfo(mmmojo_env, mmmojo_writeinfo);
    }

    public static bool SwapMMMojoWriteInfoCallback(IntPtr mmmojo_writeinfo, IntPtr mmmojo_readinfo) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.SwapMMMojoWriteInfoCallback(mmmojo_writeinfo, mmmojo_readinfo)
            : Mmmojo_x86.SwapMMMojoWriteInfoCallback(mmmojo_writeinfo, mmmojo_readinfo);
    }

    public static bool SwapMMMojoWriteInfoMessage(IntPtr mmmojo_writeinfo, IntPtr mmmojo_readinfo) {
        return SystemHandling.Is64BitOperatingSystem()
            ? Mmmojo_x64.SwapMMMojoWriteInfoMessage(mmmojo_writeinfo, mmmojo_readinfo)
            : Mmmojo_x86.SwapMMMojoWriteInfoMessage(mmmojo_writeinfo, mmmojo_readinfo);
    }
}