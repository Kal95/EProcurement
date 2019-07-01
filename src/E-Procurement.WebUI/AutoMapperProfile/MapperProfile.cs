using AutoMapper;
using E_Procurement.Data.Entity;
using E_Procurement.WebUI.Models.AccountModel;
using E_Procurement.WebUI.Models.PermissionModel;
using E_Procurement.WebUI.Models.StateModel;
using E_Procurement.WebUI.Models.CountryModel;
using E_Procurement.WebUI.Models.BankModel;
using E_Procurement.WebUI.Models.VendorCategoryModel;
using E_Procurement.WebUI.Models.RFQModel;
using E_Procurement.Repository.VendoRepo;
using E_Procurement.WebUI.Models.RfqApprovalModel;
using E_Procurement.Repository.Dtos;
using System.Collections.Generic;

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

            CreateMap<UserViewModel, User>().ReverseMap();

            CreateMap<RegisterViewModel, User>().ReverseMap();

            CreateMap<RoleViewModel, Role>().ReverseMap();

            CreateMap<PermissionViewModel, Permission>().ReverseMap();

            CreateMap<RFQApprovalConfigModel, RFQApprovalConfig>().ReverseMap();


            CreateMap<RFQGenerationModel, RFQGenerationViewModel>().ReverseMap();

            CreateMap<RFQDetailsModel, RFQDetailsViewModel>().ReverseMap();

            CreateMap<List<RFQDetailsModel>, List<RFQDetailsViewModel>>().ReverseMap();


        }
    }
}
