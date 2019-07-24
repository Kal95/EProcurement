using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.PermissionRepo;
using E_Procurement.WebUI.Models.PermissionModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    public class PermissionController :  BaseController
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;

        public PermissionController(IPermissionRepository permissionRepository, IMapper mapper, IAccountManager accountManager)
        {
            _permissionRepository = permissionRepository;
            _accountManager = accountManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var permissionList = await _permissionRepository.GetPermissionAsync();

            List<PermissionViewModel> permissions = _mapper.Map<List<PermissionViewModel>>(permissionList);

            return View(permissions);
        }

    



        public IActionResult CreatePermission()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission(PermissionViewModel permission)
        {
            if (ModelState.IsValid)
            {

                var mappedPermission = _mapper.Map<Permission>(permission);

                var result = await _permissionRepository.CreatePermissionAsync(mappedPermission);

                if (result)
                {
                    Alert("Permission created successfully.", NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    Alert("SomeProblems were encountered while trying to perform operation. <br> Please try again.", NotificationType.error);
                }


            }

            return View(permission);
        }
        public async Task<IActionResult> EditPermission(int Id)
        {
            try
            {
                var permission = await _permissionRepository.GetPermissionByIdAsync(Id);

                PermissionViewModel per = _mapper.Map<PermissionViewModel>(permission);
                if(permission == null)
                {
                   Alert("Invalid Permission selected.", NotificationType.error);
                   return RedirectToAction("Index");
                }
                return View(per);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> EditPermission(PermissionViewModel permission)
        {

            if (ModelState.IsValid)
            {
                var getPermission = await _permissionRepository.GetPermissionByIdAsync(permission.Id);
                if (getPermission == null)
                    return View(getPermission);
                getPermission.Name = permission.Name;
                getPermission.Code = permission.Code;
                
                var result = await _permissionRepository.UpdatePermissionAsync(getPermission);

              
                if (result)
                {
                    Alert("Permission updated successfully.", NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    Alert("SomeProblems were encountered while trying to perform operation. Please try again.", NotificationType.error);
                }
            }
            return View(permission);
        }

        public async Task<IActionResult> DeletePermission(int Id)
        {
          
                var result = await _permissionRepository.DeletePermissionAsync(Id);

            if (result)
            {
                Alert("Permission deleted successfully.", NotificationType.success);
                return RedirectToAction("Index");
            }
            else
            {
                Alert("SomeProblems were encountered while trying to perform operation.  Please try again.", NotificationType.error);
            }

            return View("Index");
        }


        public async Task<IActionResult> AssignRolePermissions()
        {
            try
            {
                var permissions = await _permissionRepository.GetPermissionAsync();
                var roles = await _accountManager.GetRoles();



                var rolesList = roles.Select(a => new SelectListItem()
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

                List<RolePermissionViewModel> PermissionList = new List<RolePermissionViewModel>();
                //foreach (var item in permissions)
                //{
                //    PermissionList.Add(new RolePermissionViewModel { PermissionName = item.Name, PermissionId = item.Id });
                //}

                ViewBag.roles = rolesList;
                ViewBag.permission = PermissionList;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateRolePermissions(string RoleId, List<RolePermissionViewModel> RolePermission)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    List<RolePermissionViewModel> rolesPermission = new List<RolePermissionViewModel>();
                    if (RolePermission != null)
                    {
                        rolesPermission = RolePermission.Where(x => x.SelectedPermission == true).ToList();
                    }

                    List<int> selectecPermission = new List<int>();

                    foreach (var role in rolesPermission)
                    {
                        selectecPermission.Add(role.PermissionId);
                    }


                    var result = await _permissionRepository.AssignRolePermissionAsync(int.Parse(RoleId), selectecPermission);
                    

                    if (result)
                    {
                        Alert("Permission assigned successfully.", NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Alert("SomeProblems were encountered while trying to perform operation.  Please try again.", NotificationType.error);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AssignRolePermissions(string RoleId, List<RolePermissionViewModel> RolePermission)
        {
           
            try
            {
                if (string.IsNullOrEmpty(RoleId))
                {
                    Alert("Please select Role.", NotificationType.warning);
                    return View();
                }

                    if (!string.IsNullOrEmpty(RoleId))
                {
                    var permissions = await _permissionRepository.GetPermissionAsync();
                    var roles = await _accountManager.GetRoles();

                    var currentPermissions = await _permissionRepository.GetPermissionByRoleIdAsync(Convert.ToInt16(RoleId));


                    var rolesList = roles.Select(a => new SelectListItem()
                    {
                        Value = a.Id.ToString(),
                        Text = a.Name
                    }).ToList();

                    List<RolePermissionViewModel> PermissionList = new List<RolePermissionViewModel>();
                    foreach (var item in permissions)
                    {
                        var isPermissionAssigned = currentPermissions.Any(x=> x.PermissionName.Contains(item.Name));
                        if (isPermissionAssigned)
                        {
                            PermissionList.Add(new RolePermissionViewModel { SelectedPermission = true, PermissionName = item.Name, PermissionId = item.Id });
                        }
                        else
                        {
                            PermissionList.Add(new RolePermissionViewModel { PermissionName = item.Name, PermissionId = item.Id });
                        }
                    }

                    ViewBag.roles = rolesList;
                    ViewBag.permission = PermissionList;

                    return View();
                }


               
                
            }
            catch (Exception ex)
            {
                Alert("SomeProblems were encountered while trying to perform operation.  Please try again.", NotificationType.error);
            }
            return View();
        }
        public ActionResult AccessDenied()
        {
            return View();
        }


    }
}