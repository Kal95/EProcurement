using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace E_Procurement.Repository.PoAcceptanceRepo
{
    public class POAcceptanceRepository : IPOAcceptanceRepository
    {
        private readonly EProcurementContext _context;

        public POAcceptanceRepository(EProcurementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<POAcceptanceViewModel>> GetAllPO()
        {
            var query = await (from poa in _context.PoGenerations
                               join vendor in _context.Vendors on poa.VendorId equals vendor.Id
                               join rfqDetails in _context.RfqDetails on poa.RFQId equals rfqDetails.RFQId
                               orderby poa.Id descending
                               select new POAcceptanceViewModel()
                               {
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

        public async Task<POAcceptanceViewModel> GetPODetails(int Id)
        {
            var query = await _context.PoGenerations
                .Where(x => x.Id == Id)
                .Select(x => new POAcceptanceViewModel
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

        public bool UpdatePO(int Id, DateTime ExpectedDeliveryDate, out string Message)
        {
            var oldEntry = _context.PoGenerations
               .Where(x => x.Id == Id).FirstOrDefault();

            oldEntry.ExpectedDeliveryDate = ExpectedDeliveryDate;

            //oldEntry.

            _context.SaveChanges();

            Message = "PO updated successfully";

            return true;
        }
    }
}
