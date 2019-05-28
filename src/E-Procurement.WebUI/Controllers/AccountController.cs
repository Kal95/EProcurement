using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.WebUI.Models.AccountModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using E_Procurement.Repository.AccountRepo;
using AutoMapper;
using E_Procurement.Data.Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class AccountController : Controller
    {

        private readonly  IAccountManager _accountManager;
        private readonly IMapper _mapper;


        public AccountController(IAccountManager accountManager, IMapper mapper)
        {
            _accountManager = accountManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginViewModel)
        {
      
                if (ModelState.IsValid)
                {
                    var user = await _accountManager.GetUserByEmailAsync(loginViewModel.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Email/password not found");
                        return View(loginViewModel);
                    }
                    var result = await _accountManager.CheckPasswordAsync(user, loginViewModel.Password);
                    if (result)
                    {
                        //RedirectToAction("Index", "Home");
                        return RedirectToRoute("Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email/password not found");
                        return View(loginViewModel);
                    }
                }
                return View(loginViewModel);
       
        }

        #region"USER SECTION"
        [AllowAnonymous]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterViewModel user)
        {
            if (ModelState.IsValid)
            {
             
                    var mappedUser = _mapper.Map<User>(user);

                    var result = await _accountManager.CreateUserAsync(mappedUser, user.Password);

                    if (result)
                    {
                        //await _userManager.AddToRoleAsync(user, registerViewModel.UserRoles);

                        return RedirectToAction("Users");
                    }
         
           
            }

            return View(user);
        }

        public async Task<IActionResult> Users()
        {
           
                var users = await _accountManager.GetUsers();

                List<UserViewModel> userList = _mapper.Map<List<UserViewModel>>(users);
                return View(userList);
       
        }
  
        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                var users = await _accountManager.GetUserByIdAsync(id);

                RegisterViewModel user = _mapper.Map<RegisterViewModel>(users);
                return View(user);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(RegisterViewModel user)
        {
            
            if (ModelState.IsValid)
            {
                var getUser = await _accountManager.GetUserByIdAsync(user.Id);
                if (getUser == null)
                    return View(user);
                getUser.FirstName = user.FirstName;
                getUser.LastName = user.LastName;
                getUser.Department = user.Department;
                getUser.Unit = user.Unit;

               // var mappedUser = _mapper.Map<User>(user);
                getUser.SecurityStamp = Guid.NewGuid().ToString();
                var result = await _accountManager.UpdateUserAsync(getUser);
                
                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, registerViewModel.UserRoles);

                    return RedirectToAction("Users");
                }
            }
            return View(user);
        }

        public async Task<IActionResult> AssignUserRole()
        {
            try
            {
                var roles = await _accountManager.GetRoles();
                var users = await _accountManager.GetUsers();



                var userList = users.Select(a => new SelectListItem()
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName
                }).ToList();

                List<UserRoleViewModel> userRoleVM = new List<UserRoleViewModel>();
                foreach (var item in roles)
                {
                    userRoleVM.Add(new UserRoleViewModel { Role = item.Name });
                }

                ViewBag.roles = userRoleVM;
                ViewBag.users = userList;
                
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public async Task<IActionResult> AssignUserRole(string Id, List<UserRoleViewModel> UserRole)
        {

            if (ModelState.IsValid)
            {
                List<UserRoleViewModel> roles = new List<UserRoleViewModel>();
                if (UserRole != null)
                {
                    roles = UserRole.Where(x => x.SelectedRole == true).ToList();
                }
                //else
                //{
                //    roles.Add(new UserRoleViewModel { SelectedRole = true, Role = "Enroller" });
                //}

                List<string> selectecRoles = new List<string>();

                foreach (var role in roles)
                {
                    selectecRoles.Add(role.Role);
                }


                var result = await _accountManager.AssignUserRoleAsync(Id, selectecRoles);

                if (result)
                {
                    //await _userManager.AddToRoleAsync(user, registerViewModel.UserRoles);

                    return RedirectToAction("Users");
                }



            }
            return View();
            }
        #endregion




            #region"ROLE SECTION"
        [AllowAnonymous]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                var mappedRole = _mapper.Map<Role>(role);

                var result = await _accountManager.CreateRoleAsync(mappedRole);

                if (result)
                {
                    //await _userManager.AddToRoleAsync(user, registerViewModel.UserRoles);

                    return RedirectToAction("Roles");
                }


            }

            return View(role);
        }

        public async Task<IActionResult> Roles()
        {

            var roles = await _accountManager.GetRoles();

            List<RoleViewModel> roleList = _mapper.Map<List<RoleViewModel>>(roles);
            return View(roleList);

        }

        public async Task<IActionResult> EditRole(int id)
        {
            try
            {
                var role = await _accountManager.GetRoleByIdAsync(id);

                RoleViewModel appRole = _mapper.Map<RoleViewModel>(role);
                return View(appRole);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditRole(RoleViewModel role)
        {

            if (ModelState.IsValid)
            {
                var getRole = await _accountManager.GetRoleByIdAsync(role.Id);
                if (getRole == null)
                    return View(role);
                getRole.Name = role.Name;

                // var mappedUser = _mapper.Map<User>(user);
                var result = await _accountManager.UpdateRoleAsync(getRole);

                if (result)
                {
                    //await _userManager.AddToRoleAsync(user, registerViewModel.UserRoles);

                    return RedirectToAction("Roles");
                }
            }
            return View(role);
        }
        #endregion


    }
}