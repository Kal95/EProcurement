using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Repository.Dtos;
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
        public DateTime CreatedDate { get; set; }
        public string WebsiteAddress { get; set; }
        public VendorModel vendorModel { get; set; }
        public List<VendorModel> VendorDetails { get; set; }
        public List<RFQDetailsModel> RFQDetails { get; set; }
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
        public string PONumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public int PoId { get; set; }
     
        public string BestPrice { get; set; }
        public string ProductAvailability { get; set; }
        public string ProductQuality { get; set; }
        public string DeliveryTimeFrame { get; set; }
        public string CreditFacility { get; set; }
        public string WarrantySupport { get; set; }
        public string CustomerCare { get; set; }
        public string Others { get; set; }
        public string Score { get; set; }

        public int PeriodId { get; set; }
        public string EvaluationPeriod { get; set; }
        public int EvaluationId { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string UserName { get; set; }
        public int ConfigId { get; set; }

    }
}
