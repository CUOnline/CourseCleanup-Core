using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.BLL;
using CourseCleanup.BLL.Email;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;
using CourseCleanup.Repository;
using CourseCleanup.UnusedCourseModify.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CourseCleanup.UnusedCourseModify
{
    class Program
    {
        private static ServiceProvider provider;
        private static HttpClient client = new HttpClient();
        private static AppSettings appSettings;

        private const int MAXCOURSESTHATCANBEMODIFIED = 500;

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            //Setup the database options
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            //DB Context
            services.AddDbContext<CourseCleanupContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // BLLs
            services.AddScoped<ICourseSearchQueueBLL, CourseSearchQueueBLL>();
            services.AddScoped<IUnusedCourseBLL, UnusedCourseBLL>();
            services.AddScoped<ISendEmailBLL, SendEmailBLL>();

            // Repositories
            services.AddScoped<ICourseSearchQueueRepository, CourseSearchQueueRepository>();
            services.AddScoped<IUnusedCourseRepository, UnusedCourseRepository>();

            //Setup the provider for resolving dependencies
            provider = services.BuildServiceProvider();
            appSettings = provider.GetService<IOptions<AppSettings>>().Value;

            var sendEmailBll = provider.GetService<ISendEmailBLL>();
            var unusedCourseBll = provider.GetService<IUnusedCourseBLL>();
            var courseSearchQueueBll = provider.GetService<ICourseSearchQueueBLL>();

            client.BaseAddress = new Uri(appSettings.CanvasApiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appSettings.CanvasApiAuthToken);

            ProcessCoursesPendingDeletion(unusedCourseBll, courseSearchQueueBll, sendEmailBll);
            ProcessCoursesPendingReactivation(unusedCourseBll, sendEmailBll);
        }

        static void ProcessCoursesPendingDeletion(IUnusedCourseBLL unusedCourseBll, ICourseSearchQueueBLL courseSearchQueueBll, ISendEmailBLL sendEmailBll)
        {
            DateTime deleteStartTimeStamp = DateTime.Now;

            var coursesPendingDeletion = unusedCourseBll.GetAll().Where(x => x.Status == CourseStatus.PendingDeletion).ToList();
            
            if (coursesPendingDeletion.Any())
            {
                var coursesByAccount = coursesPendingDeletion.GroupBy(x => x.AccountId);
                var coursesDeleted = 0;
                var totalErrors = 0;

                foreach (var courseList in coursesByAccount)
                {
                    var courseChunks = courseList.ToList().ChunkBy(MAXCOURSESTHATCANBEMODIFIED);
                    foreach (var courseChunk in courseChunks)
                    {
                        var courseIds = courseChunk.Select(x => x.CourseId);

                        var form = new MultipartFormDataContent();
                        foreach (var courseId in courseIds)
                        {
                            form.Add(new StringContent(courseId), "course_ids[]");
                        }
                        form.Add(new StringContent("delete"), "event");

                        var result = client.PutAsync($"accounts/{courseList.Key}/courses", form).GetAwaiter().GetResult();

                        if (result.IsSuccessStatusCode)
                        {
                            coursesDeleted += courseChunk.Count;

                            foreach (var unusedCourse in courseChunk)
                            {
                                unusedCourse.Status = CourseStatus.Deleted;
                            }

                            unusedCourseBll.UpdateRange(courseChunk);
                        }
                        else
                        {
                            totalErrors++;
                            Console.WriteLine("broke");
                        }
                    }
                }

                var deleteEndTimeStamp = DateTime.Now;

                // Send email if DELETE ALL was requested
                var courseSearchQueueIds = coursesPendingDeletion.Where(x => x.CourseSearchQueue.DeleteAllRequested)
                    .Select(x => x.CourseSearchQueueId).Distinct().ToList();

                if (courseSearchQueueIds.Any())
                {
                    foreach (var courseSearchQueueId in courseSearchQueueIds)
                    {
                        var courseSearchQueue = courseSearchQueueBll.Get(courseSearchQueueId);

                        sendEmailBll.SendBatchDeleteCoursesCompletedEmailAsync(deleteStartTimeStamp, deleteEndTimeStamp,
                            coursesDeleted, totalErrors, courseSearchQueue.SubmittedByEmail);
                    }
                }
            }
        }

        static void ProcessCoursesPendingReactivation(IUnusedCourseBLL unusedCourseBll, ISendEmailBLL sendEmailBll)
        {
            var coursesPendingReactivation = unusedCourseBll.GetAll().Where(x => x.Status == CourseStatus.PendingReactivation).ToList();
            foreach (var unusedCourse in coursesPendingReactivation)
            {
                var form = new MultipartFormDataContent();
                form.Add(new StringContent(unusedCourse.CourseId), "course_ids[]");
                form.Add(new StringContent("undelete"), "event");

                var result = client.PutAsync($"accounts/{unusedCourse.AccountId}/courses", form).GetAwaiter()
                    .GetResult();

                if (result.IsSuccessStatusCode)
                {
                    // TODO: This call return an asyncronous task url internal to Canvas.  Need to get the url and wait for it to be completed
                    unusedCourse.Status = CourseStatus.Active;
                    unusedCourseBll.Update(unusedCourse);
                }
            }
        }
    }
}
