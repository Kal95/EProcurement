using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class RFQApprovalStatus : BaseEntity.Entity
    {
        public int RFQId { get; set; }
        public int CurrentApprovalLevel { get; set; }
      
    }
}
