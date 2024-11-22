using B3cBonsai.Utility;
using Microsoft.AspNetCore.Mvc;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsaiWeb.Services;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutController(IVnPayService vnPayService, IUnitOfWork unitOfWork)
        {
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> PaymentCallBack()
        {
            var vnpayData = HttpContext.Request.Query;

            var response = _vnPayService.PaymentExecute(vnpayData);

            if (response.Success)
            {
                var order = await _unitOfWork.DonHang.Get(o => o.Id == int.Parse(response.OrderId));
                if (order != null)
                {
                    order.TrangThaiThanhToan = SD.PaymentStatusApproved;
                    order.NgayThanhToan = DateTime.Now;
                    _unitOfWork.DonHang.Update(order);
                    _unitOfWork.Save();
                }

                return View("PaymentSuccess", response);
            }

            return View("PaymentFailed", response);
        }
    }
}
