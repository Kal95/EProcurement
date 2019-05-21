using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.Web.Controller
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}