using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

[TestFixture]
public class SeleniumTests
{
    private IWebDriver driver;

    [SetUp]
    public void Setup()
    {
        // Set Chrome driver options
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--start-maximized");

        // Initialize ChromeDriver with options
        driver = new ChromeDriver(options);
    }

    [Test]
    public void TestLogin()
    {
        try
        {
            // Open the webpage
            driver.Navigate().GoToUrl("http://localhost:3000/login");

            // Find and interact with the login elements
            IWebElement emailInput = driver.FindElement(By.Name("email"));
            emailInput.SendKeys("ashsagvas123@gmail.com");

            IWebElement passwordInput = driver.FindElement(By.Name("password"));
            passwordInput.SendKeys("123456");

            IWebElement loginButton = driver.FindElement(By.CssSelector("button[type='submit']"));
            loginButton.Click();

            // Wait for the login process to complete (You can use more sophisticated waits based on your specific needs)
            System.Threading.Thread.Sleep(5000); // Adjust as needed

            // You can add more assertions here to verify successful login
            Assert.Pass("Login successful.");
        }
        catch (Exception ex)
        {
            // Log any exceptions and fail the test
            Console.WriteLine("Exception: " + ex.Message);
            Assert.Fail("Login failed.");
        }
    }

    [TearDown]
    public void Teardown()
    {
        // Close the browser
        driver.Quit();
    }
}