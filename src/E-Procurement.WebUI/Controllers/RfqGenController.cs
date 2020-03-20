using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.RequisitionRepo;
using E_Procurement.Repository.RFQGenRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Models.RequisitionModel;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class RfqGenController : BaseController
    {
        private readonly IRfqGenRepository _rfqGenRepository;
        private readonly IMapper _mapper;
        private readonly IVendorRepository _vendorRepository;
        private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IRequisitionRepository _requisitionRepository;
        private IHostingEnvironment _hostingEnv;

        public RfqGenController(IRfqGenRepository rfqRepository, IRequisitionRepository requisitionRepository, IRfqApprovalRepository RfqApprovalRepository, IMapper mapper,IVendorRepository vendorRepository, IHostingEnvironment hostingEnv)
        {
            _rfqGenRepository = rfqRepository;
            _mapper = mapper;
           _vendorRepository = vendorRepository;
            _RfqApprovalRepository = RfqApprovalRepository;
            _requisitionRepository = requisitionRepository;
            _hostingEnv = hostingEnv;
        }
        // GET: RfqGen
        public ActionResult Index()
        {
            try
            {
                var model = _rfqGenRepository.GetRfqGen().ToList();
                
                List<RfqGenModel> smodel = _mapper.Map<List<RfqGenModel>>(model);
                //return View(smodel);


                var config = _requisitionRepository.GetRequisitions2().ToList();

                var tran = (//from req in config
                            //join mode in model on req.Id equals mode.RequisitionId 
                            from mode in model
                            select new RfqGenModel()
                            {
                                ProjectId = mode.ProjectId,
                                RequisitionId = mode.RequisitionId,
                                Reference = mode.Reference,
                                Item = mode.Item,
                                VendorName = mode.VendorName,
                                StartDate = mode.StartDate,
                                EndDate = mode.EndDate,
                                CreatedDate = mode.CreatedDate,
                                RFQStatus = mode.RFQStatus,
                                RequisitionDocumentPath = config.Where(a => a.Id == mode.RequisitionId).Select(b => b.RequisitionDocument).FirstOrDefault(),
                                RFQId = mode.RFQId
                            });

                return View(tran);

                
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
       

        private string GenerateRfqReference()
        {
            var myReference = new Random();
            string referencecode = myReference.Next(23006).ToString();
            return referencecode;
        }

        private void LoadPredefinedInfo(RfqGenModel Model)
        {
            int CategoryId = Model.CategoryId;


            var ItemCategory = _rfqGenRepository.GetItemCategory().ToList();

            var Item = _rfqGenRepository.GetItem(CategoryId).ToList();

            var Vendor = _rfqGenRepository.GetVendors(Model).ToList();


            Model.ItemCategoryList = ItemCategory.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CategoryName
            });


            Model.ItemList = Item.Select(x => new SelectListItem
            {

                Value = x.Id.ToString(),
                Text = x.ItemName
            });

            Model.VendorList = Vendor.Select(x => new SelectListItem
            {

                Value = x.Id.ToString(),
                Text = x.VendorName
            });


            //Model.VendorList = Vendor;

        }

        private void LoadFilePath(RequisitionModel Model)
        {

            if (Model.RequisitionDocument != null)
            {
                var myReference = new Random();
                //string referencecode; 

                Model.RefCode = myReference.Next(23006).ToString();

                string webRootPath = _hostingEnv.WebRootPath;

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

                var checkextension1 = Path.GetExtension(Model.RequisitionDocument.FileName).ToLower();
                if (!allowedExtensions.Contains(checkextension1))
                {
                    ModelState.AddModelError("", "Invalid file extention.");
                }
                var RequisitionFilePath = Model.RefCode + "_" + "Requisition" + Path.GetExtension(Model.RequisitionDocument.FileName);
                var path1 = Path.Combine(webRootPath, "Uploads", "Requisitions", RequisitionFilePath);
                if (System.IO.File.Exists(path1)) { System.IO.File.Delete(path1); }
                using (Stream stream = new FileStream(path1, FileMode.Create)) { Model.RequisitionDocument.CopyTo(stream); }
                Model.RequisitionDocumentPath = RequisitionFilePath;

            }
        }

        // GET: Rfq/Create
        public ActionResult Create(string Initiator, int RequisitionId)
        {

            try
            {
                RfqGenModel Model = new RfqGenModel();
                if (Initiator != null)
                {
                    Model.InitiatedBy = Initiator;
                    Model.RequisitionId = RequisitionId;
                }

                LoadPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View();
            }
        }

        // POST: Rfq/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RfqGenModel Model)
        {
            try
            {
                Model.Reference = GenerateRfqReference();
                string message;
                string UserId = User.Identity.Name;
                if (ModelState.IsValid && Convert.ToDateTime(Model.StartDate) > Convert.ToDateTime(Model.EndDate))
                {
                    Alert("Start Date Cannot be Later Than End Date", NotificationType.error);
                    LoadPredefinedInfo(Model);
                    return View(Model);
                }
                if (ModelState.IsValid && Convert.ToDateTime(Model.EndDate) < Convert.ToDateTime(DateTime.Now))
                {
                    Alert("You Selected An End Date That Is In The Past", NotificationType.error);
                    LoadPredefinedInfo(Model);
                    return View(Model);
                }
                if (ModelState.IsValid)
                {
                    Model.CreatedBy = UserId;
                    if (Model.RequisitionId == 0)
                    {
                        RequisitionModel Model2 = new RequisitionModel();
                        Model2.RequisitionDocument = Model.RequisitionDocument;

                        //Get Attach Files
                        LoadFilePath(Model2);

                        Model2.Initiator = Model.InitiatedBy;
                        Model2.Description = Model.RFQTitle;
                        Model2.IsActive = false;
                        Model2.UpdatedBy = UserId;
                        Model2.DateUpdated = DateTime.Now;
                        Model2.ExpectedDate = Model.EndDate;
                        var status2 = _requisitionRepository.CreateRequisition(Model2, out message);

                        var config = _requisitionRepository.GetRequisitions2().Where(a => a.RequisitionDocument == Model2.RequisitionDocumentPath && a.UpdatedBy == UserId).FirstOrDefault();
                        Model.RequisitionId = config.Id;
                    }
                    else
                    {
                        RequisitionModel Model2 = new RequisitionModel();
                        Model2.Id = Model.RequisitionId;
                        Model2.Initiator = Model.InitiatedBy;
                        Model2.Description = Model.RFQTitle;
                        Model2.IsActive = false;
                        Model2.UpdatedBy = UserId;
                        Model2.ExpectedDate = Model.EndDate;
                        var status2 = _requisitionRepository.UpdateRequisition(Model2, out message);
                    }

                    var status = _rfqGenRepository.CreateRfqGen(Model, out message);

                    

                    if (status == true)
                    {

                        Alert("RFQ Generated Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This RFQ Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("Index", "RfqGen");
                }
                else
                {
                    LoadPredefinedInfo(Model);

                    ViewBag.StatusCode = 2;

                    Alert("RFQ Wasn't Generated", NotificationType.error);

                    return View(Model);

                }
            }

            catch (Exception)
            {

                LoadPredefinedInfo(Model);
                return View(Model);
            }
        }

        public ActionResult Edit(int RfqId)
        {

            RfqGenModel Model = new RfqGenModel();

            try
            {
                var rfq = _rfqGenRepository.GetRfqGen().Where(u => u.RFQId == RfqId).FirstOrDefault();

                if (rfq == null)
                {
                    Alert("This RFQ Doesn't Exist", NotificationType.warning);
                    return RedirectToAction("Index", "RfqGen");
                }

                Model.Reference = rfq.Reference;

                Model.ProjectId = rfq.ProjectId;

                Model.RequisitionId = rfq.RequisitionId;

                Model.StartDate = rfq.StartDate;

                Model.EndDate = rfq.EndDate;

                Model.RfqStatus = rfq.RFQStatus;

                LoadPredefinedInfo(Model);

                return View(Model);

            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RfqGenModel Model)
        {
            try
            {
                if (Convert.ToDateTime(Model.StartDate) >= Convert.ToDateTime(Model.EndDate))
                {
                    Alert("Start Date Cannot be Greater or Equal to End Date", NotificationType.error);
                    LoadPredefinedInfo(Model);
                    return View(Model);
                }
                if (ModelState.IsValid)
                {
                    string message;

                    var rfq = _rfqGenRepository.GetRfqGen().Where(u => u.RFQId == Model.Id).FirstOrDefault();

                    if (rfq == null) { Alert("This RFQ Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "RfqGen"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _rfqGenRepository.UpdateRfqGen(Model, out message);
                    

                    if (status == true)
                    {

                        Alert("RFQ Updated Successfully", NotificationType.success);
                    }

                    else
                    {
                        Alert("This RFQ Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("Index", "RfqGen");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("RFQ Wasn't Generated", NotificationType.error);

                    return View(Model);
                }
            }
            catch (Exception)
            {

                return View("Error");
            }

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Close(int RFQId2, RfqGenModel Model)
        {
            try
            {
                    Model.RFQId = RFQId2;
                    string message;

                    var rfq = _rfqGenRepository.GetRfqGen().Where(u => u.RFQId == Model.RFQId).FirstOrDefault();

                    if (rfq == null) { Alert("This RFQ Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "RfqGen"); }

                    Model.UpdatedBy = User.Identity.Name;
                    Model.Id = Model.RFQId;

                    var status = _rfqGenRepository.CloseRfqGen(Model, out message);


                    if (status == true)
                    {

                        Alert("RFQ Closed Successfully", NotificationType.success);
                    }

                    else
                    {
                        Alert("This RFQ Cannot be closed", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("Index", "RfqGen");
               
            }

            catch (Exception)
            {

                return View("Error");
            }

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Extend(RfqGenModel Model)
        {
            try
            {
                string message;

                var rfq = _rfqGenRepository.GetRfqGen().Where(u => u.RFQId == Model.RFQId).FirstOrDefault();
                var initiatedRFQ = _rfqGenRepository.GetInitiatedRfq().Where(u => u.RFQId == Model.RFQId).FirstOrDefault();
                if (initiatedRFQ != null) { Alert("This RFQ cannot been Extended, Approval has already been initiated", NotificationType.warning); return RedirectToAction("Index", "RfqGen"); }
                if (rfq == null) { Alert("This RFQ Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "RfqGen"); }

                Model.UpdatedBy = User.Identity.Name;
                Model.Id = Model.RFQId;
                Model.EndDate = Model.EndDate;

                var status = _rfqGenRepository.ExtendRfqGen(Model, out message);


                if (status == true)
                {

                    Alert("RFQ Extended Successfully", NotificationType.success);
                }

                else
                {
                    Alert("This RFQ Cannot be Extended", NotificationType.info);
                    return View(Model);
                }

                return RedirectToAction("Index", "RfqGen");

            }
            catch (Exception)
            {

                return View("Error");
            }

        }

        #region "RFQ In Pipeline"


        public async Task<IActionResult> RfqInProgress()
        {
            try { 
                    var RfqApprovalList = await _RfqApprovalRepository.GetRFQInPipelineAsync();

                    List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

                    return View(RfqApproval);

            }
            catch (Exception)
            {

                    return View("Error");
            }
        }

        public async Task<IActionResult> RfqVendorsDetails(int id)
        {
            try { 
                var RfqApprovalDetails = await _RfqApprovalRepository.GetSubmittedRFQByVendorsAsync(id);



                List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalDetails);

                if (RfqApproval == null)
                {
                    Alert("Could not load rfq details. Please, try again later.", NotificationType.error);
                    return View();
                }


                return View(RfqApproval);
            }
            catch (Exception)
            {

                    return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> RfqDetails(int id, int VendorId)
        {
            try { 
                var RfqApprovalDetails = await _RfqApprovalRepository.GetRFQDetailsAsync(id, VendorId);

           
                if (RfqApprovalDetails == null)
                {
                    Alert("Could not load rfq details. Please, try again later.", NotificationType.error);
                    return View();
                }


                return View(RfqApprovalDetails);
            }
            catch (Exception)
            {

                    return View("Error");
            }
        }

        #endregion
    }

}