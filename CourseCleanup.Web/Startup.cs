using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CourseCleanup.BLL;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Interface.Repository;
using CourseCleanup.Repository;
using CourseCleanup.Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CourseCleanup.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            
            // BLLs
            services.AddScoped<ICourseSearchQueueBLL, CourseSearchQueueBLL>();
            services.AddScoped<IUnusedCourseBLL, UnusedCourseBLL>();
            services.AddScoped<ISendEmailBLL, SendEmailBLL>();
            services.AddScoped<IEmailQueueBLL, EmailQueueBLL>();

            // Repositories
            services.AddScoped<ICourseSearchQueueRepository, CourseSearchQueueRepository>();
            services.AddScoped<IUnusedCourseRepository, UnusedCourseRepository>();

            services.AddDbContext<CourseCleanupContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            var redshiftSettings = new CanvasRedshiftSettings();
            var redshiftSettingsSection = Configuration.GetSection(nameof(CanvasRedshiftSettings));
            redshiftSettingsSection.Bind(redshiftSettings);
            services.Configure<CanvasRedshiftSettings>(redshiftSettingsSection);

            services.AddHttpClient(HttpClientNames.CanvasRedshiftClient, client =>
            {
                client.BaseAddress = new Uri(redshiftSettings.BaseUrl);
                //client.DefaultRequestHeaders.Add("Accept", "application/json");
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiAuth.ApiKey);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    }
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
