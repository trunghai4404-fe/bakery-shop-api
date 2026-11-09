# Location Management API Specification

Tài liệu đặc tả toàn bộ API tích hợp danh mục **Địa giới Hành chính Việt Nam (Tỉnh/Thành, Phường/Xã và Quận/Huyện)** phục vụ Frontend (Web Client, Mobile App và CMS Admin Portal) của hệ thống Bakery Shop.

> [!NOTE] Phí giao hàng
> Phí giao hàng (Shipping Fee) sẽ được tính động dựa theo khoảng cách (Latitude / Longitude từ vị trí khách chọn trên bản đồ tới cửa hàng), do đó danh mục địa giới hành chính chỉ đóng vai trò phân cấp địa chỉ chuẩn mực.

---

## 1. Thông tin chung

- **Base URL:** `/api/v1/locations`
- **Content-Type:** `application/json`
- **Cơ chế Cache:** Dữ liệu địa giới hành chính được lưu tại **Redis Cache** với thời gian sống (TTL) 30 ngày. Phản hồi trung bình `< 5ms`.
- **Authentication:**
  - Các API truy vấn danh mục (`GET /provinces`, `GET /wards`, `GET /districts`): **Public (Không yêu cầu Token)**.
  - API đồng bộ dữ liệu (`POST /sync`): **Yêu cầu quyền Admin** (hoặc nội bộ).

### Cấu trúc Response chuẩn của hệ thống (`ApiResponse<T>`)

**Thành công (200 OK / 201 Created):**
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": { ... },
  "errors": null,
  "timestamp": "2026-11-09T09:40:00Z",
  "traceId": "00-xxxxxxxx-01"
}
```

---

## 2. Danh sách APIs

| STT | Phương thức | Endpoint | Mô tả | Query Parameters |
| :---: | :---: | :--- | :--- | :--- |
| 1 | `GET` | `/api/v1/locations/provinces` | Lấy danh sách 63 Tỉnh/Thành phố | Không |
| 2 | `GET` | `/api/v1/locations/wards` | Lấy danh sách Phường/Xã | `?provinceId=...` hoặc `?districtId=...` (Không truyền sẽ lấy tất cả) |
| 3 | `GET` | `/api/v1/locations/districts` | Lấy danh sách Quận/Huyện | `?provinceId=...` (Không truyền sẽ lấy tất cả) |
| 4 | `POST` | `/api/v1/locations/sync` | Kích hoạt đồng bộ dữ liệu từ Open API | Không |

---

## 3. Chi tiết API Integration cho Frontend

### 3.1 Lấy danh sách 63 Tỉnh/Thành phố

- **Endpoint:** `GET /api/v1/locations/provinces`

#### Response: `200 OK`
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": [
    {
      "id": 1,
      "name": "Hà Nội",
      "fullName": "Thành phố Hà Nội",
      "divisionType": "thành phố trung ương",
      "codename": "thanh_pho_ha_noi",
      "phoneCode": 24
    },
    {
      "id": 79,
      "name": "Hồ Chí Minh",
      "fullName": "Thành phố Hồ Chí Minh",
      "divisionType": "thành phố trung ương",
      "codename": "thanh_pho_ho_chi_minh",
      "phoneCode": 28
    }
  ]
}
```

---

### 3.2 Lấy danh sách Phường/Xã (`/wards`)

- **Endpoint:** `GET /api/v1/locations/wards`
- **Query Parameters:**
  - `provinceId` (`number`, optional): Lấy tất cả phường/xã của 1 tỉnh (Ví dụ: `?provinceId=79`).
  - `districtId` (`number`, optional): Lấy phường/xã của 1 quận/huyện.

#### Request Example (Lọc trực tiếp theo Tỉnh):
```http
GET /api/v1/locations/wards?provinceId=79
```

#### Response: `200 OK`
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": [
    {
      "id": 26734,
      "districtId": 760,
      "name": "Bến Nghé",
      "fullName": "Phường Bến Nghé",
      "divisionType": "phường",
      "codename": "phuong_ben_nghe"
    },
    {
      "id": 26737,
      "districtId": 760,
      "name": "Bến Thành",
      "fullName": "Phường Bến Thành",
      "divisionType": "phường",
      "codename": "phuong_ben_thanh"
    }
  ]
}
```

---

### 3.3 Lấy danh sách Quận/Huyện (`/districts`)

- **Endpoint:** `GET /api/v1/locations/districts`
- **Query Parameters:**
  - `provinceId` (`number`, optional): Mã Tỉnh/Thành cần lọc.

#### Request Example:
```http
GET /api/v1/locations/districts?provinceId=79
```

#### Response: `200 OK`
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": "Success.",
  "data": [
    {
      "id": 760,
      "provinceId": 79,
      "name": "1",
      "fullName": "Quận 1",
      "divisionType": "quận",
      "codename": "quan_1"
    }
  ]
}
```

---

## 4. TypeScript Interfaces

```typescript
export interface Province {
  id: number;
  name: string;
  fullName: string;
  divisionType: string;
  codename: string;
  phoneCode: number | null;
}

export interface Ward {
  id: number;
  districtId: number;
  name: string;
  fullName: string;
  divisionType: string;
  codename: string;
}

export interface District {
  id: number;
  provinceId: number;
  name: string;
  fullName: string;
  divisionType: string;
  codename: string;
}
```
