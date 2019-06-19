using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.WebUI.Controllers.API
{
    [Route("api/{controller}/{action}/{id?}")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly EProcurementContext _context;
        public ItemController(EProcurementContext context)
        {
            _context = context;
        }
        //Get/api/category
       
        public List<ItemCategory> GetCategory()
        {
            return _context.ItemCategories.ToList();
        }

        //Get/api/item/1
        public List<Item> GetItem(int id)
        {
            return _context.Items.Where(u => u.ItemCategoryId == id).ToList();
        }
    }
}