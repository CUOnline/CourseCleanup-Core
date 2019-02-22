using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseCleanup.Interface.BLL
{
    public interface ISendEmailBLL
    {
        Task SendUnusedCourseSearchCompletedEmailAsync(DateTime searchStartTimeStamp, DateTime searchEndTimeStamp, int numUnusedCoursesFound, string termNames, List<string> errors, string toEmail);

        Task SendBatchDeleteCoursesCompletedEmailAsync(DateTime deleteStartTimeStamp, DateTime deleteEndTimeStamp, int numUnusedCoursesDeleted, List<string> errors, string toEmail);
    }
}
