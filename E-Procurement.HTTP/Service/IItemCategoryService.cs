using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.HTTP.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.HTTP.Service
{
    public interface IItemCategoryService 
    {
       //IEnumerable<ItemCategory> GetItemCategories();
       // Task<IEnumerable<Item>> GetItems();
             ItemCategory GetItemCategories(int Id);
        Item GetItems(int Id);



    }
}
