using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class GRNGeneration : BaseEntity.Entity
    {
        public int POId { get; set; }
        public string GRNNo { get; set; }
        public string GRNFilePath { get; set; }
        public byte GRNFileBlob { get; set; }
        public DateTime GRNUploadedDate { get; set; }
        public string GRNUploadedBy { get; set; }
       
    }
}
