using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.HTTP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Procurement.HTTP.Service
{
    public class ItemCategoryService : IItemCategoryService, IItemService
    {
        private readonly APIcontext _context;
       
        public ItemCategoryService(APIcontext context)
        {
           _context = context;
           
        }
        public ItemCategory GetItemCategories(int Id)
        {
            var details = _context.ItemsCategory.Where(x => x.Id == Id).FirstOrDefault();

            return details;
        }
        public Item GetItems(int Id)
        {
            var details = _context.Items.Where(x => x.Id == Id).FirstOrDefault();

            return details;
        }
        //public async Task<IEnumerable<ItemCategory>>GetItemCategories()
        //{
        //    return await _context.ItemsCategory.ToListAsync();
        //}
        //public async Task<IEnumerable<Item>> GetItems()
        //{
        //    return await _context.Items.ToListAsync();
        //}
        //public IEnumerable<ItemCategory> GetItemCategories()
        //{
        //    return _context.ItemsCategory.OrderByDescending(u => u.Id).ToList();
        //}
        //public IEnumerable<Item> GetItems()
        //{
        //    return _context.Items.OrderByDescending(u => u.Id).ToList();
        //}

    }
}
