using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;

[assembly: Parallelize(Workers = 3, Scope = ExecutionScope.MethodLevel)]
[TestClass]
public class LoginTests
{
    private LoginPage _loginPage;
    private IWebDriver _driver;

    public LoginTests()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/test-log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    [TestInitialize]
    public void SetUp()
    {
        Log.Information("Starting test setup");
        _driver = CreateDriver("chrome");
        _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
        _driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);
        _driver.Navigate().GoToUrl("https://www.saucedemo.com/");
        _loginPage = new LoginPage(_driver);
        Log.Information("Setup complete and navigated to the login page");
    }

    private static IWebDriver CreateDriver(string browser)
    {
        IWebDriver driver;

        switch (browser.ToLower())
        {
            case "firefox":
                var firefoxOptions = new FirefoxOptions();
                firefoxOptions.SetPreference("dom.webnotifications.enabled", false);
                driver = new FirefoxDriver(firefoxOptions);
                driver.Manage().Window.Maximize();
                break;

            case "edge":
                var edgeOptions = new EdgeOptions();
                edgeOptions.AddArgument("start-maximized");
                edgeOptions.AddArgument("--disable-gpu");
                edgeOptions.AddArgument("--no-sandbox");
                driver = new EdgeDriver(edgeOptions);
                break;

            case "chrome":
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--start-maximized");
                chromeOptions.AddArgument("--disable-notifications");
                chromeOptions.AddArgument("--disable-extensions");
                driver = new ChromeDriver(chromeOptions);
                break;

            default:
                throw new ArgumentException("Unsupported browser!");
        }

        return driver;
    }

    [TestMethod]
    [DataRow("any_user", "any_password")]
    [DataRow("sdaadsfsd", "asjjjjjjj")]
    [DataRow("aaaaaaaaaaa", "bbbbbbbb")]
    public void TestEmptyCredentials(string username, string password)
    {
        Log.Information("Testing login with empty credentials");

        _loginPage.EnterUsername(username);
        _loginPage.EnterPassword(password);

        _loginPage.ClearUsername();
        _loginPage.ClearPassword();

        _loginPage.ClickLogin();

        _loginPage.GetErrorMessage().Should().Contain("Username is required");
        Log.Information("Test for empty credentials completed successfully");
    }

    [TestMethod]
    [DataRow("any_user", "any_password")]
    [DataRow("111111111", "22222222222")]
    [DataRow("rrrrrrrrr", "ttttttttttt")]
    public void TestMissingPassword(string username, string password)
    {
        Log.Information("Testing login with missing password");

        _loginPage.EnterUsername(username);
        _loginPage.EnterPassword(password);

        _loginPage.ClearPassword();

        _loginPage.ClickLogin();

        _loginPage.GetErrorMessage().Should().Contain("Password is required");
        Log.Information("Test for missing password completed successfully");
    }

    [TestMethod]
    [DataRow("standard_user", "secret_sauce")]
    [DataRow("problem_user", "secret_sauce")]
    [DataRow("visual_user", "secret_sauce")]
    public void TestSuccessfulLogin(string username, string password)
    {
        Log.Information("Testing successful login");

        _loginPage.EnterUsername(username);
        _loginPage.EnterPassword(password);
        _loginPage.ClickLogin();

        _driver.Title.Should().Be("Swag Labs");
        Log.Information("Successful login test completed successfully");
    }

    [TestCleanup]
    public void TearDown()
    {
        Log.Information("Tearing down test and quitting driver");
        _driver?.Quit();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Log.CloseAndFlush();
    }
}