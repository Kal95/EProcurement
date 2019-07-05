using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Models.POGenerationModel
{
    public class POAcceptanceModel
    {
        public int Id { get; set; }
        public int RFQId { get; set; }
        public string PONumber { get; set; }
        public int VendorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime ActualDeliveryDate { get; set; }
    }
}
