using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;
using FluentNHibernate.Testing.Values;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Models.RFQModel
{
    public class RfqGenModel
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        //public string Description { get; set; }
        public int ProjectId { get; set; }
        public int RequisitionId { get; set; }
        public string RfqStatus { get; set; }
        public int RfqId { get; set; }
        public int VendorId { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }
        //public int  Quantity { get; set; }
        public List<string> Description { get; set; }
        public List<int> Quantity { get; set; }




        [Required(ErrorMessage = "You must select at least one Vendor")]
        public List<int> SelectedVendors { get; set; }
        //public List<Vendor> VendorList { get; set; }
        public IEnumerable<SelectListItem> VendorList { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<SelectListItem> ItemCategoryList { get; set; }


        public RfqGenModel()
        {
            ItemList = new List<SelectListItem>();
            VendorList = new List<SelectListItem>();

        }

        [Required]
        public int ItemId { get; set; }
        public List<string> SelectedItems { get; set; }
        public string ItemName { get; set; }
        public IEnumerable<SelectListItem> ItemList { get; set; }
    

        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

    
    }

}
