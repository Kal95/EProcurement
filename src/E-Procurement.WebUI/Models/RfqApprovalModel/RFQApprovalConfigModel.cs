using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.WebUI.Models.RfqApprovalModel
{
    public class RFQApprovalConfigModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public int ApprovalLevel { get; set; }
        public bool IsFinalLevel { get; set; }
        public int ApprovalTypeId { get; set; }


    }
}
