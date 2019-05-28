using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class DNGeneration : BaseEntity.Entity
    {
        public int POId { get; set; }
        public string DNFilePath { get; set; }
        public byte DNFileBlob { get; set; }
        public string DNRecievedBy { get; set; }
        public DateTime DNUploadedDate { get; set; }
        public string DNUploadedBy { get; set; }
     
    }
}
