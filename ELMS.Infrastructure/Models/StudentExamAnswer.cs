using ELMS.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ELMS.Infrastructure.Models
{
    public partial class StudentExamAnswer
    {
        public int Id { get; set; }
        [Display(Name = "Exam")]
        [Required]
        public int ExamId { get; set; }
        [Display(Name = "Student")]
        [Required]
        public string StudentId { get; set; }
        [Display(Name = "Obtained Score")]
        [Required]
        public int ObtainedScore { get; set; }
        [Display(Name = "Answer Sheet")]
        [Required]
        public string AnswerSheet { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public virtual Exam Exam { get; set; }
        [Display(Name = "File")]
        [NotMapped]
        public IFormFile FormFile { get; set; }
    }
}
