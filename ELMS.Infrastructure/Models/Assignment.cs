using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ELMS.Infrastructure.Models
{
    public partial class Assignment
    {
        public Assignment()
        {
            StudentAssignments = new HashSet<StudentAssignment>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Submission Date")]
        [Required]
        public DateTime SubmissionDate { get; set; }
        [Display(Name = "Assignment File")]

        public string AssignmentFile { get; set; }
        [Display(Name = "Total Score")]
        [Required]
        public int TotalScore { get; set; }
        [Display(Name = "Course")]
        [Required]
        public int CourseId { get; set; }

        [Display(Name = "File")]
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
    }
}
