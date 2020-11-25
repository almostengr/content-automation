using System;

namespace Almostengr.smautomation
{
    public class YouTubeVideo
    {
        public string Url { get; set; }
        public DateTime PostedDate { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }

        public string ConvertUrlToThumbnailUrl()
        {
            return Url.Replace("www.youtube.com/watch?v=", "i.ytimg.com/vi/") + "/maxresdefault.jpg";
        }
    }
}