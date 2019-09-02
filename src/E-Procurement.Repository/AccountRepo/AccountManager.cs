using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace E_Procurement.Repository.AccountRepo
{
    public class AccountManager : IAccountManager
    {
        private readonly EProcurementContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ISMTPService _emailSender;
        private IConfiguration _config;

        public object SystemConstants { get; private set; }

        public AccountManager(
            EProcurementContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
             ISMTPService emailSender,
             IConfiguration config,
            IHttpContextAccessor httpAccessor)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _config = config;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }



        public async Task<(User User, string[] Role)> GetUserAndRolesAsync(long userId)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Where(u => u.Id == userId)
                .SingleOrDefaultAsync();

            if (user == null)
                return (null, null);

            var userRoleIds = user.Roles.Select(r => r.RoleId).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToArrayAsync();



            return (user, roles);
        }



        public async Task<List<(User User, string[])>> GetUsersAndRolesAsync()
        {
            IQueryable<User> usersQuery = _context.Users
                .Include(u => u.Roles)
                .OrderBy(u => u.UserName);

            //if (page != -1)
            //    usersQuery = usersQuery.Skip((page - 1) * pageSize);

            //if (pageSize != -1)
            //    usersQuery = usersQuery.Take(pageSize);

            var users = await usersQuery.ToListAsync();

            var userRoleIds = users.SelectMany(u => u.Roles.Select(r => r.RoleId)).ToList();

            var roles = await _context.Roles
                .Where(r => userRoleIds.Contains(r.Id))
                .ToArrayAsync();

            return users.Select(u => (u,
                roles.Where(r => u.Roles.Select(ur => ur.RoleId).Contains(r.Id)).Select(r => r.Name).ToArray()))
                .ToList();
        }


        public async Task<(bool Succeeded, string[] Error)> CreatePasswordlessUserAsync(User user, IEnumerable<string> roles)
        {
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            user = await _userManager.FindByNameAsync(user.Email);

            try
            {
                result = await _userManager.AddToRolesAsync(user, roles.Distinct());
            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }

            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return (false, result.Errors.Select(e => e.Description).ToArray());
            }

            return (true, new string[] { });
        }
        public async Task<bool> CreateUserAsync(User user, string password, string Role)
        {
            var result = await _userManager.CreateAsync(user, password);
            //if (!result.Succeeded)
            //    return (false);
            
            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return false;
            }


            user = await _userManager.FindByEmailAsync(user.Email);

            try
            {
                result = await _userManager.AddToRoleAsync(user, Role.ToUpper());
                if (result.Succeeded)
                {
                    if(Role.Contains("Vendor"))
                    {
                        Vendor vendor = new Vendor();
                        vendor.UserId = user.Id;
                        vendor.Email = user.Email;
                        //vendor.CountryId = 2;
                        //vendor.StateId = 2;
                        //vendor.BankId = 2;
                        _context.Vendors.Add(vendor);
                        //var commandText = "INSERT Vendors (UserId,Email) VALUES (@UserId,@Email)";
                        //var UserId = new SqlParameter("@UserId", user.Id);
                        //var Email = new SqlParameter("@Email", user.Email);
                        //var exec=await _context.Database.ExecuteSqlCommandAsync(commandText, UserId, Email);
                    }
                }

                var requisitionURL = _config.GetSection("ExternalAPI:RequisitionURL").Value;

                var subject = "SIGNUP NOTIFICATION";
                var message = "</br><b> Dear </b>" + user.FullName;
                message += "</br><b> Your have been registered successful on Cyberspace E-procurement Portal.</br>";
                message += "</br>Kindly, log in via <a href='" + requisitionURL + "' >"+ requisitionURL +" </a> and validate the required documents.";
                message += "</br><U>LOGIN DETAILS </U>";
                message += "</br>Email :  " + user.Email;
                message += "</br>Password :  " + password;
                message += "</br>Please do change your password upon login.";
                message += "</br>Regards";

                await _emailSender.SendEmailAsync(user.Email, subject, message, "");

                //_context.SaveChanges();

            }
            catch
            {
                await DeleteUserAsync(user);
                throw;
            }
            
            if (!result.Succeeded)
            {
                await DeleteUserAsync(user);
                return false;
            }

            return (true);
        }


        public async Task<(bool Succeeded, string[] Error)> UpdateUserAsync(User user)
        {
            return await UpdateUserAsync(user, null);
        }


        public async Task<(bool Succeeded, string[] Error)> UpdateUserAsync(User user, IEnumerable<string> roles)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());


            if (roles != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var enumerable = roles as string[] ?? roles.ToArray();
                var rolesToRemove = userRoles.Except(enumerable).ToArray();
                var rolesToAdd = enumerable.Except(userRoles).Distinct().ToArray();

                if (rolesToRemove.Any())
                {
                    result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }

                if (rolesToAdd.Any())
                {
                    result = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!result.Succeeded)
                        return (false, result.Errors.Select(e => e.Description).ToArray());
                }
            }

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Error)> ResetPasswordAsync(User user, string newPassword)
        {
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, new string[] { });
        }

        public async Task<(bool Succeeded, string[] Error)> UpdatePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description).ToArray());

            return (true, new string[] { });
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                if (!_userManager.SupportsUserLockout)
                    await _userManager.AccessFailedAsync(user);

                return false;
            }

            return true;
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            var checkEmail = await _userManager.FindByEmailAsync(email);
            if (checkEmail != null)
            {
                return true;
            }

            return false;
        }



        public async Task<(bool Succeeded, string[] Error)> DeleteUserAsync(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user != null)
                return await DeleteUserAsync(user);

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Error)> DeleteUserAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }
        
        public async Task<Role> GetRoleByIdAsync(long roleId)
        {
            return await _roleManager.FindByIdAsync(roleId.ToString());
        }


        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }


        public async Task<Role> GetRoleLoadRelatedAsync(string roleName)
        {
            var role = await _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.Users)
                .Where(r => r.Name == roleName)
                .SingleOrDefaultAsync();

            return role;
        }


        public async Task<List<Role>> GetRolesLoadRelatedAsync(int page, int pageSize)
        {
            IQueryable<Role> rolesQuery = _context.Roles
                .Include(r => r.Claims)
                .Include(r => r.Users)
                .OrderBy(r => r.Name);

            if (page != -1)
                rolesQuery = rolesQuery.Skip((page - 1) * pageSize);

            if (pageSize != -1)
                rolesQuery = rolesQuery.Take(pageSize);

            var roles = await rolesQuery.ToListAsync();

            return roles;
        }


        public async Task<bool> CreateRoleAsync(Role role)
        {
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                return (false);

           

            return (true);
        }
        public async Task<bool> UpdateRoleAsync(Role role)
        {
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
                return (false);



            return (true);
        }

        public async Task<bool> AssignUserRoleAsync(string Id, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return false;


            // remove user from all roles
            var allroles = await GetRoles();
            //List<string> selectecRoles = new List<string>();

            foreach (var role in allroles)
            {
                var isUserinRole = await _userManager.IsInRoleAsync(user, role.Name);
                if (isUserinRole)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }
            //var removeRoles = await _userManager.RemoveFromRolesAsync(user, allroles.Select(r => r.Name).ToArray());
            //if (!removeRoles.Succeeded)
            //    return false;

            var addRoleResult = await _userManager.AddToRolesAsync(user, roles.ToArray<string>());
            
            if (!addRoleResult.Succeeded)
                return (false);



            return (true);
        }

        //public async Task<(bool Succeeded, string[] Error)> UpdateRoleAsync(Role role, IEnumerable<string> claims)
        //{
        //    var enumerable = claims as string[] ?? claims.ToArray();

        //        string[] invalidClaims = enumerable.Where(c => ApplicationPermissions.GetPermissionByValue(c) == null).ToArray();
        //        if (invalidClaims.Any())
        //            return (false, new[] { "The following claim types are invalid: " + string.Join(", ", invalidClaims) });



        //    var result = await _roleManager.UpdateAsync(role);
        //    if (!result.Succeeded)
        //        return (false, result.Errors.Select(e => e.Description).ToArray());



        //        var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(c => c.Type == CustomClaimTypes.Permission);
        //        var roleClaims1 = roleClaims as Claim[] ?? roleClaims.ToArray();
        //        var roleClaimValues = roleClaims1.Select(c => c.Value).ToArray();

        //        var claimsToRemove = roleClaimValues.Except(enumerable).ToArray();
        //        var claimsToAdd = enumerable.Except(roleClaimValues).Distinct().ToArray();

        //        if (claimsToRemove.Any())
        //        {
        //            foreach (string claim in claimsToRemove)
        //            {
        //                result = await _roleManager.RemoveClaimAsync(role, roleClaims1.FirstOrDefault(c => c.Value == claim));
        //                if (!result.Succeeded)
        //                    return (false, result.Errors.Select(e => e.Description).ToArray());
        //            }
        //        }

        //        if (claimsToAdd.Any())
        //        {
        //            foreach (string claim in claimsToAdd)
        //            {
        //                result = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, ApplicationPermissions.GetPermissionByValue(claim)));
        //                if (!result.Succeeded)
        //                    return (false, result.Errors.Select(e => e.Description).ToArray());
        //            }
        //        }


        //    return  (true, new string[] { });
        //}


        public async Task<bool> TestCanDeleteRoleAsync(long roleId)
        {
            return !await _context.UserRoles.Where(r => r.RoleId == roleId).AnyAsync();
        }


        public async Task<(bool Succeeded, string[] Error)> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role != null)
                return await DeleteRoleAsync(role);

            return (true, new string[] { });
        }


        public async Task<(bool Succeeded, string[] Error)> DeleteRoleAsync(Role role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return (result.Succeeded, result.Errors.Select(e => e.Description).ToArray());
        }

  

        

      


     
        
    }
}
