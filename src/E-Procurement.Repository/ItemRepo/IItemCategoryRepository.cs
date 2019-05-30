using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;



namespace E_Procurement.Repository.ItemRepo
{
    public interface IItemCategoryRepository : IDisposable
    {
            Task<List<ItemCategory>> GetAllCategories(CancellationToken ct = default(CancellationToken));
            Task<List<Item>> GetAllItems(CancellationToken ct = default(CancellationToken));

    }
}
