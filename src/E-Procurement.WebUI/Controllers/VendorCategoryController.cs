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
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    public class VendorCategoryController : BaseController
    {
        private readonly IVendorCategoryRepository _vendorCategoryRepository;
        private readonly IMapper _mapper;

        public VendorCategoryController(IVendorCategoryRepository categoryRepository, IMapper mapper)
        {
            _vendorCategoryRepository = categoryRepository;
            _mapper = mapper;
        }
        private void LoadPredefinedInfo(CategoryModel Model)
        {
            int CategoryId = Model.CategoryId;


            var ItemCategory = _vendorCategoryRepository.GetVendorCategories().ToList();
            
            Model.ItemCategoryList = ItemCategory.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.CategoryName
            });

        }
        // GET: VendorCategory
        public ActionResult CategoryIndex()
        {
            try
            {
                var model = _vendorCategoryRepository.GetVendorCategories().ToList();

                List<CategoryModel> smodel = _mapper.Map<List<CategoryModel>>(model);
                return View(smodel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: VendorCategory/Create
        public ActionResult CategoryCreate()
        {
            return View();
        }

        // POST: VendorCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryCreate(CategoryModel model)
        {
            try
            {
                string message;
                model.CreatedBy = User.Identity.Name;

                if (ModelState.IsValid)
                {

                    var status = _vendorCategoryRepository.CreateVendorCategory(model, out message);

                    if (status == true)
                    {

                        Alert("Category Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Category Already Exists", NotificationType.info);
                        return View(model);
                    }

                    return RedirectToAction("CategoryIndex", "VendorCategory");
                }
                else
                {
                    ViewBag.StatusCode = 2;
                    Alert("Category Wasn't Created", NotificationType.error);

                    return View(model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        // GET: State/Edit/5
        public ActionResult CategoryEdit(int CategoryId)
        {


            CategoryModel Model = new CategoryModel();

            try
            {

                var Category = _vendorCategoryRepository.GetVendorCategories().Where(u => u.Id == CategoryId).FirstOrDefault();

                if (Category == null)
                {
                    Alert("This Category Doesn't Exist", NotificationType.warning);

                    return RedirectToAction("CategoryIndex", "VendorCategory");
                }

                Model.Id = Category.Id;

                Model.CategoryName = Category.CategoryName;

                Model.IsActive = Category.IsActive;

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }

        }

        // POST: State/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CategoryEdit(CategoryModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    var state = _vendorCategoryRepository.GetVendorCategories().FirstOrDefault(u => u.Id == Model.Id);

                    if (state == null) { Alert("This Category Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "VendorCategory"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _vendorCategoryRepository.UpdateVendorCategory(Model, out message);


                    if (status == true)
                    {
                        Alert("Category Updated Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Category Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("CategoryIndex", "VendorCategory");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Category Wasn't Updated", NotificationType.error);

                    return View(Model);
                }
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        public ActionResult ItemIndex()
        {
            try
            {
                var model = _vendorCategoryRepository.GetItems_Categories().ToList();

                List<CategoryModel> smodel = _mapper.Map<List<CategoryModel>>(model);
                return View(smodel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: VendorCategory/Create
        public ActionResult ItemCreate()
        {
            try
            {
                CategoryModel Model = new CategoryModel();

                LoadPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View();
            }
        }

        // POST: VendorCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemCreate(CategoryModel model)
        {
            try
            {
                string message;
                model.CreatedBy = User.Identity.Name;

                if (ModelState.IsValid)
                {

                    var status = _vendorCategoryRepository.CreateItem(model, out message);

                    if (status == true)
                    {

                        Alert("Item Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Item Already Exists", NotificationType.info);
                        return View(model);
                    }

                    return RedirectToAction("ItemIndex", "VendorCategory");
                }
                else
                {
                    LoadPredefinedInfo(model);
                    ViewBag.StatusCode = 2;
                    Alert("Item Wasn't Created", NotificationType.error);

                    return View(model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        // GET: State/Edit/5
        public ActionResult ItemEdit(int ItemId)
        {


            CategoryModel Model = new CategoryModel();

            try
            {

                var Item = _vendorCategoryRepository.GetItems().Where(u => u.Id == ItemId).FirstOrDefault();

                if (Item == null)
                {
                    Alert("This Item Doesn't Exist", NotificationType.warning);

                    return RedirectToAction("ItemIndex", "VendorCategory");
                }

                Model.Id = Item.Id;

                Model.CategoryId = Item.ItemCategoryId;

                Model.ItemName = Item.ItemName;

                Model.IsActive = Item.IsActive;

                LoadPredefinedInfo(Model);
                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }

        }

        // POST: State/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ItemEdit(CategoryModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    var state = _vendorCategoryRepository.GetItems().FirstOrDefault(u => u.Id == Model.Id);

                    if (state == null) { Alert("This Item Doesn't Exist", NotificationType.warning); return RedirectToAction("ItemIndex", "VendorCategory"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _vendorCategoryRepository.UpdateItem(Model, out message);


                    if (status == true)
                    {
                        Alert("Item Updated Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Item Already Exists", NotificationType.info);
                        LoadPredefinedInfo(Model);
                        return View(Model);
                    }

                    return RedirectToAction("ItemIndex", "VendorCategory");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Item Wasn't Updated", NotificationType.error);

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