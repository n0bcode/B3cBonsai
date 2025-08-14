# B3cBonsai - Website Thương mại điện tử

Đây là một dự án website thương mại điện tử chuyên về bán cây cảnh bonsai, được xây dựng trên nền tảng ASP.NET Core.

## 🚀 Công nghệ sử dụng

Dự án được xây dựng với các công nghệ hiện đại và phổ biến:

- **Backend:**
  - **Ngôn ngữ:** C#
  - **Framework:** ASP.NET Core 8 (MVC)
  - **ORM:** Entity Framework Core 8
- **Frontend:**
  - Razor Pages, HTML, CSS, JavaScript
  - **Thư viện:** Bootstrap, jQuery
- **Cơ sở dữ liệu:**
  - Microsoft SQL Server
- **Xác thực:**
  - ASP.NET Core Identity
  - Đăng nhập qua Google, Facebook
- **Tích hợp bên thứ ba:**
  - **Thanh toán:** VnPay
  - **Thông báo:** Telegram Bot
  - **Email:** SMTP (Gmail)

## ⚙️ Hướng dẫn Cài đặt và Cấu hình

Để chạy dự án này trên máy cục bộ của bạn, hãy làm theo các bước sau.

### 1. Yêu cầu tiên quyết

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (phiên bản Express hoặc Developer)

### 2. Tải mã nguồn

```bash
git clone https://github.com/n0bcode/B3cBonsai.git
cd B3cBonsai
```

### 3. Cấu hình Secrets

Dự án yêu cầu một số khóa API và chuỗi kết nối để hoạt động. Các tệp cấu hình `B3cBonsaiWeb/appsettings.json` và `B3cBonsaiWeb/appsettings.Development.json` đã được chuẩn bị sẵn các vị trí giữ chỗ (placeholders).

👉 **Xem hướng dẫn chi tiết để lấy và điền các khóa API tại đây: [docs/api_keys_guide.md](./docs/api_keys_guide.md)**

### 4. Cập nhật Cơ sở dữ liệu

Sau khi đã cấu hình chuỗi kết nối, hãy chạy lệnh sau từ thư mục gốc của dự án để áp dụng các migrations và tạo cơ sở dữ liệu:

```bash
dotnet ef database update --project B3cBonsai.DataAccess
```

## ▶️ Cách chạy dự án

Sử dụng lệnh sau để khởi động ứng dụng web:

```bash
dotnet run --project B3cBonsaiWeb
```

Sau đó, truy cập vào `https://localhost:7020` (hoặc cổng tương ứng được hiển thị trong terminal) trên trình duyệt của bạn.

## 📂 Cấu trúc Dự án

Dự án được tổ chức theo kiến trúc Clean Architecture với các project riêng biệt:

- `B3cBonsai.DataAccess`: Chịu trách nhiệm truy cập dữ liệu, sử dụng Repository Pattern và Entity Framework Core.
- `B3cBonsai.Models`: Chứa các data models và ViewModels.
- `B3cBonsai.Utility`: Chứa các lớp tiện ích như gửi email, hằng số, v.v.
- `B3cBonsaiWeb`: Dự án chính ASP.NET Core MVC, chứa Controllers, Views, và các tài nguyên frontend.