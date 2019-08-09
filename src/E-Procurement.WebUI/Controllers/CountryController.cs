using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Procurement.Repository.CountryRepo;
using AutoMapper;
using E_Procurement.WebUI.Models.CountryModel;
using Abp.Web.Mvc.Alerts;
using static E_Procurement.WebUI.Enums.Enums;
using Microsoft.AspNetCore.Authorization;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class CountryController : BaseController
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }


        // GET: Country
        public ActionResult Index()
        {
            try
            {
                var model = _countryRepository.GetCountries().ToList();

                List<CountryModel> smodel = _mapper.Map<List<CountryModel>>(model);
                return View(smodel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: Country/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Country/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Country/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CountryModel Model)
        {
            try
            {
                string message;
                Model.CreatedBy = User.Identity.Name;

                if (ModelState.IsValid)
                {
                    var status = _countryRepository.CreateCountry(Model, out message);
                    
                    if (status == true)
                    {

                        Alert("Country Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Country Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("Index", "Country");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Country Wasn't Created", NotificationType.error);

                    return View(Model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        // GET: Country/Edit/5
        public ActionResult Edit(int CountryId)
        {

           CountryModel Model = new CountryModel();

            try
            {

                var country = _countryRepository.GetCountries().Where(u => u.Id == CountryId).FirstOrDefault();

                if (country == null)
                {
                    Alert("This Country Doesn't Exist", NotificationType.warning);
                    return RedirectToAction("Index", "Country");
                }

                Model.Id = country.Id;

                Model.CountryName = country.CountryName;
                
                Model.IsActive = country.IsActive;

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        // POST: Country/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CountryModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    var country = _countryRepository.GetCountries().Where(u => u.Id == Model.Id).FirstOrDefault();

                    if (country == null) { Alert("This Country Doesn't Exist", NotificationType.warning); return RedirectToAction("Index", "Country"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _countryRepository.UpdateCountry(Model, out message);
                    

                    if (status == true)
                    {

                        Alert("Country Updated Successfully", NotificationType.success);
                    }

                    else
                    {
                        Alert("This Country Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("Index", "Country");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Country Wasn't Updated", NotificationType.error);

                    return View(Model);
                }
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        // GET: Country/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Country/Delete/5
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