using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Text;
using System.Linq;



namespace Cash8
{
    class PIOT
    {

        //public bool cdn_check_marker_code(List<string> codes, string mark_str, ref HttpWebRequest request, string mark_str_cdn, Dictionary<string, string> d_tovar, Cash_check cash_Check, ProductData productData)
        public bool cdn_check_marker_code(List<string> codes, string mark_str, Int64 numdoc, ref HttpWebRequest request, string mark_str_cdn, Dictionary<string, string> d_tovar, Cash_check cash_Check, ProductData productData)
        {
            bool result_check = false;

            StringBuilder sb = new StringBuilder();

            //string code = "MDEwNDYyOTMwODg3NzA0NDIxRHprY1l0Mh04MDA1MTc3MDAwHTkzZEdWeg==";
            //string url = "https://esm-emu.ao-esp.ru/api/v1/codes/check";//онлайн 
            string url = "https://esm-emu.ao-esp.ru/api/v1/codes/checkoffline";//оффлайн 
            ApiResponse apiResponse = null;
            string marking_code = mark_str.Replace("\\u001d", @"u001d");

            // Заполняем информацию о клиенте
            var clientInfo = new ClientInfo
            {
                name = "Cash8",
                version = "1.0.0",
                id = "client123",
                token = "your_token_here" // Замените на реальный токен
            };
                        
            // Отправляем запрос
            var apiClient = new ApiClient();
            try
            {
                byte[] textAsBytes = Encoding.Default.GetBytes(marking_code);
                string imc = Convert.ToBase64String(textAsBytes);

                string response = apiClient.SendCodeRequest(imc, url, clientInfo);
                //строка ниже это когда офлайн ответ

//                string response = @"{
//""codesResponse"": {
//""codesResponse"": [
//{
//""code"": 0,
//""codes"": [
//{
//""cis"": ""0104670540176099215MpGKy"",
//""found"": false,
//""valid"": false,
//""printView"": ""0104670540176099215MpGKy"",
//""gtin"": ""04670540176099"",
//""groupIds"": [],
//""verified"": false,
//""realizable"": false,
//""utilised"": false,
//""isBlocked"": true,
//""ogvs"": []
//}
//],
//""reqId"": ""c9188551-817a-85a7-93e4-7042d907ab13"",
//""reqTimestamp"": ""1757681987579"",
//""isCheckedOffline"": true,
//""version"": ""6e7f1224-0e08-41ed-844c-d386675f4e50"",
//""inst"": ""4679b3db-da6a-44e0-a2e6-a684437bafb0""
//}
//]
//}
//}";
                apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response);
                var answer_check_mark = apiResponse.codesResponse.codesResponse[0];

                if (answer_check_mark.code == 0) // Это успех
                {
                    if (!answer_check_mark.isCheckedOffline)//Это была онлайн проверка 
                    {
                        string s = "ТОВАР НЕ МОЖЕТ БЫТЬ ПРОДАН!\r\n";
                        if (!answer_check_mark.codes[0].isOwner)
                        {
                            if (answer_check_mark.codes[0].groupIds != null)
                            {
                                if ((answer_check_mark.codes[0].groupIds[0] != 23) && (answer_check_mark.codes[0].groupIds[0] != 8) && (answer_check_mark.codes[0].groupIds[0] != 15))
                                {
                                    if (!productData.RrNotControlOwner())
                                    {
                                        MessageBox.Show(" Исключения групп маркрировки  23|8|15 \r\n Текущая группа маркировки  " + answer_check_mark.codes[0].groupIds[0].ToString());
                                        if (cash_Check.check_type.SelectedIndex == 0)
                                        {
                                            sb.AppendLine("Вы не являетесь владельцем!".ToUpper());
                                            MainStaticClass.write_cdn_log("CDN Код маркировки " + mark_str_cdn + " Вы не являетесь владельцем ", cash_Check.numdoc.ToString(), codes[0].ToString(), "1");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                sb.AppendLine("Не удалось определить группу товара");
                            }
                        }

                        if (!answer_check_mark.codes[0].valid)
                        {
                            sb.AppendLine("Результат проверки валидности структуры КИ / КиЗ не прошла проверку !".ToUpper());
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + mark_str_cdn + "Проверки валидности структуры КИ / КиЗ не прошла проверку !", "Документ чек", cash_Check.numdoc.ToString());
                        }

                        if (!answer_check_mark.codes[0].found)
                        {
                            sb.AppendLine("Не найден в ГИС МТ!".ToUpper());
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + mark_str_cdn + " не найден в ГИС МТ", "Документ чек", cash_Check.numdoc.ToString());
                            if ((!answer_check_mark.codes[0].realizable) && (!answer_check_mark.codes[0].sold))
                            {
                                sb.AppendLine("Нет информации о вводе в оборот!".ToUpper());
                                MainStaticClass.write_cdn_log("CDN Код маркировки " + mark_str_cdn + " нет информации о вводе в оборот. ", cash_Check.numdoc.ToString(), codes[0].ToString(), "1");
                            }
                        }
                        if (!answer_check_mark.codes[0].utilised)
                        {
                            sb.AppendLine("Эмитирован, но нет информации о его нанесении!".ToUpper());
                            MainStaticClass.write_cdn_log("CDN Код маркировки " + mark_str_cdn + " эмитирован, но нет информации о его нанесении. ", cash_Check.numdoc.ToString(), codes[0].ToString(), "1");
                        }
                        if (!answer_check_mark.codes[0].verified)
                        {
                            sb.AppendLine("Не пройдена криптографическая проверка!".ToUpper());
                            MainStaticClass.write_cdn_log("CDN Код маркировки " + mark_str_cdn + "  не пройдена криптографическая проверка.", cash_Check.numdoc.ToString(), codes[0].ToString(), "1");
                        }
                        if (answer_check_mark.codes[0].sold)
                        {
                            if (cash_Check.check_type.SelectedIndex == 0)
                            {
                                sb.AppendLine("Уже выведен из оборота!".ToUpper());
                                MainStaticClass.write_cdn_log("CDN Код маркировки " + mark_str_cdn + "  уже выведен из оборота.", cash_Check.numdoc.ToString(), codes[0].ToString(), "1");
                            }
                        }
                        if (answer_check_mark.codes[0].isBlocked)
                        {
                            sb.AppendLine("Заблокирован по решению ОГВ!".ToUpper());
                            MainStaticClass.write_cdn_log("CDN Код маркировки " + mark_str_cdn + "  заблокирован по решению ОГВ.", cash_Check.numdoc.ToString(), codes[0].ToString(), "1");
                        }
                        if (answer_check_mark.codes[0].expireDate.Year > 2000)
                        {
                            if (answer_check_mark.codes[0].expireDate < DateTime.Now)
                            {
                                sb.AppendLine("Истек срок годности!".ToUpper());
                                MainStaticClass.write_cdn_log("CDN У товара с кодом маркировки " + mark_str_cdn + "  истек срок годности.", cash_Check.numdoc.ToString(), codes[0].ToString(), "1");

                            }
                        }
                        if (sb.Length == 0)
                        {

                            if (cash_Check.verifyCDN.ContainsKey(mark_str))
                            {
                                cash_Check.verifyCDN.Remove(mark_str);
                            }

                            Cash_check.Requisite1260 requisite1260 = new Cash_check.Requisite1260();
                            requisite1260.req1262 = "030";
                            requisite1260.req1263 = "21.11.2023";
                            requisite1260.req1264 = "1944";
                            requisite1260.req1265 = "UUID=" + answer_check_mark.reqId + "&Time=" + answer_check_mark.reqTimestamp;
                            cash_Check.verifyCDN.Add(mark_str, requisite1260);

                            result_check = true;
                        }
                        else
                        {
                            int stringCount = sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length;
                            if (stringCount == 1)
                            {
                                sb.Insert(0, "Код маркировки " + mark_str + "\r\nне прошел проверку по следующей причине:\r\n".ToUpper());
                            }
                            else
                            {
                                sb.Insert(0, "Код маркировки " + mark_str + "\r\nне прошел проверку по следующим причинам:\r\n".ToUpper());
                            }
                            sb.Append(s);
                            sb.AppendLine(d_tovar.Keys.ElementAt(0));
                            sb.AppendLine(d_tovar[d_tovar.Keys.ElementAt(0)]);
                            MessageBox.Show(sb.ToString());
                        }
                    }
                    else//это была офлайн проверка 
                    {
                        if (answer_check_mark.codes[0].isBlocked)
                        {
                            result_check = false;
                        }
                        else
                        {
                            if (cash_Check.verifyCDN.ContainsKey(mark_str))
                            {
                                cash_Check.verifyCDN.Remove(mark_str);
                            }

                            Cash_check.Requisite1260 requisite1260 = new Cash_check.Requisite1260();
                            requisite1260.req1262 = "030";
                            requisite1260.req1263 = "21.11.2023";
                            requisite1260.req1264 = "1944";
                            requisite1260.req1265 = "UUID=" + answer_check_mark.reqId + "&Time=" + answer_check_mark.reqTimestamp;
                            cash_Check.verifyCDN.Add(mark_str, requisite1260);

                            result_check = true;
                        }

                    }
                }                
            }
            catch(Exception ex)
            {
                MessageBox.Show("Произошли ошибки при запросе к ПИОТ \r\n" + ex.Message);
                //MainStaticClass.write_cdn_log("CDN Код маркировки " + mark_str_cdn + "  не пройдена криптографическая проверка."+ ex.Message, cash_Check.numdoc.ToString(), codes[0].ToString(), "1");
                result_check = false;
            }            

                return result_check;
        }



        public class ApiResponse
        {
            [JsonProperty("codesResponse")]
            public CodesResponseWrapper codesResponse { get; set; }
        }

        public class CodesResponseWrapper
        {
            [JsonProperty("codesResponse")]
            public List<ResponseItem> codesResponse { get; set; }
        }

        public class ResponseItem
        {
            [JsonProperty("code")]
            public int code { get; set; }

            [JsonProperty("description")]
            public string description { get; set; }

            [JsonProperty("codes")]
            public List<CodeDetail> codes { get; set; }

            [JsonProperty("reqId")]
            public string reqId { get; set; }

            [JsonProperty("reqTimestamp")]
            public long reqTimestamp { get; set; }

            [JsonProperty("isCheckedOffline")]
            public bool isCheckedOffline { get; set; }
        }

        public class CodeDetail
        {
            [JsonProperty("cis")]
            public string cis { get; set; }

            [JsonProperty("found")]
            public bool found { get; set; }

            [JsonProperty("valid")]
            public bool valid { get; set; }

            [JsonProperty("printView")]
            public string printView { get; set; }

            [JsonProperty("gtin")]
            public string gtin { get; set; }

            [JsonProperty("groupIds")]
            public List<int> groupIds { get; set; }

            [JsonProperty("verified")]
            public bool verified { get; set; }

            [JsonProperty("realizable")]
            public bool realizable { get; set; }

            [JsonProperty("utilised")]
            public bool utilised { get; set; }

            [JsonProperty("productionDate")]
            public DateTime? productionDate { get; set; }

            [JsonProperty("isOwner")]
            public bool isOwner { get; set; }

            [JsonProperty("isBlocked")]
            public bool isBlocked { get; set; }

            [JsonProperty("ogvs")]
            public List<object> ogvs { get; set; }

            [JsonProperty("errorCode")]
            public int errorCode { get; set; }

            [JsonProperty("isTracking")]
            public bool isTracking { get; set; }

            [JsonProperty("sold")]
            public bool sold { get; set; }

            [JsonProperty("mrp")]
            public int? mrp { get; set; }

            [JsonProperty("grayZone")]
            public bool grayZone { get; set; }

            [JsonProperty("packageType")]
            public string packageType { get; set; }

            [JsonProperty("producerInn")]
            public string producerInn { get; set; }

            [JsonProperty("expireDate")]
            public DateTime expireDate { get; set; }            
        }


        public class ClientInfo
        {
            public string name { get; set; }
            public string version { get; set; }
            public string id { get; set; }
            public string token { get; set; }
        }

        public class ClientData
        {
            public List<string> codes { get; set; }
            public ClientInfo client_info { get; set; }
        }


        public class ApiClient
        {
            public string SendCodeRequest(string code, string url, ClientInfo clientInfo)
            {
                try
                {
                    // Создаем и заполняем объект данных
                    var clientData = new ClientData
                    {
                        codes = new List<string> { code },
                        client_info = clientInfo
                    };

                    // Сериализуем в JSON с помощью Newtonsoft.Json
                    string jsonData = JsonConvert.SerializeObject(clientData);

                    // Настраиваем web-запрос
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Accept = "application/json";
                    request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";

                    // Записываем данные в тело запроса
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(jsonData);
                        streamWriter.Flush();
                    }

                    // Получаем ответ
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = streamReader.ReadToEnd();
                        return result;
                    }
                }
                catch (WebException ex)
                {
                    // Обработка ошибок HTTP
                    if (ex.Response != null)
                    {
                        using (var errorResponse = (HttpWebResponse)ex.Response)
                        using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                        {
                            string errorText = reader.ReadToEnd();
                            throw new Exception($"HTTP Error: {(int)errorResponse.StatusCode} - {errorText}", ex);
                        }
                    }
                    throw new Exception("Network error: " + ex.Message, ex);
                }
            }
        }
    }

}

