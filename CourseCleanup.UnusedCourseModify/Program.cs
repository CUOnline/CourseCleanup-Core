using System;
using System.Linq;
using System.Net.Http;
using CourseCleanup.BLL;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Models.Enums;
using CourseCleanup.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseCleanup.UnusedCourseModify
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            //Setup the database options
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            var appSettings = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);

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

            var sendEmailBll = provider.GetService<ISendEmailBLL>();
            var unusedCourseBll = provider.GetService<IUnusedCourseBLL>();
            
            var coursesPendingDeletion = unusedCourseBll.GetAll().Where(x => x.Status == CourseStatus.PendingDeletion);

            foreach (var course in coursesPendingDeletion)
            {
                //ToDo: write logic to search for unused courses that are pending deletion.  Use the API to perform the deletion of a course.    
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
