using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsai.Models;
using B3cBonsai.Utility;
using B3cBonsaiWeb.Areas.Employee.Controllers.Admin;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests
{
    public class DashboardControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new DashboardController(_unitOfWorkMock.Object);
        }

        private List<DonHang> GetTestOrders()
        {
            int currentMonth = DateTime.Today.Month;
            int currentYear = DateTime.Today.Year;

            return new List<DonHang>
            {
                // Approved orders in a previous month of the current year (if not January)
                new DonHang { Id = 1, TongTienDonHang = 100, TrangThaiDonHang = SD.StatusApproved, NgayNhanHang = new DateTimeOffset(currentYear, 1, 15, 0, 0, 0, TimeSpan.Zero) },
                new DonHang { Id = 2, TongTienDonHang = 150, TrangThaiDonHang = SD.StatusApproved, NgayNhanHang = new DateTimeOffset(currentYear, 1, 20, 0, 0, 0, TimeSpan.Zero) },
                
                // Approved order in the current month
                new DonHang { Id = 3, TongTienDonHang = 200, TrangThaiDonHang = SD.StatusApproved, NgayNhanHang = new DateTimeOffset(currentYear, currentMonth, 1, 0, 0, 0, TimeSpan.Zero) },
                
                // Approved order in a different year
                new DonHang { Id = 4, TongTienDonHang = 300, TrangThaiDonHang = SD.StatusApproved, NgayNhanHang = new DateTimeOffset(currentYear - 1, 1, 1, 0, 0, 0, TimeSpan.Zero) },
                
                // Pending order in the current month
                new DonHang { Id = 5, TongTienDonHang = 500, TrangThaiDonHang = SD.StatusPending, NgayNhanHang = new DateTimeOffset(currentYear, currentMonth, 5, 0, 0, 0, TimeSpan.Zero) },

                // Shipped order in the current month
                new DonHang { Id = 6, TongTienDonHang = 600, TrangThaiDonHang = SD.StatusShipped, NgayNhanHang = new DateTimeOffset(currentYear, currentMonth, 6, 0, 0, 0, TimeSpan.Zero) }
            };
        }

        private List<SanPham> GetTestSanPhams()
        {
            return new List<SanPham>
            {
                new SanPham { Id = 1, TenSanPham = "Product A", SoLuong = 10 },
                new SanPham { Id = 2, TenSanPham = "Product B", SoLuong = 20 },
                new SanPham { Id = 3, TenSanPham = "Product C", SoLuong = 30 }
            };
        }

        private List<ChiTietDonHang> GetTestChiTietDonHangs()
        {
            return new List<ChiTietDonHang>
            {
                new ChiTietDonHang { SanPhamId = 1, SoLuong = 5 },  // Product A sold 5
                new ChiTietDonHang { SanPhamId = 2, SoLuong = 10 }, // Product B sold 10
                new ChiTietDonHang { SanPhamId = 1, SoLuong = 3 },  // Product A sold 3 (total 8)
                new ChiTietDonHang { SanPhamId = 3, SoLuong = 15 }, // Product C sold 15
                new ChiTietDonHang { SanPhamId = 2, SoLuong = 2 },  // Product B sold 2 (total 12)
            };
        }

        [Fact]
        public async Task GetEarningData_WithMonthRange_ReturnsCorrectMonthlyEarningsForCurrentYear()
        {
            // Arrange
            var orders = GetTestOrders();
            _unitOfWorkMock.Setup(uow => uow.DonHang.GetAll(null, null)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetEarningData("month");

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value;

            var nameProperty = data.GetType().GetProperty("name").GetValue(data, null) as string;
            nameProperty.Should().Be("Đơn hàng");

            var columnDataProperty = data.GetType().GetProperty("data").GetValue(data, null) as List<int>;
            columnDataProperty.Should().HaveCount(12);
            columnDataProperty[0].Should().Be(250); // 100 + 150 for January
            columnDataProperty[DateTime.Today.Month - 1].Should().Be(200); // 200 for the current month

            var categoriesProperty = data.GetType().GetProperty("categories").GetValue(data, null) as List<string>;
            categoriesProperty.Should().HaveCount(12);
            categoriesProperty[0].Should().Be("Month 1");
        }

        [Fact]
        public async Task GetAllOrderData_ReturnsCorrectOrderStatusCounts()
        {
            // Arrange
            var orders = GetTestOrders(); // This provides 4 approved, 1 pending, 1 shipped
            _unitOfWorkMock.Setup(uow => uow.DonHang.GetAll(null, null)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetAllOrderData("anyTimeRange");

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value;

            var approved = (int)data.GetType().GetProperty("approved").GetValue(data, null);
            approved.Should().Be(4);

            var pending = (int)data.GetType().GetProperty("pending").GetValue(data, null);
            pending.Should().Be(1);

            var inAllProgress = (int)data.GetType().GetProperty("inAllProgress").GetValue(data, null);
            inAllProgress.Should().Be(1); // Total (6) - Approved (4) - Pending (1) = 1
        }

        [Fact]
        public void LaySanPhamBanChayNhat_ReturnsTopSellingProducts()
        {
            // Arrange
            var products = GetTestSanPhams();
            var orderDetails = GetTestChiTietDonHangs();
            _unitOfWorkMock.Setup(uow => uow.SanPham.GetAll(null, null)).ReturnsAsync(products);
            _unitOfWorkMock.Setup(uow => uow.ChiTietDonHang.GetAll(null, null)).ReturnsAsync(orderDetails);

            // Act
            var result = _controller.LaySanPhamBanChayNhat(2);

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value;
            var topProducts = data.GetType().GetProperty("data").GetValue(data, null) as List<SanPham>;

            topProducts.Should().NotBeNull();
            topProducts.Should().HaveCount(2);

            // 1. Product C should be first (sold 15)
            topProducts[0].Id.Should().Be(3);
            topProducts[0].SoLuong.Should().Be(15); // SoLuong should be the total sold quantity

            // 2. Product B should be second (sold 12)
            topProducts[1].Id.Should().Be(2);
            topProducts[1].SoLuong.Should().Be(12);
        }

        [Fact]
        public async Task GetOrderStatusData_WithMonthRange_ReturnsCorrectStatusCounts()
        {
            // Arrange
            var orders = GetTestOrders();
            _unitOfWorkMock.Setup(uow => uow.DonHang.GetAll(null, null)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrderStatusData("month");

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value;
            var statusCounts = data.GetType().GetProperty("data").GetValue(data, null) as List<int>;

            // Based on GetTestOrders(), for the current month:
            // StatusInProcess, StatusPending, StatusCancelled, StatusShipped, StatusApproved
            var expectedCounts = new List<int> { 0, 1, 0, 1, 1 };

            statusCounts.Should().BeEquivalentTo(expectedCounts);
        }

        [Fact]
        public async Task GetOrderOverViewData_WithMonthRange_ReturnsCorrectData()
        {
            // Arrange
            var orders = GetTestOrders();
            _unitOfWorkMock.Setup(uow => uow.DonHang.GetAll(null, null)).ReturnsAsync(orders);
            int currentMonthIndex = DateTime.Today.Month - 1;

            // Act
            var result = await _controller.GetOrderOverViewData("month");

            // Assert
            var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
            var data = jsonResult.Value;
            
            var categories = data.GetType().GetProperty("categories").GetValue(data, null) as List<string>;
            categories.Should().HaveCount(12);

            var seriesData = data.GetType().GetProperty("data").GetValue(data, null) as System.Collections.IEnumerable;
            seriesData.Should().NotBeNull();
            seriesData.Cast<object>().Should().HaveCount(5);

            // Check Approved status data for the current month
            var approvedSeries = seriesData.Cast<object>().First(s => (string)s.GetType().GetProperty("name").GetValue(s, null) == SD.StatusApproved);
            var approvedData = approvedSeries.GetType().GetProperty("data").GetValue(approvedSeries, null) as List<int>;
            approvedData[currentMonthIndex].Should().Be(1);

            // Check Pending status data for the current month
            var pendingSeries = seriesData.Cast<object>().First(s => (string)s.GetType().GetProperty("name").GetValue(s, null) == SD.StatusPending);
            var pendingData = pendingSeries.GetType().GetProperty("data").GetValue(pendingSeries, null) as List<int>;
            pendingData[currentMonthIndex].Should().Be(1);

            // Check Shipped status data for the current month
            var shippedSeries = seriesData.Cast<object>().First(s => (string)s.GetType().GetProperty("name").GetValue(s, null) == SD.StatusShipped);
            var shippedData = shippedSeries.GetType().GetProperty("data").GetValue(shippedSeries, null) as List<int>;
            shippedData[currentMonthIndex].Should().Be(1);
        }
    }
}
