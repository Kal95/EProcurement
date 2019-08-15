using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.PORepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class POController : BaseController
    {
        //private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IPORepository _PORepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IRfqApprovalRepository _RfqApprovalRepository;

        public POController(IPORepository PORepository, IMapper mapper, IAccountManager accountManager, IRfqApprovalRepository RfqApprovalRepository)
        {
            _PORepository = PORepository;
            _accountManager = accountManager;
            _mapper = mapper;
            _RfqApprovalRepository = RfqApprovalRepository;

        }
        public async Task<IActionResult> Index()
        {
            var RfqApprovalList = await _PORepository.GetPOAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }

        [Route("PO/RFQDetails/{id}/{vendorId}")]
        public async Task<IActionResult> RFQDetails(int id, int vendorId)
        {
            var RfqApprovalDetails = await _RfqApprovalRepository.GetRFQDetailsAsync(id, vendorId);

           
            //RfqApproval.RFQDetails = rqfDetails;
            if (RfqApprovalDetails == null)
                return View();

            return View(RfqApprovalDetails);
        }


        [System.Web.Mvc.HttpPost]
        public async Task<IActionResult> RFQDetails(RFQGenerationModel rfqApproval)
        {

            try
            {
                if (string.IsNullOrEmpty(rfqApproval.RFQStatus) || rfqApproval.RFQStatus != "Approved")
                {
                    Alert("Can not create PO for RFQ not approved.", NotificationType.info);
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
                {

                    Alert("Can not create PO for RFQ not approved.", NotificationType.info);
                    return RedirectToAction("Index");
                }


                Alert("Can not create PO. Please try again later.", NotificationType.error);
                return View(rfqApproval);
            }
            catch (Exception ex)
            {
                Alert("Error!!! Please try again later.", NotificationType.error);
                return View();
            }

        }


    }
}