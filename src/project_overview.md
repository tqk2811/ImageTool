# Tổng quan dự án (Project Overview)

**Mô tả:** Đây là một ứng dụng Desktop sử dụng C# WPF. Chức năng chính là xử lý ảnh: lọc màu theo không gian HSV để tạo mask (trong suốt), và nhận dạng chữ (OCR) bằng Tesseract với các bước tiền xử lý ảnh.

**Các tính năng cốt lõi:**
1. **Xem trước ảnh (Image Preview):**
   - **Giao diện thân thiện**: Hỗ trợ kéo thả ảnh từ clipboard, phóng to/thu nhỏ (Zoom) và di chuyển (Pan) trong quá trình xem trước.
   - **Xem trước thời gian thực**: Cập nhật ảnh kết quả ngay lập tức khi thay đổi thông số.
   - **Tùy chỉnh nền**: Cho phép đổi nền carô (giả trong suốt) hoặc màu đơn sắc để nhìn rõ kết quả lọc.
2. **Lọc màu HSV (HSV Color Filtering):**
   - **Bộ lọc màu HSV nâng cao**: Quản lý danh sách bộ lọc đa dạng, lọc màu sắc chính xác theo dải H, S, V.
   - Giao diện có các thanh kéo dạng Range Slider (có 2 đầu mút Min/Max) cho 3 kênh H, S, V.
   - Hỗ trợ chọn và tích hợp nhiều bộ lọc (như Trắng, Cyan...) cùng lúc.
   - Áp dụng mask tạo ra từ dải HSV vào kênh Alpha của ảnh (làm trong suốt các vùng không khớp).
3. **Nhận dạng chữ (OCR) với Tesseract:**
   - **Tiền xử lý OCR**: Cung cấp các công cụ Scale, Erosion, Dilation, Closing để tối ưu ảnh cho nhận dạng văn bản.
   - Hỗ trợ các bước tiền xử lý ảnh (Pre-processing) trước khi đưa vào OCR: Closing, Dilation, Erosion, Scale.
   - Vẽ khung chữ nhật (Bounding box) lên giao diện tại toạ độ phát hiện chữ.
   - Tương tác: Đưa chuột vào khung chữ nhật sẽ hiện tooltip chứa nội dung chữ, hoặc vẽ mờ text trực tiếp lên khung.