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
        List<Vendor> GetVendorsByCategory(RfqGenModel model);
        IEnumerable<Vendor> GetVendors();
        IEnumerable<VendorEvaluation> GetVendorEvaluation();
        List<VendorEvaluation> GetVendorEvaluationByCategory(RfqGenModel model);
        IEnumerable<VendorMapping> GetMapping();
        //List<Vendor> GetVendorDetails(RfqGenModel model);
        List<RFQGenerationModel> GetRfqGen();
        List<RFQGenerationModel> GetPoGen();
        List<RFQDetails> GetRFQDetails();
        bool VendorEvaluation(RfqGenModel model, out string Message);
    }
}
