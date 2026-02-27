# ImageTool - Architecture Overview

Dự án **ImageTool** được thiết kế tuân theo mẫu kiến trúc **MVVM (Model-View-ViewModel)** nhằm tách biệt rõ ràng giữa giao diện người dùng (.xaml) và logic xử lý (.cs).

## 1. Cấu trúc Thư mục

- `Models/`: Chứa các lớp dữ liệu cơ bản.
  - `AppSettings.cs`: Cấu hình của ứng dụng (lưu/tải từ JSON).
  - `ColorFilter.cs`: Mô hình cho một bộ lọc HSV. Hỗ trợ sự kiện `PropertyChanged`.
  - `OcrResult.cs`: Đại diện cho một kết quả nhận diện từ Tesseract (text, confidence, bounds).
- `ViewModels/`: Chứa các ViewModel làm trung gian giữa Models và Views.
  - `MainViewModel.cs`: Core logic điều khiển toàn bộ ứng dụng (quản lý state, gọi dịch vụ xử lý ảnh, cập nhật commands).
- `Services/`: Các lớp cung cấp chức năng nghiệp vụ xử lý độc lập.
  - `BitmapConversionService.cs`: Hỗ trợ chuyển đổi qua lại giữa định dạng `Mat` (Emgu.CV) và `BitmapSource` (WPF) với hiệu năng cao.
  - `OcrService.cs`: Đóng gói Tesseract engine. Đọc ảnh thang độ xám (Grayscale), xử lý, và trả về tập hợp các `OcrResult`.
- `Converters/`: Classes phục vụ giao diện (XAML binding converters).
  - Gồm quy đổi Boolean thành Visibility, quy đổi Enum sang Bool (dành cho RadioButtons), cấu hình GridLength.

## 2. Luồng chạy chính (Data Flow)

1. **Nhận ảnh**:
   Người dùng thông qua `OpenImageCommand` hoặc `PasteImageCommand` truyền hình ảnh vào hệ thống. Ảnh được `BitmapConversionService` đổi thành kiểu dữ liệu `Mat` của Emgu.CV và gán vào `_originalMat`.
   
2. **Cập nhật bộ lọc**:
   Bất kỳ khi nào người dùng thay đổi giá trị cấu hình HSV, hoặc bật tắt cờ Boolean trên giao diện, `Filter_PropertyChanged` hoặc `On...Changed` kích hoạt luồng `ApplyFilters()`.

3. **Áp dụng Filter & Morphology**:
   Trong `ApplyFilters()`, ảnh khởi điểm được convert sang HSV định dạng. Mỗi phần tử `ColorFilter` gộp mask bằng Bitwise OR. Sau đó gọi `ApplyMorphology()` thực thi Co, Giãn, Closing (nếu bật). 
   Kết quả xuất ra biến đổi trở lại thành ảnh preview `BitmapSource` gán vào _processedImage_.

4. **Thực thi OCR**:
   Nếu tính năng OCR kích hoạt (`IsOcrEnabled`), `RunOcrAsync()` được đẩy xuống ThreadPool để tranh block đường UI. Khung ảnh sau filter (`_processedMat`) được deep clone. 
   Trong `OcrService`, ảnh được phóng to (nếu cần scale), quét bằng Tesseract và phát ra `ObservableCollection<OcrResult>`. View (UI) sẽ tự động sinh các phần tử `Border` và `TextBlock` dán đè lên hiển thị để đánh dấu vị trí text do có cơ chế data binding.
