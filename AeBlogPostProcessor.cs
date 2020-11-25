using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.smautomation
{
    public class AeBlogPostProcessor
    {
        public AeBlogPost GetLatestPost()
        {
            IWebDriver webDriver = null;
            AeBlogPost aeBlogPost = null;

            try
            {
                Logger.LogMessage("Checking for new blog posts");

                webDriver = new ChromeDriver();
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
                webDriver.Navigate().GoToUrl("https://thealmostengineer.com/blog");
                webDriver.FindElement(By.LinkText("All")).Click();

                webDriver.FindElement(By.LinkText("READ MORE")).Click(); // open the first blog post

                aeBlogPost = new AeBlogPost();
                aeBlogPost.Description = webDriver.FindElement(By.XPath("//meta[@name='description']")).GetAttribute("content").TrimEnd().TrimStart();
                // aeBlogPost.Title = webDriver.Title.Replace(" | Almost Engineer Services", "");
                aeBlogPost.Title = webDriver.FindElement(By.TagName("h1")).Text;
                aeBlogPost.Article = webDriver.FindElement(By.TagName("article")).Text;
                aeBlogPost.Posted = Convert.ToDateTime(webDriver.FindElement(By.Id("posted")).Text.Replace("Posted: ", "").Replace("Published: ", ""));

                try
                {
                    aeBlogPost.BlogImageUrl = webDriver.FindElement(By.Id("blogimage")).GetAttribute("src");
                }
                catch (NoSuchElementException)
                {
                    Logger.LogMessage("Blog post does not have an image");
                    aeBlogPost.BlogImageUrl = null;
                }

                try
                { 
                    aeBlogPost.Keywords = webDriver.FindElement(By.Id("keywords")).Text.Replace("Keywords: ", "");
                }
                catch (NoSuchElementException)
                {
                    Logger.LogMessage("Blog post does not have keywords");
                    aeBlogPost.Keywords = "";
                }
            }
            catch (WebDriverException wException)
            {
                Logger.LogMessage("Exception " + wException.Message);
            }

            if (webDriver != null)
            {
                Logger.LogMessage("Closing browser");
                webDriver.Quit();
            }

            Logger.LogMessage("Done checking website");

            return aeBlogPost;
        }
    }
}