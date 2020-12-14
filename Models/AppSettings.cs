using System.Collections.Generic;

namespace Almostengr.SmAutomation.Models
{
    public class AppSettings
    {
        public Twitter Twitter { get; set; }
        public Instagram Instagram { get; set; }
        public YouTube YouTube { get; set; }

        private bool _testing;
        public bool Testing
        {
            get { return _testing; }
            set { _testing = SetTesting(value); }
        }

        private bool SetTesting(bool? inValue)
        {
            if (inValue == null)
            {
                inValue = false;
            }
            return (bool)inValue;
        }
    }

    public class Twitter
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
        public string AccessToken { get; set; }
        public string AccessSecret { get; set; }
        public IList<string> FavoriteHashTags { get; set; }
        public IList<string> ExcludeWords { get; set; }
    }

    public class Instagram
    {

    }

    public class YouTube
    {
        public string ChannelUrl { get; set; }
        public string ApiKey { get; set; }
    }
}