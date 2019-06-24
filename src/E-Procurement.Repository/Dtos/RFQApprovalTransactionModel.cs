using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Repository.Dtos
{
    public class RFQApprovalTransactionModel
    {
        public int RFQId { get; set; }
        public int ApprovalLevel { get; set; }
        public int VendorId { get; set; }

    }
}
