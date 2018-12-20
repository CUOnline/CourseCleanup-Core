using CourseCleanup.Interface.BLL;
using CourseCleanup.Interface.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;

namespace CourseCleanup.BLL
{
    public class UnusedCourseBLL : IUnusedCourseBLL
    {
        private readonly IUnusedCourseRepository unusedCourseRepository;

        public UnusedCourseBLL(IUnusedCourseRepository unusedCourseRepository)
        {
            this.unusedCourseRepository = unusedCourseRepository;
        }

        public UnusedCourse Add(UnusedCourse model)
        {
            model.DateCreated = DateTime.Now;
            return unusedCourseRepository.Add(model);
        }

        public void AddRange(IEnumerable<UnusedCourse> unusedCourses)
        {
            foreach (var course in unusedCourses)
            {
                course.DateCreated = DateTime.Now;
            }

            unusedCourseRepository.AddRange(unusedCourses);
        }

        public UnusedCourse Get(int modelId)
        {
            return unusedCourseRepository.Get(modelId);
        }

        public IQueryable<UnusedCourse> GetAll()
        {
            return unusedCourseRepository.GetAll();
        }

        public UnusedCourse Remove(UnusedCourse model)
        {
            throw new NotImplementedException();
        }

        public UnusedCourse Update(UnusedCourse model)
        {
            model.LastUpdated = DateTime.Now;
            return unusedCourseRepository.Update(model);
        }

        public void UpdateRange(IEnumerable<UnusedCourse> unusedCourses)
        {
            foreach (var course in unusedCourses)
            {
                course.LastUpdated = DateTime.Now;
            }

            unusedCourseRepository.UpdateRange(unusedCourses);
        }

        public void UpdateStatusRange(IEnumerable<UnusedCourse> unusedCourses, CourseStatus status)
        {
            foreach (var course in unusedCourses)
            {
                course.Status = status;
            }

            unusedCourseRepository.UpdateRange(unusedCourses);
        }
    }
}
