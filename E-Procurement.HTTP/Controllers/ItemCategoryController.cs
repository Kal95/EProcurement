using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.HTTP.Models;

using E_Procurement.Repository.ItemRepo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.HTTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemCategoryController : ControllerBase
    {
    
        private readonly IItemCategoryRepository _itemCategoryRepository;
        private readonly IItemCategoryRepository _itemService;
        private readonly IMapper _mapper;

        public ItemCategoryController(IItemCategoryRepository itemCategoryRepository, IItemCategoryRepository itemService, IMapper mapper)
        {
            _itemCategoryRepository = itemCategoryRepository;
            _itemService = itemService;
            _mapper = mapper;
        }

        [HttpGet]
        [Produces(typeof(List<ItemCategoryRepository>))]
        public async Task<IActionResult> GetCategories(CancellationToken ct = default(CancellationToken))
        {
            var Result = await _itemCategoryRepository.GetAllCategories(ct); await _itemCategoryRepository.GetAllItems(ct);
            try
            {
                return new ObjectResult(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

       

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
