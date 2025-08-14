# Hướng dẫn Cấu hình API Keys và Secrets

Dự án này yêu cầu một số khóa API và chuỗi kết nối để hoạt động đầy đủ. Các khóa này cần được lưu trữ trong tệp `B3cBonsaiWeb/appsettings.json` hoặc thông qua .NET Secret Manager để đảm bảo an toàn.

## 1. Chuỗi kết nối (Connection Strings)

Bạn cần cung cấp chuỗi kết nối đến cơ sở dữ liệu SQL Server.

- **Vị trí:** `appsettings.json` -> `ConnectionStrings`
- **Khóa:** `ConnectString` và `ApplicationDbContextConnection`
- **Cách lấy:**
  1. Đảm bảo bạn đã cài đặt một phiên bản SQL Server.
  2. Tạo một cơ sở dữ liệu mới (ví dụ: `B3cCSDL`).
  3. Sử dụng chuỗi kết nối phù hợp với cấu hình SQL Server của bạn. Đối với môi trường phát triển cục bộ, chuỗi kết nối thường có dạng:
     ```json
     "ConnectString": "Server=.;Database=B3cCSDL;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
     ```

## 2. Cấu hình Email (Gmail)

Hệ thống sử dụng Gmail để gửi email. Bạn cần tạo một "Mật khẩu ứng dụng" cho tài khoản Google của mình.

- **Vị trí:** `appsettings.json` -> `EmailSettings`
- **Khóa:** `Password`
- **Cách lấy:**
  1. Truy cập tài khoản Google của bạn.
  2. Bật Xác minh 2 bước (2-Step Verification).
  3. Đi đến trang [Mật khẩu ứng dụng](https://myaccount.google.com/apppasswords).
  4. Tạo một mật khẩu mới cho ứng dụng, và sao chép mật khẩu 16 chữ số đó.
  5. Dán mật khẩu vào giá trị `Password` trong `EmailSettings`.

## 3. AWS Secrets Manager

Dự án sử dụng AWS Secrets Manager để quản lý các thông tin nhạy cảm (tùy chọn, có thể thay thế bằng cấu hình cục bộ).

- **Vị trí:** `appsettings.json` -> `AWS`
- **Khóa:** `AccessKeyId`, `SecretAccessKey`, `SecretArn`
- **Cách lấy:**
  1. Tạo một tài khoản AWS.
  2. Tạo một người dùng IAM với quyền truy cập vào Secrets Manager.
  3. Lấy `AccessKeyId` và `SecretAccessKey` cho người dùng đó.
  4. Tạo một secret trong AWS Secrets Manager và lấy ARN của nó (`SecretArn`).

## 4. Telegram Bot

Để tích hợp với Telegram Bot.

- **Vị trí:** `appsettings.Development.json` -> `TelegramBot`
- **Khóa:** `Token`
- **Cách lấy:**
  1. Nói chuyện với [BotFather](https://t.me/botfather) trên Telegram.
  2. Sử dụng lệnh `/newbot` để tạo một bot mới.
  3. BotFather sẽ cung cấp cho bạn một token. Hãy sao chép và dán nó vào đây.

## 5. Cổng thanh toán VnPay

Để sử dụng cổng thanh toán VnPay (chế độ sandbox cho phát triển).

- **Vị trí:** `appsettings.Development.json` -> `Vnpay`
- **Khóa:** `TmnCode`, `HashSecret`
- **Cách lấy:**
  1. Đăng ký một tài khoản thử nghiệm trên trang [VnPay Sandbox](https://sandbox.vnpayment.vn/).
  2. Sau khi đăng nhập, bạn sẽ tìm thấy `TmnCode` và `HashSecret` trong trang quản lý của mình.

Sau khi điền tất cả các giá trị cần thiết, tệp `appsettings.json` và `appsettings.Development.json` của bạn sẽ sẵn sàng để chạy dự án.
