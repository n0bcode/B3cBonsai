# 🚀 Hướng dẫn Deploy lên Heroku

Quy trình này sử dụng Heroku Buildpack cho .NET, không yêu cầu `Dockerfile` hay `Procfile`.

## 1. Yêu cầu
- [Heroku CLI](https://devcenter.heroku.com/articles/heroku-cli)
- Tài khoản Heroku đã đăng nhập (`heroku login`)

## 2. Thiết lập

1. **Tạo ứng dụng:**
   ```bash
   heroku create b3c-app-name
   ```

2. **Kết nối Git:**
   ```bash
   heroku git:remote -a b3c-app-name
   ```

3. **Thêm Database (PostgreSQL):**
   ```bash
   heroku addons:create heroku-postgresql:hobby-dev
   ```

## 3. Cấu hình Biến Môi trường (Config Vars)

Vào Heroku Dashboard > **Settings** > **Config Vars** và thêm các biến sau:

| Key | Value | Bắt buộc? |
| --- | --- | :---: |
| `ASPNETCORE_ENVIRONMENT` | `Production` | ✅ |
| `ASPNETCORE_UsePostgreSql` | `true` | ✅ |
| `ASPNETCORE_SeedSampleData` | `false` | ✅ |
| `ConnectionStrings__PostgreConnectString` | *(Copy từ Database Credentials hoặc dùng biến DATABASE_URL nếu code hỗ trợ)* | ✅ |

*Lưu ý: Với .NET trên Heroku, các biến lồng nhau trong JSON dùng dấu gạch dưới kép `__` thay cho dấu hai chấm.*

## 4. Deploy

Đẩy code từ nhánh main lên Heroku:

```bash
git push heroku main
```

## 5. Chạy Migration

Sau khi deploy thành công, chạy lệnh sau để tạo bảng trong CSDL:

```bash
heroku run dotnet ef database update --project B3cBonsai.DataAccess
```

## 6. Debugging

- Xem log: `heroku logs --tail`
- Mở web: `heroku open`
