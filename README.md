# B3cBonsai - Website Thương mại điện tử

Đây là một dự án website thương mại điện tử chuyên về bán cây cảnh bonsai, được xây dựng trên nền tảng ASP.NET Core.

## 🚀 Công nghệ sử dụng

- **Backend:**
  - **Ngôn ngữ:** C#
  - **Framework:** ASP.NET Core 8 (MVC)
  - **ORM:** Entity Framework Core 8
- **Frontend:**
  - Razor Pages, HTML, CSS, JavaScript
  - **Thư viện:** Bootstrap, jQuery
- **Cơ sở dữ liệu:**
  - **Mặc định:** PostgreSQL
  - **Hỗ trợ:** Microsoft SQL Server
- **Containerization:**
  - Docker, Docker Compose
- **Xác thực:**
  - ASP.NET Core Identity
  - Đăng nhập qua Google, Facebook
- **Tích hợp bên thứ ba:**
  - **Thanh toán:** VnPay
  - **Thông báo:** Telegram Bot
  - **Email:** SMTP (Gmail)
  - **Lưu trữ ảnh:** Local Storage hoặc Cloudinary

## ⚙️ Hướng dẫn Cài đặt và Vận hành

Dưới đây là hai phương pháp để chạy dự án. Phương pháp sử dụng Docker được khuyến khích vì tính đơn giản và đã bao gồm sẵn cơ sở dữ liệu.

### 1. Yêu cầu tiên quyết

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://docs.docker.com/get-docker/) và [Docker Compose](https://docs.docker.com/compose/install/) (Bắt buộc cho phương pháp Docker)

### 2. Cấu hình chung

Cả hai phương pháp đều yêu cầu bạn cấu hình các khóa API và thông tin nhạy cảm.

1.  **Tải mã nguồn:**
    ```bash
    git clone https://github.com/n0bcode/B3cBonsai.git
    cd B3cBonsai
    ```
2.  **Điền các khóa API:**
    Dự án yêu cầu một số khóa API cho các dịch vụ bên thứ ba (Google, Facebook, VnPay, v.v.). Hãy làm theo hướng dẫn chi tiết tại file `docs/api_keys_guide.md` để lấy và điền các giá trị này vào file `B3cBonsaiWeb/appsettings.json`.

3.  **Cấu hình Lưu trữ Ảnh:**
    Bạn có thể chọn giữa lưu trữ ảnh trên máy cục bộ hoặc sử dụng dịch vụ Cloudinary.

    - Mở file `B3cBonsaiWeb/appsettings.json`.
    - Để sử dụng **lưu trữ cục bộ**, đặt cờ `UseCloudinaryStorage` thành `false`:
      ```json
      "UseCloudinaryStorage": false,
      ```
    - Để sử dụng **Cloudinary**, đặt cờ `UseCloudinaryStorage` thành `true` và điền thông tin tài khoản của bạn vào mục `CloudinarySettings`:
      ```json
      "UseCloudinaryStorage": true,
      "CloudinarySettings": {
        "CloudName": "YOUR_CLOUD_NAME",
        "ApiKey": "YOUR_API_KEY",
        "ApiSecret": "YOUR_API_SECRET"
      }
      ```

---

### Phương pháp 1: Chạy với Docker Compose (Khuyến nghị)

Phương pháp này sẽ tự động dựng và chạy cả web app và cơ sở dữ liệu PostgreSQL trong các container riêng biệt.

1.  **Cấu hình Docker:**

    - **Mật khẩu DB:** Tạo một file mới tên là `db_password.txt` trong thư mục gốc của dự án và điền mật khẩu bạn muốn cho PostgreSQL vào đó.
    - **Compose File:** Mở file `compose.yaml` và bỏ comment (xóa dấu `#`) cho toàn bộ mục `db`.
    - **Connection String:** Trong file `B3cBonsaiWeb/appsettings.json`, đảm bảo cờ `UsePostgreSql` là `true` và chuỗi `PostgreConnectString` được cấu hình để trỏ đến service `db` của Docker:
      ```json
      "UsePostgreSql": true,
      "ConnectionStrings": {
        "PostgreConnectString": "Host=db;Database=b3cbonsai_db;Username=postgres;Password=YOUR_DB_PASSWORD",
        //...
      },
      ```
      **Lưu ý:** Thay thế `YOUR_DB_PASSWORD` bằng mật khẩu bạn đã đặt trong `db_password.txt`. Tên `Host` phải là `db` để trỏ đến service trong `compose.yaml`.

2.  **Chạy ứng dụng:**
    Mở terminal trong thư mục gốc và chạy lệnh:

    ```bash
    docker compose up --build
    ```

    Lần đầu tiên có thể mất vài phút để tải và build các images. Sau khi hoàn tất, ứng dụng sẽ có thể truy cập tại `http://localhost:8080`.

3.  **Triển khai ứng dụng của bạn lên đám mây**
    Đầu tiên, xây dựng Image của bạn, e.g.:
    `docker build -t myapp .`
    .Nếu đám mây của bạn sử dụng kiến ​​trúc CPU khác với sự phát triển của bạn
    Máy (ví dụ: bạn đang ở trên Mac M1 và nhà cung cấp đám mây của bạn là AMD64),
    Bạn sẽ muốn xây dựng Image cho nền tảng đó, ví dụ:
    `docker build --platform=linux/amd64 -t myapp .`.

Sau đó, đẩy nó vào sổ đăng ký của bạn, e.g. `docker push myregistry.com/myapp`.

Tham khảo ý kiến ​​của Docker [bắt đầu] (https://docs.docker.com/go/get-started-sharing/)
Tài liệu để biết thêm chi tiết về xây dựng và đẩy.

---

### Phương pháp 2: Chạy trên máy cục bộ (Không dùng Docker)

Phương pháp này yêu cầu bạn phải tự cài đặt và quản lý cơ sở dữ liệu (PostgreSQL hoặc SQL Server) trên máy của mình.

1.  **Cài đặt Cơ sở dữ liệu:**

    - Cài đặt [PostgreSQL](https://www.postgresql.org/download/) hoặc [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).
    - Tạo một database trống cho dự án.

2.  **Cấu hình `appsettings.json`:**

    - Mở file `B3cBonsaiWeb/appsettings.json`.
    - **Chọn Provider:**
      - Để dùng PostgreSQL, đặt `UsePostgreSql` thành `true`.
      - Để dùng SQL Server, đặt `UsePostgreSql` thành `false`.
    - **Điền Connection String:** Cung cấp chuỗi kết nối chính xác tới cơ sở dữ liệu bạn đã tạo.

3.  **Chạy ứng dụng:**
    - **Restore dependencies:**
      ```bash
      dotnet restore
      ```
    - **Áp dụng Database Migration:** Lệnh này sẽ tạo các bảng trong database của bạn dựa trên model.
      ```bash
      dotnet ef database update --project B3cBonsai.DataAccess
      ```
    - **Khởi động web app:**
      ```bash
      dotnet run --project B3cBonsaiWeb
      ```
      Ứng dụng sẽ chạy và có thể truy cập tại địa chỉ được hiển thị trong terminal (thường là `https://localhost:7020`).

## 📂 Cấu trúc Dự án

Dự án được tổ chức theo kiến trúc Clean Architecture với các project riêng biệt:

- `B3cBonsai.DataAccess`: Chịu trách nhiệm truy cập dữ liệu, sử dụng Repository Pattern và Entity Framework Core.
- `B3cBonsai.Models`: Chứa các data models và ViewModels.
- `B3cBonsai.Utility`: Chứa các lớp tiện ích như gửi email, hằng số, v.v.
- `B3cBonsaiWeb`: Dự án chính ASP.NET Core MVC, chứa Controllers, Views, và các tài nguyên frontend.

## 🚀 Deploy lên Heroku

Quy trình này sử dụng Heroku Buildpack cho .NET, không yêu cầu `Dockerfile` hay `Procfile`.

### 1. Yêu cầu

- Đã cài đặt [Heroku CLI](https://devcenter.heroku.com/articles/heroku-cli).
- Đã có tài khoản Heroku và đã đăng nhập (`heroku login`).

### 2. Thiết lập ứng dụng Heroku

1.  **Tạo ứng dụng Heroku (nếu bạn chưa có)**:

    ```bash
    # Tạo một ứng dụng mới với tên do bạn chọn
    heroku create b3c
    ```

2.  **Kết nối với repo Git hiện tại:**
    Nếu bạn đã có ứng dụng trên Heroku, hãy liên kết nó với repo của bạn:

    ```bash
    heroku git:remote -a b3c
    ```

3.  **Thêm Add-on PostgreSQL:** (Không cần nếu bạn có sẵn kết nối với bên thứ 3)
    Heroku sẽ tự động cung cấp một database và tạo biến môi trường `DATABASE_URL`.
    ```bash
    heroku addons:create heroku-postgresql:hobby-dev
    ```

### 3. Cấu hình Biến Môi trường

Đây là bước quan trọng nhất. Ứng dụng của bạn cần các khóa bí mật để hoạt động. Hãy vào Heroku Dashboard của bạn > `Settings` > `Config Vars`.

**Bấm "Reveal Config Vars" và thêm các biến sau:**

| Key                                               | Value                                                                  | Bắt buộc? |
| ------------------------------------------------- | ---------------------------------------------------------------------- | :-------: |
| `ASPNETCORE_ENVIRONMENT`                          | `Production`                                                           |    ✅     |
| `ASPNETCORE_UsePostgreSql`                        | `true`                                                                 |    ✅     |
| `ConnectionStrings__PostgreConnectString`         | `Host=link;Database=name;Username=usname;Password=pss;SslMode=Require` |    ✅     |
| `ASPNETCORE_EmailSettings__Password`              | `your_gmail_app_password`                                              |    ✅     |
| `ASPNETCORE_EmailSettings__Email`                 | `your_smtp_email@gmail.com`                                            |    ✅     |
| `ASPNETCORE_Authentication__Google__ClientId`     | `your_google_client_id`                                                |    Tùy    |
| `ASPNETCORE_Authentication__Google__ClientSecret` | `your_google_client_secret`                                            |    Tùy    |
| `ASPNETCORE_VnPay__TmnCode`                       | `your_vnpay_tmn_code`                                                  |    Tùy    |
| `ASPNETCORE_VnPay__HashSecret`                    | `your_vnpay_hash_secret`                                               |    Tùy    |
| `ASPNETCORE_CloudinarySettings__CloudName`        | `your_cloudinary_cloud_name`                                           |    Tùy    |
| `ASPNETCORE_CloudinarySettings__ApiKey`           | `your_cloudinary_api_key`                                              |    Tùy    |
| `ASPNETCORE_CloudinarySettings__ApiSecret`        | `your_cloudinary_api_secret`                                           |    Tùy    |

**Lưu ý:**

- `ASPNETCORE_` là prefix Heroku dùng cho các biến môi trường. Dấu `__` (hai dấu gạch dưới) được dùng để thay thế cho dấu `:` trong file `appsettings.json`.
- Để lấy các giá trị, hãy tham khảo file `docs/api_keys_guide.md`.

### 4. Deploy ứng dụng

Sau khi đã commit các thay đổi của bạn, chỉ cần đẩy branch chính lên Heroku.

```bash
# Đẩy branch 'main' của bạn lên branch 'main' của Heroku
git push heroku main
```

Heroku sẽ tự động build và deploy ứng dụng của bạn.

### 5. Chạy Database Migrations

Sau khi deploy lần đầu (hoặc sau mỗi lần có thay đổi database), bạn phải chạy lệnh sau để cập nhật schema cho database trên Heroku:

```bash
heroku run dotnet ef database update --project B3cBonsai.DataAccess
```

### 6. Kiểm tra và Gỡ lỗi

- **Xem log trực tiếp:**
  ```bash
  heroku logs --tail
  ```
- **Mở ứng dụng:**
  ```bash
  heroku open
  ```
- **Restart ứng dụng nếu cần:**
  ```bash
  heroku restart
  ```

### Tài liệu tham khảo

- [Hướng dẫn .NET của Docker] (https://docs.docker.com/langle/dotnet/)
- [Dotnet-docker] (https://github.com/dotnet/dotnet-docker/tree/main/samples)
- [Heroku .NET Deployment Guide](https://devcenter.heroku.com/articles/deploying-dotnet-applications)
  Kho lưu trữ có nhiều mẫu và tài liệu liên quan.
