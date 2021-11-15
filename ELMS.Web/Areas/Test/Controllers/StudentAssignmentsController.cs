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

namespace ELMS.Web.Areas.Test.Controllers
{
    [Area("Test")]
    public class StudentAssignmentsController : Controller
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
            var applicationDbContext = _context.StudentAssignments.Include(s => s.Assignment).Include(s => s.Student);
            return View(await applicationDbContext.ToListAsync());
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

        // POST: Test/StudentAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,SubmissionDate,AssignmentDetails,ObtainScore,AssignmentId")] StudentAssignment studentAssignment)
        {
            if (id != studentAssignment.Id)
            {
                return NotFound();
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
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = await _userManager.GetUsersInRoleAsync(Roles.Student.ToString());
            allUsersExceptCurrentUser = allUsersExceptCurrentUser.Where(a => a.SchoolId == currentUser.SchoolId)
                .ToList();
            ViewData["StudentId"] = new SelectList(allUsersExceptCurrentUser, "Id", "UserName", studentAssignment.StudentId);
            ViewData["AssignmentId"] = new SelectList(_context.Assignments, "Id", "Name", studentAssignment.AssignmentId);
            return View(studentAssignment);
        }

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
            _context.StudentAssignments.Remove(studentAssignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentAssignmentExists(int id)
        {
            return _context.StudentAssignments.Any(e => e.Id == id);
        }
    }
}
