using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Repository.VendoRepo;

namespace E_Procurement.Repository.ReportRepo
{
    public class ReportModel
    {
        public string VendorName { get; set; }
        public int VendorCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string VendorAddress { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactName { get; set; }
        public string VendorStatus { get; set; }
    
        public string WebsiteAddress { get; set; }
        public VendorModel vendorModel { get; set; }
        public List<VendorModel> VendorDetails { get; set; }
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
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<string> Descriptions { get; set; }
        public List<int> Quantities { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
