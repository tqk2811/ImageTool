# ImageTool - Project Summary

## Giới thiệu (Overview)
**ImageTool** là một ứng dụng Desktop được phát triển bằng WPF (.NET) kết hợp mô hình MVVM (Model-View-ViewModel). Ứng dụng cung cấp các công cụ xử lý ảnh mạnh mẽ, đặc biệt tập trung vào việc lọc màu (Color Filtering) và nhận dạng chữ viết quang học (OCR) để trích xuất văn bản từ hình ảnh.

## Mục tiêu dự án
1. **Xử lý ảnh trực quan**: Cho phép người dùng tải ảnh từ bộ nhớ hoặc clipboard, sau đó áp dụng các bộ lọc màu HSV để tách biệt vùng cần thiết.
2. **Tiền xử lý nâng cao**: Hỗ trợ các phép toán hình thái học (Morphological Operations) như Erosion, Dilation, Closing để làm rõ nét vùng ảnh trước khi đưa qua OCR.
3. **Trích xuất văn bản (OCR)**: Tích hợp engine Tesseract mạnh mẽ, hỗ trợ nhiều ngôn ngữ, trích xuất text kèm độ tin cậy và tọa độ (Bounding Box).
4. **Lưu trữ cấu hình**: Tự động lưu lại các bộ lọc màu (filters) và thiết lập giao diện (UI state, thông số OCR) của người dùng thông qua các file JSON cục bộ.

## Công nghệ sử dụng
- **Ngôn ngữ / Framework**: C#, .NET WPF
- **Kiến trúc**: MVVM (sử dụng thư viện `CommunityToolkit.Mvvm`)
- **Xử lý ảnh (Computer Vision)**: Emgu.CV (OpenCV wrapper cho .NET)
- **Nhận dạng chữ viết (OCR)**: Tesseract Engine (thư viện `Tesseract`)
- **Quản lý dữ liệu**: JSON Serialization (System.Text.Json)
