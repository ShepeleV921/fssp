using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using NLog;

namespace fssp
{
    public class JsonWorker
    {
        public List<Individual> Individual;
        public MailMessages MailMessages;
        public string urlId;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public JsonWorker(List<Individual> Individual, MailMessages MailMessages, string urlId)
        {
            this.Individual = Individual;
            this.MailMessages = MailMessages;
            this.urlId = urlId;
        }

        public void Recording()
        {
            try
            {
                if (!Directory.Exists("json"))
                    Directory.CreateDirectory("json");

                if (!File.Exists("json//" + MailMessages.date + ".json"))
                    File.Create("json//" + MailMessages.date + ".json").Close();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };
                Individual[0].urlId = urlId;
                using (StreamReader r = new StreamReader("json//" + MailMessages.date + ".json"))
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

                string json = System.Text.Json.JsonSerializer.Serialize(Individual, options);
                File.WriteAllText("json//" + MailMessages.date + ".json", json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.Info("Ошибка в сериализации Json. " + ex);
            }
        }
    }
}