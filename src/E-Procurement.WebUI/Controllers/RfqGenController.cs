using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.RFQGenRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class RfqGenController : Controller
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
        //Get PDF
        public /*ActionResult*/ void PrintPDF(RfqGenModel model, RfqNoteModel model2)
        {
            var items = _rfqGenRepository.GetItem(model.CategoryId).Where(a => model.SelectedItems.Any(b => b == a.Id.ToString())).ToList();

            var selected2 = model.Descriptions.Zip(model.Quantities, (x, y) => new { X = x, Y = y.ToString() });
            //model2.AllItemProps = items.Zip(selected2, (a, b) => new { A = a, B = b });

           
            var selectedV = model.SelectedVendors.ToList();
            var vendors = _vendorRepository.GetVendors().ToList();
            var vendorList = vendors.Where(a => selectedV.Any(b => b == a.Id)).ToList();
            foreach (var entry in vendorList)
            {
                //var itemList = items.Where(a => model.SelectedItems.Any(b => b == a.Id.ToString())).ToList();
                
                model2.ContactName = entry.ContactName;
                model2.VendorName =entry.VendorName;
                model2.VendorAddress = entry.VendorAddress;
                model2.ItemList =  items.Select(u => u.ItemName).ToList();
                model2.Quantities = model.Quantities.ToList();
                model2.Descriptions = model.Descriptions.ToList();
              
            }
            //    empEntities context = new empEntities();
            //    List<emp_table> Data = context.emp_table.ToList();

            //    return new PartialViewAsPdf("_JobPrint", Data)
            //    {
            //        FileName = "TestPartialViewAsPdf.pdf"
            //    };
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

                return View("Error");
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

                if (ModelState.IsValid)
                {
                    Model.CreatedBy = UserId;

                    var status = _rfqGenRepository.CreateRfqGen(Model, out message);

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

                    return RedirectToAction("Index", "RfqGen");
                }
                else
                {
                    LoadPredefinedInfo(Model);

                    ViewBag.StatusCode = 2;

                    return View(Model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        public ActionResult Edit(int RfqId)
        {

            RfqGenModel Model = new RfqGenModel();

            try
            {
                var rfq = _rfqGenRepository.GetRfqGen().Where(u => u.Id == RfqId).FirstOrDefault();

                if (rfq == null)
                {
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
                if (ModelState.IsValid)
                {
                    string message;

                    var rfq = _rfqGenRepository.GetRfqGen().Where(u => u.Id == Model.Id).FirstOrDefault();

                    if (rfq == null) { return RedirectToAction("Index", "RfqGen"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _rfqGenRepository.UpdateRfqGen(Model, out message);

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

                    return RedirectToAction("Index", "RfqGen");
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