using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.Repository.VendoRepo
{
   public class VendorModel
    {
        public int Id { get; set; }
        public string VendorName { get; set; }
        public int VendorCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string VendorAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public string VendorStatus { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string WebsiteAddress { get; set; }
        public string TinNo { get; set; }
        public string CacNo { get; set; }
        public string VatNo { get; set; }
        public int BankId { get; set; }
        public string BankBranch { get; set; }
        public string AccountName { get; set; }
        public string SortCode { get; set; }
        public string AccountNo { get; set; }
        public string AatCurrency { get; set; }
        public decimal AatAmount { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<SelectListItem> VendorCategoryList { get; set; }
        public IEnumerable<SelectListItem> BankList { get; set; }
        public IEnumerable<SelectListItem> CountryList { get; set; }
        public IEnumerable<SelectListItem> StateList { get; set; }
        public List<int> SelectedVendorCategories { get; set; }
        public int MappingId { get; set; }
        public int VendorId { get; set; }
        public VendorModel()
        {
            VendorCategoryList = new List<SelectListItem>();
           
        }
    }
}
