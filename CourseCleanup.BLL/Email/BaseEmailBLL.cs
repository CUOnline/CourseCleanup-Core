using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;
using Hermes;

namespace CourseCleanup.BLL.Email
{
    public abstract class BaseEmailBLL
    {
        private readonly AppSettings appSettings;

        protected BaseEmailBLL(IOptions<AppSettings> appSettingsConfiguration)
        {
            this.appSettings = appSettingsConfiguration.Value;
        }

        /// <summary>
        /// Send email with only body replacements
        /// </summary>
        protected async Task SendEmailAsync(string template, string toEmail, Dictionary<string, string> bodyReplacements)
        {
            if (appSettings.SendEmails)
            {
                var sendEmailClient = new SendEmailClient(appSettings.HermesApiUrl, appSettings.HermesSecurityToken);
                await sendEmailClient.SendAsync(template, toEmail, bodyReplacements, "");
            }
        }

        /// <summary>
        /// Send email with subject and body replacements
        /// </summary>
        protected async Task SendEmailAsync(string template, string toEmail, Dictionary<string, string> subjectReplacements, Dictionary<string, string> bodyReplacements)
        {
            if (appSettings.SendEmails)
            {
                var sendEmailClient = new SendEmailClient(appSettings.HermesApiUrl, appSettings.HermesSecurityToken);
                await sendEmailClient.SendAsync(template, toEmail, "", "", bodyReplacements, subjectReplacements, "");
            }
        }
    }
}
