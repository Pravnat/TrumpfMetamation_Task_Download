using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrumpfMetamation_Task_Download
{
    [TestFixture]
    class DownloadTask
    {
        private IWebDriver driver;
        private string downloadFolder;
        [SetUp]

        public void SetUp()
        {
            downloadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");

            // Ensure the folder exists
            if (!Directory.Exists(downloadFolder))
            {
                Directory.CreateDirectory(downloadFolder);
            }

            // Setup Chrome options for automatic download without prompt
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("download.default_directory", downloadFolder);
            chromeOptions.AddUserProfilePreference("download.prompt_for_download", false);
            chromeOptions.AddUserProfilePreference("safebrowsing.enabled", true);

            // Initialize WebDriver
            driver = new ChromeDriver(chromeOptions);
        }

        [Test]
        public void DownLoadFileTest()
        {

            driver.Navigate().GoToUrl("https://orbiter-for-testing.azurewebsites.net/products/testApp?isInternal=false");

            IWebElement file = driver.FindElement(By.XPath("//ul[@class='list-group list-group-flush']//li//a"));
            file.Click();

            // Wait for the file to appear in the download folder
            string downloadedFilePath = WaitForDownload();

            // Assert that the file has been downloaded
            Assert.IsTrue(File.Exists(downloadedFilePath), "File was not downloaded successfully.");

            // Optionally: you can perform additional checks like verifying the file's size, extension, etc.
            Console.WriteLine("Downloaded file path: " + downloadedFilePath);



        }
        private string WaitForDownload()
        {
            string downloadedFilePath = string.Empty;

            // Wait for the file to appear in the download folder
            DateTime timeout = DateTime.Now.AddMinutes(1); // Timeout after 1 minute
            while (DateTime.Now < timeout)
            {
                // Check for files in the download folder
                var files = Directory.GetFiles(downloadFolder);
                if (files.Length > 0)
                {
                    // Assuming the first file found is the one downloaded
                    downloadedFilePath = files[0];
                    break;
                }

                Thread.Sleep(500); // Check every 500ms
            }

            return downloadedFilePath;
        }
        [TearDown]
        public void TearDown()
        {
            // Close the browser
            driver.Quit();
        }
    }

    
}
