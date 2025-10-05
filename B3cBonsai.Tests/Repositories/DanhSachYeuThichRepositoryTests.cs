
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
    public class DanhSachYeuThichRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public DanhSachYeuThichRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Add_AddsWishlistItemToDatabase()
        {
            // Arrange
            var wishlistItem = new DanhSachYeuThich { Id = 1, SanPhamId = 1, NguoiDungId = "user1", LoaiDoiTuong = "SanPham" };
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new DanhSachYeuThichRepository(context);

                // Act
                repository.Add(wishlistItem);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.DanhSachYeuThichs.Should().HaveCount(1);
                context.DanhSachYeuThichs.First().NguoiDungId.Should().Be("user1");
            }
        }

        [Fact]
        public async Task Get_ReturnsSingleWishlistItem()
        {
            // Arrange
            var wishlistItem = new DanhSachYeuThich { Id = 1, SanPhamId = 1, NguoiDungId = "user1", LoaiDoiTuong = "SanPham" };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.DanhSachYeuThichs.Add(wishlistItem);
                await context.SaveChangesAsync();
            }

            // Act
            DanhSachYeuThich result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new DanhSachYeuThichRepository(context);
                result = await repository.Get(w => w.Id == 1);
            }

            // Assert
            result.Should().NotBeNull();
            result.NguoiDungId.Should().Be("user1");
        }

        [Fact]
        public async Task GetAll_ReturnsAllWishlistItemsForUser()
        {
            // Arrange
            var wishlistItems = new List<DanhSachYeuThich>
            {
                new DanhSachYeuThich { Id = 1, SanPhamId = 1, NguoiDungId = "user1", LoaiDoiTuong = "SanPham" },
                new DanhSachYeuThich { Id = 2, SanPhamId = 2, NguoiDungId = "user1", LoaiDoiTuong = "SanPham" },
                new DanhSachYeuThich { Id = 3, SanPhamId = 3, NguoiDungId = "user2", LoaiDoiTuong = "SanPham" }
            };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.DanhSachYeuThichs.AddRange(wishlistItems);
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<DanhSachYeuThich> result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new DanhSachYeuThichRepository(context);
                result = await repository.GetAll(w => w.NguoiDungId == "user1");
            }

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Remove_RemovesWishlistItemFromDatabase()
        {
            // Arrange
            var wishlistItem = new DanhSachYeuThich { Id = 1, SanPhamId = 1, NguoiDungId = "user1", LoaiDoiTuong = "SanPham" };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.DanhSachYeuThichs.Add(wishlistItem);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new DanhSachYeuThichRepository(context);
                var itemToRemove = await repository.Get(w => w.Id == 1);
                repository.Remove(itemToRemove);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.DanhSachYeuThichs.Should().BeEmpty();
            }
        }
    }
}
