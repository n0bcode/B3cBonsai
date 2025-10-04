
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
    public class ChiTietComboRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ChiTietComboRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        private async Task SeedDatabase(ApplicationDbContext context)
        {
            context.ComboSanPhams.Add(new ComboSanPham { Id = 1, TenCombo = "Test Combo" });
            context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task Add_AddsComboDetailToDatabase()
        {
            // Arrange
            var comboDetail = new ChiTietCombo { Id = 1, ComboId = 1, SanPhamId = 1, SoLuong = 1 };
            await using (var context = new ApplicationDbContext(_options))
            {
                await SeedDatabase(context);
                var repository = new ChiTietComboRepository(context);

                // Act
                repository.Add(comboDetail);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.ChiTietCombos.Should().HaveCount(1);
                context.ChiTietCombos.First().SoLuong.Should().Be(1);
            }
        }

        [Fact]
        public async Task Get_ReturnsSingleComboDetail()
        {
            // Arrange
            var comboDetail = new ChiTietCombo { Id = 1, ComboId = 1, SanPhamId = 1, SoLuong = 5 };
            await using (var context = new ApplicationDbContext(_options))
            {
                await SeedDatabase(context);
                context.ChiTietCombos.Add(comboDetail);
                await context.SaveChangesAsync();
            }

            // Act
            ChiTietCombo result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ChiTietComboRepository(context);
                result = await repository.Get(cd => cd.Id == 1);
            }

            // Assert
            result.Should().NotBeNull();
            result.SoLuong.Should().Be(5);
        }

        [Fact]
        public async Task GetAll_ReturnsAllComboDetailsForCombo()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                await SeedDatabase(context);
                context.ComboSanPhams.Add(new ComboSanPham { Id = 2, TenCombo = "Another Combo" });
                context.ChiTietCombos.AddRange(new List<ChiTietCombo>
                {
                    new ChiTietCombo { Id = 1, ComboId = 1, SanPhamId = 1, SoLuong = 1 },
                    new ChiTietCombo { Id = 2, ComboId = 1, SanPhamId = 1, SoLuong = 2 },
                    new ChiTietCombo { Id = 3, ComboId = 2, SanPhamId = 1, SoLuong = 3 }
                });
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<ChiTietCombo> result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ChiTietComboRepository(context);
                result = await repository.GetAll(cd => cd.ComboId == 1);
            }

            // Assert
            result.Should().HaveCount(2);
        }
    }
}
