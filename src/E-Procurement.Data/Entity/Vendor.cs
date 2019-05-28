namespace E_Procurement.Data.Entity
{
    /// <summary>
    /// Vendor Entity
    /// </summary>
    public class Vendor: BaseEntity.Entity
    {
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public string VendorStatus { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string WebsiteAddress { get; set; }
        public string TINNo { get; set; }
        public string CACNo { get; set; }
        public string VATNo { get; set; }
        public int BankId { get; set; }
        public string BankBranch { get; set; }
        public string AccountName { get; set; }
        public string SortCode { get; set; }
        public string AccountNo { get; set; }
        public string AATCurrency { get; set; }
        public decimal AATAmount { get; set; }
    
    }
}
