using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Models.VendorCategoryModel
{
    public class CategoryModel
    {
        
        public string CategoryName { get; set; }
        public string ItemName { get; set; }
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int ItemId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public IEnumerable<SelectListItem> ItemCategoryList { get; set; }

    }
}
