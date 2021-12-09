using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Web.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ELMCOM.Web.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]


    public class HomeController : BaseController<HomeController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            _notify.Information("Welcome " + currentUser.FirstName + " " + currentUser.LastName);
            return View();
        }
    }
}