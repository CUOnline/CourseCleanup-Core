using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS.Services.CanvasRedshift.Models
{
    public class EnrollmentTermDTO
    {
        public long Id { get; set; }
        public long CanvasId { get; set; }
        public long RootAccountId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SisSourceId { get; set; }
    }
}
