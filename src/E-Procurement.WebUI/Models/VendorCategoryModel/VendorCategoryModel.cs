using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Models.VendorCategoryModel
{
    public class VendorCategoryModel
    {
        [Required]
        public string CategoryName { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
