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
    public class SanPhamRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public SanPhamRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var context = new ApplicationDbContext(_options);
            await context.Database.EnsureCreatedAsync();
            if (!await context.SanPhams.AnyAsync())
            {
                context.SanPhams.AddRange(
                    new SanPham { Id = 1, TenSanPham = "Cay Thong", Gia = 100, SoLuong = 10, TrangThai = true, DanhMucId = 1 },
                    new SanPham { Id = 2, TenSanPham = "Cay Tung", Gia = 200, SoLuong = 5, TrangThai = true, DanhMucId = 1 }
                );
                await context.SaveChangesAsync();
            }
            return context;
        }

        [Fact]
        public async Task Add_AddsSanPhamToDatabase()
        {
            // Arrange
            var sanPham = new SanPham { Id = 3, TenSanPham = "Cay Mai", Gia = 300, SoLuong = 20, TrangThai = true, DanhMucId = 2 };
            await using var context = await GetDatabaseContext();
            var repository = new SanPhamRepository(context);

            // Act
            repository.Add(sanPham);
            await context.SaveChangesAsync();

            // Assert
            var result = await context.SanPhams.FindAsync(3);
            result.Should().NotBeNull();
            result.TenSanPham.Should().Be("Cay Mai");
            (await context.SanPhams.CountAsync()).Should().Be(3);
        }

        [Fact]
        public async Task Update_UpdatesSanPhamInDatabase()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new SanPhamRepository(context);
            var sanPhamToUpdate = await context.SanPhams.FindAsync(1);
            sanPhamToUpdate.TenSanPham = "Cay Thong Nhat Ban";

            // Act
            repository.Update(sanPhamToUpdate);
            await context.SaveChangesAsync();

            // Assert
            var result = await context.SanPhams.FindAsync(1);
            result.Should().NotBeNull();
            result.TenSanPham.Should().Be("Cay Thong Nhat Ban");
        }

        [Fact]
        public async Task Get_ReturnsSingleSanPham()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new SanPhamRepository(context);

            // Act
            var result = await repository.Get(s => s.Id == 1);

            // Assert
            result.Should().NotBeNull();
            result.TenSanPham.Should().Be("Cay Thong");
        }

        [Fact]
        public async Task GetAll_ReturnsAllSanPhams()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new SanPhamRepository(context);

            // Act
            var result = await repository.GetAll(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Remove_RemovesSanPhamFromDatabase()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new SanPhamRepository(context);
            var sanPhamToRemove = await context.SanPhams.FindAsync(1);

            // Act
            repository.Remove(sanPhamToRemove);
            await context.SaveChangesAsync();

            // Assert
            (await context.SanPhams.CountAsync()).Should().Be(1);
            (await context.SanPhams.FindAsync(1)).Should().BeNull();
        }
    }
}
