using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.DINRepo
{
    public interface IGRNRepository : IDependencyRegister
    {
        Task<bool> GRNGenerationAsync(RFQGenerationModel rfq);
        Task<IEnumerable<RFQGenerationModel>> GetPOAsync();
        Task<POGeneration> GetPOByIdAsync(int Id);
        Task<IEnumerable<RFQGenerationModel>> GetApprovedRFQAsync();
        Task<RFQGenerationModel> GetInvoiceDetailsAsync(int RFQId);
    }
}
