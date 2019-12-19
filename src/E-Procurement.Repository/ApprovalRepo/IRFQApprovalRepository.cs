using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.Repository.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using E_Procurement.Repository.VendoRepo;

namespace E_Procurement.Repository.ApprovalRepo
{
   public  interface IRfqApprovalRepository : IDependencyRegister
    {
        Task<IEnumerable<RFQGenerationModel>> GetRFQApprovalDueAsync();

        Task<IEnumerable<RFQGenerationModel>> GetRFQByVendorsAsync(int RFQId);
        Task<IEnumerable<RFQGenerationModel>> GetSubmittedRFQByVendorsAsync(int RFQId);
        Task<IEnumerable<RFQGenerationModel>> GetRFQPendingApprovalAsync();
        Task<IEnumerable<RFQGenerationModel>> GetRFQInPipelineAsync();
        Task<IEnumerable<RFQGenerationModel>> GetRFQPendingApprovalByVendorsAsync(int RFQId);
        Task<RFQGenerationModel> GetRFQDetailsAsync(int RFQId, int VendorId);

        Task<bool> CreateRFQApprovalAsync(RFQGenerationModel rFQApproval);
        Task<bool> CreateRFQPendingApprovalAsync(RFQGenerationModel rFQApproval);
        List<Vendor> GetVendors();
        List<RFQDetails> GetRFQDetails();
        List<RFQGeneration> GetRFQ();
        List<RFQGenerationModel> GetRFQPendingApproval();
        List<RFQApprovalTransactions> GetRFQTransactions();
        List<POApprovalTransactions> GetPOTransactions();
        List<Signature> GetSignatures();
        IEnumerable<User> GetApprovalRoles_Users();
        bool CreateSignature(VendorModel model, out string Message);
        bool ActivateSignature(VendorModel model, out string Message);
    }
}
