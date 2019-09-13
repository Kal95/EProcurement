using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class ApprovalType 
    {
       
        public string ApprovalTypeName { get; set; }
        public int ApprovalTypeId { get; set; }
        public DateTime DateCreated { get; set; } 
        public DateTime LastDateUpdated { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }


    }
}
