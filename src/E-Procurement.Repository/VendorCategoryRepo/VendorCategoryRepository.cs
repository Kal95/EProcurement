using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.VendorCategoryModel;

namespace E_Procurement.Repository.VendorCategoryRepo
{
    public class VendorCategoryRepository : IVendorCategoryRepository
    {
        private readonly EProcurementContext _context;


        public VendorCategoryRepository(EProcurementContext context)
        {
            _context = context;
        }
        public bool CreateVendorCategory(CategoryModel model, out string Message)
        {
            var confirm = _context.ItemCategories.Where(x => x.CategoryName == model.CategoryName).Count();

            ItemCategory category = new ItemCategory();

            if (confirm == 0)
            {

                category.CategoryName = model.CategoryName;

                category.IsActive = true;

                category.CreatedBy = model.CreatedBy;

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

        public bool UpdateVendorCategory(CategoryModel model, out string Message)
        {

            var confirm = _context.ItemCategories.Where(x => x.CategoryName == model.CategoryName && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.ItemCategories.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Category exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.CategoryName = model.CategoryName;

                oldEntry.IsActive = model.IsActive;

                oldEntry.UpdatedBy = model.CreatedBy;

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
        public bool CreateItem(CategoryModel model, out string Message)
        {
            var confirm = _context.Items.Where(x => x.ItemName == model.ItemName && x.ItemCategoryId == model.CategoryId).Count();

            Item item = new Item();

            if (confirm == 0)
            {

                item.ItemCategoryId = model.CategoryId;

                item.ItemName = model.ItemName;

                item.IsActive = true;

                item.CreatedBy = model.CreatedBy;

                item.DateCreated = DateTime.Now;

                _context.Add(item);

                _context.SaveChanges();

                Message = "Item created successfully";

                return true;
            }
            else
            {
                Message = "Item already exist";

                return false;
            }

        }

        public bool UpdateItem(CategoryModel model, out string Message)
        {

            var confirm = _context.Items.Where(x => x.ItemName == model.ItemName && x.ItemCategoryId == model.CategoryId && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.Items.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Item exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.ItemCategoryId = model.CategoryId;

                oldEntry.ItemName = model.ItemName;

                oldEntry.IsActive = model.IsActive;

                oldEntry.CreatedBy = model.CreatedBy;

                oldEntry.DateCreated = DateTime.Now;

                _context.SaveChanges();

                Message = "Item updated successfully";

                return true;
            }
            else
            {
                Message = "Item already exist";

                return false;
            }

        }

        public IEnumerable<ItemCategory> GetVendorCategories()
        {
            return _context.ItemCategories.OrderByDescending(u => u.Id).ToList();
        }
        public IEnumerable<Item> GetItems()
        {
            return _context.Items.OrderByDescending(u => u.Id).ToList();
        }
        public List<CategoryModel>GetItems_Categories()
        {
            var query = (from item in _context.Items
                         join category in _context.ItemCategories on item.ItemCategoryId equals category.Id

                         orderby item.Id descending
                         select new CategoryModel()
                         {
                             Id = item.Id,
                             CategoryId = category.Id,
                             ItemName = item.ItemName,
                             CategoryName = category.CategoryName,
                             IsActive = item.IsActive
                             
                         }).ToList();
            return query;
        }
    }
}

