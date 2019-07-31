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
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class ReportController : BaseController
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
        private void VendorPredefinedInfo(RfqGenModel Model)
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
                var Vendor = _reportRepository.GetVendorsByCategory(Model).ToList();
                List<ReportModel> vendorModel = new List<ReportModel>();
                Model.ItemCategoryList = vendorCategory.Select(x => new SelectListItem
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
                    VendorId = x.VendorId,
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
                    VendorId = x.VendorId,
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
                    VendorId = x.VendorId,
                    CreatedDate = x.CreatedDate,
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
                    VendorId = x.VendorId,
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
        private void VendorEvaluationReportPredefinedInfo(RfqGenModel Model)
        {
            //int CategoryId = Model.VendorCategoryId;
            var EvaluationScore = _reportRepository.GetVendorEvaluation().ToList();
            
                List<SelectListItem> list = new List<SelectListItem>();
                list.Add(new SelectListItem() { Text = "Excellent", Value = "5" });
                list.Add(new SelectListItem() { Text = "Very Good", Value = "4" });
                list.Add(new SelectListItem() { Text = "Good", Value = "3" });
                list.Add(new SelectListItem() { Text = "Fair", Value = "2" });
                list.Add(new SelectListItem() { Text = "Poor", Value = "1" });
           
           
            var ScoreList = (from d in EvaluationScore
                              join BP in list on d.BestPrice equals BP.Text
                              join DT in list on d.DeliveryTimeFrame equals DT.Text
                              join CF in list on d.CreditFacility equals CF.Text
                              join CC in list on d.CustomerCare equals CC.Text
                              join PA in list on d.ProductAvailability equals PA.Text
                              join PQ in list on d.ProductQuality equals PQ.Text
                              join WS in list on d.WarrantySupport equals WS.Text
                              join O in list on d.Others equals O.Text
                            
                              select new ReportModel()
            {
                VendorId = Convert.ToInt32(d.VendorId.ToString()),
                VendorName = d.VendorName,
                BestPrice = " (" + BP.Value+"/5)",
                DeliveryTimeFrame = " (" + DT.Value+"/5)",
                CreditFacility = " (" + CF.Value+"/5)",
                CustomerCare = " (" + CC.Value+"/5)",
                ProductAvailability = " (" + PA.Value+"/5)",
                ProductQuality = " (" + PQ.Value+"/5)",
                WarrantySupport = " (" + WS.Value+"/5)",
                Others = " (" + O.Value+"/5)",
                CreatedBy = d.CreatedBy,
                CreatedDate = d.DateCreated,
                Score = ((Convert.ToInt32(BP.Value) + Convert.ToInt32(DT.Value) + Convert.ToInt32(CF.Value) + Convert.ToInt32(CC.Value) + Convert.ToInt32(PA.Value) + Convert.ToInt32(PQ.Value) + Convert.ToInt32(WS.Value))*100/35).ToString() +"%"

        }).GroupBy(v => new { v.VendorId, v.Score }).Select(s => s.FirstOrDefault());

            
            var vendorCategory = _vendorRepository.GetItemCategory().ToList();
           
            if (Model.CategoryId <= 0)
            {
                var VendorEvaluation = _reportRepository.GetVendorEvaluation().ToList();

                List<ReportModel> vendorModel = new List<ReportModel>();
                Model.ItemCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                });
                var listModel = VendorEvaluation.Select(x => new ReportModel
                {
                    VendorId = Convert.ToInt32(x.VendorId.ToString()),
                    VendorName = x.VendorName,
                    BestPrice = x.BestPrice + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.BestPrice).FirstOrDefault(),
                    DeliveryTimeFrame = x.DeliveryTimeFrame + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.DeliveryTimeFrame).FirstOrDefault(),
                    CreditFacility = x.CreditFacility + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.CreditFacility).FirstOrDefault(),
                    CustomerCare = x.CustomerCare + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.CustomerCare).FirstOrDefault(),
                    ProductAvailability = x.ProductAvailability + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.ProductAvailability).FirstOrDefault(),
                    ProductQuality = x.ProductQuality + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.ProductQuality).FirstOrDefault(),
                    WarrantySupport = x.WarrantySupport + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.WarrantySupport).FirstOrDefault(),
                    Others = x.Others + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.Others).FirstOrDefault(),
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.DateCreated,
                    Score = ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.Score).FirstOrDefault(),
                });
                vendorModel.AddRange(listModel);


                Model.Report = vendorModel;
            }
            else
            {
                var VendorEvaluation = _reportRepository.GetVendorEvaluationByCategory(Model).ToList();
                List<ReportModel> vendorModel = new List<ReportModel>();
                Model.ItemCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                });
                var listModel = VendorEvaluation.Select(x => new ReportModel
                {
                    VendorId = Convert.ToInt32(x.VendorId.ToString()),
                    VendorName = x.VendorName,
                    BestPrice = x.BestPrice + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.BestPrice).FirstOrDefault(),
                    DeliveryTimeFrame = x.DeliveryTimeFrame + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.DeliveryTimeFrame).FirstOrDefault(),
                    CreditFacility = x.CreditFacility + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.CreditFacility).FirstOrDefault(),
                    CustomerCare = x.CustomerCare + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.CustomerCare).FirstOrDefault(),
                    ProductAvailability = x.ProductAvailability + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.ProductAvailability).FirstOrDefault(),
                    ProductQuality = x.ProductQuality + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.ProductQuality).FirstOrDefault(),
                    WarrantySupport = x.WarrantySupport + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.WarrantySupport).FirstOrDefault(),
                    Others = x.Others + ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.Others).FirstOrDefault(),
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.DateCreated,
                    Score = ScoreList.Where(u => u.VendorId == Convert.ToInt32(x.VendorId)).Select(u => u.Score).FirstOrDefault(),

                });
                vendorModel.AddRange(listModel);


                Model.Report = vendorModel;
            }

        }

        public ActionResult Vendor(RfqGenModel Model)
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
            var RFQ = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId && u.VendorId == Model.VendorId).FirstOrDefault();
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
            var POList = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId && u.VendorId == vendor.Id).ToList();
            model2.TotalAmount = POList.Sum(x => x.QuotedAmount);
            var listModel = POList.Select(x => new RFQDetailsModel
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
            poModel.AddRange(listModel);


            model2.RFQDetails = poModel;
            
            return View(model2);
        }
        public ActionResult RfqDetails(ReportModel Model)
        {
            var RFQ = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId && u.VendorId == Model.VendorId).FirstOrDefault();
            var vendor = _reportRepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
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
            var RFQList = _reportRepository.GetRFQDetails().Where(u => u.RFQId == Model.RfqId && u.VendorId == vendor.Id).ToList();
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
            var period = _reportRepository.GetEvaluationPeriods().ToList();
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

                Model.PeriodList = period.Where(u => u.EndDate >= DateTime.Now).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Period
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
                Model.PeriodList = period.Where(u => u.EndDate <= DateTime.Now).Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Period
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

        public ActionResult VendorEvaluationReport(RfqGenModel Model)
        {

            try
            {
                //RfqGenModel Model = new RfqGenModel();

                VendorEvaluationReportPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View();
            }
        }

        // GET: Evaluation Period
        public ActionResult EvaluationPeriodIndex()
        {
            try
            {
                List<ReportModel> poModel = new List<ReportModel>();

                var period = _reportRepository.GetEvaluationPeriods().ToList();

                //var RfqList = _rfqGenRepository.GetRfqGen().OrderBy(u => u.EndDate).ToList();
                var PeriodList = period.Select(x => new ReportModel
                {
                    PeriodId = x.Id,
                    EvaluationPeriod = x.Period,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive
                });
                return View(PeriodList);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: Report/EvaluationPeriodCreate
        public ActionResult EvaluationPeriodCreate()
        {
            return View();
        }

        // POST: Report/EvaluationPeriodCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvaluationPeriodCreate(ReportModel model)
        {
            try
            {
                string message;
                model.CreatedBy = User.Identity.Name;

                if (ModelState.IsValid)
                {

                    var status = _reportRepository.CreateEvaluationPeriod(model, out message);

                    if (Convert.ToDateTime(model.StartDate) >= Convert.ToDateTime(model.EndDate))
                    {
                        Alert("Start Date Cannot be Greater or Equal to End Date", NotificationType.error);
                        return View(model);
                    }
                    if (status == true)
                    {

                        Alert("Evaluation Period Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Evaluation Period Already Exists", NotificationType.info);
                        return View(model);
                    }

                    return RedirectToAction("EvaluationPeriodIndex", "Report");
                }
                else
                {
                    ViewBag.StatusCode = 2;
                    Alert("Evaluation Period Wasn't Created", NotificationType.error);

                    return View(model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        // GET: Report/EvaluationPeriodEdit
        public ActionResult EvaluationPeriodEdit(int PeriodId)
        {


            ReportModel Model = new ReportModel();

            try
            {

                var Period = _reportRepository.GetEvaluationPeriods().Where(u => u.Id == PeriodId).FirstOrDefault();

                if (Period == null)
                {
                    Alert("This Period Doesn't Exist", NotificationType.warning);

                    return RedirectToAction("EvaluationPeriodIndex", "Report");
                }

                Model.PeriodId = Period.Id;

                Model.EvaluationPeriod = Period.Period;

                Model.StartDate = Convert.ToDateTime(Period.StartDate);

                Model.EndDate = Convert.ToDateTime(Period.EndDate);
               

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }

        }

        // POST: Report/EvaluationPeriodEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EvaluationPeriodEdit(ReportModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    var Period = _reportRepository.GetEvaluationPeriods().FirstOrDefault(u => u.Id == Model.PeriodId);

                    if (Period == null) { Alert("This Evaluation Period Doesn't Exist", NotificationType.warning); return RedirectToAction("EvaluationPeriodIndex", "Report"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _reportRepository.UpdateEvaluationPeriod(Model, out message);

                    if (Convert.ToDateTime(Model.StartDate) >= Convert.ToDateTime(Model.EndDate))
                    {
                        Alert("Start Date Cannot be Greater or Equal to End Date", NotificationType.error);
                        return View(Model);
                    }
                    if (status == true)
                    {
                        Alert("Evaluation Period Updated Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Evaluation Period Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("EvaluationPeriodIndex", "Report");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Evaluation Period Wasn't Updated", NotificationType.error);

                    return View(Model);
                }
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

    }
}