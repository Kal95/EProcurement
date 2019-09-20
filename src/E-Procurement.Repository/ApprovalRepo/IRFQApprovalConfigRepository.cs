using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.ApprovalRepo
{
    public interface IRfqApprovalConfigRepository : IDependencyRegister
    {
        Task<bool> CreateApprovalConfigAsync(RFQApprovalConfig permission);
        Task<IEnumerable<RFQApprovalConfig>> GetApprovalConfigAsync();
        Task<RFQApprovalConfig> GetApprovalConfigByIdAsync(int Id);
        Task<bool> UpdateApprovalConfigAsync(RFQApprovalConfig approvalConfig);
        Task<bool> DeleteApprovalConfigAsync(int Id);
        Task<IEnumerable<RFQApprovalConfig>> GetFinalApprovalAsync();

        Task<IEnumerable<RFQApprovalConfig>> CheckUserApprovalAsync(int UserId);
        Task<IEnumerable<ApprovalType>> GetApprovalType();
        bool CreateApprovalType(ApprovalTypeModel model, out string Message);
        bool UpdateApprovalType(ApprovalTypeModel model, out string Message);
        IEnumerable<ApprovalType> GetApprovalTypes();
    }
       
}
