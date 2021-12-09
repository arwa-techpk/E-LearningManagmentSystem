using AutoMapper;
using ELMCOM.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace ELMCOM.Web.Areas.Admin.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, RoleViewModel>().ReverseMap();
        }
    }
}