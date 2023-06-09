﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.PORepo;
using E_Procurement.Repository.RequisitionRepo;
using E_Procurement.WebUI.Models.RequisitionModel;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IPORepository _PORepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IRequisitionRepository _requisitionRepository;
        private IHostingEnvironment _hostingEnv;
        private readonly IHttpContextAccessor _httpContextAccessor;
        

        public RfqApprovalController(IRfqApprovalRepository RfqApprovalRepository, IRequisitionRepository 
            requisitionRepository, IPORepository PORepository, IMapper mapper, IAccountManager accountManager, 
            IHostingEnvironment hostingEnv, IHttpContextAccessor httpContextAccessor)
        {
            _RfqApprovalRepository = RfqApprovalRepository;
            _PORepository = PORepository;
            _accountManager = accountManager;
            _mapper = mapper;
            _requisitionRepository = requisitionRepository;
            _hostingEnv = hostingEnv;
            _httpContextAccessor = httpContextAccessor;

        }
        
        public async Task<IActionResult> Index()
        {
            var REQconfig = _requisitionRepository.GetRequisitions2().ToList();

            //var RfqApprovalList = await _RfqApprovalRepository.GetRFQApprovalDueAsync();
            IEnumerable<RFQGenerationModel> RfqApprovalList = new List<RFQGenerationModel>();
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

            var transaction = _RfqApprovalRepository.GetRFQTransactions();

            var user = _PORepository.GetUser();

            RFQGenerationModel Model = new RFQGenerationModel();

            List<RFQDetailsModel> transac = new List<RFQDetailsModel>();
            var tran = (from trans in transaction
                        join users in user on trans.CreatedBy equals users.Email
                        select new RFQDetailsModel()
                        {
                            ApprovedBy = users.FullName,
                            Comments = trans.Comments,
                            RFQId = trans.RFQId
                        });
            transac.AddRange(tran);

            List <RFQDetailsModel> poModel = new List<RFQDetailsModel>();
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
                RequisitionDocumentPath = REQconfig.Where(a => a.Id == x.RequisitionId).Select(b => b.RequisitionDocument).FirstOrDefault(),
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
                                 where rfqDetails.RFQId == ved && rfqs.EndDate >= DateTime.Now /*&& rfqs.RFQStatus == null*/
                                 
              select new RFQDetailsModel()
                {
                    RFQId = ved,//vendList.Select(u => u.RFQId).FirstOrDefault(),
                    VendorName = v.VendorName,//.Select(u => u.VendorName).FirstOrDefault(),
                    ContactName = v.ContactName,
                    VendorId = v.Id,//vendList.Where(u => RfqApprovalList.Any(a => a.RFQId == u.RFQId)).Select(u => u.VendorId).FirstOrDefault(),
                    QuotedPrice = rfqDetails.QuotedPrice,
                    QuotedAmount = rfqDetails.QuotedAmount,
                    TotalAmount = rfqDetails.QuotedAmount,
                    QuoteDocumentPath = rfqDetails.QuoteDocument

              }).GroupBy(v => new { v.RFQId, v.VendorId }).Select(s => s.FirstOrDefault()).ToList();
                poModel2.AddRange(listModel);
            }
            Model.RFQDetails = poModel.OrderByDescending(a => a.RFQId).ToList();
            Model.RFQDetails2 = poModel2;
            Model.RFQTransaction = transac;
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


        private string LoadFilePath(RFQGenerationModel Model)
        {

            if (Model.ComparisonDocument != null)
            {
                var myReference = new Random();
                //string referencecode; 

                Model.RefCode = myReference.Next(23006).ToString();

                string webRootPath = _hostingEnv.WebRootPath;

                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

                var checkextension1 = Path.GetExtension(Model.ComparisonDocument.FileName).ToLower();
                if (!allowedExtensions.Contains(checkextension1))
                {
                    ModelState.AddModelError("", "Invalid file extention.");
                    return null;
                }
                var ComparisonFilePath = Model.RefCode + "_" + "Quote" + Path.GetExtension(Model.ComparisonDocument.FileName);
                var path1 = Path.Combine(webRootPath, "Uploads", "ComparisonDocuments", ComparisonFilePath);
                if (System.IO.File.Exists(path1)) { System.IO.File.Delete(path1); }
                using (Stream stream = new FileStream(path1, FileMode.Create)) { Model.ComparisonDocument.CopyTo(stream); }

                string url = string.Concat(_httpContextAccessor.HttpContext.Request.Scheme, "://", _httpContextAccessor.HttpContext.Request.Host, $"/Uploads/ComparisonDocuments/{ComparisonFilePath}");

                Model.ComparisonDocumentPath = ComparisonFilePath;
                return url;
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> RfqApprovalDetails([FromForm]RFQGenerationModel rfqApproval)
        {

            try
            {
                if (string.IsNullOrEmpty(rfqApproval.RFQStatus))
                {
                    var res = LoadFilePath(rfqApproval);
                    rfqApproval.ComparisonDocumentPath = res;
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