using Abp.Web.Mvc.Alerts;
using AutoMapper;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.QuoteSendingRepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static E_Procurement.WebUI.Enums.Enums;
using E_Procurement.WebUI.Models.RequisitionModel;
using E_Procurement.Repository.Dtos;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class QuoteSendingController : BaseController
    {
        private readonly IQuoteSendingRepository _quoteSendingRepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private IHostingEnvironment _hostingEnv;

        public QuoteSendingController(IQuoteSendingRepository QuoteSendingRepository, IMapper mapper, IAccountManager accountManager, IHostingEnvironment hostingEnv)
        {
            _quoteSendingRepository = QuoteSendingRepository;
            _mapper = mapper;
            _accountManager = accountManager;
            _hostingEnv = hostingEnv;
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

        private void LoadFilePath(RequisitionModel Model)
        {

            if (Model.QuoteDocument != null)
            {
                var myReference = new Random();
                //string referencecode; 

                Model.RefCode = myReference.Next(23006).ToString();

                string webRootPath = _hostingEnv.WebRootPath;

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

                var checkextension1 = Path.GetExtension(Model.QuoteDocument.FileName).ToLower();
                if (!allowedExtensions.Contains(checkextension1))
                {
                    ModelState.AddModelError("", "Invalid file extention.");
                }
                var QuoteFilePath = Model.RefCode + "_" + "Quote" + Path.GetExtension(Model.QuoteDocument.FileName);
                var path1 = Path.Combine(webRootPath, "Uploads", "Quotes", QuoteFilePath);
                if (System.IO.File.Exists(path1)) { System.IO.File.Delete(path1); }
                using (Stream stream = new FileStream(path1, FileMode.Create)) { Model.QuoteDocument.CopyTo(stream); }
                Model.QuoteDocumentPath = QuoteFilePath;

            }
        }
        public IActionResult Update(int[] DetailsId, decimal[] quotedPrice, decimal[] quotedAmount, RFQGenerationModel Model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    string message;
                    
                    var quote = _quoteSendingRepository.GetRfqDetails().Where(u => u.Id == DetailsId[0]).FirstOrDefault();

                    if (quote == null) {
                        Alert("Invalid QUOTE details.", NotificationType.error);
                        return RedirectToAction("Index", "QuoteSending");
                    }

                    RequisitionModel Model2 = new RequisitionModel();
                    Model2.QuoteDocument = Model.QuoteDocument;

                    //Get Attach Files
                    LoadFilePath(Model2);

                    var status = _quoteSendingRepository.UpdateQuote(DetailsId, quotedPrice, quotedAmount, Model2, out message);
                    if(status)
                    {
                        Alert("Quote updated successfully.", NotificationType.success);
                        return RedirectToAction("Index", "QuoteSending");
                    }
                    else
                    {
                        Alert("Quote updated successfully.", NotificationType.error);
                        return View();
                    }             
                }
                else
                {                  
                        Alert("Error!! Please try again later.", NotificationType.error);
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
