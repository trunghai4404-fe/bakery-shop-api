# Role & Permission Management API Specification (CMS RBAC)

Tài liệu đặc tả toàn bộ API phục vụ triển khai chức năng **Quản lý Vai trò & Phân quyền (Role-Based Access Control - RBAC)** trên hệ thống CMS (Admin Portal) của Bakery Shop.

---

## 1. Thông tin chung

- **Base URL:** `/api/v1`
- **Content-Type:** `application/json`
- **Authentication:** Bearer Token (`Authorization: Bearer <access_token>`)
- **Quyền hạn hệ thống (Permissions):**
  - **Quản lý vai trò:**
    - Xem danh sách / chi tiết vai trò: `Role.View`
    - Tạo mới vai trò: `Role.Create`
    - Cập nhật vai trò & quyền hạn: `Role.Update`
    - Xóa vai trò: `Role.Delete`
  - **Quản lý danh mục quyền hạn:**
    - Xem danh sách quyền hạn: `Permission.View`
    - Cập nhật quyền hạn: `Permission.Update`
  - **Phân vai trò cho người dùng:**
    - Xem danh sách người dùng & vai trò: `User.View`
    - Gán vai trò cho người dùng: `User.Update`

---

### Cấu trúc Response chuẩn

Hệ thống sử dụng vỏ bọc chuẩn `ApiResponse<T>` cho mọi phản hồi:

#### Khi thành công (`200 OK` có dữ liệu):
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": { ... },
  "errors": null,
  "timestamp": "2026-10-21T14:30:00Z",
  "traceId": null
}
```

#### Khi thành công cho các thao tác không trả về dữ liệu (Delete, Assign Roles):
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": null,
  "errors": null,
  "timestamp": "2026-10-21T14:30:00Z",
  "traceId": null
}
```

#### Cấu trúc danh sách phân trang `PagedList<T>`:
```json
{
  "items": [ ... ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 25,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

#### Khi có lỗi nghiệp vụ hoặc validation (`400 Bad Request`, `404 Not Found`, `409 Conflict`):
```json
{
  "isSuccess": false,
  "statusCode": 400,
  "message": "Role name is required.",
  "data": null,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "description": "Role name is required.",
      "fieldName": "Name"
    }
  ],
  "timestamp": "2026-10-21T14:30:00Z",
  "traceId": "00-xxxxxxxx-01"
}
```

---

## 2. Mô hình Dữ liệu & Danh mục Quyền / Vai trò chuẩn

### 2.1. Thực thể `Role` (Vai trò)

| Trường | Kiểu dữ liệu | Bắt buộc | Mô tả |
|:---|:---:|:---:|:---|
| `id` | `int` | Có | Khóa chính tự tăng |
| `code` | `string` | Có | Mã vai trò (Unique, viết hoa, ví dụ: `ADMIN`, `STAFF`, `BAKER`) |
| `name` | `string` | Có | Tên hiển thị (Ví dụ: "Quản trị viên", "Nhân viên bán hàng") |
| `description` | `string?` | Không | Mô tả chức trách của vai trò |
| `isSystemRole` | `boolean` | Có | Cờ đánh dấu vai trò mặc định của hệ thống. Nếu `true`, **không thể xóa** |
| `permissions` | `List<PermissionsDto>` / `List<string>` | Không | Danh sách các quyền hạn được gán cho vai trò này |
| `createdAt` | `DateTime?` | Không | Thời điểm tạo bản ghi (UTC) |
| `createdBy` | `string?` | Không | Người tạo bản ghi |
| `updatedAt` | `DateTime?` | Không | Thời điểm cập nhật cuối cùng (UTC) |
| `updatedBy` | `string?` | Không | Người cập nhật cuối cùng |

### 2.2. Thực thể `Permission` (Quyền hạn)

| Trường | Kiểu dữ liệu | Bắt buộc | Mô tả |
|:---|:---:|:---:|:---|
| `id` | `int` | Có | Khóa chính tự tăng |
| `code` | `string` | Có | Mã quyền chuẩn dạng `Module.Action` (Unique, ví dụ: `Role.View`) |
| `name` | `string` | Có | Tên hiển thị của quyền (Ví dụ: "Xem vai trò") |
| `description` | `string?` | Không | Mô tả chi tiết quyền hạn |

### 2.3. Bảng phân nhóm Permission mặc định trong hệ thống

Hệ thống đã cấu hình sẵn cây quyền hạn chuẩn sau đây để CMS vẽ **Ma trận phân quyền (Permission Matrix)**:

| Module / Nhóm | Mã quyền (`Code`) | Tên hiển thị (`Name`) | Mô tả hành động trên CMS |
|:---|:---|:---|:---|
| **Người dùng (User)** | `User.View` | Xem người dùng | Xem danh sách & chi tiết tài khoản |
| | `User.Create` | Thêm người dùng | Tạo tài khoản nhân viên / quản trị mới |
| | `User.Update` | Chỉnh sửa người dùng | Đổi thông tin, gán vai trò (`Assign Roles`) |
| | `User.Delete` | Xóa người dùng | Khóa / Xóa tài khoản người dùng |
| **Vai trò (Role)** | `Role.View` | Xem role | Xem danh sách & chi tiết cấu hình vai trò |
| | `Role.Create` | Thêm role | Tạo mới vai trò và gán quyền |
| | `Role.Update` | Chỉnh sửa role | Cập nhật thông tin & thay đổi danh sách quyền |
| | `Role.Delete` | Xóa role | Xóa vai trò tùy chỉnh (trừ system role) |
| **Quyền hạn (Permission)** | `Permission.View` | Xem quyền | Xem danh mục toàn bộ quyền hạn hệ thống |
| | `Permission.Update` | Chỉnh sửa quyền | Cập nhật tên / mô tả quyền hạn |
| **Sản phẩm (Product)** | `Product.View` | Xem sản phẩm | Xem danh sách & chi tiết bánh / sản phẩm |
| | `Product.Create` | Thêm sản phẩm | Tạo mới sản phẩm |
| | `Product.Update` | Chỉnh sửa sản phẩm | Cập nhật thông tin, giá, nguyên liệu |
| | `Product.Delete` | Xóa sản phẩm | Xóa / Ngừng kinh doanh sản phẩm |
| **Danh mục (Category)** | `Category.View` | Xem danh mục | Xem danh mục bánh |
| | `Category.Create` | Thêm danh mục | Tạo danh mục mới |
| | `Category.Update` | Chỉnh sửa danh mục | Cập nhật thông tin danh mục |
| | `Category.Delete` | Xóa danh mục | Xóa danh mục |
| **Banner quảng cáo (Banner)** | `Banner.View` | Xem banner | Xem danh sách & cấu hình banner |
| | `Banner.Create` | Thêm banner | Tạo mới banner |
| | `Banner.Update` | Chỉnh sửa banner | Sửa nội dung, đổi trạng thái On/Off |
| | `Banner.Delete` | Xóa banner | Xóa banner quảng cáo |

### 2.4. Các vai trò hệ thống mặc định (`System Roles`)

| Role Code | Tên vai trò | IsSystemRole | Mô tả chức trách |
|:---|:---|:---:|:---|
| `ADMIN` | Administrator | `true` | Quản trị viên cấp cao nhất - có toàn quyền mọi module, **không thể xóa** |
| `USER` | User | `false` | Khách hàng / Người dùng bình thường |

---

## 3. Danh sách Endpoints Quản lý Vai trò (Roles)

### 3.1. Lấy danh sách Vai trò (Get Roles List)

Dùng để render bảng danh sách vai trò tại CMS (bao gồm `isSystemRole`, ngày tạo `createdAt` và ngày cập nhật `updatedAt`).

- **Endpoint:** `GET /api/v1/roles`
- **Authentication:** `Bearer Token`
- **Permission Required:** `Role.View`

#### Query Parameters:
| Tham số | Kiểu dữ liệu | Bắt buộc | Mặc định | Mô tả |
|:---|:---:|:---:|:---|:---|
| `search` | `string` | Không | - | Tìm kiếm gần đúng theo Tên hoặc Mã vai trò (`Name`, `Code`) |
| `page` | `int` | Không | `1` | Số trang (bắt đầu từ 1) |
| `pageSize` | `int` | Không | `10` | Số lượng phần tử mỗi trang |

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "items": [
      {
        "id": 1,
        "code": "ADMIN",
        "name": "Administrator",
        "description": "Quản trị viên toàn quyền hệ thống",
        "isSystemRole": true,
        "createdAt": "2026-10-21T00:00:00Z",
        "updatedAt": "2026-10-21T08:00:00Z",
        "permissions": [
          "User.View",
          "User.Create",
          "User.Update",
          "User.Delete",
          "Role.View",
          "Role.Create",
          "Role.Update",
          "Role.Delete",
          "Product.View",
          "Product.Create",
          "Product.Update",
          "Product.Delete",
          "Category.View",
          "Category.Create",
          "Category.Update",
          "Category.Delete",
          "Banner.View",
          "Banner.Create",
          "Banner.Update",
          "Banner.Delete"
        ]
      },
      {
        "id": 2,
        "code": "STAFF",
        "name": "Nhân viên bán hàng",
        "description": "Chỉ có quyền xem và cập nhật đơn hàng, sản phẩm",
        "isSystemRole": false,
        "createdAt": "2026-10-21T10:00:00Z",
        "updatedAt": "2026-10-21T11:30:00Z",
        "permissions": [
          "Product.View",
          "Category.View",
          "Banner.View"
        ]
      }
    ],
    "page": 1,
    "pageSize": 10,
    "totalCount": 2,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  },
  "errors": null,
  "timestamp": "2026-10-21T14:35:00Z"
}
```

---

### 3.2. Lấy chi tiết Vai trò theo Id (Get Role Detail)

Dùng khi bấm "Xem chi tiết" hoặc mở form "Chỉnh sửa vai trò" để nạp dữ liệu quyền hạn đã chọn.

- **Endpoint:** `GET /api/v1/roles/{id}`
- **Authentication:** `Bearer Token`
- **Permission Required:** `Role.View`
- **Path Parameter:**
  - `id` (`int`, Bắt buộc): Id của vai trò cần xem.

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "id": 2,
    "code": "STAFF",
    "name": "Nhân viên bán hàng",
    "description": "Chỉ có quyền xem sản phẩm và danh mục",
    "isSystemRole": false,
    "createdAt": "2026-10-21T10:00:00Z",
    "createdBy": "admin@gmail.com",
    "updatedAt": "2026-10-21T11:30:00Z",
    "updatedBy": "admin@gmail.com",
    "permissions": [
      {
        "id": 5,
        "code": "Product.View",
        "name": "Xem sản phẩm",
        "description": "Cho phép truy cập xem danh sách sản phẩm"
      },
      {
        "id": 9,
        "code": "Category.View",
        "name": "Xem danh mục",
        "description": "Cho phép xem cây danh mục sản phẩm"
      }
    ]
  },
  "errors": null,
  "timestamp": "2026-10-21T14:36:00Z"
}
```

#### Response `404 Not Found`:
```json
{
  "isSuccess": false,
  "statusCode": 404,
  "message": "Role not found",
  "data": null,
  "errors": [
    {
      "code": "ROLE_NOT_FOUND",
      "description": "Role not found"
    }
  ],
  "timestamp": "2026-10-21T14:36:00Z"
}
```

---

### 3.3. Tạo mới Vai trò (Create Role)

Dùng khi thêm vai trò mới trên CMS. Khi tạo mới, có thể gán sẵn danh sách quyền thông qua `permissionIds`.

- **Endpoint:** `POST /api/v1/roles`
- **Authentication:** `Bearer Token`
- **Permission Required:** `Role.Create`

#### Request Body (`application/json`):
```json
{
  "code": "MARKETING",
  "name": "Nhân viên Marketing",
  "description": "Phụ trách quản lý Banner và Khuyến mãi",
  "permissionIds": [17, 18, 19, 20]
}
```

#### Quy tắc Validation & Nghiệp vụ:
- `code`: Bắt buộc, tối đa 100 ký tự. Hệ thống tự động chuyển thành chữ in hoa (`Trim().ToUpper()`). Mã vai trò là **duy nhất** (không được trùng).
- `name`: Bắt buộc, tối đa 100 ký tự.
- `description`: Không bắt buộc (tối đa 500 ký tự trong DB).
- `permissionIds`: Danh sách các ID quyền hạn cần gán. Tự động loại bỏ trùng lặp (`Distinct`).
- Vai trò tạo mới qua API mặc định có `isSystemRole = false`.

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "roleId": 3
  },
  "errors": null,
  "timestamp": "2026-10-21T14:38:00Z"
}
```

#### Response `400 Bad Request` (Trùng mã code):
```json
{
  "isSuccess": false,
  "statusCode": 400,
  "message": "Role code already exists",
  "data": null,
  "errors": [
    {
      "code": "ROLE_ALREADY_EXISTS",
      "description": "Role code already exists"
    }
  ],
  "timestamp": "2026-10-21T14:38:00Z"
}
```

---

### 3.4. Cập nhật Vai trò & Quyền hạn (Update Role)

Dùng khi chỉnh sửa tên, mô tả và cập nhật lại danh sách quyền của một vai trò.

- **Endpoint:** `PUT /api/v1/roles/{id}`
- **Authentication:** `Bearer Token`
- **Permission Required:** `Role.Update`
- **Path Parameter:**
  - `id` (`int`, Bắt buộc): Id vai trò cần cập nhật.

#### Request Body (`application/json`):
> **Ghi chú:** CMS có thể gửi danh sách quyền bằng mảng ID (`permissionIds`) hoặc mảng Code (`permissionCodes`), hoặc kết hợp cả hai. Hệ thống sẽ gộp và loại bỏ trùng lặp.

```json
{
  "name": "Chuyên viên Marketing & Nội dung",
  "description": "Quản lý Banner, Blog và Khuyến mãi cửa hàng",
  "permissionIds": [17, 18, 19, 20],
  "permissionCodes": [
    "Product.View"
  ]
}
```

#### Quy tắc Validation & Nghiệp vụ:
- `name`: Bắt buộc, tối đa 100 ký tự.
- `description`: Bắt buộc, tối đa 100 ký tự (theo Rule Validator).
- `permissionIds` / `permissionCodes`: Nếu truyền lên, hệ thống sẽ **ghi đè toàn bộ** quyền hiện tại của Role bằng danh sách mới. Nếu muốn xóa hết quyền thì truyền mảng rỗng `[]`.
- Không thể thay đổi trường `code` sau khi đã tạo để tránh ảnh hưởng đến các logic mapping cứng.

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "roleId": 3
  },
  "errors": null,
  "timestamp": "2026-10-21T14:40:00Z"
}
```

---

### 3.5. Xóa Vai trò (Delete Role)

- **Endpoint:** `DELETE /api/v1/roles/{id}`
- **Authentication:** `Bearer Token`
- **Permission Required:** `Role.Delete`
- **Path Parameter:**
  - `id` (`int`, Bắt buộc): Id vai trò cần xóa.

#### Quy tắc Nghiệp vụ:
- **Tuyệt đối không được xóa Vai trò hệ thống (`isSystemRole == true`)** như `ADMIN`.
- Khi xóa Role, toàn bộ quan hệ trong bảng `RolePermissions` và `UserRoles` liên kết với vai trò này sẽ tự động xóa (Cascade Delete).

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": null,
  "errors": null,
  "timestamp": "2026-10-21T14:42:00Z"
}
```

#### Response `409 Conflict` (Khi cố tình xóa System Role):
```json
{
  "isSuccess": false,
  "statusCode": 409,
  "message": "Can not delete system role",
  "data": null,
  "errors": [
    {
      "code": "CONFLICT",
      "description": "Can not delete system role"
    }
  ],
  "timestamp": "2026-10-21T14:42:00Z"
}
```

---

## 4. Danh sách Endpoints Quản lý Quyền hạn (Permissions)

### 4.1. Lấy danh sách Quyền hạn (Get Permissions List)

Dùng để lấy danh mục các quyền hạn để hiển thị lên **Checkbox / Ma trận phân quyền** tại màn hình tạo hoặc sửa Role.

- **Endpoint:** `GET /api/v1/permissions`
- **Authentication:** `Bearer Token`
- **Permission Required:** `Permission.View`

#### Query Parameters:
| Tham số | Kiểu dữ liệu | Bắt buộc | Mặc định | Mô tả |
|:---|:---:|:---:|:---|:---|
| `search` | `string` | Không | - | Tìm kiếm theo tên hoặc mã quyền (`Code`, `Name`) |
| `roleId` | `int` | Không | - | Lọc danh sách quyền thuộc về một vai trò cụ thể |
| `page` | `int` | Không | `1` | Số trang |
| `pageSize` | `int` | Không | `10` | Số lượng phần tử mỗi trang (CMS có thể truyền `100` để lấy toàn bộ quyền hiển thị lên Form) |

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "items": [
      {
        "id": 1,
        "code": "User.View",
        "name": "Xem người dùng",
        "description": null
      },
      {
        "id": 2,
        "code": "User.Create",
        "name": "Thêm người dùng",
        "description": null
      },
      {
        "id": 3,
        "code": "User.Update",
        "name": "Chỉnh sửa người dùng",
        "description": null
      },
      {
        "id": 4,
        "code": "User.Delete",
        "name": "Xóa người dùng",
        "description": null
      },
      {
        "id": 5,
        "code": "Product.View",
        "name": "Xem sản phẩm",
        "description": null
      },
      {
        "id": 6,
        "code": "Product.Create",
        "name": "Thêm sản phẩm",
        "description": null
      },
      {
        "id": 7,
        "code": "Product.Update",
        "name": "Chỉnh sửa sản phẩm",
        "description": null
      },
      {
        "id": 8,
        "code": "Product.Delete",
        "name": "Xóa sản phẩm",
        "description": null
      },
      {
        "id": 9,
        "code": "Role.View",
        "name": "Xem role",
        "description": null
      },
      {
        "id": 10,
        "code": "Role.Create",
        "name": "Thêm role",
        "description": null
      },
      {
        "id": 11,
        "code": "Role.Update",
        "name": "Chỉnh sửa role",
        "description": null
      },
      {
        "id": 12,
        "code": "Role.Delete",
        "name": "Xóa role",
        "description": null
      }
    ],
    "page": 1,
    "pageSize": 50,
    "totalCount": 20,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  },
  "errors": null,
  "timestamp": "2026-10-21T14:45:00Z"
}
```

---

## 5. API Phân vai trò cho Người dùng (User Role Assignment)

### 5.1. Gán vai trò cho Người dùng (Assign Roles to User)

Dùng trong màn hình Quản lý tài khoản (User Management) khi người quản trị muốn gán hoặc thay đổi vai trò của một nhân sự.

- **Endpoint:** `PUT /api/v1/users/{userId}/roles`
- **Authentication:** `Bearer Token`
- **Permission Required:** `User.Update`
- **Path Parameter:**
  - `userId` (`Guid`, Bắt buộc): Id tài khoản người dùng cần phân vai trò.

#### Request Body (`application/json`):
> Hỗ trợ truyền theo `roleIds` hoặc `roleCodes` (hoặc cả hai). Cơ chế là **ghi đè** (`Clear()` và gán lại toàn bộ).

```json
{
  "roleIds": [2, 3],
  "roleCodes": [
    "STAFF"
  ]
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
  "timestamp": "2026-10-21T14:48:00Z"
}
```

#### Response `404 Not Found` (Người dùng không tồn tại):
```json
{
  "isSuccess": false,
  "statusCode": 404,
  "message": "User not found",
  "data": null,
  "errors": [
    {
      "code": "USER_NOT_FOUND",
      "description": "User not found"
    }
  ],
  "timestamp": "2026-10-21T14:48:00Z"
}
```

---

### 5.2. Lấy danh sách Người dùng kèm Vai trò

Dùng để hiển thị bảng quản lý nhân sự & vai trò.

- **Endpoint:** `GET /api/v1/users`
- **Authentication:** `Bearer Token`
- **Permission Required:** `User.View`

#### Query Parameters:
| Tham số | Kiểu dữ liệu | Bắt buộc | Mặc định | Mô tả |
|:---|:---:|:---:|:---|:---|
| `search` | `string` | Không | - | Tìm theo email hoặc họ tên |
| `roleId` | `int` | Không | - | Lọc theo ID vai trò |
| `roleCode` | `string` | Không | - | Lọc theo mã vai trò (ví dụ: `ADMIN`, `STAFF`) |
| `isDeleted` | `boolean` | Không | - | Lọc trạng thái xóa mềm |
| `page` | `int` | Không | `1` | Số trang |
| `pageSize` | `int` | Không | `10` | Số bản ghi trên trang |

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "items": [
      {
        "id": "e9b925b4-93e1-4c12-8809-5a63eb4c7182",
        "fullName": "Nguyễn Văn Admin",
        "email": "admin@gmail.com",
        "phoneNumber": "0987654321",
        "avatar": "https://cdn.example.com/avatars/admin.png",
        "isActive": true,
        "createdAt": "2026-10-21T00:00:00Z",
        "isDeleted": false,
        "roleCode": [
          "ADMIN"
        ],
        "roleName": [
          "Administrator"
        ]
      },
      {
        "id": "b11a43d9-6052-4f10-94e8-f9b6b797b5e4",
        "fullName": "Trần Thị Thu Ngân",
        "email": "staff01@gmail.com",
        "phoneNumber": "0912345678",
        "avatar": null,
        "isActive": true,
        "createdAt": "2026-10-21T08:00:00Z",
        "isDeleted": false,
        "roleCode": [
          "STAFF"
        ],
        "roleName": [
          "Nhân viên bán hàng"
        ]
      }
    ],
    "page": 1,
    "pageSize": 10,
    "totalCount": 2,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  },
  "errors": null,
  "timestamp": "2026-10-21T14:50:00Z"
}
```

---

## 6. API Lấy thông tin Quyền của User hiện tại (Frontend Permission Guard)

CMS frontend cần gọi API này ngay sau khi đăng nhập thành công hoặc khi refresh trang để lưu `roles` và `permissions` vào State Manager (Pinia / Redux / Zustand), từ đó điều khiển hiển thị Sidebar, Route Guards và các nút hành động (Add/Edit/Delete).

- **Endpoint:** `GET /api/v1/users/me`
- **Authentication:** `Bearer Token`

#### Response `200 OK`:
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": {
    "id": "e9b925b4-93e1-4c12-8809-5a63eb4c7182",
    "fullName": "Nguyễn Văn Admin",
    "email": "admin@gmail.com",
    "phoneNumber": "0987654321",
    "avatar": "https://cdn.example.com/avatars/admin.png",
    "isActive": true,
    "sex": true,
    "lastLoginAt": "2026-10-21T14:00:00Z",
    "createdAt": "2026-10-21T00:00:00Z",
    "updatedAt": "2026-10-21T14:00:00Z",
    "isDeleted": false,
    "deletedAt": null,
    "roles": [
      "ADMIN"
    ],
    "permissions": [
      "User.View",
      "User.Create",
      "User.Update",
      "User.Delete",
      "Product.View",
      "Product.Create",
      "Product.Update",
      "Product.Delete",
      "Category.View",
      "Category.Create",
      "Category.Update",
      "Category.Delete",
      "Role.View",
      "Role.Create",
      "Role.Update",
      "Role.Delete",
      "Permission.View",
      "Permission.Update",
      "Banner.View",
      "Banner.Create",
      "Banner.Update",
      "Banner.Delete"
    ]
  },
  "errors": null,
  "timestamp": "2026-10-21T14:52:00Z"
}
```

---

## 7. Bảng Tổng Hợp Mã Lỗi (Error Codes)

| Mã lỗi (`ErrorCode`) | HTTP Status | Thông điệp (`ErrorMessage`) | Nguyên nhân & Hướng dẫn xử lý |
|:---|:---:|:---|:---|
| `ROLE_NOT_FOUND` | `404` | Role not found | Id vai trò không tồn tại trong DB |
| `ROLE_ALREADY_EXISTS` | `400` | Role code already exists | Mã vai trò (`code`) đã có sẵn, cần nhập mã khác |
| `USER_NOT_FOUND` | `404` | User not found | Id người dùng không tồn tại |
| `CONFLICT` | `409` | Can not delete system role | Cố tình xóa vai trò hệ thống (`isSystemRole = true`) |
| `FORBIDDEN` | `403` | Forbidden / Không có quyền truy cập | Tài khoản hiện tại thiếu Permission yêu cầu cho action này |
| `UNAUTHORIZED` | `401` | Unauthorized | Token hết hạn hoặc không hợp lệ |
| `VALIDATION_ERROR` | `400` | Tùy trường (Name/Code is required) | Dữ liệu form không thỏa mãn validate (trống, quá độ dài...) |

---

## 8. Hướng dẫn Tích hợp & Thiết kế UI/UX trên CMS

### 8.1. Màn hình Quản lý Vai trò (Roles Table View)
- **Các cột trên bảng:**
  1. **Mã vai trò (`Code`)**: Hiển thị in hoa dạng Tag/Badge (ví dụ: `<Tag color="blue">ADMIN</Tag>`).
  2. **Tên vai trò (`Name`)**: Tên dễ hiểu (ví dụ: Quản trị viên).
  3. **Mô tả (`Description`)**: Tóm tắt phạm vi quyền hạn.
  4. **Loại vai trò**:
     - Vai trò hệ thống (`isSystemRole = true`): Tag đỏ / khóa `[Hệ thống]` (không cho sửa mã, không cho xóa).
     - Vai trò tùy biến (`isSystemRole = false`): Tag xanh `[Tùy chỉnh]`.
  5. **Ngày tạo (`createdAt`) & Ngày cập nhật (`updatedAt`)**: Định dạng ngày giờ hiển thị trên bảng quản lý (ví dụ: `DD/MM/YYYY HH:mm`).
  6. **Quyền hạn gán kèm**: Hiển thị 3-4 Tags quyền hạn đầu tiên và Tag `+X quyền khác`. Khi hover hiển thị Tooltip/Popover danh sách đầy đủ.
  7. **Hành động**:
     - Nút "Sửa" (chỉ hiển thị nếu user có `Role.Update`).
     - Nút "Xóa" (disable hoặc ẩn nếu là System Role `isSystemRole = true`; chỉ hiển thị nếu có `Role.Delete`).

### 8.2. Ma trận Phân quyền (Permission Matrix Component)
Khi tạo mới hoặc sửa Role, nên tổ chức giao diện dưới dạng **Bảng ma trận nhóm theo Module**:

```
+-----------------------------------------------------------------------------------+
| Module          | Xem (View) | Thêm (Create) | Sửa (Update) | Xóa (Delete) | Tất cả |
+-----------------------------------------------------------------------------------+
| [x] Người dùng  |    [x]     |      [x]      |     [x]      |     [x]      |  [x]   |
| [ ] Sản phẩm    |    [x]     |      [ ]      |     [ ]      |     [ ]      |  [ ]   |
| [ ] Danh mục    |    [x]     |      [ ]      |     [ ]      |     [ ]      |  [ ]   |
| [ ] Banner      |    [x]     |      [x]      |     [x]      |     [ ]      |  [ ]   |
| [ ] Vai trò     |    [ ]     |      [ ]      |     [ ]      |     [ ]      |  [ ]   |
+-----------------------------------------------------------------------------------+
```

- **Tính năng UX tối ưu:**
  - Nút **"Chọn tất cả (Select All)"** toàn hệ thống.
  - Checkbox **"Chọn tất cả theo hàng (Row Select All)"** cho từng Module.
  - Phụ thuộc nghiệp vụ: Nếu check `Create`, `Update`, hoặc `Delete` thì tự động auto-check `View`.

### 8.3. Màn hình Quản lý Người dùng (Gán vai trò)
- Khi mở modal thông tin User:
  - Trường **"Vai trò"**: Sử dụng component `Select Multiple` (hoặc `Checkbox Group`) nạp dữ liệu từ `GET /api/v1/roles`.
  - Khi lưu: Gửi mảng `roleIds` lên `PUT /api/v1/users/{userId}/roles`.

### 8.4. Cơ chế Phân quyền ở Frontend (Frontend Authorization)
- **State:** Lưu danh sách `user.permissions: string[]` lấy từ `GET /api/v1/users/me`.
- **Helper function:**
  ```typescript
  export const hasPermission = (permission: string): boolean => {
    const userPermissions = authStore.user?.permissions || [];
    const userRoles = authStore.user?.roles || [];
    // Nếu là ADMIN thì có toàn quyền
    if (userRoles.includes('ADMIN')) return true;
    return userPermissions.includes(permission);
  };
  ```
- **Custom Directive (Vue / React / Angular):**
  - Trong Vue: `v-permission="'Role.Create'"` (nếu không có quyền thì tự động `el.parentNode.removeChild(el)`).
  - Trong React: `<Can permission="Role.Create"><Button>Thêm vai trò</Button></Can>`.
- **Vue Router / React Router Guard:**
  - Kiểm tra `meta.permission` trước khi vào Route:
  ```typescript
  router.beforeEach((to, from, next) => {
    if (to.meta.permission && !hasPermission(to.meta.permission)) {
      next({ name: 'Forbidden' }); // chuyển về trang 403
    } else {
      next();
    }
  });
  ```
