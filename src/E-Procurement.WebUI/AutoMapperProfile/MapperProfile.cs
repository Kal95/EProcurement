using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.AccountModel;
using E_Procurement.WebUI.Models.PermissionModel;
using E_Procurement.WebUI.Models.StateModel;
using E_Procurement.WebUI.Models.VendorModel;
using E_Procurement.WebUI.Models.CountryModel;
using E_Procurement.WebUI.Models.BankModel;
using E_Procurement.WebUI.Models.VendorCategoryModel;

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



            CreateMap<UserViewModel, User>().ReverseMap();

            CreateMap<RegisterViewModel, User>().ReverseMap();

            CreateMap<RoleViewModel, Role>().ReverseMap();

            CreateMap<PermissionViewModel, Permission>().ReverseMap();



        }
    }
}
