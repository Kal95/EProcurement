using E_Procurement.Data;
using E_Procurement.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Repository.ApprovalRepo;

namespace E_Procurement.Repository.RfqApprovalConfigRepository
{
   public class RfqApprovalConfigRepository : IRfqApprovalConfigRepository
    {
        private readonly EProcurementContext _context;

        public RfqApprovalConfigRepository(EProcurementContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateApprovalConfigAsync(RFQApprovalConfig rfqApproval)
        {
            if (rfqApproval != null)
            {

                await _context.AddAsync(rfqApproval);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<RFQApprovalConfig>> GetApprovalConfigAsync()
        {
            return await _context.RfqApprovalConfigs.ToListAsync();
        }

        public async Task<IEnumerable<RFQApprovalConfig>> GetFinalApprovalAsync()
        {
            return await _context.RfqApprovalConfigs. Where(x=> x.IsFinalLevel == true).ToListAsync();
        }
        public async Task<IEnumerable<RFQApprovalConfig>> CheckUserApprovalAsync(int UserId)
        {
            return await _context.RfqApprovalConfigs.Where(x => x.UserId == UserId ).ToListAsync();
        }
        
        public async Task<RFQApprovalConfig> GetApprovalConfigByIdAsync(int Id)
        {
            return await _context.RfqApprovalConfigs.FindAsync(Id);
        }

        public async Task<bool> UpdateApprovalConfigAsync(RFQApprovalConfig rfqApprovalConfig)
        {
            if (rfqApprovalConfig != null)
            {
                _context.Update(rfqApprovalConfig);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteApprovalConfigAsync(int Id)
        {
            var rfqApprovalConfig = _context.RfqApprovalConfigs.Where(x=>x.Id == Id).FirstOrDefault();
            if (rfqApprovalConfig != null)
            {
                 _context.Remove(rfqApprovalConfig);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


      
    }
}
