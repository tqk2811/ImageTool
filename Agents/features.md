# ImageTool - Features & Functionalities

Tài liệu này mô tả chi tiết các tính năng chính của phần mềm **ImageTool**.

## 1. Nguồn ảnh đầu vào
- **Mở File (Open File)**: Hỗ trợ nạp các định dạng ảnh phổ biến như `.png`, `.jpeg`, `.jpg`, `.bmp` thông qua hộp thoại chọn file.
- **Dán từ Clipboard (Paste Image)**: Tích hợp khả năng lấy ảnh trực tiếp từ bộ nhớ tạm (Clipboard), thuận tiện khi sử dụng cùng các công cụ chụp màn hình (như Snipping Tool).

## 2. Hệ thống lọc màu (Color Filtering)
- Chuyển đổi không gian màu từ BGR sang **HSV** (Hue, Saturation, Value) để dễ dàng tách nền hoặc vùng màu cụ thể.
- Cho phép tạo danh sách nhiều bộ lọc (Filters). Mỗi bộ lọc xác định một dải giá trị H-S-V (Min/Max).
- Ứng dụng kết hợp nhiều bộ lọc bằng phép toán Bitwise OR để tạo ra một *Mask* tổng hợp.
- Xem trước trực tiếp kết quả với 2 chế độ:
  - `Gray`: Hiển thị thẳng Mask dưới dạng ảnh đen/trắng (ảnh xám 1 channel).
  - `Bgra`: Hiển thị ảnh gốc nhưng sử dụng Mask làm kênh Alpha, giúp làm trong suốt các khu vực không khớp với bộ lọc.

## 3. Tiền xử lý hình thái học (Morphology)
Để cải thiện độ chính xác cho hệ thống OCR, người dùng có thể kích hoạt các phép biên đổi hình thái học trên Mask:
- **Erosion (Co)**: Thu hẹp vùng chọn màu trắng.
- **Dilation (Giãn)**: Mở rộng vùng chọn màu trắng.
- **Closing (Đóng)**: Thực hiện Dilation theo sau là Erosion, để lấp đầy các đốm đen nhỏ nằm trong vùng chọn trắng.

## 4. Nhận dạng ký tự quang học (OCR)
- Nhúng **Tesseract Engine** (đọc tệp ngôn ngữ `.traineddata` trong thư mục `tessdata`).
- **Tự động trích xuất**: Nếu OCR được bật, ngay khi bộ lọc chạy, tính năng OCR sẽ tự động quét văn bản trên nền (Background Thread).
- **Scale Factor**: Cho phép phóng to cục bộ ảnh (Upscale) trước khi đẩy vào engine Tesseract để cải thiện khả năng đọc các ký tự quá nhỏ, sau đó tự động mapping lại tọa độ (Bounding Box) về kích thước ảnh gốc.
- **Kết quả trả về**: Hiển thị chuỗi text, độ tin cậy (Confidence > 30%) và đánh dấu vị trí Bounding Box trên ảnh.

## 5. Persistence (Lưu trữ trạng thái)
- Tự động lưu và tải danh sách bộ lọc vào file `filters.json`.
- Tự động lưu kích thước panel, tùy chọn morphology, tuỳ chọn OCR ngôn ngữ, và màu nền lưới vào file `settings.json`.
