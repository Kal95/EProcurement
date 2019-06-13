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
        Task<RFQGenerationModel> GetRFQDetailsAsync(int RFQId);


    }
}
