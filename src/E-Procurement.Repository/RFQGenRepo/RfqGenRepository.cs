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
                 var selected = model.SelectedItems.Zip(model.Description,( x, y) => new { X = x, Y = y });             
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

                foreach (var item in model.SelectedItems)
                {
                    RFQGeneration RfqGen = new RFQGeneration();

                    RFQDetails RfqDet = new RFQDetails();

                    RfqDet.ItemId = Convert.ToInt32(item.ToString());
                    RfqDet.ItemName = model.ItemName;
                    RfqDet.UpdatedBy = model.CreatedBy;
                    RfqDet.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                    RfqDet.RFQId = RfqGen.Id;
                    foreach (var quantity in model.Quantity)
                    {
                        RfqDet.QuotedQuantity =  quantity;
                        _context.Add(RfqDet);
                    }

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

                oldEntry.Reference = model.Reference;

                foreach (var des in model.Description)
                {
                    oldEntry.Description = des;
                }

                oldEntry.ProjectId = model.ProjectId;

                oldEntry.RequisitionId = model.RfqId;

                oldEntry.StartDate = DateTime.ParseExact(model.StartDate.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                oldEntry.EndDate = DateTime.ParseExact(model.EndDate.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                oldEntry.RFQStatus = model.RfqStatus;

                oldEntry.UpdatedBy = model.UpdatedBy;

                oldEntry.LastDateUpdated = DateTime.Now;

                foreach (var item in model.SelectedItems)
                {

                    RFQDetails rfq = new RFQDetails();

                    var confirm2 = _context.RfqDetails.Where(u => u.RFQId == model.Id).Count();
                    var itemDetails = _context.Items.Where(u => u.Id == Convert.ToInt32(item.ToString())).FirstOrDefault();
                    if (confirm2 == model.SelectedItems.Count())
                    {
                        var oldEntry2 = _context.RfqDetails.Where(u => u.RFQId == model.Id).FirstOrDefault();

                        oldEntry2.ItemId = Convert.ToInt32(item.ToString());
                        oldEntry2.ItemName = itemDetails.ItemName;
                        oldEntry2.RFQId = model.Id;
                        foreach (var quantity in model.Quantity)
                        {
                            oldEntry2.QuotedQuantity = quantity;
                        }
                        
                        oldEntry2.UpdatedBy = model.UpdatedBy;
                        oldEntry2.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                    }
                    else if (confirm2 < model.SelectedItems.Count())
                    {
                        var oldEntry5 = item.CompareTo(_context.RfqDetails.Any(u => u.RFQId != model.Id && u.ItemId != Convert.ToInt32(item.ToString())));
                        var oldEntry1 = _context.RfqDetails.Where(u => u.RFQId == model.Id).FirstOrDefault();
                        var oldEntry2 = _context.RfqDetails.Where(u => u.RFQId == model.Id && u.ItemId == Convert.ToInt32(item.ToString())).FirstOrDefault();
                        var oldItem = item.ToString().Where(u => u != oldEntry2.ItemId).ToList();

                        foreach (var newItem in oldItem)
                        {
                            rfq.ItemId = newItem;
                            rfq.ItemName = itemDetails.ItemName;
                            foreach (var quantity in model.Quantity)
                            {
                                rfq.QuotedQuantity = quantity;
                            }
                            
                            rfq.CreatedBy = model.CreatedBy;
                            rfq.DateCreated = DateTime.Now;
                            rfq.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                            rfq.RFQId = oldEntry.Id;
                            _context.Add(rfq);
                        }
                        oldEntry1.ItemId = Convert.ToInt32(item.ToString());
                        oldEntry1.ItemName = itemDetails.ItemName;
                        oldEntry1.RFQId = model.Id;
                        foreach (var quantity in model.Quantity)
                        {
                            oldEntry1.QuotedQuantity = quantity;
                        }
                        oldEntry1.UpdatedBy = model.UpdatedBy;
                        oldEntry1.LastDateUpdated = DateTime.Now;
                        oldEntry1.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                    }
                    else if (confirm2 > model.SelectedItems.Count())
                    {
                        var oldEntry2 = _context.RfqDetails.Where(u => u.RFQId == model.Id && u.ItemId != Convert.ToInt32(item.ToString())).FirstOrDefault();
                        var oldItem = item.ToString().Where(u => u != oldEntry2.ItemId).ToList();
                        foreach (var old in oldItem)
                        {
                            _context.Remove(oldEntry2);
                        }
                        oldEntry2.ItemId = Convert.ToInt32(item.ToString());
                        oldEntry2.ItemName = itemDetails.ItemName;
                        oldEntry2.RFQId = model.Id;
                        foreach (var quantity in model.Quantity)
                        {
                            oldEntry2.QuotedQuantity = quantity;
                        }
                        oldEntry2.UpdatedBy = model.UpdatedBy;
                        oldEntry2.LastDateUpdated = DateTime.Now;
                        oldEntry2.VendorId = Convert.ToInt32(string.Join<int>(",", model.SelectedVendors));
                       
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

        public List<Vendor> GetVendors(int CategoryId)
        {
            var mapping = _context.VendorMappings.Where(u => u.VendorCategoryId == CategoryId).Select(u => u.VendorID).FirstOrDefault();
            return _context.Vendors.Where(u => u.Id == mapping).ToList();
        }

        public List<RFQGeneration> GetRfqGen()
        {
            return _context.RfqGenerations.OrderByDescending(u => u.Id).ToList();
        }
        
    }
}
