﻿using ELMCOM.Application.Enums;
using ELMCOM.Infrastructure.DbContexts;
using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Infrastructure.Models;
using ELMCOM.Web.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ELMCOM.Web.Areas.Education.Controllers
{
    [Area("Education")]
    public class StudentCoursesController : BaseController<StudentCoursesController>
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
            IQueryable<StudentCourse> applicationDbContext;
            if (User.IsInRole(Roles.SuperAdmin.ToString()))
            {
                applicationDbContext = _context.StudentCourses.Include(s => s.Course).Include(s => s.Student);
            }
            else
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                applicationDbContext = _context.StudentCourses.Include(s => s.Course).Include(s => s.Student)
                   .Where(m => m.Course.SchoolId == currentUser.SchoolId);
            }

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
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());

            var Courses = (from i in _context.Courses
                           join e in _context.StudentCourses on i.Id equals e.CourseId
                           into courseTemp
                           from c in courseTemp.DefaultIfEmpty()

                           select i
                           );
            if (!User.IsInRole(Roles.SuperAdmin.ToString()))
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                    .ToList();
                Courses = Courses.Where(a => a.SchoolId == currentUser.SchoolId);
            }

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
                if (_context.StudentCourses.FirstOrDefault(m => m.StudentId == studentCourse.StudentId && m.CourseId == studentCourse.CourseId) != null)
                {
                    ModelState.AddModelError(string.Empty, "This student already enrolled with this course");
                    return View(studentCourse);
                }
                _context.Add(studentCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());

            var Courses = (from i in _context.Courses
                           join e in _context.StudentCourses on i.Id equals e.CourseId
                           into courseTemp
                           from c in courseTemp.DefaultIfEmpty()
                           select i);
            if (!User.IsInRole(Roles.SuperAdmin.ToString()))
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                    .ToList();
                Courses = Courses.Where(a => a.SchoolId == currentUser.SchoolId);
            }
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
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());

            var Courses = (from i in _context.Courses
                           join e in _context.StudentCourses on i.Id equals e.CourseId
                           into courseTemp
                           from c in courseTemp.DefaultIfEmpty()
                           select i);
            if (!User.IsInRole(Roles.SuperAdmin.ToString()))
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                    .ToList();
                Courses = Courses.Where(a => a.SchoolId == currentUser.SchoolId);
            }
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
                if (_context.StudentCourses.FirstOrDefault(m => m.StudentId == studentCourse.StudentId 
                && m.CourseId == studentCourse.CourseId
                && m.Id!=studentCourse.Id) != null)
                {
                    ModelState.AddModelError(string.Empty, "This student already enrolled with this course");
                    return View(studentCourse);
                }
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
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());

            var Courses = (from i in _context.Courses
                           join e in _context.StudentCourses on i.Id equals e.CourseId
                           into courseTemp
                           from c in courseTemp.DefaultIfEmpty()
                           select i);
            if (!User.IsInRole(Roles.SuperAdmin.ToString()))
            {
                var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                    .ToList();
                Courses = Courses.Where(a => a.SchoolId == currentUser.SchoolId);
            }
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

            try
            {
                _context.StudentCourses.Remove(studentCourse);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                _notify.Error("You cannot delete it, Please contact IT Support");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StudentCourseExists(int id)
        {
            return _context.StudentCourses.Any(e => e.Id == id);
        }
    }
}
