using ELMCOM.Infrastructure.DbContexts;
using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Infrastructure.Models;
using ELMCOM.Web.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ELMCOM.Web.Areas.Education.Controllers
{
    [Area("Education")]
    public class LecturesController : BaseController<LecturesController>
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public LecturesController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IConfiguration config)
        {
            _userManager = userManager;
            _context = context;
            _config = config;
        }

        // GET: Education/Lectures
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            //linq method syntax
            var applicationDbContext = _context.Lectures.Include(l => l.Course)
                .Where(m => m.Course.TeacherId == currentUser.Id);

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Education/Lectures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }

            return View(lecture);
        }

        // GET: Education/Lectures/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.CourseId = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title");
            return View();
        }

        // POST: Education/Lectures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,CourseId,LectureDate,Duration")] Lecture lecture)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            if (ModelState.IsValid)
            {
                try
                {
                    //Removing the white space in url and making unique link
                    lecture.LectureJoinURL = String.Concat(("https://meet.jit.si/elmcom/" 
                        + lecture.CourseId + lecture.Title).Where(c => !Char.IsWhiteSpace(c)));

                    _context.Add(lecture);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    ViewBag.CourseId = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
                    return View(lecture);
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.CourseId = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
            return View(lecture);
        }

        // GET: Education/Lectures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lectures.FindAsync(id);
            if (lecture == null)
            {
                return NotFound();
            }
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.CourseId = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
            return View(lecture);
        }

        // POST: Education/Lectures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ZoomMeetingJoinURL,CourseId,LectureDate,Duration")] Lecture lecture)
        {
            if (id != lecture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(lecture.LectureJoinURL))
                    {

                        lecture.LectureJoinURL = String.Concat(("https://meet.jit.si/elmcom/" + lecture.CourseId + lecture.Title).Where(c => !Char.IsWhiteSpace(c)));

                    }
                    _context.Update(lecture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LectureExists(lecture.Id))
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
            ViewBag.CourseId = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
            return View(lecture);
        }

        // GET: Education/Lectures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecture = await _context.Lectures
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }

            return View(lecture);
        }

        // POST: Education/Lectures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lecture = await _context.Lectures.FindAsync(id);

            try
            {
                _context.Lectures.Remove(lecture);
                await _context.SaveChangesAsync();
            }
            catch (System.Exception)
            {

                _notify.Error("You cannot delete this Lecture, Please contact IT Support");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool LectureExists(int id)
        {
            return _context.Lectures.Any(e => e.Id == id);
        }
    }
}
