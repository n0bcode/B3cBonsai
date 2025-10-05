using System.Security.Claims;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Models.ViewModels;
using B3cBonsai.Utility.Extentions;
using B3cBonsai.Utility.Services;
using B3cBonsaiWeb.Areas.Customer.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using Xunit;

namespace B3cBonsai.Tests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<TelegramService> _mockTelegramService;
        private readonly Mock<IVnPayService> _mockVnPayService;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly PaymentController _controller;

        public PaymentControllerTests()
        {
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTelegramService = new Mock<TelegramService>("fake_token");
            _mockVnPayService = new Mock<IVnPayService>();
            _mockEmailSender = new Mock<IEmailSender>();
            _controller = new PaymentController(
                _mockSignInManager.Object,
                _mockUnitOfWork.Object,
                _mockTelegramService.Object,
                _mockVnPayService.Object,
                _mockEmailSender.Object
            );

            // Setup controller context
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [Fact]
        public async Task Index_EmptyCart_RedirectsToCart()
        {
            // Arrange
            _controller.HttpContext.Session.SetComplexData(SD.SessionCart, new List<GioHang>());

            // Act
            var result = await _controller.Index();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Cart", redirectResult.ControllerName);
        }

        [Fact]
        public async Task Index_WithCart_ReturnsViewResult()
        {
            // Arrange
            var cartItems = new List<GioHang> { new GioHang { Gia = 100000 } };
            _controller.HttpContext.Session.SetComplexData(SD.SessionCart, cartItems);

            var user = new NguoiDungUngDung { Id = "test-user", LockoutEnd = null };
            _mockUnitOfWork.Setup(u => u.NguoiDungUngDung.Get(It.IsAny<System.Linq.Expressions.Expression<System.Func<NguoiDungUngDung, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(user);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task PaymentCallBack_WithValidPayment_ReturnsOrderCompleteView()
        {
            // Arrange
            var pendingPayment = new
            {
                ReceiverName = "Test User",
                ReceiverAddress = "Test Address",
                City = "Test City",
                ReceiverPhone = "123456789",
                CartItems = new List<GioHang>(),
                TotalAmount = 100000
            };

            _controller.HttpContext.Session.SetComplexData("VNPayPendingPayment", pendingPayment);

            var collections = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["vnp_TxnRef"] = "123",
                ["vnp_ResponseCode"] = "00",
                ["vnp_SecureHash"] = "validhash"
            });

            var response = new VnPaymentResponseModel
            {
                Success = true,
                OrderId = "123",
                TransactionId = "456"
            };

            _mockVnPayService.Setup(s => s.PaymentExecute(collections)).Returns(response);

            // Mock unit of work for order creation
            var order = new DonHang { Id = 123 };
            _mockUnitOfWork.Setup(u => u.DonHang.Add(It.IsAny<DonHang>())).Callback<DonHang>(o => order = o);
            _mockUnitOfWork.Setup(u => u.Save());

            // Act
            var result = await _controller.PaymentCallBack();

            // Assert - returns View("OrderComplete", order)
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("OrderComplete", viewResult.ViewName);
        }

        [Fact]
        public async Task PaymentCallBack_WithInvalidPayment_ReturnsPaymentFailedView()
        {
            // Arrange
            var collections = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["vnp_TxnRef"] = "123",
                ["vnp_ResponseCode"] = "01",
                ["vnp_SecureHash"] = "invalidhash"
            });

            var response = new VnPaymentResponseModel
            {
                Success = false,
                OrderId = "123",
                TransactionId = "456",
                VnPayResponseCode = "01"
            };

            _mockVnPayService.Setup(s => s.PaymentExecute(collections)).Returns(response);

            // Act
            var result = await _controller.PaymentCallBack();

            // Assert - returns View("PaymentFailed", response)
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("PaymentFailed", viewResult.ViewName);
        }
    }
}
