using B3cBonsai.DataAccess.Data;
using B3cBonsai.Models;
using B3cBonsai.Models.ViewModels;
using B3cBonsai.Utility.Services;
using B3cBonsaiWeb.Areas.Employee.Controllers.Staff;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests
{
    public class ManagerProductControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly Mock<IImageStorageService> _imageStorageServiceMock;

        public ManagerProductControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _imageStorageServiceMock = new Mock<IImageStorageService>();
        }

        private ApplicationDbContext GetDatabaseContext()
        {
            var context = new ApplicationDbContext(_options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task Upsert_POST_CreatesNewProduct_WithImages()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var controller = new ManagerProductController(context, _imageStorageServiceMock.Object);

            var newProductVM = new SanPhamVM
            {
                SanPham = new SanPham { Id = 0, TenSanPham = "New Product" }
            };

            var mockImageFile = new Mock<IFormFile>();
            mockImageFile.Setup(f => f.FileName).Returns("test.jpg");
            mockImageFile.Setup(f => f.Length).Returns(1);
            mockImageFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var images = new[] { mockImageFile.Object };
            var newImageUrls = new List<string> { "/images/product/new_image.jpg" };

            _imageStorageServiceMock.Setup(s => s.StoreImagesAsync(images, "product")).ReturnsAsync(newImageUrls);

            // Act
            var result = await controller.Upsert(newProductVM, images);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            dynamic successData = jsonResult.Value;
            ((bool)successData.GetType().GetProperty("success").GetValue(successData, null)).Should().BeTrue();

            // Verify product was added
            var addedProduct = await context.SanPhams.FirstOrDefaultAsync(p => p.TenSanPham == "New Product");
            addedProduct.Should().NotBeNull();

            // Verify image was added
            var addedImage = await context.HinhAnhSanPhams.FirstOrDefaultAsync(i => i.SanPhamId == addedProduct.Id);
            addedImage.Should().NotBeNull();
            addedImage.LinkAnh.Should().Be(newImageUrls.First());

            // Verify image service was called
            _imageStorageServiceMock.Verify(s => s.StoreImagesAsync(images, "product"), Times.Once);
        }

        [Fact]
        public async Task Upsert_POST_UpdatesExistingProduct_AndReplacesImages()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var controller = new ManagerProductController(context, _imageStorageServiceMock.Object);

            // Seed database with an existing product and image
            var existingProduct = new SanPham { Id = 1, TenSanPham = "Old Product" };
            var oldImage = new HinhAnhSanPham { Id = 1, SanPhamId = 1, LinkAnh = "/images/product/old_image.jpg" };
            context.SanPhams.Add(existingProduct);
            context.HinhAnhSanPhams.Add(oldImage);
            await context.SaveChangesAsync();

            var updatedProductVM = new SanPhamVM
            {
                SanPham = new SanPham { Id = 1, TenSanPham = "Updated Product" }
            };

            var mockImageFile = new Mock<IFormFile>();
            mockImageFile.Setup(f => f.FileName).Returns("new.jpg");
            var newImages = new[] { mockImageFile.Object };
            var newImageUrls = new List<string> { "/images/product/new_image.jpg" };

            _imageStorageServiceMock.Setup(s => s.StoreImagesAsync(newImages, "product")).ReturnsAsync(newImageUrls);

            // Act
            var result = await controller.Upsert(updatedProductVM, newImages);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            dynamic successData = jsonResult.Value;
            ((bool)successData.GetType().GetProperty("success").GetValue(successData, null)).Should().BeTrue();

            // Verify product was updated
            var updatedProduct = await context.SanPhams.FindAsync(1);
            updatedProduct.TenSanPham.Should().Be("Updated Product");

            // Verify old image was deleted and new image was added
            var imagesInDb = await context.HinhAnhSanPhams.Where(i => i.SanPhamId == 1).ToListAsync();
            imagesInDb.Should().HaveCount(1);
            imagesInDb.First().LinkAnh.Should().Be(newImageUrls.First());

            // Verify image service calls
            _imageStorageServiceMock.Verify(s => s.DeleteImageAsync(oldImage.LinkAnh), Times.Once);
            _imageStorageServiceMock.Verify(s => s.StoreImagesAsync(newImages, "product"), Times.Once);
        }

        [Fact]
        public async Task Delete_RemovesProductAndRelatedData()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var controller = new ManagerProductController(context, _imageStorageServiceMock.Object);
            int productId = 1;

            // Seed database
            context.SanPhams.Add(new SanPham { Id = productId, TenSanPham = "Product to Delete" });
            context.BinhLuans.Add(new BinhLuan { Id = 1, SanPhamId = productId, NoiDungBinhLuan = "c" });
            context.DanhSachYeuThichs.Add(new DanhSachYeuThich { Id = 1, SanPhamId = productId, NguoiDungId = "u" });
            context.HinhAnhSanPhams.Add(new HinhAnhSanPham { Id = 1, SanPhamId = productId, LinkAnh = "i" });
            context.VideoSanPhams.Add(new VideoSanPham { Id = 1, SanPhamId = productId, LinkVideo = "v" });
            await context.SaveChangesAsync();

            // Act
            var result = controller.Delete(productId);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            dynamic successData = jsonResult.Value;
            ((bool)successData.GetType().GetProperty("success").GetValue(successData, null)).Should().BeTrue();

            // Verify all related data is deleted
            (await context.SanPhams.AnyAsync(p => p.Id == productId)).Should().BeFalse();
            (await context.BinhLuans.AnyAsync(p => p.SanPhamId == productId)).Should().BeFalse();
            (await context.DanhSachYeuThichs.AnyAsync(p => p.SanPhamId == productId)).Should().BeFalse();
            (await context.HinhAnhSanPhams.AnyAsync(p => p.SanPhamId == productId)).Should().BeFalse();
            (await context.VideoSanPhams.AnyAsync(p => p.SanPhamId == productId)).Should().BeFalse();
        }

        [Fact]
        public async Task ChangeStatus_TogglesProductStatus()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var controller = new ManagerProductController(context, _imageStorageServiceMock.Object);
            int productId = 1;

            // Seed database
            context.SanPhams.Add(new SanPham { Id = productId, TenSanPham = "Test Product", TrangThai = true });
            await context.SaveChangesAsync();

            // Act & Assert - First toggle (true -> false)
            var result1 = controller.ChangeStatus(productId);
            var jsonResult1 = result1.Should().BeOfType<JsonResult>().Subject;
            dynamic successData1 = jsonResult1.Value;
            ((bool)successData1.GetType().GetProperty("success").GetValue(successData1, null)).Should().BeTrue();
            (await context.SanPhams.FindAsync(productId)).TrangThai.Should().BeFalse();

            // Act & Assert - Second toggle (false -> true)
            var result2 = controller.ChangeStatus(productId);
            var jsonResult2 = result2.Should().BeOfType<JsonResult>().Subject;
            dynamic successData2 = jsonResult2.Value;
            ((bool)successData2.GetType().GetProperty("success").GetValue(successData2, null)).Should().BeTrue();
            (await context.SanPhams.FindAsync(productId)).TrangThai.Should().BeTrue();
        }
    }
}
