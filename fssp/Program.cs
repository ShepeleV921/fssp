using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Office.Interop.Outlook;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NLog;
using static fssp.Program;
using Exception = System.Exception;

namespace fssp
{
    internal class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static List<MailMessages> MailMessages = new List<MailMessages>();
        private static List<List<string>> VerifiedMailMessages = new List<List<string>>();
        public static List<Individual> Individual = new List<Individual>();

        static void Main(string[] args)
        {
            _logger.Info(@"Запуск проекта. ");
            Microsoft.Office.Interop.Outlook.Application app = null;
            Microsoft.Office.Interop.Outlook._NameSpace ns = null;
            Microsoft.Office.Interop.Outlook.PostItem item = null;
            Microsoft.Office.Interop.Outlook.MAPIFolder inboxFolder = null;
            Microsoft.Office.Interop.Outlook.MAPIFolder subFolder = null;
            try
            {
                app = new Microsoft.Office.Interop.Outlook.Application();
                ns = app.GetNamespace("MAPI");
                ns.Logon(null, null, false, false);

                inboxFolder = ns.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderInbox);
                subFolder = inboxFolder.Folders["ФССП"];

                for (int i = 1; i <= subFolder.Items.Count; i++)
                {
                    MailMessages resalt = new MailMessages();
                    dynamic items = subFolder.Items[i];
                    resalt.header = items.Subject;
                    resalt.date = items.SentOn.ToLongDateString();
                    resalt.time = DateTime.Parse(items.SentOn.ToLongTimeString());
                    resalt.fio = items.SenderName;
                    resalt.text = items.Body;
                    MailMessages.Add(resalt);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                _logger.Info(@"Список с сообщениями из почты не загрузился. Неизвестная ошибка" + ex);
            }
            finally
            {
                ns = null;
                app = null;
                inboxFolder = null;
            }


            #region Читаем все обработанные сообщения из файла json

            try
            {
                if (Directory.Exists("json"))
                {
                    string[] ListJson = Directory.GetFiles("json", "*.*", SearchOption.AllDirectories);
                    for (int j = 0; j < ListJson.Length; j++)
                    {
                        using (StreamReader r = new StreamReader(ListJson[j].ToString()))
                        {
                            string _json = r.ReadToEnd();
                            var _list = JsonConvert.DeserializeObject<List<Individual>>(_json);
                            if (_list != null)
                            {
                                for (int i = 0; i < _list.Count; i++)
                                {
                                    Individual.Add(_list[i]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.Info(@"Список json-ов не прочиталось. Неизвестная ошибка" + ex);
            }

            #endregion

            #region Сравниваем все сообщения с обработанными сообщениями (json) и убираем дубликата

            try
            {
                if (Individual.Count != 0)
                {
                    for (int i = 0; i < MailMessages.Count(); i++)
                    {
                        string urlText = MailMessages[i].text.ToString();
                        string pattern = @"https://lk.gosuslugi.ru/notifications/details/GEPS/\w*";
                        string patternId = @"(\d\d\d\d\d\d\d\d\d\d$)";
                        RegexOptions option = RegexOptions.Multiline;
                        var result = Regex.Matches(urlText, pattern, option);
                        var url = result[0].Value.Trim();
                        var resultId = Regex.Matches(url, patternId, option);
                        var urlId = resultId[0].Value.Trim();
                        for (int j = 0; j < Individual.Count; j++)

                        {
                            if (urlId == Individual[j].urlId)
                            {
                                MailMessages.RemoveAt(i);
                                i--;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.Info(@"Список json-ов не прочиталось. Неизвестная ошибка" + ex);
            }

            #endregion

            int a = 0, b = 0, c = 0, d = 0, f = 0, g = 0, h = 0, l = 0, k = 0, m = 0;
            for (int i = 0; i < Individual.Count(); i++)
            {
                if (Individual[i].DocName == "Постановление об отмене постановления об обращении взыскания на ДС")
                {
                    a++;
                }

                if (Individual[i].DocName == "Постановление о снятии запрета на совершение действий по регистрации")
                {
                    b++;
                }

                if (Individual[i].DocName == "Постановление об окончании исполнительного производства")
                {
                    c++;
                }

                if (Individual[i].DocName == "Постановление о временном ограничении на выезд должника из Российской Федерации")
                {
                    d++;
                }

                if (Individual[i].DocName == "Постановление об объединении ИП в сводное по должнику")
                {
                    f++;
                }

                if (Individual[i].DocName == "Постановление об отмене мер по обращению взыскания на доходы должника")
                {
                    g++;
                }

                if (Individual[i].DocName == "Постановление о запрете на совершение действий по регистрации")
                {
                    h++;
                }

                if (Individual[i].DocName == "Постановление об окончании и возвращении ИД взыскателю")
                {
                    l++;
                }

                if (Individual[i].DocName == "Постановление об обращении взыскания на заработную плату и иные доходы должника (об обращении взыскания на заработную плату)")
                {
                    k++;
                }

                if (Individual[i].DocName == "Постановление о запрете на регистрационные действия в отношении транспортных средств")
                {
                    m++;
                }
            }

            var itogo = a + b + c + d + f + g + h + l + k + m;
            var desiredlist = c + l;
            for (int i = 0; i < MailMessages.Count; i++)
            {
                FsspWorker.Start(MailMessages[i]);
            }
        }
    }
}