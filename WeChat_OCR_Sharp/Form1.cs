using System.Reflection;
using WeChat_OCR_Lib;

namespace WeChat_OCR_Sharp {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        static void OCRResultCallback(string imgPath, WeChatOCRResult? result) {
            if (result?.OcrResult == null) return;
            var r = result.OcrResult.SingleResult.Select(a => a.SingleStrUtf8);
            var str = string.Join("\n", r);
            textBox1.q
        }

        private static Assembly? LoadAssembly(string assemblyPath) {
            if (!File.Exists(assemblyPath)) {
                throw new Exception("未找到文件：" + assemblyPath);
            }
            Assembly? ass = Assembly.Load(File.ReadAllBytes(assemblyPath));
            return ass ?? throw new Exception("加载：" + assemblyPath + " 失败！");
        }

        private static Assembly? AssemblyResolveEventHandler(object s, ResolveEventArgs e) {
            try {
                var fields = e.Name.Split(',');
                if (!fields.Any())
                    return null;
                return fields.Select(field => field + ".dll")
                             .Select(name => (IList<string>)[
                                 ImageOcr.GetWeChatDir(),
                             ])
                             .Select(paths => paths.FirstOrDefault(File.Exists))
                             .OfType<string>()
                             .Select(path => LoadAssembly(path))
                             .FirstOrDefault();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            using var ocr = new ImageOcr();
            ocr.Run(, OCRResultCallback);
        }
    }
}
