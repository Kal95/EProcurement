using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.BankRepo;
using E_Procurement.Repository.CountryRepo;
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
    public class VendorController : BaseController
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IBankRepository _bankRepository;
        private readonly IVendorCategoryRepository _vendorCategoryRepository;
        private readonly IMapper _mapper;
        private IHostingEnvironment _hostingEnv;

        public VendorController(IVendorRepository vendorRepository, IVendorCategoryRepository vendorCategoryRepository, ICountryRepository countryRepository, IStateRepository stateRepository, IBankRepository bankRepository, IMapper mapper, IHostingEnvironment hostingEnv)
        {
            _vendorRepository = vendorRepository;
            _vendorCategoryRepository = vendorCategoryRepository;
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _bankRepository = bankRepository;
            _mapper = mapper;
            _hostingEnv = hostingEnv;
        }
        public ActionResult Index()
        {
            try
            {
                var model = _vendorRepository.GetVendors().ToList();

                List<VendorModel> smodel = _mapper.Map<List<VendorModel>>(model);
                return View(smodel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public ActionResult VendorDocuments()
        {
            try
            {
                var model = _vendorRepository.GetVendors().ToList();

                List<VendorModel> smodel = _mapper.Map<List<VendorModel>>(model);
                return View(smodel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        private void LoadFilePath(VendorModel Model)
        {
            if (Model.BankReference != null || Model.CertificateOfVAT !=null || Model.MemorandumOfAssoociation != null || Model.NoticeOfSituationAddress != null || Model.ParticularsOfDirectors != null || Model.ParticularsOfShareholders != null || Model.Reference != null || Model.TaxClearance != null)
            {
                var myReference = new Random();
                string referencecode = myReference.Next(23006).ToString();

                string webRootPath = _hostingEnv.WebRootPath;

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var checkextension1 = Path.GetExtension(Model.MemorandumOfAssoociation.FileName).ToLower();
                var checkextension2 = Path.GetExtension(Model.NoticeOfSituationAddress.FileName).ToLower();
                var checkextension3 = Path.GetExtension(Model.ParticularsOfDirectors.FileName).ToLower();
                var checkextension4 = Path.GetExtension(Model.ParticularsOfShareholders.FileName).ToLower();
                var checkextension5 = Path.GetExtension(Model.Reference.FileName).ToLower();
                var checkextension6 = Path.GetExtension(Model.TaxClearance.FileName).ToLower();
                var checkextension7 = Path.GetExtension(Model.CertificateOfVAT.FileName).ToLower();
                var checkextension8 = Path.GetExtension(Model.BankReference.FileName).ToLower();

                var MOAFilePath = referencecode + "_" + "Memorandum_Of_Association" + Path.GetExtension(Model.MemorandumOfAssoociation.FileName);
                var NOSFilePath = referencecode + "_" + "Notice_Of_Situation" + Path.GetExtension(Model.NoticeOfSituationAddress.FileName);
                var PODFilePath = referencecode + "_" + "Particulars_Of_Directors" + Path.GetExtension(Model.ParticularsOfDirectors.FileName);
                var POSFilePath = referencecode + "_" + "Particulars_Of_Shareholders" + Path.GetExtension(Model.ParticularsOfShareholders.FileName);
                var RefFilePath = referencecode + "_" + "Reference" + Path.GetExtension(Model.Reference.FileName);
                var TaxFilePath = referencecode + "_" + "Tax_Clearance" + Path.GetExtension(Model.TaxClearance.FileName);
                var COVFilePath = referencecode + "_" + "Certificate_Of_VAT" + Path.GetExtension(Model.CertificateOfVAT.FileName);
                var BankRefFilePath = referencecode + "_" + "Bank_Reference_Letter" + Path.GetExtension(Model.BankReference.FileName);

                if (!allowedExtensions.Contains(checkextension1) || !allowedExtensions.Contains(checkextension2) || !allowedExtensions.Contains(checkextension3) || !allowedExtensions.Contains(checkextension4) || !allowedExtensions.Contains(checkextension5) || !allowedExtensions.Contains(checkextension6) || !allowedExtensions.Contains(checkextension7) || !allowedExtensions.Contains(checkextension8))
                {
                    ModelState.AddModelError("", "Invalid file extention.");
                    //return View(Model);
                }

                string dbFilePath = "~/upload/VendorDocuments/";

                //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", imageFilePath);
                var path1 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", MOAFilePath);
                var path2 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", NOSFilePath);
                var path3 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", PODFilePath);
                var path4 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", POSFilePath);
                var path5 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", RefFilePath);
                var path6 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", TaxFilePath);
                var path7 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", COVFilePath);
                var path8 = Path.Combine(webRootPath, "Uploads", "VendorDocuments", BankRefFilePath);


                if (System.IO.File.Exists(path1)) { System.IO.File.Delete(path1); }
                if (System.IO.File.Exists(path2)) { System.IO.File.Delete(path2); }
                if (System.IO.File.Exists(path3)) { System.IO.File.Delete(path3); }
                if (System.IO.File.Exists(path4)) { System.IO.File.Delete(path4); }
                if (System.IO.File.Exists(path5)) { System.IO.File.Delete(path5); }
                if (System.IO.File.Exists(path6)) { System.IO.File.Delete(path6); }
                if (System.IO.File.Exists(path7)) { System.IO.File.Delete(path7); }
                if (System.IO.File.Exists(path8)) { System.IO.File.Delete(path8); }

                using (Stream stream = new FileStream(path1, FileMode.Create)) { Model.MemorandumOfAssoociation.CopyTo(stream); }
                using (Stream stream = new FileStream(path2, FileMode.Create)) { Model.NoticeOfSituationAddress.CopyTo(stream); }
                using (Stream stream = new FileStream(path3, FileMode.Create)) { Model.ParticularsOfDirectors.CopyTo(stream); }
                using (Stream stream = new FileStream(path4, FileMode.Create)) { Model.ParticularsOfShareholders.CopyTo(stream); }
                using (Stream stream = new FileStream(path5, FileMode.Create)) { Model.Reference.CopyTo(stream); }
                using (Stream stream = new FileStream(path6, FileMode.Create)) { Model.TaxClearance.CopyTo(stream); }
                using (Stream stream = new FileStream(path7, FileMode.Create)) { Model.CertificateOfVAT.CopyTo(stream); }
                using (Stream stream = new FileStream(path8, FileMode.Create)) { Model.BankReference.CopyTo(stream); }

                Model.MOAFilePath = MOAFilePath;
                Model.NOSFilePath = NOSFilePath;
                Model.PODFilePath = PODFilePath;
                Model.POSFilePath = POSFilePath;
                Model.RefFilePath = RefFilePath;
                Model.TaxFilePath = TaxFilePath;
                Model.COVFilePath = COVFilePath;
                Model.BankRefFilePath = BankRefFilePath;
            }
        }

            private void LoadPredefinedInfo(VendorModel Model)
        {


            var vendorCategory = _vendorRepository.GetItemCategory().ToList();

            var country = _countryRepository.GetCountries().ToList();

            var state = _stateRepository.GetStates().ToList();

            var bank = _bankRepository.GetBanks().ToList();


            //var vendor = _vendorRepository.GetVendors().Where(u => u.Id == Model.VendorId).FirstOrDefault();

            var mapping = _vendorRepository.GetMapping().Where(u => u.VendorID == Model.Id).ToList();

            var SelectedCategories = vendorCategory.Where(a => mapping.Any(b => b.VendorCategoryId == a.Id)).ToList();
            var NewCategories = vendorCategory.Except(SelectedCategories);//.Where(a => mapping.Any(b => b.VendorCategoryId != a.Id)).ToList();
            if (SelectedCategories.ToList().Count == 0)
            {
                Model.VendorCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName,
                    Selected = true
                });
            }
            else 
            {
                Model.VendorCategoryList = NewCategories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName,
                    
                    Selected = false
                });

                Model.CurrentVendorCategoryList = SelectedCategories.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName,

                    Selected = true
                });
            }

            Model.CountryList = country.Select(x => new SelectListItem
            {

                Value = (x.Id).ToString(),
                Text = x.CountryName
            });

            Model.StateList = state.Select(x => new SelectListItem
            {

                Value = (x.Id).ToString(),
                Text = x.StateName
            });

            Model.BankList = bank.Select(x => new SelectListItem
            {

                Value = (x.Id).ToString(),
                Text = x.BankName
            });




        }


        // GET: Vendor/Create
        [PermissionValidation("can_create_vendor")]
        public ActionResult Create()
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

        // POST: Bank/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VendorModel Model)
        {
            try
            {
                string message;
                string UserId = User.Identity.Name;

                if (ModelState.IsValid)
                {
                    Model.CreatedBy = UserId;
                    
                    LoadFilePath(Model);
                    var status = _vendorRepository.CreateVendor(Model, out message);
                    

                    if (status == true)
                    {

                        Alert("Vendor Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("Vendor Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("Index", "Vendor");
                }
                else
                {
                    LoadPredefinedInfo(Model);
                    Alert("Vendor Wasn't Created", NotificationType.error);

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
        public ActionResult Edit(int VendorId)
        {

            VendorModel Model = new VendorModel();

            try
            {
                var categories = _vendorRepository.GetItemCategory().ToList();
               
                var vendor = _vendorRepository.GetVendors().Where(u => u.Id == VendorId).FirstOrDefault();

                var mapping = _vendorRepository.GetMapping().Where(u => u.VendorID == vendor.Id).ToList();

                var SelectedCategories = categories.Where(a => mapping.Any(b => b.VendorCategoryId == a.Id)).Select(u => u.Id).ToList();
                
                if (vendor == null)
                {
                    Alert("This Vendor Doesn't Exist", NotificationType.warning);
                    return RedirectToAction("Index", "Vendor");
                }
                Model.SelectedVendorCategories = SelectedCategories.ToList();
                
              
                List<VendorModel> selcat = new List<VendorModel>();
                
                Model.Id = vendor.Id;
                Model.VendorName = vendor.VendorName;
                Model.AatAmount = vendor.AatAmount;
                Model.AatCurrency = vendor.AatCurrency;
                Model.AccountName = vendor.AccountName;
                Model.AccountNo = vendor.AccountNo;
                Model.BankBranch = vendor.BankBranch;
                Model.BankId = vendor.BankId;
                Model.CacNo = vendor.CacNo;
                Model.ContactName = vendor.ContactName;
                Model.CountryId = vendor.CountryId;
                Model.StateId = vendor.StateId;
                Model.SortCode = vendor.SortCode;
                Model.TinNo = vendor.TinNo;
                
                Model.CreatedBy = vendor.CreatedBy;
                Model.DateCreated = vendor.DateCreated;
                Model.Email = vendor.Email;
                Model.PhoneNumber = vendor.PhoneNumber;
                Model.IsActive = vendor.IsActive;
                Model.VendorAddress = vendor.VendorAddress;
                Model.VendorStatus = vendor.VendorStatus;
                Model.WebsiteAddress = vendor.WebsiteAddress;
                Model.VatNo = vendor.VatNo;
                Model.MOAFilePath = vendor.MOAFilePath;
                Model.NOSFilePath = vendor.NOSFilePath;
                Model.PODFilePath = vendor.PODFilePath;
                Model.POSFilePath = vendor.POSFilePath;
                Model.RefFilePath = vendor.RefFilePath;
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

                    var vendor = _vendorRepository.GetVendors().FirstOrDefault(u => u.Id == Model.Id);

                    if (vendor == null) { Alert("This Vendor Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "Vendor"); }

                    Model.UpdatedBy = User.Identity.Name;

                    LoadFilePath(Model);
                    var status = _vendorRepository.UpdateVendor(Model, out message);
                    

                    if (status == true)
                    {

                        Alert("Vendor Updated Successfully", NotificationType.success);
                    }

                    else
                    {
                        Alert("This Vendor Already Exists", NotificationType.info);
                        return RedirectToAction("Index", "Vendor");
                    }

                    return RedirectToAction("Index", "Vendor");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Vendor Wasn't Updated", NotificationType.error);

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