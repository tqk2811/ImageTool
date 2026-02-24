# Thiết kế Giao diện & Tương tác (UI/UX Design)

**Bố cục chính (Grid Layout):**
- **Bên trái (hoặc phía trên):** Khu vực Control Panel.
  - Danh sách các bộ lọc (CheckBox để bật/tắt).
  - 3 thanh Range Slider (H, S, V) cho mỗi bộ lọc.
  - Các tham số tiền xử lý OCR (Numeric UpDown hoặc Slider cho Dilation, Erosion, Closing size, Scale factor).
  - ComboBox/ColorPicker để chọn màu nền (Bàn cờ caro hoặc Solid Color).
- **Bên phải (hoặc khu vực trung tâm):** Khu vực hiển thị ảnh.
  - Viewport nhỏ: Hiển thị ảnh gốc. Đặt trong một `ScrollViewer` hoặc `Canvas`, bắt sự kiện `MouseWheel` để thay đổi ScaleTransform (Zoom).
  - Viewport lớn: Hiển thị ảnh sau xử lý. Dùng `DrawingBrush` tạo pattern bàn cờ caro đen/trắng làm background của control chứa ảnh.

**Tương tác OCR Layer:**
- Sau khi chạy Tesseract, tạo một Overlay `Canvas` đè lên trên ảnh kết quả.
- Vẽ các `Rectangle` (WPF Shape) dựa trên Bounding Box trả về từ Tesseract.
- Gắn thuộc tính `ToolTip` vào các `Rectangle` này để hiển thị text nhận diện được khi người dùng hover chuột vào.