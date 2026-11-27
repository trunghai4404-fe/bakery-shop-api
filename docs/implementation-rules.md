# Quy tắc triển khai tính năng

Các quy tắc này giúp agent hoàn thành yêu cầu trong đúng phạm vi và theo kiến trúc hiện có. Nếu yêu cầu người dùng chỉ rõ hành vi khác, ưu tiên yêu cầu đó và nêu tác động.

## 1. Trước khi sửa

1. Đọc `AGENTS.md`, [architecture.md](architecture.md), và tài liệu API liên quan.
2. Truy dấu một use case tương tự trong cùng module: controller → command/query → handler/validator → domain/repository → mapping/response.
3. Kiểm tra `Shared` và `SharedKernel` xem đã có abstraction, helper hoặc kiểu nền tảng phù hợp để tái sử dụng chưa; kiểm tra cả các module lân cận trước khi tạo phần dùng chung mới.
4. Xác định hợp đồng cần giữ: route, payload, response, lỗi, permission, dữ liệu và migration.
5. Chỉ hỏi người dùng khi thiếu quyết định nghiệp vụ làm thay đổi kết quả; không tự thêm hành vi theo phỏng đoán.

## 2. Chọn nơi đặt code

| Nhu cầu | Vị trí |
|---|---|
| HTTP route/binding/authorization | `src/Api/Controllers/v1` |
| Một thao tác người dùng/nghiệp vụ | `<Module>.Application/Features/<Feature>/(Commands|Queries)` |
| Luật bất biến/trạng thái của entity | `<Module>.Domain` |
| Abstraction persistence thuộc module | `<Module>.Domain/Repositories` hoặc vị trí abstraction đã dùng trong module |
| EF Core/repository/provider adapter | `<Module>.Infrastructure` |
| DTO/interface được expose cho API/module khác | `<Module>.Contracts` |
| Primitive thực sự dùng xuyên module | `SharedKernel` hoặc `Shared`, theo vai trò sẵn có |

### Bắt buộc rà soát và tái sử dụng Shared/SharedKernel

- Trước khi implement một khả năng đã có thể dùng chung (ví dụ `Result`/`Error`, entity nền tảng, paging, current user, date/time, storage, API response, authorization hoặc MediatR behavior), hãy tìm trong `src/SharedKernel` và `src/Shared` trước. Nếu phù hợp về ngữ nghĩa và hành vi, dùng lại thành phần hiện hữu thay vì tạo bản sao trong module hoặc viết lại.
- Nếu khả năng chỉ thuộc một module, giữ nó trong module đó; không chuyển vào `Shared` chỉ vì có thể tái sử dụng trong tương lai.
- Chỉ mở rộng `Shared`/`SharedKernel` khi yêu cầu hiện tại cần khả năng đó cho nhiều nơi hoặc thuộc abstraction nền tảng dùng chung. Giữ thay đổi tương thích với caller hiện tại và tránh thêm phụ thuộc nghiệp vụ của module vào shared layer.
- Nếu thành phần hiện có không đáp ứng yêu cầu, nêu rõ điểm thiếu và chỉ mở rộng hoặc thay thế phần cần thiết; không tạo song song một abstraction mới có cùng trách nhiệm.

Không chuyển nghiệp vụ vào controller, repository hoặc DbContext chỉ để rút ngắn code. Không tạo tầng, interface, generic framework hoặc abstraction mới nếu một use case đơn giản có thể theo mẫu hiện hữu.

## 3. Quy tắc use case và validation

- Giữ một command/query đại diện cho một hành động có chủ đích. Handler điều phối, không trở thành nơi chứa toàn bộ mô hình nghiệp vụ.
- Dùng FluentValidation theo convention hiện có để kiểm tra shape/range/đầu vào của request. Kiểm tra phụ thuộc dữ liệu (ví dụ tồn tại, trùng khóa) qua abstraction phù hợp trong handler.
- Bất biến phải luôn đúng với entity thì đặt trong factory/method của domain; không dựa riêng vào HTTP validator.
- Dùng `CancellationToken` xuyên suốt các thao tác async như các handler hiện có.
- Trả lỗi nghiệp vụ theo mô hình `Result`/Error của codebase và error catalog cùng module nếu phù hợp. Không ném exception để biểu diễn lỗi dự kiến.
- Tránh query tải dữ liệu không cần thiết; projection/mapping theo DTO hiện có và giữ phân trang/filter hiện tại.

## 4. API, bảo mật và tương thích

- Giữ route `/api/v1`, kiểu HTTP, request/response wrapper và status code theo endpoint gần nhất.
- Controller chỉ bind dữ liệu, tạo/gửi request qua MediatR và chuyển kết quả bằng helper chung.
- Ghi permission/authentication theo yêu cầu nghiệp vụ và convention trong controller tương tự. Không làm endpoint đang bảo vệ thành public.
- Không expose entity nội bộ, trường bí mật/token/hash, hoặc thông tin người dùng vượt quyền truy cập.
- Thay đổi breaking API chỉ thực hiện khi người dùng yêu cầu hoặc chấp nhận rõ; cập nhật tài liệu API cùng thay đổi.

## 5. Ranh giới module và dữ liệu

- Module sở hữu model, repository, DbContext và migration của mình. Không truy cập trực tiếp DbContext/entity nội bộ module khác.
- Khi cần dữ liệu module khác, trước tiên kiểm tra Contracts/interface hiện hữu và dùng contract đó; không thêm coupling ngược từ Domain sang Infrastructure/API.
- Không giả định transaction nguyên tử xuyên nhiều DbContext. Nếu use case cần bảo đảm nguyên tử/nhất quán liên module mà cơ chế hiện tại không đáp ứng, mô tả giới hạn và đề xuất nhỏ nhất trước khi thiết kế thêm.
- Thay đổi schema: migration mới ở Infrastructure sở hữu bảng, snapshot cập nhật. Không sửa migration cũ để che đi thay đổi schema.
- Không xóa hoặc đổi ý nghĩa dữ liệu, endpoint, enum đã công khai mà không có yêu cầu/plan tương thích.

## 6. Phạm vi và chất lượng thay đổi

- Chỉ chạm file liên quan trực tiếp; không đổi format hàng loạt, rename hàng loạt, nâng package/framework hoặc refactor lân cận không được yêu cầu.
- Không thêm dependency nếu thư viện/năng lực hiện có đủ.
- Giữ naming và layout gần code liền kề, kể cả khi các module khác nhau đôi chút.
- Cập nhật docs khi hợp đồng API, quy tắc nghiệp vụ đáng kể, hoặc cách vận hành thay đổi.
- Không tạo test mới hay chạy bộ test chỉ theo thói quen nếu chưa được yêu cầu; khi yêu cầu có kiểm chứng, chọn lệnh build/test nhỏ nhất phù hợp và báo kết quả thực tế.
- Không tuyên bố build/test thành công nếu chưa chạy. Nếu môi trường chặn kiểm chứng, ghi rõ phần chưa xác minh.

## 7. Checklist bàn giao

- [ ] Thay đổi đáp ứng đúng yêu cầu và không mở rộng sang việc ngoài phạm vi.
- [ ] Controller/use case/domain/infrastructure nằm đúng module và tầng.
- [ ] Hợp đồng, quyền truy cập, lỗi và mapping được giữ hoặc cập nhật nhất quán.
- [ ] Migration mới được thêm nếu schema đổi.
- [ ] Tài liệu API/kiến trúc được cập nhật nếu cần.
- [ ] Báo file chính đã đổi, kiểm chứng đã chạy và giới hạn còn lại.
