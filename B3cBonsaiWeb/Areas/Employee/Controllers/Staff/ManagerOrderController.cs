﻿using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    [Area("Employee")]
    public class ManagerOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OrderSummary()
        {
            return View();
        }
    }
}
