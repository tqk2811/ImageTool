# Hướng dẫn Tích hợp OCR (Tesseract)

Tài liệu này ghi lại các lưu ý kỹ thuật khi tích hợp Tesseract OCR vào project ImageTool.

## 1. Các gói NuGet cần thiết
Để sử dụng Tesseract với WPF và xử lý ảnh hiệu suất cao, cần các gói sau:
- `Tesseract`: Engine OCR lõi.
- `Tesseract.Drawing`: Hỗ trợ `PixConverter` để chuyển đổi `System.Drawing.Bitmap` sang `Pix`.
- `System.Drawing.Common`: Thư viện vẽ của .NET (cần cho Tesseract.Drawing).

## 2. Cấu hình Project
Để đạt hiệu suất tối ưu khi chuyển đổi dữ liệu ảnh, chúng ta sử dụng `AllowUnsafeBlocks`.
```xml
<PropertyGroup>
  <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
</PropertyGroup>
```

## 3. Chuyển đổi Mat sang Bitmap (Hiệu suất cao)
Sử dụng `unsafe` pointers để copy dữ liệu trực tiếp từ `Emgu.CV.Mat` sang `System.Drawing.Bitmap` mà không qua trung gian giúp tiết kiệm RAM và CPU.

## 4. Các lỗi thường gặp (Troubleshooting)
- **Namespce Collision**: `System.Windows.Rect` (WPF) và `Tesseract.Rect` (OCR) cùng tên. Cần sử dụng fully qualified name (ví dụ: `System.Windows.Rect`) khi định nghĩa `OcrResult`.
- **IDisposable**: Các đối tượng `TesseractEngine`, `Page`, `Pix`, và `Bitmap` đều phải được giải phóng đúng cách (sử dụng `using`).
- **Tessdata**: Thư mục `tessdata` chứa file ngôn ngữ (ví dụ `eng.traineddata`) phải nằm cùng thư mục với file `.exe` hoặc được trỏ đường dẫn chính xác.

## 5. Pipeline xử lý đề xuất
1. Lấy ảnh đã qua lọc HSV và Morphological Ops (`_processedMat`).
2. Chuyển sang Grayscale.
3. Chuyển sang `Bitmap` -> `Pix`.
4. Chạy Engine OCR.
5. Mapping toạ độ kết quả ngược lại toạ độ ảnh gốc (chia cho `ScaleFactor`).
