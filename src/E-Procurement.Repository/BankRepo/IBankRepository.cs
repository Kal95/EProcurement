using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.BankModel;

namespace E_Procurement.Repository.BankRepo
{
    public interface IBankRepository : IDependencyRegister
    {
        IEnumerable<Bank> GetBanks();
        bool CreateBank(BankModel model, out string Message);

        bool UpdateBank(BankModel model, out string Message);
    }
}
