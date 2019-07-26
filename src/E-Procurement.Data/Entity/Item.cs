using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.Data.Entity
{
    public class Item : BaseEntity.Entity
    {
        public string ItemName { get; set; }
        public int ItemCategoryId { get; set; }      
    }
}
