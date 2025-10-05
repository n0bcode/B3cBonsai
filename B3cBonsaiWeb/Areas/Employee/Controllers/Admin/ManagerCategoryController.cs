using B3cBonsai.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B3cBonsai.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using B3cBonsai.Utility;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Admin
{
    [Area("Employee")]
    public class ManagerCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManagerCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            DanhMucSanPham danhMuc = new DanhMucSanPham();
            if (id == null)
            {
                return PartialView(danhMuc);
            }
            else
            {
                danhMuc = await _unitOfWork.DanhMucSanPham.Get(u => u.Id == id);
                if (danhMuc == null)
                {
                    return PartialView(danhMuc);
                }
                return PartialView(danhMuc);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(DanhMucSanPham danhMuc)
        {
            if (ModelState.IsValid)
            {
                if (danhMuc == null || danhMuc.Id == 0)
                {
                    _unitOfWork.DanhMucSanPham.Add(danhMuc);
                }
                else
                {
                    _unitOfWork.DanhMucSanPham.Update(danhMuc);
                }

                _unitOfWork.Save();

                return Json(new { success = true, message = "Thao tác thành công" });
            }

            return Json(new { success = false, message = "Có lỗi xảy ra khi thêm/cập nhật danh mục sản phẩm." });
        }


        #region//GET API
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = (await _unitOfWork.DanhMucSanPham.GetAll());
            return Json(new { data = categories.ToList() });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int? newCategoryId)
        {
            var category = await _unitOfWork.DanhMucSanPham.Get(c => c.Id == id);
            if (category == null)
            {
                return Json(new { success = false, message = "Danh mục không tồn tại." });
            }

            // Kiểm tra số lượng danh mục
            var categories = (await _unitOfWork.DanhMucSanPham.GetAll()).ToList();
            int totalCategories = categories.Count();
            if (totalCategories <= 2)
            {
                return Json(new { success = false, message = "Cần có ít nhất 2 danh mục, không thể xóa." });
            }


            var productsInCategory = (await _unitOfWork.SanPham.GetAll(p => p.DanhMucId == id)).ToList();
            if (productsInCategory.Count > 0)
            {
                if (newCategoryId.HasValue)
                {

                    foreach (var product in productsInCategory)
                    {
                        product.DanhMucId = newCategoryId.Value;
                        _unitOfWork.SanPham.Update(product);
                    }
                    _unitOfWork.Save();
                }
                else
                {
                    return Json(new { success = false, message = "Vui lòng chọn danh mục thay thế cho sản phẩm." });
                }
            }

            _unitOfWork.DanhMucSanPham.Remove(category);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Xóa danh mục thành công." });
        }
        [HttpGet]
        public async Task<IActionResult> GetOtherCategory(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, content = "Không nhận được dữ liệu tìm kiếm" });
            }
            DanhMucSanPham? danhMucSanPham = await _unitOfWork.DanhMucSanPham.Get(dm => dm.Id == id);
            if (danhMucSanPham == null)
            {
                return Json(new { success = false, content = "Không tìm thấy dữ liệu trong hệ thống" });
            }
            IEnumerable<SelectListItem> otherCategory = (await _unitOfWork.DanhMucSanPham.GetAll(dm => dm.Id != id)).Select(dm => new SelectListItem() { Value = dm.Id.ToString(), Text = dm.TenDanhMuc });
            return Json(new { success = true, data = otherCategory, amount = (await _unitOfWork.SanPham.GetAll(sp => sp.DanhMucId == id)).Count() });
        }

        public async Task<IActionResult> DeleteAndTransferToOtherCategory(int? id, int? idChange)
        {
            if (id == null && idChange == null)
            {
                return Json(new { success = false, content = "Không nhận được dữ liệu thay đổi" });
            }
            foreach (SanPham sanPham in (await _unitOfWork.SanPham.GetAll(dm => dm.DanhMucId == id)))
            {
                sanPham.DanhMucId = idChange.Value;
            }
            _unitOfWork.Save();

            _unitOfWork.DanhMucSanPham.Remove(await _unitOfWork.DanhMucSanPham.Get(dm => dm.Id == id));
            _unitOfWork.Save();
            return Json(new { success = true, content = "Bạn đã thay đổi nội dung thành công" });
        }
        #endregion

    }
}