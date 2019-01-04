using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;
using CourseCleanup.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rss.Providers.Canvas.Helpers;
using RSS.Services.CanvasRedshift.Models;

namespace CourseCleanup.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseSearchQueueBLL courseSearchQueueBll;
        private readonly HttpClient canvasRedshiftClient;
        private readonly CanvasApiAuth canvasApiAuth;

        public HomeController(ICourseSearchQueueBLL courseSearchQueueBll, IHttpClientFactory httpClientFactory, IOptions<CanvasApiAuth> authOptions)
        {
            this.courseSearchQueueBll = courseSearchQueueBll;
            this.canvasRedshiftClient = httpClientFactory.CreateClient(HttpClientNames.CanvasRedshiftClient);
            this.canvasApiAuth = authOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();

            if (!authenticateResult.Succeeded)
            {
                return RedirectToAction("ExternalLogin");
            }

            var model = new HomeViewModel();
            if (HttpContext.User.IsInRole(RoleNames.AccountAdmin) || HttpContext.User.IsInRole(RoleNames.HelpDesk))
            {
                model.Authorized = true;
                model.BaseCanvasUrl = canvasApiAuth.BaseUrl;
            }
            else
            {
                // return unauthorized view
                model.Authorized = false;
            }
            return View();
        }

        #region LoginHelper
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult("Canvas", Url.Action("ExternalLoginCallback", "Home"));
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLogout(string provider)
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("LoggedOut");
        }

        public ActionResult LoggedOut()
        {
            return View();
        }

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        internal class ChallengeResult : UnauthorizedResult
        {
            private readonly string LoginProvider;
            private readonly string RedirectUri;
            private readonly string UserId;

            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                this.LoginProvider = provider;
                this.RedirectUri = redirectUri;
                this.UserId = userId;
            }

            public override void ExecuteResult(ActionContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Parameters.Add(XsrfKey, UserId);
                }
                context.HttpContext.ChallengeAsync(LoginProvider, properties);
            }
        }

        private async Task<string> GetCurrentUserEmail()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();
            if (authenticateResult != null)
            {
                var emailClaim = authenticateResult.Principal.Claims.Where(cl => cl.Type == ClaimTypes.Email).FirstOrDefault();

                return emailClaim?.Value;
            }

            return string.Empty;
        }
        #endregion

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
            var userClaims = User.Claims.AsQueryable();
            var userEmail = userClaims.First(x => x.Type.Contains("emailaddress")).Value;

            newSearchQueue.Status = SearchStatus.New;
            newSearchQueue.SubmittedByEmail = userEmail ?? "";
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
                    TermList = x.TermList.Length > 0 ? GetTermList(x.TermList, enrollmentTerms) : "",
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
            var termArray = termList.Split(",");
            var termStrings = new List<string>();

            if (termArray.Any())
            {
                foreach (var term in termArray)
                {
                    termStrings.Add(enrollments.First(x => x.Id.ToString() == term.Trim()).Name.Replace(" ", "&nbsp;"));
                }

                return String.Join("<br />", termStrings);
                //return string.Join(", ", termList.Split(", ").Select(x => enrollments.First(y => y.Id.ToString() == x).Name));
            }

            return "";
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
    }
}
