using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.PoAcceptanceRepo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Controllers
{
    public class POAcceptanceController : Controller
    {
        private readonly IPOAcceptanceRepository _pOAcceptaceRepository;
        private readonly IMapper _mapper;

        public POAcceptanceController(IPOAcceptanceRepository pOAcceptaceRepository, IMapper mapper)
        {
            _pOAcceptaceRepository = pOAcceptaceRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var POList = await _pOAcceptaceRepository.GetAllPO();

            List<POAcceptanceViewModel> acceptanceList = _mapper.Map<List<POAcceptanceViewModel>>(POList);

            return View(acceptanceList);
        }

        public async Task<IActionResult> PODetails(int id)
        {
            var PODetails = await _pOAcceptaceRepository.GetPODetails(id);           

            return View(PODetails);
        }

        public ActionResult UpdatePO(int DetailsId, DateTime ExpectedDeliveryDate)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;
                    
                    //var quote = _pOAcceptaceRepository.GetRfqDetails().Where(u => u.Id == DetailsId[0]).FirstOrDefault();

                    //if (quote == null) { return RedirectToAction("Index", "QuoteSending"); }

                    var status = _pOAcceptaceRepository.UpdatePO(DetailsId, ExpectedDeliveryDate, out message);

                    return RedirectToAction("Index", "POAcceptance");


                    //string UserId = User.Identity.Name;


                }
                else
                {
                    ViewBag.StatusCode = 2;

                    ViewBag.Message = TempData["MESSAGE"] as AlertMessage;

                    return View();
                }
            }
            catch (Exception)
            {

                return View("Error");
            }
        }

    }
}
