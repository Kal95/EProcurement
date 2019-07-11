using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Repository.Dtos
{
    public class POAcceptanceModel
    {
        public int Id { get; set; }
        public int RFQId { get; set; }
        public string PONumber { get; set; }
        public int VendorId { get; set; }
        public decimal Amount { get; set; }
        public string VendorName { get; set; }
        public string VendorAddress { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
    }
}
