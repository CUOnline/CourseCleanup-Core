using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS.Services.CanvasRedshift.Models
{
    public class UnusedCourseDTO
    {
        public long Id { get; set; }
        public long CanvasId { get; set; }
        public string EnrollmentTermId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string SisCourseId { get; set; }
        public string AccountId { get; set; }
    }
}
