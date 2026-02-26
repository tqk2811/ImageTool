using ImageTool.ViewModels;

namespace ImageTool.Models
{
    public class AppSettings
    {
        public double ControlPanelWidth { get; set; } = 350;
        public double CheckerSize { get; set; } = 20;
        public bool IsSolidBackground { get; set; } = false;
        public byte[] SelectedBackgroundColor { get; set; } = new byte[] { 255, 128, 128, 128 }; // ARGB

        public bool IsOcrEnabled { get; set; } = false;
        public double OcrScaleFactor { get; set; } = 1.0;
        public int OcrErosionSize { get; set; } = 0;
        public int OcrDilationSize { get; set; } = 0;
        public int OcrClosingSize { get; set; } = 0;

        public bool IsOcrScaleEnabled { get; set; } = true;
        public bool IsOcrErosionEnabled { get; set; } = true;
        public bool IsOcrDilationEnabled { get; set; } = true;
        public bool IsOcrClosingEnabled { get; set; } = true;

        public string OcrLanguage { get; set; } = "eng";
        public ResultViewMode ResultPreviewMode { get; set; } = ResultViewMode.Bgra;
    }
}

