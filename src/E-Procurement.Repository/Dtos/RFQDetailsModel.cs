using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Repository.Dtos
{
   public class RFQDetailsModel
    {
        public int DetailsId { get; set; }
        public int RFQId { get; set; }
        public int ApproverId { get; set; }
        public int ApprovalLevel { get; set; }
        public int POId { get; set; }
        public int VendorId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int QuotedQuantity { get; set; }
        public int AgreedQuantity { get; set; }
        public decimal QuotedPrice { get; set; }
        public decimal AgreedPrice { get; set; }
        public decimal QuotedAmount { get; set; }
        public decimal AgreedAmount { get; set; }
        public string Description { get; set; }
        public string ContactName { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public string RFQTitle { get; set; }
        public string RFQBody { get; set; }
        public string Reference { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string Comments { get; set; }
        public string POTitle { get; set; }
        public string POCost { get; set; }
        public string POWarranty { get; set; }
        public string POTerms { get; set; }
        public string POValidity { get; set; }
       










        public int PoId { get; set; }
      
        public int ProjectId { get; set; }
        public int RequisitionId { get; set; }
   
        public string Item { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RFQStatus { get; set; }
        public string POStatus { get; set; }
       
        public string VendorStatus { get; set; }
       
        public string VendorEmail { get; set; }
     
        public string ApprovedBy { get; set; }
      
        public string PONumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime CreatedDate { get; set; }
       
        public string PhoneNumber { get; set; }
     
        public bool IsActive { get; set; }
        public string RequisitionDocumentPath { get; set; }
        public string QuoteDocumentPath { get; set; }
    }
}
