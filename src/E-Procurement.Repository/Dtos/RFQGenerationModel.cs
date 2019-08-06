using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Repository.Dtos
{
   public class RFQGenerationModel
    {
           public int PoId { get; set; }
            public int RFQId { get; set; }
            public int  ProjectId { get; set; }
            public int RequisitionId { get; set; }
            public string Reference { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string RFQStatus { get; set; }
            public int VendorId { get; set; }
            public string VendorName { get; set; }
            public string VendorAddress { get; set; }
            public string VendorStatus { get; set; }
            public string ContactName { get; set; }
            public string VendorEmail { get; set; }
            public List<RFQDetailsModel> RFQDetails { get; set; }
            public string ApprovedBy { get; set; }
            public string Comments { get; set; }
            public string PONumber { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime ApprovedDate { get; set; }
            public DateTime ExpectedDeliveryDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public decimal QuotedAmount { get; set; }
            public string PhoneNumber { get; set; }
            public int QuotedQuantity { get; set; }
            public bool IsActive { get; set; }

            // section for INVOICE
            public IFormFile InvoiceFilePath { get; set; }
            public string DnFilePath { get; set; }
            public byte DnFileBlob { get; set; }
            public string DnRecievedBy { get; set; }
            public DateTime DnUploadedDate { get; set; }
            public string DnUploadedBy { get; set; }

            public string GRNNo { get; set; }
            public string GRNFilePath { get; set; }
            public byte GRNFileBlob { get; set; }
            public DateTime GRNUploadedDate { get; set; }
            public string GRNUploadedBy { get; set; }

    }
}
