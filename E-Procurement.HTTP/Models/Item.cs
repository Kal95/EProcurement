using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.HTTP.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
       
       

        public int ItemCategoryId { get; set; }
        public ItemCategory ItemCategory { get; set; }
    }
}
