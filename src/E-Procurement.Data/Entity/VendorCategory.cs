using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class VendorCategory : BaseEntity.Entity
    {
        public string CategoryName { get; set; }
        //public ICollection<VendorMapping> VendorMapping { get; set; }
        //public ICollection<Vendor> Vendor { get; set; }
    }
}
