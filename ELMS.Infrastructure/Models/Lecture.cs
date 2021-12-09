using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ELMCOM.Infrastructure.Models
{
    public partial class Lecture
    {
        public Lecture()
        {
            StudentLectures = new HashSet<StudentLecture>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        [Display(Name = "Course")]
        [Required]
        public int CourseId { get; set; }
        [Display(Name = "Date  & Time")]
        [Required]
        public DateTime LectureDate { get; set; }

        [Display(Name = "Duration (Minutes)")]
        [Required]
        public string Duration { get; set; }

        public string LectureJoinURL { get; set; }

        // lets add the zoom link/ google meeting link here? // generate the zoom meeting link auto
        // whenever student clik that link, his attendence will automatically marked 
        // Add the Fignma UI
        public virtual Course Course { get; set; }
        public virtual ICollection<StudentLecture> StudentLectures { get; set; }
    }
}
