﻿using Microsoft.AspNetCore.Mvc;

namespace B3cBonsaiWeb.Areas.Employee.Controllers.Staff
{
    public class ManagerProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert()
        {
            return View();
        }
        public IActionResult DetailWithDelete()
        {
            return View();
        }
    }
}
