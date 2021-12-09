using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ELMCOM.Infrastructure.Models
{
    public partial class Exam
    {
        public Exam()
        {
            StudentExamAnswers = new HashSet<StudentExamAnswer>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Course")]
        [Required]
        public int CourseId { get; set; }
        public int Duration { get; set; }
        [Display(Name = "Exam Date")]
        [Required]
        public DateTime ExamDate { get; set; }
        [Display(Name = "Total Score")]
        [Required]
        public int TotalScore { get; set; }
        [Display(Name = "Exam Questionnaire")]

        public string ExamPaper { get; set; }
        [Display(Name = "File")]
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<StudentExamAnswer> StudentExamAnswers { get; set; }
    }
}
