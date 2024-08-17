using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DevOpsDaysDemo
{
    public class JiraIssue : Screenshot
    {


        public JiraIssue(string id, IConfigurationSection config) : base(id)
        {
            this.BaseUrl = config["JiraUrl"] ?? throw new NullReferenceException("JiraUrl not configured in appsettings.json");
        }

        public string BaseUrl { get; set; }

        public override string GetUrl()
        {
            return $"{BaseUrl}browse/{Id}";
        }

        public override Task CaptureAsync(IWebDriver driver)
        {
            Console.WriteLine($"Capturing {Id}");

            driver.Navigate().GoToUrl(GetUrl());

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            wait.Until(ExpectedConditions.ElementIsVisible(
               By.CssSelector("[data-testid='issue-activity-feed.ui.buttons.History']")));


            Thread.Sleep(500);

            var title = driver.Title;



            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile($"{SavePath}\\{Id}-{SanitizeFileName(title)}-Issue.png");

            driver.FindElement(By.CssSelector("[data-testid='issue-activity-feed.ui.buttons.History']")).Click();
            //wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            wait.Until(ExpectedConditions.ElementIsVisible(
                By.CssSelector("[data-testid='issue-activity-feed.ui.buttons.History']")));
            
            //Thread.Sleep(500);

            screenshot = ((ITakesScreenshot)driver).GetScreenshot();
            screenshot.SaveAsFile($"{SavePath}\\{SanitizeFileName(title)}-History.png");


            return Task.CompletedTask;
        }
    }
}
