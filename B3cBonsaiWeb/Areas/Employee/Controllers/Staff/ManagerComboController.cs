using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using B3cBonsai.Utility.Extentions;
using B3cBonsai.Utility; //Chứa lớp ControllerExtentions để tải lại dữ liệu hiển thị

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerComboController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webEnvironment;
        public ManagerComboController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) { 
            _db = db;
            _webEnvironment = webHostEnvironment;
        }
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Staff}")]
        public IActionResult Index()
        {
            return View();
		}
        public IActionResult Upsert(string? id)
        {
            ComboSanPham comboSanPham;
            if (string.IsNullOrEmpty(id))
            {
                comboSanPham = new ComboSanPham();
            }
            else
            {
                comboSanPham = _db.ComboSanPhams.Include(cbo => cbo.ChiTietCombos)
                                 .FirstOrDefault(cbo => cbo.Id.ToString() == id) ?? new ComboSanPham();
            }

            var sanPhamList = _db.SanPhams.ToList();
            if (sanPhamList == null || !sanPhamList.Any())
            {
                return Content("Không có sản phẩm nào trong cơ sở dữ liệu.");
            }

            ViewBag.SanPhamList = sanPhamList;
            return PartialView(comboSanPham); 
        }




        [HttpPost]
        public async Task<IActionResult> Upsert(ComboSanPham obj, Dictionary<int, int> soLuong, IFormFile? file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null) // Cập nhật hình ảnh
                    {
                        string wwwRootPath = _webEnvironment.WebRootPath;

                        // Tạo tên tệp mới với GUID
                        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                        // Tạo thư mục lưu trữ
                        string comboFolder = Path.Combine("images", "combo");
                        string finalFolder = Path.Combine(wwwRootPath, comboFolder);

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(finalFolder))
                        {
                            Directory.CreateDirectory(finalFolder);
                        }

                        // Xóa hình ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(obj.LinkAnh))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, obj.LinkAnh.TrimStart('/', '\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                try
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                                catch (Exception ex)
                                {
                                    // Ghi log hoặc xử lý lỗi nếu cần
                                    Console.WriteLine($"Không thể xóa tệp: {oldImagePath}. Lỗi: {ex.Message}");
                                }
                            }
                        }

                        // Lưu hình ảnh mới
                        string finalFilePath = Path.Combine(finalFolder, fileName);
                        try
                        {
                            using (var fileStream = new FileStream(finalFilePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            // Cập nhật đường dẫn mới cho LinkAnh
                            obj.LinkAnh = $"\\{Path.Combine(comboFolder, fileName).Replace("\\", "/")}";
                        }
                        catch (Exception ex)
                        {
                            // Ghi log hoặc xử lý lỗi lưu tệp nếu cần
                            Console.WriteLine($"Không thể lưu tệp: {finalFilePath}. Lỗi: {ex.Message}");
                            throw;
                        }
                    }

                    if (obj.Id == 0) // Thêm mới nếu Id là 0
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
                    else //Cập nhập
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
                                   .Select(e => e.ErrorMessage).ToList();
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
                    cbo.MoTa,
                    cbo.LinkAnh,
                    cbo.TongGia,
					ChiTietCombos = cbo.ChiTietCombos.Select(ct => new
					{
						ct.Id,
						ct.SanPham,
						// Include other fields you need, but avoid nested `Combo` references
					})
				})
				.ToListAsync();

			return Json(new { data = combos});
		}

		#endregion
	}
}
