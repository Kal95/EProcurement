using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.PORepo
{
    public interface IPORepository : IDependencyRegister
    {
        Task<bool> GenerationPOAsync(RFQGenerationModel rfq);
        Task<IEnumerable<RFQGenerationModel>> GetPOAsync();
        Task<POGeneration> GetPOByIdAsync(int Id);
        Task<POGeneration> GetPOByPONumberAsync(string PONumber);
        List<RFQGenerationModel> GetPoGen();
        List<RFQGenerationModel> GetPoGen2();
        List<RFQDetails> GetRFQDetails();
        IEnumerable<Vendor> GetVendors();
        List<RFQGeneration> GetRFQs();
        bool POApproval(RFQDetailsModel model, out string Message);
        IEnumerable<User> GetUser();
        List<RFQGenerationModel> GetApprovedPO();
        List<RFQGenerationModel> GetApprovedPO2();
        List<RFQGenerationModel> GetApprovedRFQ();
        List<POApprovalTransactions> GetPOApprovals();
        List<RFQApprovalConfig> GetPOApprovalConfig();
    }
}
