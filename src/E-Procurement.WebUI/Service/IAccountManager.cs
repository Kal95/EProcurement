using System.Collections.Generic;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.AccountModel;

namespace E_Procurement.WebUI.Models.Service
{
    public interface IAccountManager
    {
        Task<bool> CheckPasswordAsync(User user, string password);
       // Task<(bool Succeeded, string[] Error)> CreateRoleAsync(Role role, IEnumerable<string> claims);
        Task<(bool Succeeded, string[] Error)> CreatePasswordlessUserAsync(User user, IEnumerable<string> roles);
        Task<(bool Succeeded, string[] Error)> CreateUserAsync(User user, IEnumerable<string> roles, string password);
        Task<(bool Succeeded, string[] Error)> DeleteRoleAsync(Role role);
        Task<(bool Succeeded, string[] Error)> DeleteRoleAsync(string roleName);
        Task<(bool Succeeded, string[] Error)> DeleteUserAsync(long userId);
        Task<(bool Succeeded, string[] Error)> DeleteUserAsync(User user);
        Task<Role> GetRoleByIdAsync(long roleId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<Role> GetRoleLoadRelatedAsync(string roleName);
        Task<List<Role>> GetRolesLoadRelatedAsync(int page, int pageSize);
        Task<(User User, string[] Role)> GetUserAndRolesAsync(long userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(long userId);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<(bool Succeeded, string[] Error)> ResetPasswordAsync(User user, string newPassword);
        Task<bool> TestCanDeleteRoleAsync(long roleId);
        Task<(bool Succeeded, string[] Error)> UpdatePasswordAsync(User user, string currentPassword, string newPassword);
       // Task<(bool Succeeded, string[] Error)> UpdateRoleAsync(Role role, IEnumerable<string> claims);
        Task<(bool Succeeded, string[] Error)> UpdateUserAsync(User user);
        Task<(bool Succeeded, string[] Error)> UpdateUserAsync(User user, IEnumerable<string> roles);
        //Task<List<(User User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize);
        //Task<List<UserViewModel>> GetUsersAndRolesAsync(int page, int pageSize, string sortOrder, string sortValue);
    
    }
}