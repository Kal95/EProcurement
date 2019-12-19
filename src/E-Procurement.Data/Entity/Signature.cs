using System;
using System.Collections.Generic;

namespace E_Procurement.Data.Entity
{
   
    public class Signature
    {
        public int  Id { get; set; }
        public string Signee1 { get; set; }
        public string Signee2 { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public bool IsActive { get; set; }

        public string Sign1 { get; set; }
        public string Sign2 { get; set; }
      
    }
}
