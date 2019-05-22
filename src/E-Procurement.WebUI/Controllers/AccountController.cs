using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Procurement.WebUI.Models.AccountModel;
using E_Procurement.WebUI.Models.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.WebUI.Controllers
{
    public class AccountController : Controller
    {

        private readonly  IAccountManager _accountManager;
  

        public AccountController(IAccountManager accountManager)
        {
            _accountManager = accountManager;
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
    }
}