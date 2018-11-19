using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Interface.BLL;
using CourseCleanup.Models;
using Hermes.Core.Clients;
using Hermes.Core.Clients.Models;
using Microsoft.Extensions.Options;

namespace CourseCleanup.BLL
{
    public class EmailQueueBLL : IEmailQueueBLL
    {
        private readonly AppSettings appSettings;

        public EmailQueueBLL(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public async Task<bool> AddToQueue(string toEmail, string emailTemplate, Dictionary<string, string> bodyReplacements, Dictionary<string, string> subjectReplacements)
        {
            if (appSettings.SendEmails)
            {
                var sendEmailClient = new SendEmailClient(appSettings.HermesApi);

                if (!string.IsNullOrEmpty(toEmail))
                {
                    var emailItem = new EmailItem
                    {
                        ToEmail = toEmail,
                        EmailTemplate = emailTemplate,
                        AuthorizationToken = appSettings.AuthorizationToken,
                        ReplacementValues = bodyReplacements,
                        SubjectReplacementValues = subjectReplacements
                    };

                    await sendEmailClient.SendEmailAsync(emailItem);
                    return true;
                }
            }

            return false;
        }
    }
}
