using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;
using CourseCleanup.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RSS.Services.CanvasRedshift.Models;

namespace CourseCleanup.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseSearchQueueBLL courseSearchQueueBll;
        private readonly HttpClient canvasRedshiftClient;
        public HomeController(ICourseSearchQueueBLL courseSearchQueueBll, IHttpClientFactory httpClientFactory)
        {
            this.courseSearchQueueBll = courseSearchQueueBll;
            this.canvasRedshiftClient = httpClientFactory.CreateClient(HttpClientNames.CanvasRedshiftClient);
        }

        public IActionResult Index()
        {
            return View();
        }
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<JsonResult> GetEnrollmentTerms()
        {
            var enrollmentTerms = JsonConvert.DeserializeObject<List<EnrollmentTermDTO>>(await canvasRedshiftClient.GetStringAsync("EnrollmentTerm"));
            return new JsonResult(enrollmentTerms.Where(x => x.EndDate != null).OrderBy(x => x.EndDate));
        }

        [HttpPost]
        public IActionResult AddNewSearch([FromBody]CourseSearchQueue newSearchQueue)
        {
            newSearchQueue.Status = SearchStatus.New;
            newSearchQueue.SubmittedByEmail = "current user";
            courseSearchQueueBll.Add(newSearchQueue);

            return Ok();
        }

        public async Task<JsonResult> GetCourseSearchStatuses(string sortName, string order, int page, int per_page, string filter)
        {
            var enrollmentTerms = JsonConvert.DeserializeObject<List<EnrollmentTermDTO>>(await canvasRedshiftClient.GetStringAsync("EnrollmentTerm"));
            var searchQueues = courseSearchQueueBll.GetAll().Where(x => x.DateCreated > DateTime.Now.AddYears(-1));

            var totalSearches = (double)searchQueues.Count();
            var totalPages = Math.Ceiling(totalSearches / per_page);

            if (page > totalPages)
            {
                page = (int)totalPages;
            }

            var fromSearch = ((page - 1) * per_page) + 1;
            var toSearch = Math.Min(totalSearches, fromSearch + per_page - 1);
            var prevPage = page - 1 > 0 ? $"/Home/GetCourseSearchStatuses?sortName={sortName}&order={order}&page={page - 1}&per_page={per_page}" : null;
            var nextPage = page + 1 <= totalPages ? $"/Home/GetCourseSearchStatuses?sortName={sortName}&order={order}&page={page + 1}&per_page={per_page}" : null;
            
            var data = searchQueues
                .OrderBy($"{sortName} {order ?? ""}")
                .ThenBy(x => x.Id)
                .Skip((page - 1) * per_page)
                .Take(per_page)
                .Select(x => new
                {
                    x.Id,
                    x.DateCreated,
                    x.LastUpdated,
                    TermList = GetTermList(x.TermList, enrollmentTerms),
                    x.SubmittedByEmail,
                    Status = GetCourseSearchStatuses(x.Status),
                    x.StatusMessage
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
                to = toSearch,
                total = totalSearches
            });
        }

        private string GetTermList(string termList, List<EnrollmentTermDTO> enrollments)
        {
            return string.Join(", ", termList.Split(",").Select(x => enrollments.First(y => y.Id.ToString() == x).Name));
        }

        private string GetCourseSearchStatuses(SearchStatus status)
        {
            switch (status)
            {
                case SearchStatus.Completed:
                    return "Completed";
                case SearchStatus.Failed:
                    return "Failed";
                case SearchStatus.New:
                    return "New";
                case SearchStatus.Pending:
                    return "Pending";
            }

            return "Unknown";
        }

        [HttpPost]
        public IActionResult ExternalLogOut()
        {
            //ToDo: Add logout functionality
            return View();
        }
    }
}
