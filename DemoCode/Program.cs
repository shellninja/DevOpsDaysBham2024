using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DevOpsDaysDemo
{
    internal class Program
    {
        private const string screenshotDirectory = @"C:\Temp\DevOpsDays";

        private const string jiraUrl = "https://o3insight.atlassian.net/";

        //you could use a CSV import here if you wanted.
        private static readonly string[] Issues = ["O3OPS-606", "O3OPS-585", "O3OPS-581", "O3OPS-584"];

        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .Build();

            var jiraSettings = configuration.GetSection("JiraSettings");
            var jiraUrl = jiraSettings["JiraUrl"];

            if (!Directory.Exists(screenshotDirectory))
            {
                Directory.CreateDirectory(screenshotDirectory);
            }

            CleanDirectory(screenshotDirectory);

            Console.WriteLine("Hello, Birmingham!");

            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");

            using (var driver = new ChromeDriver(options))
            {

                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

                await driver.Navigate().GoToUrlAsync(jiraUrl);

                // Wait for the page to be fully loaded
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
                wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                // Wait for the username field to be visible and interactable
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("username")));
                driver.FindElement(By.Id("username")).SendKeys(jiraSettings["Username"]);

                // Click the submit button after entering the username
                driver.FindElement(By.Id("login-submit")).Click();

                // Wait for the password field to be visible and interactable
                wait.Until(ExpectedConditions.ElementIsVisible(By.Id("password")));
                driver.FindElement(By.Id("password")).SendKeys(jiraSettings["Password"]);

                // Click the submit button after entering the password
                driver.FindElement(By.Id("login-submit")).Click();


                Console.WriteLine("2FA Away!");
                Console.ReadLine();

                var maestro = new ScreenshotMaestro();

                foreach (var issueNum in Issues)
                {
                    maestro.AddScreenshot(new JiraIssue(issueNum, jiraSettings));
                }

                await maestro.ExecuteAsync(driver);
            }
        }

        private static void CleanDirectory(string directory)
        {
            foreach (var file in Directory.EnumerateFiles(directory))
            {
                File.Delete(file);
            }
        }
    }
}
