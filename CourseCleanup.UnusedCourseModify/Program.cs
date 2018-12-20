using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CourseCleanup.BLL;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Models;
using CourseCleanup.Models.Enums;
using CourseCleanup.Repository;
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
        private static CanvasRedshiftSettings canvasRedshiftSettings;

        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            //Setup the database options
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var appSettings = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);

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
            var provider = services.BuildServiceProvider();
            canvasRedshiftSettings = provider.GetService<IOptions<CanvasRedshiftSettings>>().Value;

            var sendEmailBll = provider.GetService<ISendEmailBLL>();
            var unusedCourseBll = provider.GetService<IUnusedCourseBLL>();

            var coursesPendingDeletion = unusedCourseBll.GetAll().Where(x => x.Status == CourseStatus.PendingDeletion).ToList();
            var coursesPendingReactivation = unusedCourseBll.GetAll().Where(x => x.Status == CourseStatus.PendingReactivation).ToList();
            
            client.BaseAddress = new Uri(canvasRedshiftSettings.CanvasUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", canvasRedshiftSettings.AuthToken);

            foreach (var unusedCourse in coursesPendingDeletion)
            {
                client.DeleteAsync($"courses/{unusedCourse.CourseId}?event=delete").GetAwaiter().GetResult();

                unusedCourse.Status = CourseStatus.Deleted;
                unusedCourseBll.Update(unusedCourse);
            }

            foreach (var unusedCourse in coursesPendingReactivation)
            {
                client.DeleteAsync($"courses/{unusedCourse.CourseId}?event=undelete").GetAwaiter().GetResult();

                unusedCourse.Status = CourseStatus.Active;
                unusedCourseBll.Update(unusedCourse);
            }

        }

        public static async void GetData()
        {
            var baseUrl = "http://canvas.ucdenver.edu/api/";

            using (var client = new HttpClient())
            {
                using (var res = await client.GetAsync(baseUrl))
                {
                    using (var content = res.Content)
                    {
                        Console.WriteLine(content.ReadAsStringAsync());
                    }
                }
            }
            
        }
    }
}
