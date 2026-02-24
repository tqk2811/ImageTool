# Tổng quan dự án (Project Overview)

**Mô tả:** Đây là một ứng dụng Desktop sử dụng C# WPF. Chức năng chính là xử lý ảnh: lọc màu theo không gian HSV để tạo mask (trong suốt), và nhận dạng chữ (OCR) bằng Tesseract với các bước tiền xử lý ảnh.

**Các tính năng cốt lõi:**
1. **Xem trước ảnh (Image Preview):**
   - Ảnh gốc (kích thước nhỏ) hỗ trợ zoom bằng thao tác cuộn chuột (Mouse Wheel).
   - Ảnh kết quả (kích thước lớn) hiển thị real-time sau khi áp dụng mask hoặc các bộ lọc.
   - Nền của ảnh kết quả có thể đổi giữa "Fake transparent" (dạng bàn cờ caro sọc trắng đen) hoặc một màu đơn sắc tuỳ chọn.
2. **Lọc màu HSV (HSV Color Filtering):**
   - Giao diện có các thanh kéo dạng Range Slider (có 2 đầu mút Min/Max) cho 3 kênh H, S, V.
   - Hỗ trợ chọn và tích hợp nhiều bộ lọc (như Trắng, Cyan...) cùng lúc.
   - Áp dụng mask tạo ra từ dải HSV vào kênh Alpha của ảnh (làm trong suốt các vùng không khớp).
3. **Nhận dạng chữ (OCR) với Tesseract:**
   - Hỗ trợ các bước tiền xử lý ảnh (Pre-processing) trước khi đưa vào OCR: Closing, Dilation, Erosion, Scale.
   - Vẽ khung chữ nhật (Bounding box) lên giao diện tại toạ độ phát hiện chữ.
   - Tương tác: Đưa chuột vào khung chữ nhật sẽ hiện tooltip chứa nội dung chữ, hoặc vẽ mờ text trực tiếp lên khung.