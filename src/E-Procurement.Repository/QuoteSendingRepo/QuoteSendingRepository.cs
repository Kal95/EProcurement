using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using E_Procurement.Data;
using E_Procurement.Repository.Dtos;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Data.Entity;

namespace E_Procurement.Repository.QuoteSendingRepo
{
    public class QuoteSendingRepository : IQuoteSendingRepository
    {
        private readonly EProcurementContext _context;
        public QuoteSendingRepository(EProcurementContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RFQGenerationModel>> GetQuoteAsync()
        {
            var query = await (from rfq in _context.RfqGenerations
                               join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                               join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                               where rfq.EndDate >= DateTime.Now && rfq.RFQStatus == null
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


            return query;

            //throw new NotImplementedException();
        }

        public async Task<RFQGenerationModel> GetQuoteDetailsAsync(int RFQId)
        {
            var Item = await _context.RfqDetails.Where(x => x.RFQId == RFQId).ToListAsync();
            var query = await(from rfq in _context.RfqGenerations
                              join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                              join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                              where DateTime.Now < rfq.EndDate && rfq.RFQStatus == null && rfq.Id == RFQId
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
                DetailsId = x.Id,
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

        public List<RFQDetails> GetRfqDetails()
        {
            return _context.RfqDetails.OrderBy(x => x.Id).ToList();
        }

        public bool UpdateQuote(int[] Id, decimal[] AgreedAmount, out string Message)
        {

            for (int i = 0; i < AgreedAmount.Length; i++)
            {                          
                
                    var oldEntry = _context.RfqDetails.Where(x => x.Id == Id[i]).FirstOrDefault();


                    if (oldEntry == null)
                    {
                        throw new Exception("No RFQ exists with this Id");
                    }

                    oldEntry.AgreedAmount = AgreedAmount[i];
                

                    //foreach (var item in AgreedAmount)
                    //{
                    //    oldEntry.AgreedAmount = item[i];

                    //}

                    _context.SaveChanges();

              

                
            }

            Message = "RFQ Details updated successfully";

            return true;
        }
    }
}
