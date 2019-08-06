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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static E_Procurement.WebUI.Enums.Enums;

namespace E_Procurement.WebUI.Controllers
{
    public class GRNController : BaseController
    {
        //private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IPORepository _PORepository;
        private readonly IMapper _mapper;
        private readonly IAccountManager _accountManager;
        private readonly IRfqApprovalRepository _RfqApprovalRepository;
        private readonly IGRNRepository _grnRepository;
        private IHostingEnvironment _hostingEnv;

        public GRNController(IPORepository PORepository, 
                                IMapper mapper, 
                                IAccountManager accountManager, 
                                IRfqApprovalRepository RfqApprovalRepository,
                                IGRNRepository grnRepository,
                                IHostingEnvironment hostingEnv)
        {
            _PORepository = PORepository;
            _accountManager = accountManager;
            _mapper = mapper;
            _RfqApprovalRepository = RfqApprovalRepository;
            _grnRepository = grnRepository;
            _hostingEnv = hostingEnv;

        }
        public async Task<IActionResult> Index()
        {
            var RfqApprovalList = await _grnRepository.GetPOAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }

        public async Task<IActionResult> PODetails(int id)
        {
            var RfqApprovalDetails = await _grnRepository.GetInvoiceDetailsAsync(id);

           
            //RfqApproval.RFQDetails = rqfDetails;
            if (RfqApprovalDetails == null)
                return View();

            return View(RfqApprovalDetails);
        }


        [HttpPost]
        public async Task<IActionResult> PODetails(RFQGenerationModel rfqApproval)
        {

            try
            {
                if (string.IsNullOrEmpty(rfqApproval.RFQStatus) || rfqApproval.RFQStatus != "Approved")
                {
                    Alert("Can not upload GRN. Please try again later.", NotificationType.warning);
                    return View(rfqApproval);
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
                            return View(rfqApproval);
                        }
                   

                        //var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", imageFilePath);
                        var path = Path.Combine(webRootPath, "uploads","GRN", InvoiceFilePath);

                        
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        using (Stream stream = new FileStream(path, FileMode.Create))
                        {
                            await rfqApproval.InvoiceFilePath.CopyToAsync(stream);
                        }
                        rfqApproval.GRNFilePath = path;
                        var uploadDN = await _grnRepository.GRNGenerationAsync(rfqApproval);
                        if (uploadDN)
                        {
                            Alert("GRN uploaded successfully.", NotificationType.success);
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Alert("Can not upload GRN at this time. Please try again later.", NotificationType.error);
                            return View(rfqApproval);
                        }
                    }
                }
               
                Alert("Can not upload GRN at this time. Please try again later.", NotificationType.error);
                return View(rfqApproval);
            }
            catch (Exception ex)
            {
                Alert("Can not upload GRN at this time. Please try again later.", NotificationType.error);
                return View(rfqApproval);
            }

        }


    }
}