using E_Procurement.Data;
using E_Procurement.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.Dtos;

namespace E_Procurement.Repository.RfqApprovalConfigRepository
{
   public class RfqApprovalRepository : IRfqApprovalRepository
    {
        private readonly EProcurementContext _context;

        public RfqApprovalRepository(EProcurementContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<RFQGenerationModel>> GetRFQApprovalDueAsync()
        {
            //var query1 = _context.RfqGenerations
            //        .Join(
            //            _context.RfqDetails,
            //            rfq => rfq.Id,
            //            rfqDetails => rfqDetails.RFQId,
            //            (rfq, rfqDetails) => new
            //            {
            //                rfq,
            //                rfqDetails
            //            }
            //            )
            //            .Join(_context.Vendors,
            //                rfqDetails => rfq.rfqDetails.).ToList();

            var query = await (from rfq in _context.RfqGenerations
                         join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                         join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                         where rfq.EndDate <= DateTime.Now && rfq.RFQStatus == null
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
                             VendorName = vend.VendorName,
                             VendorAddress = vend.VendorAddress,
                             VendorStatus = vend.VendorStatus,
                             ContactName = vend.ContactName
                         }).Distinct().ToListAsync();


            return  query;
        }

        public async Task<RFQGenerationModel> GetRFQDetailsAsync(int RFQId)
        {
            var Item = await _context.RfqDetails.Where(x => x.RFQId == RFQId).ToListAsync();
            var query = await(from rfq in _context.RfqGenerations
                              join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                              join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                              where rfq.EndDate <= DateTime.Now && rfq.RFQStatus == null && rfq.Id== RFQId
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
                                  VendorName = vend.VendorName,
                                  VendorAddress = vend.VendorAddress,
                                  VendorStatus = vend.VendorStatus,
                                  ContactName = vend.ContactName
                              }).Distinct().FirstOrDefaultAsync();

            List<RFQDetailsModel> rFQDetails = new List<RFQDetailsModel>();
               //foreach (var item in Item)
               // {
               // rFQDetails.Add(new RFQDetailsModel { RFQId = item.RFQId,
               //                                                 VendorId = item.VendorId, 
               //                                                 ItemId = item.ItemId,
               //                                                 ItemName = item.ItemName,
               //                                                 QuotedQuantity = item.QuotedQuantity,
               //                                                 AgreedQuantity = item.AgreedQuantity, 
               //                                                 QuotedAmount = item.QuotedAmount,
               //                                                 AgreedAmount = item.AgreedAmount
               //                                               });
               // }

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

            query.RFQDetails = rFQDetails;


            return query;

        }
    }
}
