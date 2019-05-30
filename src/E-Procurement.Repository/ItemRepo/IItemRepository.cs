using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;


namespace E_Procurement.Repository.ItemRepo
{
     public interface IItemRepository : IDisposable
        {
            Task<List<Item>> GetAllItems(CancellationToken ct = default(CancellationToken));

        }
   
}
