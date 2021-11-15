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
    public class StudentExamAnswersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public StudentExamAnswersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        // GET: Test/StudentExamAnswers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StudentExamAnswers.Include(s => s.Exam).Include(s => s.Student);
            return View(await applicationDbContext.ToListAsync());
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
    }
}
