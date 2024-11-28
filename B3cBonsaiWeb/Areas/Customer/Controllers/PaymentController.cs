using Microsoft.AspNetCore.Mvc;
using B3cBonsai.DataAccess.Repository.IRepository;
using System.Security.Claims;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsai.Utility.Extentions;
using B3cBonsaiWeb.Services;
using B3cBonsai.Models.ViewModels;
using System.Text.RegularExpressions;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TelegramService _telegramService;
        private readonly IVnPayService _vnPayService;

        public PaymentController(IUnitOfWork unitOfWork, TelegramService telegramService, IVnPayService vnPayService)
        {
            _unitOfWork = unitOfWork;
            _telegramService = telegramService;
            _vnPayService = vnPayService;


        }

        public IActionResult Index()
        {
            string? maKhachHang = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

            foreach (var item in cartItems)
            {
                if (item.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                {
                    _unitOfWork.ChiTietDonHang.Add(new ChiTietDonHang
                    {
                        DonHangId = donHang.Id,
                        ComboId = item.MaCombo,
                        Combo = await _unitOfWork.ComboSanPham.Get(filter: cbo => cbo.Id == item.Id),
                        SoLuong = item.SoLuong,
                        Gia = (int)item.Gia,
                        LoaiDoiTuong = SD.ObjectDetailOrder_Combo
                    });
                } else { 
                    _unitOfWork.ChiTietDonHang.Add(new ChiTietDonHang
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

            return Json(new
            {
                success = true,
                message = "Thanh toán khi nhận hàng thành công!",
                redirectUrl = Url.Action("OrderComplete", "Payment", new { area = "Customer", orderId = donHang.Id })
            });
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

                foreach (var item in cartItems)
                {
                    if (item.LoaiDoiTuong == SD.ObjectDetailOrder_Combo)
                    {
                        _unitOfWork.ChiTietDonHang.Add(new ChiTietDonHang
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
                        _unitOfWork.ChiTietDonHang.Add(new ChiTietDonHang
                        {
                            DonHangId = order.Id,
                            SanPhamId = item.MaSanPham,
                            SanPham = await _unitOfWork.SanPham.Get(filter: sp => sp.Id == item.Id),
                            SoLuong = item.SoLuong,
                            Gia = (int)item.Gia,
                            LoaiDoiTuong = SD.ObjectDetailOrder_SanPham
                        });
                    }
                }

                _unitOfWork.Save();
                HttpContext.Session.Remove("VNPayPendingPayment");
                HttpContext.Session.SetComplexData(SD.SessionCart, new List<GioHang>());

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
        public async Task<IActionResult> CancelOrder(int orderId)
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

            order.TrangThaiDonHang = SD.StatusCancelled;
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
        public async Task<IActionResult> TrackOrder(int? orderId)
        {
            DonHang donHang = await _unitOfWork.DonHang.Get(filter: o => o.Id == orderId);
            return View(donHang);
        }
    }
}
