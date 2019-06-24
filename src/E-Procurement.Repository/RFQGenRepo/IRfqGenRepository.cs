using System;
using System.Collections.Generic;
using System.Text;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.Interface;
using E_Procurement.WebUI.Models.RFQModel;

namespace E_Procurement.Repository.RFQGenRepo
{
    public interface IRfqGenRepository : IDependencyRegister
    {
        List<RFQGeneration> GetRfqGen();
        bool CreateRfqGen(RfqGenModel model, out string Message);
        bool UpdateRfqGen(RfqGenModel model, out string Message);
        List<Vendor> GetVendors(RfqGenModel model);
        List<RfqGenModel> GetItemCategory();
        List<RfqGenModel> GetItem(int CategoryId);
        List<Vendor> GetVendorDetails();

    }
}
