using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.PoAcceptanceRepo
{
    public interface IPOAcceptanceRepository : IDependencyRegister
    {
        Task<IEnumerable<POAcceptanceModel>> GetAllPO();
        Task<POAcceptanceModel> GetPODetails(int Id);
       // List<RFQDetails> GetRfqDetails();
        bool UpdatePO(int Id,  out string Message);

    }
}
