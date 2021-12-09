using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Infrastructure.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ELMCOM.Web.Areas.Test.Models
{
    public class AssignmentSubmissionViewModel
    {
        public int? StudentAssignmentId { get; set; }
        [Display(Name = "Student")]
        [Required]
        public string StudentId { get; set; }
        [Display(Name = "Submission Date")]
        [Required]
        public DateTime? SubmissionDate { get; set; }
        [Display(Name = "Assignment Details")]
        [Required]
        public string AssignmentDetails { get; set; }
        [Display(Name = "Obtain Score")]
        [Required]
        public int? ObtainScore { get; set; }
        [Display(Name = "Assignment")]
        [Required]
        public int AssignmentId { get; set; }
        public ApplicationUser Student { get; set; }
        public Assignment Assignment { get; set; }
        public DateTime LastDateToSubmit { get; internal set; }
    }
}
