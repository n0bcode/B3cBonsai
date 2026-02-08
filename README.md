# B3cBonsai - Website Thương mại điện tử

Đây là một dự án website thương mại điện tử chuyên về bán cây cảnh bonsai, được xây dựng trên nền tảng **ASP.NET Core 8**. Dự án áp dụng kiến trúc Clean Architecture, hỗ trợ đa nền tảng CSDL và tích hợp sẵn Docker.

## 🚀 Công nghệ sử dụng

- **Backend:** C#, ASP.NET Core 8 (MVC), Entity Framework Core 8
- **Frontend:** Razor Pages, HTML/CSS (Bootstrap), jQuery
- **Database:** PostgreSQL (Mặc định) / SQL Server
- **Authentication:** ASP.NET Core Identity (Google, Facebook Integration)
- **Payment:** Tích hợp cổng thanh toán VnPay
- **DevOps:** Docker, Docker Compose

## 📂 Tài liệu Hướng dẫn

Để giúp bạn dễ dàng cài đặt và triển khai, tài liệu dự án được chia thành các phần chi tiết sau:

| Mục tiêu | Tài liệu tham khảo |
| :--- | :--- |
| **Cài đặt & Chạy Local** | [📄 Xem Hướng dẫn Cài đặt (Không dùng Docker)](docs/installation-guide.md) |
| **Sử dụng Docker** | [🐳 Xem Hướng dẫn Docker & Docker Compose](docs/deploy-docker.md) |
| **Deploy lên Heroku** | [☁️ Xem Hướng dẫn Deploy Heroku](docs/deploy-heroku.md) |

## 📊 Dữ liệu Mẫu (Sample Data)

Dự án tích hợp sẵn hệ thống **Data Seeding** mạnh mẽ. Khi chạy lần đầu (nếu được bật), hệ thống sẽ tự động tạo:
- 30+ Tài khoản người dùng (Admin, Customer...)
- 50+ Sản phẩm & Danh mục cây cảnh
- 200+ Đơn hàng mẫu & Đánh giá
- *Chi tiết cấu hình xem trong [Hướng dẫn Cài đặt](docs/installation-guide.md).*

## 🏗️ Cấu trúc Dự án

Dự án tuân theo **Clean Architecture**:

- **`B3cBonsai.DataAccess`**: Xử lý truy cập dữ liệu (Repository Pattern, EF Core).
- **`B3cBonsai.Models`**: Chứa Data Models và ViewModels.
- **`B3cBonsai.Utility`**: Các tiện ích hỗ trợ (Email, Helpers, Constants).
- **`B3cBonsaiWeb`**: Web App chính (Controllers, Views, Resources).

---
*Tài liệu tham khảo thêm:*
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Docker cho .NET Developers](https://docs.docker.com/language/dotnet/)
