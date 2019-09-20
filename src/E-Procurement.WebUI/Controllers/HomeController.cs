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
using E_Procurement.Repository.ApprovalRepo;

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
        private readonly IRfqApprovalRepository _rfqApprovalRepository;
        public HomeController(IRfqGenRepository rfqRepository, IRfqApprovalRepository rfqApprovalRepository, IVendorRepository vendorRepository, IPORepository poRepository, IReportRepository reportRepository)
        {
            _rfqGenRepository = rfqRepository;
            _poRepository = poRepository;
            _vendorRepository = vendorRepository;
            _reportRepository = reportRepository;
            _rfqApprovalRepository = rfqApprovalRepository;
        }

        //[Route("Identity/Account/Login")]
        //public IActionResult LoginRedirect(string ReturnUrl)
        //{
        //    return Redirect("/Account/Login?ReturnUrl=" + ReturnUrl);
        //}
        //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]

        public IActionResult Index()
        {
            var userId = _vendorRepository.GetUser().Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();

            var loggedInVendor = _vendorRepository.GetVendors().Where(u => u.UserId == userId).Select(u => u.Id).FirstOrDefault();


            if (User.IsInRole("Vendor User") && !loggedInVendor.Equals(null))
            {
                var categories = _vendorRepository.GetItemCategory().ToList();

                var vendor = _vendorRepository.GetVendors().Where(u => u.Id == loggedInVendor).FirstOrDefault();

                var mapping = _vendorRepository.GetMapping().Where(u => u.VendorID == vendor.Id).ToList();

                var SelectedCategories = categories.Where(a => mapping.Any(b => b.VendorCategoryId == a.Id)).Count().ToString();

                DashboardModel dashboard = new DashboardModel();
                dashboard.PO = _reportRepository.GetPoGen().Where(u => u.CreatedDate.Year == DateTime.Now.Year && u.VendorId == vendor.Id).Count().ToString();
                dashboard.RFQ = _reportRepository.GetRfqGen().Where(u => u.CreatedDate.Year == DateTime.Now.Year && u.VendorId == vendor.Id).Count().ToString();
                dashboard.Category = SelectedCategories;

                return View(dashboard);

            }
            else if (User.IsInRole("Approval"))
            {
                
                DashboardModel dashboard = new DashboardModel();
                var user = _reportRepository.GetUser().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                var pendingPO = _poRepository.GetPoGen2().Count().ToString();
                var pendingRFQ = _rfqApprovalRepository.GetRFQPendingApproval().Count().ToString();
                var approvedRFQ = _poRepository.GetApprovedRFQ().Count.ToString();
                var approvedPO = _poRepository.GetApprovedPO2().Count.ToString();
                if (Convert.ToInt32(pendingRFQ) != 0 || Convert.ToInt32(pendingPO) != 0)
                {
                    dashboard.pendingPO = pendingPO;
                    dashboard.pendingRFQ = pendingRFQ;
                }
                else
                {
                    dashboard.PO = approvedPO;
                    dashboard.RFQ = approvedRFQ;

                }
                dashboard.UserName = user.FullName;
                return View(dashboard);
                //return RedirectToAction("ApproverIndex", "Report");
            }
            else
            {
                var user = _reportRepository.GetUser().Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

                DashboardModel dashboard = new DashboardModel();
                dashboard.PO = _reportRepository.GetPoGen().Where(u => u.CreatedDate.Year == DateTime.Now.Year).Count().ToString();
                dashboard.RFQ = _reportRepository.GetRfqGen().Where(u => u.CreatedDate.Year == DateTime.Now.Year).Count().ToString();
                dashboard.RegVen = _reportRepository.GetVendors().Where(u => u.DateCreated.Year == DateTime.Now.Year).Count().ToString();
                dashboard.UserName = user.FullName;
                return View(dashboard);
            }

            //var selected1 = _reportRepository.GetPoGen().Count().ToString().Zip(_reportRepository.GetRfqGen().Count().ToString(), (a, b) => new { A = a, B = b });
            //var selected2 = _reportRepository.GetVendors().Count().ToString().Zip(selected1, (a, b) => new { A = a, B = b });

            //var listModel = selected2.Select(x => new DashboardModel
            //{
            //   RegVen = x.A.ToString(),
            //   PO = x.B.A.ToString(),
            //   RFQ = x.B.B.ToString()
            //});

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
