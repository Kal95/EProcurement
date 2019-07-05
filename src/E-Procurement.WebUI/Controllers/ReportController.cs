using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using E_Procurement.Repository.PORepo;
using E_Procurement.Repository.ReportRepo;
using E_Procurement.Repository.RFQGenRepo;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Procurement.WebUI.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportRepository _reportRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IRfqGenRepository _rfqGenRepository;
        private readonly IPORepository _poRepository;
        private readonly IMapper _mapper;

        public ReportController(IRfqGenRepository rfqRepository, IVendorRepository vendorRepository, IPORepository poRepository, IReportRepository reportRepository, IMapper mapper)
        {
            _rfqGenRepository = rfqRepository;
            _poRepository = poRepository;
            _vendorRepository = vendorRepository;
            _reportRepository = reportRepository;
            _mapper = mapper;
        }
        private void VendorPredefinedInfo(VendorModel Model)
        {
            //int CategoryId = Model.VendorCategoryId;

            var vendorCategory = _vendorRepository.GetItemCategory().ToList();
            if (Model.VendorCategoryId <= 0)
            {
                var Vendor = _vendorRepository.GetVendors().ToList();
                List<ReportModel> vendorModel = new List<ReportModel>();
                Model.VendorCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                });
                var listModel = Vendor.Select(x => new ReportModel
                {
                    VendorName = x.VendorName,
                    VendorAddress = x.VendorAddress,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber

                });
                vendorModel.AddRange(listModel);


                Model.Report = vendorModel;
        }
            else
            {
                var Mapping = _vendorRepository.GetMapping().ToList();
                var Vendor = _vendorRepository.GetVendors().ToList();
                var VendorList = Vendor.Where(a => Mapping.Any(b => b.VendorCategoryId == Model.VendorCategoryId)).ToList();
                 List<ReportModel> vendorModel = new List<ReportModel>();
                Model.VendorCategoryList = vendorCategory.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.CategoryName
                 });
                var listModel = VendorList.Select(x => new ReportModel
                {
                    VendorName = x.VendorName,
                    VendorAddress = x.VendorAddress,
                    ContactName = x.ContactName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber


                });
                vendorModel.AddRange(listModel);


                Model.Report = vendorModel;
            }
           
        }
        private void RfqPredefinedInfo(RfqGenModel Model)
        {
            int CategoryId = Model.CategoryId;


            var ItemCategory = _rfqGenRepository.GetItemCategory().ToList();

            var Item = _rfqGenRepository.GetItem(CategoryId).ToList();

            List<ReportModel> rfqModel = new List<ReportModel>();

            var Vendor = _rfqGenRepository.GetVendors(Model).ToList();
            if (Model.StartDate == DateTime.MinValue && Model.EndDate == DateTime.MinValue)
            {
                var RfqList = _rfqGenRepository.GetRfqGen().OrderBy(u => u.EndDate).ToList();
                var listModel = RfqList.Select(x => new ReportModel
                {
                    Reference = x.Reference,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive


                });
                rfqModel.AddRange(listModel);


                Model.Report = rfqModel;
                //Model.ItemCategoryList = ItemCategory.Select(x => new SelectListItem
                //{
                //    Value = x.Id.ToString(),
                //    Text = x.CategoryName
                //});
            }
            else
            {
                var RfqList = _rfqGenRepository.GetRfqGen().Where(u=> (Convert.ToDateTime(u.StartDate) >= Convert.ToDateTime(Model.StartDate)) && (Convert.ToDateTime(u.EndDate) <= Convert.ToDateTime(Model.EndDate))).OrderBy(u => u.EndDate).ToList();
                var listModel = RfqList.Select(x => new ReportModel
                {
                    Reference = x.Reference,
                    Description = x.Description,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.IsActive


                });
                rfqModel.AddRange(listModel);


                Model.Report = rfqModel;

            }



                //Model.VendorList = Vendor;

            }
        //[ValidateAntiForgeryToken]
        public ActionResult Vendor(VendorModel Model)
        {
            try
            {
                //VendorModel Model = new VendorModel();

                VendorPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View("Error");
            }

        }
        public ActionResult RfqGen(RfqGenModel Model)
        {
            try
            {
                //RfqGenModel Model = new RfqGenModel();

                RfqPredefinedInfo(Model);

                return View(Model);
            }
            catch (Exception)
            {

                return View();
            }
        }
        //public ActionResult PoGen()
        //{
        //    var PoGen = _reportRepository.GetPoGen();

        //    return View(PoGen);
        //}
        public async Task<IActionResult> POGen()
        {
            var RfqApprovalList = await _poRepository.GetPOAsync();

            List<RFQGenerationViewModel> RfqApproval = _mapper.Map<List<RFQGenerationViewModel>>(RfqApprovalList);

            return View(RfqApproval);
        }
    }
}