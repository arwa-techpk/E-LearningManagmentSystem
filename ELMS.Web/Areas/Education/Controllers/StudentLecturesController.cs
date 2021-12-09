using ELMCOM.Infrastructure.DbContexts;
using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Infrastructure.Models;
using ELMCOM.Web.Abstractions;
using ELMCOM.Web.Areas.Education.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ELMCOM.Web.Areas.Education.Controllers
{
    [Area("Education")]
    public class StudentLecturesController : BaseController<StudentLecturesController>
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;


        public StudentLecturesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Education/StudentLectures
        public async Task<IActionResult> Index()
        {

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // linq query method, returns all courses for this student || inner join
            var studentCourses = _context.StudentCourses
                .Include(c => c.Course)
                .Where(m => m.StudentId == currentUser.Id);

            // linq query syntax
            var applicationDbContext = (from i in _context.Lectures // lectures
                                        join e in studentCourses on i.CourseId equals e.Id // Student courses
                                        join sa in _context.StudentLectures on i.Id equals sa.LectureId // this will return, user has attended or not
                                        into courseTemp // it will be null, if he hasn't attended. Left JOIN
                                        from c in courseTemp.DefaultIfEmpty()
                                        where e.StudentId == currentUser.Id

                                        select new StudentAttendenceViewModel()
                                        {
                                            LectureId = i.Id,
                                            LectureTitle = i.Title,
                                            LectureDuration = i.Duration,
                                            LectureDate = i.LectureDate,
                                            LectureLink = i.LectureJoinURL,
                                            CourseName = e.Course.Title,
                                            HasAttended = c.Id != null,
                                            StudentId = c.StudentId
                                        });



            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> AttendLecture(int LectureId, string StudentId, string LectureLink)
        {

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var studentCourses = await _context.StudentLectures
                .FirstOrDefaultAsync(c => c.LectureId == LectureId && c.StudentId == currentUser.Id);

            if (studentCourses == null)
            {

                StudentLecture studentLecture = new StudentLecture()
                {
                    LectureId = LectureId,
                    StudentId = currentUser.Id
                };
                _context.Add(studentLecture);
                await _context.SaveChangesAsync();
            }



            return RedirectToAction(nameof(Index));
        }

    }
}
