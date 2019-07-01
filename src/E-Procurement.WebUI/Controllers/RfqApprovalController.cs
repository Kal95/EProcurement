using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.Dtos;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class RfqApprovalController : Controller
    {
        private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;

        public RfqApprovalController(IRfqApprovalRepository RfqApprovalRepository, IMapper mapper, IAccountManager accountManager)
        {
            _RfqApprovalRepository = RfqApprovalRepository;
            _accountManager = accountManager;
            _mapper = mapper;
          
        }
        public async Task<IActionResult> Index()
        {
            var RfqApprovalList = await _RfqApprovalRepository.GetRFQApprovalDueAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }

        public async Task<IActionResult> RfqApprovalDetails(int id)
        {
            var RfqApprovalDetails = await _RfqApprovalRepository.GetRFQDetailsAsync(id);

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
            if (RfqApprovalDetails == null)
                return View();

            return View(RfqApprovalDetails);
        }


        public async Task<IActionResult> PendingApproval()
        {
            var RfqApprovalList = await _RfqApprovalRepository.GetRFQPendingApprovalAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }


        [HttpPost]
        public async Task<IActionResult> RfqApprovalDetails(RFQGenerationModel rfqApproval)
        {

            try
            {
                if (string.IsNullOrEmpty(rfqApproval.RFQStatus))
                {
                    var approval = await _RfqApprovalRepository.CreateRFQApprovalAsync(rfqApproval);
                    return RedirectToAction("Index");
                }
                else
                {

                    var approval = await _RfqApprovalRepository.CreateRFQPendingApprovalAsync(rfqApproval);
                    return RedirectToAction("PendingApproval");
                }

            }
            catch(Exception ex)
            {
                return View();
            }

        }



        }
    }