# Hướng dẫn cho coding agent

Trước khi sửa code, đọc [kiến trúc hệ thống](docs/architecture.md) và [quy tắc triển khai](docs/implementation-rules.md). Đây là hướng dẫn cho repo; khi làm việc trong thư mục con, tuân theo thêm `AGENTS.md` gần nhất nếu có.

## Phạm vi mặc định

- Chỉ thay đổi những phần cần thiết để hoàn thành yêu cầu hiện tại. Không tự mở rộng thành refactor, đổi kiến trúc, đổi API hoặc dọn dẹp không liên quan.
- Ưu tiên mẫu đã có trong module đang sửa. Nếu cần phá vỡ ranh giới module, thêm hạ tầng dùng chung, hoặc thay đổi hợp đồng/API, hãy nêu rõ lý do và tác động trong kết quả.
- Không tự suy diễn yêu cầu nghiệp vụ còn thiếu. Dựa vào code và docs hiện có; nếu thiếu thông tin làm thay đổi hành vi thì hỏi trước khi chốt hành vi.
- Không sửa/xóa migration đã áp dụng. Schema thay đổi cần migration mới trong đúng module và cập nhật snapshot.
- Không thêm package, dịch vụ ngoài, module mới hay cấu hình triển khai nếu yêu cầu không cần đến chúng.

## Hoàn tất công việc

- Giữ thay đổi nhỏ, dễ review và theo convention liền kề.
- Chạy build hoặc kiểm tra phù hợp khi có thể; báo chính xác lệnh đã chạy và kết quả. Không tuyên bố đã kiểm tra nếu chưa chạy.
- Tóm tắt file/hành vi đã đổi, giả định quan trọng, và các phần chưa xác minh.
