using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.HTTP.Models
{
    public class ItemCategory
    {
            public int Id { get; set; }
            public string ItemCategoryName { get; set; }
            public IEnumerable<Item> Items { get; set; }
            public IEnumerable<ItemCategory> ItemCategories { get; set; }
            public bool Success { get; set; }     
            public int RecordsFound { get; set; }

    }
}
