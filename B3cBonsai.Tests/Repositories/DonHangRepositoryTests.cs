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
    public class DonHangRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DonHangRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var context = new ApplicationDbContext(_options);
            await context.Database.EnsureCreatedAsync();

            if (!await context.DonHangs.AnyAsync())
            {
                var sanPham = new SanPham { Id = 1, TenSanPham = "Test Product", SoLuong = 10, Gia = 100, DanhMucId = 1, TrangThai = true };
                context.SanPhams.Add(sanPham);

                var donHang = new DonHang
                {
                    Id = 1,
                    NguoiDungId = "user1",
                    TrangThaiDonHang = SD.StatusInProcess,
                    ChiTietDonHangs = new List<ChiTietDonHang>
                    {
                        new ChiTietDonHang { Id = 1, DonHangId = 1, SanPhamId = 1, SoLuong = 2, Gia = 100, LoaiDoiTuong = SD.ObjectDetailOrder_SanPham }
                    }
                };
                context.DonHangs.Add(donHang);

                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task Update_WhenOrderStatusIsCancelled_ShouldReturnProductQuantity()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new DonHangRepository(context);
            var donHang = await context.DonHangs.Include(dh => dh.ChiTietDonHangs).FirstAsync(dh => dh.Id == 1);
            var sanPham = await context.SanPhams.FirstAsync(sp => sp.Id == 1);
            var initialProductQuantity = sanPham.SoLuong; // 10
            var orderQuantity = donHang.ChiTietDonHangs.First().SoLuong; // 2

            // Act
            donHang.TrangThaiDonHang = SD.StatusCancelled;
            repository.Update(donHang);
            await context.SaveChangesAsync();

            // Assert
            var updatedSanPham = await context.SanPhams.FirstAsync(sp => sp.Id == 1);
            updatedSanPham.SoLuong.Should().Be(initialProductQuantity + orderQuantity);
        }

        [Fact]
        public async Task Update_WhenOrderStatusIsNotCancelled_ShouldNotReturnProductQuantity()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new DonHangRepository(context);
            var donHang = await context.DonHangs.FirstAsync(dh => dh.Id == 1);
            var sanPham = await context.SanPhams.FirstAsync(sp => sp.Id == 1);
            var initialProductQuantity = sanPham.SoLuong;

            // Act
            donHang.TrangThaiDonHang = SD.StatusShipped;
            repository.Update(donHang);
            await context.SaveChangesAsync();

            // Assert
            var updatedSanPham = await context.SanPhams.FirstAsync(sp => sp.Id == 1);
            updatedSanPham.SoLuong.Should().Be(initialProductQuantity);
        }

        [Fact]
        public async Task Add_AddsDonHangToDatabase()
        {
            // Arrange
            var donHang = new DonHang { Id = 2, NguoiDungId = "user2", TrangThaiDonHang = SD.StatusPending };
            await using var context = await GetDatabaseContext();
            var repository = new DonHangRepository(context);

            // Act
            repository.Add(donHang);
            await context.SaveChangesAsync();

            // Assert
            (await context.DonHangs.CountAsync()).Should().Be(2);
            (await context.DonHangs.FindAsync(2)).Should().NotBeNull();
        }
    }
}
