using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ELMS.Infrastructure.Models
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
        public TimeSpan Duration { get; set; }
        [Display(Name = "Exam Date")]
        [Required]
        public DateTime ExamDate { get; set; }
        [Display(Name = "Total Score")]
        [Required]
        public int TotalScore { get; set; }
        [Display(Name = "Exam Questionnaire")]
        [Required]
        public string ExamPaper { get; set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<StudentExamAnswer> StudentExamAnswers { get; set; }
    }
}
