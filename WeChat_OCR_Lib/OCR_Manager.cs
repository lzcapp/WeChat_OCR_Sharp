using System.Diagnostics;
using System.Runtime.InteropServices;
using Google.Protobuf;
using OcrProtobuf;

namespace WeChat_OCR_Lib;

[StructLayout(LayoutKind.Sequential)]
public class OCRManager : XPluginManager, IDisposable {
    public const int OCR_MAX_TASK_ID = 32;

    private Action<string, WeChatOCRResult?>? Callback;
    private bool isWeChatOcrRunning;
    private readonly Dictionary<int, string> dicImageID = [];
    private readonly Queue<int> queueIds = [];
    private readonly Stopwatch[] timers = new Stopwatch[OCR_MAX_TASK_ID];
    private volatile bool isConnected;

    public OCRManager() {
        for (var i = 0; i < OCR_MAX_TASK_ID; i++) {
            queueIds.Enqueue(i);
            timers[i] = new Stopwatch();
        }
    }

    public void SetUsrLibDir(string usrLibDir) {
        AppendSwitchNativeCmdLine("user-lib-dir", usrLibDir);
    }

    public void SetOcrResultCallback(Action<string, WeChatOCRResult?> func) {
        Callback = func;
    }

    public void StartWeChatOCR(IntPtr ocrManager) {
        SetCallbackUsrData(ocrManager);
        SetDefaultCallbacks();
        InitMMMojoEnv();
        isWeChatOcrRunning = true;
    }

    public void KillWeChatOCR() {
        isConnected = false;
        isWeChatOcrRunning = false;
        StopMMMojoEnv();
    }

    public void DoOcrTask(string picPath) {
        if (!isWeChatOcrRunning) throw new Exception("请先调用StartWeChatOCR启动");
        if (!File.Exists(picPath)) throw new FileNotFoundException($"给定图片路径picPath不存在: {picPath}");
        picPath = Path.GetFullPath(picPath);
        var numTry = 0;
        while (!isConnected) {
            Console.WriteLine("等待Ocr服务连接成功!");
            Thread.Sleep(100);
            if (numTry++ > 3) throw new TimeoutException("连接Ocr服务超时！");
        }

        var taskId = GetIdleTaskId();
        if (taskId < 0) throw new OverflowException("当前队列已满，请等待后重试！");
        SendOCRTask(taskId, picPath);
    }

    public void SetConnectState(bool connect) {
        isConnected = connect;
    }

    public void SendOCRTask(int taskId, string picPath) {
        dicImageID[taskId] = picPath;
        var ocrRequest = new OcrRequest {
            Unknow = 0,
            TaskId = taskId,
            PicPath = new OcrRequest.Types.PicPaths { PicPath = { picPath } }
        };
        var serializedData = ocrRequest.ToByteArray();

        SendPbSerializedData(serializedData, serializedData.Length, (int)MMMojoInfoMethod.kMMPush, 0,
            (int)RequestIdOCR.OCRPush);
    }

    public WeChatOCRResult? ParseJsonResponse(string jsonResponseStr) {
        return ParseOcrResult.ParseJson(jsonResponseStr);
    }

    /// <summary>领取TaskId（当TaskId>-1时才会发送任务）</summary>
    public int GetIdleTaskId() {
        var taskId = -1;
        try {
            taskId = queueIds.Dequeue();
            timers[taskId].Restart();
        }
        catch (InvalidOperationException) { }

        return taskId;
    }

    /// <summary>归还TaskId</summary>
    public void SetTaskIdIdle(int taskId) {
        queueIds.Enqueue(taskId);
        var lastStr = dicImageID[taskId].Substring(dicImageID[taskId].Length - 16, 16);
        timers[taskId].Stop();
        Console.WriteLine($"【…{lastStr}】由任务{taskId,2}完成，耗费【{(int)timers[taskId].Elapsed.TotalMilliseconds}】毫秒");
    }

    public void SetDefaultCallbacks() {
        SetOneCallback("kMMRemoteConnect", new DefaultCallbacks.MMRemoteConnectDelegate(OCRRemoteOnConnect));
        SetOneCallback("kMMRemoteDisconnect", new DefaultCallbacks.MMRemoteDisconnectDelegate(OCRRemoteOnDisconnect));
        SetOneCallback("kMMReadPush", new DefaultCallbacks.MMReadPushDelegate(OCRReadOnPush));
    }

    public void OCRRemoteOnConnect(bool isConnected, IntPtr userData) {
        //Console.WriteLine($"回调函数【{nameof(OCRRemoteOnConnect)}】被调用");
        if (userData == IntPtr.Zero) return;
        var restoredHandle = GCHandle.FromIntPtr(userData);
        if (restoredHandle.Target is OCRManager restoredObject) restoredObject.SetConnectState(true);
    }

    public void OCRRemoteOnDisconnect(IntPtr userData) {
        //Console.WriteLine($"回调函数【{nameof(OCRRemoteOnDisconnect)}】被调用");
        if (userData != IntPtr.Zero) SetConnectState(false);
    }

    public void OCRReadOnPush(uint requestId, IntPtr requestInfo, IntPtr userData) {
        //Console.WriteLine($"回调函数【{nameof(OCRReadOnPush)}】被调用，request_id: {requestId}, request_info: {requestInfo}");
        if (userData == IntPtr.Zero) return;
        uint pbSize = 0;
        var pbData = GetPbSerializedData(requestInfo, ref pbSize);
        if (pbSize > 20) {
            //Console.WriteLine($"正在解析pb数据，pb数据大小: {pbSize}");
            CallUserCallback(requestId, pbData, (int)pbSize);
            RemoveReadInfo(requestInfo);
        }
    }

    public void CallUserCallback(uint requestId, IntPtr serializedData, int dataSize) {
        //Console.WriteLine($"回调函数【{nameof(CallUserCallback)}】被调用，request_id: {requestId}");
        var ocrResponseArray = new byte[dataSize];
        Marshal.Copy(serializedData, ocrResponseArray, 0, dataSize);
        var ocrResponse = OcrResponse.Parser.ParseFrom(ocrResponseArray);
        if (ocrResponse.ErrCode != 0) {
            Console.WriteLine($"回调函数【{nameof(CallUserCallback)}】被调用，ErrCode: {ocrResponse.ErrCode}");
        }
        var jsonResponseStr = ocrResponse.ToString();
        var taskId = ocrResponse.TaskId;
        if (!dicImageID.TryGetValue(taskId, out var picPath)) {
            return;
        }
        Callback?.Invoke(picPath, ParseJsonResponse(jsonResponseStr));
        SetTaskIdIdle(taskId);
    }

    public new void Dispose() {
        base.Dispose();
        if (isWeChatOcrRunning) KillWeChatOCR();
    }
}

public enum RequestIdOCR {
    OCRPush = 1
}