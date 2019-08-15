using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.Repository.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.ApprovalRepo
{
   public  interface IRfqApprovalRepository : IDependencyRegister
    {
        Task<IEnumerable<RFQGenerationModel>> GetRFQApprovalDueAsync();

        Task<IEnumerable<RFQGenerationModel>> GetRFQByVendorsAsync(int RFQId);

        Task<IEnumerable<RFQGenerationModel>> GetRFQPendingApprovalAsync();
        Task<IEnumerable<RFQGenerationModel>> GetRFQPendingApprovalByVendorsAsync(int RFQId);
        Task<RFQGenerationModel> GetRFQDetailsAsync(int RFQId, int VendorId);

        Task<bool> CreateRFQApprovalAsync(RFQGenerationModel rFQApproval);
        Task<bool> CreateRFQPendingApprovalAsync(RFQGenerationModel rFQApproval);


    }
}
