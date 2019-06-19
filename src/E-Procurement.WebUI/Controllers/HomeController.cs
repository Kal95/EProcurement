using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_Procurement.WebUI.Models;
using Microsoft.AspNetCore.Authorization;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
   // [ValidateAntiForgeryToken]
    public class HomeController : Controller
    {
        //[Route("Identity/Account/Login")]
        //public IActionResult LoginRedirect(string ReturnUrl)
        //{
        //    return Redirect("/Account/Login?ReturnUrl=" + ReturnUrl);
        //}
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
