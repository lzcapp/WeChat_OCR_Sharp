﻿using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace WeChat_OCR_Lib {
    public class ImageOcr : IDisposable {
        private const int MaxRetries = 99;

        public ImageOcr() {
            OCRManager = new OCRManager();
            var ocrExePath = GetWeChatOcrExePath();
            var wechatDir = GetWeChatDir();
            //var ocrExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"extracted\WeChatOCR.exe");
            //var wechatDir = AppDomain.CurrentDomain.BaseDirectory;

            var ocrPtr = GCHandle.ToIntPtr(GCHandle.Alloc(OCRManager));
            OCRManager = (GCHandle.FromIntPtr(ocrPtr).Target as OCRManager)!;
            OCRManager.SetExePath(ocrExePath);
            OCRManager.SetUsrLibDir(wechatDir);
            OCRManager.StartWeChatOCR(ocrPtr);
        }

        public OCRManager OCRManager { get; }
        public void Dispose() => OCRManager.KillWeChatOCR();

        public void Run(string imagePath, Action<string, WeChatOCRResult?>? callback) {
            if (callback != null) {
                OCRManager.SetOcrResultCallback(callback);
            }
            var retryCount = 0;
            while (retryCount <= MaxRetries) {
                try {
                    OCRManager.DoOcrTask(imagePath);
                    return;
                } catch (OverflowException) {
                    Thread.Sleep(10);
                    retryCount++;
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }
        }

        public static string GetWeChatDir() {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\WeChat");
            if (key == null)
                return string.Empty;
            if (key.GetValue("DisplayVersion") is not string displayVersion) {
                return string.Empty;
            }

            return Path.Combine(@"C:\Program Files\Tencent\WeChat", "[" + displayVersion + "]");
        }

        private string GetWeChatOcrExePath() {
            var searchPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Tencent\WeChat\XPlugin\Plugins\WeChatOCR");
            return Directory.EnumerateFiles(searchPath, "WeChatOCR.exe", SearchOption.AllDirectories).FirstOrDefault() ?? string.Empty;
        }
    }
}