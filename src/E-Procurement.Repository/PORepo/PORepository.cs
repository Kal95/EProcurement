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

namespace E_Procurement.Repository.PORepo
{
    public class PORepository : IPORepository
    {
        private readonly EProcurementContext _context;
        private readonly IConvertViewToPDF _pdfConverter;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public PORepository(EProcurementContext context, UserManager<User> userManager, IConvertViewToPDF pdfConverter, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _pdfConverter = pdfConverter;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
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

            var Approver = 0;
            if (config2.Any(u => u.ApprovalLevel == 1)) { Approver = 1; }
            if (LevelInApproved.Contains(1) && config2.Any(u => u.ApprovalLevel == 2)) { Approver = 2; }
            if (LevelInApproved.Contains(2) && config2.Any(u => u.ApprovalLevel == 3)) { Approver = 3; }
            if (LevelInApproved.Contains(3) && config2.Any(u => u.ApprovalLevel == 4)) { Approver = 4; }
            if (LevelInApproved.Contains(4) && config2.Any(u => u.ApprovalLevel == 5)) { Approver = 5; }



            //if (config2.Any(u => u.ApprovalLevel == 1)) { config = _context.RfqApprovalConfigs.Where(u => u.ApprovalTypeId == 2).ToList(); }
           // else if (config2.Any(u => u.ApprovalLevel == 2) && LevelInApproved.Any(a => a != 1) ){ config = null; }
           // else if (config2.Any(u => u.ApprovalLevel == 3) && LevelInApproved.Any(a => a != 2)) { config = null; }
           // else if (config2.Any(u => u.ApprovalLevel == 4) && LevelInApproved.Any(a => a != 3)) { config = null; }
           // else if (config2.Any(u => u.ApprovalLevel == 5) && LevelInApproved.Any(a => a != 4)) { config = null; }

            var query = (from vend in _context.Vendors
                         join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                         join po in _context.PoGenerations on rfqDetails.VendorId equals po.VendorId
           
                         join rfq in _context.RfqGenerations on po.RFQId equals rfq.Id

                         where config.Any(u => u.UserId == int.Parse(currentUser) && u.ApprovalLevel == Approver) && !approved.Contains(int.Parse(currentUser)) && po.POStatus != "Approved"

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
            
                await _context.SaveChangesAsync();


                //re gen the detials
                var Item = await _context.RfqDetails.Where(x => x.RFQId == rfq.RFQId).ToListAsync();
                var totalAmount = Item.Sum(x => x.QuotedAmount);

                List<RFQDetailsModel> rFQDetails = new List<RFQDetailsModel>();
                
                var listModel = Item.Select(x => new RFQDetailsModel
                {
                    RFQId = x.RFQId,
                    VendorId = x.VendorId,
                    ItemId = x.ItemId,
                    ItemName = x.ItemName,
                    QuotedQuantity = x.QuotedQuantity,
                    AgreedQuantity = x.AgreedQuantity,
                    QuotedAmount = x.QuotedAmount,
                    AgreedAmount = x.AgreedAmount
                });

                rFQDetails.AddRange(listModel);
                

                rfq.RFQDetails = rFQDetails;

                //generate PDF and send mail
                await _pdfConverter.CreatePOPDF(rfq);
                return true;
            }

            return false;
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
