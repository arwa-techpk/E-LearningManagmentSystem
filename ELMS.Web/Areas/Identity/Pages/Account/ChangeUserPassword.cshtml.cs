using ELMS.Application.Constants;
using ELMS.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ELMS.Web.Areas.Identity.Pages.Account
{
    [Authorize(Policy = Permissions.Users.View)]
    [Authorize(Roles = "SuperAdmin")]
    public class ChangeUserPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangeUserPasswordModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }



        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            public string Id { get; set; }

        }

        public IActionResult OnGet(string userId)
        {
            if (User.IsInRole("SuperAdmin"))
            {
                if (userId == null)
                {
                    return BadRequest("A userId must be supplied for password reset.");
                }
                else
                {
                    Input = new InputModel
                    {
                        Id = userId
                    };
                    return Page();
                }


            }
            else return RedirectToAction("Index", "Home", new { area = "Dashboard" });

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, Input.Password);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {

                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

    }
}
