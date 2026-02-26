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
- **Tessdata**: Thư mục `tessdata` chứa file ngôn ngữ (ví dụ `eng.traineddata`). Ứng dụng tự động quét thư mục này để nạp danh sách ngôn ngữ vào UI.
- **Dynamic Loading**: `MainViewModel` sẽ gọi `LoadAvailableLanguages()` khi khởi tạo để tìm tất cả các file `*.traineddata`.

## 5. Pipeline xử lý đề xuất
1. Lấy ảnh đã qua lọc HSV và Morphological Ops.
2. **Chế độ xem trước (ResultViewMode)**:
   - **Gray**: Chỉ hiển thị mask đa qua xử lý (grayscale).
   - **Bgra**: Ghép kênh Alpha từ mask vào ảnh BGR gốc để hiển thị trên nền carô.
3. Chuyển sang Grayscale (nếu là Bgra).
4. Chuyển sang `Bitmap` -> `Pix`.
5. Chạy Engine OCR.
6. Mapping toạ độ kết quả ngược lại toạ độ ảnh gốc.
7. **Rendering (UI Fix)**: Để đảm bảo khung OCR không bị lệch khi Zoom/Pan, `Canvas` chứa kết quả OCR được đặt cùng cấp với `Image` bên trong một `Grid` (scaling container) và nằm trong `ScrollViewer`. Khi zoom, chúng ta zoom cả `Grid` này.
8. **Visualization**: Vẽ viền đỏ 1px và hiển thị text trực tiếp trên đầu bounding box (không dùng tooltip để tăng trải nghiệm người dùng).
9. **Xử lý số kênh ảnh (Channel Handling)**: Đảm bảo ảnh đầu vào preview luôn được chuẩn hóa (ví dụ: chuyển 4-channel sang 3-channel trước khi trộn alpha) để tránh lỗi hiển thị sọc (striping bug).
10. **Độ phân giải Scale**: Hỗ trợ Scale Factor từ 0.1 trở lên để người dùng linh hoạt điều chỉnh độ phân giải đầu vào cho Tesseract.

## 6. Lưu ý về Quy tắc AI
Theo `AI_Rules.md`, mọi thay đổi mã nguồn phải được phản ánh vào tài liệu này và dự án phải build thành công trước khi commit.
