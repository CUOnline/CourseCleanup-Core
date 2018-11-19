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
        public Task<bool> SendEmail(string toEmail, string emailTemplate, Dictionary<string, string> bodyReplacementValues, Dictionary<string, string> subjectReplacementValues)
        {
            throw new NotImplementedException();
        }
    }
}
