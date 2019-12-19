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
using E_Procurement.Repository.StateRepo;
using E_Procurement.Repository.VendorCategoryRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class SignController : BaseController
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

        public SignController(IVendorRepository vendorRepository, IRfqApprovalRepository rfqApprovalRepository, IAccountManager accountManager, IVendorCategoryRepository vendorCategoryRepository, IReportRepository reportRepository, ICountryRepository countryRepository, IStateRepository stateRepository, IBankRepository bankRepository, IMapper mapper, IHostingEnvironment hostingEnv)
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
        }
       

        private void LoadFilePath(VendorModel Model)
        {
         
            if (Model.Signature1 != null || Model.Signature2 !=null )
            {
                var myReference = new Random();
                //string referencecode; 
              
                Model.RefCode = myReference.Next(23006).ToString();
                
                string webRootPath = _hostingEnv.WebRootPath;

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
             
                //string dbFilePath = "~/upload/VendorDocuments/";

                //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", imageFilePath);

                if (Model.Signature1 != null)
                {
                    var checkextension1 = Path.GetExtension(Model.Signature1.FileName).ToLower();
                    if (!allowedExtensions.Contains(checkextension1))
                    {
                        ModelState.AddModelError("", "Invalid file extention."); 
                    }
                    var Sign1FilePath = Model.RefCode + "_" + "Signature_1" + Path.GetExtension(Model.Signature1.FileName);
                    var path1 = Path.Combine(webRootPath, "Uploads", "Signatures", Sign1FilePath);
                    if (System.IO.File.Exists(path1)) { System.IO.File.Delete(path1); }
                    using (Stream stream = new FileStream(path1, FileMode.Create)) { Model.Signature1.CopyTo(stream); }
                    Model.Sign1Path = Sign1FilePath;
                }
                if (Model.Signature2 != null)
                {
                    var checkextension2 = Path.GetExtension(Model.Signature2.FileName).ToLower();
                    if (!allowedExtensions.Contains(checkextension2))
                    {
                        ModelState.AddModelError("", "Invalid file extention.");
                    }
                    var Sign2FilePath = Model.RefCode + "_" + "Signature_2" + Path.GetExtension(Model.Signature2.FileName);
                    var path2 = Path.Combine(webRootPath, "Uploads", "Signatures", Sign2FilePath);
                    if (System.IO.File.Exists(path2)) { System.IO.File.Delete(path2); }
                    using (Stream stream = new FileStream(path2, FileMode.Create)) { Model.Signature2.CopyTo(stream); }
                    Model.Sign2Path = Sign2FilePath;
                }
                
    
            }
        }

        private void LoadPredefinedInfo(VendorModel Model)
        {

            //var UserRole = _accountManager.GetRoles();
            //var signature = _rfqApprovalRepository.GetSignatures().Where(x => x.IsActive == true).ToList();
            var selectedUser = _rfqApprovalRepository.GetApprovalRoles_Users().Where(a => a.Id == Model.UserId1).FirstOrDefault();

             var signee1 = _rfqApprovalRepository.GetApprovalRoles_Users().ToList();

            var signee2 = _rfqApprovalRepository.GetApprovalRoles_Users().ToList();


            if (Model.UserId1 != 0)
            {
                signee2 = signee2.Where(a => a.Id != selectedUser.Id).ToList();
            }
        

            //var signature = _rfqApprovalRepository.GetSignatures().Where(u => u.Id == Model.Id).ToList();

           

            Model.S1List = signee1.Select(x => new SelectListItem
            {

                Value = (x.Id).ToString(),
                Text = x.FullName
            });

            Model.S2List = signee2.Select(x => new SelectListItem
            {

                Value = (x.Id).ToString(),
                Text = x.FullName
            });




        }

        public ActionResult SignatureIndex()
        {
            try
            {
                List<VendorModel> poModel = new List<VendorModel>();

                var config = _rfqApprovalRepository.GetSignatures().ToList();

                //var RfqList = _rfqGenRepository.GetRfqGen().OrderBy(u => u.EndDate).ToList();
                var ConfigList = config.Select(x => new VendorModel
                {
                    SignId = x.Id,
                    Signee1 = x.Signee1,
                    Signee2 = x.Signee2,
                    Sign1Path = x.Sign1,
                    Sign2Path = x.Sign2,
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
        public ActionResult SignatureCreate()
        {

            try
            {
                VendorModel Model = new VendorModel();

                LoadPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        // POST: Config/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignatureCreate(VendorModel Model)
        {
            try
            {
                if (Model.UserId2 == 0)
                {
                    LoadPredefinedInfo(Model);

                    return View(Model);
                }
                string message;
                string UserId = User.Identity.Name;

                var UserSign = _reportRepository.GetUser().ToList();

                if (ModelState.IsValid)
                {
                    Model.CreatedBy = UserId;
                    Model.Signee1 = UserSign.Where(a => a.Id == Model.UserId1).Select(a => a.Email).FirstOrDefault();
                    Model.Signee2 = UserSign.Where(a => a.Id == Model.UserId2).Select(a => a.Email).FirstOrDefault();

                    LoadFilePath(Model);
                    var status = _rfqApprovalRepository.CreateSignature(Model, out message);


                    if (status == true)
                    {

                        Alert("Signature Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("Signature Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("SignatureIndex", "Sign");
                }
                else
                {
                    LoadPredefinedInfo(Model);
                    Alert("Signature Wasn't Created", NotificationType.error);

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

        public ActionResult Edit(int VendorId)
        {

            VendorModel Model = new VendorModel();

            try
            {
                var userId = _vendorRepository.GetUser().Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();

                var loggedInVendor = _vendorRepository.GetVendors().Where(u => u.UserId == userId).Select(u => u.Id).FirstOrDefault();

                var categories = _vendorRepository.GetItemCategory().ToList();

                if (User.IsInRole("Vendor User") && !loggedInVendor.Equals(null))
                {
                
                VendorId = loggedInVendor;
                    
                }

                var vendor = _vendorRepository.GetVendors().Where(u => u.Id == VendorId).FirstOrDefault();

                var mapping = _vendorRepository.GetMapping().Where(u => u.VendorID == vendor.Id).ToList();

                var SelectedCategories = categories.Where(a => mapping.Any(b => b.VendorCategoryId == a.Id)).Select(u => u.Id).ToList();
                
                if (vendor == null)
                {
                    Alert("This Signature Doesn't Exist", NotificationType.warning);
                    return RedirectToAction("Index", "Sign");
                }
                Model.SelectedVendorCategories = SelectedCategories.ToList();
                
              
                List<VendorModel> selcat = new List<VendorModel>();
                
                Model.Id = vendor.Id;
                Model.VendorName = vendor.VendorName;
                Model.AatAmount = vendor.AatAmount;
                Model.AatCurrency = vendor.AatCurrency;
                Model.AccountName = vendor.AccountName;
                Model.AccountNo = vendor.AccountNo;
               
                Model.CreatedBy = vendor.CreatedBy;
                Model.DateCreated = vendor.DateCreated;
                Model.Email = vendor.Email;
                Model.PhoneNumber = vendor.PhoneNumber;
                Model.IsActive = vendor.IsActive;
               
                Model.TaxFilePath = vendor.TaxFilePath;
                Model.COVFilePath = vendor.COVFilePath;
                Model.BankRefFilePath = vendor.BankRefFilePath;
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
        public ActionResult Edit(VendorModel Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string message;

                    var userId = _vendorRepository.GetUser().Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();

                    var loggedInVendor = _vendorRepository.GetVendors().Where(u => u.UserId == userId).Select(u => u.Id).FirstOrDefault();

                    var vendor = _vendorRepository.GetVendors().FirstOrDefault(u => u.Id == Model.Id);

                    if (vendor == null) { Alert("This Signature Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "Sign"); }

                    Model.UpdatedBy = User.Identity.Name;

                    LoadFilePath(Model);
                    var status = _vendorRepository.UpdateVendor(Model, out message);

                    if (User.IsInRole("Vendor User") && !loggedInVendor.Equals(null))
                    {
                        if (status == true)
                        {

                            Alert("Record Updated Successfully", NotificationType.success);
                        }

                        else
                        {
                            Alert("This Record Already Exists", NotificationType.info);
                            return RedirectToAction("Index", "Home");
                        }

                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        if (status == true)
                        {

                            Alert("Signature Updated Successfully", NotificationType.success);
                        }

                        else
                        {
                            Alert("This Signature Already Exists", NotificationType.info);
                            return RedirectToAction("Index", "Sign");
                        }

                        return RedirectToAction("Index", "Sign");
                    }
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Signature Wasn't Updated", NotificationType.error);

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