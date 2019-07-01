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
        Task<IEnumerable<POAcceptanceViewModel>> GetAllPO();
        Task<POAcceptanceViewModel> GetPODetails(int Id);
       // List<RFQDetails> GetRfqDetails();
        bool UpdatePO(int Id, DateTime ExpectedDeliveryDate, out string Message);

    }
}
