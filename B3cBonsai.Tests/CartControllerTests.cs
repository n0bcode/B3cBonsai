using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsaiWeb.Areas.Customer.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests
{
    public class CartControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly CartController _controller;
        private readonly Mock<ISession> _sessionMock;

        public CartControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _controller = new CartController(_unitOfWorkMock.Object, _userManagerMock.Object);

            _sessionMock = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _sessionMock.Object;
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            SetupUser("user123", SD.Role_Customer);
        }

        private void SetupUser(string userId, string role)
        {
            var claims = new List<Claim>();
            if (userId != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext.HttpContext.User = claimsPrincipal;
        }

        private void SetupSession(List<GioHang> cartItems)
        {
            var cartData = JsonSerializer.SerializeToUtf8Bytes(cartItems);
            _sessionMock.Setup(s => s.TryGetValue(SD.SessionCart, out cartData)).Returns(true);
        }

        [Fact]
        public async Task Add_UserNotLoggedIn_ReturnsJsonError()
        {
            // Arrange
            SetupUser(null, null);

            // Act
            var result = await _controller.Add(1, null, 1, SD.ObjectDetailOrder_SanPham);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(false);
            data.GetType().GetProperty("message").GetValue(data).Should().Be("Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.");
        }

        [Fact]
        public async Task Add_ProductNotFound_ReturnsJsonError()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.SanPham.Get(It.IsAny<Expression<System.Func<SanPham, bool>>>(), null, false)).ReturnsAsync((SanPham)null);

            // Act
            var result = await _controller.Add(1, null, 1, SD.ObjectDetailOrder_SanPham);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(false);
            data.GetType().GetProperty("message").GetValue(data).Should().Be("Không tìm thấy sản phẩm trong hệ thống.");
        }

        [Fact]
        public async Task Add_ProductAlreadyInCart_ReturnsJsonError()
        {
            // Arrange
            SetupSession(new List<GioHang> { new GioHang { MaSanPham = 1, MaKhachHang = "user123" } });

            // Act
            var result = await _controller.Add(1, null, 1, SD.ObjectDetailOrder_SanPham);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(false);
            data.GetType().GetProperty("message").GetValue(data).Should().Be("Sản phẩm đã thêm vào giỏ hàng.");
        }

        [Fact]
        public async Task Add_Successful_ReturnsJsonSuccess()
        {
            // Arrange
            var product = new SanPham { Id = 1, TenSanPham = "Test Product", Gia = 100, SoLuong = 10, TrangThai = true };
            _unitOfWorkMock.Setup(u => u.SanPham.Get(It.IsAny<Expression<System.Func<SanPham, bool>>>(), null, false)).ReturnsAsync(product);
            _unitOfWorkMock.Setup(u => u.HinhAnhSanPham.Get(It.IsAny<Expression<System.Func<HinhAnhSanPham, bool>>>(), null, false)).ReturnsAsync(new HinhAnhSanPham { LinkAnh = "/test.jpg" });

            byte[] outValue = null;
            _sessionMock.Setup(s => s.TryGetValue(SD.SessionCart, out outValue)).Returns(false);

            // Act
            var result = await _controller.Add(1, null, 1, SD.ObjectDetailOrder_SanPham);

            // Assert
            _sessionMock.Verify(s => s.Set(SD.SessionCart, It.IsAny<byte[]>()), Times.Once);
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(true);
        }

        [Fact]
        public async Task Remove_RemovesItemFromCart()
        {
            // Arrange
            var cartItems = new List<GioHang> { new GioHang { Id = 1, MaSanPham = 1, MaKhachHang = "user123" } };
            SetupSession(cartItems);

            // Act
            var result = await _controller.Remove(1);

            // Assert
            _sessionMock.Verify(s => s.Set(SD.SessionCart, It.Is<byte[]>(b => !JsonSerializer.Deserialize<List<GioHang>>(b).Any(i => i.Id == 1))), Times.Once);
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(true);
        }

        [Fact]
        public async Task Plus_IncreasesQuantityOfItemInCart()
        {
            // Arrange
            var cartItems = new List<GioHang> { new GioHang { Id = 1, MaSanPham = 1, SoLuong = 1, MaKhachHang = "user123", SanPham = new SanPham { SoLuong = 5 } } };
            SetupSession(cartItems);

            // Act
            var result = await _controller.Plus(1);

            // Assert
            _sessionMock.Verify(s => s.Set(SD.SessionCart, It.IsAny<byte[]>()), Times.Once);
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(true);
        }

        [Fact]
        public async Task Minus_DecreasesQuantityOfItemInCart()
        {
            // Arrange
            var cartItems = new List<GioHang> { new GioHang { Id = 1, MaSanPham = 1, SoLuong = 2, MaKhachHang = "user123" } };
            SetupSession(cartItems);

            // Act
            var result = await _controller.Minus(1);

            // Assert
            _sessionMock.Verify(s => s.Set(SD.SessionCart, It.IsAny<byte[]>()), Times.Once);
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(true);
        }

        [Fact]
        public async Task Minus_ItemWithQuantityOne_ReturnsError()
        {
            // Arrange
            var cartItems = new List<GioHang> { new GioHang { Id = 1, MaSanPham = 1, SoLuong = 1, MaKhachHang = "user123" } };
            SetupSession(cartItems);

            // Act
            var result = await _controller.Minus(1);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(false);
            data.GetType().GetProperty("message").GetValue(data).Should().Be("Số lượng không thể giảm dưới 1.");
        }
    }
}