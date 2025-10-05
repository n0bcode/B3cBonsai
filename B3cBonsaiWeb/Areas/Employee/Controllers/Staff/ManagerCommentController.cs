using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerCommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ManagerCommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize(Roles = $"{SD.Role_Staff},{SD.Role_Admin}")]
        public IActionResult Index()
        {
            return View();
        }

        #region//GET APIS
        public async Task<IActionResult> GetAll()
        {
            var listComments = await _unitOfWork.BinhLuan.GetAll(null, "SanPham.HinhAnhs");

            if (listComments == null || !listComments.Any())
            {
                return Json(new { data = new List<object>() }); // Return an empty data list
            }

            foreach (var comment in listComments)
            {
                comment.SanPham = new SanPham
                {
                    Id = comment.SanPham.Id,
                    TenSanPham = comment.SanPham.TenSanPham,
                    HinhAnhs = comment.SanPham.HinhAnhs.Select(ha => new HinhAnhSanPham { LinkAnh = ha.LinkAnh }).ToList()
                };
            }

            return Json(new { data = listComments });
        }
        #endregion

        #region//ACTION APIS
        public async Task<IActionResult> ChangeOneStatus(int id)
        {
            var comment = await _unitOfWork.BinhLuan.Get(bl => bl.Id == id);
            if (comment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bình luận bạn muốn thay đổi tình trạng!" });
            }
            comment.TinhTrang = !comment.TinhTrang;
            _unitOfWork.BinhLuan.Update(comment);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Thay đổi tình trạng thành công" });
        }

        public async Task<IActionResult> ChangeManyStatus(IEnumerable<int> ids)
        {
            var comments = await _unitOfWork.BinhLuan.GetAll(bl => ids.Contains(bl.Id));
            if (comments == null || comments.Count() == 0)
            {
                return Json(new { success = false, message = "Không tìm thấy các bình luận bạn muốn thay đổi tình trạng!" });
            }
            foreach (var comment in comments)
            {
                comment.TinhTrang = !comment.TinhTrang;
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Thay đổi tình trạng thành công" });
        }

        public async Task<IActionResult> DeleteOne(int id)
        {
            var comment = await _unitOfWork.BinhLuan.Get(bl => bl.Id == id);
            if (comment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bình luận bạn muốn xóa!" });
            }
            _unitOfWork.BinhLuan.Remove(comment);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa bình luận thành công" });
        }

        public async Task<IActionResult> DeleteMany(IEnumerable<int> ids)
        {
            var comments = await _unitOfWork.BinhLuan.GetAll(bl => ids.Contains(bl.Id));
            if (comments == null || comments.Count() == 0)
            {
                return Json(new { success = false, message = "Không tìm thấy các bình luận bạn muốn xóa!" });
            }
            _unitOfWork.BinhLuan.RemoveRange(comments);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Các bình luận đã được xóa thành công" });
        }

        #endregion
    }
}