using System;
using System.Collections.Generic;

namespace E_Procurement.Data.Entity
{
   
    public class Requisition
    {
        public int  Id { get; set; }
        public string Initiator { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public string RequisitionDocument { get; set; }
        
      
    }
}
