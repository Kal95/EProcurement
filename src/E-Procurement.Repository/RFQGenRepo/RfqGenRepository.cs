﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using DinkToPdf.Contracts;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AccountRepo;
using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using E_Procurement.Repository.ReportRepo;
using E_Procurement.WebUI.Models.RFQModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace E_Procurement.Repository.RFQGenRepo
{
    public class RfqGenRepository : IRfqGenRepository
    {
        private readonly EProcurementContext _context;
        private readonly ISMTPService _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;
        private IConvertViewToPDF _pdfConverter;
        private IConfiguration _config;
        private readonly IReportRepository _reportRepository;
        private readonly IAccountManager _accountManager;

        public RfqGenRepository(EProcurementContext context, IConfiguration config, ISMTPService emailSender, IHttpContextAccessor contextAccessor, IConvertViewToPDF pdfConverter, IReportRepository reportRepository, IAccountManager accountManager)
        {
            _context = context;
            _emailSender = emailSender;
            _contextAccessor = contextAccessor;
            _pdfConverter = pdfConverter;
            _config = config;
            _reportRepository = reportRepository;
            _accountManager = accountManager;
        }


        public bool CreateRfqGen(RfqGenModel model, out string Message)
        {
            var confirm = _context.RfqGenerations.Where(x => x.Reference == model.Reference).Count();

            if (confirm == 0)
            {
                //var selected = model.SelectedItems.Zip(model.Descriptions, (x, y) => new { X = x, Y = y });
                //foreach (var entry in selected)
                //{
                    RFQGeneration RfqGen = new RFQGeneration();

                    RfqGen.Reference = model.Reference;

                    RfqGen.ProjectId = model.ProjectId;

                    RfqGen.RequisitionId = model.RequisitionId;

                    RfqGen.StartDate = model.StartDate;

                    RfqGen.EndDate = model.EndDate;

                    RfqGen.RFQStatus = model.RfqStatus;

                    //RfqGen.IsActive = true;
                    RfqGen.InitiatedBy = model.InitiatedBy;

                    RfqGen.CreatedBy = model.CreatedBy;

                    RfqGen.DateCreated = DateTime.Now;

                    RfqGen.Description = string.Join(", ", model.Descriptions);

                    _context.Add(RfqGen);

                //}

                _context.SaveChanges();
                foreach (var item in model.SelectedVendors)
                {
                    List<RFQDetails> RfqDet = new List<RFQDetails>();
                    var rfq = _context.RfqGenerations.Where(u => u.Reference == model.Reference).FirstOrDefault();
                    var itemV = GetItem(model.CategoryId).ToList();
                    var itemListV = itemV.Where(a => model.SelectedItems.Any(b => b == a.Id.ToString())).ToList();

                    var selected2 = model.SelectedItems.Zip(model.Quantities, (x, y) => new { X = x, Y = y });
                    var selected3 = itemListV.Zip(model.Descriptions, (a, b) => new { A = a, B = b });
                    var selected4 = model.RFQBody.Zip(model.RFQTitle, (n, m) => new { N = n, M = m });
                    var selected5 = selected2.Zip(selected3, (o, p) => new { O = o, P = p });
                    var selected6 = selected5.Zip(selected4, (s, t) => new { S = s, T = t });
                    var listModel = selected6.Select(x => new RFQDetails
                    {
                        ItemId = Convert.ToInt32(x.S.O.X.ToString()),
                        ItemName = x.S.P.A.ItemName,
                        QuotedQuantity = x.S.O.Y,
                        ItemDescription = x.S.P.B,
                        RFQTitle = model.RFQTitle,
                        RFQBody = model.RFQBody,
                        RFQCondition = model.RFQCondition,
                        RFQId = rfq.Id,
                        //IsActive = true,
                        UpdatedBy = model.CreatedBy,
                        VendorId = item
                    });
                    RfqDet.AddRange(listModel);
                    _context.AddRange(RfqDet);


                    //RfqDet.VendorId = item;
                    //var itemV = GetItem(model.CategoryId).ToList();
                    //var itemListV = itemV.Where(a => model.SelectedItems.Any(b => b == a.Id.ToString())).ToList();
                    //var selected1 = model.SelectedItems.Zip(model.Quantities, (x, y) => new { X = x, Y = y });
                    //var selected2 = model.Descriptions.Zip(selected1, (a, b) => new { A = a, B = b });
                    //var selected3 = itemListV.Zip(selected2, (o, p) => new { O = o, P = p });
                    //foreach (var entry in selected3)
                    //{
                    //    var rfq = _context.RfqGenerations.Where(u => u.Reference == model.Reference).FirstOrDefault();
                    //    //RFQGeneration RfqGen = new RFQGeneration();

                    //    RfqDet.ItemId = Convert.ToInt32(entry.B.X.ToString());
                    //    RfqDet.ItemName = entry.A.O.ItemName;
                    //    RfqDet.UpdatedBy = model.CreatedBy;
                    //    RfqDet.ItemDescription = entry.A.P;
                    //    RfqDet.RFQId = rfq.Id;
                    //    RfqDet.IsActive = true;
                    //    RfqDet.QuotedQuantity = Convert.ToInt32(entry.B.Y.ToString());

                    //    _context.Add(RfqDet);
                    //}
                }

                _context.SaveChanges();

                //get PDF data

                var items = GetItem(model.CategoryId).ToList();
                var itemList = items.Where(a => model.SelectedItems.Any(b => b == a.Id.ToString())).ToList();
                var selectedV = model.SelectedVendors.ToList();
                var vendors = _context.Vendors.OrderByDescending(u => u.Id).ToList();

                var message = "";

                var vendorList = vendors.Where(a => selectedV.Any(b => b == a.Id)).ToList();
                //var selected3 = model.SelectedVendors.Zip(model.SelectedItems, (x, y) => new { X = x, Y = y });
                foreach (var entry in vendorList)
                {
                    var requisitionURL = _config.GetSection("ExternalAPI:RequisitionURL").Value;
                                     

                    message = "<b> Dear </b>" + entry.ContactName;
                    message += "<br><br><b> Your company: </b>" + entry.VendorName + ",";
                    message += "<br> has been chosen to provide quote for the following items: ";
                    message += string.Join(", ", itemList.Select(u => u.ItemName)) + ",";
                    message += "<br> in the following quantities: " + string.Join(", ", model.Quantities) + " respectively.";
                    message += "<br>Kindly Login to our E-Procurement Platform via " + $"<a href=\"{requisitionURL}\">" + requisitionURL + "/QuoteSending </a>" + " to respond.";
                    message += "<br><br>Regards";

                    RFQGenerationModel model2 = new RFQGenerationModel();
                    model2.VendorId = entry.Id;
                    model2.ContactName = entry.ContactName;
                    model2.VendorName = entry.VendorName;
                    model2.VendorAddress = entry.VendorAddress;
                    model2.VendorEmail = entry.Email;
                    model2.RFQId = Convert.ToInt32(model.Reference.ToString());
                    model2.RFQTitle = model.RFQTitle.ToUpper();
                    model2.RFQBody = model.RFQBody;
                    model2.RFQCondition = model.RFQCondition;
                    model2.URL = requisitionURL;
                    model2.Reference = model.Reference;
                    model2.CreatedDate = DateTime.Now;

                    List<RFQDetailsModel> rFQDetails = new List<RFQDetailsModel>();
                    var itemV = GetItem(model.CategoryId).ToList();
                    var itemListV = itemV.Where(a => model.SelectedItems.Any(b => b == a.Id.ToString())).ToList();

                    var selected2 = model.SelectedItems.Zip(model.Quantities, (x, y) => new { X = x, Y = y });
                    var selected3 = itemListV.Zip(model.Descriptions, (a, b) => new { A = a, B = b });

                    var selected4 = selected2.Zip(selected3, (o, p) => new { O = o, P = p });
                    var listModel = selected4.Select(x => new RFQDetailsModel
                    {
                        ItemName = x.P.A.ItemName,
                        QuotedQuantity = x.O.Y,
                        Description = x.P.B
                    });

                    rFQDetails.AddRange(listModel);


                    model2.RFQDetails = rFQDetails;

                    //generate PDF and send mail
                    _pdfConverter.CreateRFQPDF(model2, message);
                    //string emailBody = _pdfConverter.CreateRFQPDF(model2);
                   // _emailSender.SendEmailAsync(entry.Email, subject, message, filePath);
                   // _emailSender.SendEmailAsync(entry.Email, subject, message,"");
                }

                //Send Email to Initiator
                var user = _reportRepository.GetUser().Where(u => u.Email == model.InitiatedBy).FirstOrDefault();
                var user2 = _reportRepository.GetUser().Where(u => u.Email == model.InitiatedBy).Count();
                if (user2 == 0)
                {
                    User user3 = new User();
                    user3.Email = model.InitiatedBy;
                    user3.UserName = model.InitiatedBy;
                    user3.FirstName = "User";
                    var Password = "Password1$";
                    var Role = "Initiator";
                    var result =  _accountManager.CreateUserAsync(user3, Password, Role);

                    if (result.Result)
                    {
                        var subject = "RFQ NOTIFICATION";
                        message = "</br><b> Dear " + user3.FirstName +",</b><br>";
                        message += "<br> Please be informed that RFQ has been sent to Vendors for your request with Reference: " + model.Reference + ",<br>";
                        message += "<br><U> Details are as follows:</U> ";
                        message += "<br>" + string.Join(", ", itemList.Select(u => u.ItemName)) + ",";
                        message += "<br> in the following quantities: " + string.Join(", ", model.Quantities) + " respectively.<br>";
                        message += "<br>Your signin details have also been sent to your mail to enable you track the progress of your request.";
                        message += "<br>Regards";

                        _emailSender.SendEmailAsync(model.InitiatedBy, subject, message, "");
                    }
                }
                else
                {
                    var subject = "RFQ NOTIFICATION";
                    message = "</br><b> Dear " + user.FullName + ",</b><br>";
                    message += "<br> Please be informed that RFQ has been sent to Vendors for your request with Reference: " + model.Reference + ",<br>";
                    message += "<br><U> Details are as follows:</U> ";
                    message += "<br>" + string.Join(", ", itemList.Select(u => u.ItemName)) + ",";
                    message += "<br> in the following quantities: " + string.Join(", ", model.Quantities) + " respectively.<br>";
                    message += "<br>Regards";

                    _emailSender.SendEmailAsync(model.InitiatedBy, subject, message, "");
                }
                

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

        public bool CloseRfqGen(RfqGenModel model, out string Message)
        {
            var confirm = _context.RfqGenerations.Where(x => x.Reference == model.Reference && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.RfqGenerations.Where(u => u.Id == model.Id).FirstOrDefault();


            if (oldEntry == null)
            {
                throw new Exception("No RFQ exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.EndDate = DateTime.Now.AddDays(-1);


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

        public bool ExtendRfqGen(RfqGenModel model, out string Message)
        {
            var confirm = _context.RfqGenerations.Where(x => x.Reference == model.Reference && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.RfqGenerations.Where(u => u.Id == model.Id).FirstOrDefault();


            if (oldEntry == null)
            {
                throw new Exception("No RFQ exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.EndDate = model.EndDate;


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
                var requisitionURL = _config.GetSection("ExternalAPI:RequisitionURL").Value;
                client.BaseAddress = new Uri(requisitionURL);

                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                HttpResponseMessage response = client.GetAsync("/api/item/getitem/" + CategoryId).Result;
                string stringData = response.Content.ReadAsStringAsync().Result;
                List<RfqGenModel> data = JsonConvert.DeserializeObject<List<RfqGenModel>>(stringData);
                return data.Where(x => x.IsActive == true).ToList();
            }
        }
        public List<RfqGenModel> GetItemCategory()
        {
            using (HttpClient client = new HttpClient())
            {
                var requisitionURL = _config.GetSection("ExternalAPI:RequisitionURL").Value;
                client.BaseAddress = new Uri(requisitionURL);

                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                HttpResponseMessage response = client.GetAsync("/api/item/getcategory/").Result;
                string stringData = response.Content.ReadAsStringAsync().Result;
                List<RfqGenModel> data = JsonConvert.DeserializeObject<List<RfqGenModel>>(stringData);
                return data.Where(x => x.IsActive == true).ToList();
            }
        }

        public List<Vendor> GetVendors(RfqGenModel model)
        {
            var mapping = _context.VendorMappings.Where(u => u.VendorCategoryId == model.CategoryId).ToList();
            var vendor = _context.Vendors.Where(x => x.IsActive == true).OrderByDescending(u => u.Id).ToList();

            var vendorList = vendor.Where(a => mapping.Any(b => b.VendorID == a.Id));
            return vendorList.ToList();
        }

        public List<RFQApprovalTransactions> GetInitiatedRfq()
        {
            var RFQ = _context.RfqApprovalTransactions.ToList();
            return RFQ;
        }
        
        public List<RfqGenModel> GetRfqGen()
        {
            var ven = _context.Vendors.ToList();
            
            var des = _context.RfqDetails.ToList();
           
            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
               //VendorName = x.VendorName,
                ItemName =  x.ItemName
            }).GroupBy(v => new {v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());
            

            var vendList = (from d in des join v in ven on d.VendorId equals v.Id select new RfqGenModel()
            {
               RfqId = d.RFQId,
              VendorName = v.VendorName
            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());

            var query = (/*from rfqDetails in _context.RfqDetails*/
                         from rfq in _context.RfqGenerations /*on rfqDetails.RFQId equals rfq.Id*/
                           
                         select new RfqGenModel()
                         {
                             //QuotedAmount = rfqDetails.QuotedAmount,
                             //QuotedQuantity = rfqDetails.QuotedQuantity,
                             RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             Item = string.Join(", ", desList.Where(u => u.RfqId == rfq.Id).Select (u=> u.ItemName)),
                             VendorName = string.Join(", ", vendList.Where(a => a.RfqId == rfq.Id).Select(u => u.VendorName)),
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             CreatedDate = rfq.DateCreated,
                             RFQStatus = rfq.RFQStatus,
                             //VendorId = rfqDetails.VendorId,
                             //IsActive = rfq.IsActive
                         });

            return query.OrderByDescending(u => u.CreatedDate).ToList();
        }
        public List<Vendor> GetVendorDetails(RfqGenModel model)
        {

            //var mapping = _context.VendorMappings.Select(u => u.VendorID).FirstOrDefault();
            //return _context.Vendors.Where(u => u.Id == mapping).ToList();

            var items = GetItem(model.CategoryId).ToList();
            var itemList = items.Where(a => model.SelectedItems.Any(b => b == a.Id.ToString())).ToList();
            var selectedV = model.SelectedVendors.ToList();
            var vendors = _context.Vendors.OrderByDescending(u => u.Id).ToList();
            
            return vendors;
        }

    }
}
