using E_Procurement.Data;
using E_Procurement.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Repository.Dtos;
using System.Numerics;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using E_Procurement.Repository.ReportRepo;

namespace E_Procurement.Repository.PORepo
{
    public class PORepository : IPORepository
    {
        private readonly EProcurementContext _context;
        private readonly IConvertViewToPDF _pdfConverter;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private IConfiguration _config;
        private readonly IReportRepository _reportRepository;
        private readonly ISMTPService _emailSender;

        public PORepository(EProcurementContext context, IConfiguration config, UserManager<User> userManager, IConvertViewToPDF pdfConverter, IHttpContextAccessor contextAccessor, IReportRepository reportRepository, ISMTPService emailSender)
        {
            _context = context;
            _pdfConverter = pdfConverter;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _config = config;
            _reportRepository = reportRepository;
            _emailSender = emailSender;
        }
        public List<RFQApprovalConfig> GetPOApprovalConfig()
        {
            return _context.RfqApprovalConfigs.Where(u=> u.ApprovalTypeId == 2).OrderByDescending(u => u.Id).ToList();
        }
        public List<POApprovalTransactions>GetPOApprovals()
        {
            return _context.POApprovalTransactions.OrderByDescending(u => u.Id).ToList();
        }
        public List<RFQDetails> GetRFQDetails()
        {
            return _context.RfqDetails.OrderByDescending(u => u.Id).ToList();
        }
        public List<RFQGeneration> GetRFQs()
        {
            return _context.RfqGenerations.OrderByDescending(u => u.Id).ToList();
        }
        public IEnumerable<Vendor> GetVendors()
        {
            return _context.Vendors.Where(x => x.IsActive == true).OrderByDescending(u => u.Id).ToList();
        }
        public bool POApproval(RFQDetailsModel model, out string Message)
        {
            var confirm = _context.RfqGenerations.Where(x => x.Reference == model.Reference /*&& x.IsActive == model.IsActive*/).Count();
            var RFQEntry = _context.RfqGenerations.Where(x => x.Id == model.RFQId).FirstOrDefault();

            var oldEntry = _context.PoGenerations.Where(u => u.RFQId == model.RFQId).FirstOrDefault();
            var oldEntry2 = _context.POApprovalTransactions.Where(u => u.POId == model.POId && u.RFQId == model.RFQId && u.ApproverID != model.ApproverId).ToList();
            var Approver = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2).ToList();
            var Approver2 = Approver.Where(u => u.UserId == model.ApproverId);
           

            POGeneration generation = new POGeneration();
            POApprovalTransactions transactions = new POApprovalTransactions();
            if (oldEntry == null && oldEntry2.Count == 0)
            {
                //Initiate PO
              
                generation.RFQId = model.RFQId;
                generation.Reference = model.Reference;
                generation.Amount = model.QuotedAmount;
                generation.DateCreated = DateTime.Now;
                generation.CreatedBy = model.CreatedBy;
                generation.VendorId = model.VendorId;
                _context.Add(generation);
                _context.SaveChanges();

                Message = "";
                return true;
            }
            if (!oldEntry.Id.Equals(null)  && oldEntry2.Count() == 0)
            {
                if (Approver2.Any(u => u.IsFinalLevel) == false)
                {
                    transactions.ApprovalLevel = Approver2.Select(u => u.ApprovalLevel).SingleOrDefault();
                    transactions.ApprovalStatus = "Pending";
                    transactions.ApprovedBy = model.CreatedBy;
                    transactions.ApproverID = model.ApproverId;
                    transactions.Comments = model.Comments;
                    transactions.DateApproved = DateTime.Now;
                    transactions.POId = oldEntry.Id;
                    transactions.RFQId = model.RFQId;
                    transactions.VendorId = model.VendorId;
                    _context.Add(transactions);
                    _context.SaveChanges();

                }
                if (Approver2.Any(u => u.IsFinalLevel) == true)
                {
                    transactions.ApprovalLevel = Approver2.Select(u => u.ApprovalLevel).SingleOrDefault();
                    transactions.ApprovalStatus = "Approved";
                    transactions.ApprovedBy = model.CreatedBy;
                    transactions.ApproverID = model.ApproverId;
                    transactions.Comments = model.Comments;
                    transactions.DateApproved = DateTime.Now;
                    transactions.POId = oldEntry.Id;
                    transactions.RFQId = model.RFQId;
                    transactions.VendorId = model.VendorId;
                    _context.Add(transactions);

                    oldEntry.POStatus = "Approved";
                    oldEntry.UpdatedBy = model.UpdatedBy;
                    oldEntry.LastDateUpdated = DateTime.Now;

                    _context.SaveChanges();

                    //Send Email to Initiator
                    var user = _reportRepository.GetUser().Where(u => u.Email == RFQEntry.InitiatedBy).FirstOrDefault();
                    var message = "";
                    var subject = "PO NOTIFICATION";
                    message = "</br><b> Dear </b>" + user.FullName + "</br>";
                    message += "<br> Please be informed that Purchase Order for your request with Reference: " + RFQEntry.Reference + " has been Approved";
                    
                    message += "<br>Regards";

                    _emailSender.SendEmailAsync(user.Email, subject, message, "");

                }


                Message = "";
                return true;
            }
            
            else
            {
                Message = "";

                return false;
            }

        }

        public bool PODivert(RFQDetailsModel model, out string Message)
        {
            var confirm = _context.RfqGenerations.Where(x => x.Reference == model.Reference /*&& x.IsActive == model.IsActive*/).Count();
            var RFQEntry = _context.RfqGenerations.Where(x => x.Id == model.RFQId).FirstOrDefault();

            var oldEntry = _context.PoGenerations.Where(u => u.RFQId == model.RFQId).FirstOrDefault();
            var oldEntry2 = _context.POApprovalTransactions.Where(u => u.POId == model.POId && u.RFQId == model.RFQId && u.ApproverID != model.ApproverId).ToList();
            var Approver = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2).ToList();
            var Approver2 = Approver.Where(u => u.UserId == model.ApproverId);


            POGeneration generation = new POGeneration();
            POApprovalTransactions transactions = new POApprovalTransactions();
           
            if (!oldEntry.Id.Equals(null) && oldEntry2.Count() == 0)
            {
               
                if (Approver2.Any(u => u.IsFinalLevel) == true)
                {
                    transactions.ApprovalLevel = Approver2.Select(u => u.ApprovalLevel).SingleOrDefault();
                    transactions.ApprovalStatus = "Approved";
                    transactions.ApprovedBy = model.CreatedBy;
                    transactions.ApproverID = model.ApproverId;
                    transactions.Comments = model.Comments;
                    transactions.DateApproved = DateTime.Now;
                    transactions.POId = oldEntry.Id;
                    transactions.RFQId = model.RFQId;
                    transactions.VendorId = model.VendorId;
                    _context.Add(transactions);

                    oldEntry.POStatus = "Approved";
                    oldEntry.VendorId = model.VendorId;
                    oldEntry.UpdatedBy = model.UpdatedBy;
                    oldEntry.LastDateUpdated = DateTime.Now;

                    _context.SaveChanges();

                    //Send Email to Initiator
                    var user = _reportRepository.GetUser().Where(u => u.Email == RFQEntry.InitiatedBy).FirstOrDefault();
                    var message = "";
                    var subject = "PO NOTIFICATION";
                    message = "</br><b> Dear </b>" + user.FullName + "</br>";
                    message += "<br> Please be informed that Purchase Order for your request with Reference: " + RFQEntry.Reference + " has been Approved";

                    message += "<br>Regards";

                    _emailSender.SendEmailAsync(user.Email, subject, message, "");

                }


                Message = "";
                return true;
            }

            else
            {
                Message = "";

                return false;
            }

        }
        public IEnumerable<User> GetUser()
        {
            return _userManager.Users.OrderByDescending(u => u.Id).ToList();
        }

        public List<RFQGenerationModel> GetPoGen()
        {
            var ven = _context.Vendors.ToList();

            var des = _context.RfqDetails.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

            var QamountList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedAmount = x.QuotedAmount
            }).GroupBy(v => new { v.RfqId, v.QuotedAmount }).Select(s => s.FirstOrDefault());

            var PriceList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedPrice = x.QuotedPrice
            }).GroupBy(v => new { v.RfqId, v.QuotedPrice }).Select(s => s.FirstOrDefault());

            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());


            var query = (from vend in _context.Vendors
                              join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                              join transaction in _context.RfqApprovalTransactions on rfqDetails.VendorId equals transaction.VendorId
                              join approvalStatus in _context.RfqApprovalStatuses on transaction.RFQId equals approvalStatus.RFQId
                              join config in _context.RfqApprovalConfigs on approvalStatus.CurrentApprovalLevel equals config.ApprovalLevel
                              join rfq in _context.RfqGenerations on approvalStatus.RFQId equals rfq.Id
                              where approvalStatus.CurrentApprovalLevel == config.ApprovalLevel && config.IsFinalLevel == true
                              && !(from po in _context.PoGenerations select po.RFQId).Contains(rfq.Id)
                              orderby rfq.Id, rfq.EndDate descending
                              select new RFQGenerationModel()
                         {
                             QuotedAmount = QamountList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedAmount).FirstOrDefault(),
                             QuotedPrice = PriceList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedPrice).FirstOrDefault(),
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             QuotedQuantity = rfqDetails.QuotedQuantity,
                             
                             RFQStatus = rfq.RFQStatus,
                             VendorId = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorId).FirstOrDefault(),

                             VendorEmail = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorEmail).FirstOrDefault(),
                             PhoneNumber = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.PhoneNumber).FirstOrDefault(),
                             VendorAddress = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorAddress).FirstOrDefault(),
                             VendorStatus = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorStatus).FirstOrDefault(),
                             ContactName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.ContactName).FirstOrDefault(),
                             Item = string.Join(", ", desList.Where(u => u.RfqId == rfq.Id).Select(u => u.ItemName)),
                             Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                             VendorName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                             //VendorAddress = vend.VendorAddress,
                             //VendorStatus = vend.VendorStatus,
                             //ContactName = vend.ContactName
                         }).GroupBy(v => new { v.RFQId, v.Item }).Select(s => s.FirstOrDefault()).ToList();




            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }
        public List<RFQApprovalConfig> GetPOApprover()
        {
            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var approver = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2 && u.UserId == int.Parse(currentUser)).ToList();

            return approver;
        }
        public List<RFQGenerationModel> GetPoGen2()
        {
            var ven = _context.Vendors.ToList();

            var des = _context.RfqDetails.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

            var QamountList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedAmount = x.QuotedAmount
            }).GroupBy(v => new { v.RfqId, v.QuotedAmount }).Select(s => s.FirstOrDefault());

            var PriceList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedPrice = x.QuotedPrice
            }).GroupBy(v => new { v.RfqId, v.QuotedPrice }).Select(s => s.FirstOrDefault());

            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());

            var Con2 = _context.PoGenerations.Where(a => a.POStatus != "Approved").ToList();
            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var OthersApproved = _context.POApprovalTransactions.Where(u => Con2.Any(n => u.RFQId == n.RFQId /*&& u.ApproverID != int.Parse(currentUser)*/)).ToList();
            var CurrentApproved = _context.POApprovalTransactions.Where(u => Con2.Any(n => u.RFQId == n.RFQId && u.ApproverID == int.Parse(currentUser))).Select(u => u.RFQId).ToList();
            var OthersConfig = _context.RfqApprovalConfigs.Where(u => OthersApproved.Any(n => u.ApprovalTypeId == 2 && u.UserId == n.ApproverID)).ToList();
            //var CurrentConfig = _context.RfqApprovalConfigs.Where(u => CurrentApproved.Any(n => u.ApprovalTypeId == 2 && u.UserId == n.ApproverID)).ToList();
            var CurrentConfig = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2 && u.UserId == int.Parse(currentUser)).FirstOrDefault();//.ToList();

            var LevelInApproved = OthersApproved.Where(a => OthersConfig.Any(b => b.ApprovalLevel == a.ApprovalLevel && b.ApprovalTypeId == 2)).Select(u => u.ApprovalLevel).ToList();//_context.POApprovalTransactions.Where(u => con.Any(k => /*u.ApproverID == userId &&*/ k.RFQId == u.RFQId)).Select(u => u.ApprovalLevel).ToList();
            var LevelInApproved2 = OthersApproved.Where(a => OthersConfig.Any(b => b.ApprovalLevel == a.ApprovalLevel && b.ApprovalTypeId == 2)).ToList();

            List<SelectListItem> Approver = new List<SelectListItem>();
            //if (CurrentConfig == null)
            //{
            //    CurrentConfig = OthersConfig.Where(a => a.UserId == int.Parse(currentUser)).FirstOrDefault();
            //}
                if (CurrentConfig.ApprovalLevel == 1)
                {
                    foreach (var c in Con2)
                    {
                        if (CurrentConfig.ApprovalLevel == 1) { Approver.Add(new SelectListItem() { Text = c.RFQId.ToString(), Value = "1" }); }
                    }
                }
                else
                {
                    //List<int> Approver = new List<int>();
                    foreach (var level in LevelInApproved2)
                    {
                        //if (CurrentConfig.ApprovalLevel == 1) { Approver.Add(new SelectListItem() { Text = level.RFQId.ToString(), Value = "1" }); }
                        if (level.ApprovalLevel == 1 && CurrentConfig.ApprovalLevel == 2 && level.ApprovalLevel == CurrentConfig.ApprovalLevel - 1) { Approver.Add(new SelectListItem() { Text = level.RFQId.ToString(), Value = "2" });/*Approver.Add(2);*/ }
                        if (level.ApprovalLevel == 2 && CurrentConfig.ApprovalLevel == 3 && level.ApprovalLevel == CurrentConfig.ApprovalLevel - 1) { Approver.Add(new SelectListItem() { Text = level.RFQId.ToString(), Value = "3" }); }
                        if (level.ApprovalLevel == 3 && CurrentConfig.ApprovalLevel == 4 && level.ApprovalLevel == CurrentConfig.ApprovalLevel - 1) { Approver.Add(new SelectListItem() { Text = level.RFQId.ToString(), Value = "4" }); }
                        if (level.ApprovalLevel == 4 && CurrentConfig.ApprovalLevel == 5 && level.ApprovalLevel == CurrentConfig.ApprovalLevel - 1) { Approver.Add(new SelectListItem() { Text = level.RFQId.ToString(), Value = "5" }); }
                    }
                }
            
            
                var query = (from vend in _context.Vendors
                             join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                             join po in _context.PoGenerations on rfqDetails.VendorId equals po.VendorId
                             //join app in _context.POApprovalTransactions on po.RFQId equals app.RFQId
                             join rfq in _context.RfqGenerations on po.RFQId equals rfq.Id

                             where Approver.Any(a => Convert.ToInt32(a.Value) == CurrentConfig.ApprovalLevel && Convert.ToInt32(a.Text) == rfq.Id && CurrentConfig.UserId == int.Parse(currentUser)) && !CurrentApproved.Contains(rfq.Id) && po.POStatus != "Approved"

                             orderby rfq.Id, rfq.EndDate descending
                             select new RFQGenerationModel()
                             {
                                 QuotedAmount = QamountList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedAmount).FirstOrDefault(),
                                 QuotedPrice = PriceList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedPrice).FirstOrDefault(),
                                 RFQId = rfq.Id,
                                 ProjectId = rfq.ProjectId,
                                 RequisitionId = rfq.RequisitionId,
                                 Reference = rfq.Reference,
                                 StartDate = rfq.StartDate,
                                 EndDate = rfq.EndDate,
                                 QuotedQuantity = rfqDetails.QuotedQuantity,

                                 RFQStatus = rfq.RFQStatus,
                                 VendorId = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorId).FirstOrDefault(),

                                 VendorEmail = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorEmail).FirstOrDefault(),
                                 PhoneNumber = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.PhoneNumber).FirstOrDefault(),
                                 VendorAddress = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorAddress).FirstOrDefault(),
                                 VendorStatus = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorStatus).FirstOrDefault(),
                                 ContactName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.ContactName).FirstOrDefault(),
                                 Item = string.Join(", ", desList.Where(u => u.RfqId == rfq.Id).Select(u => u.ItemName)),
                                 Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                                 VendorName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                                 //VendorAddress = vend.VendorAddress,
                                 //VendorStatus = vend.VendorStatus,
                                 //ContactName = vend.ContactName
                             }).GroupBy(v => new { v.RFQId, v.Item }).Select(s => s.FirstOrDefault()).ToList();


                return query.ToList();

            
        }
        public List<RFQGenerationModel> GetRFQUpdate()
        {
            var ven = _context.Vendors.ToList();

            var des = _context.RfqDetails.ToList();

            var InitiatedVendor = _context.RfqApprovalTransactions.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

            var QamountList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedAmount = x.QuotedAmount
            }).GroupBy(v => new { v.RfqId, v.QuotedAmount }).Select(s => s.FirstOrDefault());

            var PriceList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedPrice = x.QuotedPrice
            }).GroupBy(v => new { v.RfqId, v.QuotedPrice }).Select(s => s.FirstOrDefault());

            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());

            //List<RFQGenerationModel> RfqGen = new List<RFQGenerationModel>();
            //var query = (from po in _context.PoGenerations
            //             join rfqDetails in _context.RfqDetails on po.RFQId equals rfqDetails.RFQId

            //             join rfq in _context.RfqGenerations on rfqDetails.RFQId equals rfq.Id
            //             // join po in _context.POGenerations on rfq.Id equals  po.RFQId
            //             //where approvalStatus.CurrentApprovalLevel == config.ApprovalLevel && config.IsFinalLevel == true
            //             //&& !(from po in _context.PoGenerations select po.RFQId).Contains(rfq.Id)
            //             orderby po.Id descending
            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var User = _context.Users.Where(a => a.Id == int.Parse(currentUser)).FirstOrDefault();
            //User.Email = null;

            var approved = _context.POApprovalTransactions.Select(u => u.ApproverID).ToList();
            var config = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2).ToList();
            var config2 = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2 && u.UserId == int.Parse(currentUser)).ToList();
            var ApprovalLevel = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2).Select(u => u.ApprovalLevel).ToList();
            var LevelInApproved = _context.POApprovalTransactions.Select(u => u.ApprovalLevel).ToList();


            var query = (from vend in _context.Vendors
                         join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                         //join po in _context.PoGenerations on rfqDetails.VendorId equals po.VendorId

                         join rfq in _context.RfqGenerations on rfqDetails.RFQId equals rfq.Id

                         where rfq.InitiatedBy == User.Email//(from po in _context.PoGenerations select po.RFQId).Contains(rfq.Id)

                         //&& !(from ap in approved select ap.RFQId).Contains(rfq.Id) && config.ApprovalTypeId == 2
                         orderby rfq.Id, rfq.EndDate descending
                         select new RFQGenerationModel()
                         {
                             QuotedAmount = QamountList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedAmount).FirstOrDefault(),
                             QuotedPrice = PriceList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedPrice).FirstOrDefault(),
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             QuotedQuantity = rfqDetails.QuotedQuantity,

                             RFQStatus = rfq.RFQStatus,
                             VendorId = InitiatedVendor.Where(u => u.RFQId == rfq.Id).Select(u => u.VendorId).FirstOrDefault(),
                             //VendorId = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorId).FirstOrDefault(),

                             //VendorEmail = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorEmail).FirstOrDefault(),
                             //PhoneNumber = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.PhoneNumber).FirstOrDefault(),
                             //VendorAddress = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorAddress).FirstOrDefault(),
                             //VendorStatus = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorStatus).FirstOrDefault(),
                             //ContactName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.ContactName).FirstOrDefault(),
                             Item = string.Join(", ", desList.Where(u => u.RfqId == rfqDetails.RFQId).Select(u => u.ItemName)),
                             Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                             //VendorName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                             //VendorAddress = vend.VendorAddress,
                             //VendorStatus = vend.VendorStatus,
                             //ContactName = vend.ContactName
                         }).GroupBy(v => new { v.RFQId, v.Item }).Select(s => s.FirstOrDefault()).ToList();




            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }

        public List<RFQGenerationModel> GetPOUpdate()
        {
            var ven = _context.Vendors.ToList();

            var des = _context.RfqDetails.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

            var QamountList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedAmount = x.QuotedAmount
            }).GroupBy(v => new { v.RfqId, v.QuotedAmount }).Select(s => s.FirstOrDefault());

            var PriceList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedPrice = x.QuotedPrice
            }).GroupBy(v => new { v.RfqId, v.QuotedPrice }).Select(s => s.FirstOrDefault());

            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());

            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var User = _context.Users.Where(a => a.Id == int.Parse(currentUser)).FirstOrDefault();


            var query = (from vend in _context.Vendors
                         join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                         join po in _context.PoGenerations on rfqDetails.VendorId equals po.VendorId

                         join rfq in _context.RfqGenerations on po.RFQId equals rfq.Id

                         where rfq.InitiatedBy == User.Email
                         //where po.POStatus == "Approved" || po.POStatus == "Generated"

                         //&& !(from ap in approved select ap.RFQId).Contains(rfq.Id) && config.ApprovalTypeId == 2
                         orderby rfq.Id, rfq.EndDate descending
                         select new RFQGenerationModel()
                         {
                             QuotedAmount = QamountList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedAmount).FirstOrDefault(),
                             QuotedPrice = PriceList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedPrice).FirstOrDefault(),
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             QuotedQuantity = rfqDetails.QuotedQuantity,
                             POStatus = po.POStatus,
                             RFQStatus = rfq.RFQStatus,
                             VendorId = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorId).FirstOrDefault(),

                             VendorEmail = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorEmail).FirstOrDefault(),
                             PhoneNumber = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.PhoneNumber).FirstOrDefault(),
                             VendorAddress = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorAddress).FirstOrDefault(),
                             VendorStatus = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorStatus).FirstOrDefault(),
                             ContactName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.ContactName).FirstOrDefault(),
                             Item = string.Join(", ", desList.Where(u => u.RfqId == rfqDetails.RFQId).Select(u => u.ItemName)),
                             Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                             VendorName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                             //VendorAddress = vend.VendorAddress,
                             //VendorStatus = vend.VendorStatus,
                             //ContactName = vend.ContactName
                         }).GroupBy(v => new { v.RFQId, v.VendorName }).Select(s => s.FirstOrDefault()).ToList();




            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }

        public List<RFQGenerationModel> GetApprovedRFQ()
        {
            var ven = _context.Vendors.ToList();

            var des = _context.RfqDetails.ToList();

            var InitiatedVendor = _context.RfqApprovalTransactions.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

            var QamountList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedAmount = x.QuotedAmount
            }).GroupBy(v => new { v.RfqId, v.QuotedAmount }).Select(s => s.FirstOrDefault());

            var PriceList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedPrice = x.QuotedPrice
            }).GroupBy(v => new { v.RfqId, v.QuotedPrice }).Select(s => s.FirstOrDefault());

            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());

            //List<RFQGenerationModel> RfqGen = new List<RFQGenerationModel>();
            //var query = (from po in _context.PoGenerations
            //             join rfqDetails in _context.RfqDetails on po.RFQId equals rfqDetails.RFQId

            //             join rfq in _context.RfqGenerations on rfqDetails.RFQId equals rfq.Id
            //             // join po in _context.POGenerations on rfq.Id equals  po.RFQId
            //             //where approvalStatus.CurrentApprovalLevel == config.ApprovalLevel && config.IsFinalLevel == true
            //             //&& !(from po in _context.PoGenerations select po.RFQId).Contains(rfq.Id)
            //             orderby po.Id descending
            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var approved = _context.POApprovalTransactions.Select(u => u.ApproverID).ToList();
            var config = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2).ToList();
            var config2 = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2 && u.UserId == int.Parse(currentUser)).ToList();
            var ApprovalLevel = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2).Select(u => u.ApprovalLevel).ToList();
            var LevelInApproved = _context.POApprovalTransactions.Select(u => u.ApprovalLevel).ToList();


            var query = (from vend in _context.Vendors
                         join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                         //join po in _context.PoGenerations on rfqDetails.VendorId equals po.VendorId

                         join rfq in _context.RfqGenerations on rfqDetails.RFQId equals rfq.Id

                         where rfq.RFQStatus == "Approved"//(from po in _context.PoGenerations select po.RFQId).Contains(rfq.Id)

                         //&& !(from ap in approved select ap.RFQId).Contains(rfq.Id) && config.ApprovalTypeId == 2
                         orderby rfq.Id, rfq.EndDate descending
                         select new RFQGenerationModel()
                         {
                             QuotedAmount = QamountList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedAmount).FirstOrDefault(),
                             QuotedPrice = PriceList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedPrice).FirstOrDefault(),
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             QuotedQuantity = rfqDetails.QuotedQuantity,

                             RFQStatus = rfq.RFQStatus,
                             VendorId = InitiatedVendor.Where(u => u.RFQId == rfq.Id).Select(u => u.VendorId).FirstOrDefault(),
                             //VendorId = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorId).FirstOrDefault(),

                             //VendorEmail = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorEmail).FirstOrDefault(),
                             //PhoneNumber = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.PhoneNumber).FirstOrDefault(),
                             //VendorAddress = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorAddress).FirstOrDefault(),
                             //VendorStatus = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorStatus).FirstOrDefault(),
                             //ContactName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.ContactName).FirstOrDefault(),
                             Item = string.Join(", ", desList.Where(u => u.RfqId == rfqDetails.RFQId).Select(u => u.ItemName)),
                             Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                             //VendorName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                             //VendorAddress = vend.VendorAddress,
                             //VendorStatus = vend.VendorStatus,
                             //ContactName = vend.ContactName
                         }).GroupBy(v => new { v.RFQId, v.Item}).Select(s => s.FirstOrDefault()).ToList();




            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }
        public List<RFQGenerationModel> GetApprovedPO()
        {
            var ven = _context.Vendors.ToList();

            var des = _context.RfqDetails.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

            var QamountList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedAmount = x.QuotedAmount
            }).GroupBy(v => new { v.RfqId, v.QuotedAmount }).Select(s => s.FirstOrDefault());

            var PriceList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedPrice = x.QuotedPrice
            }).GroupBy(v => new { v.RfqId, v.QuotedPrice }).Select(s => s.FirstOrDefault());

            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());

            var ApprovedPO = _context.PoGenerations.Where(a => a.POStatus == "Approved").ToList();
            var query = (from vend in _context.Vendors
                         join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                         join po in _context.PoGenerations on rfqDetails.VendorId equals po.VendorId

                         join rfq in _context.RfqGenerations on po.RFQId equals rfq.Id

                         where po.POStatus == "Approved" 

                         //&& !(from ap in approved select ap.RFQId).Contains(rfq.Id) && config.ApprovalTypeId == 2
                         orderby rfq.Id, rfq.EndDate descending
                         select new RFQGenerationModel()
                         {
                             QuotedAmount = QamountList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedAmount).FirstOrDefault(),
                             QuotedPrice = PriceList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedPrice).FirstOrDefault(),
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             QuotedQuantity = rfqDetails.QuotedQuantity,
                             POStatus = po.POStatus,
                             RFQStatus = rfq.RFQStatus,
                             VendorId = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorId).FirstOrDefault(),

                             VendorEmail = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorEmail).FirstOrDefault(),
                             PhoneNumber = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.PhoneNumber).FirstOrDefault(),
                             VendorAddress = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorAddress).FirstOrDefault(),
                             VendorStatus = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorStatus).FirstOrDefault(),
                             ContactName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.ContactName).FirstOrDefault(),
                             Item = string.Join(", ", desList.Where(u => u.RfqId == rfqDetails.RFQId).Select(u => u.ItemName)),
                             Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                             VendorName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                             //VendorAddress = vend.VendorAddress,
                             //VendorStatus = vend.VendorStatus,
                             //ContactName = vend.ContactName
                         }).GroupBy(v => new { v.RFQId, v.VendorName }).Select(s => s.FirstOrDefault()).ToList();




            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }
        public List<RFQGenerationModel> GetApprovedPO2()
        {
            var ven = _context.Vendors.ToList();

            var des = _context.RfqDetails.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

            var QamountList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedAmount = x.QuotedAmount
            }).GroupBy(v => new { v.RfqId, v.QuotedAmount }).Select(s => s.FirstOrDefault());

            var PriceList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                QuotedPrice = x.QuotedPrice
            }).GroupBy(v => new { v.RfqId, v.QuotedPrice }).Select(s => s.FirstOrDefault());

            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());


            var query = (from vend in _context.Vendors
                         join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                         join po in _context.PoGenerations on rfqDetails.VendorId equals po.VendorId

                         join rfq in _context.RfqGenerations on po.RFQId equals rfq.Id

                         where po.POStatus == "Approved" || po.POStatus == "Generated"

                         //&& !(from ap in approved select ap.RFQId).Contains(rfq.Id) && config.ApprovalTypeId == 2
                         orderby rfq.Id, rfq.EndDate descending
                         select new RFQGenerationModel()
                         {
                             QuotedAmount = QamountList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedAmount).FirstOrDefault(),
                             QuotedPrice = PriceList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.QuotedPrice).FirstOrDefault(),
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             QuotedQuantity = rfqDetails.QuotedQuantity,
                             POStatus = po.POStatus,
                             RFQStatus = rfq.RFQStatus,
                             VendorId = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorId).FirstOrDefault(),

                             VendorEmail = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorEmail).FirstOrDefault(),
                             PhoneNumber = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.PhoneNumber).FirstOrDefault(),
                             VendorAddress = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorAddress).FirstOrDefault(),
                             VendorStatus = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.VendorStatus).FirstOrDefault(),
                             ContactName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfqDetails.RFQId).Select(u => u.ContactName).FirstOrDefault(),
                             Item = string.Join(", ", desList.Where(u => u.RfqId == rfqDetails.RFQId).Select(u => u.ItemName)),
                             Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                             VendorName = vendList.Where(u => u.VendorId == rfqDetails.VendorId && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                             //VendorAddress = vend.VendorAddress,
                             //VendorStatus = vend.VendorStatus,
                             //ContactName = vend.ContactName
                         }).GroupBy(v => new { v.RFQId, v.VendorName }).Select(s => s.FirstOrDefault()).ToList();




            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }
        public async  Task<bool> GenerationPOAsync(RFQGenerationModel rfq)
        {
            var RFQ = _context.RfqGenerations.Where(a => a.Id == rfq.RFQId).FirstOrDefault();
            var oldEntry = _context.PoGenerations.Where(u => u.RFQId == rfq.RFQId).FirstOrDefault();
            if (rfq != null)
            {
                var myReference = new Random();

                oldEntry.PONumber = myReference.Next(23006).ToString();
                oldEntry.POStatus = "Generated";
                oldEntry.ExpectedDeliveryDate = rfq.ExpectedDeliveryDate;
                oldEntry.POCost = rfq.POCost;
                oldEntry.POPreamble = rfq.POPreamble;
                oldEntry.POTerms = rfq.POTerms;
                oldEntry.POTitle = rfq.POTitle;
                oldEntry.POValidity = rfq.POValidity;
                oldEntry.POWarranty = rfq.POWarranty;
                oldEntry.Reference = RFQ.Reference;

                await _context.SaveChangesAsync();

                //MAP TO RFQ
                var requisitionURL = _config.GetSection("ExternalAPI:RequisitionURL").Value;
                var signature = _context.Signatures.Where(a => a.IsActive == true).FirstOrDefault();

                rfq.Reference = RFQ.Reference;
                rfq.PONumber = oldEntry.PONumber;
                rfq.POTitle = rfq.POTitle.ToUpper();
                rfq.CreatedDate = DateTime.Now;
                //re gen the detials
                var Item = await _context.RfqDetails.Where(x => x.RFQId == rfq.RFQId && x.VendorId == rfq.VendorId).ToListAsync();
                var totalAmount = Item.Sum(x => x.QuotedAmount);
                rfq.TotalAmount = totalAmount;
                rfq.URL = requisitionURL;
                rfq.Signature1 = signature.Sign1;
                rfq.Signature2 = signature.Sign2;
                List<RFQDetailsModel> rFQDetails = new List<RFQDetailsModel>();

                var listModel = Item.Select(x => new RFQDetailsModel
                {
                    RFQId = x.RFQId,
                    VendorId = x.VendorId,
                    ItemId = x.ItemId,
                    ItemName = x.ItemName,
                    Description = x.ItemDescription,
                    QuotedQuantity = x.QuotedQuantity,
                    AgreedQuantity = x.AgreedQuantity,
                    QuotedAmount = x.QuotedAmount,
                    AgreedAmount = x.AgreedAmount
                });

                rFQDetails.AddRange(listModel);


                rfq.RFQDetails = rFQDetails;

                //generate PDF and send mail
                await _pdfConverter.CreatePOPDF(rfq);

                //Send Email to Initiator
                var user = _reportRepository.GetUser().Where(u => u.Email == RFQ.InitiatedBy).FirstOrDefault();
                var message = "";
                var subject = "PO NOTIFICATION";
                message = "</br><b> Dear </b>" + user.FullName + "</br>";
                message += "<br> Please be informed that Purchase Order for your request with Reference: " + RFQ.Reference + " has been Generated";

                message += "<br>Regards";

                await _emailSender.SendEmailAsync(user.Email, subject, message, "");

                return true;
            }
            else
            {
                return false;
            }
        }

      

        public async Task<IEnumerable<RFQGenerationModel>> GetPOAsync()
        {
            var query = await (from vend in _context.Vendors
                               join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                               join transaction in _context.RfqApprovalTransactions on rfqDetails.VendorId equals transaction.VendorId
                               join approvalStatus in _context.RfqApprovalStatuses on transaction.RFQId equals approvalStatus.RFQId
                               join config in _context.RfqApprovalConfigs on approvalStatus.CurrentApprovalLevel equals config.ApprovalLevel
                               join rfq in _context.RfqGenerations on approvalStatus.RFQId equals rfq.Id
                               where approvalStatus.CurrentApprovalLevel == config.ApprovalLevel && config.IsFinalLevel == true
                               && !(from po in _context.PoGenerations select po.RFQId).Contains(rfq.Id)
                               orderby rfq.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   RFQId = rfq.Id,
                                   ProjectId = rfq.ProjectId,
                                   RequisitionId = rfq.RequisitionId,
                                   Reference = rfq.Reference,
                                   Description = rfq.Description,
                                   StartDate = rfq.StartDate,
                                   EndDate = rfq.EndDate,
                                   RFQStatus = rfq.RFQStatus,
                                   VendorId = vend.Id,
                                   VendorName = vend.VendorName,
                                   VendorAddress = vend.VendorAddress,
                                   VendorStatus = vend.VendorStatus,
                                   ContactName = vend.ContactName
                               }).Distinct().ToListAsync();



            return query;
        }

        public async Task<POGeneration> GetPOByIdAsync(int Id)
        {
            return await _context.PoGenerations.FindAsync(Id);
        }

 

        public async  Task<POGeneration> GetPOByPONumberAsync(string PONumber)
        {
            return await _context.PoGenerations.Where(x => x.PONumber == PONumber).FirstOrDefaultAsync();
        }

        public BigInteger GuidToBigInteger(Guid guid)
        {
            BigInteger l_retval = 0;
            byte[] ba = guid.ToByteArray();
            int i = ba.Count();
            //for (int k = 0; k < 6; k++)
            //{
            //    l_retval += ba[k];
            //}
            foreach (byte b in ba)
            {
                l_retval += b * BigInteger.Pow(3, --i);
            }
            return l_retval;
        }
    }
}
