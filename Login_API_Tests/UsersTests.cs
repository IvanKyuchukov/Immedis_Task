using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V113.IndexedDB;
using OpenQA.Selenium.Support.UI;
using System.Linq.Expressions;
using SeleniumExtras.WaitHelpers;
using System.Xml.Linq;
using System.Threading;

namespace LibraryTests
{
    public class UsersTests
    {
        private WebDriver driver;
        private const string url = "https://qa-task.immedis.com/";       

        [SetUp]
        public void Setup()
        {
            this.driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Navigate().GoToUrl(url);
        }

        [TearDown]
        public void CloseBrowser()
        {
            this.driver.Quit();
        }       
      
        [Test]
        [TestCase("", "")]  // Login with empty username and password      
        [TestCase("", "123456")]  // Login with empty username
        [TestCase("admin", "")]  // Login with empty password
        [TestCase("test", "12345678")]  // Login with non-existent data
        [TestCase("admin", "123457")]  // Login with wrong password
        public void LoginWintIncorectCredentials(string username, string password)
        {         
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys(username);
            passwordField.SendKeys(password);
            loginButton.Click();

            //Thread.Sleep is used, because the system does not display error message for incorect user credentials.   
            Thread.Sleep(TimeSpan.FromSeconds(2));
            //The asserts must verify the element with error message when implemented
            Assert.That(usernameField.Displayed);
            Assert.That(passwordField.Displayed);
        }

        [Test]
        [TestCase("admin", "123456")]  // Login with correct user credentials  
        public void LoginWithCorrectCredentials(string username, string password)
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));             
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys(username);
            passwordField.SendKeys(password);
            loginButton.Click();
            var menuLibraryButton = driver.FindElement(By.XPath("//a[@class='navbar-brand' and text()='Library']"));

            Assert.That(menuLibraryButton.Displayed);            
        }

        [Test]
        [TestCase("01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567891")]  // Username longer than 100 symbols    
        [TestCase("")]  // Empty username
        public void createNewLibraryUserWithInvalidName(string libraryUsername)
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div")); 
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            driver.FindElement(By.XPath("//a[@href = '/Users/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(libraryUsername);
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            Assert.That(driver.FindElement(By.XPath("//h2[@class='text-danger' and text()='An error occurred while processing your request.']")).Displayed);
        }

        [Test]      
        public void createNewLibraryUserWithValidName()
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();            
            string libraryUsername = "user-" + new Random().Next();
            driver.FindElement(By.XPath("//a[@href = '/Users/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(libraryUsername);
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();
            driver.Navigate().GoToUrl(url+ "/Users");

            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'"+ libraryUsername + "')]")).Displayed);
        }

        [Test]
        [TestCase("01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567891")]  // Username longer than 100 symbols    
        [TestCase("")]  // Empty username
        public void updateLibraryUserWithInvalidName(string newLibraryUsername)
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            driver.FindElement(By.XPath("//td[contains(text(),'user-250390853')]/..//a[text()='Details']")).Click();
            driver.FindElement(By.XPath("//a[text()='Edit']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(Keys.Control + "A" + Keys.Delete);
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(newLibraryUsername);       
            driver.FindElement(By.XPath("//input[@value='Save']")).Click();
            Assert.That(driver.FindElement(By.XPath("//h2[@class='text-danger' and text()='An error occurred while processing your request.']")).Displayed);         
        }

        [Test]
        public void updateLibraryUserWithValidName()
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            string libraryUsername = "user-" + new Random().Next();
            driver.FindElement(By.XPath("//a[@href = '/Users/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(libraryUsername);
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            driver.Navigate().GoToUrl(url + "/Users");
            driver.FindElement(By.XPath("//td[contains(text(),'"+ libraryUsername + "')]/..//a[text()='Details']")).Click();
            driver.FindElement(By.XPath("//a[text()='Edit']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys("-New");
            libraryUsername += "-New";
            driver.FindElement(By.XPath("//input[@value='Save']")).Click();
            driver.Navigate().GoToUrl(url + "/Users");

            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + libraryUsername + "')]")).Displayed);
        }
    }
}