using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.Utility.Extentions; //Chứa lớp ControllerExtentions để tải lại dữ liệu hiển thị

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerComboController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ManagerComboController(ApplicationDbContext db) { 
            _db = db;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
		}
        public IActionResult Upsert(string? id)
        {
            ComboSanPham comboSanPham = string.IsNullOrEmpty(id)
                ? new ComboSanPham()
                : _db.ComboSanPhams.Include(cbo => cbo.ChiTietCombos)
                  .FirstOrDefault(cbo => cbo.Id.ToString() == id) ?? new ComboSanPham();

            var sanPhamList = _db.SanPhams.ToList();
            if (sanPhamList == null || !sanPhamList.Any())
            {
                return Content("Không có sản phẩm nào trong cơ sở dữ liệu.");
            }

            ViewBag.SanPhamList = sanPhamList;
            return PartialView(comboSanPham);
        }



        [HttpPost]
        public async Task<IActionResult> Upsert(ComboSanPham obj, Dictionary<int, int> soLuong)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (obj.Id == 0)
                    {
                        _db.ComboSanPhams.Add(obj);
                        await _db.SaveChangesAsync();

                        foreach (var entry in soLuong)
                        {
                            var chiTietCombo = new ChiTietCombo
                            {
                                ComboId = obj.Id,
                                SanPhamId = entry.Key,
                                SoLuong = entry.Value
                            };
                            _db.ChiTietCombos.Add(chiTietCombo);
                        }

                        await _db.SaveChangesAsync();
                        return Json(new { success = true, message = "Thêm Combo thành công" });
                    }
                    else
                    {
                        _db.ComboSanPhams.Update(obj);

                        var existingDetails = _db.ChiTietCombos.Where(ct => ct.ComboId == obj.Id).ToList();
                        _db.ChiTietCombos.RemoveRange(existingDetails);

                        foreach (var entry in soLuong)
                        {
                            var chiTietCombo = new ChiTietCombo
                            {
                                ComboId = obj.Id,
                                SanPhamId = entry.Key,
                                SoLuong = entry.Value
                            };
                            _db.ChiTietCombos.Add(chiTietCombo);
                        }

                        await _db.SaveChangesAsync();
                        return Json(new { success = true, message = "Cập nhật Combo thành công" });
                    }
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
        .           Select(e => e.ErrorMessage).ToList();
                    Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Upsert: " + ex.Message);
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }
    




    [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id) // Đảm bảo nhận id từ body
        {
            try
            {
                var combo = await _db.ComboSanPhams.FindAsync(id);
                if (combo == null)
                {
                    return Json(new { success = false, message = "Combo không tồn tại." });
                }

                // Xóa các chi tiết của combo
                var chiTietCombos = _db.ChiTietCombos.Where(ct => ct.ComboId == id).ToList();
                _db.ChiTietCombos.RemoveRange(chiTietCombos);
                _db.ComboSanPhams.Remove(combo);

                await _db.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa Combo thành công." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Delete: " + ex.Message); // Log lỗi chi tiết trên server
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
            }
        }





        #region//GET APIS
        public async Task<IActionResult> GetAll()
		{
			var combos = await _db.ComboSanPhams
				.Select(cbo => new
				{
					cbo.Id,
					cbo.TenCombo,
					ChiTietCombos = cbo.ChiTietCombos.Select(ct => new
					{
						ct.Id,
						ct.SanPham,
						// Include other fields you need, but avoid nested `Combo` references
					})
				})
				.ToListAsync();

			return Json(new { data = combos
			});
		}

		#endregion
	}
}
