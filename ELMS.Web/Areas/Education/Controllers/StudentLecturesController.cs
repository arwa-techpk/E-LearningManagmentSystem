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
using ELMS.Web.Abstractions;

namespace ELMS.Web.Areas.Education.Controllers
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
            var applicationDbContext = _context.StudentLectures.Include(s => s.Lecture)
                .Include(s => s.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Education/StudentLectures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentLecture = await _context.StudentLectures
                .Include(s => s.Lecture)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentLecture == null)
            {
                return NotFound();
            }

            return View(studentLecture);
        }

        // GET: Education/StudentLectures/Create
        public async Task<IActionResult> Create()
        {

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            var Lectures = _context.Lectures.Include(m=>m.Course)
               .Select(x => new
               {
                   Id = x.Id,
                   Name = x.Course.Title + " > " + x.Title
               });

            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName");
            ViewData["LectureId"] = new SelectList(Lectures, "Id", "Name");
            return View();
        }

        // POST: Education/StudentLectures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,LectureId")] StudentLecture studentLecture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentLecture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentLecture.StudentId);
            ViewData["LectureId"] = new SelectList(_context.Lectures, "Id", "Title", studentLecture.LectureId);

            return View(studentLecture);
        }

        // GET: Education/StudentLectures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentLecture = await _context.StudentLectures.FindAsync(id);
            if (studentLecture == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentLecture.StudentId);
            ViewData["LectureId"] = new SelectList(_context.Lectures, "Id", "Title", studentLecture.LectureId);

            return View(studentLecture);
        }

        // POST: Education/StudentLectures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,LectureId")] StudentLecture studentLecture)
        {
            if (id != studentLecture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentLecture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentLectureExists(studentLecture.Id))
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
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentLecture.StudentId);
            ViewData["LectureId"] = new SelectList(_context.Lectures, "Id", "Title", studentLecture.LectureId);

            return View(studentLecture);
        }

        // GET: Education/StudentLectures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentLecture = await _context.StudentLectures
                .Include(s => s.Lecture)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentLecture == null)
            {
                return NotFound();
            }

            return View(studentLecture);
        }

        // POST: Education/StudentLectures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentLecture = await _context.StudentLectures.FindAsync(id);

            try
            {
                _context.StudentLectures.Remove(studentLecture);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                _notify.Error("You cannot delete it, Please contact IT Support");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StudentLectureExists(int id)
        {
            return _context.StudentLectures.Any(e => e.Id == id);
        }
    }
}
