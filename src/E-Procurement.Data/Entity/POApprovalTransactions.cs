using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class POApprovalTransactions
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public int RFQId { get; set; }
        public int POId { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public int ApproverID { get; set; }
        public int ApprovalLevel { get; set; }
        public string Comments { get; set; }
        public DateTime DateApproved { get; set; }

    }
}
