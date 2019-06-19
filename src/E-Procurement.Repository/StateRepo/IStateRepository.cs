using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.StateModel;

namespace E_Procurement.Repository.StateRepo
{
  public  interface IStateRepository : IDependencyRegister
    {
        IEnumerable<State> GetStates();
        bool CreateState(StateModel model,  out string Message);

        bool UpdateState(StateModel model, out string Message);
    }
}
