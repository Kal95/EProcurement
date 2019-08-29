using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class State: BaseEntity.Entity
    {
        public string StateName { get; set; }
        //public ICollection<Vendor> Vendor { get; set; }
    }
}
