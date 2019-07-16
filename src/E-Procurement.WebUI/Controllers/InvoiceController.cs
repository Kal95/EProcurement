using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.DINRepo;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.PORepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class InvoiceController : Controller
    {
        //private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IPORepository _PORepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IDINRepository _dinRepository;

        public InvoiceController(IPORepository PORepository, 
                                IMapper mapper, 
                                IAccountManager accountManager, 
                                IRfqApprovalRepository RfqApprovalRepository,
                                IDINRepository dinRepository)
        {
            _PORepository = PORepository;
            _accountManager = accountManager;
            _mapper = mapper;
            _RfqApprovalRepository = RfqApprovalRepository;
            _dinRepository = dinRepository;

        }
        public async Task<IActionResult> Index()
        {
            var RfqApprovalList = await _dinRepository.GetPOAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }

        public async Task<IActionResult> InvoiceDetails(int id)
        {
            var RfqApprovalDetails = await _RfqApprovalRepository.GetRFQDetailsAsync(id);

           
            //RfqApproval.RFQDetails = rqfDetails;
            if (RfqApprovalDetails == null)
                return View();

            return View(RfqApprovalDetails);
        }


        [HttpPost]
        public async Task<IActionResult> InvoiceDetails(RFQGenerationModel rfqApproval)
        {

            try
            {
                if (string.IsNullOrEmpty(rfqApproval.RFQStatus) || rfqApproval.RFQStatus != "Approved")
                {
                    ModelState.AddModelError("", "Can not create PO for RFQ not approved.");
                    return View(rfqApproval);
                }
         
                //POGeneration poDetails = new POGeneration
                //{
                //    Amount = Convert.ToDecimal(rfqApproval.TotalAmmount),
                //    RFQId = rfqApproval.RFQId,
                //    VendorId = rfqApproval.VendorId,
                //    ExpectedDeliveryDate = rfqApproval.ExpectedDeliveryDate
                //};
                var generatePo = await _PORepository.GenerationPOAsync(rfqApproval);
                if(generatePo)
                    return RedirectToAction("Index");


                return View(rfqApproval);
            }
            catch (Exception ex)
            {
                return View();
            }

        }


    }
}