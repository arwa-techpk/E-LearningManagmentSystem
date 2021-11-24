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
using ELMS.Infrastructure.Zoom;
using Microsoft.Extensions.Configuration;

namespace ELMS.Web.Areas.Education.Controllers
{
    [Area("Education")]
    public class LecturesController : Controller
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
            var applicationDbContext = _context.Lectures.Include(l => l.Course)
                .Where(m=>m.Course.TeacherId== currentUser.Id);
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
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m=>m.TeacherId==currentUser.Id), "Id", "Title");
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
                    ZoomNet zoomNet = new ZoomNet();
                    ZoomMeetingResponse zoomMeetingResponse = zoomNet.CreateZoomMeeting(new ZoomMeetingRequest()
                    {
                        Topic = lecture.Title,
                        Duration = lecture.Duration,
                        StartDateTime = lecture.LectureDate,
                        Type = "2",
                        schedule_for = currentUser.Email,
                        TimeZone = _config.GetValue<string>(
                    "ZoomCredentials:API:TimeZone"),
                        APIKey = _config.GetValue<string>(
                    "ZoomCredentials:API:Key"),
                        ApiSecret = _config.GetValue<string>(
                    "ZoomCredentials:API:Secret"),
                        UserId = _config.GetValue<string>(
                    "ZoomCredentials:API:UserId"),
                    });
                    lecture.ZoomMeetingHostURL = zoomMeetingResponse.start_url;
                    lecture.ZoomMeetingJoinURL = zoomMeetingResponse.join_url;

                    _context.Add(lecture);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
                    return View(lecture);
                }
                
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
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
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
            return View(lecture);
        }

        // POST: Education/Lectures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,CourseId,LectureDate,Duration")] Lecture lecture)
        {
            if (id != lecture.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(m => m.TeacherId == currentUser.Id), "Id", "Title", lecture.CourseId);
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
            _context.Lectures.Remove(lecture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LectureExists(int id)
        {
            return _context.Lectures.Any(e => e.Id == id);
        }
    }
}
