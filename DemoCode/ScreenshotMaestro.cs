using OpenQA.Selenium;
using System.Text.RegularExpressions;

namespace DevOpsDaysDemo
{
    public class ScreenshotMaestro
    {
        private readonly List<IScreenshot> _screenshots = new();

        public void AddScreenshot(IScreenshot screenshot)
        {
            _screenshots.Add(screenshot);
        }

        public async Task ExecuteAsync(IWebDriver driver)
        {
            foreach (var screenshot in _screenshots)
            {
                await screenshot.CaptureAsync(driver);
            }
        }
    }

    public interface IScreenshot
    {
        string Id { get; set; }
        string GetUrl();
        Task CaptureAsync(IWebDriver driver);
    }

    public abstract class Screenshot(string id) : IScreenshot
    {
        protected const string SavePath = @"C:\Temp\DevOpsDays";
        public string Id { get; set; } = id;

        public abstract string GetUrl();
        
        public abstract Task CaptureAsync(IWebDriver driver);

        // Method to sanitize file names
        protected string SanitizeFileName(string fileName)
        {
            // Replace any invalid characters with a dash
            return Regex.Replace(fileName, @"[\\\/:*?""<>|]", "-");
        }
    }

}