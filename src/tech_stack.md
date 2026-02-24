# Công nghệ & Thư viện (Tech Stack & Libraries)

**Ngôn ngữ & Framework:**
- Ngôn ngữ: C#
- UI Framework: WPF (Windows Presentation Foundation) .NET 6 hoặc mới hơn.
- Pattern: MVVM (Model-View-ViewModel) - *Khuyến khích dùng CommunityToolkit.Mvvm.*

**Thư viện xử lý:**
1. **Emgu.CV:** Dùng cho toàn bộ thao tác xử lý ảnh (chuyển đổi hệ màu, InRange HSV, MorphologyEx cho Dilation/Erosion/Closing, Resize cho Scale).
2. **Tesseract OCR:** Sử dụng wrapper cho .NET (ví dụ: `Tesseract` engine).
3. **WPF Extended Toolkit (hoặc tương đương):** Cần thiết để sử dụng control `RangeSlider` (thanh trượt có 2 đầu mút cho Min/Max) như trong ảnh mẫu thiết kế.

**Xử lý hiệu suất:**
- Quá trình xử lý ảnh (Image Processing Pipeline) và OCR phải chạy trên Background Thread (Task) để không làm đơ UI (Non-blocking UI).
- Tối ưu chuyển đổi giữa `Mat` (Emgu.CV) và `BitmapSource` / `WriteableBitmap` (WPF) để render ảnh nhanh chóng.