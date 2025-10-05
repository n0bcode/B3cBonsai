
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Models.ViewModels;
using B3cBonsai.Utility.Services;
using B3cBonsaiWeb.Areas.Employee.Controllers.Staff;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests
{
    public class ManagerProductControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IImageStorageService> _mockImageStorageService;
        private readonly ManagerProductController _controller;

        public ManagerProductControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockImageStorageService = new Mock<IImageStorageService>();
            _controller = new ManagerProductController(_mockUnitOfWork.Object, _mockImageStorageService.Object);
        }

        [Fact]
        public async Task Upsert_POST_CreatesNewProduct_WithImages()
        {
            // Arrange
            var newProductVM = new SanPhamVM
            {
                SanPham = new SanPham { Id = 0, TenSanPham = "New Product" }
            };

            var mockImageFile = new Mock<IFormFile>();
            var images = new[] { mockImageFile.Object };
            var newImageUrls = new List<string> { "/images/product/new_image.jpg" };

            _mockImageStorageService.Setup(s => s.StoreImagesAsync(images, "product")).ReturnsAsync(newImageUrls);

            // Act
            var result = await _controller.Upsert(newProductVM, images);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            dynamic successData = jsonResult.Value;
            ((bool)successData.GetType().GetProperty("success").GetValue(successData, null)).Should().BeTrue();

            _mockUnitOfWork.Verify(uow => uow.SanPham.Add(It.IsAny<SanPham>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.HinhAnhSanPham.Add(It.Is<HinhAnhSanPham>(i => i.LinkAnh == newImageUrls.First())), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Exactly(2)); // Once for product, once for images
            _mockImageStorageService.Verify(s => s.StoreImagesAsync(images, "product"), Times.Once);
        }

        [Fact]
        public async Task Upsert_POST_UpdatesExistingProduct_AndReplacesImages()
        {
            // Arrange
            var existingProduct = new SanPham { Id = 1, TenSanPham = "Old Product" };
            var oldImage = new HinhAnhSanPham { Id = 1, SanPhamId = 1, LinkAnh = "/images/product/old_image.jpg" };
            var oldImages = new List<HinhAnhSanPham> { oldImage };

            var updatedProductVM = new SanPhamVM
            {
                SanPham = new SanPham { Id = 1, TenSanPham = "Updated Product" }
            };

            var mockImageFile = new Mock<IFormFile>();
            var newImages = new[] { mockImageFile.Object };
            var newImageUrls = new List<string> { "/images/product/new_image.jpg" };

            _mockUnitOfWork.Setup(uow => uow.SanPham.Get(It.IsAny<Expression<System.Func<SanPham, bool>>>(), null, false)).ReturnsAsync(existingProduct);
            _mockUnitOfWork.Setup(uow => uow.HinhAnhSanPham.GetAll(It.IsAny<Expression<System.Func<HinhAnhSanPham, bool>>>(), null)).ReturnsAsync(oldImages);
            _mockImageStorageService.Setup(s => s.StoreImagesAsync(newImages, "product")).ReturnsAsync(newImageUrls);

            // Act
            var result = await _controller.Upsert(updatedProductVM, newImages);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            dynamic successData = jsonResult.Value;
            ((bool)successData.GetType().GetProperty("success").GetValue(successData, null)).Should().BeTrue();

            _mockUnitOfWork.Verify(uow => uow.SanPham.Update(It.Is<SanPham>(p => p.TenSanPham == "Updated Product")), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.HinhAnhSanPham.Remove(oldImage), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.HinhAnhSanPham.Add(It.Is<HinhAnhSanPham>(i => i.LinkAnh == newImageUrls.First())), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Exactly(3)); // Product, remove old images, add new images

            _mockImageStorageService.Verify(s => s.DeleteImageAsync(oldImage.LinkAnh), Times.Once);
            _mockImageStorageService.Verify(s => s.StoreImagesAsync(newImages, "product"), Times.Once);
        }

        [Fact]
        public async Task Delete_RemovesProductAndRelatedData()
        {
            // Arrange
            int productId = 1;
            var productToDelete = new SanPham { Id = productId };

            _mockUnitOfWork.Setup(uow => uow.SanPham.Get(It.IsAny<Expression<System.Func<SanPham, bool>>>(), null, false)).ReturnsAsync(productToDelete);
            _mockUnitOfWork.Setup(uow => uow.BinhLuan.GetAll(It.IsAny<Expression<System.Func<BinhLuan, bool>>>(), null)).ReturnsAsync(new List<BinhLuan>());
            // ... setup for other repositories as well

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            dynamic successData = jsonResult.Value;
            ((bool)successData.GetType().GetProperty("success").GetValue(successData, null)).Should().BeTrue();

            _mockUnitOfWork.Verify(uow => uow.SanPham.Remove(productToDelete), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Exactly(2));
        }

        [Fact]
        public async Task ChangeStatus_TogglesProductStatus()
        {
            // Arrange
            int productId = 1;
            var product = new SanPham { Id = productId, TrangThai = true };

            _mockUnitOfWork.Setup(uow => uow.SanPham.Get(It.IsAny<Expression<System.Func<SanPham, bool>>>(), null, false)).ReturnsAsync(product);

            // Act
            var result = await _controller.ChangeStatus(productId);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            dynamic successData = jsonResult.Value;
            ((bool)successData.GetType().GetProperty("success").GetValue(successData, null)).Should().BeTrue();

            _mockUnitOfWork.Verify(uow => uow.SanPham.Update(It.Is<SanPham>(p => p.TrangThai == false)), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }
    }
}
