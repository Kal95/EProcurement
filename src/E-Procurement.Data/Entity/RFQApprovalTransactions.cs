using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class RFQApprovalTransactions : BaseEntity.Entity
    {
        public int VendorId { get; set; }
        public int RFQId { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public int ApprovalLevel { get; set; }
        public string Comments { get; set; }

    }
}
