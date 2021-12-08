using ELMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;
// why don't using Apllication user class ? 
namespace ELMS.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; } = true;
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The Password and Confirm Password fields do not match.")]
        public string ConfirmPassword { get; set; }
        public string ContactNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public int? SchoolId { get; set; }
        public School School { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }
        public SelectList Schools { get; internal set; }
    }
}