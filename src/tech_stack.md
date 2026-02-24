# Công nghệ & Thư viện (Tech Stack & Libraries)

**Ngôn ngữ & Framework:**
- Ngôn ngữ: C#
- UI Framework: WPF (Windows Presentation Foundation) .NET 6 hoặc mới hơn.
- Pattern: MVVM (Model-View-ViewModel) - *Khuyến khích dùng CommunityToolkit.Mvvm.*

**Thư viện xử lý:**
1. **Emgu CV**: Wrapper .NET cho OpenCV, dùng để xử lý ảnh (chuyển đổi hệ màu, lọc vùng HSV, phép toán hình thái học).
2. **Tesseract OCR:** Sử dụng wrapper cho .NET (gói `Tesseract` và `Tesseract.Drawing` để hỗ trợ `System.Drawing.Bitmap`).
3. **Extended WPF Toolkit**: Cung cấp các control nâng cao như `RangeSlider`, `ColorPicker`, `WatermarkTextBox`.
4. **System.Text.Json**: Xử lý lưu/tải cấu hình bộ lọc và cài đặt ứng dụng.

**Xử lý hiệu suất:**
- **AllowUnsafeBlocks**: Sử dụng con trỏ (pointers) và `Buffer.MemoryCopy` để chuyển đổi dữ liệu cực nhanh giữa `Mat` và `Bitmap`.
- Quá trình xử lý ảnh (Image Processing Pipeline) và OCR phải chạy trên Background Thread (Task) để không làm đơ UI (Non-blocking UI).
- Tối ưu chuyển đổi giữa `Mat` (Emgu.CV) và `BitmapSource` / `WriteableBitmap` (WPF) để render ảnh nhanh chóng.