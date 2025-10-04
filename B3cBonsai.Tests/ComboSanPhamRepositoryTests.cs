
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
    public class ComboSanPhamRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ComboSanPhamRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Add_AddsComboToDatabase()
        {
            // Arrange
            var combo = new ComboSanPham { Id = 1, TenCombo = "Test Combo", GiamGia = 10, SoLuong = 2, TongGia = 100 };
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ComboSanPhamRepository(context);

                // Act
                repository.Add(combo);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.ComboSanPhams.Should().HaveCount(1);
                context.ComboSanPhams.First().TenCombo.Should().Be("Test Combo");
            }
        }

        [Fact]
        public async Task Get_ReturnsSingleCombo()
        {
            // Arrange
            var combo = new ComboSanPham { Id = 1, TenCombo = "Test Combo", GiamGia = 10, SoLuong = 2, TongGia = 100 };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.ComboSanPhams.Add(combo);
                await context.SaveChangesAsync();
            }

            // Act
            ComboSanPham result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ComboSanPhamRepository(context);
                result = await repository.Get(c => c.Id == 1);
            }

            // Assert
            result.Should().NotBeNull();
            result.TenCombo.Should().Be("Test Combo");
        }

        [Fact]
        public async Task GetAll_ReturnsAllCombos()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                context.ComboSanPhams.AddRange(new List<ComboSanPham>
                {
                    new ComboSanPham { Id = 1, TenCombo = "Combo 1" },
                    new ComboSanPham { Id = 2, TenCombo = "Combo 2" }
                });
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<ComboSanPham> result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ComboSanPhamRepository(context);
                result = await repository.GetAll(null);
            }

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Remove_RemovesComboFromDatabase()
        {
            // Arrange
            var combo = new ComboSanPham { Id = 1, TenCombo = "Test Combo" };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.ComboSanPhams.Add(combo);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ComboSanPhamRepository(context);
                var itemToRemove = await repository.Get(c => c.Id == 1);
                repository.Remove(itemToRemove);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.ComboSanPhams.Should().BeEmpty();
            }
        }

        [Fact]
        public async Task Update_UpdatesComboInDatabase()
        {
            // Arrange
            var combo = new ComboSanPham { Id = 1, TenCombo = "Initial Combo" };
            using (var context = new ApplicationDbContext(_options))
            {
                context.ComboSanPhams.Add(combo);
                await context.SaveChangesAsync();
            }

            // Act
            combo.TenCombo = "Updated Combo";
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new ComboSanPhamRepository(context);
                repository.Update(combo);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(_options))
            {
                context.ComboSanPhams.Should().HaveCount(1);
                context.ComboSanPhams.First().TenCombo.Should().Be("Updated Combo");
            }
        }
    }
}
