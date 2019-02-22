using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;
using CourseCleanup.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            return View();
        }

        [HttpPost]
        public ActionResult UnusedCoursesReport(SearchQueueResultViewModel viewModel)
        {
            var courseSearchQueue = courseSearchQueueBll.Get(viewModel.CourseSearchQueueId);
            courseSearchQueue.DeleteAllRequested = true;
            courseSearchQueueBll.Update(courseSearchQueue);

            var unusedCourses = unusedCourseBll.GetAll()
                .Where(x => x.CourseSearchQueueId == viewModel.CourseSearchQueueId).ToList();
            unusedCourseBll.UpdateStatusRange(unusedCourses, CourseStatus.PendingDeletion);
            
            viewModel.UnusedCourses = unusedCourses;

            return View();
        }

        public async Task<JsonResult> GetUnusedCoursesReport(int id, string sortName, string order, int page, int per_page, string filter)
        {
            var unusedCourses = Enumerable.Empty<UnusedCourse>().AsQueryable();

            filter = JsonConvert.DeserializeObject<string>(filter ?? "");

            if (filter != null)
            {
                unusedCourses = unusedCourseBll.GetAll().Where(x => x.CourseSearchQueueId == id && JsonConvert.SerializeObject(x).ToLower().Contains(filter.ToLower()));
            }
            else
            {
                unusedCourses = unusedCourseBll.GetAll().Where(x => x.CourseSearchQueueId == id);
            }

            var totalCourses = (double) unusedCourses.Count();
            var totalPages = Math.Ceiling(totalCourses / per_page);

            if (page > totalPages)
            {
                page = (int) totalPages;
            }

            var fromSearch = ((page - 1) * per_page) + 1;
            var toSearch = Math.Min(totalCourses, fromSearch + per_page - 1);
            var prevPage = page - 1 > 0
                ? $"/Report/GetUnusedCoursesReport?sortName={sortName}&order={order}&page={page - 1}&per_page={per_page}"
                : null;
            var nextPage = page + 1 <= totalPages
                ? $"/Report/GetUnusedCoursesReport?sortName={sortName}&order={order}&page={page + 1}&per_page={per_page}"
                : null;

            var data = unusedCourses
                .OrderBy($"{sortName} {order ?? ""}")
                .ThenBy(x => x.Id)
                .Skip((page - 1) * per_page)
                .Take(per_page)
                .Select(x => new
                {
                    x.Id,
                    x.DateCreated,
                    x.CourseId,
                    x.CourseCanvasId,
                    x.CourseName,
                    x.CourseSISID,
                    x.CourseCode,
                    x.Term,
                    x.Status
                });

            return Json(new
            {
                current_page = page,
                data,
                from = fromSearch,
                last_page = totalPages,
                next_page_url = nextPage,
                per_page,
                prev_page_url = prevPage,
                to = toSearch
            });
        }

        public async Task<JsonResult> GetDeletedCoursesReport(string sortName, string order, int page,
            int per_page, string filter)
        {
            var deletedCourses = unusedCourseBll.GetAll().Where(x => x.Status == CourseStatus.Deleted || x.Status == CourseStatus.PendingReactivation);
            var totalCourses = (double)deletedCourses.Count();
            var totalPages = Math.Ceiling(totalCourses / per_page);

            if (page > totalPages)
            {
                page = (int)totalPages;
            }

            var fromSearch = ((page - 1) * per_page) + 1;
            var toSearch = Math.Min(totalCourses, fromSearch + per_page - 1);
            var prevPage = page - 1 > 0
                ? $"/Report/GetDeletedCoursesReport?sortName={sortName}&order={order}&page={page - 1}&per_page={per_page}"
                : null;
            var nextPage = page + 1 <= totalPages
                ? $"/Report/GetDdeletedCoursesReport?sortName={sortName}&order={order}&page={page + 1}&per_page={per_page}"
                : null;

            var data = deletedCourses
                .OrderBy($"{sortName} {order ?? ""}")
                .ThenBy(x => x.Id)
                .Skip((page - 1) * per_page)
                .Take(per_page)
                .Select(x => new
                {
                    x.Id,
                    x.LastUpdated,
                    x.CourseId,
                    x.CourseCanvasId,
                    x.CourseName,
                    x.CourseSISID,
                    x.CourseCode,
                    x.Term,
                    x.Status
                });

            return Json(new
            {
                current_page = page,
                data,
                from = fromSearch,
                last_page = totalPages,
                next_page_url = nextPage,
                per_page,
                prev_page_url = prevPage,
                to = toSearch
            });
        }

        public async Task<JsonResult> DeleteCourse(int id)
        {
            var unusedCourse = unusedCourseBll.Get(id);

            unusedCourse.Status = CourseStatus.PendingDeletion;
            unusedCourseBll.Update(unusedCourse);

            return Json(unusedCourse);
        }

        public async Task<JsonResult> ReactivateCourse(int id)
        {
            var deletedCourse = unusedCourseBll.Get(id);

            deletedCourse.Status = CourseStatus.PendingReactivation;
            unusedCourseBll.Update(deletedCourse);

            return Json(deletedCourse);
        }
    }
}