using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.RequisitionModel;
using E_Procurement.WebUI.Models.StateModel;

namespace E_Procurement.Repository.RequisitionRepo
{
  public  interface IRequisitionRepository : IDependencyRegister
    {
        List<Requisition> GetRequisitions();
        bool CreateRequisition(RequisitionModel model, out string Message);
        bool UpdateRequisition(RequisitionModel model, out string Message);
        List<Requisition> GetRequisitions2();
    }
}
