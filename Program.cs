using System;
using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace content_automation
{
    class Program
    {
        static IWebDriver driver = null;


        static void Main(string[] args)
        {
            string youtubeChannelUrl = "https://www.youtube.com/channel/UC4HCouBLtXD1j1U_17aBqig";

            ChromeOptions options = new ChromeOptions();
#if RELEASE
            options.AddArgument("--headless");
#endif

            try
            {

                driver = new ChromeDriver(options);

                // go to youtube channel
                driver.Navigate().GoToUrl(youtubeChannelUrl);

                // go to the videos page
                driver.FindElement(By.LinkText("VIDEOS")).Click();

                // get all of the videos 
                ReadOnlyCollection<IWebElement> uploadedVideos = driver.FindElements(By.Id("video-title"));
                foreach (var video in uploadedVideos)
                {
                    video.Click();
                    break;
                }

                driver.FindElement(By.LinkText("Kenny The Almost Engineer"));

                string videoTitle = driver.Title.ToString();


                string videoDescription = driver.FindElement(By.Id("description")).Text.ToString();
                videoDescription = videoDescription.Substring(0, videoDescription.IndexOf("CHANNEL"));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (driver != null)
            {
                driver.Quit();
            }
        }


        static void PostTwitter()
        {
            string password="";

            driver.Navigate().GoToUrl("https://twitter.com/login");

            driver.FindElement(By.Name("session[username_or_email]")).SendKeys("almostengr");
            driver.FindElement(By.Name("session[password]")).SendKeys(password);
            driver.FindElement(By.XPath("//*[@id="react-root"]/div/div/div[2]/main/div/div/div[1]/form/div/div[3]/div/div/span/span")).Click();


        }
    }
}
