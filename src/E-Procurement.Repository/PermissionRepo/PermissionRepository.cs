using E_Procurement.Data;
using E_Procurement.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace E_Procurement.Repository.PermissionRepo
{
   public class PermissionRepository : IPermissionRepository
    {
        private readonly EProcurementContext _context;

        public PermissionRepository(EProcurementContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePermissionAsync(Permission permission)
        {
            if (permission != null)
            {

                await _context.AddAsync(permission);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Permission>> GetPermissionAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task<Permission> GetPermissionByIdAsync(int Id)
        {
            return await _context.Permissions.FindAsync(Id);
        }

        public async Task<bool> UpdatePermissionAsync(Permission permission)
        {
            if (permission != null)
            {
                _context.Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> DeletePermissionAsync(int Id)
        {
            var permission = _context.Permissions.FindAsync(Id);
            if (permission != null)
            {
                 _context.Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


        public async Task<bool> AssignRolePermissionAsync(int Id, List<int> permission)
        {
          
            try
            {
                List<PermissionRole> rolePermissions = new List<PermissionRole>();

                foreach (var model in permission)
                {
                    rolePermissions.Add(new PermissionRole
                    {
                        RoleId = Id,
                        PermissionId = model
                    });
                }

                await _context.PermissionRoles.AddRangeAsync(rolePermissions);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return false;
            }





            return (true);
        }

    }
}
