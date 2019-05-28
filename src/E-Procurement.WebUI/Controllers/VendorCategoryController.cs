using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Repository.VendorCategoryRepo;
using E_Procurement.WebUI.Models.VendorCategoryModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.WebUI.Controllers
{
    public class VendorCategoryController : Controller
    {
        private readonly IVendorCategoryRepository _vendorCategoryRepository;
        private readonly IMapper _mapper;

        public VendorCategoryController(IVendorCategoryRepository bankRepository, IMapper mapper)
        {
            _vendorCategoryRepository = bankRepository;
            _mapper = mapper;
        }
        // GET: VendorCategory
        public ActionResult Index()
        {
            try
            {
                var model = _vendorCategoryRepository.GetVendorCategories().ToList();

                List<VendorCategoryModel> smodel = _mapper.Map<List<VendorCategoryModel>>(model);
                return View(smodel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: VendorCategory/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: VendorCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VendorCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VendorCategoryModel Model)
        {
            try
            {
                string message;
                string UserId = User.Identity.Name;

                if (ModelState.IsValid)
                {
                    var status = _vendorCategoryRepository.CreateVendorCategory(Model.CategoryName, UserId, out message);

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

                    return RedirectToAction("Index", "VendorCategory");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    return View(Model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        // GET: VendorCategory/Edit/5
        public ActionResult Edit(int CategoryId)
        {
            VendorCategoryModel Model = new VendorCategoryModel();

            try
            {

                var country = _vendorCategoryRepository.GetVendorCategories().Where(u => u.Id == CategoryId).FirstOrDefault();

                if (country == null)
                {
                    return RedirectToAction("Index", "VendorCategory");
                }

                Model.Id = country.Id;

                Model.CategoryName = country.CategoryName;

                Model.IsActive = country.IsActive;

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        // POST: VendorCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VendorCategoryModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    var category = _vendorCategoryRepository.GetVendorCategories().Where(u => u.Id == Model.Id).FirstOrDefault();

                    if (category == null) { return RedirectToAction("Index", "VendorCategory"); }

                    string UserId = User.Identity.Name;

                    var status = _vendorCategoryRepository.UpdateVendorCategory(Model.Id, Model.CategoryName, Model.IsActive, UserId, out message);

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

                    return RedirectToAction("Index", "VendorCategory");
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

        // GET: VendorCategory/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: VendorCategory/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}