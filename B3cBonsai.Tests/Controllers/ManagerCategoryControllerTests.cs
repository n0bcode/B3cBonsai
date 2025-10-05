
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsaiWeb.Areas.Employee.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests
{
    public class ManagerCategoryControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly ManagerCategoryController _controller;

        public ManagerCategoryControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _controller = new ManagerCategoryController(_mockUnitOfWork.Object);
        }

        [Fact]
        public void Index_ReturnsAViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Upsert_Get_ForNewCategory_ReturnsPartialViewWithNewCategory()
        {
            // Arrange
            int? id = null;

            // Act
            var result = await _controller.Upsert(id);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsType<DanhMucSanPham>(partialViewResult.Model);
            Assert.Equal(0, model.Id);
        }

        [Fact]
        public async Task Upsert_Get_ForExistingCategory_ReturnsPartialViewWithCorrectCategory()
        {
            // Arrange
            int categoryId = 5;
            var category = new DanhMucSanPham { Id = categoryId, TenDanhMuc = "Bonsai Lớn" };
            _mockUnitOfWork.Setup(uow => uow.DanhMucSanPham.Get(u => u.Id == categoryId, null, false)).ReturnsAsync(category);

            // Act
            var result = await _controller.Upsert(categoryId);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            var model = Assert.IsType<DanhMucSanPham>(partialViewResult.Model);
            Assert.Equal(categoryId, model.Id);
            Assert.Equal("Bonsai Lớn", model.TenDanhMuc);
        }

        [Fact]
        public async Task Upsert_Post_WithInvalidModel_ReturnsJsonFalse()
        {
            // Arrange
            var category = new DanhMucSanPham { Id = 1, TenDanhMuc = "" }; // Invalid name
            _controller.ModelState.AddModelError("TenDanhMuc", "The TenDanhMuc field is required.");

            // Act
            var result = await _controller.Upsert(category);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = (bool)jsonResult.Value.GetType().GetProperty("success").GetValue(jsonResult.Value, null);
            Assert.False(success);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Never);
        }

        [Fact]
        public async Task Upsert_Post_ForNewCategory_AddsCategoryAndReturnsJsonSuccess()
        {
            // Arrange
            var newCategory = new DanhMucSanPham { Id = 0, TenDanhMuc = "New Category" };

            // Act
            var result = await _controller.Upsert(newCategory);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.DanhMucSanPham.Add(newCategory), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = (bool)jsonResult.Value.GetType().GetProperty("success").GetValue(jsonResult.Value, null);
            Assert.True(success);
        }

        [Fact]
        public async Task Upsert_Post_ForExistingCategory_UpdatesCategoryAndReturnsJsonSuccess()
        {
            // Arrange
            var existingCategory = new DanhMucSanPham { Id = 7, TenDanhMuc = "Updated Category" };

            // Act
            var result = await _controller.Upsert(existingCategory);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.DanhMucSanPham.Update(existingCategory), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = (bool)jsonResult.Value.GetType().GetProperty("success").GetValue(jsonResult.Value, null);
            Assert.True(success);
        }

        [Fact]
        public async Task GetAllCategories_ReturnsJsonWithCategoryList()
        {
            // Arrange
            var categories = new List<DanhMucSanPham>
            {
                new DanhMucSanPham { Id = 1, TenDanhMuc = "Bonsai Mini" },
                new DanhMucSanPham { Id = 2, TenDanhMuc = "Bonsai Tầm Trung" }
            };
            _mockUnitOfWork.Setup(uow => uow.DanhMucSanPham.GetAll(null, null)).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetAllCategories();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<DanhMucSanPham>>((jsonResult.Value.GetType().GetProperty("data").GetValue(jsonResult.Value, null)));
            Assert.Equal(2, data.Count());
        }

        [Fact]
        public async Task Delete_DeletesCategoryAndReturnsJsonSuccess()
        {
            // Arrange
            int categoryId = 10;
            var categoryToDelete = new DanhMucSanPham { Id = categoryId, TenDanhMuc = "To Delete" };
            var allCategories = new List<DanhMucSanPham> { categoryToDelete, new DanhMucSanPham(), new DanhMucSanPham() }; // Total > 2
            var productsInCategory = new List<SanPham>(); // No products in category

            _mockUnitOfWork.Setup(uow => uow.DanhMucSanPham.Get(c => c.Id == categoryId, null, false)).ReturnsAsync(categoryToDelete);
            _mockUnitOfWork.Setup(uow => uow.DanhMucSanPham.GetAll(null, null)).ReturnsAsync(allCategories);
            _mockUnitOfWork.Setup(uow => uow.SanPham.GetAll(p => p.DanhMucId == categoryId, null)).ReturnsAsync(productsInCategory);

            // Act
            var result = await _controller.Delete(categoryId, null);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.DanhMucSanPham.Remove(categoryToDelete), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
            var jsonResult = Assert.IsType<JsonResult>(result);
            var success = (bool)jsonResult.Value.GetType().GetProperty("success").GetValue(jsonResult.Value, null);
            Assert.True(success);
        }
    }
}
