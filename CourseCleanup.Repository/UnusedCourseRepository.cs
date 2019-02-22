using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CourseCleanup.Repository
{
    public class UnusedCourseRepository : RepositoryBase, IUnusedCourseRepository
    {
        public UnusedCourseRepository(CourseCleanupContext context) : base(context) { }

        public UnusedCourse Add(UnusedCourse model)
        {
            Context.UnusedCourse.Add(model);
            Context.SaveChanges();
            return model;
        }

        public void AddRange(IEnumerable<UnusedCourse> unusedCourses)
        {
            Context.UnusedCourse.AddRange(unusedCourses);
            Context.SaveChanges();
        }

        public void UpdateRange(IEnumerable<UnusedCourse> unusedCourses)
        {
            Context.UnusedCourse.UpdateRange(unusedCourses);
            Context.SaveChanges();
        }

        public void UpdateStatusRange(IEnumerable<UnusedCourse> unusedCourses, CourseStatus status)
        {
            Context.UnusedCourse.UpdateRange(unusedCourses);
            Context.SaveChanges();
        }


        public UnusedCourse Get(int modelId)
        {
            return Context.UnusedCourse.Find(modelId);
        }

        public IQueryable<UnusedCourse> GetAll()
        {
            return Context.UnusedCourse;
        }
        
        public UnusedCourse Update(UnusedCourse model)
        {
            Context.UnusedCourse.Update(model);
            Context.SaveChanges();
            return model;
        }
    }
}
