using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.RFQGenRepo;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class RfqGenController : Controller
    {
        private readonly IRfqGenRepository _rfqGenRepository;
        private readonly IMapper _mapper;

        public RfqGenController(IRfqGenRepository rfqRepository, IMapper mapper)
        {
            _rfqGenRepository = rfqRepository;
            _mapper = mapper;
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

            var Vendor = _rfqGenRepository.GetVendors(CategoryId).ToList();

           
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
                //var participantList = Model.SelectMany(a => a
                //                                     .SelectMany(b => b, c=> c
                //                                                       .SelectMany(r => r.Matches)))
                //                    .Where(m => m.Home.ParticipantId.IsNotNull() ||
                //                                m.Away.ParticipantId.IsNotNull())
                //                    .ToList();
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
    }
}