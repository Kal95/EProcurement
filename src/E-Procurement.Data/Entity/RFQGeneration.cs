using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class RFQGeneration : BaseEntity.Entity
    {
       
        public int ProjectId { get; set; }
        public int RequisitionId { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RFQStatus { get; set; }
        public string AlteredBy { get; set; }
        public string InitiatedBy { get; set; }
        public DateTime DateAltered { get; set; }
    }
}
