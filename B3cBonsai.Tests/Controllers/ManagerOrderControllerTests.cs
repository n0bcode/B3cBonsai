using System.Threading;
using System.Threading.Tasks;
using B3cBonsai.DataAccess.Data;
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsaiWeb.Areas.Employee.Controllers.Staff;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System;
using B3cBonsai.Utility.Services;

namespace B3cBonsai.Tests
{
    public class ManagerOrderControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly Mock<TelegramService> _telegramServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public ManagerOrderControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _telegramServiceMock = new Mock<TelegramService>("123456789:AABBCCDDEEFFaabbccddeeff-123456789");

            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        private ApplicationDbContext GetDatabaseContext()
        {
            var context = new ApplicationDbContext(_options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetAll_ReturnsAllOrders_WhenNoStatusFilter()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            
            var orders = new List<DonHang>
            {
                new DonHang { TenNguoiNhan = "User 1", TrangThaiDonHang = SD.StatusPending, NguoiDungUngDung = new NguoiDungUngDung{HoTen = "Test User" } },
                new DonHang { TenNguoiNhan = "User 2", TrangThaiDonHang = SD.StatusInProcess, NguoiDungUngDung = new NguoiDungUngDung{HoTen = "Test User" } },
                new DonHang { TenNguoiNhan = "User 3", TrangThaiDonHang = SD.StatusShipped, NguoiDungUngDung = new NguoiDungUngDung{HoTen = "Test User" } }
            };
            
            context.DonHangs.AddRange(orders);
            await context.SaveChangesAsync();

            var controller = new ManagerOrderController(context, _telegramServiceMock.Object, _unitOfWorkMock.Object);

            // Act
            var result = await controller.GetAll(null);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var dataProperty = jsonResult.Value.GetType().GetProperty("data");
            dataProperty.Should().NotBeNull();
            var data = dataProperty.GetValue(jsonResult.Value, null).Should().BeAssignableTo<IEnumerable<DonHang>>().Subject;
            
            data.Should().HaveCount(orders.Count);
            data.Select(d => d.TenNguoiNhan).Should().BeEquivalentTo(orders.Select(o => o.TenNguoiNhan));
        }
    }
}