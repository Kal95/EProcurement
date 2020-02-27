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

        public ActionResult ApprovedRFQ()
        {
            try
            {
                var config = _PORepository.GetApprovedRFQ().ToList();

                if (User.IsInRole("Initiator"))
                {
                    config = _PORepository.GetRFQUpdate().ToList();
                }

                var ven = _RfqApprovalRepository.GetVendors();

                var des = _RfqApprovalRepository.GetRFQDetails();

                var rfq = _RfqApprovalRepository.GetRFQ();

                var transaction = _RfqApprovalRepository.GetRFQTransactions();

                var user = _PORepository.GetUser();

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


                RFQGenerationModel Model = new RFQGenerationModel();

                List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
                var ConfigList = config.Select(x => new RFQDetailsModel
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

                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    RFQStatus = x.RFQStatus,
                    POStatus = x.POStatus
                });
                poModel.AddRange(ConfigList);

                List<RFQDetailsModel> poModel2 = new List<RFQDetailsModel>();

                foreach (var ved in poModel.Select(u => u.RFQId).ToList())
                {
                    var listModel = (from rfqs in rfq
                                     join rfqDetails in des on rfqs.Id equals rfqDetails.RFQId
                                     //join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                                     join v in ven on rfqDetails.VendorId equals v.Id
                                     where rfqDetails.RFQId == ved

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
                Model.RFQTransaction = transac;
                return View(Model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        public ActionResult ApprovedPO()
        {
            try
            {
                var config = _PORepository.GetApprovedPO2().ToList();

                if (User.IsInRole("Initiator"))
                {
                    config = _PORepository.GetPOUpdate().ToList();
                }

                var ven = _RfqApprovalRepository.GetVendors();

                var des = _RfqApprovalRepository.GetRFQDetails();

                var rfq = _RfqApprovalRepository.GetRFQ();

                var transaction = _RfqApprovalRepository.GetPOTransactions();

                var user = _PORepository.GetUser();

                List<RFQDetailsModel> transac = new List<RFQDetailsModel>();
                var tran = (from trans in transaction
                            join users in user on trans.ApprovedBy equals users.Email
                            select new RFQDetailsModel()
                            {
                                ApprovedBy = users.FullName,
                                Comments = trans.Comments,
                                RFQId = trans.RFQId,
                                VendorId = trans.VendorId,
                                POStatus = trans.ApprovalStatus
                                
                            });
                transac.AddRange(tran);

                RFQGenerationModel Model = new RFQGenerationModel();

                List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
                var ConfigList = config.Select(x => new RFQDetailsModel
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

                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    RFQStatus = x.RFQStatus,
                    POStatus = x.POStatus
                });
                poModel.AddRange(ConfigList);

                List<RFQDetailsModel> poModel2 = new List<RFQDetailsModel>();

                foreach (var ved in poModel.Select(u => u.RFQId).ToList())
                {
                    var listModel = (from rfqs in rfq
                                     join rfqDetails in des on rfqs.Id equals rfqDetails.RFQId
                                     //join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                                     join v in ven on rfqDetails.VendorId equals v.Id
                                     where rfqDetails.RFQId == ved

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
                Model.RFQTransaction = transac;
                return View(Model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public ActionResult POApproval()
        {
            try
            {
                
                var config= _PORepository.GetPoGen().ToList();
             
                if (User.IsInRole("Procurement") && !User.IsInRole("Approval"))
                {
                config = _PORepository.GetPoGen().ToList();
                }
                else if (User.IsInRole("Approval"))
                {
                    config = _PORepository.GetPoGen2();
                }
                //var RfqList = _rfqGenRepository.GetRfqGen().OrderBy(u => u.EndDate).ToList();
                var ven = _RfqApprovalRepository.GetVendors();

                var des = _RfqApprovalRepository.GetRFQDetails();

                var rfq = _RfqApprovalRepository.GetRFQ();

                var transaction = _RfqApprovalRepository.GetPOTransactions();

                var user = _PORepository.GetUser();

                var ApproverId = user.Where(a => a.UserName == User.Identity.Name).FirstOrDefault();

                var POApprover = _PORepository.GetPOApprovalConfig().Where(a => a.UserId == ApproverId.Id).Select(a => a.IsFinalLevel).FirstOrDefault();

                var POApproverName = ApproverId.FullName;

                List<RFQDetailsModel> transac = new List<RFQDetailsModel>();
                var tran = (from trans in transaction
                            join users in user on trans.ApprovedBy equals users.Email
                            select new RFQDetailsModel()
                            {
                                ApprovedBy = users.FullName,
                                Comments = trans.Comments,
                                RFQId = trans.RFQId
                            });
                transac.AddRange(tran);

                RFQGenerationModel Model = new RFQGenerationModel();

                List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
                var ConfigList = config.Select(x => new RFQDetailsModel
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
                    
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    RFQStatus = x.RFQStatus,
                    POStatus = x.POStatus
                });
                poModel.AddRange(ConfigList);

                List<RFQDetailsModel> poModel2 = new List<RFQDetailsModel>();

                foreach (var ved in poModel.Select(u => u.RFQId).ToList())
                {
                    var listModel = (from rfqs in rfq
                                     join rfqDetails in des on rfqs.Id equals rfqDetails.RFQId
                                     //join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                                     join v in ven on rfqDetails.VendorId equals v.Id
                                     where rfqDetails.RFQId == ved 

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
                Model.RFQTransaction = transac;
                Model.FinalApprover = POApprover;
                Model.ApproverName = POApproverName;
                return View(Model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        public ActionResult POApprovalDetails(RFQDetailsModel Model)
        {
            var RFQ = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == Model.VendorId).FirstOrDefault();
            var vendor = _PORepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
            var RFQin = _PORepository.GetRFQs().Where(u => u.Id == Model.RFQId).FirstOrDefault();
            //var PO = _reportRepository.GetPoGen().Where(u => u.RFQId == Model.RFQId).FirstOrDefault();

            RFQGenerationModel model2 = new RFQGenerationModel();
            model2.VendorId = vendor.Id;
            model2.ContactName = vendor.ContactName;
            model2.VendorName = vendor.VendorName;
            model2.VendorAddress = vendor.VendorAddress;
            model2.VendorEmail = vendor.Email;
            model2.Reference = RFQin.Reference;
            model2.RFQStatus = RFQin.RFQStatus;
            model2.StartDate = RFQin.StartDate;
            model2.EndDate = RFQin.EndDate;
            //model2.CreatedDate = PO.CreatedDate;


            List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
            var POList = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == vendor.Id).ToList();
            model2.TotalAmount = POList.Sum(x => x.QuotedAmount);
            var listModel = POList.Select(x => new RFQDetailsModel
            {
                RFQId = x.RFQId,
                VendorId = x.VendorId,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                Description = x.ItemDescription,
                QuotedQuantity = x.QuotedQuantity,
                AgreedQuantity = x.AgreedQuantity,
                QuotedAmount = x.QuotedAmount,
                AgreedAmount = x.AgreedAmount,
                QuotedPrice = x.QuotedPrice

            });
            poModel.AddRange(listModel);


            model2.RFQDetails = poModel;

            return View(model2);
        }

        public ActionResult PODivert(RFQDetailsModel Model)
        {
            var RFQ = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == Model.VendorId).FirstOrDefault();
            var vendor = _PORepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
            var RFQin = _PORepository.GetRFQs().Where(u => u.Id == Model.RFQId).FirstOrDefault();
            //var PO = _reportRepository.GetPoGen().Where(u => u.RFQId == Model.RFQId).FirstOrDefault();

            RFQGenerationModel model2 = new RFQGenerationModel();
            model2.VendorId = vendor.Id;
            model2.ContactName = vendor.ContactName;
            model2.VendorName = vendor.VendorName;
            model2.VendorAddress = vendor.VendorAddress;
            model2.VendorEmail = vendor.Email;
            model2.Reference = RFQin.Reference;
            model2.RFQStatus = RFQin.RFQStatus;
            model2.StartDate = RFQin.StartDate;
            model2.EndDate = RFQin.EndDate;
            //model2.CreatedDate = PO.CreatedDate;


            List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
            var POList = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == vendor.Id).ToList();
            model2.TotalAmount = POList.Sum(x => x.QuotedAmount);
            var listModel = POList.Select(x => new RFQDetailsModel
            {
                RFQId = x.RFQId,
                VendorId = x.VendorId,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                Description = x.ItemDescription,
                QuotedQuantity = x.QuotedQuantity,
                AgreedQuantity = x.AgreedQuantity,
                QuotedAmount = x.QuotedAmount,
                AgreedAmount = x.AgreedAmount,
                QuotedPrice = x.QuotedPrice

            });
            poModel.AddRange(listModel);


            model2.RFQDetails = poModel;

            return View(model2);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PODiverting(RFQDetailsModel Model)
        {
            try
            {
                string message;

                var RFQ = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == Model.VendorId).FirstOrDefault();
                //var vendor = _PORepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
                //var RFQin = _PORepository.GetRFQs().Where(u => u.Id == Model.RFQId).FirstOrDefault();

                var userId = _PORepository.GetUser().Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();

                Model.ApproverId = userId;
                Model.CreatedBy = User.Identity.Name;
                Model.RFQId = Model.RFQId;
                Model.QuotedAmount = RFQ.QuotedAmount;

                var status = _PORepository.PODivert(Model, out message);


                if (status == true)
                {

                    Alert("PO Approved Successfully", NotificationType.success);
                }

                else
                {
                    Alert("PO cannot be Approved", NotificationType.info);
                    return View(Model);
                }

                return RedirectToAction("POApproval", "PO");

            }
            catch (Exception)
            {

                return View("Error");
            }

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult POApproval(RFQDetailsModel Model)
        {
            try
            {
                string message;

               var RFQ = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == Model.VendorId).FirstOrDefault();
                //var vendor = _PORepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
                //var RFQin = _PORepository.GetRFQs().Where(u => u.Id == Model.RFQId).FirstOrDefault();

                var userId = _PORepository.GetUser().Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefault();
               
                Model.ApproverId = userId;
                Model.CreatedBy = User.Identity.Name;
                Model.RFQId = Model.RFQId;
                Model.QuotedAmount = RFQ.QuotedAmount;

                var status = _PORepository.POApproval(Model, out message);


                if (status == true)
                {

                    Alert("PO Approved Successfully", NotificationType.success);
                }

                else
                {
                    Alert("PO cannot be Approved", NotificationType.info);
                    return View(Model);
                }

                return RedirectToAction("POApproval", "PO");

            }
            catch (Exception)
            {

                return View("Error");
            }

        }

       

        public ActionResult GeneratePO()
        {
            try
            {
                var config = _PORepository.GetApprovedPO().ToList();

                var ven = _RfqApprovalRepository.GetVendors();

                var des = _RfqApprovalRepository.GetRFQDetails();

                var rfq = _RfqApprovalRepository.GetRFQ();

                var transaction = _RfqApprovalRepository.GetPOTransactions();

                var user = _PORepository.GetUser();


                List<RFQDetailsModel> transac = new List<RFQDetailsModel>();
                var tran = (from trans in transaction
                            join users in user on trans.ApprovedBy equals users.Email
                            select new RFQDetailsModel()
                            {
                                ApprovedBy = users.FullName,
                                Comments = trans.Comments,
                                RFQId = trans.RFQId
                            });
                transac.AddRange(tran);

                RFQGenerationModel Model = new RFQGenerationModel();

                List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
                var ConfigList = config.Select(x => new RFQDetailsModel
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

                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    RFQStatus = x.RFQStatus,
                    POStatus = x.POStatus
                });
                poModel.AddRange(ConfigList);

                List<RFQDetailsModel> poModel2 = new List<RFQDetailsModel>();

                foreach (var ved in poModel.Select(u => u.RFQId).ToList())
                {
                    var listModel = (from rfqs in rfq
                                     join rfqDetails in des on rfqs.Id equals rfqDetails.RFQId
                                     //join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                                     join v in ven on rfqDetails.VendorId equals v.Id
                                     where rfqDetails.RFQId == ved

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
                Model.RFQTransaction = transac;
                return View(Model);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }
        public ActionResult POGenDetails(RFQDetailsModel Model)
        {
            var RFQ = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == Model.VendorId).FirstOrDefault();
            var vendor = _PORepository.GetVendors().Where(u => u.Id == RFQ.VendorId).FirstOrDefault();
            var RFQin = _PORepository.GetRFQs().Where(u => u.Id == Model.RFQId).FirstOrDefault();
            var ApprovedPO = _PORepository.GetApprovedPO().Where(u => u.RFQId == Model.RFQId).FirstOrDefault();
            //var PO = _reportRepository.GetPoGen().Where(u => u.RFQId == Model.RFQId).FirstOrDefault();

            RFQGenerationModel model2 = new RFQGenerationModel();
            model2.VendorId = vendor.Id;
            model2.ContactName = vendor.ContactName;
            model2.VendorName = vendor.VendorName;
            model2.VendorAddress = vendor.VendorAddress;
            model2.VendorEmail = vendor.Email;
            model2.Reference = RFQin.Reference;
            model2.RFQStatus = RFQin.RFQStatus;
            model2.POStatus = ApprovedPO.POStatus;
            model2.StartDate = RFQin.StartDate;
            model2.EndDate = RFQin.EndDate;
            //model2.CreatedDate = PO.CreatedDate;


            List<RFQDetailsModel> poModel = new List<RFQDetailsModel>();
            var POList = _PORepository.GetRFQDetails().Where(u => u.RFQId == Model.RFQId && u.VendorId == vendor.Id).ToList();
            model2.TotalAmount = POList.Sum(x => x.QuotedAmount);
            var listModel = POList.Select(x => new RFQDetailsModel
            {
                RFQId = x.RFQId,
                VendorId = x.VendorId,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                Description = x.ItemDescription,
                QuotedQuantity = x.QuotedQuantity,
                AgreedQuantity = x.AgreedQuantity,
                QuotedAmount = x.QuotedAmount,
                AgreedAmount = x.AgreedAmount,
                QuotedPrice = x.QuotedPrice

            });
            poModel.AddRange(listModel);


            model2.RFQDetails = poModel;

            return View(model2);
        }

        [HttpPost]
        public async Task<IActionResult> POGenDetails(RFQGenerationModel rfqApproval)
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
                if (generatePo)
                {
                    Alert("PO Generated Successfully", NotificationType.success);
                    return RedirectToAction("GeneratePO", "PO");
                   
                }

                else
                {
                    Alert("PO cannot be Generated", NotificationType.info);
                    return View(rfqApproval);
                }
                
            }
            catch (Exception ex)
            {
                Alert("Error!!! Please try again later.", NotificationType.error);
                return View();
            }

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


        [HttpPost]
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