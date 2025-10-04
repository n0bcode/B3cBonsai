
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
    public class VideoSanPhamRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public VideoSanPhamRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task Add_AddsVideoToDatabase()
        {
            // Arrange
            var video = new VideoSanPham { Id = 1, TenVideo = "Test Video", LinkVideo = "/videos/test.mp4", SanPhamId = 1 };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                await context.SaveChangesAsync();

                var repository = new VideoSanPhamRepository(context);

                // Act
                repository.Add(video);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.VideoSanPhams.Should().HaveCount(1);
                context.VideoSanPhams.First().TenVideo.Should().Be("Test Video");
            }
        }

        [Fact]
        public async Task Get_ReturnsSingleVideo()
        {
            // Arrange
            var video = new VideoSanPham { Id = 1, TenVideo = "Test Video", LinkVideo = "/videos/test.mp4", SanPhamId = 1 };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                context.VideoSanPhams.Add(video);
                await context.SaveChangesAsync();
            }

            // Act
            VideoSanPham result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new VideoSanPhamRepository(context);
                result = await repository.Get(v => v.Id == 1);
            }

            // Assert
            result.Should().NotBeNull();
            result.TenVideo.Should().Be("Test Video");
        }

        [Fact]
        public async Task GetAll_ReturnsAllVideosForProduct()
        {
            // Arrange
            await using (var context = new ApplicationDbContext(_options))
            {
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                context.SanPhams.Add(new SanPham { Id = 2, TenSanPham = "Another Product" });
                context.VideoSanPhams.AddRange(new List<VideoSanPham>
                {
                    new VideoSanPham { Id = 1, TenVideo = "Video 1", LinkVideo = "/v/1.mp4", SanPhamId = 1 },
                    new VideoSanPham { Id = 2, TenVideo = "Video 2", LinkVideo = "/v/2.mp4", SanPhamId = 1 },
                    new VideoSanPham { Id = 3, TenVideo = "Video 3", LinkVideo = "/v/3.mp4", SanPhamId = 2 }
                });
                await context.SaveChangesAsync();
            }

            // Act
            IEnumerable<VideoSanPham> result;
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new VideoSanPhamRepository(context);
                result = await repository.GetAll(v => v.SanPhamId == 1);
            }

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task Remove_RemovesVideoFromDatabase()
        {
            // Arrange
            var video = new VideoSanPham { Id = 1, TenVideo = "Test Video", LinkVideo = "/videos/test.mp4", SanPhamId = 1 };
            await using (var context = new ApplicationDbContext(_options))
            {
                context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Test Product" });
                context.VideoSanPhams.Add(video);
                await context.SaveChangesAsync();
            }

            // Act
            await using (var context = new ApplicationDbContext(_options))
            {
                var repository = new VideoSanPhamRepository(context);
                var itemToRemove = await repository.Get(v => v.Id == 1);
                repository.Remove(itemToRemove);
                await context.SaveChangesAsync();
            }

            // Assert
            await using (var context = new ApplicationDbContext(_options))
            {
                context.VideoSanPhams.Should().BeEmpty();
            }
        }
    }
}
