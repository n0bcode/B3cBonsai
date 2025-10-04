
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
    public class HinhAnhSanPhamRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public HinhAnhSanPhamRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Add_AddsImageToDatabase()
        {
            // Arrange
            var image = new HinhAnhSanPham { Id = 1, LinkAnh = "/images/test.jpg", SanPhamId = 1 };
            await using (var context = new ApplicationDbContext(_options))
            {
                // Need to add the product first to avoid foreign key constraint issues
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                await context.SaveChangesAsync();

                var repository = new HinhAnhSanPhamRepository(context);

                // Act
                repository.Add(image);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.HinhAnhSanPhams.Should().HaveCount(1);
                context.HinhAnhSanPhams.First().LinkAnh.Should().Be("/images/test.jpg");
            }
        }

        [Fact]
        public async Task Get_ReturnsSingleImage()
        {
            // Arrange
            var image = new HinhAnhSanPham { Id = 1, LinkAnh = "/images/test.jpg", SanPhamId = 1 };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                context.HinhAnhSanPhams.Add(image);
                await context.SaveChangesAsync();
            }

            // Act
            HinhAnhSanPham result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new HinhAnhSanPhamRepository(context);
                result = await repository.Get(i => i.Id == 1);
            }

            // Assert
            result.Should().NotBeNull();
            result.LinkAnh.Should().Be("/images/test.jpg");
        }

        [Fact]
        public async Task GetAll_ReturnsAllImagesForProduct()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                context.SanPhams.Add(new SanPham { Id = 2, TenSanPham = "Another Product" });
                context.HinhAnhSanPhams.AddRange(new List<HinhAnhSanPham>
                {
                    new HinhAnhSanPham { Id = 1, LinkAnh = "/images/img1.jpg", SanPhamId = 1 },
                    new HinhAnhSanPham { Id = 2, LinkAnh = "/images/img2.jpg", SanPhamId = 1 },
                    new HinhAnhSanPham { Id = 3, LinkAnh = "/images/img3.jpg", SanPhamId = 2 }
                });
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<HinhAnhSanPham> result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new HinhAnhSanPhamRepository(context);
                result = await repository.GetAll(i => i.SanPhamId == 1);
            }

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Remove_RemovesImageFromDatabase()
        {
            // Arrange
            var image = new HinhAnhSanPham { Id = 1, LinkAnh = "/images/test.jpg", SanPhamId = 1 };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                context.HinhAnhSanPhams.Add(image);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new HinhAnhSanPhamRepository(context);
                var itemToRemove = await repository.Get(i => i.Id == 1);
                repository.Remove(itemToRemove);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.HinhAnhSanPhams.Should().BeEmpty();
            }
        }
    }
}
