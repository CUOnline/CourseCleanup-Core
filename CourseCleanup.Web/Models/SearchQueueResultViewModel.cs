using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;

namespace CourseCleanup.Web.Models
{
    public class SearchQueueResultViewModel
    {
        public int CourseSearchQueueId { get; set; }

        public List<UnusedCourse> UnusedCourses { get; set; }
    }
}
