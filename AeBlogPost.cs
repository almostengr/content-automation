using System;

namespace Almostengr.smautomation
{
    public class AeBlogPost
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Posted { get; set; }
        // public DateTime Updated { get; set; }
        public string Article { get; set; }
        public string BlogImageUrl { get; set; }
        public string Category { get; set; }
        public string Keywords { get; set; }
    }

    public enum AeBlogPostType
    {
        Technology,
        Handyman
    }
}