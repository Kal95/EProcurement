using System.Collections.Generic;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.VendoRepo
{
    public interface IVendorRepository: IDependencyRegister
    {
        List<VendorModel> GetItemCategory();
        IEnumerable<Vendor> GetVendors();
        IEnumerable<VendorMapping> GetMapping();
        bool CreateVendor(VendorModel model, out string Message);

        bool UpdateVendor(VendorModel model, out string Message);
    }
}