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
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class RfqApprovalController : BaseController
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
            if (User.IsInRole("Procurement") && !User.IsInRole("Approval"))
            {
               RfqApprovalList = await _RfqApprovalRepository.GetRFQApprovalDueAsync();
            }
            else if (User.IsInRole("Approval"))
            {
                RfqApprovalList = await _RfqApprovalRepository.GetRFQPendingApprovalAsync();
            }
            var ven = _RfqApprovalRepository.GetVendors();
            
            var des = _RfqApprovalRepository.GetRFQDetails();

            var rfq = _RfqApprovalRepository.GetRFQ();
            

            RFQGenerationModel Model = new RFQGenerationModel();

            List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
            var ConfigList = RfqApprovalList.Select(x => new RFQDetailsModel
            {
                VendorId = x.VendorId,
                CreatedDate = x.CreatedDate,
                RFQId = x.RFQId,
                Reference = x.Reference,
                Description = x.Description,
                Item = x.Item,
                PONumber = x.PONumber,
                TotalAmount = x.QuotedAmount,
                ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                //VendorName = x.VendorName,
                //ContactName = x.ContactName,
                //VendorEmail = x.VendorEmail,
                //VendorStatus = x.VendorStatus,
                //PhoneNumber = x.PhoneNumber,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                RFQStatus = x.RFQStatus,
                POStatus = x.POStatus
            });
            poModel.AddRange(ConfigList);
            
            List<RFQDetailsModel> poModel2 = new List<RFQDetailsModel>();
           
            foreach (var ved in poModel.Select(u => u.RFQId).ToList())
            {
                var listModel = ( from rfqs in rfq
                                 join rfqDetails in des on rfqs.Id equals rfqDetails.RFQId
                                 //join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                                 join v in ven on rfqDetails.VendorId equals v.Id
                                 where rfqDetails.RFQId == ved && rfqs.EndDate <= DateTime.Now /*&& rfqs.RFQStatus == null*/
                                 
              select new RFQDetailsModel()
                {
                    RFQId = ved,//vendList.Select(u => u.RFQId).FirstOrDefault(),
                    VendorName = v.VendorName,//.Select(u => u.VendorName).FirstOrDefault(),
                    ContactName = v.ContactName,
                    VendorId = v.Id,//vendList.Where(u => RfqApprovalList.Any(a => a.RFQId == u.RFQId)).Select(u => u.VendorId).FirstOrDefault(),
                    QuotedPrice = rfqDetails.QuotedPrice,
                    QuotedAmount = rfqDetails.QuotedAmount,
                    TotalAmount = rfqDetails.QuotedAmount


              }).GroupBy(v => new { v.RFQId, v.VendorId }).Select(s => s.FirstOrDefault()).ToList();
                poModel2.AddRange(listModel);
            }
            Model.RFQDetails = poModel;
            Model.RFQDetails2 = poModel2;
            return View(Model);

            //return View(RfqApproval);
        }

        //[HttpGet("rfqApproval/RfqApprovalDetails/{id}/{vendorId}")]
        [HttpGet]
        public async Task<IActionResult> RfqApprovalDetails(int id, int VendorId)
        {
            var RfqApprovalDetails = await _RfqApprovalRepository.GetRFQDetailsAsync(id, VendorId);

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
            {
                Alert("Could not load approval details. Please, try again later.", NotificationType.error);
                return View();
            }
                

            return View(RfqApprovalDetails);
        }


        [HttpPost]
        public async Task<IActionResult> RfqApprovalDetails(RFQGenerationModel rfqApproval)
        {

            try
            {
                if (string.IsNullOrEmpty(rfqApproval.RFQStatus))
                {
                    var approval = await _RfqApprovalRepository.CreateRFQApprovalAsync(rfqApproval);
                    if (approval)
                    {

                        Alert("RFQ Approval sent.", NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    Alert("RFQ Approval Error.", NotificationType.error);
                    return View(rfqApproval);
                }
                else
                {

                    var approval = await _RfqApprovalRepository.CreateRFQPendingApprovalAsync(rfqApproval);
                    if (approval)
                    {

                        Alert("RFQ Approval sent.", NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    Alert("RFQ Approval Error.", NotificationType.error);
                    return View(rfqApproval);
                }

            }
            catch (Exception ex)
            {
                Alert("An error encountered. Please try again later.", NotificationType.error);
                return View(rfqApproval);
            }

        }

        public async Task<IActionResult> RfqVendorsDetails(int id)
        {
            var RfqApprovalDetails = await _RfqApprovalRepository.GetRFQByVendorsAsync(id);

           // List<RFQGenerationViewModel> rqfDetails = new List<RFQDetailsViewModel>();

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


            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalDetails);

            //RfqApproval.RFQDetails = rqfDetails;
            if (RfqApproval == null)
            {
                Alert("Could not load rfq details. Please, try again later.", NotificationType.error);
                return View();
            }


            return View(RfqApproval);
        }

        public async Task<IActionResult> RfqVendorsPendingApproval(int id)
        {
            var RfqApprovalDetails = await _RfqApprovalRepository.GetRFQPendingApprovalByVendorsAsync(id);

           


            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalDetails);

            //RfqApproval.RFQDetails = rqfDetails;
            if (RfqApproval == null)
            {
                Alert("Could not load rfq details. Please, try again later.", NotificationType.error);
                return View();
            }


            return View(RfqApproval);
        }

        public async Task<IActionResult> PendingApproval()
        {
            var RfqApprovalList = await _RfqApprovalRepository.GetRFQPendingApprovalAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }

    }
}