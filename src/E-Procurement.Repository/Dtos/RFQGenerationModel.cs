﻿using E_Procurement.WebUI.Models.RequisitionModel;
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
            public string Item { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string RFQStatus { get; set; }
            public string POStatus { get; set; }
            public int VendorId { get; set; }
            public string VendorName { get; set; }
            public string VendorAddress { get; set; }
            public string VendorStatus { get; set; }
            public string ContactName { get; set; }
            public string VendorEmail { get; set; }
            public List<RFQDetailsModel> RFQDetails { get; set; }
            public List<RFQDetailsModel> RFQDetails2 { get; set; }
            public List<RequisitionModel> Requisition { get; set; }
            public List<RFQDetailsModel> RFQTransaction { get; set; }
            public string ApprovedBy { get; set; }
            public string Comments { get; set; }
            public string PONumber { get; set; }
            public decimal TotalAmount { get; set; }
            public DateTime ApprovedDate { get; set; }
            public DateTime ExpectedDeliveryDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public decimal QuotedAmount { get; set; }
            public decimal QuotedPrice { get; set; }
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
            public string RFQTitle { get; set; }
            public string RFQBody { get; set; }
            public string RFQCondition { get; set; }

            public string POTitle { get; set; }
            public string POPreamble { get; set; }
            public string POCost { get; set; }
            public string POWarranty { get; set; }
            public string POTerms { get; set; }
            public string POValidity { get; set; }
            public string URL { get; set; }
            public string Signature1 { get; set; }
            public string Signature2 { get; set; }
            public bool FinalApprover { get; set; }
            public string ApproverName { get; set; }
            public IFormFile QuoteDocument { get; set; }
            public string QuoteDocumentPath { get; set; }

            public string ComparisonDocumentPath { get; set; }
            public IFormFile ComparisonDocument { get; set; }
            public string RefCode { get; set; }


    }
}
