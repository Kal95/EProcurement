using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace E_Procurement.Repository.VendoRepo
{
    public class VendorRepository : IVendorRepository
    {
        private readonly EProcurementContext _context;
        private readonly ISMTPService _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;
        private IConfiguration _config;

        public VendorRepository(EProcurementContext context, IConfiguration config, ISMTPService emailSender, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _emailSender = emailSender;
            _contextAccessor = contextAccessor;
            _config = config;
        }
        public bool CreateVendor(VendorModel model, out string Message)
        {
            var confirm = _context.Vendors.Where(x => x.VendorName == model.VendorName).Count();

            Vendor vendor = new Vendor();


            if (confirm == 0)
            {

                vendor.VendorName = model.VendorName;
                vendor.AatAmount = model.AatAmount;
                vendor.AatCurrency = model.AatCurrency;
                vendor.AccountName = model.AccountName;
                vendor.AccountNo = model.AccountNo;
                vendor.BankBranch = model.BankBranch;
                vendor.BankId = model.BankId;
                vendor.CacNo = model.CacNo;
                vendor.ContactName = model.ContactName;
                vendor.CountryId = model.CountryId;
                vendor.CreatedBy = model.CreatedBy;
                vendor.DateCreated = DateTime.Now;
                vendor.Email = model.Email;
                vendor.PhoneNumber = model.PhoneNumber;
                vendor.IsActive = true;
                vendor.SortCode = model.SortCode;
                vendor.StateId = model.StateId;
                vendor.TinNo = model.TinNo;
                vendor.VendorAddress = model.VendorAddress;
                vendor.VendorStatus = model.VendorStatus;
                vendor.WebsiteAddress = model.WebsiteAddress;
                vendor.VatNo = model.VatNo;
                vendor.BankRefFilePath = model.BankRefFilePath;
                vendor.COVFilePath = model.COVFilePath;
                vendor.MOAFilePath = model.MOAFilePath; 
                vendor.NOSFilePath = model.NOSFilePath;
                vendor.PODFilePath = model.PODFilePath;
                vendor.POSFilePath = model.POSFilePath;
                vendor.RefFilePath = model.RefFilePath;
                vendor.TaxFilePath = model.TaxFilePath;
                _context.Vendors.Add(vendor);


                foreach (var category in model.SelectedVendorCategories)
                {
                    VendorMapping mapping = new VendorMapping();
                    mapping.VendorCategoryId = Convert.ToInt32(category.ToString());
                    mapping.VendorID = vendor.Id;
                    vendor.VendorMapping.Add(mapping);
                }
                var subject = "SIGNUP NOTIFICATION";

                var message = "</br><b> Dear </b>" + model.ContactName.ToString();
                message += "</br><b> Your company: </b>" + model.VendorName;
                message += "</br>has been registered successful on Cyberspace E-procurement Portal.</br>";
                message += "</br>Kindly, log in and validate the required documents.";
                message += "</br>Regards";

                _emailSender.SendEmailAsync(vendor.Email, subject, message,"");

                _context.SaveChanges();

                Message = "Vendor created successfully";

                return true;
            }
            else
            {
                Message = "Vendor already exist";

                return false;
            }

        }

        public bool UpdateVendor(VendorModel model, out string Message)
        {
            var confirm4 = _context.VendorMappings.Where(u => u.VendorID == model.Id).Count();
            var confirm5 = model.SelectedVendorCategories.Count();
            var confirm = _context.Vendors.Where(x => x.VendorName == model.VendorName && x.IsActive == model.IsActive && confirm4 == confirm5).Count();

            var oldEntry = _context.Vendors.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Vendor exists with this Id");
            }

            if (confirm == 0)
            {
                oldEntry.VendorName = model.VendorName;

                oldEntry.AatAmount = model.AatAmount;
                oldEntry.AatCurrency = model.AatCurrency;
                oldEntry.AccountName = model.AccountName;
                oldEntry.AccountNo = model.AccountNo;
                oldEntry.BankBranch = model.BankBranch;
                oldEntry.BankId = model.BankId;
                oldEntry.CacNo = model.CacNo;
                oldEntry.ContactName = model.ContactName;
                oldEntry.CountryId = model.CountryId;
                oldEntry.Email = model.Email;
                oldEntry.PhoneNumber = model.PhoneNumber;
                oldEntry.SortCode = model.SortCode;
                oldEntry.StateId = model.StateId;
                oldEntry.TinNo = model.TinNo;
                oldEntry.VendorAddress = model.VendorAddress;
                oldEntry.VendorStatus = model.VendorStatus;
                oldEntry.WebsiteAddress = model.WebsiteAddress;
                oldEntry.VatNo = model.VatNo;
                oldEntry.UpdatedBy = model.UpdatedBy;
                oldEntry.LastDateUpdated = DateTime.Now;
                oldEntry.IsActive = model.IsActive;
                oldEntry.BankRefFilePath = model.BankRefFilePath;
                oldEntry.COVFilePath = model.COVFilePath;
                oldEntry.MOAFilePath = model.MOAFilePath;
                oldEntry.NOSFilePath = model.NOSFilePath;
                oldEntry.PODFilePath = model.PODFilePath;
                oldEntry.POSFilePath = model.POSFilePath;
                oldEntry.RefFilePath = model.RefFilePath;
                oldEntry.TaxFilePath = model.TaxFilePath;

                foreach (var category in model.SelectedVendorCategories)
                {
                    var confirm2 = _context.VendorMappings.Where(u => u.VendorID == model.Id).Count();

                    if (confirm2 == model.SelectedVendorCategories.Count())
                    {
                        var oldEntry2 = _context.VendorMappings.Where(u => u.VendorID == model.Id).FirstOrDefault();

                        oldEntry2.VendorCategoryId = category;
                        oldEntry2.VendorID = model.Id;

                    }
                    else if (confirm2 != model.SelectedVendorCategories.Count())
                    {
                        foreach (var category2 in _context.VendorMappings.Where(u => u.VendorID == model.Id /*&& u.VendorCategoryId != category*/))
                        {
                            _context.VendorMappings.Remove(category2);

                        }
                        VendorMapping mapping = new VendorMapping();
                        mapping.VendorCategoryId = category;
                        mapping.VendorID = oldEntry.Id;
                        oldEntry.VendorMapping.Add(mapping);

                    }

                }

                _context.SaveChanges();

                Message = "Vendor updated successfully";

                return true;
            }
            else
            {
                Message = "Vendor already exist";

                return false;
            }

        }
        public List<VendorModel> GetItemCategory()
        {
            using (HttpClient client = new HttpClient())
            {
                var requisitionURL = _config.GetSection("ExternalAPI:RequisitionURL").Value;
                client.BaseAddress = new Uri(requisitionURL);

                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                HttpResponseMessage response = client.GetAsync("/api/item/getcategory/").Result;
                string stringData = response.Content.ReadAsStringAsync().Result;
                List<VendorModel> data = JsonConvert.DeserializeObject<List<VendorModel>>(stringData);
                return data.ToList();
            }
        }
        public IEnumerable<Vendor> GetVendors()
        {
            return _context.Vendors.OrderByDescending(u => u.Id).ToList();
        }
        public IEnumerable<VendorMapping> GetMapping()
        {
            return _context.VendorMappings.OrderByDescending(u => u.Id).ToList();
        }
    }
}
