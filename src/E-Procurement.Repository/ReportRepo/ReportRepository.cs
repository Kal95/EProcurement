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
        public List<Vendor> GetVendors(RfqGenModel model)
        {
            var mapping = _context.VendorMappings.Where(u => u.VendorCategoryId == model.CategoryId).ToList();
            var vendor = _context.Vendors.OrderByDescending(u => u.Id).ToList();

            var vendorList = vendor.Where(a => mapping.Any(b => b.VendorID == a.Id));
            return vendorList.ToList();
        }
        public List<RFQGeneration> GetRfqGen()
        {

            return _context.RfqGenerations.OrderByDescending(u => u.Id).ToList();
        }

        public List<RFQGenerationModel> GetPoGen()
        {
            //List<RFQGenerationModel> RfqGen = new List<RFQGenerationModel>();
            var query = (from vend in _context.Vendors
                               join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                               join transaction in _context.RfqApprovalTransactions on rfqDetails.VendorId equals transaction.Id
                               join approvalStatus in _context.RfqApprovalStatuses on transaction.RFQId equals approvalStatus.RFQId
                               join config in _context.RfqApprovalConfigs on approvalStatus.CurrentApprovalLevel equals config.ApprovalLevel
                               join rfq in _context.RfqGenerations on approvalStatus.RFQId equals rfq.Id
                              // join po in _context.POGenerations on rfq.Id equals  po.RFQId
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
                               });
           



            return query.ToList();
            //return _context.PoGenerations.OrderByDescending(u => u.Id).ToList();
        }

    }
}
