using E_Procurement.WebUI.Models.RequisitionModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Procurement.Repository.Dtos
{
    public class InvoiceGenerationModel
    {
        public string PONumber { get; set; }
        public string Description { get; set; }
        public string VendorName { get; set; }   
       public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public IFormFile InvoiceFilePath { get; set; }
        public string DnFilePath { get; set; }
        //public int CurrentPage { get; set; }
        //public int TotalPages { get; set; }
        //public int PageSize { get; set; }

    }

}
