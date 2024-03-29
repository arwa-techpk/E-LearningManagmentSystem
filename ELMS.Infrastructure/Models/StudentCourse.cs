﻿using ELMCOM.Infrastructure.Identity.Models;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ELMCOM.Infrastructure.Models
{
    public partial class StudentCourse
    {
        public int Id { get; set; }
        [Display(Name = "Student")]
        [Required]
        public string StudentId { get; set; }
        [Display(Name = "Course")]
        [Required]
        public int CourseId { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public virtual Course Course { get; set; }
    }
}
