using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace E_Procurement.Repository.PoAcceptanceRepo
{
    public class POAcceptanceRepository : IPOAcceptanceRepository
    {
        private readonly EProcurementContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public POAcceptanceRepository(EProcurementContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<IEnumerable<POAcceptanceModel>> GetAllPO()
        {
            var currentUser = _contextAccessor.HttpContext.User.FindFirst("Email").Value;
            var query = await (from poa in _context.PoGenerations
                               join vendor in _context.Vendors on poa.VendorId equals vendor.Id
                               join rfqDetails in _context.RfqDetails on poa.RFQId equals rfqDetails.RFQId
                               where poa.POStatus == "Generated" && vendor.Email == currentUser
                               orderby poa.Id descending
                               select new POAcceptanceModel()
                               {
                                   Id = poa.Id,
                                   RFQId = poa.RFQId,
                                   VendorId = vendor.Id,
                                   PONumber = poa.PONumber,
                                   Amount = poa.Amount,
                                   VendorName = vendor.VendorName,
                                   VendorAddress = vendor.VendorAddress,
                                   ActualDeliveryDate = poa.ActualDeliveryDate,
                                   ExpectedDeliveryDate = poa.ExpectedDeliveryDate
                               }).Distinct().ToListAsync();

            return query;
                        
        }

        public async Task<POAcceptanceModel> GetPODetails(int Id)
        {
            var query = await _context.PoGenerations
                .Where(x => x.Id == Id)
                .Select(x => new POAcceptanceModel
                {
                    PONumber = x.PONumber,
                    Amount = x.Amount,
                    ActualDeliveryDate = x.ActualDeliveryDate,
                    ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                    RFQId = x.RFQId
                }).Distinct().FirstOrDefaultAsync();

            //var query = await (from poa in _context.PoGenerations
            //                   join vendor in _context.Vendors on poa.VendorId equals vendor.Id
            //                   join rfqDetails in _context.RfqDetails on poa.RFQId equals rfqDetails.RFQId
            //                   orderby poa.Id descending
            //                   select new POAcceptanceViewModel()
            //                   {
            //                       PONumber = poa.PONumber,
            //                       Amount = poa.Amount,
            //                       VendorName = vendor.VendorName,
            //                       VendorAddress = vendor.VendorAddress,
            //                       ActualDeliveryDate = poa.ActualDeliveryDate,
            //                       ExpectedDeliveryDate = poa.ExpectedDeliveryDate
            //                   }).Distinct().FirstOrDefaultAsync();

            return query;
            
        }

        public bool UpdatePO(int Id, out string Message)
        {
            var oldEntry = _context.PoGenerations
               .Where(x => x.Id == Id).FirstOrDefault();

           // oldEntry.ExpectedDeliveryDate = ExpectedDeliveryDate;
            oldEntry.POStatus = "Accepted";

            //oldEntry.

            _context.SaveChanges();

            Message = "PO updated successfully";

            return true;
        }
    }
}
