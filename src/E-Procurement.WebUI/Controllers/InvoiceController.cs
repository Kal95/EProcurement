using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.DINRepo;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.PORepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    [Authorize]
    public class InvoiceController : BaseController
    {
        //private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IPORepository _PORepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IDINRepository _dinRepository;
        private IHostingEnvironment _hostingEnv;

        public InvoiceController(IPORepository PORepository, 
                                IMapper mapper, 
                                IAccountManager accountManager, 
                                IRfqApprovalRepository RfqApprovalRepository,
                                IDINRepository dinRepository,
                                IHostingEnvironment hostingEnv)
        {
            _PORepository = PORepository;
            _accountManager = accountManager;
            _mapper = mapper;
            _RfqApprovalRepository = RfqApprovalRepository;
            _dinRepository = dinRepository;
            _hostingEnv = hostingEnv;

        }
        public async Task<IActionResult> Index()
        {
            var RfqApprovalList = await _dinRepository.GetPOAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }

        public async Task<IActionResult> PODetails(int id)
        {
            var RfqApprovalDetails = await _dinRepository.GetInvoiceDetailsAsync(id);

           
            //RfqApproval.RFQDetails = rqfDetails;
            if (RfqApprovalDetails == null)
                return View();

            return View(RfqApprovalDetails);
        }


        [HttpPost]
        public async Task<IActionResult> PODetails(RFQGenerationModel rfqApproval)
        {
            var RfqApprovalDetails = await _dinRepository.GetInvoiceDetailsAsync(rfqApproval.RFQId);
            try
            {
                if (string.IsNullOrEmpty(rfqApproval.RFQStatus) || rfqApproval.RFQStatus != "Approved")
                {
                    Alert("Can not upload invoice. Please try again later.", NotificationType.warning);
                    return View(RfqApprovalDetails);
                }
                if (ModelState.IsValid)
                {
                    if (rfqApproval.InvoiceFilePath != null && rfqApproval.InvoiceFilePath.Length > 0)
                    {
                        string webRootPath = _hostingEnv.WebRootPath;

                        var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                        var checkextension = Path.GetExtension(rfqApproval.InvoiceFilePath.FileName).ToLower();
                        var InvoiceFilePath = rfqApproval.PONumber + Path.GetExtension(rfqApproval.InvoiceFilePath.FileName);

                        if (!allowedExtensions.Contains(checkextension))
                        {
                            Alert("Invalid file extention.", NotificationType.error);
                            return View(RfqApprovalDetails);
                        }
                   

                        //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", imageFilePath);
                        var path = Path.Combine(webRootPath, "uploads","Invoice", InvoiceFilePath);

                        
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        using (Stream stream = new FileStream(path, FileMode.Create))
                        {
                            await rfqApproval.InvoiceFilePath.CopyToAsync(stream);
                        }
                        rfqApproval.DnFilePath = path;
                        var uploadDN = await _dinRepository.DNGenerationAsync(rfqApproval);
                        if (uploadDN)
                        {
                            Alert("Invoice uploaded successfully.", NotificationType.success);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Alert("Can not upload invoice at this time. Please try again later.", NotificationType.error);
                            return View(RfqApprovalDetails);
                        }
                    }
                }
               
                Alert("Can not upload invoice at this time. Please try again later.", NotificationType.error);
                return View(RfqApprovalDetails);
            }
            catch (Exception ex)
            {
                Alert("Can not upload invoice at this time. Please try again later.", NotificationType.error);
                return View(RfqApprovalDetails);
            }

        }


    }
}