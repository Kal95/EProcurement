using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;

namespace E_Procurement.Repository.StateRepo
{
  public  interface IStateRepository : IDependencyRegister
    {
        IEnumerable<State> GetStates();
        bool CreateState(string StateName, string UserId, out string Message);

        bool UpdateState(int Id, string StateName, bool IsActive, string UserId, out string Message);
    }
}
