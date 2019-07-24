using E_Procurement.Data;
using E_Procurement.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Repository.Dtos;
using Microsoft.AspNetCore.Identity;
using E_Procurement.Repository.AccountRepo;

namespace E_Procurement.Repository.PermissionRepo
{
   public class PermissionRepository : IPermissionRepository
    {
        private readonly EProcurementContext _context;
        private readonly IAccountManager _accountManager;

        public PermissionRepository(EProcurementContext context,
                                IAccountManager accountManager )
        {
            _context = context;
            _accountManager = accountManager;
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
            var permission = await _context.Permissions.Where(x=>x.Id==Id).FirstAsync();
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
                var currentper = _context.PermissionRoles.Where(x => x.RoleId == Id);
                if(currentper.Count() > 0)
                {
                    _context.PermissionRoles.RemoveRange(currentper);
                }
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

        public async Task<IEnumerable<RolePermissionsModel>> GetPermissionByRoleIdAsync(List<int> RoleId)
        {
            
                var rolePermission = await _context.Permissions
                                .Join(
                                    _context.PermissionRoles,
                                    per => per.Id,
                                    roles => roles.PermissionId,
                                    (p, proles) => new RolePermissionsModel
                                    {
                                        PermissionName = p.Name,
                                        RoleId = proles.RoleId
                                    }
                                ).Where(x=> RoleId.Contains(x.RoleId)).Distinct().ToListAsync();
                return rolePermission;
           
        }
        public async Task<IEnumerable<RolePermissionsModel>> GetPermissionByRoleIdAsync(int RoleId)
        {

            var rolePermission = await _context.Permissions
                            .Join(
                                _context.PermissionRoles,
                                per => per.Id,
                                roles => roles.PermissionId,
                                (p, proles) => new RolePermissionsModel
                                {
                                    PermissionName = p.Name,
                                    RoleId = proles.RoleId
                                }
                            ).Where(x => RoleId==x.RoleId).Distinct().ToListAsync();
            return rolePermission;

        }


    }
}
