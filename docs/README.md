# Tài liệu dự án

- [Kiến trúc và cấu trúc hệ thống](architecture.md): bản đồ source, trách nhiệm từng project và ranh giới module.
- [Quy tắc triển khai tính năng](implementation-rules.md): quy tắc phạm vi và checklist cho người triển khai/agent.
- `banner-api.md`, `location-api.md`, `role-permission-api.md`: tài liệu endpoint hiện có.
- [Quản lý cửa hàng](store-api.md): endpoint CMS, danh sách public cho Web và gắn snapshot store vào đơn khi checkout.
- [Quản lý thông tin liên hệ](contents-api.md): CRUD cho CMS và danh sách liên hệ đang hoạt động cho web-app.
- [API giỏ hàng, checkout và đơn hàng](basket-api.md) dành cho CMS và Web-app, gồm lịch sử trạng thái đơn hàng.

Điểm bắt đầu cho agent là [AGENTS.md](../AGENTS.md). Khi API hoặc nghiệp vụ đổi, cập nhật tài liệu endpoint liên quan trong cùng thay đổi.
