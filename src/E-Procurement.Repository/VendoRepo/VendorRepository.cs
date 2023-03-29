using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace E_Procurement.Repository.VendoRepo
{
    public class VendorRepository : IVendorRepository
    {
        private readonly EProcurementContext _context;
        private readonly ISMTPService _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAccountManager _accountManager;
        private IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public VendorRepository(EProcurementContext context, UserManager<User> userManager, IConfiguration config, ISMTPService emailSender, IHttpContextAccessor contextAccessor, IAccountManager accountManager)
        {
            _context = context;
            _emailSender = emailSender;
            _contextAccessor = contextAccessor;
            _accountManager = accountManager;
            _config = config;
            _userManager = userManager;
        }


        public static class PasswordGenerator
        {
            private const int DefaultPasswordLength = 12;

            public static string GeneratePassword(int minLength = DefaultPasswordLength)
            {
                const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
                const string digitChars = "0123456789";
                const string specialChars = "!@#$%^&*+[]{}|;:,.<>/?";
                const string allChars = uppercaseChars + lowercaseChars + digitChars + specialChars;

                byte[] randomBytes = new byte[minLength];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomBytes);
                }

                StringBuilder sb = new StringBuilder(minLength);
                bool hasUppercase = false;
                bool hasLowercase = false;
                bool hasDigit = false;
                bool hasSpecialChar = false;

                for (int i = 0; i < minLength; i++)
                {
                    int charIndex = randomBytes[i] % allChars.Length;
                    char c = allChars[charIndex];

                    if (char.IsUpper(c))
                    {
                        hasUppercase = true;
                    }
                    else if (char.IsLower(c))
                    {
                        hasLowercase = true;
                    }
                    else if (char.IsDigit(c))
                    {
                        hasDigit = true;
                    }
                    else
                    {
                        hasSpecialChar = true;
                    }

                    sb.Append(c);
                }

                // Ensure that the password meets the requirements
                if (!hasUppercase)
                {
                    sb[randomBytes[0] % minLength] = uppercaseChars[randomBytes[1] % uppercaseChars.Length];
                }
                if (!hasLowercase)
                {
                    sb[randomBytes[2] % minLength] = lowercaseChars[randomBytes[3] % lowercaseChars.Length];
                }
                if (!hasDigit)
                {
                    sb[randomBytes[4] % minLength] = digitChars[randomBytes[5] % digitChars.Length];
                }
                if (!hasSpecialChar)
                {
                    sb[randomBytes[6] % minLength] = specialChars[randomBytes[7] % specialChars.Length];
                }

                return sb.ToString();


                //const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*";
                //var randomBytes = new byte[length];
                //using (var rng = RandomNumberGenerator.Create())
                //{
                //    rng.GetBytes(randomBytes);
                //}

                //var result = new char[length];
                //for (int i = 0; i < length; i++)
                //{
                //    var index = randomBytes[i] % validChars.Length;
                //    result[i] = validChars[index];
                //}
                //return new string(result);
            }
        }


        private async Task<(bool success, string password)> UserAccountCreate(VendorModel model)
        {
            var user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.VendorName,
                PhoneNumber = model.PhoneNumber,
            };

            var password = PasswordGenerator.GeneratePassword(8);

            var result = await _accountManager.CreateUserAsync(user, password, "Vendor User");

            if(result)
            {
                //Alert("Account created sucsessfully.", NotificationType.success);
                return await Task.FromResult((true, password));
            }

            else
            {
                //Alert("User account could not be created. Please try again later.", NotificationType.error);

                return await Task.FromResult((false, ""));
            }

        }
        public bool CreateVendor(VendorModel model, out string Message)
        {
            var ExistingUser = _userManager.Users.Any(a => a.Email == model.Email);
            if (ExistingUser)
            {
                Message = "Vendor already exist";

                return false;
            }
            var creation =  UserAccountCreate(model).Result;
            if(!creation.success)
            {
                Message = "Account could not be created";

                return false;

            }
            var confirm = _context.Vendors.Where(x => x.VendorName == model.VendorName).Count();

            Vendor vendor = new Vendor();


            if (confirm == 0)
            {
                var user = _context.Users.Where(x => x.Email == model.Email).FirstOrDefault();
                vendor.UserId = user.Id;
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
                var requisitionURL = _config.GetSection("ExternalAPI:RequisitionURL").Value;

                var subject = "SIGNUP NOTIFICATION";

                var message = "</br><b> Dear </b>" + model.ContactName.ToString();
                message += "</br><b> Your company: </b>" + model.VendorName;
                message += "</br>has been registered successful on Cyberspace E-procurement Portal.</br>";
                message += $"</br>Your username is: {model.Email}</br>";
                message += $"</br>Your password is: {creation.password}</br>";
                message += $"</br>You are advised to change your password upon login</br>";
                message += "</br>Kindly, log in via " + requisitionURL +" and validate the required documents.";
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
            //var confirm5 = model.SelectedVendorCategories.Count();
            var confirm = _context.Vendors.Where(x => x.VendorName == model.VendorName && x.IsActive == model.IsActive /*&& confirm4 == confirm5*/).Count();

            var oldEntry = _context.Vendors.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Vendor exists with this Id");
            }

            if (confirm >= 0)
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
                if (model.BankRefFilePath != null)
                {
                    oldEntry.BankRefFilePath = model.BankRefFilePath;
                }
                if (model.COVFilePath != null)
                {
                    oldEntry.COVFilePath = model.COVFilePath;
                }
                if (model.MOAFilePath != null)
                {
                    oldEntry.MOAFilePath = model.MOAFilePath;
                }
                if (model.NOSFilePath != null)
                {
                    oldEntry.NOSFilePath = model.NOSFilePath;
                }
                if (model.PODFilePath != null)
                {
                    oldEntry.PODFilePath = model.PODFilePath;
                }
                if (model.POSFilePath != null)
                {
                    oldEntry.POSFilePath = model.POSFilePath;
                }
                if (model.RefFilePath != null)
                {
                    oldEntry.RefFilePath = model.RefFilePath;
                }
                if (model.TaxFilePath != null)
                {
                    oldEntry.TaxFilePath = model.TaxFilePath;
                }

                if (model.SelectedVendorCategories != null)
                {
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
        public IEnumerable<User> GetUser()
        {
            return _userManager.Users.OrderByDescending(u => u.Id).ToList();
        }
    }
}
