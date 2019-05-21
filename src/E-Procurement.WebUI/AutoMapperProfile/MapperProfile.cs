using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.VendorModel;

namespace E_Procurement.WebUI.AutoMapperProfile
{
    public class MapperProfile:Profile

    {

        public MapperProfile()
        {
            CreateMap<VendorModel, Vendor>().ReverseMap();
        }
    }
}
