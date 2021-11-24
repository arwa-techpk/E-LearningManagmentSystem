using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELMS.Infrastructure.DbContexts;
using ELMS.Infrastructure.Models;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using ELMS.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using ELMS.Web.Areas.Test.Models;
using ELMS.Web.Abstractions;

namespace ELMS.Web.Areas.Test.Controllers
{
    [Area("Test")]
    public class ExamsController : BaseController<AssignmentsController>
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public ExamsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Test/Exams
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Exams.Include(e => e.Course);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Test/Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // GET: Test/Exams/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title");
            return View();
        }

        // POST: Test/Exams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (ModelState.IsValid)
            {
                if (exam.FormFile == null || exam.FormFile.Length == 0)
                {

                }
                else
                {
                    exam.ExamPaper = await UploadFile(exam);
                }

                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", exam.CourseId);
            return View(exam);
        }

        // GET: Test/Exams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams.FindAsync(id);

            if (exam == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", exam.CourseId);
            return View(exam);
        }

        // POST: Test/Exams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam exam)
        {
            if (id != exam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (exam.FormFile == null || exam.FormFile.Length == 0)
                    {

                    }
                    else
                    {
                        exam.ExamPaper = await UploadFile(exam);
                    }

                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", exam.CourseId);
            return View(exam);
        }


        #region download the file
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
        private static async Task<string> UploadFile(Exam exam)
        {
            //get file name
            var filename = ContentDispositionHeaderValue.Parse(exam.FormFile.ContentDisposition).FileName.Trim('"');

            //get path
            var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
            try
            {
                //create directory "Uploads" if it doesn't exists
                if (!Directory.Exists(MainPath))
                {
                    Directory.CreateDirectory(MainPath);
                }


                exam.ExamPaper = exam.Name + exam.FormFile.FileName;
                var filePath = Path.Combine(MainPath, exam.ExamPaper);
                using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    await exam.FormFile.CopyToAsync(stream);
                }
                return exam.ExamPaper;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        // GET: Test/Exams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // POST: Test/Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }


        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> SubmittedExamAnswers()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var students = await _userManager.GetUsersInRoleAsync("Student");


            // var teacherCourses = _context.Courses.Where(m => m.TeacherId == currentUser.Id);

            var Courses = await (from i in _context.Exams
                                 join e in _context.Courses on i.CourseId equals e.Id
                                 join s in _context.StudentCourses on e.Id equals s.CourseId

                                 join sa in _context.StudentExamAnswers on i.Id equals sa.ExamId //&& e.StudentId equals s.StudentId  
                                into courseTemp
                                 from c in courseTemp.DefaultIfEmpty()
                                 where e.TeacherId == currentUser.Id

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
                                         Course = e
                                     },
                                     //  Student=new ApplicationUser() { Id=u.Id,Email=u.Email},
                                     AnswerSheet = c.AnswerSheet,
                                     ObtainedScore = c.ObtainedScore,
                                     SubmissionDate = c.SubmissionDate,
                                     ExamDate = i.ExamDate,
                                     StudentId = c.StudentId
                                 }
                           ).ToListAsync();
            foreach (var item in Courses)
            {
                if (item.StudentId != null)
                {
                    item.Student = students.First(m => m.Id == item.StudentId);
                }
            }

            return View(Courses);



        }


        [Authorize(Roles = "Teacher")]
        // GET: Test/Exams/Edit/5
        public async Task<IActionResult> ExamFeedback(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var Exam = await _context.StudentExamAnswers.FindAsync(id);
            if (Exam == null)
            {
                return NotFound();
            }

            return View(Exam);
        }

        // POST: Test/Exams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ExamFeedback(int id, [Bind("ObtainedScore,Id")] StudentExamAnswer Exam)
        {
            if (id != Exam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var ExamToUpdate = await _context.StudentExamAnswers.FindAsync(id);
                ExamToUpdate.ObtainedScore = Exam.ObtainedScore;
                try
                {
                    _context.Update(ExamToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(Exam.Id))
                    {
                        _notify.Warning($"Exam with ID {Exam.Id} not Found.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("SubmittedExamAnswers");
            }



            return View(Exam);
        }
    }
}
