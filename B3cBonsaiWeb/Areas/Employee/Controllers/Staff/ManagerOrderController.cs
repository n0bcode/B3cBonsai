using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsaiWeb.Services;
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
        private readonly IUnitOfWork _unit;
        private readonly TelegramService _telegramService;

        public ManagerOrderController(ApplicationDbContext applicationDbContext, TelegramService telegramService, IUnitOfWork unitOfWork)
        {
            _context = applicationDbContext;
            _telegramService = telegramService;
            _unit = unitOfWork;
        }

        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Staff}")]
        public IActionResult Index()
        {
            // Đếm số lượng đơn hàng theo từng tình trạng
            var statusPending = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusPending);
            var statusInProcess = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusInProcess);
            var statusApproved = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusApproved);
            var statusShipped = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusShipped);
            var statusCancelled = _context.DonHangs.Count(d => d.TrangThaiDonHang == SD.StatusCancelled);
            var statusRefunded = _context.DonHangs.Count(d => d.TrangThaiThanhToan == SD.StatusRefunded);

            // Lưu kết quả vào TempData để truyền tới view
            TempData["StatusPending"] = statusPending;
            TempData["StatusInProcess"] = statusInProcess;
            TempData["StatusApproved"] = statusApproved;
            TempData["StatusShipped"] = statusShipped;
            TempData["StatusCancelled"] = statusCancelled;
            TempData["StatusRefunded"] = statusRefunded;

            return View();
        }
        public async Task<IActionResult> Detail(int id)
        {
            // Fetching the order including related products and images
            var order = await _context.DonHangs
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ctdh => ctdh.SanPham)
                        .ThenInclude(sp => sp.HinhAnhs)
                .AsNoTracking()
                .FirstOrDefaultAsync(dh => dh.Id == id);

            // Check if the order is null, return NotFound if so
            if (order == null)
            {
                return NotFound(); // Handle the case when no order is found
            }

            // Map the fetched order to the desired DTO (Data Transfer Object)
            var takeDataOrder = new DonHang
            {
                Id = order.Id,
                TenNguoiNhan = order.TenNguoiNhan,
                SoTheoDoi = order.SoTheoDoi,
                TrangThaiDonHang = order.TrangThaiDonHang,
                NgayDatHang = order.NgayDatHang,
                NgayNhanHang = order.NgayNhanHang,
                TrangThaiThanhToan = order.TrangThaiThanhToan,
                TongTienDonHang = order.TongTienDonHang,
                ChiTietDonHangs = order.ChiTietDonHangs.Where(ctdh => ctdh.DonHangId == id).Select(ctorder => new ChiTietDonHang
                {
                    Id = ctorder.Id,
                    SoLuong = ctorder.SoLuong,
                    SanPham = ctorder.SanPham != null ? new SanPham
                    {
                        TenSanPham = ctorder.SanPham.TenSanPham,
                        HinhAnhs = ctorder.SanPham.HinhAnhs?.Select(ha => new HinhAnhSanPham { LinkAnh = ha.LinkAnh }).ToList() ?? new List<HinhAnhSanPham>()
                    } : null, // Assign null if SanPham is null
                    LoaiDoiTuong = ctorder.LoaiDoiTuong,
                    Combo = ctorder.Combo,
                    SanPhamId = ctorder.SanPhamId,
                    ComboId = ctorder.ComboId
                }).ToList() // Convert the selection to a list
            };


            return PartialView( takeDataOrder ); // Return the constructed object
        }



        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Staff}")]
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
                        },
                        LoaiDoiTuong = ctdh.LoaiDoiTuong,
                        Combo = ctdh.Combo,
                        SanPhamId = ctdh.SanPhamId,
                        ComboId = ctdh.ComboId
                    }).ToList()
                })
                .ToList();
            return orders;
        }
        #region//GET API
        public async Task<IActionResult> GetAll(string? orderStatus)
        {
            var orders = TakeAllOrders();
            if (!string.IsNullOrEmpty(orderStatus))
                orders = orders.Where(or => or.TrangThaiDonHang == orderStatus).ToList();
            return Json(new { data = orders });
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

            if ((donHang.TrangThaiThanhToan == SD.PaymentStatusApproved) ||
                (donHang.TrangThaiThanhToan == SD.PaymentStatusRejected))
            {
                return Json(new { success = false, title = "Lỗi", content = "Không thể thay đổi tình trạng thanh toán theo hướng ngược lại!" });
            }

            donHang.TrangThaiThanhToan = statusPayment;
            _context.Update(donHang);
            await _context.SaveChangesAsync();

            // Gửi thông báo đến Telegram
            var message = $"Đơn hàng #{donHang.Id} của {donHang.TenNguoiNhan} đã thay đổi trạng thái thanh toán thành: {statusPayment}.";
            await _telegramService.SendMessageAsync(838657228, message); // Thay `123456789` bằng Chat ID thực tế.

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

            if ((donHang.TrangThaiDonHang == SD.StatusShipped) ||
                (donHang.TrangThaiDonHang == SD.StatusCancelled) ||
                (donHang.TrangThaiDonHang == SD.StatusRefunded))
            {
                return Json(new { success = false, title = "Lỗi", content = "Không thể thay đổi tình trạng đơn hàng theo hướng ngược lại!" });
            }

            donHang.TrangThaiDonHang = statusOrder;
            _context.Update(donHang);
            await _context.SaveChangesAsync();

            // Gửi thông báo đến Telegram
            var message = $"Đơn hàng #{donHang.Id} của {donHang.TenNguoiNhan} đã thay đổi trạng thái đơn hàng thành: {statusOrder}.";
            await _telegramService.SendMessageAsync(838657228, message);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id, string reason)
        {
            var donHang = await _context.DonHangs
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null)
            {
                return Json(new { success = false, title = "Lỗi", content = "Không tìm thấy đơn hàng!" });
            }

            if ((donHang.TrangThaiDonHang == SD.StatusShipped) ||
                (donHang.TrangThaiDonHang == SD.StatusCancelled) ||
                (donHang.TrangThaiDonHang == SD.StatusRefunded))
            {
                return Json(new { success = false, title = "Lỗi", content = "Không thể thay đổi tình trạng đơn hàng theo hướng ngược lại!" });
            }

            donHang.TrangThaiThanhToan = SD.PaymentStatusRejected;
            donHang.TrangThaiDonHang = SD.StatusCancelled;

            donHang.LyDoHuyDonHang = reason;

            _context.Update(donHang);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đã hủy đơn hàng thành công!" });
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



