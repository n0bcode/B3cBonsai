
using B3cBonsai.DataAccess.Repository.IRepository;
using B3cBonsaiWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace B3cBonsai.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockUnitOfWork.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task Index_ReturnsAViewResult()
        {
            // Arrange

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}
