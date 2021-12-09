using System.ComponentModel.DataAnnotations;

namespace ELMCOM.Application.DTOs.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}