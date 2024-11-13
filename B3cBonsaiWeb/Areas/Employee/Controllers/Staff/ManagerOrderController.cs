using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using ClosedXML.Excel;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerOrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ManagerOrderController(ApplicationDbContext unitOfWork)
        {
			_context = unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
            // Đếm số lượng đơn hàng theo từng tình trạng
            var notStartedCount = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusPending );
            var inProgressCount = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusApproved );
            var testingCount = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusInProcess );
            var awaitingCount = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusShipped );
            var completeCount = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusCancelled );
            var pendingCount = _context.DonHangs.Count(d => d.TrangThaiThanhToan == SD.StatusRefunded);

            // Lưu kết quả vào TempData để truyền tới view
            TempData["NotStartedCount"] = notStartedCount;
            TempData["InProgressCount"] = inProgressCount;
            TempData["TestingCount"] = testingCount;
            TempData["AwaitingCount"] = awaitingCount;
            TempData["CompleteCount"] = completeCount;
            TempData["PendingCount"] = pendingCount;

            return View();
        }


        public IActionResult OrderSummary()
        {

            return View(TakeAllOrders()); 
        }

        [NonAction]
        private List<DonHang> TakeAllOrders()
        {
            var ordersQuery = _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ctdh => ctdh.SanPham)
                .ThenInclude(sp => sp.HinhAnhs)
                .AsNoTracking()
                .AsQueryable();

            var orders = ordersQuery
                .Select(dh => new DonHang
                {
                    Id = dh.Id,
                    TenNguoiNhan = dh.TenNguoiNhan,
                    SoTheoDoi = dh.SoTheoDoi,
                    TrangThaiDonHang = dh.TrangThaiDonHang,
                    NgayDatHang = dh.NgayDatHang,
                    NgayNhanHang = dh.NgayNhanHang,
                    TrangThaiThanhToan = dh.TrangThaiThanhToan,
                    TongTienDonHang = dh.TongTienDonHang,
                    ChiTietDonHangs = dh.ChiTietDonHangs.Select(ctdh => new ChiTietDonHang
                    {
                        Id = ctdh.Id,
                        SoLuong = ctdh.SoLuong,
                        SanPham = new SanPham
                        {
                            TenSanPham = ctdh.SanPham.TenSanPham,
                            HinhAnhs = ctdh.SanPham.HinhAnhs.Select(ha => new HinhAnhSanPham { LinkAnh = ha.LinkAnh }).ToList()
                        }
                    }).ToList()
                })
                .ToList();
            return orders;
        }
        #region//GET API
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = TakeAllOrders() });
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatusPayment(int id, string statusPayment)
        {
            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null)
            {
                return Json(new { success = false, title = "Lỗi", content = "Không tìm thấy đơn hàng!" });
            }

            // Kiểm tra điều kiện ngược lại cho tình trạng thanh toán
            if ((donHang.TrangThaiThanhToan == SD.PaymentStatusApproved && statusPayment == SD.PaymentStatusPending) ||
                (donHang.TrangThaiThanhToan == SD.PaymentStatusRejected && statusPayment != SD.PaymentStatusPending))
            {
                return Json(new { success = false, title = "Lỗi", content = "Không thể thay đổi tình trạng thanh toán theo hướng ngược lại!" });
            }

            donHang.TrangThaiThanhToan = statusPayment;
            _context.Update(donHang);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatusOrder(int id, string statusOrder)
        {
            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null)
            {
                return Json(new { success = false, title = "Lỗi", content = "Không tìm thấy đơn hàng!" });
            }

            // Kiểm tra điều kiện ngược lại cho tình trạng đơn hàng
            if ((donHang.TrangThaiDonHang == SD.StatusShipped && statusOrder == SD.StatusPending) ||
                (donHang.TrangThaiDonHang == SD.StatusCancelled && statusOrder != SD.StatusRefunded))
            {
                return Json(new { success = false, title = "Lỗi", content = "Không thể thay đổi tình trạng đơn hàng theo hướng ngược lại!" });
            }

            donHang.TrangThaiDonHang = statusOrder;
            _context.Update(donHang);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ExportOrders()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachDonHang");
                var currentRow = 1;

                // Tiêu đề cho các cột
                worksheet.Cell(currentRow, 1).Value = "Mã Đơn Hàng";          // Id
                worksheet.Cell(currentRow, 2).Value = "Tên Người Nhận";       // TenNguoiNhan
                worksheet.Cell(currentRow, 3).Value = "Số Theo Dõi";          // SoTheoDoi
                worksheet.Cell(currentRow, 4).Value = "Trạng Thái Đơn Hàng";  // TrangThaiDonHang
                worksheet.Cell(currentRow, 5).Value = "Ngày Đặt Hàng";        // NgayDatHang
                worksheet.Cell(currentRow, 6).Value = "Ngày Nhận Hàng";       // NgayNhanHang
                worksheet.Cell(currentRow, 7).Value = "Trạng Thái Thanh Toán"; // TrangThaiThanhToan
                worksheet.Cell(currentRow, 8).Value = "Tổng Tiền Đơn Hàng";   // TongTienDonHang

                // Thêm các dòng cho từng đơn hàng
                foreach (var order in TakeAllOrders())
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.Id;
                    worksheet.Cell(currentRow, 2).Value = order.TenNguoiNhan;
                    worksheet.Cell(currentRow, 3).Value = order.SoTheoDoi;
                    worksheet.Cell(currentRow, 4).Value = order.TrangThaiDonHang;
                    worksheet.Cell(currentRow, 5).Value = order.NgayDatHang.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 6).Value = order.NgayNhanHang?.ToString("dd/MM/yyyy");
                    worksheet.Cell(currentRow, 7).Value = order.TrangThaiThanhToan;
                    worksheet.Cell(currentRow, 8).Value = order.TongTienDonHang;

                    // Chi tiết đơn hàng (tùy chọn: lặp và ghi thông tin chi tiết sản phẩm nếu cần)
                    foreach (var detail in order.ChiTietDonHangs)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 2).Value = "Sản phẩm: " + detail.SanPham.TenSanPham;
                        worksheet.Cell(currentRow, 3).Value = "Số lượng: " + detail.SoLuong;

                        var imageLinks = string.Join(", ", detail.SanPham.HinhAnhs.Select(ha => ha.LinkAnh));
                        worksheet.Cell(currentRow, 4).Value = "Hình ảnh: " + imageLinks;
                    }
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachDonHang.xlsx");
                }
            }

        }
        #endregion
    }
}
	


