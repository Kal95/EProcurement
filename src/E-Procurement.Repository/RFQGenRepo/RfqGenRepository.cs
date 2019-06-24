using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace E_Procurement.Repository.RFQGenRepo 
{
    public class RfqGenRepository : IRfqGenRepository
    {
        private readonly EProcurementContext _context;

        public RfqGenRepository(EProcurementContext context)
        {
            _context = context;
        }

       
        public bool CreateRfqGen (RfqGenModel model, out string Message)
        {
            var confirm = _context.RfqGenerations.Where(x => x.Reference == model.Reference).Count();

            if (confirm == 0)
            {
                 var selected = model.SelectedItems.Zip(model.Descriptions,( x, y) => new { X = x, Y = y });             
                foreach (var entry in selected)
                {
                    RFQGeneration RfqGen = new RFQGeneration();

                    RfqGen.Reference = model.Reference;

                    RfqGen.ProjectId = model.ProjectId;

                    RfqGen.RequisitionId = model.RequisitionId;

                    RfqGen.StartDate = model.StartDate;

                    RfqGen.EndDate = model.EndDate;

                    RfqGen.RFQStatus = model.RfqStatus;

                    RfqGen.IsActive = true;

                    RfqGen.CreatedBy = model.CreatedBy;

                    RfqGen.DateCreated = DateTime.Now;

                    RfqGen.Description = entry.Y;

                    _context.Add(RfqGen);
                    
                }
                
                _context.SaveChanges();

                var selected2 = model.SelectedItems.Zip(model.Quantities, (x, y) => new { X = x, Y = y });
                foreach (var entry in selected2)
                {
                    RFQGeneration RfqGen = new RFQGeneration();

                    RFQDetails RfqDet = new RFQDetails();

                    RfqDet.ItemId = Convert.ToInt32(entry.X.ToString());
                    RfqDet.ItemName = model.ItemName;
                    RfqDet.UpdatedBy = model.CreatedBy;
                    RfqDet.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                    RfqDet.RFQId = RfqGen.Id;
                    RfqDet.QuotedQuantity = Convert.ToInt32(entry.Y.ToString());

                    _context.Add(RfqDet);
                }
                
                _context.SaveChanges();

                Message = "RFQ generated successfully";

                return true;
            }
            else
            {
                Message = "RFQ already exist";

                return false;
            }
        }
        
        public bool UpdateRfqGen(RfqGenModel model, out string Message)
        {
            var confirm = _context.RfqGenerations.Where(x => x.Reference == model.Reference && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.RfqGenerations.Where(u => u.Id == model.Id).FirstOrDefault();
            

            if (oldEntry == null)
            {
                throw new Exception("No RFQ exists with this Id");
            }

            if (confirm == 0)
            {
             var selected = model.SelectedItems.Zip(model.Descriptions, (x, y) => new { X = x, Y = y });
             foreach (var entry in selected)
             {
                oldEntry.Reference = model.Reference;

                oldEntry.Description = entry.Y;
          
                oldEntry.ProjectId = model.ProjectId;

                oldEntry.RequisitionId = model.RfqId;

                oldEntry.StartDate = model.StartDate;

                oldEntry.EndDate = model.EndDate;

                oldEntry.RFQStatus = model.RfqStatus;

                oldEntry.UpdatedBy = model.UpdatedBy;

                oldEntry.LastDateUpdated = DateTime.Now;

                 _context.Add(oldEntry);

             }

                var selected2 = model.SelectedItems.Zip(model.Quantities, (x, y) => new { X = x, Y = y });
                foreach (var entry in selected2)
                {
                    var confirm2 = _context.RfqDetails.Where(u => u.RFQId == model.Id).Count();
                    var itemDetails = _context.Items.Where(u => u.Id == Convert.ToInt32(entry.X.ToString())).FirstOrDefault();
                    if (confirm2 == model.SelectedItems.Count())
                    {
                        var oldEntry2 = _context.RfqDetails.Where(u => u.RFQId == model.Id).FirstOrDefault();

                        oldEntry2.ItemId = Convert.ToInt32(entry.X.ToString());

                        oldEntry2.ItemName = itemDetails.ItemName;

                        oldEntry2.RFQId = model.Id;

                        oldEntry2.QuotedQuantity = entry.Y;

                        oldEntry2.UpdatedBy = model.UpdatedBy;

                        oldEntry2.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                    }
                    else if (confirm2 != selected2.Count())
                    {
                        foreach (var category2 in _context.RfqDetails.Where(u => u.RFQId == model.Id && u.ItemId != Convert.ToInt32(entry.X.ToString())))
                        {
                            _context.RfqDetails.Remove(category2);

                        }

                        RFQGeneration RfqGen = new RFQGeneration();

                        RFQDetails RfqDet = new RFQDetails();

                        RfqDet.ItemId = Convert.ToInt32(entry.X.ToString());
                        RfqDet.ItemName = model.ItemName;
                        RfqDet.UpdatedBy = model.CreatedBy;
                        RfqDet.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                        RfqDet.RFQId = RfqGen.Id;
                        RfqDet.QuotedQuantity = Convert.ToInt32(entry.Y.ToString());

                        _context.Add(RfqDet);
                    }

                }

                _context.SaveChanges();

                Message = "RFQ updated successfully";

                return true;
            }
            else
            {
                Message = "RFQ already exist";

                return false;
            }
        }

        public List<RfqGenModel> GetItem(int CategoryId)
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44395");

                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                HttpResponseMessage response = client.GetAsync("/api/item/getitem/" + CategoryId).Result;
                string stringData = response.Content.ReadAsStringAsync().Result;
                List<RfqGenModel> data = JsonConvert.DeserializeObject<List<RfqGenModel>>(stringData);
                return data.ToList();
            }
        }
        public List<RfqGenModel> GetItemCategory()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44395");

                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                HttpResponseMessage response = client.GetAsync("/api/item/getcategory/").Result;
                string stringData = response.Content.ReadAsStringAsync().Result;
                List<RfqGenModel> data = JsonConvert.DeserializeObject<List<RfqGenModel>>(stringData);
                return data.ToList();
            }
        }

        public List<Vendor> GetVendors(RfqGenModel model)
        {
            var mapping = _context.VendorMappings.Where(u => u.VendorCategoryId == model.CategoryId).ToList();
            var vendor = _context.Vendors.OrderByDescending(u => u.Id).ToList();
          
            var vendorList = vendor.Where(a => mapping.Any(b => b.VendorID == a.Id));
            return vendorList.ToList();
        }

        public List<RFQGeneration> GetRfqGen()
        {

            return _context.RfqGenerations.OrderByDescending(u => u.Id).ToList();
        }
        public List<Vendor> GetVendorDetails()
        {
           var mapping =  _context.VendorMappings.Select(u => u.VendorID).FirstOrDefault();
            
            return _context.Vendors.Where(u => u.Id == mapping).ToList();
        }

    }
}
