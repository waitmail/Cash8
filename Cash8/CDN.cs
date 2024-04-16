using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Cash8
{
    class CDN
    {
        string kontur_url = "https://cdn.crpt.ru";//рабочий контур
        //string kontur_url     = "https://markirovka.sandbox.crptech.ru";//тестовый контур
        string info_url = "/api/v4/true-api/cdn/info";
        string health_url = "/api/v4/true-api/cdn/health/check";
        string codes_url = "/api/v4/true-api/codes/check";
        //string token    = "537e95bb-eb82-4eb5-83f6-4d177d4eed49";//тестовый токен
        //string token      = "c09faf94-383f-4fcf-a2da-2e786a585d8e";//боевой токен
        public int error_timeout = 0;//это счетчик ошибок при проверке кодов маркировки поможет понять что например нет интрнета

        public class Host
        {
            public string host { get; set; }
            public int avgTimeMs { get; set; }
            public long latensy { get; set; }
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
            public bool isOwner { get; set; }            
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
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            string url = kontur_url + info_url;
            CDN_List list = null;
            try
            {
                // Создание объекта HttpWebRequest
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 5000;
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
                MainStaticClass.write_event_in_log("Получение списка CDN get_cdn_info " + ex.Message, "Документ чек", "0");
                MessageBox.Show("Ошибка при запросе списка CDN площадок " + ex.Message);                
            }

            return list;
        }

        public class CDNHealth
        {
            public int code { get; set; }
            public string description { get; set; }
            public int avgTimeMs { get; set; }
            public long latency { get; set; }
        }

        ///// <summary>
        ///// Количество подходящих 
        ///// CDN серверов
        ///// </summary>
        ///// <param name="cdn_list"></param>
        ///// <returns></returns>
        //public int selection_and_sorting(CDN_List cdn_list)
        //{
        //    cdn_list.hosts = cdn_list.hosts.Where(h => h.dateTime < DateTime.Now).OrderBy(h => h.latensy).ToList();
        //    return cdn_list.hosts.Count;
        //}


        /// <summary>
        /// Возращает список cdn серверов
        /// </summary>
        /// <returns></returns>
        public CDN_List get_cdn_list()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            CDN_List list = get_cdn_info();
            if (list != null)
            {
                list = get_cdn_list_health(list);
            }
            return list;
        }


        /// <summary>
        /// Возращает список cdn серверов с данными по доступу к ним
        /// </summary>
        /// <returns></returns>
        public CDN_List get_cdn_list_health(CDN_List list)
        {
            if (list != null)
            {
                try
                {
                    CDNHealth cDNHealth = null;
                    if (list.code == 0)//ответ без ошибок 
                    {
                        foreach (Host host in list.hosts)
                        {
                            cDNHealth = cdn_health_check(host.host.ToString());
                            if (cDNHealth != null)
                            {
                                if (cDNHealth.code == 0)
                                {
                                    host.avgTimeMs = cDNHealth.avgTimeMs;
                                    host.dateTime = DateTime.Now;
                                }
                                else
                                {
                                    host.dateTime = DateTime.Now.AddMinutes(15);
                                }
                                host.latensy = cDNHealth.latency;
                            }
                            else
                            {
                                host.dateTime = DateTime.Now.AddMinutes(15);
                            }
                        }
                        //list.hosts = list.hosts.OrderBy(h => h.latensy).ThenByDescending(h => h.dateTime).ToList();
                        //возвращает список с наименьшим latensy и где время доступности меньше чем текущее, время может быть больше если площадка была помечена как не рабочая на 15 минут 
                        //list.hosts = list.hosts.Where(h => h.dateTime < DateTime.Now).OrderBy(h => h.latensy).ToList();
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
            }

            return list;
        }

        ///// <summary>
        ///// Возращает список cdn серверов
        ///// </summary>
        ///// <returns></returns>
        //public CDN_List get_cdn_list(CDN_List list)
        //{
        //    //CDN_List list = get_cdn_info();
        //    try
        //    {
        //        CDNHealth cDNHealth = null;
        //        if (list.code == 0)//ответ без ошибок 
        //        {
        //            foreach (Host host in list.hosts)
        //            {
        //                cDNHealth = cdn_health_check(host.host.ToString());
        //                if (cDNHealth.code == 0)
        //                {
        //                    host.avgTimeMs = cDNHealth.avgTimeMs;
        //                    host.dateTime = DateTime.Now;
        //                }
        //                else
        //                {
        //                    host.dateTime = DateTime.Now.AddMinutes(15);
        //                }
        //            }
        //            list.hosts = list.hosts.OrderBy(h => h.latensy).ThenByDescending(h => h.dateTime).ToList();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Произошли ошибка при опросе досутности CDN серверов, код ошибки  " + list.code + " , описание ошибки " + list.description);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Произошли ошибки при опросе досутности CDN серверов " + ex.Message);
        //    }

        //    return list;
        //}

        private CDNHealth cdn_health_check(string url_sdn)
        {
            string url = url_sdn + health_url;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            long latency = 9999999999;
            CDNHealth cDNHealth = null;// new CDNHealth();
            //cDNHealth.latency = latency;
            try
            {

                Stopwatch stopwatch = new Stopwatch();
                // Создание запроса
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 5000;

                // Добавление заголовков            
                request.Headers.Add("X-API-KEY", MainStaticClass.CDN_Token);
                request.ContentType = "application/json";

                // Запуск таймера непосредственно перед отправкой запроса
                stopwatch.Start();

                // Отправка запроса и получение ответа
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Остановка таймера сразу после получения ответа
                    stopwatch.Stop();

                    // Вывод времени задержки в миллисекундах
                    latency = stopwatch.ElapsedMilliseconds;
                //}

                //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;

                // Создание объекта HttpWebRequest
                //request = (HttpWebRequest)WebRequest.Create(url);
                //request.Method = "GET";
                //request.Timeout = 5000;

                //// Добавление заголовков            
                //request.Headers.Add("X-API-KEY", MainStaticClass.CDN_Token);
                //request.ContentType = "application/json";

                //// Получение ответа от сервера
                //using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                //{
                    // Вывод статус кода ответа
                    //Console.WriteLine("Status Code: " + (int)response.StatusCode);
                    int status_code = (int)response.StatusCode;
                    if (status_code != 200)
                    {
                        MessageBox.Show("Получен неверный ответ при запросе о доступности для CDN площадки" + url + ", код ответа = " + status_code.ToString(), "Опрос статуса доступности CDN площадки");
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
                            cDNHealth.latency = latency;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainStaticClass.write_event_in_log("Проверка доступности CDN cdn_health_check "+ url_sdn +"  "+ ex.Message, "Документ чек", "0");
                MessageBox.Show("Произошла ошибка " + ex.Message + " при запросе о доступности для CDN площадки " + url, "Получение списка CDN серверов");
            }

            return cDNHealth;
        }
               
        public bool check_marker_code(List<string> codes, string mark_str, ref Dictionary<string, Cash_check.CdnMarkerDateTime> cdn_markers_date_time, Int64 numdoc, ref HttpWebRequest request)
        {

            bool result_check = false;
            AnswerCheckMark answer_check_mark = null;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            CDN_List cdn_list = MainStaticClass.CDN_List;
            if (cdn_list == null)
            {
                MessageBox.Show("Список CDN серверов пустой");
                MainStaticClass.write_event_in_log("Список CDN серверов пустой check_marker_code ", "Документ чек", numdoc.ToString());
                result_check = true;
                return result_check;
            }
            else
            {
                MainStaticClass.write_event_in_log("Список CDN серверов успешно получен check_marker_code ", "Документ чек", numdoc.ToString());
            }
            
            cdn_list.hosts = cdn_list.hosts.Where(h => h.dateTime < DateTime.Now).OrderBy(h => h.latensy).ToList();

            if (cdn_list.hosts.Count == 0)
            {
                MessageBox.Show("Нет доступных CDN площадок для проверки кода маркировки, пробуем еще раз check_marker_code");
                MainStaticClass.write_event_in_log("Нет доступных CDN площадок для проверки кода маркировки, пробуем еще раз check_marker_code", "Документ чек", numdoc.ToString());
                cdn_list = MainStaticClass.CDN_List;
                if (cdn_list == null)
                {
                    MessageBox.Show("Список CDN серверов пустой check_marker_code");
                    MainStaticClass.write_event_in_log("Список CDN серверов пустой после 2-й попытки check_marker_code", "Документ чек", numdoc.ToString());
                    result_check = true;
                    return result_check;
                }
                cdn_list.hosts = cdn_list.hosts.Where(h => h.dateTime < DateTime.Now).OrderBy(h => h.latensy).ToList();

                if (cdn_list.hosts.Count == 0)
                {
                    MessageBox.Show("Нет доступных CDN площадок для проверки кода маркировки, вторая попытка check_marker_code");
                    MainStaticClass.write_event_in_log("Нет доступных CDN площадок для проверки кода маркировки, вторая попытка check_marker_code", "Документ чек", numdoc.ToString());
                    result_check = true;
                    return result_check;
                }
                else
                {
                    MainStaticClass.write_event_in_log("Список CDN серверов успешно получен, при повторном запросе,check_marker_code ", "Документ чек", numdoc.ToString());
                }
            }
            else
            {
                for (int i = 0; i < cdn_list.hosts.Count; i++)
                {                   
                   MainStaticClass.write_event_in_log(cdn_list.hosts[i].host+ " latensy = "+ cdn_list.hosts[i].latensy.ToString(), "Документ чек", numdoc.ToString());
                }
            }

            //cdn_list.hosts.Count
            string url = "";
            bool error = false;
            
            //foreach (Host host in cdn_list.hosts)
            //{
            CheckMark check_mark = new CheckMark();
            check_mark.codes = codes;
            check_mark.fiscalDriveNumber = MainStaticClass.FiscalDriveNumber;

            string body = JsonConvert.SerializeObject(check_mark, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            body = body.Replace("\\u001d", @"u001d");
            error = false; result_check = false;
            for (int i = 0; i < cdn_list.hosts.Count; i++)
            {
                Host host = cdn_list.hosts[i];
                //error = false;
//                result_check = false;
                url = host.host + codes_url;

                try
                {                    
                                    
                    if ((request == null)||(error))
                    {
                        request = (HttpWebRequest)WebRequest.Create(url);
                        request.KeepAlive = true;
                        request.Timeout = 1500;
                    }
                    request.Method = "POST";

                    // Добавление заголовков            
                    request.Headers.Add("X-API-KEY", MainStaticClass.CDN_Token);
                    request.ContentType = "application/json";

                    //CheckMark check_mark = new CheckMark();
                    //check_mark.codes = codes;
                    //check_mark.fiscalDriveNumber = MainStaticClass.FiscalDriveNumber;

                    //string body = JsonConvert.SerializeObject(check_mark, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    byte[] byteArray = Encoding.UTF8.GetBytes(body);

                    // Устанавливаем заголовок Content-Length
                    request.ContentLength = byteArray.Length;
                    //try
                    //{
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
                                MainStaticClass.write_event_in_log(host.host + " Успешное получение данных ", "Документ чек", numdoc.ToString());
                            }
                        }
                    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(" Ошибка при проверке кодов маркировки на CDN сервере " + ex.Message);
                    //}

                    if (answer_check_mark != null)
                    {
                        error = false;
                        StringBuilder sb = new StringBuilder();

                        string s = "ТОВАР НЕ МОЖЕТ БЫТЬ ПРОДАН!";
                        if (!answer_check_mark.codes[0].isOwner)
                        {
                            //MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + " Вы не являетесь владельцем " + s, "CDN проверка");
                            sb.Append("Вы не являетесь владельцем!\r\n".ToUpper());
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + answer_check_mark.codes[0].gtin + " Вы не являетесь владельцем ", "Документ чек", numdoc.ToString());
                        }
                        if (!answer_check_mark.codes[0].found)
                        {
                            //MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + " не найден в ГИС МТ" + s, "CDN проверка");
                            sb.Append("Не найден в ГИС МТ!\r\n".ToUpper());
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + answer_check_mark.codes[0].gtin + " не найден в ГИС МТ", "Документ чек", numdoc.ToString());
                            if ((!answer_check_mark.codes[0].realizable) && (!answer_check_mark.codes[0].sold))
                            {
                                sb.Append("Нет информации о вводе в оборот!\r\n".ToUpper());
                                //MessageBox.Show("Для кода маркировки " + answer_check_mark.codes[0].gtin + " нет информации о вводе в оборот." + s, "CDN проверка");
                                MainStaticClass.write_event_in_log("CDN Для кода маркировки " + answer_check_mark.codes[0].gtin + " нет информации о вводе в оборот.", "Документ чек", numdoc.ToString());
                            }
                        }
                        if (!answer_check_mark.codes[0].utilised)
                        {
                            sb.Append("митирован, но нет информации о его нанесении!\r\n".ToUpper());
                            //MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + " эмитирован, но нет информации о его нанесении." + s, "CDN проверка");
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + answer_check_mark.codes[0].gtin + " эмитирован, но нет информации о его нанесении.", "Документ чек", numdoc.ToString());
                        }
                        if (!answer_check_mark.codes[0].verified)
                        {
                            sb.Append("Не пройдена криптографическая проверка!\r\n".ToUpper());
                            //MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + "  не пройдена криптографическая проверка." + s, "CDN проверка");
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + answer_check_mark.codes[0].gtin + "  не пройдена криптографическая проверка.", "Документ чек", numdoc.ToString());
                        }
                        if (answer_check_mark.codes[0].sold)
                        {
                            sb.Append("Уже выведен из оборота!\r\n".ToUpper());
                            //MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + "  уже выведен из оборота." + s, "CDN проверка");
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + answer_check_mark.codes[0].gtin + "  уже выведен из оборота.", "Документ чек", numdoc.ToString());
                        }
                        if (answer_check_mark.codes[0].isBlocked)
                        {
                            sb.Append("Заблокирован по решению ОГВ!\r\n".ToUpper());
                            //MessageBox.Show("Код маркировки " + answer_check_mark.codes[0].gtin + "  заблокирован по решению ОГВ." + s, "CDN проверка");
                            MainStaticClass.write_event_in_log("CDN Код маркировки " + answer_check_mark.codes[0].gtin + "  заблокирован по решению ОГВ.", "Документ чек", numdoc.ToString());
                        }
                        //if ((!answer_check_mark.codes[0].realizable) && (!answer_check_mark.codes[0].sold))
                        //{
                        //    sb.Append("Нет информации о вводе в оборот!\r\n".ToUpper());
                        //    //MessageBox.Show("Для кода маркировки " + answer_check_mark.codes[0].gtin + " нет информации о вводе в оборот." + s, "CDN проверка");
                        //    MainStaticClass.write_event_in_log("CDN Для кода маркировки " + answer_check_mark.codes[0].gtin + " нет информации о вводе в оборот.", "Документ чек", numdoc.ToString());
                        //}
                        if (answer_check_mark.codes[0].expireDate.Year>2000)
                        {
                            if (answer_check_mark.codes[0].expireDate < DateTime.Now)
                            {
                                sb.Append("Истек срок годности!\r\n".ToUpper());
                                //MessageBox.Show("У товара с кодом маркировки " + answer_check_mark.codes[0].gtin + "  истек срок годности." + s, "CDN проверка");
                                MainStaticClass.write_event_in_log("CDN У товара с кодом маркировки " + answer_check_mark.codes[0].gtin + "  истек срок годности.", "Документ чек", numdoc.ToString());
                            }
                        }
                        if (sb.Length == 0)
                        {
                            result_check = true;
                            Cash_check.CdnMarkerDateTime cdnMarkerDateTime = new Cash_check.CdnMarkerDateTime();
                            cdnMarkerDateTime.reqId = answer_check_mark.reqId;
                            cdnMarkerDateTime.reqTimestamp = answer_check_mark.reqTimestamp;
                            cdn_markers_date_time[mark_str] = cdnMarkerDateTime;
                            break;//проверка успешно завершена
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
                            MessageBox.Show(sb.ToString());
                        }
                    }
                }
                catch (WebException ex)
                {
                    MainStaticClass.write_event_in_log("check_marker_code " + host.host+" "+ex.Message, "Документ чек", numdoc.ToString());
                    if (ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.ConnectionClosed)
                    {
                        //request = (HttpWebRequest)WebRequest.Create(url);
                        //request.KeepAlive = true;
                        //request.Timeout = 1500;
                        //result_check = true;//ошибка работы с интернет если таймаут и закрытое соедниненте тогда не является ошибкой кода маркировки
                        error = true;
                        if (error_timeout == 0)//если это первая ошибка по таймауту или соединение закрыто тогда попробуем обновить соединение и еще раз соединиться с наиболее быстрым CDN сервером
                        {
                            i--;
                        }
                        error_timeout++;                        
                    }
                    else
                    {
                        host.dateTime = DateTime.Now.AddMinutes(15);
                        result_check = false; //ошибка работы с интернет не является ошибкой кода маркировки
                        error = true;
                    }
                    //MainStaticClass.write_event_in_log("WebException при проверке кода маркировки check_marker_code "+ ex.Message, "Документ чек", numdoc.ToString());
                }
                catch (Exception ex)
                {
                    MainStaticClass.write_event_in_log("check_marker_code " + host.host + " " + ex.Message, "Документ чек", numdoc.ToString());
                    //host.dateTime = DateTime.Now.AddMinutes(15);
                    error = true;
                }
                if (!error)
                {
                    break;
                }
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
