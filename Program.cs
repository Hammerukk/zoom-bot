using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace zoom_bot
{
    class Program
    {
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
                Console.WriteLine("Bot {0} connecting...", i + 1);

                try
                {
                    var options = new FirefoxOptions();
                    options.SetPreference("permissions.default.microphone", 1);
                    options.SetPreference("permissions.default.camera", 1);
                    drivers.Add(new FirefoxDriver(@".\WebDriver\bin\", options));

                    drivers[i].Navigate().GoToUrl(url);
                    drivers[i].FindElement(By.ClassName("mbTuDeF1")).Click();
                    drivers[i].FindElement(By.CssSelector(".pUmU_FLW > h3:nth-child(2) > span:nth-child(1) > a:nth-child(1)")).Click();
                    drivers[i].FindElement(By.Id("inputname")).SendKeys(nicks[i]);
                    drivers[i].FindElement(By.Id("joinBtn")).Click();

                    Console.WriteLine("Bot {0} connected!", i + 1);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Bot {0} FAILED!", i + 1);
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
