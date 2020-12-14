using System;
using System.Collections.Generic;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace Almostengr.SmAutomation.Models
{
    public class YouTubeVideo
    {
        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                ConvertUrlToThumbnailUrl(value);
                _url = value;
            }
        }

        public DateTime PostedDate { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; private set; }
        // public File ThumbnailImage { get; set; }
        public byte[] ThumbnailImage { get; set; }
        public string Description { get; set; }
        // public string Keywords { get; set; }
        public IList<string> Keywords { get; set; }
        public string Transcript { get; set; }

        public void ConvertUrlToThumbnailUrl(string value)
        {
            ThumbnailUrl = value.Replace("www.youtube.com/watch?v=", "i.ytimg.com/vi/") + "/maxresdefault.jpg";
        }
    }
}