using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.smautomation
{
    public class YouTubeProcessor
    {
        public YouTubeVideo GetLatestVideo()
        {
            YouTubeVideo youtubeVideo = new YouTubeVideo();
            IWebDriver webDriver = null;

            try
            {
                webDriver = new ChromeDriver();
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                webDriver.Navigate().GoToUrl("https://www.youtube.com/channel/UC4HCouBLtXD1j1U_17aBqig/videos");

                // get all of the videos 
                ReadOnlyCollection<IWebElement> uploadedVideos = webDriver.FindElements(By.Id("video-title"));
                foreach (var video in uploadedVideos)
                {
                    video.Click();
                    break;
                }

                webDriver.FindElement(By.LinkText("Kenny The Almost Engineer"));

                youtubeVideo.Title = webDriver.Title.Replace(" - YouTube", "");
                youtubeVideo.Url = webDriver.Url;
                youtubeVideo.PostedDate = Convert.ToDateTime(webDriver.FindElement(By.Id("date")).Text.Substring(1));
                youtubeVideo.ThumbnailUrl = youtubeVideo.ConvertUrlToThumbnailUrl();

                string description = webDriver.FindElement(By.Id("description")).Text;
                youtubeVideo.Description = description.Substring(0, description.IndexOf("CHANNEL")).TrimEnd().TrimStart();

                youtubeVideo.Keywords = webDriver.FindElement(By.XPath("//meta[@name='keywords']")).GetAttribute("content");
            }
            catch (WebDriverException ex)
            {
                Logger.LogMessage("Exception: " + ex.Message);
            }

            if (webDriver != null)
            {
                webDriver.Quit();
            }

            return youtubeVideo;
        }
    }
}