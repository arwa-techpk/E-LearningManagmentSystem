using ELMS.Domain.Entities;
using ELMS.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ELMS.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
       

        //public int UsernameChangeLimit { get; set; } = 10;
        public byte[] ProfilePicture { get; set; }
        public bool IsActive { get; set; } = true;

        public int? SchoolId { get; set; }

        public virtual School School { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
        public virtual ICollection<StudentExamAnswer> StudentExamAnswers { get; set; }
        public virtual ICollection<StudentLecture> StudentLectures { get; set; }

    }
}