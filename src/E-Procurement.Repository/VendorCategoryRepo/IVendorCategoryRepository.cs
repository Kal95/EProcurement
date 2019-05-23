using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.VendorCategoryRepo
{
    public interface IVendorCategoryRepository : IDependencyRegister
    {
        IEnumerable<VendorCategory> GetVendorCategories();
        bool CreateVendorCategory(string CategoryName, string UserId, out string Message);

        bool UpdateVendorCategory(int Id, string CategoryName, bool IsActive, string UserId, out string Message);
    }
}
