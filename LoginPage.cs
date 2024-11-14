using OpenQA.Selenium;
using System;

public class LoginPage
{
    private readonly IWebDriver _driver;

    public LoginPage(IWebDriver driver)
    {
        _driver = driver;
    }

    private IWebElement usernameInput => _driver.FindElement(By.Id("user-name"));
    private IWebElement passwordInput => _driver.FindElement(By.Id("password"));
    private IWebElement loginButton => _driver.FindElement(By.Id("login-button"));
    private IWebElement errorMessage => _driver.FindElement(By.ClassName("error-message-container"));

    public void EnterUsername(string username) => usernameInput.SendKeys(username);
    public void EnterPassword(string password) => passwordInput.SendKeys(password);
    public void ClearUsername()
    {
        usernameInput.SendKeys(Keys.Control + "a");
        usernameInput.SendKeys(Keys.Delete);
    }

    public void ClearPassword()
    {
        passwordInput.SendKeys(Keys.Control + "a");
        passwordInput.SendKeys(Keys.Delete);
    }
    public void ClickLogin() => loginButton.Click();
    public string GetErrorMessage() => errorMessage.Text;
}