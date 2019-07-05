using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.QuoteSendingRepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Controllers
{
    public class QuoteSendingController : Controller
    {
        private readonly IQuoteSendingRepository _quoteSendingRepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;

        public QuoteSendingController(IQuoteSendingRepository QuoteSendingRepository, IMapper mapper, IAccountManager accountManager)
        {
            _quoteSendingRepository = QuoteSendingRepository;
            _mapper = mapper;
            _accountManager = accountManager;
        }

        public async Task<IActionResult> Index()
        {
            var QuoteList = await _quoteSendingRepository.GetQuoteAsync();

            List<RFQGenerationViewModel> Quote = _mapper.Map<List<RFQGenerationViewModel>>(QuoteList);

            return View(Quote);
        }

        public async Task<IActionResult> QuoteDetails(int id)
        {
            var QuoteDetails = await _quoteSendingRepository.GetQuoteDetailsAsync(id);

            //List<RFQDetailsViewModel> rqfDetails = new List<RFQDetailsViewModel>();

            //foreach (var item in RfqApprovalDetails.RFQDetails)
            //{
            //    rqfDetails.Add(new RFQDetailsViewModel
            //    {
            //        RFQId = item.RFQId,
            //        VendorId = item.VendorId,
            //        ItemId = item.ItemId,
            //        ItemName = item.ItemName,
            //        QuotedQuantity = item.QuotedQuantity,
            //        AgreedQuantity = item.AgreedQuantity,
            //        QuotedAmount = item.QuotedAmount,
            //        AgreedAmount = item.AgreedAmount
            //    });
            //}

            //ViewBag.RFQDetails = rqfDetails;


            //RFQGenerationViewModel RfqApproval = _mapper.Map<RFQGenerationViewModel>(RfqApprovalDetails);

            //RfqApproval.RFQDetails = rqfDetails;

            return View(QuoteDetails);
        }
         
        public ActionResult Update(int[] DetailsId, decimal[] agreedAmount)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;

                    //foreach (var Id in RFQId)
                    //{
                        var quote = _quoteSendingRepository.GetRfqDetails().Where(u => u.Id == DetailsId[0]).FirstOrDefault();

                        if (quote == null) { return RedirectToAction("Index", "QuoteSending"); }

                        var status = _quoteSendingRepository.UpdateQuote(DetailsId, agreedAmount, out message);

                        //for (int i = 0; i < agreedAmount.Length; i++)
                        //{
                        //    var status = _quoteSendingRepository.UpdateQuote(Id, agreedAmount[], out message);

                        //    ViewBag.Message = TempData["MESSAGE"] as AlertMessage;

                        //    if (status == true)
                        //    {

                        //        ViewBag.Message = TempData["MESSAGE"] as AlertMessage;
                        //    }

                        //    else
                        //    {
                        //        ViewBag.Message = TempData["MESSAGE"] as AlertMessage;
                        //        return View();
                        //    }

                           
                        //}

                       


                    //}
                    return RedirectToAction("Index", "QuoteSending");


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
