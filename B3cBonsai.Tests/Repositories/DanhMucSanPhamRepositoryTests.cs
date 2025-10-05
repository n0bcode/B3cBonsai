using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace B3cBonsai.Tests
{
    public class DanhMucSanPhamRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public DanhMucSanPhamRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Update_UpdatesDanhMucSanPhamInDatabase()
        {
            // Arrange
            var danhMuc = new DanhMucSanPham { Id = 1, TenDanhMuc = "Initial Category" };
            using (var context = new ApplicationDbContext(_options))
            {
                context.DanhMucSanPhams.Add(danhMuc);
                await context.SaveChangesAsync();
            }

            // Act
            danhMuc.TenDanhMuc = "Updated Category";
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new DanhMucSanPhamRepository(context);
                repository.Update(danhMuc);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(_options))
            {
                context.DanhMucSanPhams.Should().HaveCount(1);
                context.DanhMucSanPhams.First().TenDanhMuc.Should().Be("Updated Category");
            }
        }

        [Fact]
        public void GetFirstOrDefault_ReturnsCorrectItem()
        {
            // Arrange
            var danhMucs = new List<DanhMucSanPham>
            {
                new DanhMucSanPham { Id = 1, TenDanhMuc = "Category 1" },
                new DanhMucSanPham { Id = 2, TenDanhMuc = "Category 2" }
            };
            using (var context = new ApplicationDbContext(_options))
            {
                context.DanhMucSanPhams.AddRange(danhMucs);
                context.SaveChanges();
            }

            // Act
            DanhMucSanPham result;
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new DanhMucSanPhamRepository(context);
                result = repository.GetFirstOrDefault(d => d.Id == 2);
            }

            // Assert
            result.Should().NotBeNull();
            result.TenDanhMuc.Should().Be("Category 2");
        }
    }
}
