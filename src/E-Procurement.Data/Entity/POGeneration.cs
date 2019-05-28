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
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
       
    }
}
