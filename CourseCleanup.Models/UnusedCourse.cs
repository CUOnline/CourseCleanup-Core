using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models.Enums;

namespace CourseCleanup.Models
{
    public class UnusedCourse : ModelBase
    {
        [Display(Name = "Course Id")]
        public string CourseId { get; set; }

        [Display(Name = "Course Canvas Id")]
        public string CourseCanvasId { get; set; }

        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Display(Name = "Course SIS ID")]
        public string CourseSISID { get; set; }

        [Display(Name = "Course Code")]
        public string CourseCode { get; set; }

        public string AccountId { get; set; }

        public string TermId { get; set; }

        public string Term { get; set; }

        public CourseStatus Status { get; set; }

        public int CourseSearchQueueId { get; set; }

        public CourseSearchQueue CourseSearchQueue { get; set; }
    }
}
