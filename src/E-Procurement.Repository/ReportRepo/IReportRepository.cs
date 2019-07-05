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
        List<Vendor> GetVendors(RfqGenModel model);
       // IEnumerable<Vendor> GetVendors();
        //List<Vendor> GetVendorDetails(RfqGenModel model);
        List<RFQGeneration> GetRfqGen();
        List<RFQGenerationModel> GetPoGen();
    }
}
