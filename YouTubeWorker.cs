using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Almostengr.SmAutomation.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace Almostengr.SmAutomation
{
    public class YouTubeWorker : BackgroundService
    {
        private IWebDriver driver = null;
        private readonly ILogger<YouTubeWorker> _logger;
        private readonly IConfiguration _config;
        private readonly AppSettings _appSettings;
        private TwitterClient twitterClient;
        private HttpClient httpClient;

        public YouTubeWorker(ILogger<YouTubeWorker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _appSettings = new AppSettings();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            ConfigurationBinder.Bind(_config, _appSettings);

            twitterClient = new TwitterClient(
                _appSettings.Twitter.ConsumerKey,
                _appSettings.Twitter.ConsumerSecret,
                _appSettings.Twitter.AccessToken,
                _appSettings.Twitter.AccessSecret
            );

            httpClient = new HttpClient();

            return base.StartAsync(cancellationToken);
        }

        private void StartBrowser()
        {
            ChromeOptions options = new ChromeOptions();
            // options.AddArgument("--headless");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string previousVideoTitle = "", currentVideoTitle = "";

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    StartBrowser();

                    driver.Navigate().GoToUrl(_appSettings.YouTube.ChannelUrl);
                    currentVideoTitle = GetLatestVideoTitle();

                    if (string.IsNullOrEmpty(previousVideoTitle))
                    {
                        previousVideoTitle = currentVideoTitle;
                    }
                    else if (previousVideoTitle != currentVideoTitle)
                    {
                        YouTubeVideo youTubeVideo = await GetVideoDetailsAsync();
                        previousVideoTitle = await PostNewVideoToTwitter(youTubeVideo, previousVideoTitle, currentVideoTitle);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is NoSuchElementException || ex is WebDriverException)
                    {
                        _logger.LogError(string.Concat(ex.GetType(), " ", ex.Message));
                    }
                }

                CloseBrowser();

                Random random = new Random();
                int randomDelay = random.Next(20, 40);
                await Task.Delay(TimeSpan.FromHours(randomDelay), stoppingToken);
            }
        }

        private async Task<string> PostNewVideoToTwitter(YouTubeVideo ytVideo, string previousVideoTitle, string currentVideoTitle)
        {
            string dayHashTag = "";
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                dayHashTag = "#saturdayproject ";
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
            {
                dayHashTag = "#techtuesday ";
            }

            string keywordHashtag = "";
            for (int i = 0; i < ytVideo.Keywords.Count; i++)
            {
                keywordHashtag += string.Concat("#", ytVideo.Keywords[i].Replace(" ", ""), " ");

                // limit the number of keywords used
                if (i == 2)
                {
                    break;
                }
            }

            string tweetText = string.Concat("NEW VIDEO: ", ytVideo.Title,
                "#youtuber ", "#youtube ", dayHashTag, keywordHashtag);

            while (tweetText.Length > 280)
            {
                _logger.LogWarning("Tweet is too long. Truncating. BEFORE: {tweetText}", tweetText);
                tweetText = tweetText.Substring(0, tweetText.LastIndexOf(" "));
            }

            await twitterClient.Tweets.PublishTweetAsync(tweetText);

            previousVideoTitle = currentVideoTitle;
            return previousVideoTitle;
        }

        private string GetLatestVideoTitle()
        {
            string currentVideoTitle = "";
            ReadOnlyCollection<IWebElement> uploadedVideos = driver.FindElements(By.Id("video-title"));

            foreach (var video in uploadedVideos)
            {
                currentVideoTitle = video.GetAttribute("title");
                video.Click();
                break;
            }

            return currentVideoTitle;
        }

        private async Task<YouTubeVideo> GetVideoDetailsAsync()
        {
            YouTubeVideo youTubeVideo = new YouTubeVideo();

            youTubeVideo.Title = driver.Title.Replace(" - YouTube", "");
            youTubeVideo.Url = driver.Url;
            youTubeVideo.PostedDate = Convert.ToDateTime(driver.FindElement(By.Id("date")).Text.Substring(1));
            youTubeVideo.Keywords = driver.FindElement(By.XPath("//meta[@name='keywords']")).GetAttribute("content").Split(",");

            string description = driver.FindElement(By.Id("description")).Text;
            youTubeVideo.Description = description.Substring(0, description.IndexOf("CHANNEL")).TrimEnd().TrimStart();

            HttpResponseMessage response = await httpClient.GetAsync(youTubeVideo.ThumbnailUrl);
            if (response.IsSuccessStatusCode)
            {
                byte[] content = await response.Content.ReadAsByteArrayAsync();
                // youTubeVideo.ThumbnailImage = new File();
                // System.Drawing.
                youTubeVideo.ThumbnailImage = content;
            }

            return youTubeVideo;
        }

        private async Task PostVideoToTwitterAsync(YouTubeVideo youTubeVideo)
        {
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

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            CloseBrowser();
            return base.StopAsync(cancellationToken);
        }

        private void CloseBrowser()
        {
            if (driver != null)
            {
                driver.Quit();
            }
        }
    }
}
