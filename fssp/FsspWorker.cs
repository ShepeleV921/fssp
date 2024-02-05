using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog;
using OpenQA.Selenium.DevTools;

namespace fssp
{
    public class FsspWorker
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static bool Start(MailMessages VerifiedMailMessages)
        {
            try
            {
                string NameXml = null;
                string urlText = VerifiedMailMessages.text.ToString();
                string pattern = @"https://lk.gosuslugi.ru/notifications/details/GEPS/\w*";
                string patternId = @"(\d\d\d\d\d\d\d\d\d\d$)";
                RegexOptions option = RegexOptions.Multiline;
                var result = Regex.Matches(urlText, pattern, option);
                var url = result[0].Value.Trim();
                var resultId = Regex.Matches(url, patternId, option);
                var urlId = resultId[0].Value.Trim();
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("--disable-popup-blocking");
                options.AddArgument("--incognito");
                //options.AddArgument("no-sandbox");
                //options.AddArgument("headless");
                //Options.AddUserProfilePreference("download.default_directory", "C:\\Downloads");
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("download.directory_upgrade", true);
                options.AddUserProfilePreference("safebrowsing.enabled", true);
                using (var driver = new ChromeDriver(@"C:\VS project\fssp\fssp\bin\Debug\net6.0\chromedriver.exe", options, TimeSpan.FromMinutes(5)))
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(120));
                    try
                    {
                        driver.Navigate().GoToUrl(url);
                        wait.Until(d => d.FindElements(By.XPath(@"//*[@id='login']")).Count > 0);
                        Thread.Sleep(500);
                        driver.FindElement(By.XPath(@"//*[@id='login']")).SendKeys("+7(988)943-81-13");
                        Thread.Sleep(500);
                        driver.FindElement(By.XPath(@"//*[@id='password']")).SendKeys("Kpsa131084$");
                        Thread.Sleep(4500);
                        wait.Until(d => d.FindElements(By.XPath(@"/html/body/esia-root/div/esia-login/div/div[1]/form/div[4]/button")).Count > 0);
                        driver.FindElement(By.XPath(@"/html/body/esia-root/div/esia-login/div/div[1]/form/div[4]/button")).Click();
                        wait.Until(d => d.FindElements(By.XPath(@"/html/body/esia-root/div/esia-login/div/div/div/div[4]/button[2]")).Count > 0);
                        driver.FindElement(By.XPath(@"/html/body/esia-root/div/esia-login/div/div/div/div[4]/button[2]")).Click();
                        Thread.Sleep(3500);
                        wait.Until(d => d.FindElements(By.XPath(@"/html/body/app-root/main/div/div/div/div/button[2]/div/div/div[2]/div[1]")).Count > 0);
                        driver.FindElement(By.XPath(@"/html/body/app-root/main/div/div/div/div/button[2]/div/div/div[2]/div[1]")).Click();
                        driver.Navigate().GoToUrl(url);
                        //wait.Until(d => d.FindElements(By.XPath(@"//*[@id='mid-" + urlId + "']/div[6]/lk-files/div/div/div[3]/lib-file/div/div/div[1]/div/div[2]")).Count > 0);
                        Thread.Sleep(1000);
                        var ExaminationNameXml = driver.FindElements(By.XPath(@"//*[@id='mid-" + urlId + "']/div[6]/lk-files/div/div/div[3]/lib-file/div/div/div[1]/div/div[2]")).Count > 0;
                        if (ExaminationNameXml)
                        {
                            NameXml = driver.FindElement(By.XPath(@"//*[@id='mid-" + urlId + "']/div[6]/lk-files/div/div/div[3]/lib-file/div/div/div[1]/div/div[2]")).Text
                                .ToString();
                            if (File.Exists(@"C:\Users\j.shepelev\Downloads\" + NameXml + ".xml"))
                            {
                                File.Delete(@"C:\Users\j.shepelev\Downloads\" + NameXml + ".xml");
                                _logger.Info(@"Сообщение было ранее обработано." + urlId + ", " + NameXml);
                            }
                            else if (File.Exists("xml//" + NameXml + ".xml"))
                            {
                                File.Delete(@"C:\Users\j.shepelev\Downloads\" + NameXml + ".xml");
                                _logger.Info(@"Сообщение было ранее обработано." + urlId + ", " + NameXml);
                            }
                            else
                            {
                                bool downloadV1 = driver.FindElements(By.XPath(@"//*[@id='mid-" + urlId + "']/div[6]/lk-files/div/div/div[3]/lib-file/div/div/div[2]/div/lib-actions-menu/div/div/div[1]/div[1]/ul/li/a"))
                                    .Count > 0;
                                bool downloadV2 = driver.FindElements(By.XPath(@"//*[@id='mid-" + urlId + "']/div[6]/lk-files/div/div/div[3]/lib-file/div/div/div[1]/div/div[2]"))
                                    .Count > 0;
                                if (downloadV1)
                                {
                                    driver.FindElement(By.XPath(@"//*[@id='mid-" + urlId + "']/div[6]/lk-files/div/div/div[3]/lib-file/div/div/div[2]/div/lib-actions-menu/div/div/div[1]/div[1]/ul/li/a")).Click();
                                    Thread.Sleep(6500);
                                    _logger.Info(@"Был успешно скачан " + urlId + ".xml");
                                    XmlWorker worker = new XmlWorker(VerifiedMailMessages, NameXml, urlId);
                                    worker.Start();
                                }
                                else if (downloadV2)
                                {
                                    driver.FindElement(By.XPath(@"//*[@id='mid-" + urlId + "']/div[6]/lk-files/div/div/div[3]/lib-file/div/div/div[1]/div/div[2]")).Click();
                                    
                                    Thread.Sleep(6500);
                                    _logger.Info(@"Был успешно скачан " + urlId + ".xml");
                                    XmlWorker worker = new XmlWorker(VerifiedMailMessages, NameXml, urlId);
                                    worker.Start();
                                }
                                driver.Quit();
                            }
                        }
                        else 
                        {
                            _logger.Info(@"Неизвестная ошибка, невозможно скачать и прочитать xml файл ");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Info(@"Неизвестная ошибка с парсингом сайта. " + ex);
                        Console.WriteLine(ex.Message);
                        driver.Quit();
                        return Start(VerifiedMailMessages);
                    }
                    // finally
                    // {
                    //     driver.Quit();
                    // }
                }
            }
            catch (Exception ex)
            {
                _logger.Info(@"Неизвестная ошибка. " + ex);
                Console.WriteLine(ex.Message);
                return Start(VerifiedMailMessages);
            }

            return true;
        }
    }
}