namespace CourseCleanup.Web.Models
{
    public class CanvasOAuth
    {
        public string AuthorizationEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}