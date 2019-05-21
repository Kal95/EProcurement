using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;

namespace E_Procurement.Repository.VendoRepo
{
    public class VendorRepository : IVendorRepository
    {
        private readonly EProcurementContext _context;

        public VendorRepository(EProcurementContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Register Vendor 
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        public async Task<bool> RegisterVendor(Vendor vendor)
        {
            if (vendor != null)
            {
                await _context.AddAsync(vendor);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

    }
}
