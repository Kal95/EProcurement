using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.HTTP.Models;
using E_Procurement.HTTP.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.HTTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCategoryController : ControllerBase
    {
    
        private readonly IItemCategoryService _itemCategoryService;
        private readonly IItemCategoryService _itemService;
        private readonly IMapper _mapper;

        public ItemCategoryController(IItemCategoryService itemCategoryService, IItemCategoryService itemService, IMapper mapper)
        {
            _itemCategoryService = itemCategoryService;
            _itemService = itemService;
            _mapper = mapper;
        }

        // GET: api/ItemCategory
        [HttpGet]
        [Route("GetItemCategories")]
      
        public ItemCategory GetItemCategories(int Id)
        {
           ItemCategory model  = new ItemCategory();
            try
            {
                
                var itemCategories = _itemCategoryService.GetItemCategories(Id).ItemCategories;
                
                if (itemCategories != null)
                {
                    model.Success = true;

                    model.ItemCategories = itemCategories;

                    model.RecordsFound = itemCategories.Count();

                    
                }
                else if (itemCategories.Count() == 0)
                {
                    model.Success = true;

                    model.ItemCategories = null;

                    model.RecordsFound = 0;

                }
                return model;
            }
            catch (Exception)
            {
                model.Success = true;

                model.ItemCategories = null;
            
                model.RecordsFound = 0;
                
            }
            return model;
        }

        // GET: api/Item
        //[HttpGet]
        //[Route("GetItems")]
        //public Item GetItems(int Id)
        //{
        //    ItemCategory model = new ItemCategory();
        //    try
        //    {

        //        var items = _itemService.GetItems(Id).ToList();

        //        if (items.Count() > 0)
        //        {
        //            model.Success = true;

        //            model.Items = items.ToList();

        //            model.RecordsFound = items.Count();
        //        }
        //        else if (items.Count() == 0)
        //        {
        //            model.Success = true;

        //            model.Items = null;

        //            model.RecordsFound = 0;

        //        }

        //        return model;

        //    }
        //    catch (Exception)
        //    {
        //        model.Success = true;

        //        model.Items = null;

        //        model.RecordsFound = 0;
        //        return model;
        //    }
        //}

        // GET: api/ItemCategory/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ItemCategory
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/ItemCategory/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
