# Quy Tắc Bắt Buộc Dành Cho AI (AI Instructions)

Tài liệu này định nghĩa các quy định khắt khe và bắt buộc mà bất kỳ AI Assistant nào tiếp nhận dự án này đều phải tuân thủ tuyệt đối trong mọi tình huống.

## 1. Luôn tự động cập nhật tài liệu (.md)
Mọi thay đổi trong mã nguồn của hệ thống (thêm class, thay đổi logic, thay đổi UI, thêm tính năng) **bắt buộc** phải được phản ánh ngay lập tức vào các file tài liệu `.md` tương ứng.
- **Sửa đổi**: Phải chỉnh sửa các file `.md` hiện có nếu logic thay đổi.
- **Tạo mới**: Phải tạo file `.md` mới nếu thêm một thành phần/module lớn hoàn toàn mới.
- **Đổi tên**: Tự động đổi tên các file `.md` nếu tên class hoặc module thay đổi nhằm đảm bảo tài liệu luôn song hành và ánh xạ 1:1 với dự án thực tế.

## 2. Luôn kiểm tra Build dự án
Trước khi thực hiện bất kỳ cập nhật nào cho tài liệu (.md) hoặc thực hiện `git commit`, AI Assistant **bắt buộc** phải:
- Chạy lệnh build (ví dụ: `dotnet build`) để đảm bảo toàn bộ các project trong solution đều biên dịch thành công.
- Nếu build thất bại, AI phải tìm cách sửa lỗi code cho đến khi build thành công mới được tiến hành bước tiếp theo.

## 3. Luôn tự động Git Commit
AI Assistant **không bao giờ được để code dở dang** trên local mà không commit:
- Sau mỗi lần giải quyết xong một Issue, một tính năng mới, hoặc thay đổi bất kỳ file `.cs`, `.xaml` hay `.md` nào, AI **phải tự động tạo ít nhất một git commit**.
- **Điều kiện tiên quyết**: Chỉ commit khi build đã thành công.
- Thông điệp (commit message) phải rõ ràng, giải thích được những gì vừa thay đổi (ví dụ: `feat: [Mô tả]`, `fix: [Mô tả]`, `docs: [Mô tả]`).
- Việc commit phải được thực hiện chủ động thông qua shell command (`git add .` và `git commit -m "..."`) trước khi thông báo lại kết quả cho người dùng.
