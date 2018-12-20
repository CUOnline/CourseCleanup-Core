using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;

namespace CourseCleanup.Interface.Repository
{
    public interface IUnusedCourseRepository : IRepository<UnusedCourse>
    {
        void UpdateRange(IEnumerable<UnusedCourse> unusedCourses);
        void UpdateStatusRange(IEnumerable<UnusedCourse> unusedCourses, CourseStatus status);
        void AddRange(IEnumerable<UnusedCourse> unusedCourses);
    }
}
