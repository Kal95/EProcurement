using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using Newtonsoft.Json;

namespace E_Procurement.Repository.VendoRepo
{
    public class VendorRepository : IVendorRepository
    {
        private readonly EProcurementContext _context;


        public VendorRepository(EProcurementContext context)
        {
            _context = context;
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
                _context.Vendors.Add(vendor);


                foreach (var category in model.SelectedVendorCategories)
                {
                    VendorMapping mapping = new VendorMapping();
                    mapping.VendorCategoryId =  Convert.ToInt32(category.ToString());
                    mapping.VendorID = vendor.Id;
                    vendor.VendorMapping.Add(mapping);
                }                
               
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

            var confirm = _context.Vendors.Where(x => x.VendorName == model.VendorName && x.IsActive == model.IsActive).Count();

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
                

                foreach (var category in model.SelectedVendorCategories)
                {
                    
                   
                    var oldEntry2 = _context.VendorMappings.Where(u => u.Id == model.MappingId).FirstOrDefault();
                   
                    var confirm2 = _context.VendorMappings.Where(u => u.VendorID == model.Id).Count();

                    if (confirm2 == model.SelectedVendorCategories.Count())
                    {
                        foreach (var category2 in _context.VendorMappings.Where(u => u.Id == model.MappingId && u.VendorCategoryId == model.VendorCategoryId))
                        {
                            oldEntry2.VendorCategoryId = category2.VendorCategoryId = category;
                            oldEntry2.VendorID = category2.VendorID = model.VendorId;

                        }
                    }
                    else if (confirm2 != model.SelectedVendorCategories.Count())
                    {
                        foreach (var category2 in _context.VendorMappings.Where(u => u.Id == model.MappingId && u.VendorCategoryId != model.VendorCategoryId))
                        {
                            _context.VendorMappings.Remove(category2);

                        }
                        foreach (var category3 in model.SelectedVendorCategories)
                        {
                            VendorMapping mapping = new VendorMapping();
                            mapping.VendorCategoryId = category3;
                            mapping.VendorID = oldEntry.Id;
                            oldEntry.VendorMapping.Add(mapping);
                        }
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
                client.BaseAddress = new Uri("https://localhost:44395");

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
