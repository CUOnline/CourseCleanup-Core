using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseCleanup.Repository
{
    public class CourseCleanupContext : DbContext
    {

        public CourseCleanupContext(DbContextOptions<CourseCleanupContext> options) : base(options) { }

        public DbSet<UnusedCourse> UnusedCourses { get; set; }
        public DbSet<CourseSearchQueue> CourseSearchQueues { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
