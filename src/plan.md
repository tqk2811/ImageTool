# Kế hoạch thực thi (Implementation Plan)

Vui lòng thực hiện dự án theo các bước sau, hoàn thành và test kỹ bước trước rồi mới sang bước sau:

* **Phase 1: Khởi tạo & Cấu trúc UI cơ bản**
    * Khởi tạo project C# WPF.
    * Cài đặt các Nuget packages: Emgu.CV, Emgu.CV.runtime.windows, Tesseract, CommunityToolkit.Mvvm.
    * Dựng Layout XAML chia vùng (Preview gốc, Preview kết quả, Control Panel).
    * Tạo Background dạng caro cho vùng Preview kết quả.

- [x] Phase 1: Khởi tạo dự án và giao diện cơ bản.
- [x] Phase 2: Chức năng nạp ảnh (Open/Paste) và Viewport tương tác (Zoom/Pan).
- [x] Phase 3: Quản lý bộ lọc (Add/Remove) và Logic lọc HSV thời gian thực.
- [x] Phase 4: Tùy chỉnh hiển thị (Background) và Ghi nhớ trạng thái UI.
- [x] Phase 5: Nâng cao Tiền xử lý OCR và Persistence.
- [ ] Phase 6: Tích hợp OCR (Tesseract/OmniPage) và Xuất kết quả.

* **Phase 2: Viewport & Tương tác ảnh**
    * Xây dựng tính năng Load ảnh vào `Mat`.
    * Viết hàm chuyển đổi `Mat` sang `BitmapSource` an toàn.
    * Thêm tính năng Zoom (cuộn chuột) và Pan (kéo thả ảnh) cho Viewport ảnh gốc.

* **Phase 3: Logic Lọc HSV & Masking**
    * Tích hợp control RangeSlider cho H (0-180), S (0-255), V (0-255).
    * Viết pipeline xử lý: Convert BGR -> HSV -> `CvInvoke.InRange` tạo mask đen trắng -> Tách kênh Alpha của ảnh gốc -> Ghi đè mask vào kênh Alpha -> Hiển thị lên Preview.
    * Hỗ trợ gộp (Bitwise OR) nhiều mask nếu có nhiều dải HSV được chọn (như Trắng, Cyan...).

* **Phase 4: Tiền xử lý ảnh (Pre-processing)**
    * Thêm UI controls cho Scale, Erosion, Dilation, Closing.
    * Cập nhật pipeline: Áp dụng các phép toán hình thái học (`CvInvoke.MorphologyEx`) trên vùng ảnh sau khi đã qua Mask.

* **Phase 5: Nhận dạng chữ (Tesseract OCR) & Overlay**
    * Khởi tạo Tesseract Engine với ngôn ngữ thích hợp.
    * Thực hiện OCR trên ảnh đã qua tiền xử lý, lấy ra danh sách các Result Iterator (chứa toạ độ X, Y, W, H và Text).
    * Mapping toạ độ (Scale lại toạ độ nếu ảnh đã bị thu phóng) và vẽ các `Rectangle` lên một `Canvas` nằm đè lên Viewport.
    * Gắn sự kiện/Tooltip để hiển thị Text khi đưa chuột vào `Rectangle`.