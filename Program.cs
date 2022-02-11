using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace zoom_bot
{
    class Program
    {
        static void StartBot(string url, List<string> nicks, int id)
        {
            try
            {
                var options = new FirefoxOptions();
                options.SetPreference("permissions.default.microphone", 1);
                options.SetPreference("permissions.default.camera", 1);

                var driver = new FirefoxDriver(@".\WebDriver\bin\", options);

                driver.Navigate().GoToUrl(url);
                driver.FindElement(By.ClassName("mbTuDeF1")).Click();
                driver.FindElement(By.CssSelector(".pUmU_FLW > h3:nth-child(2) > span:nth-child(1) > a:nth-child(1)")).Click();
                driver.FindElement(By.Id("inputname")).SendKeys(nicks[id]);
                driver.FindElement(By.Id("joinBtn")).Click();

                Console.WriteLine("Bot {0} connected!", id + 1);
            }
            catch (Exception e)
            {
                Console.WriteLine("Bot {0} FAILED!", id + 1);
                Console.WriteLine(e.Message);
            }            
        }
        static void Main(string[] args)
        {
            Console.Write("Enter meeting URL: ");
            string url = Console.ReadLine();

            Console.Write("Enter bot count: ");
            int count = Convert.ToInt32(Console.ReadLine());

            List<string> nicks = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Console.Write("Enter nickname number {0}: ", i + 1);
                nicks.Add(Console.ReadLine());
            }

            List<FirefoxDriver> drivers = new List<FirefoxDriver>();
            for (int i = 0; i < count; i++)
            {
                var t = new Thread(() => StartBot(url, nicks, i));
                t.Start();
                Thread.Sleep(500);
            }
        }
    }
}
