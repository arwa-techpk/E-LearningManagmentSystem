using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELMS.Infrastructure.DbContexts;
using ELMS.Infrastructure.Models;
using ELMS.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using ELMS.Application.Enums;

namespace ELMS.Web.Areas.Education.Controllers
{
    [Area("Education")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public CoursesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Education/Courses
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Courses.Include(c => c.School).Include(c => c.Teacher);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Education/Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.School)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Education/Courses/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Teacher.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["TeacherId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName");
            ViewData["SchoolId"] = new SelectList(_context.School, "Id", "Name");
            return View();
        }

        // POST: Education/Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,TeacherId,SchoolId,Credit,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Teacher.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["TeacherId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", course.TeacherId);
            ViewData["SchoolId"] = new SelectList(_context.School, "Id", "Name", course.SchoolId);
            return View(course);
        }

        // GET: Education/Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Teacher.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["TeacherId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", course.TeacherId);
            ViewData["SchoolId"] = new SelectList(_context.School, "Id", "Name", course.SchoolId);
            return View(course);
        }

        // POST: Education/Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,TeacherId,SchoolId,Credit,Description")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
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
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Teacher.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["TeacherId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", course.TeacherId);
            ViewData["SchoolId"] = new SelectList(_context.School, "Id", "Name", course.SchoolId);
            return View(course);
        }

        // GET: Education/Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.School)
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Education/Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
