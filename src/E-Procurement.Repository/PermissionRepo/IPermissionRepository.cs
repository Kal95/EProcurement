using E_Procurement.Data.Entity;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace E_Procurement.Repository.PermissionRepo
{
   public  interface IPermissionRepository : IDependencyRegister
    {
        Task<bool> CreatePermissionAsync(Permission permission);
        Task<IEnumerable<Permission>> GetPermissionAsync();
        Task<IEnumerable<RolePermissionsModel>> GetPermissionByRoleIdAsync(List<int> RoleId);
        Task<IEnumerable<RolePermissionsModel>> GetPermissionByRoleIdAsync(int RoleId);
        Task<Permission> GetPermissionByIdAsync(int Id);
        Task<bool> UpdatePermissionAsync(Permission permission);
        Task<bool> DeletePermissionAsync(int Id);
        Task<bool> AssignRolePermissionAsync(int Id, List<int> permission);
    }
}
