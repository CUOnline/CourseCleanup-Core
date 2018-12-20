using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using Hermes.Core.Clients;
using Hermes.Core.Clients.Models;

namespace CourseCleanup.BLL
{
    public class SendEmailBLL : ISendEmailBLL
    {
        public async Task SendUnusedCourseSearchReportFinishedEmailAsync(string status)
        {
            var subjectReplacements = new Dictionary<string, string>
            {
                {"@STATUS", status}
            };

            var bodyReplacements = new Dictionary<string, string>
            {
            };

            await SendEmail("ALEXANDER.KARKLINS@UCDENVER.EDU", "CUOnline_UnusedCourseSearch", bodyReplacements, subjectReplacements);
        }

        public Task<bool> SendEmail(string toEmail, string emailTemplate, Dictionary<string, string> bodyReplacementValues, Dictionary<string, string> subjectReplacementValues)
        {
            throw new NotImplementedException();
        }
    }
}
