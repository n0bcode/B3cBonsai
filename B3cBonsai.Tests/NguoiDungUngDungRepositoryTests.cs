using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository;
using B3cBonsai.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Moq;


namespace B3cBonsai.Tests
{
    public class NguoiDungUngDungRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly Mock<B3cBonsai.Utility.Services.IImageStorageService> _imageStorageServiceMock;

        public NguoiDungUngDungRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            _imageStorageServiceMock = new Mock<B3cBonsai.Utility.Services.IImageStorageService>();
        }

        private async Task<ApplicationDbContext> GetDatabaseContext()
        {
            var context = new ApplicationDbContext(_options);
            await context.Database.EnsureCreatedAsync();
            if (!await context.NguoiDungUngDungs.AnyAsync())
            {
                context.NguoiDungUngDungs.AddRange(
                    new NguoiDungUngDung { Id = "user1", UserName = "user1@test.com", HoTen = "User One" },
                    new NguoiDungUngDung { Id = "user2", UserName = "user2@test.com", HoTen = "User Two" }
                );
                await context.SaveChangesAsync();
            }
            return context;
        }

        [Fact]
        public async Task Add_AddsNguoiDungToDatabase()
        {
            // Arrange
            var newUser = new NguoiDungUngDung { Id = "user3", UserName = "user3@test.com", HoTen = "User Three" };
            await using var context = await GetDatabaseContext();
            var repository = new NguoiDungUngDungRepository(context, _imageStorageServiceMock.Object);

            // Act
            repository.Add(newUser);
            await context.SaveChangesAsync();

            // Assert
            (await context.NguoiDungUngDungs.CountAsync()).Should().Be(3);
            var result = await context.NguoiDungUngDungs.FindAsync("user3");
            result.Should().NotBeNull();
            result.HoTen.Should().Be("User Three");
        }

        [Fact]
        public async Task Get_ReturnsSingleNguoiDung()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new NguoiDungUngDungRepository(context, _imageStorageServiceMock.Object);

            // Act
            var result = await repository.Get(u => u.Id == "user1");

            // Assert
            result.Should().NotBeNull();
            result.HoTen.Should().Be("User One");
        }

        [Fact]
        public async Task GetAll_ReturnsAllNguoiDungs()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new NguoiDungUngDungRepository(context, _imageStorageServiceMock.Object);

            // Act
            var result = await repository.GetAll(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Remove_RemovesNguoiDungFromDatabase()
        {
            // Arrange
            await using var context = await GetDatabaseContext();
            var repository = new NguoiDungUngDungRepository(context, _imageStorageServiceMock.Object);
            var userToRemove = await context.NguoiDungUngDungs.FindAsync("user1");

            // Act
            repository.Remove(userToRemove);
            await context.SaveChangesAsync();

            // Assert
            (await context.NguoiDungUngDungs.CountAsync()).Should().Be(1);
            (await context.NguoiDungUngDungs.FindAsync("user1")).Should().BeNull();
        }
    }
}