using System.Collections.Generic;
using System.Threading.Tasks;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.AccountRepo
{
    public interface IAccountManager : IDependencyRegister
    {
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<(bool Succeeded, string[] Error)> CreatePasswordlessUserAsync(User user, IEnumerable<string> roles);
        Task<bool> CreateUserAsync(User user, string password, string Role);
        Task<(bool Succeeded, string[] Error)> DeleteRoleAsync(Role role);
        Task<(bool Succeeded, string[] Error)> DeleteRoleAsync(string roleName);
        Task<(bool Succeeded, string[] Error)> DeleteUserAsync(long userId);
        Task<(bool Succeeded, string[] Error)> DeleteUserAsync(User user);
        Task<IEnumerable<User>> GetUsers();
        Task<Role> GetRoleByIdAsync(long roleId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<Role> GetRoleLoadRelatedAsync(string roleName);
        Task<List<Role>> GetRolesLoadRelatedAsync(int page, int pageSize);
        Task<(User User, string[] Role)> GetUserAndRolesAsync(long userId);        
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<(bool Succeeded, string[] Error)> ResetPasswordAsync(User user, string newPassword);
        Task<IEnumerable<Role>> GetRoles();
        Task<bool> TestCanDeleteRoleAsync(long roleId);
        Task<bool> IsEmailExistAsync(string email);
        Task<(bool Succeeded, string[] Error)> UpdatePasswordAsync(User user, string currentPassword, string newPassword);
       // Task<(bool Succeeded, string[] Error)> UpdateRoleAsync(Role role, IEnumerable<string> claims);
        Task<(bool Succeeded, string[] Error)> UpdateUserAsync(User user);
        Task<(bool Succeeded, string[] Error)> UpdateUserAsync(User user, IEnumerable<string> roles);
        Task<bool> AssignUserRoleAsync(string Id, List<string> roles);
        //Task<List<(User User, string[] Roles)>> GetUsersAndRolesAsync(int page, int pageSize);
        //Task<List<UserViewModel>> GetUsersAndRolesAsync(int page, int pageSize, string sortOrder, string sortValue);

    }
}