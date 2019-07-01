using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.StateModel;

namespace E_Procurement.Repository.StateRepo
{
    public class StateRepository : IStateRepository
    {
        private readonly EProcurementContext _context;
      

        public StateRepository(EProcurementContext context)
        {
            _context = context;
        }
        public bool CreateState(StateModel model, out string Message)
        {
            var confirm = _context.States.Where(x => x.StateName == model.StateName).Count();

            State state = new State();

            if (confirm == 0)
            {

                state.StateName = model.StateName;

                state.IsActive = true;

                state.CreatedBy = model.CreatedBy;

                state.DateCreated = DateTime.Now;

                _context.Add(state);

                _context.SaveChanges();

                Message = "State created successfully";

                return true;
            }
            else
            {
                Message = "State already exist";

                return false;
            }

        }

        public bool UpdateState(StateModel model, out string Message)
        {

            var confirm = _context.States.Where(x => x.StateName == model.StateName && x.IsActive == model.IsActive).Count();

            var oldEntry = _context.States.Where(u => u.Id == model.Id).FirstOrDefault();

            if (oldEntry == null)
            {
                throw new Exception("No state exists with this Id");
            }

            if (confirm == 0)
            {

                oldEntry.StateName = model.StateName;

                oldEntry.IsActive = model.IsActive;

                oldEntry.UpdatedBy = model.UpdatedBy;

                oldEntry.LastDateUpdated = DateTime.Now;

                _context.SaveChanges();

                Message = "State updated successfully";

                return true;
            }
            else
            {
                Message = "State already exist";

                return false;
            }

        }

        public IEnumerable<State> GetStates()
        {
            return _context.States.OrderByDescending(u => u.Id).ToList();
        }
    }
}
