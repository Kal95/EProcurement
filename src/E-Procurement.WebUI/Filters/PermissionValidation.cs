
using E_Procurement.Data.Entity;
using E_Procurement.Repository.PermissionRepo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_Procurement.WebUI.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]

    public class PermissionValidation : AuthorizeAttribute,IActionFilter
    {
        private readonly string[] permissions;
        public IPermissionRepository _permissionRepository { get; set; }
        /// <summary>
        /// Gets or sets the permissions for the users at both controller and action level
        /// </summary>
        public string Permissions { get; set; }

        public PermissionValidation(params string[] permissions)
        {
            this.permissions = permissions;
        }
        public bool UserHasRequiredPemission(ActionExecutingContext context)
        {

            bool hasRequiredPermission = false;
            try
            {
                var currentUser = context.HttpContext.User.Identity.Name;
                var userIdentity = (ClaimsIdentity)context.HttpContext.User.Identity;
                var claims = userIdentity.Claims;
                var roleClaimType = userIdentity.RoleClaimType;

                var roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
                
                var permissionsFromClaimsList = claims.Where(c => c.Type == "Permissions").Select(c => c.Value).ToList();

                //var permissionsForUser = await _permissionRepository.GetPermissionByRoleIdAsync(roles);



                //foreach (var claim in permissionsForUser)
                //{
                //    claims..Add(new Claim("Permissions", claim.PermissionName));
                //}


                List<string> requiredPermissionsEnumList = permissions.ToList();
                var matchingPermissions = requiredPermissionsEnumList.Select(r => r.ToString())
                  .Where(r => permissionsFromClaimsList.Any(p => p.Contains(r))).ToList();
                //permissionsFromClaimsList.Where(p => p.Any())
                if (matchingPermissions != null && matchingPermissions.Count > 0)
                {
                    hasRequiredPermission = true;
                }
            }
            catch (Exception ex)
            {

            }
            return hasRequiredPermission;

        }

     
            public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var hasRequiredPemission =  UserHasRequiredPemission(context);
            if (!hasRequiredPemission)
            {
                // terminate request pipeline by setting Result
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Permission", action = "AccessDenied" }));

                context.Result.ExecuteResultAsync(context);

                //context.Result = new ContentResult()
                //{
                //    Content = "Unauthorized. User does not have the reqired permission",
                //    StatusCode = 401,
                //    ContentType = "application/json"
                //};
            }
          
        }
    }
}
