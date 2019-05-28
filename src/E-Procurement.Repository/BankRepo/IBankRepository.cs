using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.BankRepo
{
    public interface IBankRepository : IDependencyRegister
    {
        IEnumerable<Bank> GetBanks();
        bool CreateBank(string BankName, string SortCode, string UserId, out string Message);

        bool UpdateBank(int Id, string BankName, string SortCode, bool IsActive, string UserId, out string Message);
    }
}
