using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Procurement.Repository.BankRepo;
using E_Procurement.WebUI.Models.BankModel;
using Abp.Web.Mvc.Alerts;

namespace E_Procurement.WebUI.Controllers
{
    public class BankController : Controller
    {
        private readonly IBankRepository _bankRepository;
        private readonly IMapper _mapper;

        public BankController(IBankRepository bankRepository, IMapper mapper)
        {
            _bankRepository = bankRepository;
            _mapper = mapper;
        }

        // GET: Bank
        public ActionResult Index()
        {
            try
            {
                var model = _bankRepository.GetBanks().ToList();

                List<BankModel> smodel = _mapper.Map<List<BankModel>>(model);
                return View(smodel);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        // GET: Bank/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Bank/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bank/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BankModel Model)
        {
            try
            {
                string message;
                Model.CreatedBy = User.Identity.Name;

                if (ModelState.IsValid)
                {
                    var status = _bankRepository.CreateBank(Model, out message);

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

                    return RedirectToAction("Index", "Bank");
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

        // GET: Bank/Edit/5
        public ActionResult Edit(int BankId)
        {

            BankModel Model = new BankModel();

            try
            {

                var bank = _bankRepository.GetBanks().Where(u => u.Id == BankId).FirstOrDefault();

                if (bank == null)
                {
                    return RedirectToAction("Index", "Bank");
                }

                Model.Id = bank.Id;

                Model.BankName = bank.BankName;

                Model.SortCode = bank.SortCode;

                Model.IsActive = bank.IsActive;

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

        // POST: Bank/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BankModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    var bank = _bankRepository.GetBanks().Where(u => u.Id == Model.Id).FirstOrDefault();

                    if (bank == null) { return RedirectToAction("Index", "Bank"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _bankRepository.UpdateBank(Model, out message);

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

                    return RedirectToAction("Index", "Bank");
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

        // GET: Bank/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Bank/Delete/5
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