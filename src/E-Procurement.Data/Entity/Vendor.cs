using System.Collections.Generic;

namespace E_Procurement.Data.Entity
{
    /// <summary>
    /// Vendor Entity
    /// </summary>
    public class Vendor: BaseEntity.Entity
    {
        public Vendor()
        {
            VendorMapping = new List<VendorMapping>();
        }
        public string VendorName { get; set; }
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
        public ICollection<VendorMapping> VendorMapping { get; set; }
        //public VendorCategory VendorCategory { get; set; }
        public Bank Bank { get; set; }
        public Country Country { get; set; }
        public State State { get; set; }
    }
}
