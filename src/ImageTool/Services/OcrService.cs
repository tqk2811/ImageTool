using Emgu.CV;
using Emgu.CV.CvEnum;
using ImageTool.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Tesseract;

namespace ImageTool.Services
{
    /// <summary>
    /// Dịch vụ OCR dùng Tesseract. Hỗ trợ eng / vie.
    /// Gọi InitializeOcr() sau khi đổi Language rồi mới PerformOcr().
    /// </summary>
    public class OcrService : IDisposable
    {
        private TesseractEngine? _engine;
        private bool _disposed;

        public string TessdataPath { get; }
        public string Language { get; private set; } = "eng";

        public OcrService()
        {
            TessdataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
        }

        /// <summary>Khởi tạo (hoặc khởi tạo lại) engine với ngôn ngữ mới.</summary>
        public void InitializeOcr(string language = "eng")
        {
            Language = language;
            _engine?.Dispose();
            _engine = null;

            if (!Directory.Exists(TessdataPath))
            {
                Debug.WriteLine($"[OCR] tessdata folder not found: {TessdataPath}");
                return;
            }

            try
            {
                _engine = new TesseractEngine(TessdataPath, language, EngineMode.Default);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OCR] InitializeOcr error: {ex.Message}");
            }
        }

        /// <summary>
        /// Chạy OCR trên _processedMat sau khi đã qua pipeline.
        /// scaleFactor: ảnh được phóng to bao nhiêu lần trước khi đưa vào Tesseract.
        /// Tọa độ bounding box trả về đã được chia lại scaleFactor → map về ảnh gốc.
        /// </summary>
        public List<OcrResult> PerformOcr(Mat sourceMat, double scaleFactor)
        {
            var results = new List<OcrResult>();
            if (_engine == null || sourceMat == null || sourceMat.IsEmpty)
                return results;

            try
            {
                // 1. Scale ảnh nếu scaleFactor > 1
                Mat workMat = sourceMat;
                bool shouldDispose = false;
                if (scaleFactor > 1.0 && scaleFactor <= 10.0)
                {
                    workMat = new Mat();
                    CvInvoke.Resize(sourceMat, workMat,
                        new System.Drawing.Size(
                            (int)(sourceMat.Width * scaleFactor),
                            (int)(sourceMat.Height * scaleFactor)),
                        0, 0, Inter.Cubic);
                    shouldDispose = true;
                }

                // 2. Chuyển về Grayscale
                using Mat grayMat = new Mat();
                if (workMat.NumberOfChannels == 1)
                    workMat.CopyTo(grayMat);
                else if (workMat.NumberOfChannels >= 3)
                    CvInvoke.CvtColor(workMat, grayMat, ColorConversion.Bgr2Gray);
                else
                    workMat.CopyTo(grayMat);

                // 3. Mat → System.Drawing.Bitmap (unsafe copy)
                using Bitmap bitmap = MatToGrayscaleBitmap(grayMat);

                // 4. Bitmap → Pix → OCR
                using var pix = PixConverter.ToPix(bitmap);
                using var page = _engine.Process(pix);

                // 5. Lấy kết quả cấp Word
                using var iter = page.GetIterator();
                iter.Begin();
                do
                {
                    if (iter.IsAtBeginningOf(PageIteratorLevel.Word))
                    {
                        string text = iter.GetText(PageIteratorLevel.Word)?.Trim() ?? string.Empty;
                        float conf = iter.GetConfidence(PageIteratorLevel.Word);

                        if (!string.IsNullOrEmpty(text) && conf > 30f)
                        {
                            if (iter.TryGetBoundingBox(PageIteratorLevel.Word, out var rect))
                            {
                                // Map tọa độ về ảnh gốc (chia scaleFactor)
                                results.Add(new OcrResult
                                {
                                    Text = text,
                                    Confidence = conf,
                                    Bounds = new System.Windows.Rect(
                                        rect.X1 / scaleFactor,
                                        rect.Y1 / scaleFactor,
                                        (rect.X2 - rect.X1) / scaleFactor,
                                        (rect.Y2 - rect.Y1) / scaleFactor)
                                });
                            }
                        }
                    }
                }
                while (iter.Next(PageIteratorLevel.Word));

                if (shouldDispose) workMat.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[OCR] PerformOcr error: {ex.Message}");
            }

            return results;
        }

        /// <summary>
        /// Chuyển Mat (8-bit grayscale) sang System.Drawing.Bitmap bằng unsafe pointer copy.
        /// Nhanh hơn nhiều so dùng Marshal.Copy vì không có overhead.
        /// </summary>
        private static unsafe Bitmap MatToGrayscaleBitmap(Mat grayMat)
        {
            int width = grayMat.Width;
            int height = grayMat.Height;

            var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            // Thiết lập bảng màu grayscale
            var palette = bitmap.Palette;
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
            bitmap.Palette = palette;

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            try
            {
                byte* src = (byte*)grayMat.DataPointer;
                byte* dst = (byte*)bitmapData.Scan0;
                int srcStride = grayMat.Step;
                int dstStride = bitmapData.Stride;

                for (int row = 0; row < height; row++)
                    Buffer.MemoryCopy(src + row * srcStride, dst + row * dstStride, dstStride, srcStride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _engine?.Dispose();
                _disposed = true;
            }
        }
    }
}
