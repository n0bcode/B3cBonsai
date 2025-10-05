using B3cBonsai.Models.ViewModels;
using B3cBonsai.Utility.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace B3cBonsai.Tests.Services
{
    public class VnPayServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly VnPayService _vnPayService;

        public VnPayServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _vnPayService = new VnPayService(_mockConfig.Object);
        }

        [Fact]
        public void CreatePaymentUrl_ReturnsValidPaymentUrl()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);

            _mockConfig.Setup(x => x["VnPay:Version"]).Returns("2.1.0");
            _mockConfig.Setup(x => x["VnPay:Command"]).Returns("pay");
            _mockConfig.Setup(x => x["VnPay:TmnCode"]).Returns("TestTmnCode");
            _mockConfig.Setup(x => x["VnPay:CurrCode"]).Returns("VND");
            _mockConfig.Setup(x => x["VnPay:Locale"]).Returns("vn");
            _mockConfig.Setup(x => x["VnPay:PaymentBackReturnUrl"]).Returns("http://example.com/vnpay_return");
            _mockConfig.Setup(x => x["VnPay:BaseUrl"]).Returns("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html");
            _mockConfig.Setup(x => x["VnPay:HashSecret"]).Returns("testsecurehashsecret");

            var model = new VnPaymentRequestModel
            {
                Amount = 100000,
                CreatedDate = DateTime.Now,
                OrderId = "12345"
            };

            // Act
            var result = _vnPayService.CreatePaymentUrl(mockHttpContext.Object, model);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html");
            result.Should().Contain("vnp_Version=2.1.0");
            result.Should().Contain("vnp_Command=pay");
            result.Should().Contain("vnp_TmnCode=TestTmnCode");
            result.Should().Contain("vnp_CurrCode=VND");
        }

        [Fact]
        public void PaymentExecute_WithValidSignature_ReturnsSuccessResponse()
        {
            // Arrange
            var collections = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["vnp_TxnRef"] = "123456789",
                ["vnp_TransactionNo"] = "987654321",
                ["vnp_OrderInfo"] = "Thanh toán cho đơn hàng:123",
                ["vnp_ResponseCode"] = "00",
                ["vnp_SecureHash"] = "validhash"
            });

            _mockConfig.Setup(x => x["VnPay:HashSecret"]).Returns("testsecurehashsecret");

            // Act
            var result = _vnPayService.PaymentExecute(collections);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.PaymentMethod.Should().Be("VnPay");
            result.OrderId.Should().Be("123456789");
            result.TransactionId.Should().Be("987654321");
            result.VnPayResponseCode.Should().Be("00");
        }

        [Fact]
        public void PaymentExecute_WithInvalidSignature_ReturnsFailureResponse()
        {
            // Arrange
            var collections = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                ["vnp_TxnRef"] = "123456789",
                ["vnp_TransactionNo"] = "987654321",
                ["vnp_OrderInfo"] = "Thanh toán cho đơn hàng:123",
                ["vnp_ResponseCode"] = "00",
                ["vnp_SecureHash"] = "invalidhash"
            });

            _mockConfig.Setup(x => x["VnPay:HashSecret"]).Returns("testsecurehashsecret");

            // Act
            var result = _vnPayService.PaymentExecute(collections);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
        }
    }
}
