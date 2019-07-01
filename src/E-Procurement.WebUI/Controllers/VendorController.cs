using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IBankRepository _bankRepository;
        private readonly IVendorCategoryRepository _vendorCategoryRepository;
        private readonly IMapper _mapper;

        public VendorController(IVendorRepository vendorRepository, IVendorCategoryRepository vendorCategoryRepository, ICountryRepository countryRepository, IStateRepository stateRepository, IBankRepository bankRepository, IMapper mapper)
        {
            _vendorRepository = vendorRepository;
            _vendorCategoryRepository = vendorCategoryRepository;
            _countryRepository = countryRepository;
            _stateRepository = stateRepository;
            _bankRepository = bankRepository;
            _mapper = mapper;
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


        private void LoadPredefinedInfo(VendorModel Model)
        {


            var vendorCategory = _vendorRepository.GetItemCategory().ToList();

            var country = _countryRepository.GetCountries().ToList();

            var state = _stateRepository.GetStates().ToList();

            var bank = _bankRepository.GetBanks().ToList();


            Model.VendorCategoryList = vendorCategory.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CategoryName
            });


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

                    var status = _vendorRepository.CreateVendor(Model, out message);

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

                    return RedirectToAction("Index", "Vendor");
                }
                else
                {
                    LoadPredefinedInfo(Model);

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
                var vendor = _vendorRepository.GetVendors().Where(u => u.Id == VendorId).FirstOrDefault();
              
                if (vendor == null)
                {
                    return RedirectToAction("Index", "Vendor");
                }

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

                    if (vendor == null) { return RedirectToAction("Index", "Vendor"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _vendorRepository.UpdateVendor(Model, out message);

                    ViewBag.Message = TempData["MESSAGE"] as AlertMessage;

                    if (status == true)
                    {

                        ViewBag.Message = TempData["MESSAGE"] as AlertMessage;
                    }

                    else
                    {
                        ViewBag.Message = TempData["MESSAGE"] as AlertMessage;
                        return RedirectToAction("Index", "Vendor");
                    }

                    return RedirectToAction("Index", "Vendor");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    ViewBag.Message = TempData["MESSAGE"] as AlertMessage;

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