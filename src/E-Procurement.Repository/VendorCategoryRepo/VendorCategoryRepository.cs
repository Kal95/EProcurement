using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;

namespace E_Procurement.Repository.VendorCategoryRepo
{
    public class VendorCategoryRepository : IVendorCategoryRepository
    {
        private readonly EProcurementContext _context;


        public VendorCategoryRepository(EProcurementContext context)
        {
            _context = context;
        }
        public bool CreateVendorCategory(string CategoryName, string UserId, out string Message)
        {
            var confirm = _context.VendorCategories.Where(x => x.CategoryName == CategoryName).Count();

            VendorCategory category = new VendorCategory();

            if (confirm == 0)
            {

                category.CategoryName = CategoryName;

                category.IsActive = true;

                category.CreatedBy = UserId;

                category.DateCreated = DateTime.Now;

                _context.Add(category);

                _context.SaveChanges();

                Message = "Category created successfully";

                return true;
            }
            else
            {
                Message = "Category already exist";

                return false;
            }

        }

        public bool UpdateVendorCategory(int Id, string CategoryName, bool IsActive, string UserId, out string Message)
        {

            var confirm = _context.VendorCategories.Where(x => x.CategoryName == CategoryName && x.IsActive == IsActive).Count();

            var oldEntry = _context.VendorCategories.Where(u => u.Id == Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Category exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.CategoryName = CategoryName;

                oldEntry.IsActive = IsActive;

                oldEntry.UpdatedBy = UserId;

                oldEntry.LastDateUpdated = DateTime.Now;

                _context.SaveChanges();

                Message = "Category updated successfully";

                return true;
            }
            else
            {
                Message = "Category already exist";

                return false;
            }

        }

        public IEnumerable<VendorCategory> GetVendorCategories()
        {
            return _context.VendorCategories.OrderByDescending(u => u.Id).ToList();
        }
    }
}

