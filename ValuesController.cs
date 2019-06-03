using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        
        // GET api/values
        [HttpGet("GetAllItem")]
        public ActionResult<IEnumerable<string>> GetAllItem()
        {
            return new string[] { "Table", "Chair", "Pen", "Biro", "Mouse", "Phone", "Chair", "Fejiro" };
        }

        [HttpGet("GetAllItemCategory")]
        public ActionResult<IEnumerable<string>> GetAllItemCategory()
        {
            return new string[] { "Furniture", "Electronics", "Automobile"};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
