using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using E_Procurement.Repository.VendoRepo;
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

        bool CreateEvaluationPeriod(ReportModel model, out string Message);
        bool UpdateEvaluationPeriod(ReportModel model, out string Message);
        IEnumerable<EvaluationPeriodConfig> GetEvaluationPeriods();
        List<VendorEvaluation> GetEvaluationByPeriod(RfqGenModel model);
        bool CreateUserToCategory(VendorModel model, out string Message);
        bool UpdateUserToCategory(VendorModel model, out string Message);
        IEnumerable<UserToCategoryConfig> GetUserToCategoryConfig();
        IEnumerable<User> GetUser();
        List<ReportModel> GetUserToCategoryList();
       
    }
}
