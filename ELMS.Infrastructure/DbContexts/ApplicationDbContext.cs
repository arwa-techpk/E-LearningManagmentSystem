using AspNetCoreHero.Abstractions.Domain;
//using ELMCOM.Domain.Entities.Catalog;
using AspNetCoreHero.EntityFrameworkCore.AuditTrail;
using ELMCOM.Application.Interfaces.Contexts;
using ELMCOM.Application.Interfaces.Shared;
using ELMCOM.Domain.Entities;
using ELMCOM.Infrastructure.Identity.Models;
using ELMCOM.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ELMCOM.Infrastructure.DbContexts
{
    public class ApplicationDbContext :DbContext // why don't extend identityDbContext ? 
    {
        private readonly IDateTimeService _dateTime;
        private readonly IAuthenticatedUserService _authenticatedUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, IAuthenticatedUserService authenticatedUser) : base(options)
        {
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;
        }

        public DbSet<School> School { get; set; }
        public virtual DbSet<Assignment> Assignments { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Lecture> Lectures { get; set; }
        public virtual DbSet<StudentAssignment> StudentAssignments { get; set; }
        public virtual DbSet<StudentCourse> StudentCourses { get; set; }
        public virtual DbSet<StudentExamAnswer> StudentExamAnswers { get; set; }
        public virtual DbSet<StudentLecture> StudentLectures { get; set; }
       
        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }



            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "Users", schema: "Identity");
            });

            base.OnModelCreating(builder);
        }
    }
}