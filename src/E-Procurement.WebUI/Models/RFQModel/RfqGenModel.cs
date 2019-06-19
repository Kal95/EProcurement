using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;

namespace E_Procurement.WebUI.Models.RFQModel
{
    public class RfqGenModel
    {
        public string Reference { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int RequisitionId { get; set; }
        public string RfqStatus { get; set; }
        public int RfqId { get; set; }
        public int VendorId { get; set; }
        
        [Required(ErrorMessage = "You must select at least one Vendor")]
        public List<int> SelectedVendors { get; set; }
        public List<Vendor> VendorList { get; set; }
     

        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ItemCategory> ItemCategoryList { get; set; }

        [Required]
        List<int> SelectedItems { get; set; }
        public string ItemName { get; set; }
        public List<Item> ItemList { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }


    }
}
