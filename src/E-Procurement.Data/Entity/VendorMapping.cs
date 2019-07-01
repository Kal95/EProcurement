using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class VendorMapping 
    {
      public int Id { get; set; }
      public int VendorID { get; set; }
      public int VendorCategoryId { get; set; }
        [ForeignKey("VendorID")]
        public Vendor Vendor { get; set; }
        //public VendorCategory VendorCategory { get; set; }
    }
}
