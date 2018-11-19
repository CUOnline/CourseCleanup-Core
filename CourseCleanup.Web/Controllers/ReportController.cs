using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Models.Enums;
using CourseCleanup.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CourseCleanup.Web.Controllers
{
    public class ReportController : Controller
    {
        private readonly ICourseSearchQueueBLL courseSearchQueueBll;
        private readonly IUnusedCourseBLL unusedCourseBll;

        public ReportController(ICourseSearchQueueBLL courseSearchQueueBll, IUnusedCourseBLL unusedCourseBll)
        {
            this.courseSearchQueueBll = courseSearchQueueBll;
            this.unusedCourseBll = unusedCourseBll;
        }

        public ActionResult DeletedCoursesReport()
        {
            var deletedCourses = unusedCourseBll.GetAll().Where(x => x.Status == CourseStatus.Deleted);

            var model = new SearchQueueResultViewModel()
            {
                UnusedCourses = deletedCourses.ToList()
            };

            return View(model);
        }

        public ActionResult UnusedCoursesReport(int courseSearchQueueId)
        {
            var unusedCourses = unusedCourseBll.GetAll().Where(x => x.CourseSearchQueueId == courseSearchQueueId);

            var model = new SearchQueueResultViewModel()
            {
                CourseSearchQueueId = courseSearchQueueId,
                UnusedCourses = unusedCourses.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult UnusedCoursesReport(SearchQueueResultViewModel viewModel)
        {
            var unusedCourses = unusedCourseBll.GetAll().Where(x => x.CourseSearchQueueId == viewModel.CourseSearchQueueId).ToList();

            foreach (var course in unusedCourses)
            {
                course.Status = CourseStatus.PendingDeletion;
                unusedCourseBll.Update(course);
            }

            viewModel.UnusedCourses = unusedCourses;

            return View(viewModel);
        }
    }
}
