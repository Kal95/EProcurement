using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class VendorEvaluation : BaseEntity.Entity
    {
        public string VendorId {get; set;}
        public string VendorName { get; set; }
        public string BestPrice { get; set; }
        public string ProductAvailability { get; set; }
        public string ProductQuality { get; set; }
        public string DeliveryTimeFrame { get; set; }
        public string CreditFacility { get; set; }
        public string WarrantySupport { get; set; }
        public string CustomerCare { get; set; }
        public string Others { get; set; }

    }
}
