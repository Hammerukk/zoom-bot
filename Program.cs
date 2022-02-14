using System;
using System.Collections.Generic;
using System.IO;
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

                var driver = new FirefoxDriver(@"./WebDriver/bin/", options);

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

        static void StartBotThreads(int count, string url, List<string> nicks, string message, int messagePeriod, int creatingPeriod)
        {
            var threads = new List<Thread>();
            for (int i = 0; i < count; i++)
            {
                threads.Add(new Thread(() => StartBot(url, nicks, i, message, messagePeriod)));
                threads[i].Start();
                Thread.Sleep(creatingPeriod);
            }
        }

        static void Main(string[] args)
        {
            string url, message;
            int count, messagePeriod, creatingPeriod;
            List<string> nicks = new List<string>();

            const string path = "params.txt";

            if (File.Exists(path))
            {
                Console.Write("Parameters from previous session detected.\nDo you want to load it? (n for no, otherwise yes): ");
                string confirm = Console.ReadLine();

                if (confirm.ToLower() != "n")
                {
                    StreamReader sr = new StreamReader(path);

                    count = Convert.ToInt32(sr.ReadLine());
                    url = sr.ReadLine();
                    for (int i = 0; i < count; i++)
                    {
                        nicks.Add(sr.ReadLine());
                    }
                    message = sr.ReadLine();
                    messagePeriod = Convert.ToInt32(sr.ReadLine());
                    creatingPeriod = Convert.ToInt32(sr.ReadLine());

                    sr.Close();

                    StartBotThreads(count, url, nicks, message, messagePeriod, creatingPeriod);
                    return;
                }
            }

            Console.Write("Enter meeting URL: ");
            url = Console.ReadLine();

            Console.Write("Enter bot count: ");
            count = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter what bots should type in chat (leave empty to disable this): ");
            message = Console.ReadLine();

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
            var temp2 = Console.ReadLine();
            if (temp2 != "")
            {
                creatingPeriod = Convert.ToInt32(temp2);
            }
            else
            {
                creatingPeriod = 500;
            }

            nicks = new List<string>();
            for (int i = 0; i < count; i++)
            {
                Console.Write("Enter nickname number {0}: ", i + 1);
                nicks.Add(Console.ReadLine());
            }
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            StreamWriter sw = new StreamWriter(path);

            sw.WriteLine(count);
            sw.WriteLine(url);
            for (int i = 0; i < count; i++)
            {
                sw.WriteLine(nicks[i]);
            }
            sw.WriteLine(message);
            sw.WriteLine(messagePeriod);
            sw.WriteLine(creatingPeriod);

            sw.Close();

            StartBotThreads(count, url, nicks, message, messagePeriod, creatingPeriod);
        }
    }
}
