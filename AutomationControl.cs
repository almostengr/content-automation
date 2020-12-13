using System;
using System.Globalization;

namespace Almostengr.smautomation
{
    public class AutomationControl
    {
        public void RunAutomation()
        {
            // AeBlogPostProcessor websiteProcessor = new AeBlogPostProcessor();
            // AeBlogPost latestWebPost = websiteProcessor.GetLatestPost();

            // Console.WriteLine(latestWebPost.Article);
            // Console.WriteLine(latestWebPost.Category);
            // Console.WriteLine(latestWebPost.Description);
            // Console.WriteLine(latestWebPost.BlogImageUrl);
            // Console.WriteLine(latestWebPost.Posted);
            // Console.WriteLine(latestWebPost.Title);
            // // Console.WriteLine(latestWebPost.Updated);
            // Console.WriteLine(latestWebPost.Keywords);

            // YouTubeProcessor youTubeProcessor = new YouTubeProcessor();
            // YouTubeVideo youTubeVideo = youTubeProcessor.GetLatestVideo();

            // Console.WriteLine(youTubeVideo.Title);
            // Console.WriteLine(youTubeVideo.PostedDate);
            // Console.WriteLine(youTubeVideo.ThumbnailUrl);
            // Console.WriteLine(youTubeVideo.Url);
            // Console.WriteLine(youTubeVideo.Description);


            InstagramProcessor igProcessor = new InstagramProcessor();
            igProcessor.LoginInstagram();
            igProcessor.LikeHashtagPosts();
            igProcessor.CloseBrowser();
        }

        public string KeywordsToHashtags(string keywords, int maxTags = 20)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            string keywordsTitled = textInfo.ToTitleCase(keywords);
            keywordsTitled = keywordsTitled.Replace("#", "");

            var keywordsArray = keywordsTitled.Split(",");
            string hashtags = "";

            for (int i = 0; i < keywordsArray.Length; i++)
            {
                hashtags += keywordsArray[i];

                if (i >= maxTags)
                    break;
            }

            // string hashtags = keywordsTitled.Replace("#", "").Replace(" ", "").Replace(",", " #");
            hashtags = hashtags.Replace(" ", "").Replace(",", " #");

            return hashtags;
        }
    }
}