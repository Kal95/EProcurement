using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class RFQDetails : BaseEntity.Entity
    {
        public int RFQId { get; set; }
        public int VendorId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int QuotedQuantity { get; set; }
        public int AgreedQuantity { get; set; }
        public decimal QuotedPrice { get; set; }
        public decimal AgreedPrice { get; set; }
        public decimal QuotedAmount { get; set; }
        public decimal AgreedAmount { get; set; }
        public string ItemDescription { get; set; }
        public string RFQTitle { get; set; }
        public string RFQBody { get; set; }
        public string RFQCondition { get; set; }
        public string QuoteDocument { get; set; }
    }
}
