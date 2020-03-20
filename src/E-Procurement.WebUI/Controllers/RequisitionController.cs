using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.BankRepo;
using E_Procurement.Repository.CountryRepo;
using E_Procurement.Repository.ReportRepo;
using E_Procurement.Repository.RequisitionRepo;
using E_Procurement.Repository.StateRepo;
using E_Procurement.Repository.VendorCategoryRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Filters;
using E_Procurement.WebUI.Models.RequisitionModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class RequisitionController : BaseController
    {
        private readonly IRfqApprovalRepository _rfqApprovalRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IBankRepository _bankRepository;
        private readonly IVendorCategoryRepository _vendorCategoryRepository;
        private readonly IMapper _mapper;
        private IHostingEnvironment _hostingEnv;
        private readonly IAccountManager _accountManager;
        private readonly IRequisitionRepository _requisitionRepository;

        public RequisitionController(IVendorRepository vendorRepository, IRequisitionRepository requisitionRepository, IRfqApprovalRepository rfqApprovalRepository, IAccountManager accountManager, IVendorCategoryRepository vendorCategoryRepository, IReportRepository reportRepository, ICountryRepository countryRepository, IStateRepository stateRepository, IBankRepository bankRepository, IMapper mapper, IHostingEnvironment hostingEnv)
        {
            _vendorRepository = vendorRepository;
            _vendorCategoryRepository = vendorCategoryRepository;
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _bankRepository = bankRepository;
            _reportRepository = reportRepository;
            _mapper = mapper;
            _hostingEnv = hostingEnv;
            _rfqApprovalRepository = rfqApprovalRepository;
            _accountManager = accountManager;
            _requisitionRepository = requisitionRepository;
        }
       

        private void LoadFilePath(RequisitionModel Model)
        {
         
            if (Model.RequisitionDocument != null  )
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


        public ActionResult RequisitionIndex()
        {
            try
            {

                var config = _requisitionRepository.GetRequisitions().ToList();

                var ConfigList = config.Select(x => new RequisitionModel
                {
                    Id = x.Id,
                    Initiator = x.Initiator,
                    Description = x.Description,
                    ExpectedDate = x.ExpectedDate,
                    RequisitionDocumentPath = x.RequisitionDocument,
                    DateCreated = x.DateCreated,
                    IsActive = x.IsActive
                });
                return View(ConfigList);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }


        // GET: Config/Create
        public ActionResult RequisitionCreate()
        {

            try
            {

                return View();
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        // POST: Config/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RequisitionCreate(RequisitionModel Model)
        {
            try
            {
                string message;
                string UserId = User.Identity.Name;

                var UserSign = _reportRepository.GetUser().Where(a => a.UserName == UserId).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    Model.Initiator = UserSign.Email;

                    LoadFilePath(Model);
                    Model.IsActive = true;
                    var status = _requisitionRepository.CreateRequisition(Model, out message);


                    if (status == true)
                    {

                        Alert("Requisition Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("Requisition Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("ApprovedRFQ", "PO");
                }
                else
                {
                    
                    Alert("Requisition Wasn't Created", NotificationType.error);

                    ViewBag.StatusCode = 2;

                    return View(Model);

                }
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ex.Message);
            }
        }

        public ActionResult Activate(int SignId, VendorModel Model)
        {
            try
            {
                Model.SignId = SignId;
                string message;

                var sign = _rfqApprovalRepository.GetSignatures().Where(u => u.Id == Model.SignId).FirstOrDefault();

                if (sign == null) { Alert("This RFQ Doesn't Exist", NotificationType.warning); return RedirectToAction("SignatureIndex", "Sign"); }

                Model.UpdatedBy = User.Identity.Name;
                

                var status = _rfqApprovalRepository.ActivateSignature(Model, out message);


                if (status == true)
                {

                    Alert("Signature Activated Successfully", NotificationType.success);
                }

                else
                {
                    Alert("This Signature is already active", NotificationType.info);
                    return View(Model);
                }

                return RedirectToAction("SignatureIndex", "Sign");

            }

            catch (Exception)
            {

                return View("Error");
            }

        }

        public ActionResult SignatureEdit(int UserId)
        {

            VendorModel Model = new VendorModel();

            try
            {
                

                var config = _reportRepository.GetUserToCategoryConfig().Where(u => u.UserId == UserId).FirstOrDefault();


                if (config == null)
                {
                    Alert("This Config Doesn't Exist", NotificationType.warning);
                    return RedirectToAction("UserToCategoryIndex", "Report");
                }
                
                Model.UserId = config.UserId;
                Model.IsActive = config.IsActive;
                //LoadPredefinedInfo(Model);


                return View(Model);

            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignatureEdit(VendorModel Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string message;

                    var config = _reportRepository.GetUserToCategoryConfig().FirstOrDefault(u => u.UserId == Model.UserId);

                    if (config == null) { Alert("This Config Doesn't Exist", NotificationType.warning); return RedirectToAction("UserToCategoryIndex", "Report"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _reportRepository.UpdateUserToCategory(Model, out message);


                    if (status == true)
                    {

                        Alert("Config Updated Successfully", NotificationType.success);
                    }

                    else
                    {
                        Alert("This Config Already Exists", NotificationType.info);
                        return RedirectToAction("UserToCategoryIndex", "Report");
                    }

                    return RedirectToAction("UserToCategoryIndex", "Report");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Config Wasn't Updated", NotificationType.error);

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