using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using OpenQA.Selenium.DevTools;
using Org.BouncyCastle.Crypto.Signers;
using NLog;

namespace fssp;

public class XmlWorker
{
    public static XmlNamespaceManager _nsManager;
    public static List<Individual> Individual { get; set; }
    public MailMessages MailMessages;
    public string NameXml;
    public string urlId;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public XmlWorker(MailMessages MailMessages, string NameXml, string urlId)
    {
        this.MailMessages = MailMessages;
        this.NameXml = NameXml;
        this.urlId = urlId;
    }

    public void Start()
    {
        try
        {
            _logger.Info(@"Загрузка данных в базу данных");
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\j.shepelev\Downloads\" + NameXml + ".xml");

            _nsManager = new XmlNamespaceManager(doc.NameTable);
            _nsManager.AddNamespace("x", doc.DocumentElement.GetNamespaceOfPrefix("fssp"));

            Individual = new List<Individual>();
            Individual.Add(new Individual());

            XmlElement? xRoot = doc.DocumentElement;
            if (xRoot != null)
            {
                XmlNode? DocNameNum = xRoot.SelectSingleNode("x:DocName", _nsManager);
                if (DocNameNum != null)
                    Individual[0].DocName = DocNameNum.InnerText;
                        
                XmlNode? DbtrNameNum = xRoot.SelectSingleNode("x:DbtrName", _nsManager);
                if (DbtrNameNum != null)
                    Individual[0].DbtrName = DbtrNameNum.InnerText;

                XmlNode? IdDocNoNum = xRoot.SelectSingleNode("x:IdDocNo", _nsManager);
                if (IdDocNoNum != null)
                    Individual[0].IdDocNo = IdDocNoNum.InnerText;

                XmlNode? IpNoNum = xRoot.SelectSingleNode("x:IpNo", _nsManager);
                if (IpNoNum != null)
                    Individual[0].IpNo = IpNoNum.InnerText;

                XmlNode? IdCourtNameNum = xRoot.SelectSingleNode("x:IdCourtName", _nsManager);
                if (IdCourtNameNum != null)
                    Individual[0].IdCourtName = IdCourtNameNum.InnerText;

                XmlNode? DbtrAdrNum = xRoot.SelectSingleNode("x:DbtrAdr", _nsManager);
                if (DbtrAdrNum != null)
                    Individual[0].DbtrAdr = DbtrAdrNum.InnerText;

                XmlNode? IdDbtrBornNum = xRoot.SelectSingleNode("x:idDbtrBorn", _nsManager);
                if (IdDbtrBornNum != null)
                    Individual[0].IdDbtrBorn = Convert.ToDateTime(IdDbtrBornNum.InnerText);

                XmlNode? DbtrInnNum = xRoot.SelectSingleNode("x:DbtrInn", _nsManager);
                if (DbtrInnNum != null)
                    Individual[0].DbtrInn = DbtrInnNum.InnerText;

                XmlNode? IdDbtrSNILSNum = xRoot.SelectSingleNode("x:IdDbtrSNILS", _nsManager);
                if (IdDbtrSNILSNum != null)
                    Individual[0].IdDbtrSNILS = IdDbtrSNILSNum.InnerText;

                XmlNode? IdDebtSumNum = xRoot.SelectSingleNode("x:IdDebtSum", _nsManager);
                if (IdDebtSumNum != null)
                    Individual[0].IdDebtSum = IdDebtSumNum.InnerText;

                XmlNode? TotalArrestDebtSumNum = xRoot.SelectSingleNode("x:TotalArrestDebtSum", _nsManager);
                if (TotalArrestDebtSumNum != null)
                    Individual[0].TotalArrestDebtSum = TotalArrestDebtSumNum.InnerText;

                XmlNode? IsSvodNum = xRoot.SelectSingleNode("x:IsSvod", _nsManager);
                if (IsSvodNum != null)
                    Individual[0].IsSvod = Convert.ToBoolean(IsSvodNum.InnerText);

                XmlNode? DebtorTypeNum = xRoot.SelectSingleNode("x:DebtorType", _nsManager);
                if (DebtorTypeNum != null)
                    Individual[0].DebtorType = DebtorTypeNum.InnerText;

                XmlNode? DocTypeNum = xRoot.SelectSingleNode("x:DocType", _nsManager);
                if (DocTypeNum != null)
                    Individual[0].DocType = DocTypeNum.InnerText;

                JsonWorker worker = new JsonWorker(Individual, MailMessages, urlId);
                worker.Recording();
            }

            if (!Directory.Exists("xml"))
                Directory.CreateDirectory("xml");

            FileInfo XmlFile = new FileInfo(@"C:\Users\j.shepelev\Downloads\" + NameXml + ".xml");
            if (XmlFile.Exists)
            {
                XmlFile.MoveTo(@"xml\" + NameXml + ".xml");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            _logger.Info(@"Произошла ошибка с чтением xml файла. " + ex);
        }
    }
}