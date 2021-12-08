using AutoMapper;
using ELMS.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace ELMS.Web.Areas.Admin.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, RoleViewModel>().ReverseMap();
        }
    }
}