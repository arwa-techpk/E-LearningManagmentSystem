using ELMS.Infrastructure.Identity.Models;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ELMS.Infrastructure.Models
{
    public partial class StudentLecture
    {
        public int Id { get; set; }
        [Display(Name = "Student")]
        [Required]
        public string StudentId { get; set; }
        [Display(Name = "Lecture")]
        [Required]
        public int LectureId { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public virtual Lecture Lecture { get; set; }
    }
}
