using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Http;

namespace E_Procurement.Repository.ReportRepo
{
    public class ReportRepository : IReportRepository
    {
        private readonly EProcurementContext _context;
        private readonly ISMTPService _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;
        private IConvertViewToPDF _pdfConverter;
        public ReportRepository(EProcurementContext context, ISMTPService emailSender, IHttpContextAccessor contextAccessor, IConvertViewToPDF pdfConverter)
        {
            _context = context;
            _emailSender = emailSender;
            _contextAccessor = contextAccessor;
            _pdfConverter = pdfConverter;
        }
        public IEnumerable<Vendor> GetVendors()
        {
            return _context.Vendors.OrderByDescending(u => u.Id).ToList();
        }
        public IEnumerable<VendorMapping> GetMapping()
        {
            return _context.VendorMappings.OrderByDescending(u => u.Id).ToList();
        }
        //public List<Vendor> GetVendors(RfqGenModel model)
        //{
        //    var mapping = _context.VendorMappings.Where(u => u.VendorCategoryId == model.CategoryId).ToList();
        //    var vendor = _context.Vendors.OrderByDescending(u => u.Id).ToList();

        //    var vendorList = vendor.Where(a => mapping.Any(b => b.VendorID == a.Id));
        //    return vendorList.ToList();
        //}
        public List<RFQGenerationModel> GetRfqGen()
        {
            var query = (from rfqDetails in _context.RfqDetails 
                         join rfq in _context.RfqGenerations on rfqDetails.RFQId equals rfq.Id
                         select new RFQGenerationModel()
                         {
                             QuotedAmount = rfqDetails.QuotedAmount,
                             QuotedQuantity = rfqDetails.QuotedQuantity,
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             Description = rfqDetails.ItemDescription,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             CreatedDate = rfq.DateCreated,
                             RFQStatus = rfq.RFQStatus,
                             VendorId = rfqDetails.VendorId,
                             IsActive = rfqDetails.IsActive
                         });

            return query.ToList();//_context.RfqGenerations.OrderByDescending(u => u.Id).ToList();
        }
        public List<RFQDetails> GetRFQDetails()
        {
           return _context.RfqDetails.OrderByDescending(u => u.Id).ToList();
        }

        public List<RFQGenerationModel> GetPoGen()
        {
            //List<RFQGenerationModel> RfqGen = new List<RFQGenerationModel>();
            var query = (from po in _context.PoGenerations
                               join vend in _context.Vendors on po.VendorId equals vend.Id
                               join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                               join transaction in _context.RfqApprovalTransactions on rfqDetails.VendorId equals transaction.Id
                               join approvalStatus in _context.RfqApprovalStatuses on transaction.RFQId equals approvalStatus.RFQId
                               join config in _context.RfqApprovalConfigs on approvalStatus.CurrentApprovalLevel equals config.ApprovalLevel
                               join rfq in _context.RfqGenerations on approvalStatus.RFQId equals rfq.Id
                              // join po in _context.POGenerations on rfq.Id equals  po.RFQId
                               //where approvalStatus.CurrentApprovalLevel == config.ApprovalLevel && config.IsFinalLevel == true
                               //&& !(from po in _context.PoGenerations select po.RFQId).Contains(rfq.Id)
                               //orderby rfq.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   PONumber = po.PONumber,
                                   ExpectedDeliveryDate = po.ExpectedDeliveryDate,
                                   QuotedAmount = rfqDetails.QuotedAmount,
                                   PoId = po.Id,
                                   RFQId = rfq.Id,
                                   ProjectId = rfq.ProjectId,
                                   RequisitionId = rfq.RequisitionId,
                                   Reference = rfq.Reference,
                                   Description = rfq.Description,
                                   StartDate = rfq.StartDate,
                                   EndDate = rfq.EndDate,
                                   CreatedDate = po.DateCreated,
                                   RFQStatus = rfq.RFQStatus,
                                   VendorId = vend.Id,
                                   VendorName = vend.VendorName,
                                   VendorEmail = vend.Email,
                                   PhoneNumber = vend.PhoneNumber,
                                   VendorAddress = vend.VendorAddress,
                                   VendorStatus = vend.VendorStatus,
                                   ContactName = vend.ContactName
                               });
           



            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }

    }
}
