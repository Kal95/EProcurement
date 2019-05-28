using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Data.Entity
{
    public class DNGeneration : BaseEntity.Entity
    {
        public int PoId { get; set; }
        public string DnFilePath { get; set; }
        public byte DnFileBlob { get; set; }
        public string DnRecievedBy { get; set; }
        public DateTime DnUploadedDate { get; set; }
        public string DnUploadedBy { get; set; }
     
    }
}
