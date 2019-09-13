using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class RFQApprovalConfig : BaseEntity.Entity
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public int ApprovalLevel { get; set; }
        public bool IsFinalLevel { get; set; }
        public int ApprovalTypeId { get; set; }


    }
}
