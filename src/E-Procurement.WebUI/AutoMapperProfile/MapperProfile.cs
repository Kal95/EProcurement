using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.AccountModel;
using E_Procurement.WebUI.Models.PermissionModel;
using E_Procurement.WebUI.Models.VendorModel;

namespace E_Procurement.WebUI.AutoMapperProfile
{
    public class MapperProfile:Profile

    {

        public MapperProfile()
        {
            CreateMap<VendorModel, Vendor>().ReverseMap();

            CreateMap<UserViewModel, User>().ReverseMap();

            CreateMap<RegisterViewModel, User>().ReverseMap();

            CreateMap<RoleViewModel, Role>().ReverseMap();

            CreateMap<PermissionViewModel, Permission>().ReverseMap();



        }
    }
}
