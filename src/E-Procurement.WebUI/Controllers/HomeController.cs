using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using E_Procurement.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using E_Procurement.WebUI.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;
using E_Procurement.Data;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Repository.PORepo;
using E_Procurement.Repository.RFQGenRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.Repository.ReportRepo;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    // [ValidateAntiForgeryToken]

    public class HomeController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IRfqGenRepository _rfqGenRepository;
        private readonly IPORepository _poRepository;
        public HomeController(IRfqGenRepository rfqRepository, IVendorRepository vendorRepository, IPORepository poRepository, IReportRepository reportRepository)
        {
            _rfqGenRepository = rfqRepository;
            _poRepository = poRepository;
            _vendorRepository = vendorRepository;
            _reportRepository = reportRepository;
        
        }

        //[Route("Identity/Account/Login")]
        //public IActionResult LoginRedirect(string ReturnUrl)
        //{
        //    return Redirect("/Account/Login?ReturnUrl=" + ReturnUrl);
        //}
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]

        public IActionResult Index()
        {
            DashboardModel dashboard = new DashboardModel();
            dashboard.PO = _reportRepository.GetPoGen().Where(u => u.CreatedDate.Year == DateTime.Now.Year).Count().ToString();
            dashboard.RegVen = _reportRepository.GetRfqGen().Where(u => u.CreatedDate.Year == DateTime.Now.Year).Count().ToString();
            dashboard.RFQ = _reportRepository.GetVendors().Where(u => u.DateCreated.Year == DateTime.Now.Year).Count().ToString();

            //var selected1 = _reportRepository.GetPoGen().Count().ToString().Zip(_reportRepository.GetRfqGen().Count().ToString(), (a, b) => new { A = a, B = b });
            //var selected2 = _reportRepository.GetVendors().Count().ToString().Zip(selected1, (a, b) => new { A = a, B = b });

            //var listModel = selected2.Select(x => new DashboardModel
            //{
            //   RegVen = x.A.ToString(),
            //   PO = x.B.A.ToString(),
            //   RFQ = x.B.B.ToString()
            //});

            return View(dashboard);
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
