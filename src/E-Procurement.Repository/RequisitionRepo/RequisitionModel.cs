using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Models.RequisitionModel
{
    public class RequisitionModel
    {
        public int Id { get; set; }
        public string Initiator { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; }
        public DateTime ExpectedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        //public string RequisitionDocument { get; set; }
        public string RequisitionDocumentPath { get; set; }
        public IFormFile RequisitionDocument { get; set; }
        public string QuoteDocumentPath { get; set; }
        public IFormFile QuoteDocument { get; set; }
        public string RefCode { get; set; }
    }

}
