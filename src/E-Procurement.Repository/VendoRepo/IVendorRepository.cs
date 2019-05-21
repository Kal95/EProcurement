using System.Threading.Tasks;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.VendoRepo
{
    public interface IVendorRepository: IDependencyRegister
    {
        /// <summary>
        /// Register Vendor 
        /// </summary>
        /// <param name="vendor"></param>
        /// <returns></returns>
        Task<bool> RegisterVendor(Vendor vendor);
    }
}