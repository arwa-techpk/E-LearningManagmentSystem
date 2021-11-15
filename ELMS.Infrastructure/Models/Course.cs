using ELMS.Domain.Entities;
using ELMS.Infrastructure.Identity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ELMS.Infrastructure.Models
{
    public partial class Course
    {
        public Course()
        {
            Assignments = new HashSet<Assignment>();
            Exams = new HashSet<Exam>();
            Lectures = new HashSet<Lecture>();
            StudentCourses = new HashSet<StudentCourse>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        [Display(Name = "Teacher")]
        [Required]
        public string TeacherId { get; set; }
        [Display(Name = "School")]
        [Required]
        public int SchoolId { get; set; }
        [Display(Name = "Credit")]
        [Required]
        public int Credit { get; set; }
        public string Description { get; set; }
        public virtual ApplicationUser Teacher { get; set; }
        public virtual School School { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Exam> Exams { get; set; }
        public virtual ICollection<Lecture> Lectures { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
