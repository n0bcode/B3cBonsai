using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Staff)]
    public class ManagerJournalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public ManagerJournalController(IUnitOfWork unitOfWork, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get all journal entries
            var journals = await _unitOfWork.NhatKyCay.GetAll(includeProperties: "CayCuaToi.NguoiDungUngDung");

            var result = journals.Select(j => new
            {
                j.Id,
                Cay = j.CayCuaToi.TenCay,
                NguoiDung = j.CayCuaToi.NguoiDungUngDung.HoTen,
                j.NgayTao,
                j.GiaiDoanPhatTrien,
                j.NoiDung,
                j.HinhAnh
            });
            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var journal = await _unitOfWork.NhatKyCay.Get(x => x.Id == id);
            if (journal == null) return Json(new { success = false, message = "Không tìm thấy nhật ký" });

            try
            {
                if (!string.IsNullOrEmpty(journal.HinhAnh))
                {
                    await _fileStorageService.DeleteFileAsync(journal.HinhAnh);
                }

                _unitOfWork.NhatKyCay.Remove(journal);
                _unitOfWork.Save();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Lỗi khi xóa nhật ký" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var journal = await _unitOfWork.NhatKyCay.Get(x => x.Id == id, includeProperties: "CayCuaToi.NguoiDungUngDung");
            if (journal == null) return NotFound();
            return PartialView(journal);
        }
    }
}
