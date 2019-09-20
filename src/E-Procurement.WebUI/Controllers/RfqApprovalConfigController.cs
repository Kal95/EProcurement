using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class RfqApprovalConfigController : BaseController
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
            var ApprovalType = await _RfqApprovalConfigRepository.GetApprovalType();
            var user = await _accountManager.GetUsers();
            
            var UserList = user.Select(a => new SelectListItem()
            {
                Value = a.Id.ToString(),
                Text = a.FullName + " (" + a.Email +")"
            }).ToList();

            var ApprovalTypeList = ApprovalType.Select(a => new SelectListItem()
            {
                Value = a.ApprovalTypeId.ToString(),
                Text = a.ApprovalTypeName
            }).ToList();

            ViewBag.users = UserList;
            ViewBag.ApprovalType = ApprovalTypeList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RFQApprovalConfigModel RfqApprovalConfig)
        {
            if (ModelState.IsValid)
            {
                // to reload the list
                var ApprovalType = await _RfqApprovalConfigRepository.GetApprovalType();
                var users = await _accountManager.GetUsers();

                var UserList = users.Select(a => new SelectListItem()
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName + " (" + a.Email + ")"
                }).ToList();

                var ApprovalTypeList = ApprovalType.Select(a => new SelectListItem()
                {
                    Value = a.ApprovalTypeId.ToString(),
                    Text = a.ApprovalTypeName
                }).ToList();

                ViewBag.users = UserList;
                ViewBag.ApprovalType = ApprovalTypeList;
           
                var user = await _accountManager.GetUserByIdAsync(RfqApprovalConfig.UserId);
                RfqApprovalConfig.Email = user.Email;

                var userApprovalCheck = await _RfqApprovalConfigRepository.CheckUserApprovalAsync(RfqApprovalConfig.UserId);
                if (userApprovalCheck.Count() > 2)
                {
                    Alert("Can not create multiple approval level for the selected user!! Please try again.", NotificationType.info);
                    return View(RfqApprovalConfig);
                }

                var finalApprovalCheck = await _RfqApprovalConfigRepository.GetFinalApprovalAsync();
                if (finalApprovalCheck.Count() > 2)
                {
                    Alert("Cannot create new approval level because Final Approval has already been assigned!! Please try again.", NotificationType.info);
                    return View(RfqApprovalConfig);
                }

                var mappedRfqApprovalConfig = _mapper.Map<RFQApprovalConfig>(RfqApprovalConfig);
                var result = await _RfqApprovalConfigRepository.CreateApprovalConfigAsync(mappedRfqApprovalConfig);

              
                if (result)
                {
                    Alert("Approval config created successfully.", NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    Alert("Some problems were encountered while trying to perform operation. </br> Please try again.", NotificationType.error);
                }

            }
            Alert("Some entry fields were are not validated. </br> Kindly, review all entry field.", NotificationType.error);
            return View(RfqApprovalConfig);
        }
        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                var ApprovalType = await _RfqApprovalConfigRepository.GetApprovalType();
                var user = await _accountManager.GetUsers();

                var UserList = user.Select(a => new SelectListItem()
                {
                    Value = a.Id.ToString(),
                    Text = a.FullName + " (" + a.Email + ")"
                }).ToList();

                var ApprovalTypeList = ApprovalType.Select(a => new SelectListItem()
                {
                    Value = a.ApprovalTypeId.ToString(),
                    Text = a.ApprovalTypeName
                }).ToList();

                ViewBag.users = UserList;
                ViewBag.ApprovalType = ApprovalTypeList;
               

                var RfqApprovalConfig = await _RfqApprovalConfigRepository.GetApprovalConfigByIdAsync(Id);

                RFQApprovalConfigModel per = _mapper.Map<RFQApprovalConfigModel>(RfqApprovalConfig);
                return View(per);
            }
            catch (Exception ex)
            {
                Alert("Some problems were encountered while trying to perform operation. </br> Please try again.", NotificationType.error);
                return RedirectToAction("Index");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RFQApprovalConfigModel RfqApprovalConfig)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    Alert("Some problems were encountered while trying to perform operation. Please try again.", NotificationType.error);
                    return View(RfqApprovalConfig);
                }
                if (ModelState.IsValid)
                {
                    var user = await _accountManager.GetUserByIdAsync(RfqApprovalConfig.UserId);
                    RfqApprovalConfig.Email = user.Email;
                    var getRfqApprovalConfig = await _RfqApprovalConfigRepository.GetApprovalConfigByIdAsync(RfqApprovalConfig.Id);
                    if (getRfqApprovalConfig == null)
                    {
                        Alert("Invalid approval config. Please try again.", NotificationType.error);
                        return View(getRfqApprovalConfig);
                    }

                    getRfqApprovalConfig.UserId = RfqApprovalConfig.UserId;
                    getRfqApprovalConfig.ApprovalLevel = RfqApprovalConfig.ApprovalLevel;
                    getRfqApprovalConfig.Email = RfqApprovalConfig.Email;
                    getRfqApprovalConfig.IsFinalLevel = RfqApprovalConfig.IsFinalLevel;

                    var result = await _RfqApprovalConfigRepository.UpdateApprovalConfigAsync(getRfqApprovalConfig);

                    if (result)
                    {
                        Alert("Approval config updated successfully.", NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Alert("Some problems were encountered while trying to perform operation. Please try again.", NotificationType.error);
                        return View(RfqApprovalConfig);
                    }
                }
            }
            catch (Exception ex)
            {
                Alert("Some problems were encountered while trying to perform operation. </br> Please try again.", NotificationType.error);
               
            }
            return View(RfqApprovalConfig);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            try
            { 
                var result = await _RfqApprovalConfigRepository.DeleteApprovalConfigAsync(Id);
            
                if (result)
                {
                    Alert("Approval Config deleted successfully.", NotificationType.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    Alert("Some problems were encountered while trying to perform operation. Please try again.", NotificationType.error);
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                Alert("Some problems were encountered while trying to perform operation. Please try again.", NotificationType.error);
            }
            return View("Index");
        }

        public ActionResult ApprovalTypeCreate()
        {
            return View();
        }

        // POST: Report/EvaluationPeriodCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovalTypeCreate(ApprovalTypeModel model)
        {
            try
            {
                string message;
                model.CreatedBy = User.Identity.Name;


                if (ModelState.IsValid)
                {

                    var status = _RfqApprovalConfigRepository.CreateApprovalType(model, out message);

                    if (status == true)
                    {

                        Alert("Approval Type Created Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Approval Type Already Exists", NotificationType.info);
                        return View(model);
                    }

                    return RedirectToAction("ApprovalTypeIndex", "RfqApprovalConfig");
                }
                else
                {
                    ViewBag.StatusCode = 2;
                    Alert("Approval Type Wasn't Created", NotificationType.error);

                    return View(model);

                }
            }

            catch (Exception)
            {

                return View("Error");
            }
        }

        // GET: Report/EvaluationPeriodEdit
        public ActionResult ApprovalTypeEdit(int ApprovalTypeId)
        {


            ApprovalTypeModel Model = new ApprovalTypeModel();

            try
            {

                var Period = _RfqApprovalConfigRepository.GetApprovalTypes().Where(u => u.ApprovalTypeId == ApprovalTypeId).FirstOrDefault();

                if (Period == null)
                {
                    Alert("This Approval Type Doesn't Exists", NotificationType.info);

                    return RedirectToAction("ApprovalTypeIndex", "RfqApprovalConfig");
                }

                Model.ApprovalTypeId = Period.ApprovalTypeId;

                Model.ApprovalTypeName = Period.ApprovalTypeName;


                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }

        }

        // POST: Report/EvaluationPeriodEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovalTypeEdit(ApprovalTypeModel Model)
        {
            try
            {
               
                if (ModelState.IsValid)
                {
                    string message;

                    var Period = _RfqApprovalConfigRepository.GetApprovalTypes().FirstOrDefault(u => u.ApprovalTypeId == Model.ApprovalTypeId);

                    if (Period == null) { Alert("This Approval Type Doesn't Exist", NotificationType.warning); return RedirectToAction("ApprovalTypeIndex", "RfqApprovalConfig"); }

                    Model.UpdatedBy = User.Identity.Name;

                    var status = _RfqApprovalConfigRepository.UpdateApprovalType(Model, out message);


                    if (status == true)
                    {
                        Alert("Approval Type Updated Successfully", NotificationType.success);

                    }

                    else
                    {
                        Alert("This Approval Type Already Exists", NotificationType.info);
                        return View(Model);
                    }

                    return RedirectToAction("ApprovalTypeIndex", "RfqApprovalConfig");
                }
                else
                {
                    ViewBag.StatusCode = 2;

                    Alert("Approval Type Wasn't Updated", NotificationType.error);

                    return View(Model);
                }
            }
            catch (Exception)
            {

                return View("Error");
            }
        }
        public ActionResult ApprovalTypeIndex()
        {
            try
            {
                List<ApprovalTypeModel> poModel = new List<ApprovalTypeModel>();

                var period = _RfqApprovalConfigRepository.GetApprovalTypes().ToList();

                //var RfqList = _rfqGenRepository.GetRfqGen().OrderBy(u => u.EndDate).ToList();
                var PeriodList = period.Select(x => new ApprovalTypeModel
                {
                    ApprovalTypeId = x.ApprovalTypeId,
                    ApprovalTypeName = x.ApprovalTypeName
                    
                });
                return View(PeriodList);
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

    }
}