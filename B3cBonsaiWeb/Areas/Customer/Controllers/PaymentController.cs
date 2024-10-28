using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Customer.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Policy() { 
            return View();
        }
        public IActionResult OrderComplete() {
            return View();
        }
        public IActionResult TrackOrder()
        {
            return View();
        }
    }
}
