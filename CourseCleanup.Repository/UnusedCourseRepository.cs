using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Models;

namespace CourseCleanup.Repository
{
    public class UnusedCourseRepository : RepositoryBase, IUnusedCourseRepository
    {
        public UnusedCourseRepository(CourseCleanupContext context) : base(context) { }

        public UnusedCourse Add(UnusedCourse model)
        {
            Context.UnusedCourses.Add(model);
            Context.SaveChanges();
            return model;
        }

        public void AddRange(IEnumerable<UnusedCourse> unusedCourses)
        {
            Context.UnusedCourses.AddRange(unusedCourses);
            Context.SaveChanges();
        }

        public void UpdateRange(IEnumerable<UnusedCourse> unusedCourses)
        {
            Context.UnusedCourses.UpdateRange(unusedCourses);
            Context.SaveChanges();
        }

        public UnusedCourse Get(int modelId)
        {
            return Context.UnusedCourses.Find(modelId);
        }

        public IQueryable<UnusedCourse> GetAll()
        {
            return Context.UnusedCourses;
        }
        
        public UnusedCourse Update(UnusedCourse model)
        {
            Context.UnusedCourses.Update(model);
            Context.SaveChanges();
            return model;
        }
        
    }
}
