using AutoMapper;
using ELMCOM.Web.Areas.Admin.Models;
using System.Security.Claims;

namespace ELMCOM.Web.Areas.Admin.Mappings
{
    public class ClaimsProfile : Profile
    {
        public ClaimsProfile()
        {
            CreateMap<Claim, RoleClaimsViewModel>().ReverseMap();
        }
    }
}