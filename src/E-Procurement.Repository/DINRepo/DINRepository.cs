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
using E_Procurement.Repository.PORepo;

namespace E_Procurement.Repository.DINRepo
{
    public class DINRepository : IDINRepository
    {
        private readonly EProcurementContext _context;
        private readonly IConvertViewToPDF _pdfConverter;
        private readonly IPORepository _poRepository;

        public DINRepository(EProcurementContext context, IConvertViewToPDF pdfConverter, IPORepository poRepository)
        {
            _context = context;
            _pdfConverter = pdfConverter;
            _poRepository = poRepository;
        }

        public async  Task<bool> DNGenerationAsync(RFQGenerationModel rfq)
        {
            if (rfq != null)
            {
                var poNumber = _poRepository.GetPOByPONumberAsync(rfq.PONumber);
                DNGeneration dnDetails = new DNGeneration
                {
                    PoId = poNumber.Id
                   
                    // = poNumber,
                    //Amount = rfq.TotalAmount,
                    //RFQId = rfq.RFQId,
                    //VendorId = rfq.VendorId,
                    //POStatus = "Generated",
                    //ExpectedDeliveryDate = rfq.ExpectedDeliveryDate
                };
               
                await _context.AddAsync(dnDetails);
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
            var query = await (from po in _context.PoGenerations
                               join vend in _context.Vendors on po.VendorId equals vend.Id
                               join rfq in _context.RfqGenerations on po.RFQId equals rfq.Id
                               orderby po.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   RFQId = rfq.Id,
                                   PONumber = po.PONumber,                                   
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
