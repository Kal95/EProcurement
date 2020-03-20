using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.RequisitionModel;
using E_Procurement.WebUI.Models.StateModel;

namespace E_Procurement.Repository.RequisitionRepo
{
    public class RequisitionRepository : IRequisitionRepository
    {
        private readonly EProcurementContext _context;
      

        public RequisitionRepository(EProcurementContext context)
        {
            _context = context;
        }


        public List<Requisition> GetRequisitions()
        {
            var transac = _context.Requisitions.Where(a => a.IsActive == true).OrderByDescending(a => a.Id).ToList();
            return transac;
        }
        public List<Requisition> GetRequisitions2()
        {
            var transac = _context.Requisitions.OrderByDescending(a => a.Id).ToList();
            return transac;
        }
        public bool CreateRequisition(RequisitionModel model, out string Message)
        {
            var confirm = _context.Requisitions.Where(x => x.Initiator == model.Initiator && x.RequisitionDocument == model.RequisitionDocumentPath).Count();

            Requisition requisition = new Requisition();


            if (confirm == 0)
            {

                requisition.Initiator = model.Initiator;
                requisition.DateCreated = DateTime.Now;
                requisition.IsActive = model.IsActive;
                requisition.RequisitionDocument = model.RequisitionDocumentPath;
                requisition.ExpectedDate = model.ExpectedDate;
                requisition.Description = model.Description;
                requisition.UpdatedBy = model.UpdatedBy;
                requisition.DateUpdated = model.DateUpdated;

                _context.Requisitions.Add(requisition);

                _context.SaveChanges();

                Message = "Requisition created successfully";

                return true;
            }
            else
            {
                Message = "Requisition already exist";

                return false;
            }

        }
        public bool UpdateRequisition(RequisitionModel model, out string Message)
        {

            var confirm = _context.Requisitions.Where(x => x.Id == model.Id && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.Requisitions.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No Requisition exists with this Id");
            }

            if (confirm == 0)
            {
                oldEntry.IsActive = model.IsActive;

                oldEntry.UpdatedBy = model.UpdatedBy;

                oldEntry.DateUpdated = DateTime.Now;

                _context.SaveChanges();

                Message = "Requisition updated successfully";

                return true;
            }
            else
            {
                Message = "Requisition already exist";

                return false;
            }

        }
    }
}
