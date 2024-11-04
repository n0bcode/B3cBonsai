using B3cBonsai.DataAccess.Repository;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerCustomerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ManagerCustomerController> _logger;

        public ManagerCustomerController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, ILogger<ManagerCustomerController> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Upsert()
        {
            return View();
        }
        public IActionResult DetailWithDelete()
        {
            return View();
        }

        #region API CALLS
        public async Task<IActionResult> GetAll()
        {
            // Lấy danh sách của tất cả các người dùng
            var allUsers = _userManager.Users.ToList();

            // Lọc người dùng có vai trò 'Role_Customer'
            var customers = new List<NguoiDungUngDung>();
            foreach (var user in allUsers)
            {
                if (_userManager.IsInRoleAsync(user, SD.Role_Customer).Result)
                {
                    // Giả định rằng bạn có phương thức để chuyển đổi từ IdentityUser sang NguoiDungUngDung
                    var customer = new NguoiDungUngDung
                    {
                        // Điền thông tin từ user vào customer
                        Id = user.Id,
                        UserName = user.UserName,
                        // Các thuộc tính khác mà bạn cần
                    };
                    customers.Add(customer);
                }
            }

            return Json(new { data = customers });
        }
        #endregion
    }
}
