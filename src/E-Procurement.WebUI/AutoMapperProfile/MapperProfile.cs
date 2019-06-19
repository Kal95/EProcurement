using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.StateModel;
using E_Procurement.WebUI.Models.CountryModel;
using E_Procurement.WebUI.Models.BankModel;
using E_Procurement.WebUI.Models.VendorCategoryModel;
using E_Procurement.WebUI.Models.RFQModel;
using E_Procurement.Repository.VendoRepo;

namespace E_Procurement.WebUI.AutoMapperProfile
{
    public class MapperProfile:Profile

    {

        public MapperProfile()
        {
            CreateMap<VendorModel, Vendor>().ReverseMap();

            CreateMap<StateModel, State>().ReverseMap();

            CreateMap<CountryModel, Country>().ReverseMap();

            CreateMap<BankModel, Bank>().ReverseMap();

            CreateMap<VendorCategoryModel, VendorCategory>().ReverseMap();

            CreateMap<RfqGenModel, RFQGeneration>().ReverseMap();



        }
    }
}
