using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELMS.Infrastructure.DbContexts;
using ELMS.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using ELMS.Infrastructure.Identity.Models;
using ELMS.Application.Enums;
using ELMS.Web.Areas.Test.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.IO;

namespace ELMS.Web.Areas.Test.Controllers
{
    [Area("Test")]
    public class StudentExamAnswersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentExamAnswersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Student")]
        // GET: Test/StudentExamAnswers
        public async Task<IActionResult> Index()
        {
           // var applicationDbContext = _context.StudentExamAnswers.Include(s => s.Exam).Include(s => s.Student);
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);



            var studentCourses = _context.StudentCourses.Include(c => c.Course).Where(m => m.StudentId == currentUser.Id);
            var applicationDbContext = await (from i in _context.Exams
                                 join e in studentCourses on i.CourseId equals e.Id
                                 join s in _context.StudentCourses on e.Id equals s.CourseId

                                 join sa in _context.StudentExamAnswers on i.Id equals sa.ExamId //&& e.StudentId equals s.StudentId  
                                into courseTemp
                                 from c in courseTemp.DefaultIfEmpty()
                                 where e.StudentId == currentUser.Id

                                 select new ExamAnswerSubmissionViewModel()
                                 {
                                     ExamId = i.Id,
                                     StudentExamId = c.Id,

                                     Exam = new Exam()
                                     {
                                         Id = i.Id,
                                         ExamPaper = i.ExamPaper,
                                         Name = i.Name,
                                         ExamDate = i.ExamDate,
                                         TotalScore = i.TotalScore,
                                         CourseId = i.CourseId,
                                         Course = e.Course
                                     },
                                     //  Student=new ApplicationUser() { Id=u.Id,Email=u.Email},
                                     AnswerSheet = c.AnswerSheet,
                                     ObtainedScore = c.ObtainedScore,
                                     SubmissionDate = c.SubmissionDate,
                                     ExamDate = i.ExamDate,
                                     StudentId = c.StudentId
                                 }
                    ).ToListAsync();
            return View(applicationDbContext);

        }

        // GET: Test/StudentExamAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentExamAnswer = await _context.StudentExamAnswers
                .Include(s => s.Exam)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentExamAnswer == null)
            {
                return NotFound();
            }

            return View(studentExamAnswer);
        }

        // GET: Test/StudentExamAnswers/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName");
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name");
            return View();
        }

        // POST: Test/StudentExamAnswers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExamId,StudentId,ObtainedScore,AnswerSheet")] StudentExamAnswer studentExamAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentExamAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentExamAnswer.StudentId);
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name", studentExamAnswer.ExamId);
            return View(studentExamAnswer);
        }

        // GET: Test/StudentExamAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentExamAnswer = await _context.StudentExamAnswers.FindAsync(id);
            if (studentExamAnswer == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentExamAnswer.StudentId);
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name", studentExamAnswer.ExamId);
            return View(studentExamAnswer);
        }

        // POST: Test/StudentExamAnswers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExamId,StudentId,ObtainedScore,AnswerSheet")] StudentExamAnswer studentExamAnswer)
        {
            if (id != studentExamAnswer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentExamAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExamAnswerExists(studentExamAnswer.Id))
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
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentExamAnswer.StudentId);
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Name", studentExamAnswer.ExamId);
            return View(studentExamAnswer);
        }

        // GET: Test/StudentExamAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentExamAnswer = await _context.StudentExamAnswers
                .Include(s => s.Exam)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentExamAnswer == null)
            {
                return NotFound();
            }

            return View(studentExamAnswer);
        }

        // POST: Test/StudentExamAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentExamAnswer = await _context.StudentExamAnswers.FindAsync(id);
            _context.StudentExamAnswers.Remove(studentExamAnswer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExamAnswerExists(int id)
        {
            return _context.StudentExamAnswers.Any(e => e.Id == id);
        }

        #region download the file


        private static async Task<string> UploadFile(StudentExamAnswer assignment)
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


                assignment.AnswerSheet = assignment.ExamId + assignment.FormFile.FileName;
                var filePath = Path.Combine(MainPath, assignment.AnswerSheet);
                using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    await assignment.FormFile.CopyToAsync(stream);
                }
                return assignment.AnswerSheet;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IActionResult> DownloadExam(string id)
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


        public async Task<IActionResult> SubmitExam(int Examid, int courseid)
        {
            //if (Examid == null)
            //{
            //    return NotFound();
            //}
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var studentExam = await _context.StudentExamAnswers.FirstOrDefaultAsync(m => m.ExamId == Examid && m.StudentId == currentUser.Id);
            if (studentExam == null)
            {
                studentExam = new StudentExamAnswer()
                {
                    ExamId = Examid,
                    StudentId = currentUser.Id

                };

            }


            return View(studentExam);
        }
        // POST: Test/StudentExams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitExam(int id, StudentExamAnswer studentExam)
        {
            studentExam.SubmissionDate = DateTime.Now;

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            studentExam.StudentId = currentUser.Id;
            if (studentExam.FormFile == null || studentExam.FormFile.Length == 0)
            {

            }
            else
            {
                studentExam.AnswerSheet = await UploadFile(studentExam);
            }

            //if (id != studentExam.a)
            //{
            //    return NotFound();
            //}
            var existingExam = await _context.StudentExamAnswers.AsNoTracking().FirstOrDefaultAsync(m => m.StudentId == studentExam.StudentId && m.ExamId == studentExam.ExamId);


            if (existingExam == null)
            {
                _context.Add(studentExam);
                await _context.SaveChangesAsync();

            }
            else if (studentExam.Id == 0)
            {
                studentExam.Id = existingExam.Id;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentExam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExamAnswerExists(studentExam.Id))
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

            return View(studentExam);
        }

    }
}
