using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELMS.Infrastructure.DbContexts;
using ELMS.Infrastructure.Models;
using ELMS.Application.Enums;
using ELMS.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace ELMS.Web.Areas.Education.Controllers
{
    [Area("Education")]
    public class StudentCoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

       
        public StudentCoursesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Education/StudentCourses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentCourses.Include(s => s.Course).Include(s => s.Student);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Education/StudentCourses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentCourse = await _context.StudentCourses
                .Include(s => s.Course)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentCourse == null)
            {
                return NotFound();
            }

            return View(studentCourse);
        }

        // GET: Education/StudentCourses/Create
        public async Task<IActionResult> Create()
        {
           
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();

            var Courses = (from i in _context.Courses
                        join e in _context.StudentCourses on i.Id equals e.CourseId
                        into courseTemp 
                        from c in courseTemp.DefaultIfEmpty()
                        select  i);

            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName");
            ViewData["CourseId"] = new SelectList(Courses, "Id", "Title");
            return View();
        }

        // POST: Education/StudentCourses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,CourseId")] StudentCourse studentCourse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(studentCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentCourse.StudentId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", studentCourse.CourseId);

            return View(studentCourse);
        }

        // GET: Education/StudentCourses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentCourse = await _context.StudentCourses.FindAsync(id);
            if (studentCourse == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentCourse.StudentId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", studentCourse.CourseId);

            return View(studentCourse);
        }

        // POST: Education/StudentCourses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,CourseId")] StudentCourse studentCourse)
        {
            if (id != studentCourse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(studentCourse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentCourseExists(studentCourse.Id))
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
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentCourse.StudentId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", studentCourse.CourseId);

            return View(studentCourse);
        }

        // GET: Education/StudentCourses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentCourse = await _context.StudentCourses
                .Include(s => s.Course)
                .Include(s => s.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentCourse == null)
            {
                return NotFound();
            }

            return View(studentCourse);
        }

        // POST: Education/StudentCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentCourse = await _context.StudentCourses.FindAsync(id);
            _context.StudentCourses.Remove(studentCourse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentCourseExists(int id)
        {
            return _context.StudentCourses.Any(e => e.Id == id);
        }
    }
}
