using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;

namespace E_Procurement.Repository.BankRepo
{
    public class BankRepository: IBankRepository
    {
        private readonly EProcurementContext _context;


        public BankRepository(EProcurementContext context)
        {
            _context = context;
        }
        public bool CreateBank(string BankName, string SortCode, string UserId, out string Message)
        {
            var confirm = _context.Banks.Where(x => x.BankName == BankName).Count();

            Bank bank = new Bank();

            if (confirm == 0)
            {

                bank.BankName = BankName;

                bank.SortCode = SortCode;

                bank.IsActive = true;

                bank.CreatedBy = UserId;

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

        public bool UpdateBank(int Id, string BankName,string SortCode, bool IsActive, string UserId, out string Message)
        {

            var confirm = _context.Banks.Where(x => x.BankName == BankName && x.SortCode == SortCode && x.BankName == BankName && x.IsActive == IsActive).Count();

            var oldEntry = _context.Banks.Where(u => u.Id == Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Bank exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.BankName = BankName;

                oldEntry.SortCode = SortCode;

                oldEntry.IsActive = IsActive;

                oldEntry.UpdatedBy = UserId;

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

