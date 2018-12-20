using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseCleanup.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseCleanup.Web.Models
{
    public class HomeViewModel
    {
        [Display(Name = "Start Term")]
        public string StartTerm { get; set; }

        [Display(Name = "End Term")]
        public string EndTerm { get; set; }

        public string UserEmail { get; set; }

        public IEnumerable<SelectListItem> Terms { get; set; }

        public IEnumerable<CourseSearchQueue> CourseSearchQueues { get; set; }

        public bool Authorized { get; set; }
        public string BaseCanvasUrl { get; set; }
    }
}
