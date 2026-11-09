# Banner Management API Specification (CMS CRUD)

Tài liệu đặc tả toàn bộ API phục vụ triển khai chức năng **CRUD Banner** trên hệ thống CMS (Admin Portal) và Web Client của Bakery Shop.

---

## 1. Thông tin chung

- **Base URL:** `/api/v1`
- **Content-Type:** `application/json` (trừ API upload file là `multipart/form-data`)
- **Authentication:** Bearer Token (`Authorization: Bearer <access_token>`)
- **Quyền hạn (Permissions):**
  - Xem danh sách / chi tiết: `Banner.View`
  - Tạo mới banner: `Banner.Create`
  - Cập nhật banner / Đổi trạng thái: `Banner.Update`
  - Xóa banner: `Banner.Delete`

### Cấu trúc Response chuẩn

Hệ thống sử dụng vỏ bọc chuẩn `ApiResponse<T>`:

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": { ... },
  "errors": null,
  "timestamp": "2026-10-21T09:00:00Z",
  "traceId": null
}
```

Khi có lỗi:
```json
{
  "isSuccess": false,
  "statusCode": 400,
  "message": "Banner title is required.",
  "data": null,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "description": "Banner title is required.",
      "fieldName": "Title"
    }
  ],
  "timestamp": "2026-10-21T09:00:00Z",
  "traceId": "00-xxxxxxxx-01"
}
```

---

## 2. Định nghĩa Enums & Model dữ liệu

### 2.1. `BannerActionType` (Hành động khi click banner)

| Value (Int) | Name | Ý nghĩa | Quy tắc `targetValue` |
|:---:|:---|:---|:---|
| `0` | `None` | Chỉ hiển thị, không click | Bỏ trống hoặc `null` |
| `1` | `ExternalLink` | Mở liên kết ngoài trang web | URL đầy đủ (ví dụ: `https://facebook.com/...`) |
| `2` | `InternalRoute` | Chuyển trang nội bộ | Đường dẫn route (ví dụ: `/promotions/summer`) |
| `3` | `Product` | Chuyển hướng tới chi tiết sản phẩm | `ProductId` hoặc `slug` sản phẩm |
| `4` | `Category` | Chuyển hướng tới danh mục | `CategoryId` hoặc `slug` danh mục |
| `5` | `Promotion` | Chuyển hướng tới trang khuyến mãi | `PromotionId` hoặc mã chiến dịch |
| `6` | `ApplyVoucher` | Nhận / Áp dụng voucher nhanh | Mã voucher (`VOUCHER_CODE`) |

> **Lưu ý nghiệp vụ:** Khi `actionType != 0 (None)`, trường `targetValue` là **bắt buộc**.

### 2.2. `BannerStatus` (Trạng thái hiển thị banner)

Trạng thái được tính toán động từ hệ thống dựa trên `isActive`, `startDate`, `endDate` so với thời gian hiện tại (`DateTimeOffset.UtcNow`):

| Value (Int) | Name | Mô tả | Điều kiện |
|:---:|:---|:---|:---|
| `0` | `Inactive` | Đang tắt thủ công | `isActive == false` |
| `1` | `Active` | Đang hiển thị hợp lệ | `isActive == true` và trong khoảng `[startDate, endDate]` |
| `2` | `Scheduled` | Đã lên lịch, chưa tới ngày | `isActive == true` và `startDate > UtcNow` |
| `3` | `Expired` | Đã hết hạn hiển thị | `isActive == true` và `endDate < UtcNow` |

---

## 3. Danh sách Endpoints CMS

### 3.1. Lấy danh sách Banner có phân trang & lọc (CMS)

Sử dụng để hiển thị bảng quản lý Banner trên CMS.

- **Endpoint:** `GET /api/v1/banners`
- **Yêu cầu đăng nhập:** Có (`[Authorize]`)
- **Permission:** `Banner.View`

#### Query Parameters:
| Tham số | Kiểu dữ liệu | Bắt buộc | Mặc định | Mô tả |
|:---|:---:|:---:|:---:|:---|
| `search` | `string` | Không | - | Tìm kiếm gần đúng theo tiêu đề banner (`title`) |
| `isActive` | `boolean` | Không | - | Lọc theo cờ bật/tắt: `true` / `false` |
| `status` | `int` | Không | - | Lọc theo `BannerStatus`: `0` (Inactive), `1` (Active), `2` (Scheduled), `3` (Expired) |
| `page` | `int` | Không | `1` | Số trang (bắt đầu từ 1) |
| `pageSize` | `int` | Không | `10` | Số lượng phần tử trên mỗi trang |

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "items": [
      {
        "id": "e3009772-23c2-4a7b-a4ff-7d9a8385db11",
        "title": "Bánh Croissant Mùa Hè - Giảm 20%",
        "imageUrl": "https://cdn.example.com/banners/croissant-summer.jpg",
        "mobileImageUrl": "https://cdn.example.com/banners/croissant-summer-mobile.jpg",
        "actionType": "Category",
        "actionTypeValue": 4,
        "targetValue": "banh-croissant",
        "openInNewTab": false,
        "isActive": true,
        "status": "Active",
        "statusValue": 1,
        "startDate": "2026-10-01T00:00:00+07:00",
        "endDate": "2026-11-01T23:59:59+07:00",
        "metadata": "{\"campaign\":\"Summer2026\"}",
        "createdAt": "2026-10-07T08:30:00Z",
        "lastModifiedAt": "2026-10-07T09:15:00Z"
      }
    ],
    "page": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3,
    "hasPreviousPage": false,
    "hasNextPage": true
  },
  "errors": null,
  "timestamp": "2026-10-21T09:00:00Z"
}
```

---

### 3.2. Lấy chi tiết Banner theo Id

Sử dụng khi mở form chỉnh sửa hoặc modal xem chi tiết.

- **Endpoint:** `GET /api/v1/banners/{id}`
- **Yêu cầu đăng nhập:** Không bắt buộc (Endpoint mở)
- **Path Parameter:**
  - `id` (`Guid`): Id của banner cần lấy.

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "id": "e3009772-23c2-4a7b-a4ff-7d9a8385db11",
    "title": "Bánh Croissant Mùa Hè - Giảm 20%",
    "imageUrl": "https://cdn.example.com/banners/croissant-summer.jpg",
    "mobileImageUrl": "https://cdn.example.com/banners/croissant-summer-mobile.jpg",
    "actionType": "Category",
    "actionTypeValue": 4,
    "targetValue": "banh-croissant",
    "openInNewTab": false,
    "isActive": true,
    "status": "Active",
    "statusValue": 1,
    "startDate": "2026-10-01T00:00:00+07:00",
    "endDate": "2026-11-01T23:59:59+07:00",
    "metadata": "{\"campaign\":\"Summer2026\"}",
    "createdAt": "2026-10-07T08:30:00Z",
    "lastModifiedAt": "2026-10-07T09:15:00Z"
  },
  "errors": null,
  "timestamp": "2026-10-21T09:00:00Z"
}
```

#### Response `404 Not Found`:
```json
{
  "isSuccess": false,
  "statusCode": 404,
  "message": "Banner not found.",
  "data": null,
  "errors": [
    {
      "code": "BANNER_NOT_FOUND",
      "description": "Banner not found."
    }
  ]
}
```

---

### 3.3. Tạo mới Banner

- **Endpoint:** `POST /api/v1/banners`
- **Yêu cầu đăng nhập:** Có (`[Authorize]`)
- **Permission:** `Banner.Create`

#### Request Body (`application/json`):
```json
{
  "title": "Khuyến mãi Khai trương cơ sở mới",
  "imageUrl": "https://cdn.example.com/banners/grand-opening.jpg",
  "mobileImageUrl": "https://cdn.example.com/banners/grand-opening-mobile.jpg",
  "actionType": 1,
  "targetValue": "https://bakery.com/events/opening",
  "openInNewTab": true,
  "isActive": true,
  "startDate": "2026-11-01T00:00:00Z",
  "endDate": "2026-11-15T23:59:59Z",
  "metadata": "{\"badge\":\"HOT\"}"
}
```

#### Quy tắc Validation:
- `title`: Bắt buộc, tối đa 250 ký tự.
- `imageUrl`: Bắt buộc, tối đa 1000 ký tự.
- `mobileImageUrl`: Tùy chọn, tối đa 1000 ký tự.
- `actionType`: Giá trị từ `0` đến `6`.
- `targetValue`: Bắt buộc khi `actionType != 0`.
- `endDate`: Nếu có `startDate` và `endDate`, thì `endDate >= startDate`.

#### Response `200 OK`:
Trả về thông tin Banner vừa được tạo (kèm ID vừa sinh).

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "id": "c1f72922-a9b3-4632-9cb8-bdfaa7329471",
    "title": "Khuyến mãi Khai trương cơ sở mới",
    "imageUrl": "https://cdn.example.com/banners/grand-opening.jpg",
    "mobileImageUrl": "https://cdn.example.com/banners/grand-opening-mobile.jpg",
    "actionType": "ExternalLink",
    "actionTypeValue": 1,
    "targetValue": "https://bakery.com/events/opening",
    "openInNewTab": true,
    "isActive": true,
    "status": "Scheduled",
    "statusValue": 2,
    "startDate": "2026-11-01T00:00:00Z",
    "endDate": "2026-11-15T23:59:59Z",
    "metadata": "{\"badge\":\"HOT\"}",
    "createdAt": "2026-10-21T09:10:00Z",
    "lastModifiedAt": null
  },
  "errors": null,
  "timestamp": "2026-10-21T09:10:00Z"
}
```

---

### 3.4. Cập nhật Banner

- **Endpoint:** `PUT /api/v1/banners/{id}`
- **Yêu cầu đăng nhập:** Có (`[Authorize]`)
- **Permission:** `Banner.Update`
- **Path Parameter:**
  - `id` (`Guid`): Id của banner cần sửa.

#### Request Body (`application/json`):
```json
{
  "title": "Khuyến mãi Khai trương cơ sở mới (Đã cập nhật)",
  "imageUrl": "https://cdn.example.com/banners/grand-opening-v2.jpg",
  "mobileImageUrl": null,
  "actionType": 2,
  "targetValue": "/events/opening",
  "openInNewTab": false,
  "isActive": true,
  "startDate": "2026-11-01T00:00:00Z",
  "endDate": "2026-11-20T23:59:59Z",
  "metadata": "{\"badge\":\"SUPER_HOT\"}"
}
```

#### Response `200 OK`:
Trả về thông tin Banner sau khi đã cập nhật.

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "id": "c1f72922-a9b3-4632-9cb8-bdfaa7329471",
    "title": "Khuyến mãi Khai trương cơ sở mới (Đã cập nhật)",
    "imageUrl": "https://cdn.example.com/banners/grand-opening-v2.jpg",
    "mobileImageUrl": null,
    "actionType": "InternalRoute",
    "actionTypeValue": 2,
    "targetValue": "/events/opening",
    "openInNewTab": false,
    "isActive": true,
    "status": "Scheduled",
    "statusValue": 2,
    "startDate": "2026-11-01T00:00:00Z",
    "endDate": "2026-11-20T23:59:59Z",
    "metadata": "{\"badge\":\"SUPER_HOT\"}",
    "createdAt": "2026-10-21T09:10:00Z",
    "lastModifiedAt": "2026-10-21T09:15:30Z"
  },
  "errors": null,
  "timestamp": "2026-10-21T09:15:30Z"
}
```

---

### 3.5. Bật / Tắt trạng thái hoạt động (Quick Toggle)

Dùng cho nút bấm Switch/Toggle on-off trực tiếp ngay trên Table ở CMS mà không cần mở form Edit.

- **Endpoint:** `PATCH /api/v1/banners/{id}/toggle`
- **Yêu cầu đăng nhập:** Có (`[Authorize]`)
- **Permission:** `Banner.Update`
- **Path Parameter:**
  - `id` (`Guid`): Id của banner.

#### Request Body (`application/json`):
*Tùy chọn: có thể gửi giá trị cụ thể, hoặc gửi body rỗng `{}` để hệ thống tự đảo ngược giá trị hiện tại (`!isActive`).*

```json
{
  "isActive": false
}
```

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": null,
  "errors": null,
  "timestamp": "2026-10-21T09:20:00Z"
}
```

---

### 3.6. Xóa Banner (Soft Delete)

- **Endpoint:** `DELETE /api/v1/banners/{id}`
- **Yêu cầu đăng nhập:** Có (`[Authorize]`)
- **Permission:** `Banner.Delete`
- **Path Parameter:**
  - `id` (`Guid`): Id của banner cần xóa.

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Banner deleted successfully.",
  "data": null,
  "errors": null,
  "timestamp": "2026-10-21T09:25:00Z"
}
```

---

### 3.7. Upload Ảnh Banner (Phục vụ Form CMS)

CMS gọi API này để tải file ảnh lên trước khi lưu URL vào Banner.

- **Endpoint:** `POST /api/v1/contents/files`
- **Content-Type:** `multipart/form-data`
- **Yêu cầu đăng nhập:** Có

#### Form Data:
- `files`: File danh sách ảnh (hoặc 1 file).

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "fileUrls": [
      "https://cdn.example.com/uploads/2026/10/banner_image_01.jpg"
    ]
  },
  "errors": null,
  "timestamp": "2026-10-21T09:05:00Z"
}
```

---

## 4. API Dành Cho Storefront / Web Khách Hàng (Tham khảo)

Lấy toàn bộ banner đang có hiệu lực (`isActive = true`, nằm trong khung thời gian `startDate` -> `endDate`), sắp xếp mới nhất lên đầu.

- **Endpoint:** `GET /api/v1/banners/active`
- **Yêu cầu đăng nhập:** Không

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": [
    {
      "id": "e3009772-23c2-4a7b-a4ff-7d9a8385db11",
      "title": "Bánh Croissant Mùa Hè - Giảm 20%",
      "imageUrl": "https://cdn.example.com/banners/croissant-summer.jpg",
      "mobileImageUrl": "https://cdn.example.com/banners/croissant-summer-mobile.jpg",
      "actionType": "Category",
      "actionTypeValue": 4,
      "targetValue": "banh-croissant",
      "openInNewTab": false,
      "isActive": true,
      "status": "Active",
      "statusValue": 1,
      "startDate": "2026-10-01T00:00:00+07:00",
      "endDate": "2026-11-01T23:59:59+07:00",
      "metadata": null,
      "createdAt": "2026-10-07T08:30:00Z",
      "lastModifiedAt": null
    }
  ]
}
```

---

## 5. Hướng dẫn thiết kế giao diện CMS (UI/UX Best Practices)

### 5.1. Bảng danh sách Banner (Table View)
- **Cột hiển thị:**
  1. Hình ảnh thumbnail (`imageUrl` preview)
  2. Tiêu đề (`title`)
  3. Loại hành động (`ActionType` badge: Sản phẩm, Danh mục, Link ngoài...)
  4. Trạng thái (`Status` Tag: Active - xanh lá, Scheduled - xanh dương, Expired - xám, Inactive - đỏ cam)
  5. Thời gian áp dụng (`startDate` -> `endDate`)
  6. Switch kích hoạt (`isActive` toggle trực tiếp gọi API `PATCH /toggle`)
  7. Thao tác (Nút Sửa, Xóa)
- **Thanh công cụ lọc:**
  - Ô tìm kiếm theo tên (`search`)
  - Dropdown lọc trạng thái (`status`: Tất cả / Hoạt động / Đã lên lịch / Đã hết hạn / Đang tắt)
  - Nút "Thêm mới Banner"

### 5.2. Form Thêm / Sửa Banner (Modal hoặc Page)
- **Tiêu đề banner** *(Input text, bắt buộc, max 250)*.
- **Upload ảnh Desktop** *(Upload component -> gọi `POST /api/v1/contents/files` -> binding `imageUrl`)*.
- **Upload ảnh Mobile** *(Upload component -> binding `mobileImageUrl`)*.
- **Hành động khi click (Action Type)**: Dropdown chọn:
  - *Không hành động (`0`)*: ẩn ô Target Value.
  - *Mở link ngoài (`1`)*: hiển thị input URL.
  - *Trang nội bộ (`2`)*: hiển thị input path.
  - *Sản phẩm (`3`)*: hiển thị Select tìm kiếm sản phẩm.
  - *Danh mục (`4`)*: hiển thị Dropdown danh mục bánh.
  - *Khuyến mãi (`5`)* / *Voucher (`6`)*: hiển thị ô nhập mã tương ứng.
- **Mở trong tab mới (`openInNewTab`)**: Checkbox / Switch.
- **Thời gian áp dụng**: DateRangePicker (`startDate`, `endDate`). Nếu không chọn thì banner hiển thị vô thời hạn (khi `isActive = true`).
- **Kích hoạt ngay (`isActive`)**: Switch bật/tắt.
- **Metadata**: JSON Editor hoặc trường key-value mở rộng (nếu cần cấu hình thêm màu sắc, nhãn phụ...).
