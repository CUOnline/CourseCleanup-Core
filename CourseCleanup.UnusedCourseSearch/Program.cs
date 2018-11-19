using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CourseCleanup.BLL;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Models;
using CourseCleanup.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RSS.Services.CanvasRedshift.Models;

namespace CourseCleanup.UnusedCourseSearch
{
    class Program
    {
        private static ServiceProvider provider;
        private static HttpClient client = new HttpClient();
        private static CanvasRedshiftSettings canvasRedshiftSettings;

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            //Setup the database options
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var canvasReshiftSettingsSection = configuration.GetSection("CanvasRedshiftSettings");
            services.Configure<CanvasRedshiftSettings>(canvasReshiftSettingsSection);
            
            //DB Context
            services.AddDbContext<CourseCleanupContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // BLLs
            services.AddScoped<ICourseSearchQueueBLL, CourseSearchQueueBLL>();
            services.AddScoped<IUnusedCourseBLL, UnusedCourseBLL>();
            services.AddScoped<ISendEmailBLL, SendEmailBLL>();
            services.AddScoped<IEmailQueueBLL, EmailQueueBLL>();

            // Repositories
            services.AddScoped<ICourseSearchQueueRepository, CourseSearchQueueRepository>();
            services.AddScoped<IUnusedCourseRepository, UnusedCourseRepository>();

            //Setup the provider for resolving dependencies
            provider = services.BuildServiceProvider();

            canvasRedshiftSettings = provider.GetService<IOptions<CanvasRedshiftSettings>>().Value;
            
            var courseSearchQueueBll = provider.GetService<ICourseSearchQueueBLL>();
            var nextSearch = courseSearchQueueBll.GetNextSearchToProcess();
            if (nextSearch != null)
            {
                SearchForUnusedCourses(nextSearch).GetAwaiter().GetResult();
            }
        }

        private static async Task SearchForUnusedCourses(CourseSearchQueue nextSearch)
        {
            var termIds = nextSearch.TermList.Split(",").Select(x => long.Parse(x));
            var sendEmailBll = provider.GetService<ISendEmailBLL>();
            var unusedCourseBll = provider.GetService<IUnusedCourseBLL>();

            client.BaseAddress = new Uri(canvasRedshiftSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var enrollmentTerms = JsonConvert.DeserializeObject<List<EnrollmentTermDTO>>(await client.GetStringAsync("EnrollmentTerm"));

            var foundUnusedCourses = new List<UnusedCourse>();
            foreach (var termId in termIds)
            {
                var unUsedCourses = JsonConvert.DeserializeObject<List<UnusedCourseDTO>>(await client.GetStringAsync($"Courses/GetUnusedCourses?termId={termId}"));
                foundUnusedCourses.AddRange(unUsedCourses.Select(x => new UnusedCourse()
                {
                    CourseId = x.Id.ToString(),
                    CourseName = x.Name,
                    CourseSISID = x.CanvasId.ToString(),
                    CourseCode = x.Code,
                    TermId = termId.ToString(),
                    Term = enrollmentTerms.First(y => y.CanvasId == termId).Name,
                    CourseSearchQueueId = nextSearch.Id,
                }));
            }
            
            // Separate the existing courses from the new courses
            var existingCourses = unusedCourseBll.GetAll().Where(x => foundUnusedCourses.Any(y => y.CourseId == x.CourseId));
            foreach (var course in existingCourses)
            {
                course.CourseSearchQueueId = nextSearch.Id;
            }

            foundUnusedCourses.RemoveAll(x => existingCourses.Any(y => y.CourseId == x.CourseId));

            unusedCourseBll.AddRange(existingCourses);
            unusedCourseBll.UpdateRange(existingCourses);
        }
    }
}
