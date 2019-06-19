using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.BankModel;

namespace E_Procurement.Repository.BankRepo
{
    public class BankRepository: IBankRepository
    {
        private readonly EProcurementContext _context;


        public BankRepository(EProcurementContext context)
        {
            _context = context;
        }
        public bool CreateBank(BankModel model, out string Message)
        {
            var confirm = _context.Banks.Where(x => x.BankName == model.BankName).Count();

            Bank bank = new Bank();

            if (confirm == 0)
            {

                bank.BankName = model.BankName;

                bank.SortCode = model.SortCode;

                bank.IsActive = true;

                bank.CreatedBy = model.CreatedBy;

                bank.DateCreated = DateTime.Now;

                _context.Add(bank);

                _context.SaveChanges();

                Message = "Bank created successfully";

                return true;
            }
            else
            {
                Message = "Bank already exist";

                return false;
            }

        }

        public bool UpdateBank(BankModel model, out string Message)
        {

            var confirm = _context.Banks.Where(x => x.BankName == model.BankName && x.SortCode == model.SortCode && x.BankName == model.BankName && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.Banks.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Bank exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.BankName = model.BankName;

                oldEntry.SortCode = model.SortCode;

                oldEntry.IsActive = model.IsActive;

                oldEntry.UpdatedBy = model.UpdatedBy;

                oldEntry.LastDateUpdated = DateTime.Now;

                _context.SaveChanges();

                Message = "Bank updated successfully";

                return true;
            }
            else
            {
                Message = "Bank already exist";

                return false;
            }

        }

        public IEnumerable<Bank> GetBanks()
        {
            return _context.Banks.OrderByDescending(u => u.Id).ToList();
        }
    }
}

