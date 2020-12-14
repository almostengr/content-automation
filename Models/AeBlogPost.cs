using System;
using System.Collections.Generic;

namespace Almostengr.SmAutomation.Models
{
    public class AeBlogPost
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Posted { get; set; }
        public DateTime Updated { get; set; }
        public string Article { get; set; }
        public string Author { get; set; }
        public string BlogImageUrl { get; set; }
        public AeBlogPostType Category { get; set; }
        public IEnumerable<string> Keywords { get; set; }
    }

    public enum AeBlogPostType
    {
        Technology,
        Handyman
    }
}