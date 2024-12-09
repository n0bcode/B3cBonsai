using Microsoft.AspNetCore.Mvc;
using B3cBonsai.DataAccess.Repository.IRepository;
using System.Security.Claims;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Extentions;
using B3cBonsaiWeb.Services;
using B3cBonsai.Models.ViewModels;
using System.Text.RegularExpressions;
using System.Text;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Image;
using iText.IO.Font;
using iText.Kernel.Font;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TelegramService _telegramService;
        private readonly IVnPayService _vnPayService;
        private readonly IEmailSender _emailSender;

        public PaymentController(SignInManager<IdentityUser> signInManager,IUnitOfWork unitOfWork, TelegramService telegramService, IVnPayService vnPayService, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _telegramService = telegramService;
            _vnPayService = vnPayService;
            _emailSender = emailSender;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);


            NguoiDungUngDung nguoiDungUngDung = await _unitOfWork.NguoiDungUngDung.Get(nd => nd.Id == maKhachHang);

            if (nguoiDungUngDung.LockoutEnd != null && nguoiDungUngDung.LockoutEnd > DateTime.UtcNow)
            {
                await _signInManager.SignOutAsync();
                return LocalRedirect("/identity/account/AccessDenied");
            }

            var cartItems = HttpContext.Session.GetComplexData<List<GioHang>>(SD.SessionCart);

            if (cartItems == null || !cartItems.Any())
            {
                ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart", new { area = "Customer" });
            }

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCashPayment(string receiverName, string receiverAddress, string city, string receiverPhone)
        {
            try
            {
                string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var cartItems = HttpContext.Session.GetComplexData<List<GioHang>>(SD.SessionCart);

                if (cartItems == null || !cartItems.Any())
                {
                    return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });
                }

                if (string.IsNullOrWhiteSpace(receiverName) || string.IsNullOrWhiteSpace(receiverAddress) ||
                    string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(receiverPhone))
                {
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin giao hàng." });
                }

                string? nhanVienId = null;
                if (User.IsInRole(SD.Role_Admin))
                {
                    nhanVienId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                var donHang = new DonHang
                {
                    NguoiDungId = maKhachHang,
                    NhanVienId = nhanVienId,
                    NgayDatHang = DateTime.Now,
                    TongTienDonHang = cartItems.Sum(c => c.SoLuong * c.Gia),
                    TrangThaiThanhToan = SD.PaymentStatusPending,
                    TrangThaiDonHang = SD.StatusPending,
                    TenNguoiNhan = receiverName,
                    Duong = receiverAddress,
                    ThanhPho = city,
                    Tinh = "Không xác định",
                    MaBuuDien = "00000",
                    SoDienThoai = receiverPhone
                };

                _unitOfWork.DonHang.Add(donHang);
                _unitOfWork.Save();

                List<ChiTietDonHang> chiTietDonHangs = new List<ChiTietDonHang>();
                foreach (var item in cartItems)
                {
                    if (item.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                    {
                        chiTietDonHangs.Add(new ChiTietDonHang
                        {
                            DonHangId = donHang.Id,
                            ComboId = item.MaCombo,
                            Combo = await _unitOfWork.ComboSanPham.Get(filter: cbo => cbo.Id == item.Id),
                            SoLuong = item.SoLuong,
                            Gia = (int)item.Gia,
                            LoaiDoiTuong = SD.ObjectDetailOrder_Combo
                        });
                    }
                    else
                    {
                        chiTietDonHangs.Add(new ChiTietDonHang
                        {
                            DonHangId = donHang.Id,
                            SanPhamId = item.MaSanPham,
                            SanPham = await _unitOfWork.SanPham.Get(filter: cbo => cbo.Id == item.Id),
                            SoLuong = item.SoLuong,
                            Gia = (int)item.Gia,
                            LoaiDoiTuong = SD.ObjectDetailOrder_SanPham
                        });
                    }
                }

                _unitOfWork.ChiTietDonHang.AddRange(chiTietDonHangs);

                foreach (var ctdh in chiTietDonHangs)
                {
                    if (ctdh.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                    {
                        var combo = await _unitOfWork.ComboSanPham.Get(filter: cbo => cbo.Id == ctdh.ComboId);
                        combo.SoLuong = combo.SoLuong - ctdh.SoLuong;
                    }
                    else
                    {
                        var sanPham = await _unitOfWork.SanPham.Get(filter: cbo => cbo.Id == ctdh.SanPhamId);
                        sanPham.SoLuong = sanPham.SoLuong - ctdh.SoLuong;
                    }
                }

                _unitOfWork.Save();
                HttpContext.Session.SetComplexData(SD.SessionCart, new List<GioHang>());
                try
                {
                    var message = $"Đơn hàng #{donHang.Id} đã được đặt thành công.\n" +
                                  $"Phương thức thanh toán: Thanh toán khi nhận hàng.\n" +
                                  $"Tổng tiền: {donHang.TongTienDonHang:N0} đ.";
                    await _telegramService.SendMessageAsync(838657228, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi tin nhắn Telegram: {ex.Message}");
                }

                try
                {
                    // Gửi email xác nhận đơn hàng
                    await SendOrderConfirmationEmail(donHang);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi email xác nhận đơn hàng: {ex.Message}");
                }


                return Json(new
                {
                    success = true,
                    message = "Thanh toán khi nhận hàng thành công!",
                    redirectUrl = Url.Action("OrderComplete", "Payment", new { area = "Customer", orderId = donHang.Id })
                });
            }
            catch
            {
                return Json(new { success = false, message = "Dữ liệu số lượng của sản phẩm đã thay đổi, vui lòng đổi lại sau." });
            }
        }


        //VNPAY
        [HttpPost]
        public IActionResult ProcessVNPayPayment(string receiverName, string receiverAddress, string city, string receiverPhone)
        {
            if (string.IsNullOrWhiteSpace(receiverName) || string.IsNullOrWhiteSpace(receiverAddress) ||
                string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(receiverPhone))
            {
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin giao hàng." });
            }

            var cartItems = HttpContext.Session.GetComplexData<List<GioHang>>(SD.SessionCart);
            if (cartItems == null || !cartItems.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });
            }

            // Tính tổng tiền
            var totalAmount = cartItems.Sum(c => c.SoLuong * c.Gia);

            // Tạo URL thanh toán qua VNPay
            var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, new VnPaymentRequestModel
            {
                Amount = Convert.ToInt32(totalAmount),
                OrderId = Guid.NewGuid().ToString(), // Tạo mã tạm thời để theo dõi thanh toán
                CreatedDate = DateTime.Now
            });

            // Lưu thông tin cần thiết để xử lý callback
            HttpContext.Session.SetComplexData("VNPayPendingPayment", new
            {
                ReceiverName = receiverName,
                ReceiverAddress = receiverAddress,
                City = city,
                ReceiverPhone = receiverPhone,
                CartItems = cartItems,
                TotalAmount = totalAmount
            });

            return Json(new { success = true, redirectUrl = paymentUrl });
        }



        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var vnpayData = HttpContext.Request.Query;
            var response = _vnPayService.PaymentExecute(vnpayData);

            if (response.Success && response.TransactionId != "0")
            {
                var cartItems = HttpContext.Session.GetComplexData<List<GioHang>>(SD.SessionCart);

                var pendingPayment = HttpContext.Session.GetComplexData<dynamic>("VNPayPendingPayment");
                if (pendingPayment == null)
                {
                    return View("PaymentFailed", response);
                }

                // Tạo đơn hàng sau khi thanh toán thành công
                var order = new DonHang
                {
                    NguoiDungId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    NgayDatHang = DateTime.Now,
                    TongTienDonHang = (double)pendingPayment.TotalAmount,
                    TrangThaiDonHang = SD.StatusPending,
                    TrangThaiThanhToan = SD.PaymentStatusApproved,
                    TenNguoiNhan = pendingPayment.ReceiverName,
                    Duong = pendingPayment.ReceiverAddress,
                    ThanhPho = pendingPayment.City,
                    Tinh = "Không xác định",
                    MaBuuDien = "00000",
                    SoDienThoai = pendingPayment.ReceiverPhone,
                    NgayThanhToan = DateTime.Now,
                    MaPhienThanhToan = response.TransactionId
                };

                _unitOfWork.DonHang.Add(order);
                _unitOfWork.Save();

                List<ChiTietDonHang> chiTietDonHangs = new List<ChiTietDonHang>();
                foreach (var item in cartItems)
                {
                    if (item.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                    {
                        chiTietDonHangs.Add(new ChiTietDonHang
                        {
                            DonHangId = order.Id,
                            ComboId = item.MaCombo,
                            Combo = await _unitOfWork.ComboSanPham.Get(filter: cbo => cbo.Id == item.Id),
                            SoLuong = item.SoLuong,
                            Gia = (int)item.Gia,
                            LoaiDoiTuong = SD.ObjectDetailOrder_Combo
                        });
                    }
                    else
                    {
                        chiTietDonHangs.Add(new ChiTietDonHang
                        {
                            DonHangId = order.Id,
                            SanPhamId = item.MaSanPham,
                            SanPham = await _unitOfWork.SanPham.Get(filter: cbo => cbo.Id == item.Id),
                            SoLuong = item.SoLuong,
                            Gia = (int)item.Gia,
                            LoaiDoiTuong = SD.ObjectDetailOrder_SanPham
                        });
                    }
                }

                _unitOfWork.ChiTietDonHang.AddRange(chiTietDonHangs);

                foreach (var ctdh in chiTietDonHangs)
                {
                    if (ctdh.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                    {
                        var combo = await _unitOfWork.ComboSanPham.Get(filter: cbo => cbo.Id == ctdh.ComboId);
                        combo.SoLuong = combo.SoLuong - ctdh.SoLuong;
                    }
                    else
                    {
                        var sanPham = await _unitOfWork.SanPham.Get(filter: cbo => cbo.Id == ctdh.SanPhamId);
                        sanPham.SoLuong = sanPham.SoLuong - ctdh.SoLuong;
                    }
                }

                _unitOfWork.Save();
                HttpContext.Session.Remove("VNPayPendingPayment");
                HttpContext.Session.SetComplexData(SD.SessionCart, new List<GioHang>());

                try
                {
                    // Gửi email xác nhận đơn hàng
                    await SendOrderConfirmationEmail(order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi email xác nhận đơn hàng: {ex.Message}");
                }

                try
                {
                    var message = $"Đơn hàng #{order.Id} đã được thanh toán thành công.\n" +
                                  $"Phương thức thanh toán: Thanh toán bằng VNPay.\n" +
                                  $"Tổng tiền: {order.TongTienDonHang:N0} đ.\n" +
                                  $"Mã giao dịch: {response.TransactionId}.";
                    await _telegramService.SendMessageAsync(838657228, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi tin nhắn Telegram: {ex.Message}");
                }

                return View("OrderComplete", order);
            }

            return View("PaymentFailed", response);
        }


        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId, string reason)
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _unitOfWork.DonHang.Get(o => o.Id == orderId && o.NguoiDungId == maKhachHang);

            if (order == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại hoặc bạn không có quyền hủy đơn hàng này." });
            }

            if (order.TrangThaiDonHang != SD.StatusPending)
            {
                return Json(new { success = false, message = "Đơn hàng đã được xử lý và không thể hủy." });
            }

            order.TrangThaiThanhToan = SD.PaymentStatusRejected; //fail
            order.TrangThaiDonHang = SD.StatusCancelled;
            order.LyDoHuyDonHang = reason;

            _unitOfWork.DonHang.Update(order);
            await Task.Run(() => _unitOfWork.Save());

            return Json(new { success = true, message = "Đơn hàng đã được hủy thành công." });
        }


        public async Task<IActionResult> OrderComplete(int orderId)
        {
            var orderDetail = await _unitOfWork.DonHang.Get(
                filter: o => o.Id == orderId,
                includeProperties: "ChiTietDonHangs,ChiTietDonHangs.SanPham,ChiTietDonHangs.Combo"
            );

            if (orderDetail == null)
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            return View(orderDetail);
        }

        public IActionResult Policy()
        {
            return View();
        }
        public async Task<IActionResult> ExportBill(int orderId)
        {
            // Lấy thông tin đơn hàng
            var order = (await _unitOfWork.DonHang.Get(
                includeProperties: "NguoiDungUngDung,ChiTietDonHangs.SanPham,ChiTietDonHangs.Combo",
                filter: dh => dh.Id == orderId));

            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại.");
            }

            // Tạo PDF
            using (var memoryStream = new MemoryStream())
            {
                // Khởi tạo tài liệu PDF
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Sử dụng font hỗ trợ tiếng Việt
                var fontPath = Path.Combine("wwwroot", "fonts", "Merriweather", "Merriweather-Regular.ttf");
                PdfFont font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                document.SetFont(font);

                // Thêm tiêu đề hóa đơn
                document.Add(new Paragraph("HÓA ĐƠN BÁN HÀNG")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SimulateBold());

                // Thông tin cửa hàng
                document.Add(new Paragraph("Cửa Hàng Cây Cảnh Online")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(14)
                    .SetMarginBottom(20));
                document.Add(new Paragraph("Địa chỉ: Ngõ 6 Hà Duy Tập, Buôn Ma Thuột, Đắk Lắk")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12));
                document.Add(new Paragraph("Số điện thoại: (024) 7300 1955 - Website: Caodang@fpt.edu.vn")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SetMarginBottom(20));

                // Thông tin khách hàng
                document.Add(new Paragraph($"Thông Tin Khách Hàng")
                    .SetFontSize(14)
                    .SimulateBold()
                    .SetMarginBottom(10));
                document.Add(new Paragraph($"Tên Người Nhận: {order.TenNguoiNhan}"));
                document.Add(new Paragraph($"Số Điện Thoại: {order.SoDienThoai}"));
                document.Add(new Paragraph($"Địa Chỉ: {order.Duong}, {order.ThanhPho}, {order.Tinh}, {order.MaBuuDien}"));
                document.Add(new Paragraph($"Ngày Đặt Hàng: {order.NgayDatHang:dd/MM/yyyy}")
                    .SetMarginBottom(20));

                // Bảng chi tiết đơn hàng
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2 }))
                    .UseAllAvailableWidth();
                table.AddHeaderCell(new Cell().Add(new Paragraph("Sản Phẩm").SimulateBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Số Lượng").SimulateBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Giá").SimulateBold()));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Tổng").SimulateBold()));

                double totalAmount = 0;
                foreach (var item in order.ChiTietDonHangs)
                {
                    double itemTotal = item.Gia * item.SoLuong;
                    totalAmount += itemTotal;

                    string productName = item.LoaiDoiTuong == SD.ObjectDetailOrder_SanPham
                        ? item.SanPham.TenSanPham
                        : item.Combo.TenCombo;

                    table.AddCell(new Cell().Add(new Paragraph(productName)));
                    table.AddCell(new Cell().Add(new Paragraph(item.SoLuong.ToString())))
                        .SetTextAlignment(TextAlignment.CENTER);
                    table.AddCell(new Cell().Add(new Paragraph($"{String.Format("{0:n0}", item.Gia)} đ")));
                    table.AddCell(new Cell().Add(new Paragraph($"{String.Format("{0:n0}", itemTotal)} đ")));
                }

                document.Add(table.SetMarginBottom(20));

                // Tổng tiền và trạng thái thanh toán
                document.Add(new Paragraph($"Tổng Tiền: {String.Format("{0:n0}", totalAmount)} đ")
                    .SimulateBold()
                    .SetFontSize(14));
                document.Add(new Paragraph($"Trạng Thái Thanh Toán: {SD.OrderStatusDictionary[order.TrangThaiThanhToan]}")
                    .SetFontSize(12)
                    .SetMarginBottom(20));


                // Cảm ơn khách hàng
                document.Add(new Paragraph("Cảm ơn quý khách đã mua hàng tại Cửa Hàng Cây Cảnh Online!")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(12)
                    .SimulateItalic());

                // Kết thúc tài liệu
                document.Close();

                // Trả về file PDF
                byte[] pdfBytes = memoryStream.ToArray();
                return File(pdfBytes, "application/pdf", $"hoa_don_{orderId}.pdf");
            }
        }
        public async Task<IActionResult> TrackOrder(int? orderId)
        {
            DonHang donHang = await _unitOfWork.DonHang.Get(filter: o => o.Id == orderId);
            return View(donHang);
        }

        #region//NonAction
        [NonAction]
        private async Task SendOrderConfirmationEmail(DonHang order)
        {
            // Tạo nội dung email
            var emailSubject = $"Xác nhận đơn hàng #{order.Id}";
            var emailBody = new StringBuilder();

            emailBody.AppendLine($"<h3>Cảm ơn bạn đã đặt hàng tại Cửa Hàng Cây Cảnh Online!</h3>");
            emailBody.AppendLine($"<p>Thông tin đơn hàng của bạn:</p>");
            emailBody.AppendLine($"<p>Mã đơn hàng: <b>#{order.Id}</b></p>");
            emailBody.AppendLine($"<p>Ngày đặt hàng: {order.NgayDatHang:dd/MM/yyyy}</p>");
            emailBody.AppendLine($"<p>Tổng tiền: <b>{order.TongTienDonHang:n0} đ</b></p>");
            emailBody.AppendLine($"<p>Trạng thái thanh toán: <b>{order.TrangThaiThanhToan}</b></p>");
            emailBody.AppendLine("<hr>");
            emailBody.AppendLine("<h4>Chi tiết đơn hàng:</h4>");

            foreach (var item in order.ChiTietDonHangs)
            {
                string productName = item.LoaiDoiTuong == SD.ObjectDetailOrder_SanPham
                    ? item.SanPham.TenSanPham
                    : item.Combo.TenCombo;

                emailBody.AppendLine($"<p>- {productName}: {item.SoLuong} x {item.Gia:n0} đ</p>");
            }

            emailBody.AppendLine("<hr>");
            emailBody.AppendLine($"<p>Người nhận: {order.TenNguoiNhan}</p>");
            emailBody.AppendLine($"<p>Địa chỉ: {order.Duong}, {order.ThanhPho}, {order.Tinh}, {order.MaBuuDien}</p>");
            emailBody.AppendLine($"<p>Số điện thoại: {order.SoDienThoai}</p>");
            emailBody.AppendLine("<p>Cảm ơn bạn đã mua hàng!</p>");

            // Gửi email
            await _emailSender.SendEmailAsync(order.NguoiDungUngDung.Email, emailSubject, emailBody.ToString());
        }
        #endregion

    }
}
