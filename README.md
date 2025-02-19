# Không còn cập nhập

# Cấu hình cấu trúc Project của Solution

![diagram](https://github.com/user-attachments/assets/4b34e8c2-eb03-4ace-8108-cf756062255c)

## Cấu trúc dự án B3cBonsai

### 1. B3cBonsai.DataAccess (Class Library)
- **Data**: Thư mục chứa cấu hình CSDL DbContext/IdentityDbContext<>
  - `ApplicationDbContext.cs`: Class đại diện cho DbContext.
- **Migrations**: Thư mục chứa các phương thức đối chiếu các thay đổi đối với CSDL.
- **Repository**: Thư mục chứa các class triển khai phương thức từ interface trong thư mục IRepository.
  - **IRepository**: Thư mục chứa các class interface cấu trúc phương thức với CSDL.

### 2. B3cBonsai.Models (Class Library)
- **ViewModels**: Thư mục chứa các class tạo lại để hiển thị.
- ...: Các class đối chiếu với bảng trong CSDL.

### 3. B3cBonsai.Utility (Class Library)
- `SD.cs`: Class chứa các tên cố định cho vai trò, tình trạng thanh toán, tình trạng đơn hàng, tên session sử dụng cho Cart.
- ...: Các class mang tính năng thực tế như EmailSender, v.v.

### 4. B3cBonsaiWeb (ASP.NET Core Web App MVC)
- **wwwroot**: Thư mục chứa mã nguồn giao diện.
  - **employee**: Thư mục mã giao diện nhân viên.
  - **customer**: Thư mục mã giao diện người dùng.
- **Areas**: 
  - **Customer**: Các Controller-Views về phương thức truy cập và chức năng của người dùng.
  - **Employee**: Các Controller-Views về chức năng quản lý của nhân viên.
- **Identity**:
  - **Account**: Chứa các phương thức quản lý chức năng truy cập, chủ yếu là RazorPage.
    - **Manager**: Chứa các phương thức cho người dùng đã đăng nhập.
    - ...: Các trang RazorPage chủ yếu cung cấp phương thức cho người dùng chưa truy cập.

### 5. Cấu hình dự án
- `appsettings.json`: Cấu hình dự án (hiện chưa có nội dung).
- `Program.cs`: Class điều hướng đến trang Error404 và sửa Route mở đầu là giao diện trang chủ người dùng.

## Cụ thể cấu trúc các Areas của dự án

![Snapshot](https://github.com/user-attachments/assets/e47f019b-686b-40bc-bd9e-dd4089d44b0c)

### 1. Areas

#### Employee
- **Controller**: Tách làm hai thư mục dựa vào Role nhân viên.
  - **Admin**: 
    - `DashboardController.cs`: Class hiển thị chart thống kê và bảng dữ liệu (có thể bỏ).
    - `ManagerCategoryController.cs`: Class CRUD với dữ liệu loại sản phẩm.
    - `ManagerCustomerController.cs`: Class CRUD với dữ liệu người dùng.
    - `ManagerEmployeeController.cs`: Class CRUD với dữ liệu nhân viên.
  - **Staff**: 
    - `ManagerOrderController.cs`: Class CRUD với dữ liệu đơn hàng.
      - `Index.cshtml`: View hiển thị bảng dữ liệu đơn hàng.
      - `OrderSummary.cshtml`: View hiển thị các thẻ dữ liệu đơn giản.
    - `ManagerProductController.cs`: Class CRUD với dữ liệu sản phẩm.
    - `EmployeeProfileController.cs`: Class thay đổi thông tin cá nhân nhân viên.

- **Views**: Thư mục chứa view tương ứng với các phương thức bên Controller.

#### Customer
- **Controller**:
  - `CartController.cs`: Class quản lý giỏ hàng.
    - `Index.cshtml`: View hiển thị danh sách sản phẩm trong giỏ hàng.
  - `ClientProductController.cs`: Class hiển thị sản phẩm cho người dùng.
    - `Index.cshtml`: View hiển thị tìm kiếm sản phẩm.
    - `Detail.cshtml`: View hiển thị chi tiết sản phẩm.
  - `ClientProfileController.cs`: Class xử lý thông tin cá nhân người dùng.
    - `Index.cshtml`: View danh sách đơn hàng cá nhân khách hàng.
    - `Profile.cshtml`: View hiển thị thông tin cá nhân người dùng.
    - `ChangePassword.cshtml`: View form thay đổi mật khẩu.
    - `ProAddress.cshtml`: View hiển thị địa chỉ khách hàng khi đặt đơn hàng.
    - `ProTickets.cshtml`: View hiển thị ticket của người dùng (tính năng chưa phù hợp với C
