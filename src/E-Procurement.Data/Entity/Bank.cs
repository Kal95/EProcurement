using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class Bank : BaseEntity.Entity
    {
        public string BankName { get; set; }
        public string SortCode { get; set; }
        //public ICollection<Vendor> Vendor { get; set; }
    }
}
