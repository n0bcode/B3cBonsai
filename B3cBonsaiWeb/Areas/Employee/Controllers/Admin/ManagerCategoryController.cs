using B3cBonsai.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using B3cBonsai.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _unitOfWork.DanhMucSanPham.GetAll();
            return Json(new { data = categories.ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            DanhMucSanPham danhMuc = new DanhMucSanPham();
            if (id == null)
            {
                return PartialView("Upsert", danhMuc);
            }
            else
            {
                danhMuc = await Task.FromResult(_unitOfWork.DanhMucSanPham.GetFirstOrDefault(u => u.Id == id));
                if (danhMuc == null)
                {
                    return NotFound();
                }
                return PartialView("Upsert", danhMuc);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(DanhMucSanPham danhMuc)
        {
            if (ModelState.IsValid)
            {
                if (danhMuc.Id == 0)
                {
                    _unitOfWork.DanhMucSanPham.Add(danhMuc);
                    TempData["success"] = "Thêm danh mục sản phẩm thành công";
                }
                else
                {
                    _unitOfWork.DanhMucSanPham.Update(danhMuc);
                    TempData["success"] = "Cập nhật danh mục sản phẩm thành công";
                }
                await Task.Run(() => _unitOfWork.Save());


                return Json(new { success = true, message = TempData["success"].ToString() });
            }

            return Json(new { success = false, message = "Có lỗi xảy ra khi thêm/cập nhật danh mục sản phẩm." });
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id, int? newCategoryId)
        {
            var category = await Task.FromResult(_unitOfWork.DanhMucSanPham.GetFirstOrDefault(c => c.Id == id));
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
                    await Task.Run(() => _unitOfWork.Save());
                }
                else
                {
                    return Json(new { success = false, message = "Vui lòng chọn danh mục thay thế cho sản phẩm." });
                }
            }

            _unitOfWork.DanhMucSanPham.Remove(category);
            await Task.Run(() => _unitOfWork.Save());

            return Json(new { success = true, message = "Xóa danh mục thành công." });
        }


    }
}
