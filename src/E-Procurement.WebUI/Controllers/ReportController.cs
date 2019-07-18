using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.PORepo;
using E_Procurement.Repository.ReportRepo;
using E_Procurement.Repository.RFQGenRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IRfqGenRepository _rfqGenRepository;
        private readonly IPORepository _poRepository;
        private readonly IMapper _mapper;

        public ReportController(IRfqGenRepository rfqRepository, IVendorRepository vendorRepository, IPORepository poRepository, IReportRepository reportRepository, IMapper mapper)
        {
            _rfqGenRepository = rfqRepository;
            _poRepository = poRepository;
            _vendorRepository = vendorRepository;
            _reportRepository = reportRepository;
            _mapper = mapper;
        }
        private void VendorPredefinedInfo(VendorModel Model)
        {
            //int CategoryId = Model.VendorCategoryId;

            var vendorCategory = _vendorRepository.GetItemCategory().ToList();
            if (Model.VendorCategoryId <= 0)
            {
                var Vendor = _reportRepository.GetVendors().ToList();
                List<ReportModel> vendorModel = new List<ReportModel>();
                Model.VendorCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                });
                var listModel = Vendor.Select(x => new ReportModel
                {
                    VendorName = x.VendorName,
                    VendorAddress = x.VendorAddress,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber

                });
                vendorModel.AddRange(listModel);


                Model.Report = vendorModel;
        }
            else
            {
                var Mapping = _reportRepository.GetMapping().ToList();
                var Vendor = _reportRepository.GetVendors().ToList();
                var VendorList = Vendor.Where(a => Mapping.Any(b => b.VendorCategoryId == Model.VendorCategoryId)).ToList();
                 List<ReportModel> vendorModel = new List<ReportModel>();
                Model.VendorCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                 });
                var listModel = VendorList.Select(x => new ReportModel
                {
                    VendorName = x.VendorName,
                    VendorAddress = x.VendorAddress,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber


                });
                vendorModel.AddRange(listModel);


                Model.Report = vendorModel;
            }
           
        }
        private void RfqPredefinedInfo(RfqGenModel Model)
        {
            //int CategoryId = Model.CategoryId;


            //var ItemCategory = _rfqGenRepository.GetItemCategory().ToList();

            //var Item = _rfqGenRepository.GetItem(CategoryId).ToList();

            List<ReportModel> rfqModel = new List<ReportModel>();

            //var Vendor = _rfqGenRepository.GetVendors(Model).ToList();
            if (Model.StartDate == DateTime.MinValue && Model.EndDate == DateTime.MinValue)
            {
                var RfqList = _reportRepository.GetRfqGen().OrderBy(u => u.EndDate).ToList();
                var listModel = RfqList.Select(x => new ReportModel
                {
                    RfqId = x.RFQId,
                    Reference = x.Reference,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive,
                    VendorName =x.VendorName

                });
                rfqModel.AddRange(listModel);


                Model.Report = rfqModel;
               
            }
            else
            {
                var RfqList = _reportRepository.GetRfqGen().Where(u => (Convert.ToDateTime(u.StartDate) >= Convert.ToDateTime(Model.StartDate)) && (Convert.ToDateTime(u.EndDate) <= Convert.ToDateTime(Model.EndDate))).OrderBy(u => u.EndDate).ToList();
                var listModel = RfqList.Select(x => new ReportModel
                {
                    RfqId = x.RFQId,
                    Reference = x.Reference,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive,
                    VendorName = x.VendorName


                });
                rfqModel.AddRange(listModel);


                Model.Report = rfqModel;

            }
        }
        private void POPredefinedInfo(RfqGenModel Model)
        {
            if (Model.StartDate == DateTime.MinValue && Model.EndDate == DateTime.MinValue)
            {
                List<ReportModel> poModel = new List<ReportModel>();
                var PO = _reportRepository.GetPoGen().ToList();
               
                //var RfqList = _rfqGenRepository.GetRfqGen().OrderBy(u => u.EndDate).ToList();
                var listModel = PO.Select(x => new ReportModel
                {
                    RfqId = x.RFQId,
                    PoId = x.PoId,
                    Reference = x.Reference,
                    Description = x.Description,
                    PONumber = x.PONumber,
                    TotalAmount = x.QuotedAmount,
                    ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                    VendorName = x.VendorName,
                    ContactName = x.ContactName,
                    Email = x.VendorEmail,
                    VendorStatus = x.VendorStatus,
                    PhoneNumber = x.PhoneNumber

                });
                poModel.AddRange(listModel);


                Model.Report = poModel;
                //Model.ItemCategoryList = ItemCategory.Select(x => new SelectListItem
                //{
                //    Value = x.Id.ToString(),
                //    Text = x.CategoryName
                //});
            }
            else
            {
                List<ReportModel> poModel = new List<ReportModel>();
                var POList = _reportRepository.GetPoGen().Where(u => (Convert.ToDateTime(u.CreatedDate) >= Convert.ToDateTime(Model.StartDate)) && (Convert.ToDateTime(u.CreatedDate) <= Convert.ToDateTime(Model.EndDate))).OrderBy(u => u.EndDate).ToList();
                var listModel = POList.Select(x => new ReportModel
                {
                    CreatedDate = x.CreatedDate,
                    RfqId = x.RFQId,
                    Reference = x.Reference,
                    Description = x.Description,
                    PONumber = x.PONumber,
                    TotalAmount = x.QuotedAmount,
                    ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                    VendorName = x.VendorName,
                    ContactName = x.ContactName,
                    Email = x.VendorEmail,
                    VendorStatus = x.VendorStatus,
                    PhoneNumber = x.PhoneNumber
                });
                poModel.AddRange(listModel);


                Model.Report = poModel;

            }

            //Model.VendorList = Vendor;

        }
       
        public ActionResult Vendor(VendorModel Model)
        {
            try
            {
                //VendorModel Model = new VendorModel();

                VendorPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }

        }
        public ActionResult RfqGen(RfqGenModel Model)
        {
            try
            {
                //RfqGenModel Model = new RfqGenModel();

                RfqPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View();
            }
        }
        public ActionResult PoGen(RfqGenModel Model)
        {

            try
            {
                //RfqGenModel Model = new RfqGenModel();

                POPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View();
            }
        }
        public ActionResult PoDetails (ReportModel Model)
        {
            var RFQ = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId).FirstOrDefault();
            var vendor = _reportRepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
            var PO = _reportRepository.GetPoGen().Where(u => u.RFQId == Model.RfqId).FirstOrDefault();

            RFQGenerationModel model2 = new RFQGenerationModel();
            model2.VendorId = vendor.Id;
            model2.ContactName = vendor.ContactName;
            model2.VendorName = vendor.VendorName;
            model2.VendorAddress = vendor.VendorAddress;
            model2.VendorEmail = vendor.Email;
            model2.CreatedDate = PO.CreatedDate;
            

            List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
            var POList = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId).ToList();
            model2.TotalAmount = POList.Sum(x => x.QuotedAmount);
            var listModel = POList.Select(x => new RFQDetailsModel
            {
                RFQId = x.RFQId,
                VendorId = x.VendorId,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                QuotedQuantity = x.QuotedQuantity,
                AgreedQuantity = x.AgreedQuantity,
                QuotedAmount = x.QuotedAmount,
                AgreedAmount = x.AgreedAmount
                
            });
            poModel.AddRange(listModel);


            model2.RFQDetails = poModel;
            
            return View(model2);
        }
        public ActionResult RfqDetails(ReportModel Model)
        {
            //var RFQ = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId).FirstOrDefault();
            var vendor = _reportRepository.GetVendors().Where(a => _reportRepository.GetRFQDetails().Any(b => b.VendorId == a.Id && b.RFQId == Model.RfqId)).FirstOrDefault();
            //var vendor = _reportRepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
            var rfq = _reportRepository.GetRfqGen().Where(u => u.RFQId == Model.RfqId).FirstOrDefault();

            RFQGenerationModel model2 = new RFQGenerationModel();
            model2.VendorId = vendor.Id;
            model2.ContactName = vendor.ContactName;
            model2.VendorName = vendor.VendorName;
            model2.VendorAddress = vendor.VendorAddress;
            model2.VendorEmail = vendor.Email;
            model2.CreatedDate = rfq.CreatedDate;


            List<RFQDetailsModel> rfqModel = new List<RFQDetailsModel>();
            var RFQList = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId).ToList();
            model2.TotalAmount = RFQList.Sum(x => x.QuotedAmount);
            var listModel = RFQList.Select(x => new RFQDetailsModel
            {
                RFQId = x.RFQId,
                VendorId = x.VendorId,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                Description = x.ItemDescription,
                QuotedQuantity = x.QuotedQuantity,
                AgreedQuantity = x.AgreedQuantity,
                QuotedAmount = x.QuotedAmount,
                AgreedAmount = x.AgreedAmount

            });
            rfqModel.AddRange(listModel);


            model2.RFQDetails = rfqModel;

            return View(model2);
        }
        private void VendorEvaluationPredefinedInfo(RfqGenModel Model)
        {
            //int CategoryId = Model.VendorCategoryId;

            var vendorCategory = _vendorRepository.GetItemCategory().ToList();
            if (Model.CategoryId <= 0)
            {
                var Vendor = _reportRepository.GetVendors().ToList();
                List<ReportModel> vendorModel = new List<ReportModel>();
                Model.ItemCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                });

                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem(){ Text = "Excellent", Value = "Excellent"});
                list.Add(new SelectListItem() { Text = "Very Good", Value = "Very Good" });
                list.Add(new SelectListItem() { Text = "Good", Value = "Good" });
                list.Add(new SelectListItem() { Text = "Fair", Value = "Fair" });
                list.Add(new SelectListItem() { Text = "Poor", Value = "Poor" });
                Model.CriteriaList = list.Select(x => new SelectListItem
                {
                    Value = x.Value.ToString(),
                    Text = x.Text
                });

                Model.VendorList = Vendor.Select(x => new SelectListItem
                {

                    Value = x.Id.ToString(),
                    Text = x.VendorName
                });
                
            }
            else
            {
                var Vendor = _reportRepository.GetVendorsByCategory(Model).ToList();
                
                List<ReportModel> vendorModel = new List<ReportModel>();
                Model.ItemCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                });
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Text = "Excellent", Value = "Excellent" });
                list.Add(new SelectListItem() { Text = "Very Good", Value = "Very Good" });
                list.Add(new SelectListItem() { Text = "Good", Value = "Good" });
                list.Add(new SelectListItem() { Text = "Fair", Value = "Fair" });
                list.Add(new SelectListItem() { Text = "Poor", Value = "Poor" });
                Model.CriteriaList = list.Select(x => new SelectListItem
                {
                    Value = x.Value.ToString(),
                    Text = x.Text
                });

                Model.VendorList = Vendor.Select(x => new SelectListItem
                {

                    Value = x.Id.ToString(),
                    Text = x.VendorName
                });
            }

        }

        [HttpGet]
        public ActionResult VendorEvaluation()
        {

            try
            {
                RfqGenModel Model = new RfqGenModel();
                //Model.CategoryId = CategoryId;
                VendorEvaluationPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorEvaluation(RfqGenModel Model)
        {
            try
            {
                string message;
                string UserId = User.Identity.Name;

                //ModelState.AddModelError("BestPrice", "Region is mandatory");
                
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = UserId;

                    var status = _reportRepository.VendorEvaluation(Model, out message);

                    ViewBag.Message = TempData["MESSAGE"] as AlertMessage;

                    if (status == true)
                    {

                        ViewBag.Message = TempData["MESSAGE"] as AlertMessage;

                    }

                    else
                    {
                        ViewBag.Message = TempData["MESSAGE"] as AlertMessage;
                        return View(Model);
                    }

                    return RedirectToAction("VendorEvaluation", "Report");
                }
                else
                {
                    VendorEvaluationPredefinedInfo(Model);

                    ViewBag.StatusCode = 2;

                    return View(Model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        //[HttpGet]
        ////[ValidateAntiForgeryToken]
        //public ActionResult VendorEvaluation(string selectedValue)
        //{
        //    return null;
        //}

    }
}