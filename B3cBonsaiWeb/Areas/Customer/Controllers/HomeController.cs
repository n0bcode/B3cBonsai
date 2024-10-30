using System.Diagnostics;
using B3cBonsaiWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Controllers
{
    //Cho hiển thị các view giao diện đơn giản
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        #region//Other View
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult FAQ() { 
            return View();
        }

        public IActionResult Terms() {
            return View();
        }
        public IActionResult ReturnPolicy()
        {
            return View();
        }
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult ComingSoon()
        {
            return View();
        }
        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
