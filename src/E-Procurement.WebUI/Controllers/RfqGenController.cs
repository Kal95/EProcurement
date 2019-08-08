using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.RFQGenRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    public class RfqGenController : BaseController
    {
        private readonly IRfqGenRepository _rfqGenRepository;
        private readonly IMapper _mapper;
        private readonly IVendorRepository _vendorRepository;

        public RfqGenController(IRfqGenRepository rfqRepository, IMapper mapper,IVendorRepository vendorRepository)
        {
            _rfqGenRepository = rfqRepository;
            _mapper = mapper;
           _vendorRepository = vendorRepository;
    }
        // GET: RfqGen
        public ActionResult Index()
        {
            try
            {
                var model = _rfqGenRepository.GetRfqGen().ToList();
                
                List<RfqGenModel> smodel = _mapper.Map<List<RfqGenModel>>(model);

                return View(smodel);
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
        
        // GET: Rfq/Create
        public ActionResult Create()
        {

            try
            {
                RfqGenModel Model = new RfqGenModel();

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
    }

}