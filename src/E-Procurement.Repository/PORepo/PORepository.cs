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

namespace E_Procurement.Repository.PORepo
{
    public class PORepository : IPORepository
    {
        private readonly EProcurementContext _context;
        private readonly IConvertViewToPDF _pdfConverter;

        public PORepository(EProcurementContext context, IConvertViewToPDF pdfConverter)
        {
            _context = context;
            _pdfConverter = pdfConverter;
        }

        public async  Task<bool> GenerationPOAsync(RFQGenerationModel rfq)
        {
            if (rfq != null)
            {
                var poNumber = GuidToBigInteger(Guid.NewGuid()).ToString().Substring(0, 6);
                POGeneration poDetails = new POGeneration
                {
                    PONumber = poNumber,
                    Amount = rfq.TotalAmount,
                    RFQId = rfq.RFQId,
                    VendorId = rfq.VendorId,
                    ExpectedDeliveryDate = rfq.ExpectedDeliveryDate
                };
                rfq.PONumber = poNumber;
                await _context.AddAsync(poDetails);
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

        public Task<IEnumerable<RFQGenerationModel>> GetApprovedRFQAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RFQGenerationModel>> GetPOAsync()
        {
            var query = await (from vend in _context.Vendors
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
                               }).Distinct().ToListAsync();



            return query;
        }

        public async Task<POGeneration> GetPOByIdAsync(int Id)
        {
            return await _context.PoGenerations.FindAsync(Id);
        }
        public async Task<POGeneration> GetPOByPONumberAsync(string PONumber)
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
