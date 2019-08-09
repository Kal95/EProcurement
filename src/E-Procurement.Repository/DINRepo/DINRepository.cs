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
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace E_Procurement.Repository.DINRepo
{
    public class DINRepository : IDINRepository
    {
        private readonly EProcurementContext _context;
        private readonly IConvertViewToPDF _pdfConverter;
        private readonly IPORepository _poRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public DINRepository(EProcurementContext context, IConvertViewToPDF pdfConverter, IPORepository poRepository, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _pdfConverter = pdfConverter;
            _poRepository = poRepository;
            _contextAccessor = contextAccessor;
        }

        public async  Task<bool> DNGenerationAsync(RFQGenerationModel rfq)
        {
            if (rfq != null)
            {
                var poNumber = await _context.PoGenerations.Where(x => x.PONumber == rfq.PONumber).FirstOrDefaultAsync();
                DNGeneration dnDetails = new DNGeneration
                {
                    PoId = poNumber.Id,
                    DnUploadedDate = DateTime.Now,
                    DnFilePath= rfq.DnFilePath,
                    DnUploadedBy= _contextAccessor.HttpContext.User.Identity.Name,
                    DnRecievedBy = rfq.DnRecievedBy
                };
               
                await _context.AddAsync(dnDetails);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<RFQGenerationModel> GetInvoiceDetailsAsync(int RFQId)
        {
            var Item = await _context.RfqDetails.Where(x => x.RFQId == RFQId).ToListAsync();
            var totalAmount = Item.Sum(x => x.QuotedAmount);
            var query = await (from rfq in _context.RfqGenerations
                               join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                               join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                               join po in _context.PoGenerations on rfq.Id equals po.RFQId
                               where rfq.Id == RFQId
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
                                   ContactName = vend.ContactName,
                                   VendorEmail = vend.Email,
                                   TotalAmount = totalAmount,
                                   PONumber = po.PONumber
                               }).Distinct().FirstOrDefaultAsync();

            List<RFQDetailsModel> rFQDetails = new List<RFQDetailsModel>();
            foreach (var item in Item)
            {
                rFQDetails.Add(new RFQDetailsModel
                {
                    RFQId = item.RFQId,
                    VendorId = item.VendorId,
                    ItemId = item.ItemId,
                    ItemName = item.ItemName,
                    QuotedQuantity = item.QuotedQuantity,
                    AgreedQuantity = item.AgreedQuantity,
                    QuotedAmount = item.QuotedAmount,
                    AgreedAmount = item.AgreedAmount
                }
                );
            }

            query.RFQDetails = rFQDetails;


            return query;

        }
        public Task<IEnumerable<RFQGenerationModel>> GetApprovedRFQAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RFQGenerationModel>> GetPOAsync()
        {
           // var currentUser = _contextAccessor.HttpContext.User.Identity.Name;
            var currentUser = _contextAccessor.HttpContext.User.FindFirst("Email").Value;
            var query = await (from po in _context.PoGenerations
                               join vend in _context.Vendors on po.VendorId equals vend.Id
                               join rfq in _context.RfqGenerations on po.RFQId equals rfq.Id
                               //join dn in _context.DnGenerations on po.Id equals dn.PoId
                               where !(from dn in _context.DnGenerations select dn.PoId).Contains(po.Id)
                               && vend.Email== currentUser
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
