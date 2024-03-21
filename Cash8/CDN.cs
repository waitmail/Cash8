using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Cash8
{
    class CDN
    {         
        string kontur_url = "https://cdn.crpt.ru";//рабочий контур
        //string kontur_url     = "https://markirovka.sandbox.crptech.ru";//тестовый контур
        string info_url       = "/api/v4/true-api/cdn/info";
        string health_url = "/api/v4/true-api/cdn/health/check";
        string codes_url  = "/api/v4/true-api/codes/check";
        //string token    = "537e95bb-eb82-4eb5-83f6-4d177d4eed49";//тестовый токен
        //string token      = "c09faf94-383f-4fcf-a2da-2e786a585d8e";//боевой токен

        public class Host
        {
            public string host { get; set; }
            public int avgTimeMs { get; set; }
            public DateTime dateTime { get; set; }
        }
        
        public class CDN_List
        {
            public int code { get; set; }
            public string description { get; set; }
            public List<Host> hosts { get; set; }
            public DateTime createDateTime { get; set; }
        }

        public class CDN_Health
        {
            public int code { get; set; }
            public string description { get; set; }
            public int avgTimeMs { get; set; }
        }

        public class CheckMark
        {
            public List<string> codes { get; set; }
            public string fiscalDriveNumber { get; set; }
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Code
        {
            public string cis { get; set; }
            public bool valid { get; set; }
            public string printView { get; set; }
            public string gtin { get; set; }
            public List<int> groupIds { get; set; }
            public bool verified { get; set; }
            public bool found { get; set; }
            public bool realizable { get; set; }
            public bool utilised { get; set; }
            public bool isBlocked { get; set; }
            public DateTime expireDate { get; set; }
            public DateTime productionDate { get; set; }
            public int errorCode { get; set; }
            public bool isTracking { get; set; }
            public bool sold { get; set; }
            public string packageType { get; set; }
            public string producerInn { get; set; }
            public bool grayZone { get; set; }
            public int soldUnitCount { get; set; }
            public int innerUnitCount { get; set; }
        }

        public class AnswerCheckMark
        {
            public int code { get; set; }
            public string description { get; set; }
            public List<Code> codes { get; set; }
            public string reqId { get; set; }
            public long reqTimestamp { get; set; }
        }

        private CDN_List get_cdn_info()
        {
            string url = kontur_url + info_url;
            CDN_List list = null;
            try
            {
                // Создание объекта HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                // Добавление заголовков            
                request.Headers.Add("X-API-KEY", MainStaticClass.CDN_Token);
                request.ContentType = "application/json";
                
                // Получение ответа от сервера
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Вывод статус кода ответа
                    Console.WriteLine("Status Code: " + (int)response.StatusCode);
                    int status_code = (int)response.StatusCode;
                    if (status_code != 200)
                    {
                        MessageBox.Show("Получен неверный ответ от сервера при запросе списка CDN серверов, кот ответа = " + status_code.ToString(), "Получение списка CDN серверов");
                        return list;
                    }

                    // Чтение и вывод содержимого ответа
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string sdn_list = reader.ReadToEnd();
                            //MessageBox.Show("Response: " + sdn_list);
                            //txtB_result.Text = sdn_list;
                            list = JsonConvert.DeserializeObject<CDN_List>(sdn_list);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при запросе списка CDN площадок " + ex.Message);
            }

            return list;
        }

        public class CDNHealth
        {
            public int code { get; set; }
            public string description { get; set; }
            public int avgTimeMs { get; set; }
        }
               
        /// <summary>
        /// Возращает список cdn серверов
        /// </summary>
        /// <returns></returns>
        public CDN_List get_cdn_list()
        {
            CDN_List list = get_cdn_info();
            try
            {
                CDNHealth cDNHealth = null;
                if (list.code == 0)//ответ без ошибок 
                {
                    foreach (Host host in list.hosts)
                    {
                        cDNHealth = cdn_health_check(host.host.ToString());
                        if (cDNHealth.code == 0)
                        {
                            host.avgTimeMs = cDNHealth.avgTimeMs;
                            host.dateTime = DateTime.Now;
                        }
                        else
                        {
                            host.dateTime = DateTime.Now.AddMinutes(15);
                        }
                    }
                    list.hosts = list.hosts.OrderBy(h => h.avgTimeMs).ThenByDescending(h => h.dateTime).ToList();
                }
                else
                {
                    MessageBox.Show("Произошли ошибка при опросе досутности CDN серверов, код ошибки  "+ list.code+" , описание ошибки "+list.description);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при опросе досутности CDN серверов "+ex.Message);
            }

            return list;
        }

        /// <summary>
        /// Возращает список cdn серверов
        /// </summary>
        /// <returns></returns>
        public CDN_List get_cdn_list(CDN_List list)
        {
            //CDN_List list = get_cdn_info();
            try
            {
                CDNHealth cDNHealth = null;
                if (list.code == 0)//ответ без ошибок 
                {
                    foreach (Host host in list.hosts)
                    {
                        cDNHealth = cdn_health_check(host.host.ToString());
                        if (cDNHealth.code == 0)
                        {
                            host.avgTimeMs = cDNHealth.avgTimeMs;
                            host.dateTime = DateTime.Now;
                        }
                        else
                        {
                            host.dateTime = DateTime.Now.AddMinutes(15);
                        }
                    }
                    list.hosts = list.hosts.OrderBy(h => h.avgTimeMs).ThenByDescending(h => h.dateTime).ToList();
                }
                else
                {
                    MessageBox.Show("Произошли ошибка при опросе досутности CDN серверов, код ошибки  " + list.code + " , описание ошибки " + list.description);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при опросе досутности CDN серверов " + ex.Message);
            }

            return list;
        }

        private CDNHealth cdn_health_check(string url_sdn)
        {
            string url = url_sdn + health_url;

            CDNHealth cDNHealth = null;
            try
            {
                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                // Создание объекта HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                // Добавление заголовков            
                request.Headers.Add("X-API-KEY", MainStaticClass.CDN_Token);
                request.ContentType = "application/json";

                // Получение ответа от сервера
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Вывод статус кода ответа
                    Console.WriteLine("Status Code: " + (int)response.StatusCode);
                    int status_code = (int)response.StatusCode;
                    if (status_code != 200)
                    {
                        MessageBox.Show("Получен неверный ответ при запросе о доступности для CDN площадки"+url+", код ответа = " + status_code.ToString(), "Опрос статуса доступности CDN площадки");
                        return cDNHealth;
                    }

                    // Чтение и вывод содержимого ответа
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string cdn_health = reader.ReadToEnd();
                            //MessageBox.Show("Response: " + sdn_list);
                            //txtB_result.Text += cdn_health + "\r\n";
                            cDNHealth = JsonConvert.DeserializeObject<CDNHealth>(cdn_health);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка "+ex.Message+ " при запросе о доступности для CDN площадки "+ url, "Получение списка CDN серверов");
            }

            return cDNHealth;
        }


        public bool check_marker_code(List<string> codes, string mark_str, ref Dictionary<string, Cash_check.CdnMarkerDateTime> cdn_markers_date_time,Int64 numdoc)
        {
            bool result_check = false;
            AnswerCheckMark answer_check_mark = null;
            try
            {
                //string url = kontur_url + codes_url;
                CDN_List cdn_list = get_cdn_list();
                if (cdn_list == null)
                {
                    MessageBox.Show("Список CDN серверов пустой");
                    return result_check;
                }
                MessageBox.Show(cdn_list.hosts[0].host, "check_marker_code");
                MessageBox.Show(Application.StartupPath);
                File.WriteAllText("Application.StartupPath/cdn.txt",cdn_list.hosts[0].host);
                string url = cdn_list.hosts[0].host + codes_url;
                //string url = "https://cdn01.am.crptech.ru"+ codes_url;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                // Добавление заголовков            
                request.Headers.Add("X-API-KEY", MainStaticClass.CDN_Token);
                request.ContentType = "application/json";

                CheckMark check_mark = new CheckMark();
                check_mark.codes = codes;
                check_mark.fiscalDriveNumber = MainStaticClass.FiscalDriveNumber;

                string body = JsonConvert.SerializeObject(check_mark, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                byte[] byteArray = Encoding.UTF8.GetBytes(body);

                // Устанавливаем заголовок Content-Length
                request.ContentLength = byteArray.Length;
                try
                {
                    // Пишем данные в поток запроса
                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }

                    // Получаем ответ
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        // Открываем поток ответа
                        using (var responseStream = response.GetResponseStream())
                        {
                            // Читаем поток ответа
                            using (var reader = new StreamReader(responseStream))
                            {
                                string responseFromServer = reader.ReadToEnd();
                                answer_check_mark = JsonConvert.DeserializeObject<AnswerCheckMark>(responseFromServer);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" Ошибка при проверке кодов маркировки на CDN сервере " + ex.Message);
                }

                if (answer_check_mark != null)
                {
                    string s = " ТОВАР НЕ МОЖЕТ БЫТЬ ПРОДАН!!! ";
                    if (!answer_check_mark.codes[0].found)
                    {
                        MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + " не найден в ГИС МТ" + s, "CDN проверка");
                        MainStaticClass.write_event_in_log("Код маркировки " + answer_check_mark.codes[0].gtin + " не найден в ГИС МТ", "Документ чек", numdoc.ToString());
                    }
                    else if (!answer_check_mark.codes[0].utilised)
                    {
                        MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + " эмитирован, но нет информации о его нанесении." + s, "CDN проверка");
                        MainStaticClass.write_event_in_log("Код маркировки " + answer_check_mark.codes[0].gtin + " эмитирован, но нет информации о его нанесении.", "Документ чек", numdoc.ToString());
                    }
                    else if (!answer_check_mark.codes[0].verified)
                    {
                        MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + "  не пройдена криптографическая проверка." + s, "CDN проверка");
                        MainStaticClass.write_event_in_log("Код маркировки " + answer_check_mark.codes[0].gtin + "  не пройдена криптографическая проверка.", "Документ чек", numdoc.ToString());
                    }
                    else if (answer_check_mark.codes[0].sold)
                    {
                        MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + "  уже выведен из оборота." + s, "CDN проверка");
                        MainStaticClass.write_event_in_log("Код маркировки " + answer_check_mark.codes[0].gtin + "  уже выведен из оборота.", "Документ чек", numdoc.ToString());
                    }
                    else if (answer_check_mark.codes[0].isBlocked)
                    {
                        MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + "  заблокирован по решению ОГВ." + s, "CDN проверка");
                        MainStaticClass.write_event_in_log("Код маркировки " + answer_check_mark.codes[0].gtin + "  заблокирован по решению ОГВ.", "Документ чек", numdoc.ToString());
                    }
                    else if ((!answer_check_mark.codes[0].realizable) && (!answer_check_mark.codes[0].sold))
                    {
                        MessageBox.Show("Для кода маркировки " + answer_check_mark.codes[0].gtin + " нет информации о вводе в оборот." + s, "CDN проверка");
                        MainStaticClass.write_event_in_log("Для кода маркировки " + answer_check_mark.codes[0].gtin + " нет информации о вводе в оборот.", "Документ чек", numdoc.ToString());
                    }
                    else if (answer_check_mark.codes[0].expireDate >= DateTime.Now)
                    {
                        MessageBox.Show("У товара с кодом маркировки " + answer_check_mark.codes[0].gtin + "  истек срок годности." + s, "CDN проверка");
                        MainStaticClass.write_event_in_log("У товара с кодом маркировки " + answer_check_mark.codes[0].gtin + "  истек срок годности.", "Документ чек", numdoc.ToString());
                    }
                    else
                    {
                        result_check = true;
                        Cash_check.CdnMarkerDateTime cdnMarkerDateTime = new Cash_check.CdnMarkerDateTime();
                        cdnMarkerDateTime.reqId = answer_check_mark.reqId;
                        cdnMarkerDateTime.reqTimestamp = answer_check_mark.reqTimestamp;

                        cdn_markers_date_time[mark_str] = cdnMarkerDateTime;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Ошибка при проверке кода маркировки ");
                MainStaticClass.write_event_in_log("Ошибка при проверке кода маркировки check_marker_code " + mark_str+"  "+ex.Message , "Документ чек", numdoc.ToString());
            }

            return result_check;
        }       


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public CDN_List get_cdn_list()
        //{

        //    CDN_List list = null;
        //    try
        //    {
        //        string url = kontur + info;
        //        // Создание объекта HttpWebRequest
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //        request.Method = "GET";

        //        // Добавление заголовков            
        //        request.Headers.Add("X-API-KEY", MainStaticClass.SDN_Token);
        //        request.ContentType = "application/json";

        //        // Получение ответа от сервера
        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            // Вывод статус кода ответа
        //            Console.WriteLine("Status Code: " + (int)response.StatusCode);
        //            int status_code = (int)response.StatusCode;
        //            if (status_code != 200)
        //            {
        //                MessageBox.Show("Получен неверный ответ от сервера при запросе списка CDN серверов, кот ответа = " + status_code.ToString(), "Получение списка CDN серверов");
        //                return list;
        //            }

        //            // Чтение и вывод содержимого ответа
        //            using (Stream stream = response.GetResponseStream())
        //            {
        //                using (StreamReader reader = new StreamReader(stream))
        //                {
        //                    string sdn_list = reader.ReadToEnd();
        //                    MessageBox.Show("Response: " + sdn_list);
        //                    list = JsonConvert.DeserializeObject<CDN_List>(sdn_list);
        //                    list.createDateTime = DateTime.Now;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Получен неверный ответ от сервера при запросе списка CDN серверов " + ex.Message, "Получение списка CDN серверов");
        //    }

        //    return list;
        //}
    }
}
