using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models.Enums;

namespace CourseCleanup.Models
{
    public class CourseSearchQueue : ModelBase
    {
        public string TermList { get; set; }
        public SearchStatus Status { get; set; }
        public string StatusMessage { get; set; }
        public string SubmittedByEmail { get; set; }
    }
}
