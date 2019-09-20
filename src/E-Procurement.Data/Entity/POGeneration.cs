using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class POGeneration : BaseEntity.Entity
    {
        public int RFQId { get; set; }
        public string PONumber { get; set; }
        public int VendorId { get; set; }
        public decimal Amount { get; set; }
        public string POStatus { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
        public string POTitle { get; set; }
        public string POPreamble { get; set; }
        public string POCost { get; set; }
        public string POWarranty { get; set; }
        public string POTerms { get; set; }
        public string POValidity { get; set; }
        public string Reference { get; set; }

    }
}
