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
    public class BinhLuanRepositoryTests
    {
        private DbContextOptions<ApplicationDbContext> _options;

        public BinhLuanRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Add_AddsBinhLuanToDatabase()
        {
            // Arrange
            var binhLuan = new BinhLuan { Id = 1, NoiDungBinhLuan = "Test comment" };
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new BinhLuanRepository(context);

                // Act
                repository.Add(binhLuan);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(_options))
            {
                context.BinhLuans.Should().HaveCount(1);
                context.BinhLuans.First().NoiDungBinhLuan.Should().Be("Test comment");
            }
        }

        [Fact]
        public async Task Update_UpdatesBinhLuanInDatabase()
        {
            // Arrange
            var binhLuan = new BinhLuan { Id = 1, NoiDungBinhLuan = "Initial comment" };
            using (var context = new ApplicationDbContext(_options))
            {
                context.BinhLuans.Add(binhLuan);
                await context.SaveChangesAsync();
            }

            // Act
            binhLuan.NoiDungBinhLuan = "Updated comment";
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new BinhLuanRepository(context);
                repository.Update(binhLuan);
                await context.SaveChangesAsync();
            }

            // Assert
            using (var context = new ApplicationDbContext(_options))
            {
                context.BinhLuans.Should().HaveCount(1);
                context.BinhLuans.First().NoiDungBinhLuan.Should().Be("Updated comment");
            }
        }
        [Fact]
        public async Task Get_ReturnsSingleBinhLuan()
        {
            // Arrange
            var binhLuan = new BinhLuan { Id = 1, NoiDungBinhLuan = "Test comment" };
            using (var context = new ApplicationDbContext(_options))
            {
                context.BinhLuans.Add(binhLuan);
                await context.SaveChangesAsync();
            }

            // Act
            BinhLuan result;
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new BinhLuanRepository(context);
                result = await repository.Get(b => b.Id == 1);
            }

            // Assert
            result.Should().NotBeNull();
            result.NoiDungBinhLuan.Should().Be("Test comment");
        }

        [Fact]
        public async Task GetAll_ReturnsAllBinhLuans()
        {
            // Arrange
            var binhLuans = new List<BinhLuan>
            {
                new BinhLuan { Id = 1, NoiDungBinhLuan = "Comment 1" },
                new BinhLuan { Id = 2, NoiDungBinhLuan = "Comment 2" }
            };
            using (var context = new ApplicationDbContext(_options))
            {
                context.BinhLuans.AddRange(binhLuans);
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<BinhLuan> result;
            using (var context = new ApplicationDbContext(_options))
            {
                var repository = new BinhLuanRepository(context);
                result = await repository.GetAll(null);
            }

            // Assert
            result.Should().HaveCount(2);
        }
    }
}
