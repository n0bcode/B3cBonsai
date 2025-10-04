using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;

using B3cBonsai.Utility;
using B3cBonsaiWeb.Areas.Customer.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests
{
    public class WishlistControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly WishlistController _controller;

        public WishlistControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new WishlistController(_unitOfWorkMock.Object);
            SetupUser("user123");
        }

        private void SetupUser(string userId)
        {
            var claims = new List<Claim>();
            if (userId != null)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task LikeOrNot_UserNotLoggedIn_ReturnsJsonError()
        {
            // Arrange
            SetupUser(null);

            // Act
            var result = await _controller.LikeOrNot(1, SD.ObjectLike_SanPham);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(false);
            data.GetType().GetProperty("message").GetValue(data).Should().Be("Bạn cần đăng nhập để sử dụng chức năng này!");
        }

        [Fact]
        public async Task LikeOrNot_InvalidData_ReturnsJsonError()
        {
            // Act
            var result = await _controller.LikeOrNot(null, SD.ObjectLike_SanPham);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(false);
            data.GetType().GetProperty("message").GetValue(data).Should().Be("Dữ liệu không hợp lệ.");
        }

        [Fact]
        public async Task LikeOrNot_ProductNotFound_ReturnsJsonError()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.DanhSachYeuThich.Get(It.IsAny<Expression<System.Func<DanhSachYeuThich, bool>>>(), null, false)).ReturnsAsync((DanhSachYeuThich)null);
            _unitOfWorkMock.Setup(u => u.SanPham.Get(It.IsAny<Expression<System.Func<SanPham, bool>>>(), null, false)).ReturnsAsync((SanPham)null);

            // Act
            var result = await _controller.LikeOrNot(1, SD.ObjectLike_SanPham);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(false);
            data.GetType().GetProperty("message").GetValue(data).Should().Be("Không tìm thấy sản phẩm.");
        }

        [Fact]
        public async Task LikeOrNot_LikeNewProduct_AddsToWishlist()
        {
            // Arrange
            var productId = 1;
            var userId = "user123";
            _unitOfWorkMock.Setup(u => u.DanhSachYeuThich.Get(It.IsAny<Expression<System.Func<DanhSachYeuThich, bool>>>(), null, false)).ReturnsAsync((DanhSachYeuThich)null);
            _unitOfWorkMock.Setup(u => u.SanPham.Get(It.IsAny<Expression<System.Func<SanPham, bool>>>(), null, false)).ReturnsAsync(new SanPham { Id = productId });
            _unitOfWorkMock.Setup(u => u.DanhSachYeuThich.Add(It.IsAny<DanhSachYeuThich>()));

            // Act
            var result = await _controller.LikeOrNot(productId, SD.ObjectLike_SanPham);

            // Assert
            _unitOfWorkMock.Verify(u => u.DanhSachYeuThich.Add(It.Is<DanhSachYeuThich>(d => d.SanPhamId == productId && d.NguoiDungId == userId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(true);
        }

        [Fact]
        public async Task LikeOrNot_UnlikeExistingProduct_RemovesFromWishlist()
        {
            // Arrange
            var productId = 1;
            var userId = "user123";
            var existingLike = new DanhSachYeuThich { Id = 1, SanPhamId = productId, NguoiDungId = userId, LoaiDoiTuong = SD.ObjectLike_SanPham };
            _unitOfWorkMock.Setup(u => u.DanhSachYeuThich.Get(It.IsAny<Expression<System.Func<DanhSachYeuThich, bool>>>(), null, false)).ReturnsAsync(existingLike);
            _unitOfWorkMock.Setup(u => u.DanhSachYeuThich.Remove(It.IsAny<DanhSachYeuThich>()));

            // Act
            var result = await _controller.LikeOrNot(productId, SD.ObjectLike_SanPham);

            // Assert
            _unitOfWorkMock.Verify(u => u.DanhSachYeuThich.Remove(existingLike), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value.Should().BeAssignableTo<object>().Subject;
            data.GetType().GetProperty("success").GetValue(data).Should().Be(true);
        }
    }
}
