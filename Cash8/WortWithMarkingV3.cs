using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;
using System.Threading;

namespace Cash8
{
    public class WortWithMarkingV3
    {
        public static string shablon = MainStaticClass.shablon;
        public static string url = MainStaticClass.url;

        public class ClearMarkingCodeValidationResult
        {
            public string type { get; set; }
            public ClearMarkingCodeValidationResult()
            {
                type = "clearMarkingCodeValidationResult";
            }
        }

        public class GetMarkingCodeValidationStatus
        {
            public string type { get; set; }
            public GetMarkingCodeValidationStatus()
            {
                type = "getMarkingCodeValidationStatus";
            }
        }

        public class AcceptMarkingCode
        {
            public string type { get; set; }
            public AcceptMarkingCode()
            {
                type = "acceptMarkingCode";
            }
        }

        public class DeclineMarkingCode
        {
            public string type { get; set; }
            public DeclineMarkingCode()
            {
                type = "declineMarkingCode";
            }
        }



        public class ItemInfoCheckResult
        {
            public bool ecrStandAloneFlag { get; set; }
            public bool imcCheckFlag { get; set; }
            public bool imcCheckResult { get; set; }
            public bool imcEstimatedStatusCorrect { get; set; }
            public bool imcStatusInfo { get; set; }
        }



        public class MarkOperatorResponse
        {
            public bool responseStatus { get; set; }
            public bool itemStatusCheck { get; set; }
        }

        public class OnlineValidation
        {
            public ItemInfoCheckResult itemInfoCheckResult { get; set; }
            public string markOperatorItemStatus { get; set; }
            public MarkOperatorResponse markOperatorResponse { get; set; }
            public string markOperatorResponseResult { get; set; }
            public string imcType { get; set; }
            public string imcBarcode { get; set; }
            public int imcModeProcessing { get; set; }
        }

        public class CancelMarkingCodeValidation
        {
            public string type { get; set; }
            public CancelMarkingCodeValidation()
            {
                type = "cancelMarkingCodeValidation";
            }
        }

        public class OfflineValidation
        {
            public bool fmCheck { get; set; }
            public string fmCheckErrorReason { get; set; }
            public bool fmCheckResult { get; set; }
        }

        public class Result
        {
            public string status { get; set; }
            public Result2 result { get; set; }
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
        }

        public class DriverError
        {
            public int code { get; set; }
            public string description { get; set; }
            public string error { get; set; }
        }

        public class Result2
        {
            public OfflineValidation offlineValidation { get; set; }
            public OnlineValidation onlineValidation { get; set; }
            public DriverError driverError { get; set; }
            public bool ready { get; set; }
            public bool sentImcRequest { get; set; }
        }

        public class Root
        {
            public List<Result> results { get; set; }
            public bool ready { get; set; }
            public bool sentImcRequest { get; set; }
            public OnlineValidation onlineValidation { get; set; }
        }

        private static Root GetOfflineValidation(string Url, string Data)
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

            var results = JsonConvert.DeserializeObject<Root>(Out);

            //AnswerAddingKmArrayToTableTested results = JsonConvert.DeserializeObject<AnswerAddingKmArrayToTableTested>(Out);
            //Thread.Sleep(1000);
            req = null;
            resp.Close();
            stream.Close();
            sr = null;

            return results;
            //return Out;
        }

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



        public Root clearMarkingCodeValidationResult()
        {
            ClearMarkingCodeValidationResult clearMarkingCodeValidationResult = new ClearMarkingCodeValidationResult();
            ////////////////////////////////////////
            string status = "";
            Root result = null;
            string clearMarking = JsonConvert.SerializeObject(clearMarkingCodeValidationResult, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = shablon.Replace("body", clearMarking);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    result = GetOfflineValidation(url, guid);
                    status = result.results[0].status;
                    if ((status != "ready") && (status != "error"))
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




        public class BeginMarkingCodeValidation
        {
            public string type { get; set; }
            public Params @params { get; set; }
        }

        public class Params
        {
            public string imcType { get; set; }
            public string imc { get; set; }
            public string itemEstimatedStatus { get; set; }
            public int imcModeProcessing { get; set; }

        }

        /// <summary>
        /// Посылаем запрос beginMarkingCodeValidation и получает ответ 
        /// </summary>
        /// <param name="imcType"></param>
        /// <param name="imc"></param>
        /// <param name="itemEstimatedStatus"></param>
        /// <param name="itemQuantity"></param>
        /// <param name="itemUnits"></param>
        /// <param name="imcModeProcessing"></param>
        /// <param name="notSendToServer"></param>
        public Root beginMarkingCodeValidation(string imcType, string imc, string itemEstimatedStatus, double itemQuantity, string itemUnits, int imcModeProcessing, bool notSendToServer)
        {

            BeginMarkingCodeValidation beginMarkingCodeValidation = new BeginMarkingCodeValidation();
            beginMarkingCodeValidation.type = "beginMarkingCodeValidation";
            beginMarkingCodeValidation.@params = new Params();
            beginMarkingCodeValidation.@params.imcType = imcType;
            beginMarkingCodeValidation.@params.imc = imc;
            beginMarkingCodeValidation.@params.itemEstimatedStatus = itemEstimatedStatus;
            beginMarkingCodeValidation.@params.imcModeProcessing = imcModeProcessing;

            string status = "";
            Root root = null;
            string сheck_validation = JsonConvert.SerializeObject(beginMarkingCodeValidation, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = shablon.Replace("body", сheck_validation);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    root = GetOfflineValidation(url, guid);
                    status = root.results[0].status;
                    if ((status != "ready") && (status != "error"))
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
            return root;
        }

        public Root cancelMarkingCodeValidation()
        {
            string status = "";
            Root root = null;
            CancelMarkingCodeValidation cancelMarkingCodeValidation = new CancelMarkingCodeValidation();
            string сheck_validation = JsonConvert.SerializeObject(cancelMarkingCodeValidation, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = shablon.Replace("body", сheck_validation);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    root = GetOfflineValidation(url, guid);
                    status = root.results[0].status;
                    //status = "9";
                    if ((status != "ready") && (status != "error"))
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
            return root;
        }

        public Root acceptMarkingCode()
        {
            string status = "";
            Root root = null;
            AcceptMarkingCode acceptMarkingCode = new AcceptMarkingCode();
            string сheck_validation = JsonConvert.SerializeObject(acceptMarkingCode, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = shablon.Replace("body", сheck_validation);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    root = GetOfflineValidation(url, guid);
                    status = root.results[0].status;
                    //status = "9";
                    if ((status != "ready") && (status != "error"))
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
            return root;
        }

        public Root declineMarkingCode()
        {
            string status = "";
            Root root = null;
            DeclineMarkingCode declineMarkingCode = new DeclineMarkingCode();
            string сheck_validation = JsonConvert.SerializeObject(declineMarkingCode, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = shablon.Replace("body", сheck_validation);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    root = GetOfflineValidation(url, guid);
                    status = root.results[0].status;
                    //status = "9";
                    if ((status != "ready") && (status != "error"))
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
            return root;
        }

        public Root getMarkingCodeValidationStatus()
        {
            string status = "";
            Root root = null;
            GetMarkingCodeValidationStatus getMarkingCodeValidationStatus = new GetMarkingCodeValidationStatus();
            string сheck_validation = JsonConvert.SerializeObject(getMarkingCodeValidationStatus, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string json = shablon.Replace("body", сheck_validation);
            string guid = Guid.NewGuid().ToString();
            string replace = "\"uuid\": \"" + guid + "\"";
            json = json.Replace("uuid", replace);
            if (POST(url, json) == "Created")
            {
                int count = 0;
                while (1 == 1)
                {
                    count++;
                    Thread.Sleep(1000);
                    root = GetOfflineValidation(url, guid);
                    status = root.results[0].status;
                    //status = "9";
                    if ((status != "ready") && (status != "error"))
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
            return root;
        }

    }

}
