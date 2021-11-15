using ELMS.Infrastructure.Identity.Models;
using ELMS.Web.Areas.Admin.Models;
using AutoMapper;

namespace ELMS.Web.Areas.Admin.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
        }
    }
}