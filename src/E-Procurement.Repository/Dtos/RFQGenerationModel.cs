using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Repository.Dtos
{
   public class RFQGenerationModel
    {
            public int RFQId { get; set; }
            public int  ProjectId { get; set; }
            public int RequisitionId { get; set; }
            public string Reference { get; set; }
            public string Description { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string RFQStatus { get; set; }
            public string VendorName { get; set; }
            public string VendorAddress { get; set; }
            public string VendorStatus { get; set; }
            public string ContactName { get; set; }
            public List<RFQDetailsModel> RFQDetails { get; set; }

    }
}
