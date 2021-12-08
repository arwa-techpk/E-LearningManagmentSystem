using ELMS.Application.Constants;
using ELMS.Application.Enums;
using ELMS.Infrastructure.DbContexts;
using ELMS.Infrastructure.Identity.Models;
using ELMS.Web.Abstractions;
using ELMS.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ELMS.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : BaseController<UserController>
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [Authorize(Policy = Permissions.Users.View)]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> LoadAll()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);


            if (User.IsInRole("SuperAdmin"))
            {
                var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Admin.ToString());
                allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.Id != currentUser.Id)
                    .ToList();
                var model = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                return PartialView("_ViewAll", model);

            }
            else
            {

                var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id && a.SchoolId == currentUser.SchoolId).ToListAsync();
                var model = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                return PartialView("_ViewAll", model);
            }

        }

        public async Task<IActionResult> OnGetCreate()
        {
            UserViewModel userViewModel = new UserViewModel();
            if (User.IsInRole("SuperAdmin"))
            {

                var schoolsList = new SelectList(_context.School, "Id", "Name");

                userViewModel.Schools = schoolsList;
            }
            else
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                userViewModel.SchoolId = currentUser.SchoolId;


            }
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_Create", userViewModel) });
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreate(UserViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                MailAddress address = new MailAddress(userModel.Email);
                string userName = address.User;
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userModel.Email,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    EmailConfirmed = true,
                    DateOfBirth = userModel.DateOfBirth,
                    ContactNumber = userModel.ContactNumber,
                    Gender = userModel.Gender,
                    SchoolId = userModel.SchoolId
                };
                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Admin.ToString());

                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    string htmlData = string.Empty;
                    if (User.IsInRole("SuperAdmin"))
                    {
                        var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Admin.ToString());
                        allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.Id != currentUser.Id && a.SchoolId == currentUser.SchoolId)
                            .ToList();
                        var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                        htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);

                    }
                    else
                    {

                        var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id && a.SchoolId == currentUser.SchoolId).ToListAsync();
                        var users = _mapper.Map<IEnumerable<UserViewModel>>(allUsersExceptCurrentUser);
                        htmlData = await _viewRenderer.RenderViewToStringAsync("_ViewAll", users);
                    }

                    _notify.Success($"Account for {user.Email} created.");
                    return new JsonResult(new { isValid = true, html = htmlData });
                }
                foreach (var error in result.Errors)
                {
                    _notify.Error(error.Description);
                }
                var html = await _viewRenderer.RenderViewToStringAsync("_Create", userModel);
                return new JsonResult(new { isValid = false, html = html });
            }
            return default;
        }
    }
}