using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace zoom_bot
{
    class Program
    {
        static void StartBot(string url, List<string> nicks, int id, string message, int messagePeriod)
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

                if (message == "")
                {
                    Console.WriteLine("Bot {0} ({1}) connected!", id + 1, nicks[id]);
                    return;
                }

                Console.WriteLine("Bot {0} ({1}) connected! Starting message sending cycle...", id + 1, nicks[id]);
                
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1000);
                        driver.FindElement(By.CssSelector("div.footer-button__wrapper:nth-child(3) > button:nth-child(1)")).Click();
                        break;
                    }
                    catch (Exception) {}
                }

                var textarea = driver.FindElement(By.ClassName("chat-box__chat-textarea"));

                if (messagePeriod == 0)
                {
                    textarea.SendKeys(message);
                    textarea.SendKeys(Keys.Enter);
                    Console.WriteLine("Bot {0} ({1}) sent message, nicks[id]", id + 1);
                }
                else
                {
                    while (true)
                    {
                        textarea.SendKeys(message);
                        textarea.SendKeys(Keys.Enter);
                        Console.WriteLine("Bot {0} ({1}) sent message", id + 1, nicks[id]);
                        Thread.Sleep(messagePeriod);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Bot {0} DIED! RIP {1}...", id + 1, nicks[id]);
                Console.WriteLine("{{{0}}}", e.Message);
            }            
        }
        static void Main(string[] args)
        {
            Console.Write("Enter meeting URL: ");
            string url = Console.ReadLine();

            Console.Write("Enter bot count: ");
            int count = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter what bots should type in chat (leave empty to disable this): ");
            string message = Console.ReadLine();

            int messagePeriod;
            if (message != "")
            {
                Console.Write("Enter message period in ms (leave empty for single time): ");
                var temp = Console.ReadLine();
                if (temp != "")
                {
                    messagePeriod = Convert.ToInt32(temp);
                }
                else
                {
                    messagePeriod = 0;
                }
            }
            else
            {
                messagePeriod = -1; 
            }

            Console.Write("Enter bots creating period in ms (leave empty for default): ");
            int creatingPeriod;
            var temp2 = Console.ReadLine();
            if (temp2 != "")
            {
                creatingPeriod = Convert.ToInt32(temp2);
            }
            else
            {
                creatingPeriod = 500;
            }

            List<string> nicks = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Console.Write("Enter nickname number {0}: ", i + 1);
                nicks.Add(Console.ReadLine());
            }

            List<FirefoxDriver> drivers = new List<FirefoxDriver>();
            for (int i = 0; i < count; i++)
            {
                var t = new Thread(() => StartBot(url, nicks, i, message, messagePeriod));
                t.Start();
                Thread.Sleep(creatingPeriod);
            }
        }
    }
}
