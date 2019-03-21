using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseCleanup.Models
{
    public class AppSettings
    {
        // Hermes
        public string HermesApiUrl { get; set; }
        public string HermesSecurityToken { get; set; }
        public bool SendEmails { get; set; }

        // Canvas Redshift
        public string CanvasRedshiftApiUrl { get; set; }
        public string CanvasRedshiftConnectionString { get; set; }

        // Canvas API
        public string CanvasApiUrl { get; set; }
        public string CanvasApiAuthToken { get; set; }

        // Canvas OAuth
        public string CanvasOAuthBaseUrl { get; set; }
        public string CanvasOAuthAuthorizationEndpointUrl { get; set; }
        public string CanvasOAuthTokenEndpointUrl { get; set; }
        public string CanvasOAuthClientId { get; set; }
        public string CanvasOAuthClientSecret { get; set; }

        // Misc
        public string AdminEmail { get; set; }
    }
}
