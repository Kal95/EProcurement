using E_Procurement.Data;
using E_Procurement.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using E_Procurement.Repository.ApprovalRepo;
using E_Procurement.Repository.Dtos;
using System.Transactions;
using E_Procurement.Repository.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Numerics;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using E_Procurement.WebUI.Models.RFQModel;
using E_Procurement.Repository.VendoRepo;
using Microsoft.AspNetCore.Identity;
using E_Procurement.Repository.ReportRepo;

namespace E_Procurement.Repository.RfqApprovalConfigRepository
{
   public class RfqApprovalRepository : IRfqApprovalRepository
    {
        private readonly EProcurementContext _context;
        private readonly ISMTPService _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;
        private IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IReportRepository _reportRepository;

        public RfqApprovalRepository(EProcurementContext context, UserManager<User> userManager, IConfiguration config, ISMTPService emailSender, IHttpContextAccessor contextAccessor, IReportRepository reportRepository)
        {
            _context = context;
            _emailSender = emailSender;
            _config = config;
            _contextAccessor = contextAccessor;
            _userManager = userManager;
            _reportRepository = reportRepository;
        }
        public IEnumerable<User> GetApprovalRoles_Users()
        {
            var Roles = _context.Roles.Where(a => a.Name == "Approval").ToList();
            var UserRole = _context.UserRoles.Where(a => Roles.Any(b => a.RoleId == b.Id)).ToList();
            return _userManager.Users.Where(a => UserRole.Any(b => a.Id == b.UserId)).OrderByDescending(u => u.Id).ToList();
        }

        public bool CreateSignature(VendorModel model, out string Message)
        {
            var confirm = _context.Signatures.Where(x => x.Signee1 == model.Signee1 && x.Signee2 == model.Signee2).Count();

            Signature signature = new Signature();


            if (confirm == 0)
            {

                signature.CreatedBy = model.CreatedBy;
                signature.DateCreated = DateTime.Now;
                signature.Signee1 = model.Signee1;
                signature.Signee2 = model.Signee2;
                signature.IsActive = false;
                signature.Sign1 = model.Sign1Path;
                signature.Sign2 = model.Sign2Path;

                _context.Signatures.Add(signature);

                _context.SaveChanges();

                Message = "Signature created successfully";

                return true;
            }
            else
            {
                Message = "Signature already exist";

                return false;
            }

        }

        public bool ActivateSignature(VendorModel model, out string Message)
        {
            var confirm = _context.Signatures.Where(x => x.Id == model.SignId && x.IsActive == true).Count();

            var oldEntry = _context.Signatures.Where(u => u.Id == model.SignId).FirstOrDefault();
            var deactivate = _context.Signatures.Where(a => a.Id != oldEntry.Id).ToList();

            if (oldEntry == null)
            {
                throw new Exception("No Signature exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.IsActive = true;
                foreach(var item in deactivate)
                {
                    item.IsActive = false;
                }

                _context.SaveChanges();

                Message = "Signature Activated successfully";

                return true;
            }
            else
            {
                Message = "Signature is already Active";

                return false;
            }

        }
        public List<Signature> GetSignatures()
        {
            var transac = _context.Signatures.OrderByDescending(a => a.IsActive == true).ToList();
            return transac;
        }
        public List<POApprovalTransactions> GetPOTransactions()
        {
            var transac = _context.POApprovalTransactions.ToList();
            return transac;
        }
        public List<RFQApprovalTransactions> GetRFQTransactions()
        {
            var transac = _context.RfqApprovalTransactions.ToList();
            return transac;
        }
        public List<Vendor> GetVendors()
        {
            var vendor = _context.Vendors.ToList();
            return vendor;
        }

        public List<RFQDetails> GetRFQDetails()
        {
            var details = _context.RfqDetails.ToList();
            return details;
        }
        public List<RFQGeneration> GetRFQ()
        {
            var rfq = _context.RfqGenerations.ToList();
            return rfq;
        }

        public async Task<bool> CreateRFQApprovalAsync(RFQGenerationModel rFQApproval)
        {
            try
            {
                string approvallimit = _config.GetSection("Approval:Limit").Value;
                // create a transaction scope

                using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted))
                {
                  
                   // var rfqLevel = _context.RfqApprovalConfigs.Where(x => x.ApprovalLevel == 1).First();
                    var rfqLevel = _context.RfqApprovalConfigs.ToList();
                    var rfq = _context.RfqGenerations.Where(x => x.Id == rFQApproval.RFQId).First();

                    //add to approval transactions
                    RFQApprovalTransactions rfqTransaction = new RFQApprovalTransactions
                    {
                        RFQId = rFQApproval.RFQId,
                        VendorId = rFQApproval.VendorId,
                        Comments = rFQApproval.Comments
                    };

              

                    await _context.AddAsync(rfqTransaction);

                    // add to approval status
                    RFQApprovalStatus rfqstatus = new RFQApprovalStatus
                    {
                        RFQId = rFQApproval.RFQId
                    };

                    string approvalEmail = "";
                    if (rFQApproval.TotalAmount <= Convert.ToInt32(approvallimit))
                    {
                        approvalEmail = rfqLevel.Where(x => x.ApprovalLevel == 2).First().Email;
                        rfqstatus.CurrentApprovalLevel = 2;
                        rfqTransaction.ApprovalLevel = 2;
                    }
                    else
                    {
                        approvalEmail = rfqLevel.Where(x => x.ApprovalLevel == 1).First().Email;
                        rfqstatus.CurrentApprovalLevel = 1;
                        rfqTransaction.ApprovalLevel = 1;
                    }

                    await _context.AddAsync(rfqstatus);


                    // update rfq generation
                    rfq.RFQStatus = "Pending Approval";

                    //send mail to approval
                    var subject = "RFQ APPROVAL NOTIFICATION";

                    var message = "A new RFQ has been sent for your approval.</br>";
                    message += "</br><b>RFQ Number  : </b>" + rfq.Id.ToString();
                    message += "</br><b>Vendor  : </b>" + rFQApproval.VendorName;
                    message += "</br>Kindly, log on to the Application and approve accordingly.";
                    message += "</br>Regards";

                    _context.Update<RFQGeneration>(rfq);
                    await _emailSender.SendEmailAsync(approvalEmail, subject, message,"");

                    //await _context.SaveChangesAsync();
                    transaction.Commit();

                    return true;
                }
                
               
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateRFQPendingApprovalAsync(RFQGenerationModel rFQApproval)
        {
            try
            {
                var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                //var currentUser = _contextAccessor.HttpContext.User.Identity;

                // create a transaction scope
                using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted))
                {
                    var currentLevel = _context.RfqApprovalStatuses.Where(x => x.RFQId == rFQApproval.RFQId).First();
                    
                    var ThisrfqLevel = _context.RfqApprovalConfigs.Where(x => x.ApprovalLevel == currentLevel.CurrentApprovalLevel).First();
                    var rfq = _context.RfqGenerations.Where(x => x.Id == rFQApproval.RFQId).First();
                    string approvalEmail = "";
                   

                    // check if approval is the final
                    if (ThisrfqLevel.IsFinalLevel)
                    {
                        //add to approval transactions
                        RFQApprovalTransactions rfqTransaction = new RFQApprovalTransactions
                        {
                            // ApprovalLevel = rfqLevel.ApprovalLevel,
                            RFQId = rFQApproval.RFQId,
                            VendorId = rFQApproval.VendorId,
                            ApprovalLevel = ThisrfqLevel.ApprovalLevel,
                            ApprovedBy = currentUser,
                            Comments = rFQApproval.Comments
                        };

                        await _context.AddAsync(rfqTransaction);
                        // await _context.AddAsync(rfqstatus);

                        rfq.RFQStatus = "Approved";
                        rfqTransaction.ApprovalStatus = "Approved";
                        _context.Update<RFQGeneration>(rfq);
                        await _context.SaveChangesAsync();
                        //approvalEmail = "";
                        // email 

                        //Send Email to Initiator
                        if (rfq.InitiatedBy != null)
                        {
                            var user = _reportRepository.GetUser().Where(u => u.Email == rfq.InitiatedBy).FirstOrDefault();
                            var message = "";
                            var subject = "PO NOTIFICATION";
                            message = "</br><b> Dear </b>" + user.FullName + "</br>";
                            message += "<br> Please be informed that Request For Quote for your request with Reference: " + rfq.Reference + " has been Approved";

                            message += "<br>Regards";

                            await _emailSender.SendEmailAsync(user.Email, subject, message, "");
                        }
                    }
                    else
                    {
                        var rfqLevel = _context.RfqApprovalConfigs.Where(x => x.ApprovalLevel == currentLevel.CurrentApprovalLevel + 1).First();
                        //add to approval transactions
                        RFQApprovalTransactions rfqTransaction = new RFQApprovalTransactions
                        {
                            // ApprovalLevel = rfqLevel.ApprovalLevel,
                            RFQId = rFQApproval.RFQId,
                            VendorId = rFQApproval.VendorId,
                            ApprovalLevel = ThisrfqLevel.ApprovalLevel,
                            ApprovedBy = currentUser,
                            Comments = rFQApproval.Comments
                        };

                        await _context.AddAsync(rfqTransaction);
                        // await _context.AddAsync(rfqstatus);

                        approvalEmail = rfqLevel.Email;

                        // add to approval status
                        currentLevel.RFQId = rFQApproval.RFQId;
                        currentLevel.CurrentApprovalLevel = rfqLevel.ApprovalLevel;


                        _context.Update<RFQApprovalStatus>(currentLevel);

                        //send mail to approval
                        var subject = "RFQ APPROVAL NOTIFICATION";

                        var message = "A new RFQ has been sent for your approval.</br>";
                        message += "</br><b>RFQ Number  : </b>" + rfq.Id.ToString();
                        message += "</br><b>Vendor  : </b>" + rFQApproval.VendorName;
                        message += "</br>Kindly, log on to the Application and approve accordingly.";
                        message += "</br>Regards";
                        await _emailSender.SendEmailAsync(approvalEmail, subject, message, "");
                    }
                    
                    //await _context.SaveChangesAsync();
                    transaction.Commit();

                    return true;
                }


            }
            catch (Exception ex)
            {
                return false;
            }
            // get approval level
        }

        public async Task<IEnumerable<RFQGenerationModel>> GetRFQApprovalDueAsync()
        {
            var ven = _context.Vendors.ToList();
            
            var des = _context.RfqDetails.ToList();

            var desList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                ItemName = x.ItemName
            }).GroupBy(v => new { v.RfqId, v.ItemName }).Select(s => s.FirstOrDefault());

            var descList = des.Select(x => new RfqGenModel
            {
                RfqId = x.RFQId,
                VendorId = x.VendorId,
                //VendorName = x.VendorName,
                Description = x.ItemDescription
            }).GroupBy(v => new { v.RfqId, v.Description }).Select(s => s.FirstOrDefault());

           
            var vendList = (from d in des
                            join v in ven on d.VendorId equals v.Id
                            select new RfqGenModel()
                            {
                                //Item = d.ItemName,
                                VendorId = v.Id,
                                RfqId = d.RFQId,
                                VendorName = v.VendorName,
                                VendorAddress = v.VendorAddress,
                                VendorEmail = v.Email,
                                ContactName = v.ContactName,
                                VendorStatus = v.VendorStatus,
                                PhoneNumber = v.PhoneNumber
                            }).GroupBy(v => new { v.RfqId, v.VendorName }).Select(s => s.FirstOrDefault());

           // var NewDes = string.Join(", ", descList.Select(u => u.Description)); 

            var query = await (from rfq in _context.RfqGenerations
                         join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                         //join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                         where rfq.EndDate <= DateTime.Now && rfq.RFQStatus == null
                         orderby rfq.Id, rfq.EndDate descending
                         select new RFQGenerationModel()
                         {
                              RFQId = rfq.Id,
                             ProjectId = rfq.ProjectId,
                             RequisitionId = rfq.RequisitionId,
                             Reference = rfq.Reference,
                             StartDate = rfq.StartDate,
                             EndDate = rfq.EndDate,
                             QuotedQuantity = rfqDetails.QuotedQuantity,

                             RFQStatus = rfq.RFQStatus,
                              Item = string.Join(", ", desList.Where(u => u.RfqId == rfqDetails.RFQId).Select(u => u.ItemName)),
                             
                             Description = string.Join(", ", descList.Where(u => u.RfqId == rfq.Id).Select(u => u.Description)),
                             //VendorName = vend.VendorName//vendList.Where(u => u.VendorId == vend.Id /*rfqDetails.VendorId*/ && u.RfqId == rfq.Id).Select(u => u.VendorName).FirstOrDefault(),
                         }).GroupBy(v => new { v.RFQId, v.Item }).Select(s => s.FirstOrDefault()).ToListAsync();//.Distinct().ToListAsync();


            return  query;
        }

        public async Task<IEnumerable<RFQGenerationModel>> GetRFQInPipelineAsync()
        {


            var query = await (from rfq in _context.RfqGenerations
                               join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                               join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                               where (rfq.EndDate >= DateTime.Now) && rfq.RFQStatus == null && rfqDetails.QuotedAmount > 0
                               orderby rfq.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   RFQId = rfq.Id,
                                   ProjectId = rfq.ProjectId,
                                   RequisitionId = rfq.RequisitionId,
                                   Reference = rfq.Reference,
                                   Description = rfq.Description,
                                   StartDate = rfq.StartDate,
                                   EndDate = rfq.EndDate,
                                   RFQStatus = rfq.RFQStatus
                               }).Distinct().ToListAsync();


            return query;
        }
        public async Task<IEnumerable<RFQGenerationModel>> GetSubmittedRFQByVendorsAsync(int RFQId)
        {

            var query = await (from rfq in _context.RfqGenerations
                               join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                               join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                               where rfq.RFQStatus == null && rfq.Id == RFQId && rfqDetails.QuotedAmount > 0
                               orderby rfq.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   RFQId = rfq.Id,
                                   ProjectId = rfq.ProjectId,
                                   RequisitionId = rfq.RequisitionId,
                                   Reference = rfq.Reference,
                                   Description = rfq.Description,
                                   StartDate = rfq.StartDate,
                                   EndDate = rfq.EndDate,
                                   RFQStatus = rfq.RFQStatus,
                                   VendorId = vend.Id,
                                   VendorName = vend.VendorName,
                                   VendorAddress = vend.VendorAddress,
                                   VendorStatus = vend.VendorStatus,
                                   ContactName = vend.ContactName,
                                   TotalAmount = _context.RfqDetails.Where(it => it.RFQId == rfq.Id && it.VendorId == rfqDetails.VendorId).Select(it => it.QuotedAmount).Sum()
                               }).Distinct().ToListAsync();


            return query;
        }

        public async Task<IEnumerable<RFQGenerationModel>> GetRFQByVendorsAsync(int RFQId)
        {

            var query = await(from rfq in _context.RfqGenerations
                              join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                              join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                              where rfq.RFQStatus == null && rfq.Id == RFQId 
                              orderby rfq.Id, rfq.EndDate descending
                              select new RFQGenerationModel()
                              {
                                  RFQId = rfq.Id,
                                  ProjectId = rfq.ProjectId,
                                  RequisitionId = rfq.RequisitionId,
                                  Reference = rfq.Reference,
                                  Description = rfq.Description,
                                  StartDate = rfq.StartDate,
                                  EndDate = rfq.EndDate,
                                  RFQStatus = rfq.RFQStatus,
                                  VendorId = vend.Id,
                                  VendorName = vend.VendorName,
                                  VendorAddress = vend.VendorAddress,
                                  VendorStatus = vend.VendorStatus,
                                  ContactName = vend.ContactName,
                                  TotalAmount = _context.RfqDetails.Where(it => it.RFQId == rfq.Id && it.VendorId == rfqDetails.VendorId).Select(it => it.QuotedAmount).Sum()
                              }).Distinct().ToListAsync();


            return query;
        }

        public async Task<RFQGenerationModel> GetRFQDetailsAsync(int RFQId, int VendorId)
        {
            var Item = await _context.RfqDetails.Where(x => x.RFQId == RFQId && x.VendorId == VendorId).ToListAsync();
            var totalAmount = Item.Sum(x => x.QuotedAmount);
            var query = await(from rfq in _context.RfqGenerations
                              join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                              join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                              where rfq.Id== RFQId && rfqDetails.VendorId == VendorId
                              orderby rfq.Id, rfq.EndDate descending
                              select new RFQGenerationModel()
                              {
                                  RFQId = rfq.Id,
                                  ProjectId = rfq.ProjectId,
                                  RequisitionId = rfq.RequisitionId,
                                  Reference = rfq.Reference,
                                  Description = rfq.Description,
                                  StartDate = rfq.StartDate,
                                  EndDate = rfq.EndDate,
                                  RFQStatus = rfq.RFQStatus,
                                  VendorId = vend.Id,
                                  VendorName = vend.VendorName,
                                  VendorAddress = vend.VendorAddress,
                                  VendorStatus = vend.VendorStatus,
                                  ContactName = vend.ContactName,
                                  VendorEmail=vend.Email,
                                  TotalAmount = totalAmount
                              }).Distinct().FirstOrDefaultAsync();

            List<RFQDetailsModel> rFQDetails = new List<RFQDetailsModel>();
               foreach (var item in Item)
                {
                 rFQDetails.Add(new RFQDetailsModel { RFQId = item.RFQId,
                                                        VendorId = item.VendorId, 
                                                        ItemId = item.ItemId,
                                                        ItemName = item.ItemName,
                                                        Description = item.ItemDescription,
                                                        QuotedQuantity = item.QuotedQuantity,
                                                        AgreedQuantity = item.AgreedQuantity,
                                                         QuotedPrice = item.QuotedPrice,
                                                         QuotedAmount = item.QuotedAmount,
                                                        AgreedAmount = item.AgreedAmount
                                                     }
                 );
                }

            query.RFQDetails = rFQDetails;


            return query;

        }

        public async Task<IEnumerable<RFQGenerationModel>> GetRFQPendingApprovalByVendorsAsync(int RFQId)
        {

            //get current logged on user

            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;




            var query = await (from vend in _context.Vendors
                               join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                               join transaction in _context.RfqApprovalTransactions on rfqDetails.VendorId equals transaction.VendorId
                               join approvalStatus in _context.RfqApprovalStatuses on transaction.RFQId equals approvalStatus.RFQId
                               join config in _context.RfqApprovalConfigs on approvalStatus.CurrentApprovalLevel equals config.ApprovalLevel
                               join rfq in _context.RfqGenerations on approvalStatus.RFQId equals rfq.Id
                               where config.UserId == int.Parse(currentUser) && rfq.RFQStatus == "Pending Approval" && rfq.Id == RFQId
                               orderby rfq.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   RFQId = rfq.Id,
                                   ProjectId = rfq.ProjectId,
                                   RequisitionId = rfq.RequisitionId,
                                   Reference = rfq.Reference,
                                   Description = rfq.Description,
                                   RFQStatus = rfq.RFQStatus,
                                   VendorId = vend.Id,
                                   VendorName = vend.VendorName,
                                   VendorAddress = vend.VendorAddress,
                                   VendorStatus = vend.VendorStatus,
                                   ContactName = vend.ContactName,
                                   TotalAmount = _context.RfqDetails.Where(it => it.RFQId == rfq.Id && it.VendorId == transaction.VendorId).Select(it => it.QuotedAmount).Sum()
                               }).Distinct().ToListAsync();
            var vendorId = query.FirstOrDefault().VendorId;

            var query1 = await (from vend in _context.Vendors
                                join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                                join rfq in _context.RfqGenerations on rfqDetails.RFQId equals rfq.Id
                                where rfq.Id == RFQId && rfqDetails.VendorId != vendorId
                                          orderby rfq.Id, rfq.EndDate descending
                                     select new RFQGenerationModel()
                                     {
                                         RFQId = rfq.Id,
                                         ProjectId = rfq.ProjectId,
                                         RequisitionId = rfq.RequisitionId,
                                         Reference = rfq.Reference,
                                         Description = rfq.Description,
                                         VendorId = vend.Id,
                                         VendorName = vend.VendorName,
                                         VendorAddress = vend.VendorAddress,
                                         VendorStatus = vend.VendorStatus,
                                         ContactName = vend.ContactName,
                                         TotalAmount = _context.RfqDetails.Where(it => it.RFQId == rfq.Id && it.VendorId == vend.Id).Select(it => it.QuotedAmount).Sum()
                                     }).Distinct().ToListAsync();

            var retQuery = query.Union(query1);
           // var returnQ = query.Distinct();

            return retQuery;
        }
        public async Task<IEnumerable<RFQGenerationModel>> GetRFQPendingApprovalAsync()
        {

            //get current logged on user
            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;


            var query = await (from vend in _context.Vendors
                               join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                               join transaction in _context.RfqApprovalTransactions on rfqDetails.VendorId equals transaction.VendorId
                               join approvalStatus in _context.RfqApprovalStatuses on transaction.RFQId equals approvalStatus.RFQId
                               join config in _context.RfqApprovalConfigs on approvalStatus.CurrentApprovalLevel equals config.ApprovalLevel
                               join rfq in _context.RfqGenerations on approvalStatus.RFQId equals rfq.Id
                               where config.UserId == int.Parse(currentUser) && rfq.RFQStatus == "Pending Approval"
                               orderby rfq.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   RFQId = rfq.Id,
                                   ProjectId = rfq.ProjectId,
                                   RequisitionId = rfq.RequisitionId,
                                   Reference = rfq.Reference,
                                   Description = rfq.Description,
                                   StartDate = rfq.StartDate,
                                   EndDate = rfq.EndDate,
                                   RFQStatus = rfq.RFQStatus,
                                   VendorId = transaction.VendorId
                               }).GroupBy(v => new { v.RFQId, v.Item }).Select(s => s.FirstOrDefault()).ToListAsync();//.Distinct().ToListAsync();


            return query;
        }

        public List<RFQGenerationModel> GetRFQPendingApproval()
        {

            //get current logged on user
            var currentUser = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var InitiatedVendor = _context.RfqApprovalTransactions.ToList();

            var query =  (from vend in _context.Vendors
                               join rfqDetails in _context.RfqDetails on vend.Id equals rfqDetails.VendorId
                               join transaction in _context.RfqApprovalTransactions on rfqDetails.VendorId equals transaction.VendorId
                               join approvalStatus in _context.RfqApprovalStatuses on transaction.RFQId equals approvalStatus.RFQId
                               join config in _context.RfqApprovalConfigs on approvalStatus.CurrentApprovalLevel equals config.ApprovalLevel
                               join rfq in _context.RfqGenerations on approvalStatus.RFQId equals rfq.Id
                               where config.UserId == int.Parse(currentUser) && rfq.RFQStatus == "Pending Approval"
                               orderby rfq.Id, rfq.EndDate descending
                               select new RFQGenerationModel()
                               {
                                   RFQId = rfq.Id,
                                   ProjectId = rfq.ProjectId,
                                   RequisitionId = rfq.RequisitionId,
                                   Reference = rfq.Reference,
                                   Description = rfq.Description,
                                   StartDate = rfq.StartDate,
                                   EndDate = rfq.EndDate,
                                   RFQStatus = rfq.RFQStatus,
                                   //VendorId = InitiatedVendor.Where(u => u.RFQId == rfq.Id).Select(u => u.VendorId).FirstOrDefault()
                               }).GroupBy(v => new { v.RFQId, v.Item }).Select(s => s.FirstOrDefault()).ToList();//.Distinct().ToListAsync();


            return query;
        }
    }
}
