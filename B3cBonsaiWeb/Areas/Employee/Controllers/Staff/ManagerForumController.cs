using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Staff)]
    public class ManagerForumController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ManagerForumController> _logger;

        public ManagerForumController(IUnitOfWork unitOfWork, ILogger<ManagerForumController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var threads = await _unitOfWork.ChuDe.GetAll(includeProperties: "DanhMucDienDan,NguoiDungUngDung");
            var result = threads.Select(t => new
            {
                t.Id,
                t.TieuDe,
                DanhMuc = t.DanhMucDienDan.TenDanhMuc,
                NguoiTao = t.NguoiDungUngDung.HoTen,
                t.NgayTao,
                t.LuotXem,
                t.TrangThai
            });
            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var thread = await _unitOfWork.ChuDe.Get(t => t.Id == id);
            if (thread == null) return Json(new { success = false, message = "Không tìm thấy chủ đề" });

            thread.TrangThai = !thread.TrangThai;
            _unitOfWork.ChuDe.Update(thread);
            _unitOfWork.Save();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var thread = await _unitOfWork.ChuDe.Get(t => t.Id == id);
            if (thread == null) return Json(new { success = false, message = "Không tìm thấy chủ đề" });

            try
            {
                // Delete all posts in thread first
                _unitOfWork.BaiViet.DeleteByThreadId(id);
                
                _unitOfWork.ChuDe.Remove(thread);
                _unitOfWork.Save();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting thread {ThreadId}", id);
                return Json(new { success = false, message = "Lỗi khi xóa chủ đề" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var thread = await _unitOfWork.ChuDe.Get(t => t.Id == id, includeProperties: "DanhMucDienDan,NguoiDungUngDung,BaiViets.NguoiDungUngDung");
            if (thread == null) return NotFound();
            return PartialView(thread);
        }
    }
}
