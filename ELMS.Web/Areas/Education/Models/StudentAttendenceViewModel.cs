using System;

namespace ELMCOM.Web.Areas.Education.Models
{
    public class StudentAttendenceViewModel
    {
        public int LectureId { get; internal set; }
        public string CourseName { get; internal set; }
        public string LectureLink { get; internal set; }
        public bool HasAttended { get; internal set; }
        public string StudentId { get; internal set; }
        public string LectureDuration { get; internal set; }
        public string LectureTitle { get; internal set; }
        public DateTime LectureDate { get; internal set; }
    }
}
