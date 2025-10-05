
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
    public class ChiTietDonHangRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ChiTietDonHangRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        private async Task SeedDatabase(ApplicationDbContext context)
        {
            context.NguoiDungUngDungs.Add(new NguoiDungUngDung { Id = "user1", UserName = "testuser" });
            context.DonHangs.Add(new DonHang { Id = 1, NguoiDungId = "user1", TrangThaiDonHang = "Pending" });
            context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Add_AddsOrderDetailToDatabase()
        {
            // Arrange
            var orderDetail = new ChiTietDonHang { Id = 1, DonHangId = 1, SanPhamId = 1, SoLuong = 1, Gia = 100, LoaiDoiTuong = "SanPham" };
            await using (var context = new ApplicationDbContext(_options))
            {
                await SeedDatabase(context);
                var repository = new ChiTietDonHangRepository(context);

                // Act
                repository.Add(orderDetail);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.ChiTietDonHangs.Should().HaveCount(1);
                context.ChiTietDonHangs.First().SoLuong.Should().Be(1);
            }
        }

        [Fact]
        public async Task Get_ReturnsSingleOrderDetail()
        {
            // Arrange
            var orderDetail = new ChiTietDonHang { Id = 1, DonHangId = 1, SanPhamId = 1, SoLuong = 5, Gia = 100, LoaiDoiTuong = "SanPham" };
            await using (var context = new ApplicationDbContext(_options))
            {
                await SeedDatabase(context);
                context.ChiTietDonHangs.Add(orderDetail);
                await context.SaveChangesAsync();
            }

            // Act
            ChiTietDonHang result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ChiTietDonHangRepository(context);
                result = await repository.Get(od => od.Id == 1);
            }

            // Assert
            result.Should().NotBeNull();
            result.SoLuong.Should().Be(5);
        }

        [Fact]
        public async Task GetAll_ReturnsAllOrderDetailsForOrder()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                await SeedDatabase(context);
                context.DonHangs.Add(new DonHang { Id = 2, NguoiDungId = "user1", TrangThaiDonHang = "Shipped" });
                context.ChiTietDonHangs.AddRange(new List<ChiTietDonHang>
                {
                    new ChiTietDonHang { Id = 1, DonHangId = 1, SanPhamId = 1, SoLuong = 1, Gia = 100 },
                    new ChiTietDonHang { Id = 2, DonHangId = 1, SanPhamId = 1, SoLuong = 2, Gia = 200 },
                    new ChiTietDonHang { Id = 3, DonHangId = 2, SanPhamId = 1, SoLuong = 3, Gia = 300 }
                });
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<ChiTietDonHang> result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ChiTietDonHangRepository(context);
                result = await repository.GetAll(od => od.DonHangId == 1);
            }

            // Assert
            result.Should().HaveCount(2);
        }
    }
}
