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
        public async Task<IEnumerable<ApprovalType>> GetApprovalType()
        {
            return await _context.ApprovalTypes.ToListAsync();
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
        public bool CreateApprovalType(ApprovalTypeModel model, out string Message)
        {
            var confirm = _context.ApprovalTypes.Where(x => x.ApprovalTypeName == model.ApprovalTypeName).Count();

            ApprovalType approvalType = new ApprovalType();

            if (confirm == 0)
            {

                approvalType.ApprovalTypeName = model.ApprovalTypeName;

                approvalType.CreatedBy = model.CreatedBy;

                approvalType.DateCreated = DateTime.Now;

                _context.Add(approvalType);

                _context.SaveChanges();

                Message = "Approval Type created successfully";

                return true;
            }
            else
            {
                Message = "Approval Type already exist";

                return false;
            }

        }

        public bool UpdateApprovalType(ApprovalTypeModel model, out string Message)
        {

            var confirm = _context.ApprovalTypes.Where(x => x.ApprovalTypeName == model.ApprovalTypeName).Count();

            var oldEntry = _context.ApprovalTypes.Where(u => u.ApprovalTypeId == model.ApprovalTypeId).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Approval Type exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.ApprovalTypeName = model.ApprovalTypeName;

                oldEntry.UpdatedBy = model.UpdatedBy;

                oldEntry.LastDateUpdated = DateTime.Now;

                _context.ApprovalTypes.Update(oldEntry);
                _context.SaveChanges();

                Message = "Aproval Type updated successfully";

                return true;
            }
            else
            {
                Message = "Approval Type already exist";

                return false;
            }

        }

        public IEnumerable<ApprovalType> GetApprovalTypes()
        {
            return _context.ApprovalTypes.OrderByDescending(u => u.ApprovalTypeId).ToList();
        }


    }
}
