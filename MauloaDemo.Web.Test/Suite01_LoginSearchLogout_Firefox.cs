using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture]
    public class Suite01_LoginSearchLogout_Firefox
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        
        [SetUp]
        public void SetupTest() {
            driver = new FirefoxDriver();
            baseURL = "http://bookingtest.projectmhawaii.com/";
            verificationErrors = new StringBuilder();
            driver.Url = baseURL;
        }
        
        [TearDown]
        public void TeardownTest()
        {
            try {
                driver.Quit();
            }
            catch (Exception){
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }
        
        [Test]
        public void Case001_LoginSearchLogout_FireFox(){
            driver.FindElement(By.Name("login_id")).SendKeys("pmtest");
            driver.FindElement(By.Name("password")).SendKeys("pm123");
            driver.FindElement(By.Name("submit")).Click();
            Thread.Sleep(2000);
            Assert.AreEqual("Calendar", driver.Title);

            driver.FindElement(By.LinkText("Search")).Click();
            Thread.Sleep(3000);
            Assert.AreEqual("Search", driver.Title);

            driver.FindElement(By.LinkText("Log Out")).Click();
            Thread.Sleep(1000);
            Assert.AreEqual("Please login", driver.Title);
        }


        private bool IsElementPresent(By by){
            try {
                driver.FindElement(by);
                return true;
            } catch (NoSuchElementException){
                return false;
            }
        }
        
        private bool IsAlertPresent() {
            try {
                driver.SwitchTo().Alert();
                return true;
            } catch (NoAlertPresentException) {
                return false;
            }
        }
        
        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                } else {
                    alert.Dismiss();
                }
                return alertText;
            } finally {
                acceptNextAlert = true;
            }
        }
    }
}
