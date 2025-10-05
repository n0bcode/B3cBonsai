
using System.Linq.Expressions;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsaiWeb.Areas.Employee.Controllers.Staff;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests
{
    public class ManagerCommentControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ManagerCommentController _controller;

        public ManagerCommentControllerTests()
        {
            // Arrange - Setup
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new ManagerCommentController(_unitOfWorkMock.Object);
        }

        [Fact]
        public void Index_WhenCalled_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsJsonResultWithData()
        {
            // Arrange
            var comments = new List<BinhLuan>
            {
                new BinhLuan { Id = 1, NoiDungBinhLuan = "Comment 1", SanPham = new SanPham { Id = 1, TenSanPham = "Product 1", HinhAnhs = new List<HinhAnhSanPham>() } },
                new BinhLuan { Id = 2, NoiDungBinhLuan = "Comment 2", SanPham = new SanPham { Id = 2, TenSanPham = "Product 2", HinhAnhs = new List<HinhAnhSanPham>() } }
            };
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.GetAll(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.Is<string>(s => s == "SanPham.HinhAnhs"))).ReturnsAsync(comments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            
            var data = jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value, null);
            data.Should().BeEquivalentTo(comments);
        }
        [Fact]
        public async Task ChangeOneStatus_WhenCommentExists_ReturnsSuccessJson()
        {
            // Arrange
            var comment = new BinhLuan { Id = 1, TinhTrang = true };
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.Get(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(comment);

            // Act
            var result = await _controller.ChangeOneStatus(1);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeTrue();

            _unitOfWorkMock.Verify(uow => uow.BinhLuan.Update(comment), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }
        [Fact]
        public async Task ChangeOneStatus_WhenCommentDoesNotExist_ReturnsNotFoundJson()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.Get(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync((BinhLuan)null);

            // Act
            var result = await _controller.ChangeOneStatus(1);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeFalse();

            _unitOfWorkMock.Verify(uow => uow.BinhLuan.Update(It.IsAny<BinhLuan>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
        [Fact]
        public async Task DeleteOne_WhenCommentExists_ReturnsSuccessJson()
        {
            // Arrange
            var comment = new BinhLuan { Id = 1 };
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.Get(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(comment);

            // Act
            var result = await _controller.DeleteOne(1);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeTrue();

            _unitOfWorkMock.Verify(uow => uow.BinhLuan.Remove(comment), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }
        [Fact]
        public async Task DeleteOne_WhenCommentDoesNotExist_ReturnsNotFoundJson()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.Get(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync((BinhLuan)null);

            // Act
            var result = await _controller.DeleteOne(1);

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeFalse();

            _unitOfWorkMock.Verify(uow => uow.BinhLuan.Remove(It.IsAny<BinhLuan>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
        [Fact]
        public async Task ChangeManyStatus_WhenCommentsExist_ReturnsSuccessJson()
        {
            // Arrange
            var comments = new List<BinhLuan>
            {
                new BinhLuan { Id = 1, TinhTrang = true },
                new BinhLuan { Id = 2, TinhTrang = true }
            };
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.GetAll(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>())).ReturnsAsync(comments);

            // Act
            var result = await _controller.ChangeManyStatus(new List<int> { 1, 2 });

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeTrue();

            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }
        [Fact]
        public async Task ChangeManyStatus_WhenCommentsDoNotExist_ReturnsNotFoundJson()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.GetAll(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<BinhLuan>());

            // Act
            var result = await _controller.ChangeManyStatus(new List<int> { 1, 2 });

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeFalse();

            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
        [Fact]
        public async Task DeleteMany_WhenCommentsExist_ReturnsSuccessJson()
        {
            // Arrange
            var comments = new List<BinhLuan>
            {
                new BinhLuan { Id = 1 },
                new BinhLuan { Id = 2 }
            };
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.GetAll(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>())).ReturnsAsync(comments);

            // Act
            var result = await _controller.DeleteMany(new List<int> { 1, 2 });

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeTrue();

            _unitOfWorkMock.Verify(uow => uow.BinhLuan.RemoveRange(comments), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Once);
        }
        [Fact]
        public async Task DeleteMany_WhenCommentsDoNotExist_ReturnsNotFoundJson()
        {
            // Arrange
            _unitOfWorkMock.Setup(uow => uow.BinhLuan.GetAll(It.IsAny<Expression<Func<BinhLuan, bool>>>(), It.IsAny<string>())).ReturnsAsync(new List<BinhLuan>());

            // Act
            var result = await _controller.DeleteMany(new List<int> { 1, 2 });

            // Assert
            result.Should().BeOfType<JsonResult>();
            var jsonResult = result as JsonResult;
            jsonResult.Value.Should().NotBeNull();
            dynamic value = jsonResult.Value;
            ((bool)value.GetType().GetProperty("success").GetValue(value, null)).Should().BeFalse();

            _unitOfWorkMock.Verify(uow => uow.BinhLuan.RemoveRange(It.IsAny<IEnumerable<BinhLuan>>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.Save(), Times.Never);
        }
    }
}
