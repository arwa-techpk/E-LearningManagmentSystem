using ELMS.Infrastructure.Identity.Models;
using ELMS.Infrastructure.Models;
using System;

namespace ELMS.Web.Areas.Test.Models
{
    public class ExamAnswerSubmissionViewModel
    {
        public int ExamId { get; internal set; }
        public int? StudentExamId { get; internal set; }
        public Exam Exam { get; internal set; }

        public string StudentId { get; internal set; }
        public ApplicationUser Student { get; internal set; }
        public DateTime ExamDate { get; internal set; }
        public DateTime? SubmissionDate { get; internal set; }
        public int? ObtainedScore { get; internal set; }
        public string AnswerSheet { get; internal set; }
    }
}
