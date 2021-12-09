using ELMCOM.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ELMCOM.Infrastructure.Models
{
    public partial class StudentAssignment
    {
        public int Id { get; set; }
        [Display(Name = "Student")]
        [Required]
        public string StudentId { get; set; }
        [Display(Name = "Submission Date")]
        [Required]
        public DateTime SubmissionDate { get; set; }
        [Display(Name = "Assignment Details")]
        [Required]
        public string AssignmentDetails { get; set; }
        [Display(Name = "Obtain Score")]
        [Required]
        public int ObtainScore { get; set; }
        [Display(Name = "Assignment")]
        [Required]
        public int AssignmentId { get; set; }
        [Display(Name = "File")]
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public virtual Assignment Assignment { get; set; }
    }
}
