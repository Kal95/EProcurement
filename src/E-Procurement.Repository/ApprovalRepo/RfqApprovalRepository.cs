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

namespace E_Procurement.Repository.RfqApprovalConfigRepository
{
   public class RfqApprovalRepository : IRfqApprovalRepository
    {
        private readonly EProcurementContext _context;
        private readonly ISMTPService _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;
        private IConfiguration _config;

        public RfqApprovalRepository(EProcurementContext context, IConfiguration config, ISMTPService emailSender, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _emailSender = emailSender;
            _config = config;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> CreateRFQApprovalAsync(RFQGenerationModel rFQApproval)
        {
            try
            {
                string approvallimit = _config.GetSection("Approval:Limit").Value;
                // create a transaction scope

                using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted))
                {
                  
                    var rfqLevel = _context.RfqApprovalConfigs.Where(x => x.ApprovalLevel == 1).First();
                    var rfq = _context.RfqGenerations.Where(x => x.Id == rFQApproval.RFQId).First();

                    //add to approval transactions
                    RFQApprovalTransactions rfqTransaction = new RFQApprovalTransactions
                    {
                        ApprovalLevel = rfqLevel.ApprovalLevel,
                        RFQId = rFQApproval.RFQId,
                        VendorId = rFQApproval.VendorId,
                        Comments = rFQApproval.Comments
                    };

              

                    await _context.AddAsync(rfqTransaction);

                    // add to approval status
                    RFQApprovalStatus rfqstatus = new RFQApprovalStatus
                    {
                        CurrentApprovalLevel = rfqLevel.ApprovalLevel,
                        RFQId = rFQApproval.RFQId
                    };
                    if (rFQApproval.TotalAmount <= Convert.ToInt32(approvallimit))
                    {
                        rfqstatus.CurrentApprovalLevel = 2;
                        rfqTransaction.ApprovalLevel = 2;
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
                    await _emailSender.SendEmailAsync(rfqLevel.Email, subject, message,"");

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
                    var rfqLevel = _context.RfqApprovalConfigs.Where(x => x.ApprovalLevel == currentLevel.CurrentApprovalLevel + 1).First();                    
                    var rfq = _context.RfqGenerations.Where(x => x.Id == rFQApproval.RFQId).First();
                    string approvalEmail = "";
                    //add to approval transactions
                    RFQApprovalTransactions rfqTransaction = new RFQApprovalTransactions
                    {
                       // ApprovalLevel = rfqLevel.ApprovalLevel,
                        RFQId = rFQApproval.RFQId,
                        VendorId = rFQApproval.VendorId,
                        ApprovalLevel = rfqLevel.ApprovalLevel,
                        ApprovedBy = currentUser,
                        Comments = rFQApproval.Comments
                    };

                    approvalEmail = rfqLevel.Email;

                    // add to approval status
                    currentLevel.RFQId = rFQApproval.RFQId;
                    currentLevel.CurrentApprovalLevel = rfqLevel.ApprovalLevel;
                   

                    _context.Update<RFQApprovalStatus>(currentLevel); 

                    await _context.AddAsync(rfqTransaction);
                   // await _context.AddAsync(rfqstatus);

                    // check if approval is the final
                    if (rfqLevel.IsFinalLevel)
                    {
                       
                        rfq.RFQStatus = "Approved";
                        rfqTransaction.ApprovalStatus = "Approved";
                        _context.Update<RFQGeneration>(rfq);
                        await _context.SaveChangesAsync();
                        //approvalEmail = "";
                        // email 
                    }
                    else
                    {
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
  

            var query = await (from rfq in _context.RfqGenerations
                         join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                         join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                         where rfq.EndDate <= DateTime.Now && rfq.RFQStatus == null
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


            return  query;
        }

        public async Task<IEnumerable<RFQGenerationModel>> GetRFQByVendorsAsync(int RFQId)
        {

            var query = await(from rfq in _context.RfqGenerations
                              join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                              join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                              where rfq.EndDate <= DateTime.Now && rfq.RFQStatus == null && rfq.Id == RFQId

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
                              }).ToListAsync();


            return query;
        }

        public async Task<RFQGenerationModel> GetRFQDetailsAsync(int RFQId, int VendorId)
        {
            var Item = await _context.RfqDetails.Where(x => x.RFQId == RFQId && x.VendorId == VendorId).ToListAsync();
            var totalAmount = Item.Sum(x => x.QuotedAmount);
            var query = await(from rfq in _context.RfqGenerations
                              join rfqDetails in _context.RfqDetails on rfq.Id equals rfqDetails.RFQId
                              join vend in _context.Vendors on rfqDetails.VendorId equals vend.Id
                              where rfq.EndDate <= DateTime.Now && rfq.Id== RFQId && rfqDetails.VendorId == VendorId
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
                                                        QuotedQuantity = item.QuotedQuantity,
                                                        AgreedQuantity = item.AgreedQuantity, 
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
                                     }).ToListAsync();

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
                                   RFQStatus = rfq.RFQStatus
                               }).Distinct().ToListAsync();


            return query;
        }

    }
}
