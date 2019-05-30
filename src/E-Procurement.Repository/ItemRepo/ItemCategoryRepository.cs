using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace E_Procurement.Repository.ItemRepo
{
    public class ItemCategoryRepository : IItemCategoryRepository, IItemRepository
    {
        private readonly EProcurementContext _context;
       
        public ItemCategoryRepository(EProcurementContext context)
        {
           _context = context;
           
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<ItemCategory>> GetAllCategories(CancellationToken ct = default(CancellationToken))
        {
            return await _context.ItemCategories.ToListAsync(ct);
        }
        public async Task<List<Item>> GetAllItems(CancellationToken ct = default(CancellationToken))
        {
            return await _context.Items.ToListAsync(ct);
        }

        //public ItemCategory GetItemCategories(int Id)
        //{
        //    var details = _context.ItemsCategory.Where(x => x.Id == Id).FirstOrDefault();

        //    return details;
        //}
        //public Item GetItems(int Id)
        //{
        //    var details = _context.Items.Where(x => x.Id == Id).FirstOrDefault();

        //    return details;
        //}
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
