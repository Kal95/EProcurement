using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.HTTP.Models
{
    public class ItemCategoryModel
    {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; }
     
            public IList<ItemModel> Items { get; set; }
        
    }

}
