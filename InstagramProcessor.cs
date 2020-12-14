using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Almostengr.SmAutomation
{
    public class InstagramProcessor : BackgroundService
    {

        IWebDriver driver = null;
        private readonly ILogger<InstagramProcessor> _logger;

        public InstagramProcessor(ILogger<InstagramProcessor> logger)
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

        public void LoginInstagram()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

            driver.Manage().Window.Maximize();

            driver.Navigate().GoToUrl("https://www.instagram.com/");

            driver.FindElement(By.Name("username")).SendKeys("almostengr");
            driver.FindElement(By.Name("password")).SendKeys("pa!Ins19");
            driver.FindElement(By.Name("username")).Submit();

            for (int i = 0; i < 2; i++)
            {
                // first time for save login, second for notifications
                driver.FindElement(By.XPath("//button[text()='Not Now']")).Click();
            }
        }

        public void CloseBrowser()
        {
            Thread.Sleep(10000);

            if (driver != null)
            {
                driver.Quit();
            }
        }

        public void LikeHashtagPosts()
        {
            string[] hashTags = { "programming", }; // "learntocode", "buildupdevs", "saturdayproject", "handyman" };

            foreach (var tag in hashTags)
            {
                driver.Navigate().GoToUrl($"https://www.instagram.com/explore/tags/{tag}");

                var images = driver.FindElements(By.ClassName("_9AhH0"));

                int likeCounter = 0;
                // for (int i = 0; i < 3; i++)
                foreach (var image in images)
                {
                    // images[i].Click();
                    image.Click();
                    likeCounter++;

                    try
                    {
                        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                        // var likeButton = driver.FindElement(By.CssSelector("[aria-label='Like']"));
                        var likeButton = By.CssSelector("[aria-label='Like']");
                        // wait.Until(drv => drv.FindElement(likeButton));

                        // Thread.Sleep(3000);
                        
                        // driver.FindElement(likeButton).Click();
                        // driver.FindElement(By.XPath("/html/body/div[5]/div[2]/div/article/div[3]/section[1]/span[1]/button/div/span/svg"))
                        // driver.FindElement(By.XPath("//span/svg/"))
                        // driver.FindElement(By.ClassName("_8-yf5")).Click();
                        // driver.FindElement(By.ClassName("FY9nT")).Click();
                        // driver.FindElement(By.ClassName("QBdPU")).Click();
                        driver.FindElement(By.XPath("//span/svg[@aria-label='Like']")).Click();

                    }
                    catch (NoSuchElementException)
                    {
                        _logger.LogInformation("Did not find Like button. Perhaps image already liked.");
                    }

                    driver.FindElement(By.CssSelector("[aria-label='Close']")).Click();

                    if (likeCounter >= 3)
                    {
                        break;
                    }
                }
            }
        }
    }
}