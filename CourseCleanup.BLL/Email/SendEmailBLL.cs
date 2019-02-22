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
        public SendEmailBLL(IOptions<AppSettings> appSettingsConfiguration) : base(appSettingsConfiguration)
        {
        }

        public async Task SendUnusedCourseSearchCompletedEmailAsync(DateTime searchStartTimeStamp, DateTime searchEndTimeStamp, int numUnusedCoursesFound, List<string> termNames, int totalErrors, string toEmail)
        {
            var status = "";
            var statusText = "";

            if (totalErrors > 0)
            {
                status = "With Errors";
                statusText = "To view these errors, please navigate to the report on the website.";
            }

            var bodyReplacements = new Dictionary<string, string>
            {
                { "@DELETESTARTTIMESTAMP", searchStartTimeStamp.ToString("f") },
                { "@DELETEENDTIMESTAMP", searchEndTimeStamp.ToString("f") },
                { "@STATUS", status},
                { "@STATUSTEXT", statusText },
                { "@TOTALERRORS", totalErrors.ToString() },
                { "@NUMUNUSEDCOURSESFOUND", numUnusedCoursesFound.ToString() }
            };

            await SendEmailAsync("CUOnline_UnusedCourseSearch", toEmail, bodyReplacements);
        }

        public async Task SendBatchDeleteCoursesCompletedEmailAsync(DateTime deleteStartTimeStamp, DateTime deleteEndTimeStamp, int numUnusedCoursesDeleted, int totalErrors, string toEmail)
        {
            var status = "";
            var statusText = "";

            if (totalErrors > 0)
            {
                status = "With Errors";
                statusText = "To view these errors, please navigate to the report on the website.";
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
                { "@STATUSTEXT", statusText },
                { "@TOTALERRORS", totalErrors.ToString() },
                { "@NUMUNUSEDCOURSESDELETED", numUnusedCoursesDeleted.ToString() }
            };

            await SendEmailAsync("CUOnline_UnusedCourseSearch", toEmail, subjectReplacements, bodyReplacements);
        }
    }
}
