# ⚙️ Hướng dẫn Cài đặt và Chạy Local

Tài liệu này hướng dẫn cách thiết lập môi trường phát triển và chạy dự án trên máy cá nhân mà không sử dụng Docker.

## 1. Yêu cầu tiên quyết

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) hoặc [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## 2. Cấu hình chung

Dù chạy theo phương pháp nào, bạn cần cấu hình các thông tin cơ bản sau trong file `B3cBonsaiWeb/appsettings.json`.

### Điền các khóa API
Dự án yêu cầu một số khóa API cho các dịch vụ bên thứ ba. Hãy làm theo hướng dẫn chi tiết tại `docs/api_keys_guide.md` (nếu có) để lấy và điền các giá trị.

### Cấu hình Dữ liệu Mẫu (Seeding)
Mặc định, ứng dụng sẽ tạo dữ liệu mẫu (User, Product, Order...) khi khởi chạy lần đầu.
- **Bật:** `"SeedSampleData": true`
- **Tắt:** `"SeedSampleData": false`

### Cấu hình Lưu trữ Ảnh
Bạn có thể chọn lưu ảnh tại local hoặc trên Cloudinary.

**Cách 1: Lưu trữ cục bộ**
```json
"UseCloudinaryStorage": false
```

**Cách 2: Sử dụng Cloudinary**
```json
"UseCloudinaryStorage": true,
"CloudinarySettings": {
  "CloudName": "YOUR_CLOUD_NAME",
  "ApiKey": "YOUR_API_KEY",
  "ApiSecret": "YOUR_API_SECRET"
}
```

## 3. Chạy ứng dụng trên máy Local

### Bước 1: Cấu hình Database
Mở file `B3cBonsaiWeb/appsettings.json`:
1. **Chọn Provider:**
   - Dùng PostgreSQL: `"UsePostgreSql": true`
   - Dùng SQL Server: `"UsePostgreSql": false`
2. **Connection String:** Điền chuỗi kết nối vào `ConnectionStrings`.

### Bước 2: Chạy lệnh
Mở terminal tại thư mục gốc dự án:

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```
2. **Cập nhật Database (Migrations):**
   ```bash
   dotnet ef database update --project B3cBonsai.DataAccess --startup-project B3cBonsaiWeb
   ```
3. **Khởi động ứng dụng:**
   ```bash
   dotnet run --project B3cBonsaiWeb
   ```

Ứng dụng sẽ chạy tại địa chỉ hiển thị trong terminal (thường là `https://localhost:7020`).
