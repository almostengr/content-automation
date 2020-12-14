using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Almostengr.SmAutomation.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Almostengr.SmAutomation
{
    public class TaeWorker : BackgroundService
    {
        private readonly ILogger<TaeWorker> _logger;

        public TaeWorker(ILogger<TaeWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }



        public AeBlogPost GetLatestPost()
        {
            IWebDriver webDriver = null;
            AeBlogPost aeBlogPost = null;

            try
            {
                _logger.LogInformation("Checking for new blog posts");

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
                    _logger.LogInformation("Blog post does not have an image");
                    aeBlogPost.BlogImageUrl = null;
                }

                try
                { 
                    // aeBlogPost.Keywords = webDriver.FindElement(By.Id("keywords")).Text.Replace("Keywords: ", "");
                    aeBlogPost.Keywords = webDriver.FindElement(By.Id("keywords")).Text.Split(",");
                }
                catch (NoSuchElementException)
                {
                    _logger.LogInformation("Blog post does not have keywords");
                    // aeBlogPost.Keywords = 
                }
            }
            catch (WebDriverException wException)
            {
                _logger.LogInformation("Exception " + wException.Message);
            }

            if (webDriver != null)
            {
                _logger.LogInformation("Closing browser");
                webDriver.Quit();
            }

            _logger.LogInformation("Done checking website");

            return aeBlogPost;
        }
    }
}
