using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.VendorCategoryModel;

namespace E_Procurement.Repository.VendorCategoryRepo
{
    public interface IVendorCategoryRepository : IDependencyRegister
    {
        IEnumerable<ItemCategory> GetVendorCategories();
        bool CreateVendorCategory(CategoryModel model, out string Message);

        bool UpdateVendorCategory(CategoryModel model, out string Message);
        IEnumerable<Item> GetItems();
        bool CreateItem(CategoryModel model, out string Message);

        bool UpdateItem(CategoryModel model, out string Message);

        List<CategoryModel> GetItems_Categories();
    }
}
