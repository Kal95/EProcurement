using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class RfqApprovalConfigController : Controller
    {
        private readonly IRfqApprovalConfigRepository _RfqApprovalConfigRepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;

        public RfqApprovalConfigController(IRfqApprovalConfigRepository RfqApprovalConfigRepository, IMapper mapper, IAccountManager accountManager)
        {
            _RfqApprovalConfigRepository = RfqApprovalConfigRepository;
            _accountManager = accountManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var RfqApprovalConfigList = await _RfqApprovalConfigRepository.GetApprovalConfigAsync();

            List<RFQApprovalConfigModel> RfqApprovalConfigs = _mapper.Map<List<RFQApprovalConfigModel>>(RfqApprovalConfigList);

            return View(RfqApprovalConfigs);
        }

        public async Task<IActionResult> Create()
        {
    
            var user = await _accountManager.GetUsers();
            
            var UserList = user.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.FullName + " (" + a.Email +")"
            }).ToList();

            ViewBag.users = UserList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RFQApprovalConfigModel RfqApprovalConfig)
        {
            if (ModelState.IsValid)
            {
                // to reload the list
                var users = await _accountManager.GetUsers();
                    var UserList = users.Select(a => new SelectListItem()
                    {
                        Value = a.Id.ToString(),
                        Text = a.FullName + " (" + a.Email + ")"
                    }).ToList();

                    ViewBag.users = UserList;

                var user = await _accountManager.GetUserByIdAsync(RfqApprovalConfig.UserId);
                RfqApprovalConfig.Email = user.Email;

                var userApprovalCheck = await _RfqApprovalConfigRepository.CheckUserApprovalAsync(RfqApprovalConfig.UserId);
                if (userApprovalCheck != null)
                {
                    ModelState.AddModelError("", "Can not create multiple approval level for the selected user!!");
                    return View(RfqApprovalConfig);
                }

                var finalApprovalCheck = await _RfqApprovalConfigRepository.GetFinalApprovalAsync();
                if (finalApprovalCheck != null)
                {
                    ModelState.AddModelError("", "Can not assign final approval to multiple user!!");
                    return View(RfqApprovalConfig);
                }

                var mappedRfqApprovalConfig = _mapper.Map<RFQApprovalConfig>(RfqApprovalConfig);
                var result = await _RfqApprovalConfigRepository.CreateApprovalConfigAsync(mappedRfqApprovalConfig);

                if (result)
                {
                    //await _userManager.AddToRoleAsync(user, registerViewModel.UserRoles);
                    return RedirectToAction("Index");
                }


            }

            return View(RfqApprovalConfig);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                var user = await _accountManager.GetUsers();
                
                var UserList = user.Select(a => new SelectListItem()
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName + " (" + a.Email + ")"
                }).ToList();

                ViewBag.users = UserList;

                var RfqApprovalConfig = await _RfqApprovalConfigRepository.GetApprovalConfigByIdAsync(Id);

                RFQApprovalConfigModel per = _mapper.Map<RFQApprovalConfigModel>(RfqApprovalConfig);
                return View(per);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RFQApprovalConfigModel RfqApprovalConfig)
        {

            if (ModelState.IsValid)
            {
                var user = await _accountManager.GetUserByIdAsync(RfqApprovalConfig.UserId);
                RfqApprovalConfig.Email = user.Email;
                var getRfqApprovalConfig = await _RfqApprovalConfigRepository.GetApprovalConfigByIdAsync(RfqApprovalConfig.Id);
                if (getRfqApprovalConfig == null)
                    return View(getRfqApprovalConfig);
                getRfqApprovalConfig.ApprovalLevel = RfqApprovalConfig.ApprovalLevel;
                getRfqApprovalConfig.Email = RfqApprovalConfig.Email;
                getRfqApprovalConfig.IsFinalLevel = RfqApprovalConfig.IsFinalLevel;

                var result = await _RfqApprovalConfigRepository.UpdateApprovalConfigAsync(getRfqApprovalConfig);

                if (result)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(RfqApprovalConfig);
        }

        public async Task<IActionResult> Delete(int Id)
        {
          
                var result = await _RfqApprovalConfigRepository.DeleteApprovalConfigAsync(Id);

                if (result)
                {
                    return RedirectToAction("Index");
                }

            return View("Index");
        }

        
    }
}