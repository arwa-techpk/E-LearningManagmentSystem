using ELMS.Infrastructure.Identity.Models;
using ELMS.Web.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ELMS.Web.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]

     
    public class HomeController : BaseController<HomeController>
    {
        private readonly UserManager<ApplicationUser> _userManger;

        public HomeController(UserManager<ApplicationUser> userManger)
        {
            _userManger = userManger;
        }
        public async Task<IActionResult> Index()
        {
            var currentuser = await _userManger.GetUserAsync(HttpContext.User);
            _notify.Information("Hi" + currentuser.FirstName + "" + currentuser.LastName);
            return View(currentuser);
        }
    }
}