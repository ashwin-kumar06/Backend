using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

class Program
{
    static void Main()
    {
        // Set Chrome driver options
         // Maximize the browser window

        // Initialize ChromeDriver without specifying the path
        IWebDriver driver = new ChromeDriver();

        try
        {
            // Open the webpage
            driver.Navigate().GoToUrl("http://localhost:3000/login");

            // Wait for the page to load
            Thread.Sleep(3000); // You can use more sophisticated waits based on your specific needs

            // Find and interact with the login elements
            IWebElement emailInput = driver.FindElement(By.Name("email"));
            emailInput.SendKeys("ashsagvas123@gmail.com");

            IWebElement passwordInput = driver.FindElement(By.Name("password"));
            passwordInput.SendKeys("123456");

            IWebElement loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            loginButton.Click();

            // Wait for the login process to complete
            Thread.Sleep(5000); // Adjust as needed

            // You can add more actions here, like assertions to verify successful login

            // Close the browser
            driver.Quit();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            driver.Quit();
        }
    }
}