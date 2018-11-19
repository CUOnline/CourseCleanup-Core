using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;

namespace CourseCleanup.Interface.BLL
{
    public interface IUnusedCourseBLL : IBLL<UnusedCourse>
    {
        void UpdateRange(IEnumerable<UnusedCourse> unusedCourses);
        void AddRange(IEnumerable<UnusedCourse> unusedCourses);
    }
}
