using System.Diagnostics;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Controllers
{
    [Area("Customer")]
    //Cho hiển thị các view giao diện đơn giản
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork context)
        {
            _unitOfWork = context;
        }

        // Hiển thị danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sản phẩm theo trạng thái
            var products = await _unitOfWork.SanPham.GetAll(
                includeProperties: "DanhMuc,HinhAnhs",
                filter: x => x.TrangThai
            );

            // Lấy 12 sản phẩm mới nhất dựa vào NgayTao
            var latestProducts = products
                .OrderByDescending(x => x.NgayTao) // Sắp xếp giảm dần theo ngày tạo
                .Take(12) // Lấy 12 sản phẩm mới nhất
                .ToList();

            return View(latestProducts);
        }



        // Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Detail(int id)
        {
            var product = await _unitOfWork.SanPham.Get(includeProperties: "DanhMuc,HinhAnhs,BinhLuans,DanhSachYeuThichs", filter: x => x.TrangThai && x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        #region//Other View
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult FAQ() { 
            return View();
        }

        public IActionResult Terms() {
            return View();
        }
        public IActionResult ReturnPolicy()
        {
            return View();
        }
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult ComingSoon()
        {
            return View();
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
