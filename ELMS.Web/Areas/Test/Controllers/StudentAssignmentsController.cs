using ELMS.Application.Enums;
using ELMS.Infrastructure.DbContexts;
using ELMS.Infrastructure.Identity.Models;
using ELMS.Infrastructure.Models;
using ELMS.Web.Abstractions;
using ELMS.Web.Areas.Test.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ELMS.Web.Areas.Test.Controllers
{
    [Area("Test")]
    public class StudentAssignmentsController : BaseController<StudentAssignmentsController>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentAssignmentsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Test/StudentAssignments
        // Create a page for students to view the assignments , so they can submit the assignment after seeing the details
        // remove the score, studentid, assignment id from assignment submission view
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (await _userManager.IsInRoleAsync(currentUser, "Student"))
            {
                var studentCourses = _context.StudentCourses.Include(c => c.Course).Where(m => m.StudentId == currentUser.Id);
                var Courses = (from i in _context.Assignments
                               join e in studentCourses on i.CourseId equals e.CourseId
                               join s in _context.StudentAssignments on i.Id equals s.AssignmentId //&& e.StudentId equals s.StudentId  
                              into courseTemp
                               from c in courseTemp.DefaultIfEmpty()
                               where i.SubmissionDate <= DateTime.Now
                               select new AssignmentSubmissionViewModel()
                               {
                                   AssignmentId = i.Id,
                                   Assignment = new Assignment()
                                   {
                                       Id = i.Id,
                                       AssignmentFile = i.AssignmentFile,
                                       Name = i.Name,
                                       SubmissionDate = i.SubmissionDate,
                                       TotalScore = i.TotalScore,
                                       CourseId = i.CourseId,
                                       Course = e.Course
                                   },
                                   AssignmentDetails = c.AssignmentDetails,
                                   ObtainScore = c.ObtainScore,
                                   SubmissionDate = c.SubmissionDate,
                                   LastDateToSubmit = i.SubmissionDate,
                               }
                           );
                return View(await Courses.ToListAsync());
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "Teacher"))
            {
                var teacherCourses = _context.Courses.Where(m => m.TeacherId == currentUser.Id);
                var Courses = (from i in _context.Assignments
                               join e in teacherCourses on i.CourseId equals e.Id
                               join s in _context.StudentAssignments on i.Id equals s.AssignmentId //&& e.StudentId equals s.StudentId  
                              into courseTemp
                               from c in courseTemp.DefaultIfEmpty()
                                   //where i.SubmissionDate <= DateTime.Now
                               select new AssignmentSubmissionViewModel()
                               {
                                   AssignmentId = i.Id,
                                   Assignment = new Assignment()
                                   {
                                       Id = i.Id,
                                       AssignmentFile = i.AssignmentFile,
                                       Name = i.Name,
                                       SubmissionDate = i.SubmissionDate,
                                       TotalScore = i.TotalScore,
                                       CourseId = i.CourseId,
                                       Course = e
                                   },
                                   AssignmentDetails = c.AssignmentDetails,
                                   ObtainScore = c.ObtainScore,
                                   SubmissionDate = c.SubmissionDate,
                                   LastDateToSubmit = i.SubmissionDate,
                               }
                           );
                return View(await Courses.ToListAsync());
            }
            return View();

        }

        // GET: Test/StudentAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssignment = await _context.StudentAssignments
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAssignment == null)
            {
                return NotFound();
            }

            return View(studentAssignment);
        }

        // GET: Test/StudentAssignments/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName");
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Name");

            return View();
        }

        // POST: Test/StudentAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,SubmissionDate,AssignmentDetails,ObtainScore,AssignmentId")] StudentAssignment studentAssignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentAssignment.StudentId);
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Name", studentAssignment.AssignmentId);
            return View(studentAssignment);
        }

        // GET: Test/StudentAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssignment = await _context.StudentAssignments.FindAsync(id);
            if (studentAssignment == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentAssignment.StudentId);
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Name", studentAssignment.AssignmentId); ;
            return View(studentAssignment);
        }

        public async Task<IActionResult> SubmitAssignment(int assignmentid, int courseid)
        {
            ////if (assignmentid == null)
            ////{
            ////    return NotFound();
            ////}
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var studentAssignment = await _context.StudentAssignments.FirstOrDefaultAsync(m => m.AssignmentId == assignmentid && m.StudentId == currentUser.Id);
            if (studentAssignment == null)
            {
                studentAssignment = new StudentAssignment()
                {
                    AssignmentId = assignmentid,
                    StudentId = currentUser.Id

                };

            }


            return View(studentAssignment);
        }
        // POST: Test/StudentAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAssignment(int id, StudentAssignment studentAssignment)
        {
            studentAssignment.SubmissionDate = DateTime.Now;

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            studentAssignment.StudentId = currentUser.Id;
            if (studentAssignment.FormFile == null || studentAssignment.FormFile.Length == 0)
            {

            }
            else
            {
                studentAssignment.AssignmentDetails = await UploadFile(studentAssignment);
            }

            //if (id != studentAssignment.a)
            //{
            //    return NotFound();
            //}
            var existingAssignment = await _context.StudentAssignments.AsNoTracking().FirstOrDefaultAsync(m => m.StudentId == studentAssignment.StudentId && m.AssignmentId == studentAssignment.AssignmentId);


            if (existingAssignment == null)
            {
                _context.Add(studentAssignment);
                await _context.SaveChangesAsync();

            }
            else if (studentAssignment.Id == 0)
            {
                studentAssignment.Id = existingAssignment.Id;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentAssignmentExists(studentAssignment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(studentAssignment);
        }

        #region download the file


        private static async Task<string> UploadFile(StudentAssignment assignment)
        {
            //get file name
            var filename = ContentDispositionHeaderValue.Parse(assignment.FormFile.ContentDisposition).FileName.Trim('"');

            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
            try
            {
                //create directory "Uploads" if it doesn't exists
                if (!Directory.Exists(MainPath))
                {
                    Directory.CreateDirectory(MainPath);
                }


                assignment.AssignmentDetails = assignment.AssignmentId + assignment.FormFile.FileName;
                var filePath = Path.Combine(MainPath, assignment.AssignmentDetails);
                using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    await assignment.FormFile.CopyToAsync(stream);
                }
                return assignment.AssignmentDetails;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IActionResult> DownloadAssignment(string id)
        {
            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot\\Uploads", id);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

        #endregion

        // GET: Test/StudentAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentAssignment = await _context.StudentAssignments
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentAssignment == null)
            {
                return NotFound();
            }

            return View(studentAssignment);
        }

        // POST: Test/StudentAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentAssignment = await _context.StudentAssignments.FindAsync(id);

            try
            {
                _context.StudentAssignments.Remove(studentAssignment);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                _notify.Error("You cannot delete it, Please contact IT Support");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StudentAssignmentExists(int id)
        {
            return _context.StudentAssignments.Any(e => e.Id == id);
        }
    }
}
