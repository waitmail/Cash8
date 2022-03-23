using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Windows.Forms;
namespace Cash8
{
  

    public class FiscallPrintJason
    {

        public class Operator
        {
            public string name { get; set; }
            public string vatin { get; set; }
        }

        public static string guid = "";
       


        public static IWebProxy CreateWebProxyWithCredentials(String sUrl, string ProxyUserName, string ProxyUserPassword, string sAuthType, string ProxyUserDomain)
        {
            if (String.IsNullOrEmpty(ProxyUserName) || String.IsNullOrEmpty(ProxyUserPassword))
            {
                return null;
            }
            // get default proxy and assign it to the WebService. Alternatively, you can replace this with manual WebProxy creation.
            IWebProxy iDefaultWebProxy = WebRequest.DefaultWebProxy;
            Uri uriProxy = iDefaultWebProxy.GetProxy(new Uri(sUrl));
            string sProxyUrl = uriProxy.AbsoluteUri;
            if (sProxyUrl == sUrl)
            {//no proxy specified
                return null;
            }
            IWebProxy proxyObject = new WebProxy(sProxyUrl, true);
            // assign the credentials to the Proxy
            //todo do we need to add credentials to  WebService too??
            if ((!String.IsNullOrEmpty(sAuthType)) && (sAuthType.ToLower() != "basic"))
            {
                //from http://www.mcse.ms/archive105-2004-10-1165271.html
                // create credentials cache - it will hold both, the WebProxy credentials (??and the WebService credentials too??)
                System.Net.CredentialCache cache = new System.Net.CredentialCache();
                // add default credentials for Proxy (notice the authType = 'Kerberos' !) Other types are 'Basic', 'Digest', 'Negotiate', 'NTLM'
                cache.Add(new Uri(sProxyUrl), sAuthType, new System.Net.NetworkCredential(ProxyUserName, ProxyUserPassword, ProxyUserDomain));
                proxyObject.Credentials = cache;
            }
            else//special case for Basic (from http://www.xmlwebservices.cc/index_FAQ.htm )
            {
                proxyObject.Credentials = new System.Net.NetworkCredential(ProxyUserName, ProxyUserPassword);
            }
            return proxyObject;
        }

        //Внесение средств
        public static string POST(string Url, string Data)
        {
            string Out = String.Empty;
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(Url);                
                req.Method = "POST";
                req.Timeout = 5000;
                req.ContentType = "application/json";
                byte[] sentData = Encoding.UTF8.GetBytes(Data);
                req.ContentLength = sentData.Length;
                System.IO.Stream sendStream = req.GetRequestStream();
                sendStream.Write(sentData, 0, sentData.Length);
                sendStream.Close();
                //System.Net.WebResponse res = req.GetResponse();
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

                if (myHttpWebResponse.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    Out = "Created";
                }
                else
                {
                    Out = "Error";
                    MessageBox.Show("Ошибка Post запрос, статус " + myHttpWebResponse.StatusCode);
                }
                req = null;
                sendStream = null;                
                myHttpWebResponse.Close();// = null;
            }
            catch (WebException ex)
            {
                Out = ex.Message;
                MessageBox.Show("Ошибка2 Post запрос, статус " + Out);
            }

            return Out;
        }

        private static RootObject GET(string Url, string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "/" + Data);
            req.Timeout = 10000;                        
            System.Net.WebResponse resp = req.GetResponse();
            //HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

            System.IO.Stream stream = resp.GetResponseStream();

            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            
            //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Out);
            //var obj = JsonConvert.DeserializeObject(Out) as System.Collections.Generic.ICollection<Results>; 
                        
            var results = JsonConvert.DeserializeObject<RootObject>(Out);
            //Thread.Sleep(1000);
            req = null;
            resp.Close();
            stream.Close();            
            sr = null;

            return results;
            //return Out;
        }


        //private static RootObject GET(string Url, string Data)
        //{
        //    System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "/" + Data);        
        //    System.Net.WebResponse resp = req.GetResponse();
        //    //HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

        //    System.IO.Stream stream = resp.GetResponseStream();

        //    System.IO.StreamReader sr = new System.IO.StreamReader(stream);
        //    string Out = sr.ReadToEnd();
        //    sr.Close();
        //    //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Out);
        //    //var obj = JsonConvert.DeserializeObject(Out) as System.Collections.Generic.ICollection<Results>; 

        //    var results = JsonConvert.DeserializeObject<Che>(Out);

        //    return results;
        //    //return Out;
        //}




        #region GetResults

        public class Fn
        {
            public int code { get; set; }
            public string description { get; set; }
        }

        public class Ofd
        {
            public int code { get; set; }
            public string description { get; set; }
        }

        public class Network
        {
            public int code { get; set; }
            public string description { get; set; }
        }

        public class Errors
        {
            public int fnCommandCode { get; set; }
            public int documentNumber { get; set; }
            public Fn fn { get; set; }
            public Ofd ofd { get; set; }
            public Network network { get; set; }
        }

        public class Status
        {
            public int notSentFirstDocNumber { get; set; }
            public int notSentCount { get; set; }
            public DateTime notSentFirstDocDateTime { get; set; }
        }


        public class Counters
        {
            public double cashSum { get; set; }
        }

        public class DeviceInfo
        {
            public string modelName { get; set; }
            public string configurationVersion { get; set; }
            public string fnFfdVersion { get; set; }
            public string serial { get; set; }
            public int receiptLineLength { get; set; }
            public int model { get; set; }
            public int receiptLineLengthPix { get; set; }
            public string firmwareVersion { get; set; }
            public string ffdVersion { get; set; }
        }

        public class Result2
        {
            public Counters counters { get; set; }
            public Errors errors { get; set; }
            public Status status { get; set; }
            public ShiftStatus shiftStatus { get; set; }
            public DeviceInfo deviceInfo { get; set; }
        }

        public class Result
        {
            public Result2 result { get; set; }
            public string errorDescription { get; set; }
            public int errorCode { get; set; }
            public string status { get; set; }
//            public ShiftStatus shiftStatus { get; set; }            
        }

        public class ShiftStatus
        {
            public DateTime expiredTime { get; set; }
            public int number { get; set; }
            public string state { get; set; }
        }

        public class RootObject
        {
            public List<Result> results { get; set; }
        }

        #endregion


        /// <summary>
        /// Служебное внесение, выдача(Инкассация)
        /// </summary>
        /// <returns></returns>
        public static RootObject cashe_in_out(string type,double cashSum)
        {
            string status = "";
            RootObject result = null;
            CachInOut cash_in_out = new CachInOut();
            cash_in_out.@operator = new Cash8.FiscallPrintJason.Operator();
            cash_in_out.@operator.name = MainStaticClass.Cash_Operator;// "name";//необходимо переопределить
            cash_in_out.@operator.vatin = MainStaticClass.cash_operator_inn;// "123654789507";//необходимо переопределить
            cash_in_out.type = type;
            cash_in_out.cashSum = cashSum;
            string cash_in = JsonConvert.SerializeObject(cash_in_out, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = MainStaticClass.shablon.Replace("body", cash_in);
            guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(MainStaticClass.url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    result = GET(MainStaticClass.url, guid);
                    status = result.results[0].status;
                    if (status != "ready")
                    {
                        if (count > 14)
                        {
                            break;
                        }
                    }
                    else if ((status == "ready") || (status == "error"))
                    {
                        break;
                    }
                }
            }           
            
            return result;
        }
        
        public static string delete_last_job()
        {
            string Out = String.Empty;

            System.Net.WebRequest req = System.Net.WebRequest.Create(MainStaticClass.url + "/" + guid);            
            req.Method = "DELETE";
            req.Timeout = 100000;
            req.ContentType = "application/json";

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

            if (myHttpWebResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Out = "OK";
            }
            else
            {
                Out = "Error";
            }

            req = null;
            return Out;
        }


        public class Root
        {
            public string type { get; set; }
        }



        //public static RootObject execute_operator_type2(string type)
        //{
        //    string status = "";
        //    RootObject result = null;
        //    //Operator_type cds = new Operator_type();
        //    //cds.@operator = new Operator();
        //    //cds.@operator.name = MainStaticClass.Cash_Operator;//"name";//необходимо переопределить
        //    //cds.@operator.vatin = MainStaticClass.cash_operator_inn;// "123654789507";//необходимо переопределить
        //    //cds.type = type;
        //    Root root = new Root();
        //    root.type = type;
        //    string c_d_s = JsonConvert.SerializeObject(root, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    string json = MainStaticClass.shablon.Replace("body", c_d_s);
        //    guid = Guid.NewGuid().ToString();
        //    string replace = "\"uuid\": \"" + guid + "\"";
        //    json = json.Replace("uuid", replace);
        //    if (POST(MainStaticClass.url, json) == "Created")
        //    {
        //        int count = 0;
        //        while (1 == 1)
        //        {
        //            count++;
        //            Thread.Sleep(1000);
        //            result = GET(MainStaticClass.url, guid);
        //            status = result.results[0].status;
        //            if (status != "ready")
        //            {
        //                if (count > 14)
        //                {
        //                    break;
        //                }
        //            }
        //            else if ((status == "ready") || (status == "error"))
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    //cds = null;

        //    return result;
        //}

        public static RootObject execute_operator_type(string type)
        {
            string status = "";
            RootObject result = null;
            Operator_type cds = new Operator_type();
            cds.@operator = new Operator();
            cds.@operator.name = MainStaticClass.Cash_Operator;//"name";//необходимо переопределить
            cds.@operator.vatin = MainStaticClass.cash_operator_inn;// "123654789507";//необходимо переопределить
            cds.type = type;
            string c_d_s = JsonConvert.SerializeObject(cds, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = MainStaticClass.shablon.Replace("body", c_d_s);
            guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(MainStaticClass.url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    result = GET(MainStaticClass.url, guid);
                    status = result.results[0].status;
                    if (status != "ready")
                    {
                        if (count > 14)
                        {
                            break;
                        }
                    }
                    else if ((status == "ready") || (status == "error"))
                    {
                        break;
                    }
                }
            }

            cds = null;

            return result;
        }
       
        public class CachInOut
        {            
            public string type { get; set; }
            public double cashSum { get; set; }
            public FiscallPrintJason.Operator @operator { get; set; }
        }

        public class Operator_type
        {
            public string type { get; set; }
            public FiscallPrintJason.Operator @operator { get; set; }
        }

        #region PrintDocument

        public class PostItem
        {
            public string type { get; set; }
            public string text { get; set; }
            public string alignment { get; set; }
            public string barcode { get; set; }
            public string barcodeType { get; set; }
        }
       
        public class Tax
        {
            public string type { get; set; }
        }

        public class PayingAgent
        {
            public string operation { get; set; }
            public List<string> phones { get; set; }
        }

        public class ReceivePaymentsOperator
        {
            public List<string> phones { get; set; }
        }

        public class MoneyTransferOperator
        {
            public List<string> phones { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public string vatin { get; set; }
        }

        public class AgentInfo
        {
            public List<string> agents { get; set; }
            public PayingAgent payingAgent { get; set; }
            public ReceivePaymentsOperator receivePaymentsOperator { get; set; }
            public MoneyTransferOperator moneyTransferOperator { get; set; }
        }

        public class SupplierInfo
        {
            public List<string> phones { get; set; }
            public string name { get; set; }
            public string vatin { get; set; }
        }

        public class Item
        {
            public string type { get; set; }
            public string name { get; set; }
            public double price { get; set; }
            public double quantity { get; set; }
            public double amount { get; set; }
            public NomenclatureCode nomenclatureCode { get; set; }
            //public string nomenclatureCode { get; set; }
            //public double infoDiscountAmount { get; set; }
            public int department { get; set; }
            public string measurementUnit { get; set; }
            public string paymentMethod { get; set; }
            public string paymentObject { get; set; }            
            public Tax tax { get; set; }
            public AgentInfo agentInfo { get; set; }
            public SupplierInfo supplierInfo { get; set; }
            public string text { get; set; }
            public string alignment { get; set; }
            public int? font { get; set; }
            public bool? doubleWidth { get; set; }
            public bool? doubleHeight { get; set; }
            public string additionalAttribute { get; set; }
            public string barcode { get; set; }
            public string barcodeType { get; set; }
            public int? scale { get; set; }
            public MarkingCode markingCode { get; set; }
        }
        /// <summary>
        /// Код маркировки 
        /// </summary>
        public class MarkingCode
        {
            //public string type { get; set; }
            public string mark { get; set; }
        }


        public class Payment
        {
            public string type { get; set; }
            public double sum { get; set; }
        }

        public class ClientInfo
        {
            public string emailOrPhone { get; set; }
            public string vatin { get; set; }
            public string name { get; set; }
        }

        public class NomenclatureCode
        {
            public string type { get; set; }
            public string gtin { get; set; }
            public string serial { get; set; }
        }

        public class Check
        {
            public string type { get; set; }
            public string taxationType { get; set; }
            public bool ignoreNonFiscalPrintErrors { get; set; }
            public Operator @operator { get; set; }
            public List<Item> items { get; set; }
            public List<Payment> payments { get; set; }
            public double total { get; set; }
            //public string clientInfo { get; set; }
            public List<PostItem> postItems { get; set; }
            public ClientInfo clientInfo { get; set; }            
            public bool electronically { get; set; }
        }


        //public static RootObject check_last_check_print()
        //{
        //    string status = "";
        //    RootObject result = null;
        //    int count = 0; string last_guid = MainStaticClass.read_last_sell_guid();
        //    if (last_guid.Trim().Length!=36)
        //    {
        //        return result;
        //    }
        //    while (1 == 1)
        //    {
        //        count++;
        //        Thread.Sleep(1000);
        //        result = GET(MainStaticClass.url, last_guid);
        //        status = result.results[0].status;
        //        if (status != "ready")
        //        {
        //            if (count > 14)
        //            {
        //                break;
        //            }
        //        }
        //        else if ((status == "ready") || (status == "error"))
        //        {
        //            break;
        //        }
        //    }
        //    return result;
        //}
        
        public static RootObject check_print(string type, Check check,string num_doc)
        {
            string status = "";
            RootObject result = null;
            //Check check = new Check();
            check.@operator = new Cash8.FiscallPrintJason.Operator();
            check.@operator.name = MainStaticClass.Cash_Operator;// "name";//необходимо переопределить
            check.@operator.vatin = MainStaticClass.cash_operator_inn;// "123654789507";//необходимо переопределить
            check.type = type;
            string _check_ = JsonConvert.SerializeObject(check, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = MainStaticClass.shablon.Replace("body", _check_);
            guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            //MainStaticClass.write_last_sell_guid(guid,num_doc);
            if (POST(MainStaticClass.url, json) == "Created")
            {
                //Здесь отметить задание, что оно принято к исполнению
                MainStaticClass.write_document_wil_be_printed(num_doc);
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    result = GET(MainStaticClass.url, guid);
                    status = result.results[0].status;
                    if (status != "ready")
                    {
                        if (count > 14)
                        {
                            break;
                        }
                    }
                    else if ((status == "ready") || (status == "error"))
                    {
                        break;
                    }
                }
            }

            return result;

        }


        //private AnswerAddingKmArrayToTableTested GET(string Url, string Data)
        //{
        //    System.Net.WebRequest req = System.Net.WebRequest.Create(Url + "/" + Data);
        //    req.Timeout = 10000;
        //    System.Net.WebResponse resp = req.GetResponse();
        //    //HttpWebResponse myHttpWebResponse = (HttpWebResponse)req.GetResponse();

        //    System.IO.Stream stream = resp.GetResponseStream();

        //    System.IO.StreamReader sr = new System.IO.StreamReader(stream);
        //    string Out = sr.ReadToEnd();
        //    sr.Close();

        //    //Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(Out);
        //    //var obj = JsonConvert.DeserializeObject(Out) as System.Collections.Generic.ICollection<Results>; 

        //    var results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);
        //    //Thread.Sleep(1000);
        //    req = null;
        //    resp.Close();
        //    stream.Close();
        //    sr = null;

        //    return results;
        //    //return Out;
        //}

        //private static AnswerAddingKmArrayToTableTested Tested_Km(AddingKmArrayToTableTested addingKmArrayToTableTestedKm)
        //{
        //    string status = "";            
        //    AnswerAddingKmArrayToTableTested result = null;
        //    //CachInOut cash_in_out = new CachInOut();
        //    //cash_in_out.@operator = new Cash8.FiscallPrintJason.Operator();
        //    //cash_in_out.@operator.name = MainStaticClass.Cash_Operator;// "name";//необходимо переопределить
        //    //cash_in_out.@operator.vatin = MainStaticClass.cash_operator_inn;// "123654789507";//необходимо переопределить
        //    //cash_in_out.type = type;
        //    //cash_in_out.cashSum = cashSum;
        //    string km_array_to_table_tested = JsonConvert.SerializeObject(addingKmArrayToTableTestedKm, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    string json = MainStaticClass.shablon.Replace("body", km_array_to_table_tested);
        //    guid = Guid.NewGuid().ToString();
        //    string replace = "\"uuid\": \"" + guid + "\"";
        //    json = json.Replace("uuid", replace);
        //    if (POST(MainStaticClass.url, json) == "Created")
        //    {
        //        int count = 0;
        //        while (1 == 1)
        //        {
        //            count++;
        //            Thread.Sleep(1000);
        //            result = GET(MainStaticClass.url, guid);
        //            status = result.results[0].status;
        //            if (status != "ready")
        //            {
        //                if (count > 14)
        //                {
        //                    break;
        //                }
        //            }
        //            else if ((status == "ready") || (status == "error"))
        //            {
        //                break;
        //            }
        //        }
        //    }

        //    return result;
        //}



        #endregion

    }
}
