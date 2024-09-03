using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WeChat_OCR_Lib;
using Image = System.Drawing.Image;

namespace WeChat_OCR_Sharp;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class Form1 : Form {
    public Form1() {
        InitializeComponent();
        CheckForIllegalCrossThreadCalls = false;
    }

    private void Form1_Load(object sender, EventArgs e) {
        FileHandling.CopyMmmojoDll();
    }

    private void OCRResultCallback(string imgPath, WeChatOCRResult? result) {
        if (result?.OcrResult?.SingleResult == null) return;
        var str = string.Empty;
        var r = result.OcrResult.SingleResult;
        if (r != null) str = r.Aggregate(str, (current, singleResult) => current + singleResult.SingleStrUtf8);

        textBox1.Text = str;
    }

    private static Assembly? LoadAssembly(string assemblyPath) {
        if (!File.Exists(assemblyPath)) throw new Exception("未找到文件：" + assemblyPath);

        var ass = Assembly.Load(File.ReadAllBytes(assemblyPath));
        return ass ?? throw new Exception("加载：" + assemblyPath + " 失败！");
    }

    private static Assembly? AssemblyResolveEventHandler(object s, ResolveEventArgs e) {
        try {
            var fields = e.Name.Split(',');
            if (!fields.Any()) return null;

            return fields.Select(field => field + ".dll")
                .Select(name => (IList<string>) [
                    ImageOcr.GetWeChatDir()
                ])
                .Select(paths => paths.FirstOrDefault(File.Exists))
                .OfType<string>()
                .Select(LoadAssembly)
                .FirstOrDefault();
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    private void pictureBox1_Click(object sender, EventArgs e) {
        using var openFileDialog = new OpenFileDialog();
        openFileDialog.InitialDirectory = "c:\\";
        openFileDialog.Filter = @"Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
        openFileDialog.Title = @"Select an Image";

        if (openFileDialog.ShowDialog() != DialogResult.OK) return;
        var filePath = openFileDialog.FileName;
        pictureBox1.Image = Image.FromFile(filePath);
        using var ocr = new ImageOcr();
        ocr.Run(filePath, OCRResultCallback);
    }
}