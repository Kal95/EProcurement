using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.ReportRepo;
using FluentNHibernate.Testing.Values;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Models.RFQModel
{
    public class RfqGenModel
    {
        public List<RFQGeneration> Rfqs { get; set; }
        public int Id { get; set; }
        public string Reference { get; set; }
        //public string Description { get; set; }
        public int ProjectId { get; set; }
        public int RequisitionId { get; set; }
        public string RfqStatus { get; set; }
        public int RfqId { get; set; }
        public int VendorId { get; set; }
        public int PoId { get; set; }
        public string CreatedBy { get; set; }
        public string InitiatedBy { get; set; }
        public bool IsActive { get; set; }
        public string UpdatedBy { get; set; }

        //public int Quantity { get; set; }

        //public string Description { get; set; }
        public int RFQId { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public string RFQStatus { get; set; }
        public DateTime CreatedDate { get; set; }

        public string RFQTitle { get; set; }
        public string RFQBody { get; set; }
        public string RFQCondition { get; set; }
        //[Required]
        public List<string> Descriptions { get; set; }
        //[Required]
        public List<int> Quantities { get; set; }
        //public List<string> VendorName { get; set; }
        public List<ReportModel> Report { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string VendorStatus { get; set; }
        public string ContactName { get; set; }
        public string VendorEmail { get; set; }
        public string PhoneNumber { get; set; }
        public decimal QuotedAmount { get; set; }
        public decimal QuotedPrice { get; set; }

        [Required(ErrorMessage = "You must select at least one Vendor")]
        public List<int> SelectedVendors { get; set; }
        //public List<Vendor> VendorList { get; set; }
        public IEnumerable<SelectListItem> VendorList { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public IEnumerable<SelectListItem> ItemCategoryList { get; set; }
        public List<string>BestPrice {get; set;}
        public List<string> AbilityToDeliver { get; set; }
        public List<string> CreditFacility { get; set; }
        public List<string> CustomerSupport { get; set; }
        public List<string> ProductAvailability { get; set; }
        public List<string> ProductQuality { get; set; }
        public List<string> WarrantySupport { get; set; }
        public List<string> Others { get; set; }

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
        public IEnumerable<SelectListItem> CriteriaList { get; set; }
        public IEnumerable<SelectListItem> PeriodList { get; set; }
        public int PeriodId { get; set; }
        public IFormFile RequisitionDocument { get; set; }
        public string RequisitionDocumentPath { get; set; }
    }

}
