using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Models;
using Microsoft.Extensions.Options;

namespace CourseCleanup.BLL.Email
{
    public class SendEmailBLL : BaseEmailBLL, ISendEmailBLL
    {
        private readonly AppSettings appSettings;

        public SendEmailBLL(IOptions<AppSettings> appSettingsConfiguration) : base(appSettingsConfiguration)
        {
            this.appSettings = appSettingsConfiguration.Value;
        }

        public async Task SendUnusedCourseSearchCompletedEmailAsync(DateTime searchStartTimeStamp, DateTime searchEndTimeStamp, int numUnusedCoursesFound, string termNames, List<string> errors, string toEmail)
        {
            var status = "";
            var statusText = "";

            if (errors.Count > 0)
            {
                status = "With Errors";
                statusText = string.Join("<BR>", errors);
            }

            var subjectReplacements = new Dictionary<string, string>
            {
                {"@STATUS", status}
            };

            var bodyReplacements = new Dictionary<string, string>
            {
                { "@SEARCHSTARTTIMESTAMP", searchStartTimeStamp.ToString("f") },
                { "@SEARCHENDTIMESTAMP", searchEndTimeStamp.ToString("f") },
                { "@STATUS", status},
                { "@ERRORTEXT", statusText },
                { "@TOTALERRORS", errors.Count.ToString() },
                { "@NUMUNUSEDCOURSESFOUND", numUnusedCoursesFound.ToString() },
                { "@TERMNAMES", termNames }
            };

            await SendEmailAsync(EmailTemplate.UnusedCourseSearchCompleted, toEmail, subjectReplacements, bodyReplacements);

            if (toEmail != appSettings.AdminEmail)
            {
                await SendEmailAsync(EmailTemplate.UnusedCourseSearchCompleted, appSettings.AdminEmail, subjectReplacements, bodyReplacements);
            }
        }

        public async Task SendBatchDeleteCoursesCompletedEmailAsync(DateTime deleteStartTimeStamp, DateTime deleteEndTimeStamp, int numUnusedCoursesDeleted, List<string> errors, string toEmail)
        {
            var status = "";
            var statusText = "";

            if (errors.Count > 0)
            {
                status = "With Errors";
                statusText = string.Join("<BR>", errors);
            }

            var subjectReplacements = new Dictionary<string, string>
            {
                {"@STATUS", status}
            };

            var bodyReplacements = new Dictionary<string, string>
            {
                { "@DELETESTARTTIMESTAMP", deleteStartTimeStamp.ToString("f") },
                { "@DELETEENDTIMESTAMP", deleteEndTimeStamp.ToString("f") },
                { "@STATUS", status},
                { "@ERRORTEXT", statusText },
                { "@TOTALERRORS", errors.Count.ToString() },
                { "@NUMUNUSEDCOURSESDELETED", numUnusedCoursesDeleted.ToString() }
            };

            await SendEmailAsync(EmailTemplate.BatchDeleteCoursesCompleted, toEmail, subjectReplacements, bodyReplacements);

            if (toEmail != appSettings.AdminEmail)
            {
                await SendEmailAsync(EmailTemplate.BatchDeleteCoursesCompleted, appSettings.AdminEmail, subjectReplacements, bodyReplacements);
            }
        }
    }
}
