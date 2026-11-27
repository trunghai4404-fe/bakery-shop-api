# Kiến trúc và cấu trúc hệ thống

## Mục đích

Tài liệu này ghi lại kiến trúc đang thể hiện trong source, đồng thời đặt các ranh giới mà thay đổi mới cần tôn trọng. Đây là một ứng dụng ASP.NET Core dạng modular monolith: một API host, nhiều module nghiệp vụ, build và triển khai cùng nhau. Không có bằng chứng trong cấu trúc hiện tại cho thấy đây là các microservice độc lập.

## Bản đồ repository

```text
src/
  Api/                         HTTP host, controllers, middleware, composition root
  Shared/                      thành phần dùng chung ở tầng ứng dụng/hạ tầng/web
  SharedKernel/                kiểu nền tảng và abstraction nhỏ dùng giữa các module
  Modules/
    Catalog/{Domain,Application,Infrastructure,Contracts}
    Identity/{Domain,Application,Infrastructure,Contracts}
    Ordering/{Domain,Application,Infrastructure,Contracts}
    Basket/{Domain,Application,Infrastructure,Contracts}
    Contents/{Domain,Application,Infrastructure,Contracts}
docs/                           tài liệu API/nghiệp vụ hiện có và hướng dẫn repo
```

Mỗi module có project riêng cho các tầng. Các thư mục `Features` trong Application thường nhóm theo use case (Commands/Queries và handler/validator); Infrastructure chứa EF Core context, repository, cấu hình, migration và adapter; Contracts chứa DTO hoặc interface được module khác/API host dùng. Mức độ hoàn thiện và cách tổ chức có thể khác giữa các module, vì vậy hãy dùng feature gần nhất trong chính module làm ví dụ.

## Vai trò các thành phần

- **Api**: khởi tạo host, đăng ký module, cấu hình pipeline và định tuyến HTTP. `Controllers/v1` ánh xạ request thành MediatR command/query rồi chuyển kết quả thành HTTP response. Controller không phải nơi đặt quy tắc nghiệp vụ.
- **Module.Application**: điều phối use case, kiểm tra đầu vào ở validator, gọi abstraction/repository và trả kết quả. MediatR được đăng ký từ assembly Application của từng module.
- **Module.Domain**: entity, value object, enum, lỗi nghiệp vụ và interface repository/unit of work thuộc về module. Quy tắc bất biến của entity nên nằm ở đây.
- **Module.Infrastructure**: triển khai persistence/adapter, EF Core DbContext, repository, seeding, external service và đăng ký dependency injection.
- **Module.Contracts**: bề mặt trao đổi ổn định mà API hoặc module khác cần dùng (request/response DTO, interface contract). Không dùng entity EF/domain làm payload HTTP hoặc hợp đồng xuyên module.
- **SharedKernel**: primitive và abstraction nền tảng nhỏ, không thuộc riêng module nào. **Shared** hiện chứa web/authz, behaviors, persistence/service dùng chung. Chỉ thêm thứ dùng chung khi có nhu cầu thực tế; không biến các thư mục này thành nơi gom tiện ích tùy tiện.

## Luồng request điển hình

```text
HTTP Controller → MediatR Command/Query → Handler
                                      → Domain + repository/interface
                                      → Infrastructure/DbContext
                                      → Result → ApiControllerBase/ApiResponse
```

Infrastructure cài đặt interface được Domain/Application tiêu thụ và được kết nối ở composition root (`Api/Program.cs` cùng extension DI của project). Các DbContext hiện được sở hữu theo module (ví dụ CatalogDbContext, IdentityDbContext, OrderingDbContext, ContentsDbContext, BasketDbContext); migration nằm cùng Infrastructure của module tương ứng. Đừng giả định có một transaction hoặc một database context chung giữa các module.

## Ranh giới module

- **Catalog**: sản phẩm, biến thể, danh mục và dữ liệu liên quan đến catalog.
- **Identity**: người dùng, đăng nhập/token, vai trò và quyền.
- **Ordering**: đơn hàng, trạng thái đơn, phương thức thanh toán/giao hàng.
- **Basket**: giỏ hàng và checkout trước khi tạo đơn.
- **Contents**: nội dung hiển thị như banner và dữ liệu địa điểm.

Đây là phân vùng theo nghiệp vụ, không phải lời khẳng định rằng mọi luồng hiện đã hoàn chỉnh. Ví dụ, luồng checkout có thể phối hợp Basket, Catalog và Ordering; giữ giao tiếp đó qua contract/abstraction hiện có thay vì để module truy cập DbContext hoặc entity nội bộ của nhau. API host có thể tham chiếu các module để lắp ghép hệ thống.

## Tích hợp API và lỗi

- Route hiện theo dạng `/api/v1/...`; controller đặt trong `Api/Controllers/v1`.
- Endpoint có quyền bảo vệ dùng ASP.NET authorization và permission attribute theo mẫu controller hiện hữu.
- Request đi qua controller đến MediatR; response thường được đóng gói bằng `ApiResponse<T>` và chuyển đổi qua `ApiControllerBase`.
- `SharedKernel.Domain.Result`/`Error` được dùng để biểu diễn một số lỗi nghiệp vụ; validation exception và lỗi không xử lý được có xử lý tập trung trong `Api/Infrastructure/GlobalExceptionHandler`.
- Giữ hợp đồng response, mã lỗi, status code, route, permission và semantics tương thích trừ khi yêu cầu nêu rõ cần thay đổi.

## Persistence

EF Core với PostgreSQL được đăng ký trong Infrastructure của module. Entity mapping đặt tại `Persistence/Configurations`; repository triển khai interface của module; migration và model snapshot đặt cạnh DbContext tương ứng. Thay đổi schema phải tạo migration mới đúng project/context và không chỉnh migration cũ đã tồn tại như cách sửa lịch sử.

## Điểm vào để tra cứu

- Composition root/pipeline: `src/Api/Program.cs`
- HTTP controllers: `src/Api/Controllers/v1/`
- Shared HTTP behavior: `src/Shared/Web/`
- Domain primitives: `src/SharedKernel/Domain/`
- Use cases và DI: `src/Modules/<Module>/<Module>.Application/`
- Persistence và DI: `src/Modules/<Module>/<Module>.Infrastructure/`
- Hợp đồng API/module: `src/Modules/<Module>/<Module>.Contracts/`
- API docs hiện có: `docs/*-api.md`

