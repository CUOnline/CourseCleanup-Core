using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseCleanup.Interface.BLL
{
    public interface IEmailQueueBLL
    {
        Task<bool> AddToQueue(string email, string emailTemplateName, Dictionary<string, string> bodyReplacements, Dictionary<string, string> subjectReplacements);
    }
}
