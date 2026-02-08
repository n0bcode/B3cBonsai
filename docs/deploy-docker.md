# 🐳 Hướng dẫn Chạy và Deploy với Docker

Phương pháp này được khuyến khích vì tính đơn giản, môi trường đồng nhất và đã bao gồm sẵn cơ sở dữ liệu PostgreSQL.

## 1. Yêu cầu
- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## 2. Cấu hình Docker Compose

Trước khi chạy, bạn cần thiết lập kết nối giữa App và Database Container.

1. **Tạo mật khẩu DB:**
   Tạo file `db_password.txt` ở thư mục gốc dự án, nhập mật khẩu PostgreSQL bạn muốn vào đó (ví dụ: `MySecretPass123`).

2. **Kiểm tra `compose.yaml`:**
   Mở file `compose.yaml` và đảm bảo service `db` không bị comment.

3. **Cấu hình Connection String (QUAN TRỌNG):**

   Đây là điểm hay gây lỗi nhất.
   - **Bên trong Docker:** Container gọi nhau qua tên service (`Host=db`).
   - **Bên ngoài (Máy của bạn):** Công cụ `dotnet ef` gọi Database qua localhost (`Host=localhost`).

   Để tự động hóa việc này, hãy cấu hình file `B3cBonsaiWeb/appsettings.Development.json` như sau (file này sẽ đè lên file chính khi bạn chạy lệnh trên máy):

   ```json
   {
     "ConnectionStrings": {
       // Dùng localhost để chạy lệnh dotnet ef từ terminal máy tính
       "PostgreConnectString": "Host=localhost;Database=B3cBonsai;Username=postgres;Password=postgres"
     }
   }
   ```
   *Lưu ý: Mật khẩu `postgres` là mặc định trong `compose.yaml`, nếu bạn đổi mật khẩu trong docker compose, hãy cập nhật lại ở đây.*

## 3. Chạy Database Migrations

Khi sử dụng Docker, bạn cần đảm bảo Database đang chạy trước khi thực hiện lệnh Migration từ máy host.

1. **Khởi động Database:**
   ```bash
   docker compose up -d db
   ```

2. **Chạy lệnh Migration:**
   ```bash
   dotnet ef migrations add <TenMigration> --project B3cBonsai.DataAccess --startup-project B3cBonsaiWeb
   dotnet ef database update --project B3cBonsai.DataAccess --startup-project B3cBonsaiWeb
   ```
   *Nếu gặp lỗi `SocketException: Name or service not known`, hãy kiểm tra lại file `appsettings.Development.json` đã set `Host=localhost` chưa.*

## 4. Chạy dự án (Development)

Mở terminal tại thư mục gốc và chạy:

```bash
docker compose up --build
```

- Lần đầu chạy sẽ mất vài phút để build image.
- Truy cập web tại: `http://localhost:8080`

## 5. Build & Push Image (Production)

Để deploy lên cloud (AWS, Azure, DigitalOcean...):

1. **Build Image:**
   ```bash
   # Ví dụ build cho kiến trúc Linux AMD64
   docker build --platform=linux/amd64 -t myapp .
   ```

2. **Push lên Registry:**
   ```bash
   docker push myregistry.com/myapp
   ```

Tham khảo thêm: [Docker Sharing Guide](https://docs.docker.com/go/get-started-sharing/)
