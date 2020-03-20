using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using E_Procurement.Data;
using E_Procurement.Repository.Dtos;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Data.Entity;
using Microsoft.AspNetCore.Http;
using E_Procurement.WebUI.Models.RequisitionModel;

namespace E_Procurement.Repository.QuoteSendingRepo
{
    public class QuoteSendingRepository : IQuoteSendingRepository
    {
        private readonly EProcurementContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public QuoteSendingRepository(EProcurementContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }
        public async Task<IEnumerable<RFQGenerationModel>> GetQuoteAsync()
        {

            var currentUser = _contextAccessor.HttpContext.User.FindFirst("Email").Value;
            var query = await (from rfq in _context.RfqGenerations
                               join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                               join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                               where rfq.EndDate >= DateTime.Now && rfq.RFQStatus == null && vend.Email == currentUser
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
            var currentUser = _contextAccessor.HttpContext.User.FindFirst("Email").Value;
            var query = await(from rfq in _context.RfqGenerations
                              join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                              join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                              where DateTime.Now < rfq.EndDate && rfq.RFQStatus == null && rfq.Id == RFQId && vend.Email == currentUser
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
                                  VendorId = rfqDetails.VendorId,
                                  VendorAddress = vend.VendorAddress,
                                  VendorStatus = vend.VendorStatus,
                                  ContactName = vend.ContactName
                              }).Distinct().FirstOrDefaultAsync();

            List<RFQDetailsModel> rFQDetails = new List<RFQDetailsModel>();
            var Item = await _context.RfqDetails.Where(x => x.RFQId == RFQId && x.VendorId == query.VendorId).ToListAsync();
            

            var listModel = Item.Select(x => new RFQDetailsModel
            {
                DetailsId = x.Id,
                RFQId = x.RFQId,
                VendorId = x.VendorId,
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                Description = x.ItemDescription,
                QuotedQuantity = x.QuotedQuantity,
                AgreedQuantity = x.AgreedQuantity,
                QuotedPrice = x.QuotedPrice,
                AgreedPrice = x.AgreedPrice,
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

        public bool UpdateQuote(int[] Id, decimal[] quotedPrice, decimal[] quotedAmount, RequisitionModel model, out string Message)
        {

            for (int i = 0; i < quotedAmount.Length; i++)
            {                          
                
                    var oldEntry = _context.RfqDetails.Where(x => x.Id == Id[i]).FirstOrDefault();


                    if (oldEntry == null)
                    {
                        throw new Exception("No RFQ exists with this Id");
                    }

                    oldEntry.QuotedPrice = quotedPrice[i];
                    oldEntry.QuotedAmount = quotedAmount[i];
                    oldEntry.QuoteDocument = model.QuoteDocumentPath;


                //foreach (var item in AgreedAmount)
                //{
                //    oldEntry.QuotedAmount = item[i];

                //}

                _context.SaveChanges();

              

                
            }

            Message = "RFQ Details updated successfully";

            return true;
        }
    }
}
