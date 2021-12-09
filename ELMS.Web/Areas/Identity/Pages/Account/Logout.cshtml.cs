using ELMCOM.Application.Interfaces.Shared;
using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Web.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ELMCOM.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : BasePageModel<LogoutModel>
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthenticatedUserService _userService;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, IMediator mediator, IAuthenticatedUserService userService)
        {
            _signInManager = signInManager;

            _mediator = mediator;
            _userService = userService;
        }

        public async Task<IActionResult> OnGet()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            await _signInManager.SignOutAsync();

            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _notyf.Information("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}