using AutoMapper;
using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Web.Areas.Admin.Models;

namespace ELMCOM.Web.Areas.Admin.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
        }
    }
}