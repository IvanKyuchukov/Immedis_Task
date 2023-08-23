using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V113.IndexedDB;
using OpenQA.Selenium.Support.UI;
using System.Linq.Expressions;
using SeleniumExtras.WaitHelpers;
using System.Xml.Linq;
using System.Threading;
using NUnit.Framework;
using System.Globalization;

namespace LibraryTests
{
    public class BooksTests
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

        [Test, Order(1)]
        [TestCase("", "Joanne Rowling", "Fantasy", 100)]  // New book with empty name     
        [TestCase("Harry Potter", "", "Fantasy", 100)]  // New book with empty author  
        [TestCase("Harry Potter", "Joanne Rowling", "", 100)]  // New book with empty genre  
        [TestCase("01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567891", "Joanne Rowling", "Fantasy", "100")]  // New book with name longer than 250 symbols
        [TestCase("Harry Potter", "01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567891", "Fantasy", "100")]  // New book with Author longer than 100 symbols 
        [TestCase("Harry Potter", "Joanne Rowling", "012345678901234567890123456789012345678901234567891", 100)]  // New book with Genre longer than 50 symbols         
        [TestCase("Harry Potter", "Joanne Rowling", "Fantasy", -10)]  // New book with negative quontity
        public void createNewBookWithInvalidData(string name, string author, string genre, int quontity)
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();

            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//a[@href = '/Books/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(name);
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys(author);
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys(genre);
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys(quontity.ToString());
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            Assert.That(driver.FindElement(By.XPath("//h2[@class='text-danger' and text()='An error occurred while processing your request.']")).Displayed);
        } 
       
        [Test, Order(2)]      
        public void createNewBookWithValidName()
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            string bookName = "Book-" + new Random().Next();
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();

            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//a[@href = '/Books/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(bookName);
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys("Joanne Rowling");
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys("Fantasy");
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys("100");
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            driver.Navigate().GoToUrl(url+ "/Books");
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'"+ bookName + "')]")).Displayed);
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "')]/../*[2]")).Text, Is.EqualTo("Joanne Rowling"));
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "')]/../*[3]")).Text, Is.EqualTo("Fantasy"));
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "')]/../*[4]")).Text, Is.EqualTo("100"));
        }

        [Test, Order(3)]
        public void editNewBookWithValidData()
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            string bookName = "Book-" + new Random().Next();         
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//a[@href = '/Books/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(bookName);
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys("Joanne Rowling");
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys("Fantasy");
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys("100");
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//td[contains(text(),'"+bookName+"')]/..//a[text()='Edit']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys("-New");
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys("-New");
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys("-New");
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys("1");
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Save']")).Click();

            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "-New')]")).Displayed);
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "-New')]/../*[2]")).Text, Is.EqualTo("Joanne Rowling-New"));
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "-New')]/../*[3]")).Text, Is.EqualTo("Fantasy-New"));
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "-New')]/../*[4]")).Text, Is.EqualTo("1100"));
        }

        [Test, Order(4)]
        [TestCase("", "Joanne Rowling", "Fantasy", 100)]  // New book with empty name     
        [TestCase("Harry Potter", "", "Fantasy", 100)]  // New book with empty author  
        [TestCase("Harry Potter", "Joanne Rowling", "", 100)]  // New book with empty genre  
        [TestCase("01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567891", "Joanne Rowling", "Fantasy", "100")]  // New book with name longer than 250 symbols
        [TestCase("Harry Potter", "01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567891", "Fantasy", "100")]  // New book with Author longer than 100 symbols 
        [TestCase("Harry Potter", "Joanne Rowling", "012345678901234567890123456789012345678901234567891", 100)]  // New book with Genre longer than 50 symbols         
        [TestCase("Harry Potter", "Joanne Rowling", "Fantasy", -10)]  // New book with negative quontity
        public void editNewBookWithInvalidData(string name, string author, string genre, int quontity)
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            string bookName = "Book-" + new Random().Next();
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//a[@href = '/Books/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(bookName);
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys("Joanne Rowling");
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys("Fantasy");
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys("100");
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "')]/..//a[text()='Edit']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(Keys.Control + "A" + Keys.Delete);
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys(Keys.Control + "A" + Keys.Delete);
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys(Keys.Control + "A" + Keys.Delete);
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys(Keys.Control + "A" + Keys.Delete);
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(name);            
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys(author);
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys(genre);
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys(quontity.ToString());
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Save']")).Click();

            Assert.That(driver.FindElement(By.XPath("//h2[@class='text-danger' and text()='An error occurred while processing your request.']")).Displayed);
        }



        [Test, Order(5)]
        public void deleteNewBookWithValidData()
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            string bookName = "Book-" + new Random().Next();
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();

            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//a[@href = '/Books/Create' and text()='Create New']")).Click();
            driver.FindElement(By.XPath("//input[@id='Name']")).SendKeys(bookName);
            driver.FindElement(By.XPath("//input[@id='Author']")).SendKeys("Joanne Rowling");
            driver.FindElement(By.XPath("//input[@id='Genre']")).SendKeys("Fantasy");
            driver.FindElement(By.XPath("//input[@id='Quontity']")).SendKeys("100");
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            driver.Navigate().GoToUrl(url + "/Books");
            driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "')]/..//a[text()='Delete']")).Click();
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Delete']")).Click();
            driver.Navigate().GoToUrl(url + "/Books");

            try
            {
                driver.FindElement(By.XPath("//td[contains(text(),'" + bookName + "')]"));
                Assert.Fail("Element exist");
            }
            catch (NoSuchElementException)
            {
                Assert.Pass("Element does not exist");
            }        
        }

        [Test, Order(6)]
        public void addBookRequest()
        {
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));           
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();

            driver.Navigate().GoToUrl(url + "/GetBook");
            driver.FindElement(By.XPath("//a[@href = '/GetBook/Create' and text()='Create New']")).Click();
            new SelectElement(driver.FindElement(By.XPath("//select[@id='UserId']"))).SelectByText("user-37124999");
            new SelectElement(driver.FindElement(By.XPath("//select[@id='BookId']"))).SelectByText("Joanne Rowling");
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Create']")).Click();

            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'user-37124999')]")).Displayed);
        }

        [Test, Order(7)]
        public void editBookRequest()
        {
            DateTime currentDateTime = DateTime.Now;
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));           
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            
            driver.Navigate().GoToUrl(url + "/GetBook");
            driver.FindElement(By.XPath("//td[contains(text(),'user-37124999')]/..//a[text()='Edit']")).Click();
            new SelectElement(driver.FindElement(By.XPath("//select[@id='UserId']"))).SelectByText("user-250390853");
            new SelectElement(driver.FindElement(By.XPath("//select[@id='BookId']"))).SelectByText("Steven King");
            driver.FindElement(By.XPath("//input[@id='DateTaken']")).SendKeys(currentDateTime.Day.ToString() + currentDateTime.Month.ToString() + Keys.ArrowRight +currentDateTime.Year.ToString() + Keys.ArrowRight + currentDateTime.Hour.ToString() + currentDateTime.Minute.ToString());
            string seconds = driver.FindElement(By.XPath("//input[@id='DateTaken']")).GetAttribute("value").Substring(17, 2);            
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Save']")).Click();
            driver.Navigate().GoToUrl(url + "/GetBook");             
            string formattedDate = DateTime.ParseExact(currentDateTime.Day + "/" + currentDateTime.Month + "/" + currentDateTime.Year + " " + currentDateTime.Hour + ":" + currentDateTime.Minute + ":" + seconds, "dd/M/yyyy H:m:ss", CultureInfo.InvariantCulture).ToString(@"M\/d\/yyyy h:mm:ss tt");           

            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'"+ formattedDate + "')]")).Displayed);
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'"+ formattedDate + "')]/..//td[2]")).Text.Trim(), Is.EqualTo("Steven King"));
            Assert.That(driver.FindElement(By.XPath("//td[contains(text(),'"+ formattedDate + "')]/..//td[3]")).Text.Trim(), Is.EqualTo("user-250390853"));
        }

        [Test, Order(8)]
        public void deleteBookRequest()
        {
            DateTime currentDateTime = DateTime.Now;
            var usernameField = driver.FindElement(By.XPath("//input[@name='username']"));
            var passwordField = driver.FindElement(By.XPath("//input[@name='password']"));
            var loginButton = driver.FindElement(By.XPath("/html/body/form/div/a[1]/div"));
            usernameField.SendKeys(Keys.Control + "A" + Keys.Delete);
            usernameField.SendKeys("admin");
            passwordField.SendKeys("123456");
            loginButton.Click();
            driver.Navigate().GoToUrl(url + "/GetBook");

            string currentDate = (currentDateTime.Month + "/" + currentDateTime.Day + "/" + currentDateTime.Year);
            Console.WriteLine(currentDate);
            driver.FindElement(By.XPath("//td[contains(text(),'"+ currentDate + "')]/../td[2][contains(text(),'Steven King')]/../td[3][contains(text(),'user-250390853')]/..//a[text()='Delete']")).Click();
            driver.FindElement(By.XPath("//input[@type='submit' and @value='Delete']")).Click();

            try
            {
                driver.FindElement(By.XPath("//td[contains(text(),'" + currentDate + "')]/../td[2][contains(text(),'Steven King')]/../td[3][contains(text(),'user-250390853')]"));
                Assert.Fail("Element exist");
            }
            catch (NoSuchElementException)
            {
                Assert.Pass("Element does not exist");
            }
        }
    }    
}