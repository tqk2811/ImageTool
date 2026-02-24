using System.Windows;

namespace ImageTool.Models
{
    /// <summary>
    /// Lưu kết quả OCR cho một từ/dòng chữ nhận dạng được.
    /// Dùng System.Windows.Rect để tránh conflict với Tesseract.Rect.
    /// </summary>
    public class OcrResult
    {
        public string Text { get; set; } = string.Empty;

        /// <summary>Tọa độ trên ảnh đã được scale (sau khi chia lại scaleFactor).</summary>
        public Rect Bounds { get; set; }

        public float Confidence { get; set; }
    }
}
