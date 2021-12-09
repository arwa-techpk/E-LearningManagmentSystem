using ELMCOM.Infrastructure.DbContexts;
using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Infrastructure.Models;
using ELMCOM.Web.Abstractions;
using ELMCOM.Web.Areas.Test.Models;
using Microsoft.AspNetCore.Authorization;
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

namespace ELMCOM.Web.Areas.Test.Controllers
{
    [Area("Test")]
    public class AssignmentsController : BaseController<AssignmentsController>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AssignmentsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // GET: Test/Assignments
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var applicationDbContext = _context.Assignments.Include(a => a.Course).Where(m => m.Course.TeacherId == currentUser.Id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Test/Assignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // GET: Test/Assignments/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title");

            return View();
        }

        #region download the file
        public async Task<IActionResult> DownloadAssignment(string id) // catch download file error 
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
        // POST: Test/Assignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                if (!(assignment.FormFile == null || assignment.FormFile.Length == 0))
                {

                    try
                    {
                        assignment.AssignmentFile = await UploadFile(assignment);
                    }
                    catch (Exception ex)
                    {

                        _notify.Error(ex.Message);
                        return null;
                    }

                }

                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", assignment.CourseId);
            return View(assignment);
        }

        private static async Task<string> UploadFile(Assignment assignment)
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


                assignment.AssignmentFile = assignment.Name + assignment.FormFile.FileName;
                var filePath = Path.Combine(MainPath, assignment.AssignmentFile);
                using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    await assignment.FormFile.CopyToAsync(stream);
                }
                return assignment.AssignmentFile;
            }
            catch (Exception)
            {
                throw;

            }
        }

        // GET: Test/Assignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", assignment.CourseId);
            return View(assignment);
        }

        // POST: Test/Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Assignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!(assignment.FormFile == null || assignment.FormFile.Length == 0))
                {

                    try
                    {
                        assignment.AssignmentFile = await UploadFile(assignment);
                    }
                    catch (Exception ex)
                    {

                        _notify.Error(ex.Message);
                        return null;
                    }
                }

                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id))
                    {
                        _notify.Warning($"Assignment with ID {assignment.Id} not Found.");
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
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", assignment.CourseId);
            return View(assignment);
        }

        // GET: Test/Assignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Test/Assignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);

            try
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                _notify.Error("You cannot delete it, Please contact IT Support");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> SubmittedAssignments()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var students = await _userManager.GetUsersInRoleAsync("Student");


            // var teacherCourses = _context.Courses.Where(m => m.TeacherId == currentUser.Id);

            var Courses = await (from i in _context.Assignments
                                 join e in _context.Courses on i.CourseId equals e.Id
                                 join s in _context.StudentCourses on e.Id equals s.CourseId

                                 join sa in _context.StudentAssignments on i.Id equals sa.AssignmentId //&& e.StudentId equals s.StudentId  
                                into courseTemp
                                 from c in courseTemp.DefaultIfEmpty()
                                 where e.TeacherId == currentUser.Id

                                 select new AssignmentSubmissionViewModel()
                                 {
                                     AssignmentId = i.Id,
                                     StudentAssignmentId = c.Id,

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
                                     //  Student=new ApplicationUser() { Id=u.Id,Email=u.Email},
                                     AssignmentDetails = c.AssignmentDetails,
                                     ObtainScore = c.ObtainScore,
                                     SubmissionDate = c.SubmissionDate,
                                     LastDateToSubmit = i.SubmissionDate,
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
        // GET: Test/Assignments/Edit/5
        public async Task<IActionResult> AssignmentFeedback(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var assignment = await _context.StudentAssignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            return View(assignment);
        }

        // POST: Test/Assignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AssignmentFeedback(int id, [Bind("ObtainScore,Id")] StudentAssignment assignment)
        {
            if (id != assignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var assignmentToUpdate = await _context.StudentAssignments.FindAsync(id);
                assignmentToUpdate.ObtainScore = assignment.ObtainScore;
                try
                {
                    _context.Update(assignmentToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id))
                    {
                        _notify.Warning($"Assignment with ID {assignment.Id} not Found.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("SubmittedAssignments");
            }



            return View(assignment);
        }

    }
}
