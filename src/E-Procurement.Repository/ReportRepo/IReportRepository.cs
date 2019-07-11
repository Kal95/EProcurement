using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.RFQModel;

namespace E_Procurement.Repository.ReportRepo
{
    public interface IReportRepository : IDependencyRegister
    {
        //List<Vendor> GetVendors(RfqGenModel model);
        IEnumerable<Vendor> GetVendors();
        IEnumerable<VendorMapping> GetMapping();
        //List<Vendor> GetVendorDetails(RfqGenModel model);
        List<RFQGenerationModel> GetRfqGen();
        List<RFQGenerationModel> GetPoGen();
        List<RFQDetails> GetRFQDetails();
    }
}
