using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace Cash8
{
    public partial class SentDataOnBonusEva : Form
    {
        public bool run_in_the_background = false;       

        public SentDataOnBonusEva()
        {
            InitializeComponent();
        }


        public class BuyCommit
        {
            public string transactionId { get; set; }
            public string cardNum { get; set; }            
        }


        public bool sent_document_buyCommit(BuyCommit buyCommit, string document_number)
        {
            bool result = true;

            string json = JsonConvert.SerializeObject(buyCommit, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            txtB_jason.Text = json;
            //string url = "http://92.242.41.218/processing/v3/buyCommit/";
            string url = MainStaticClass.GetStartUrl + "/v3/buyCommit/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //string shop_request = "";
            //if (MainStaticClass.Nick_Shop.Substring(0, 1).ToUpper() == "A")
            //{
            //    shop_request = MainStaticClass.Nick_Shop + MainStaticClass.CashDeskNumber;
            //}
            //else
            //{
            //    shop_request = "1" + Convert.ToInt16(MainStaticClass.Nick_Shop.Substring(1, 2)).ToString() + MainStaticClass.CashDeskNumber;
            //}

            ////var authString = Convert.ToBase64String(Encoding.Default.GetBytes("A011" + ":" + "JpDkHs~AE%zS8Y7HDpVM"));
            //var authString = Convert.ToBase64String(Encoding.Default.GetBytes(shop_request + ":" + MainStaticClass.PassPromo));
            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }


                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    byte[] buf = new byte[10000];
                    int count = -1;
                    String read = "";
                    do
                    {
                        count = response.GetResponseStream().Read(buf, 0, buf.Length);
                        read += Encoding.UTF8.GetString(buf, 0, count);
                    } while (response.GetResponseStream().CanRead && count != 0);
                    //txtB_json_response.Text = JsonConvert.DeserializeObject(read).ToString();
                    //string answer = JsonConvert.DeserializeObject(read.Replace("{}", @"""""")).ToString();//read.Replace("{}","\"\"")
                    BuynewResponse buynewResponse = JsonConvert.DeserializeObject<BuynewResponse>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (buynewResponse.res == "1")
                    {
                        conn.Open();
                        string query = "UPDATE checks_header SET sent_to_processing_center=1 WHERE document_number =" + document_number;
                        NpgsqlCommand command = new NpgsqlCommand(query, conn);
                        int rowsaffected = command.ExecuteNonQuery();
                        conn.Close();
                        command.Dispose();
                    }
                    else //Куда то записать информацию о трудностях
                    {

                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                //buynewResponse = null;
            }
            catch (NpgsqlException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                //buynewResponse = null;
            }
            catch (Exception ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                //buynewResponse = null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        public bool sent_document_buyNew(BuyNewRequest buyNewRequest, string document_number, ref BuynewResponse buynewResponse)
        {
            bool result = true;

            string json = JsonConvert.SerializeObject(buyNewRequest, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            txtB_jason.Text = json;
            //string url = "http://92.242.41.218/processing/v3/buyNew/";
            string url = MainStaticClass.GetStartUrl + "/v3/buyNew/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //string shop_request = "";
            //if (MainStaticClass.Nick_Shop.Substring(0, 1).ToUpper() == "A")
            //{
            //    shop_request = MainStaticClass.Nick_Shop + MainStaticClass.CashDeskNumber;
            //}
            //else
            //{
            //    shop_request = "1" + Convert.ToInt16(MainStaticClass.Nick_Shop.Substring(1, 2)).ToString() + MainStaticClass.CashDeskNumber;
            //}

            ////var authString = Convert.ToBase64String(Encoding.Default.GetBytes("A011" + ":" + "JpDkHs~AE%zS8Y7HDpVM"));
            //var authString = Convert.ToBase64String(Encoding.Default.GetBytes(shop_request + ":" + MainStaticClass.PassPromo));
            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }




                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    byte[] buf = new byte[10000];
                    int count = -1;
                    String read = "";
                    do
                    {
                        count = response.GetResponseStream().Read(buf, 0, buf.Length);
                        read += Encoding.UTF8.GetString(buf, 0, count);
                    } while (response.GetResponseStream().CanRead && count != 0);
                    //txtB_json_response.Text = JsonConvert.DeserializeObject(read).ToString();
                    //string answer = JsonConvert.DeserializeObject(read.Replace("{}", @"""""")).ToString();//read.Replace("{}","\"\"")
                    buynewResponse = JsonConvert.DeserializeObject<BuynewResponse>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (buynewResponse.res == "1")
                    {
                        if (buyNewRequest.commit == "1")
                        {
                            conn.Open();
                            string query = "UPDATE checks_header SET sent_to_processing_center=1 WHERE document_number =" + document_number;
                            NpgsqlCommand command = new NpgsqlCommand(query, conn);
                            int rowsaffected = command.ExecuteNonQuery();
                            conn.Close();
                            command.Dispose();
                        }
                    }
                    else //Куда то записать информацию о трудностях
                    {

                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                buynewResponse = null;
            }
            catch (NpgsqlException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                buynewResponse = null;
            }
            catch (Exception ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                buynewResponse = null;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }


        //public bool sent_document_buyReturn(BuyNewRequest buyNewRequest, string document_number, ref BuynewResponse buynewResponse)
        //{
        //    bool result = true;

        //    string json = JsonConvert.SerializeObject(buyNewRequest, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    txtB_jason.Text = json;
        //    //string url = "http://92.242.41.218/processing/v3/buyNew/";
        //    string url = MainStaticClass.GetStartUrl + "/v3/buyNew/";

        //    byte[] body = Encoding.UTF8.GetBytes(json);
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    //string shop_request = "";
        //    //if (MainStaticClass.Nick_Shop.Substring(0, 1).ToUpper() == "A")
        //    //{
        //    //    shop_request = MainStaticClass.Nick_Shop + MainStaticClass.CashDeskNumber;
        //    //}
        //    //else
        //    //{
        //    //    shop_request = "1" + Convert.ToInt16(MainStaticClass.Nick_Shop.Substring(1, 2)).ToString() + MainStaticClass.CashDeskNumber;
        //    //}

        //    ////var authString = Convert.ToBase64String(Encoding.Default.GetBytes("A011" + ":" + "JpDkHs~AE%zS8Y7HDpVM"));
        //    //var authString = Convert.ToBase64String(Encoding.Default.GetBytes(shop_request + ":" + MainStaticClass.PassPromo));
        //    var authString = MainStaticClass.GetAuthStringProcessing;

        //    request.Headers.Add("Authorization", "Basic " + authString);

        //    request.Method = "POST";
        //    request.ContentType = "application/json; charset=utf-8";
        //    request.ContentLength = body.Length;
        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

        //    try
        //    {
        //        using (Stream stream = request.GetRequestStream())
        //        {
        //            stream.Write(body, 0, body.Length);
        //            stream.Close();
        //        }




        //        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        //        {
        //            byte[] buf = new byte[10000];
        //            int count = -1;
        //            String read = "";
        //            do
        //            {
        //                count = response.GetResponseStream().Read(buf, 0, buf.Length);
        //                read += Encoding.UTF8.GetString(buf, 0, count);
        //            } while (response.GetResponseStream().CanRead && count != 0);
        //            //txtB_json_response.Text = JsonConvert.DeserializeObject(read).ToString();
        //            //string answer = JsonConvert.DeserializeObject(read.Replace("{}", @"""""")).ToString();//read.Replace("{}","\"\"")
        //            buynewResponse = JsonConvert.DeserializeObject<BuynewResponse>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //            if (buynewResponse.res == "1")
        //            {
        //                if (buyNewRequest.commit == "1")
        //                {
        //                    conn.Open();
        //                    string query = "UPDATE checks_header SET sent_to_processing_center=1 WHERE document_number =" + document_number;
        //                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                    int rowsaffected = command.ExecuteNonQuery();
        //                    conn.Close();
        //                    command.Dispose();
        //                }
        //            }
        //            else //Куда то записать информацию о трудностях
        //            {

        //            }
        //            response.Close();
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        if (!run_in_the_background)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //        buynewResponse = null;
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        if (!run_in_the_background)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //        buynewResponse = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!run_in_the_background)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //        buynewResponse = null;
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //    return result;
        //}

            
        /// <summary>
        /// Отправка чеков прожади в режиме commit=1
        /// </summary>
        /// <param name="buyNewRequest"></param>
        /// <param name="document_number"></param>
        /// <returns></returns>
        public bool sent_document(BuyNewRequest buyNewRequest, string document_number)
        {
            bool result = true;

            string json = JsonConvert.SerializeObject(buyNewRequest, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            txtB_jason.Text = json;            
            string url = MainStaticClass.GetStartUrl + "/v3/buyNew/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);            
            var authString = MainStaticClass.GetAuthStringProcessing;
            request.Headers.Add("Authorization", "Basic " + authString);

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }



                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    byte[] buf = new byte[10000];
                    int count = -1;
                    String read = "";
                    do
                    {
                        count = response.GetResponseStream().Read(buf, 0, buf.Length);
                        read += Encoding.UTF8.GetString(buf, 0, count);
                    } while (response.GetResponseStream().CanRead && count != 0);                    
                    BuynewResponse buynewResponse = JsonConvert.DeserializeObject<BuynewResponse>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (buynewResponse.res == "1")
                    {
                        if (buyNewRequest.commit == "1")
                        {
                            conn.Open();
                            //string query = "UPDATE checks_header SET sent_to_processing_center=1, id_transaction_sale = "+ buynewResponse.transactionId +" WHERE document_number =" + document_number ;
                            string query = "UPDATE checks_header SET sent_to_processing_center=1  WHERE document_number =" + document_number;
                            NpgsqlCommand command = new NpgsqlCommand(query, conn);
                            int rowsaffected = command.ExecuteNonQuery();

                            //if (buyNewRequest.type == "2")//понадобится для возвратов 
                            //{
                            //    query = "UPDATE checks_header SET id_transaction=" + buynewResponse.transactionId + " WHERE document_number = " + document_number;
                            //    command = new NpgsqlCommand(query, conn);
                            //    command.ExecuteNonQuery();
                            //}
                            conn.Close();
                            command.Dispose();
                        }
                    }
                    else //Куда то записать информацию о трудностях
                    {
                        //MessageBox.Show(buynewResponse.res);
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (NpgsqlException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// Отправка чеков возврата в режиме commit=1
        /// </summary>
        /// <param name="buyNewRequest"></param>
        /// <param name="document_number"></param>
        /// <returns></returns>
        public bool sent_document(BuyReturnRequest buyReturnRequest, string document_number)
        {
            bool result = true;

            string json = JsonConvert.SerializeObject(buyReturnRequest, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });            
            string url = MainStaticClass.GetStartUrl + "/v3/buyNew/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var authString = MainStaticClass.GetAuthStringProcessing;
            request.Headers.Add("Authorization", "Basic " + authString);

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    byte[] buf = new byte[10000];
                    int count = -1;
                    String read = "";
                    do
                    {
                        count = response.GetResponseStream().Read(buf, 0, buf.Length);
                        read += Encoding.UTF8.GetString(buf, 0, count);
                    } while (response.GetResponseStream().CanRead && count != 0);
                    TransactionResponse transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    if (transactionResponse.res == "1")
                    {                       
                            conn.Open();
                            //string query = "UPDATE checks_header SET sent_to_processing_center=1, id_transaction_sale = " + transactionResponse.transactionId + "   WHERE document_number =" + document_number;
                            string query = "UPDATE checks_header SET sent_to_processing_center=1 WHERE document_number =" + document_number;
                            NpgsqlCommand command = new NpgsqlCommand(query, conn);
                            conn.Close();
                            command.Dispose();                        
                    }
                    else //Куда то записать информацию о трудностях
                    {
                        //MessageBox.Show(buynewResponse.res);
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (NpgsqlException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return result;
        }

        public bool sent_documentbuyReturn(BuyReturnRequest buyReturnRequest, string document_number, ref TransactionResponse transactionResponse)
        {
            bool result = true;

            string json = JsonConvert.SerializeObject(buyReturnRequest, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });            
            string url = MainStaticClass.GetStartUrl + "/v3/buyReturn/";

            byte[] body = Encoding.UTF8.GetBytes(json);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);            
            var authString = MainStaticClass.GetAuthStringProcessing;

            request.Headers.Add("Authorization", "Basic " + authString);

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = body.Length;


            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }             


                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    byte[] buf = new byte[10000];
                    int count = -1;
                    String read = "";
                    do
                    {
                        count = response.GetResponseStream().Read(buf, 0, buf.Length);
                        read += Encoding.UTF8.GetString(buf, 0, count);
                    } while (response.GetResponseStream().CanRead && count != 0);
                    //txtB_json_response.Text = JsonConvert.DeserializeObject(read).ToString();
                    //string answer = JsonConvert.DeserializeObject(read.Replace("{}", @"""""")).ToString();//read.Replace("{}","\"\"")
                    //BuynewResponse buynewResponse = JsonConvert.DeserializeObject<BuynewResponse>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    //if (buynewResponse.res == "1")
                    //{
                    //    conn.Open();
                    //    string query = "UPDATE checks_header SET sent_to_processing_center=1,id_transaction=" + buynewResponse.transactionId + " WHERE document_number =" + document_number;
                    //    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    //    int rowsaffected = command.ExecuteNonQuery();
                    //    conn.Close();
                    //    command.Dispose();
                    //}
                    //else //Куда то записать информацию о трудностях
                    //{
                    //    get_description_errors_on_code(buynewResponse.res);
                    //}
                    transactionResponse = JsonConvert.DeserializeObject<TransactionResponse>(read.Replace("{}", @""""""), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                result = false;
            }
            catch (NpgsqlException ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                if (!run_in_the_background)
                {
                    MessageBox.Show(ex.Message);
                }
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// получить текстовое описание по коду 
        /// </summary>
        /// <param name="code_error"></param>
        private void get_description_errors_on_code(string code_answer)
        {
            switch (code_answer)
            {
                case "2":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("2. Неверный запрос ");
                    }
                    break;
                case "3":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("3. Клиент не найден ");
                    }
                    break;
                case "4":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("4. Недостаточно прав для операции ");
                    }
                    break;
                case "5":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("5. Карта не найдена ");
                    }
                    break;
                case "6":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("6. Операции с картой запрещены ");
                    }
                    break;
                case "7":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("7. Ошибочная операция с картой ");
                    }
                    break;
                case "8":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("8. Недостаточно средств ");
                    }
                    break;
                case "9":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("9. Транзакция не найдена(неверный transactionId) ");
                    }
                    break;
                case "10":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("10. Значение параметра выходит за границы ");
                    }
                    break;
                case "11":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("11. Id чека, сгенерированный кассой, не уникален ");
                    }
                    break;
                case "12":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("12. Артикул товара(SKU) не найден ");
                    }
                    break;
                case "13":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("13. Ошибка запроса ");
                    }
                    break;
                case "14":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("14. Ошибка базы данных ");
                    }
                    break;
                case "15":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("15. Транзакция отклонена ");
                    }
                    break;
                case "16":
                    if (!run_in_the_background)
                    {
                        MessageBox.Show("16. Транзакция уже существует ");
                    }
                    break;

                default:
                    if (!run_in_the_background)
                    {
                        MessageBox.Show(" Неизвестный ответ от процессингового центра ");
                    }
                    break;
            }
        }


        /// <summary>
        /// Главный вызываемый метод по отправке бонусов
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void sent_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.PassPromo == "")
            {
                MessageBox.Show("Эта касса не учавствует в бонусной программе ");
                return;
            }

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = " SELECT document_number, client, cash_desk_number,date_time_start,bonuses_it_is_written_off,id_transaction,id_transaction_sale,check_type  " +
                    " FROM checks_header WHERE sent_to_processing_center = 0 and its_deleted=0";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                BuyNewRequest buyNewRequest = new BuyNewRequest();
                while (reader.Read())//Здесь перебираем документы
                {
                    //предварительно было отправлено buy_new, поэтому если нет оплаты бонусами то нужно отправить buycommit
                    //if ((reader["id_transaction"].ToString().Trim() != "")&&(Convert.ToInt32(reader["bonuses_it_is_written_off"]) == 0))
                    if (reader["id_transaction"].ToString().Trim() != "")
                    {
                        BuyCommit buyCommit = new BuyCommit();
                        buyCommit.transactionId = reader["id_transaction"].ToString().Trim();
                        sent_document_buyCommit(buyCommit, reader["document_number"].ToString().Trim());
                        continue;
                    }
                    if (reader["check_type"].ToString() == "0")
                    {
                        buyNewRequest.cashierName = MainStaticClass.Cash_Operator;
                        buyNewRequest.commit = "1";
                        buyNewRequest.date = Convert.ToDateTime(reader["date_time_start"]).ToString("yyyy-MM-dd HH:mm:ss");
                        //if (reader["client"].ToString().Trim().Length == 36)
                        //{
                        //    buyNewRequest.cardTrack2 = reader["client"].ToString();
                        //}
                        if (reader["client"].ToString().Trim().Length >= 10)
                        {
                            buyNewRequest.cardNum = reader["client"].ToString();
                        }

                        if ((Convert.ToInt32(reader["bonuses_it_is_written_off"]) != 0) && (reader["client"].ToString().Trim().Length == 10))
                        {
                            buyNewRequest.charge = (Convert.ToInt32(reader["bonuses_it_is_written_off"]) * 100).ToString();
                        }

                        //if (reader["client"].ToString().Trim().Length == 0)
                        //{
                        //    buyNewRequest.type = "6";
                        //}
                        //else
                        //{
                        //    buyNewRequest.type = "2";
                        //}
                        if (reader["client"].ToString().Trim().Length == 0)
                        {
                            buyNewRequest.type = "6";
                        }
                        else
                        {
                            if (buyNewRequest.charge != null)
                            {
                                buyNewRequest.type = "3";
                            }
                            else
                            {
                                buyNewRequest.type = "2";
                            }
                        }
                        if (fill_items(buyNewRequest, reader["document_number"].ToString(), reader["client"].ToString()))
                        {
                            sent_document(buyNewRequest, reader["document_number"].ToString());
                        }
                    }
                    else if (reader["check_type"].ToString() == "1")
                    {
                        BuyReturnRequest buyReturnRequest = new BuyReturnRequest();
                        buyReturnRequest.cashierName = MainStaticClass.Cash_Operator;
                        buyReturnRequest.commit = "1";
                        buyReturnRequest.date = Convert.ToDateTime(reader["date_time_start"]).ToString("yyyy-MM-dd HH:mm:ss");
                        buyReturnRequest.transactionId = reader["id_transaction_sale"].ToString();
                        if (fill_items(buyReturnRequest, reader["document_number"].ToString(), reader["client"].ToString()))
                        {
                            sent_document(buyReturnRequest, reader["document_number"].ToString());
                        }
                    }                    
                }
                reader.Close();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                if (!run_in_the_background)
                {
                    MyMessageBox mmb = new MyMessageBox(ex.Message, " Отправка неотправленных бонусов ");
                    mmb.ShowDialog();
                }

            }
            catch (Exception ex)
            {
                if (!run_in_the_background)
                {
                    MyMessageBox mmb = new MyMessageBox(ex.Message, " Отправка неотправленных бонусов ");
                    mmb.ShowDialog();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        public class TransactionResponse
        {
            public string transactionId { get; set; }
            public string returnTransactionId { get; set; }
            public string amountSum { get; set; }
            public string discountSum { get; set; }
            public string bonusSum { get; set; }
            public string chargeSum { get; set; }
            public string totalSum { get; set; }
            public string message { get; set; }
            public string requestId { get; set; }
            public string res { get; set; }
            public string error { get; set; }
        }

        public bool fill_items(BuyNewRequest buyNewRequest,string document_number,string client)
        {
            bool result = true;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn() ;
            try
            {
                conn.Open();
                string query = "SELECT numstr, tovar_code, quantity, price,price_at_a_discount,sum_at_a_discount,action_num_doc2 FROM checks_table where document_number =" + document_number+" order by numstr desc";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                buyNewRequest.items = new Items();
                Items items = new Items();
                //List<Item> list_items= new List<Item>();// items2 = new items.item();
                items.item = new List<Item>();// items2 = new items.item();
                while (reader.Read())
                {                    
                    Item item = new Item();
                    item.pos = (Convert.ToInt32(reader["numstr"])+1).ToString();
                    //item.pos = Convert.ToInt32(reader["numstr"]).ToString();
                    item.SKU = reader["tovar_code"].ToString();
                    item.qty = reader["quantity"].ToString();
                    if (client == "")
                    {
                        item.price = Convert.ToInt32((Convert.ToDecimal(reader["price_at_a_discount"]) * 100)).ToString();
                    }
                    else
                    {
                        item.price = Convert.ToInt32((Convert.ToDecimal(reader["price"]) * 100)).ToString();
                    }
                    if (reader["action_num_doc2"].ToString() == "0")
                    {
                        item.productMarketingFlags = "7";
                    }
                    else
                    {
                        item.productMarketingFlags = "0";
                    }

                    //item.discount = Convert.ToInt32(((Convert.ToDecimal(reader["sum_at_a_discount"]) - Convert.ToDecimal(reader["quantity"]) * Convert.ToDecimal(reader["price"]))*100)*-1).ToString();
                    //item.discount = Convert.ToInt32(((Convert.ToDecimal(reader["price_at_a_discount"])- Convert.ToDecimal(reader["price"])) * 100)*-1).ToString();
                    items.item.Add(item);
                    //list_items.Add(item);
                    //buyNewrequest.items.Add(item);
                }
                //items.item = list_items;
                buyNewRequest.items = items;
            }
            catch (NpgsqlException ex)
            {
                result = false;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }

        public bool fill_items(BuyReturnRequest buyReturnRequest, string document_number, string client)
        {
            bool result = true;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT numstr, tovar_code, quantity, price,price_at_a_discount,sum_at_a_discount FROM checks_table where document_number =" + document_number + " order by numstr desc";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                buyReturnRequest.items = new Items();
                Items items = new Items();
                //List<Item> list_items= new List<Item>();// items2 = new items.item();
                items.item = new List<Item>();// items2 = new items.item();
                while (reader.Read())
                {
                    Item item = new Item();
                    item.pos = (Convert.ToInt32(reader["numstr"]) + 1).ToString();
                    //item.pos = Convert.ToInt32(reader["numstr"]).ToString();
                    item.SKU = reader["tovar_code"].ToString();
                    item.qty = reader["quantity"].ToString();
                    if (client == "")
                    {
                        item.price = Convert.ToInt32((Convert.ToDecimal(reader["price_at_a_discount"]) * 100)).ToString();
                    }
                    else
                    {
                        item.price = Convert.ToInt32((Convert.ToDecimal(reader["price"]) * 100)).ToString();
                    }
                    //item.discount = Convert.ToInt32(((Convert.ToDecimal(reader["sum_at_a_discount"]) - Convert.ToDecimal(reader["quantity"]) * Convert.ToDecimal(reader["price"]))*100)*-1).ToString();
                    //item.discount = Convert.ToInt32(((Convert.ToDecimal(reader["price_at_a_discount"])- Convert.ToDecimal(reader["price"])) * 100)*-1).ToString();
                    items.item.Add(item);
                    //list_items.Add(item);
                    //buyNewrequest.items.Add(item);
                }
                //items.item = list_items;
                buyReturnRequest.items = items;
            }
            catch (NpgsqlException ex)
            {
                result = false;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return result;
        }


        #region buynewResponse

        public class Balance
        {
            public string activeBalance { get; set; }
            public string inactiveBalance { get; set; }
        }

        public class BuynewResponse
        {
            public string transactionId { get; set; }
            public string date { get; set; }
            public string amountSum { get; set; }
            public string discountSum { get; set; }
            public string bonusSum { get; set; }
            public string chargeSum { get; set; }
            public string totalSum { get; set; }
            public Balance balance { get; set; }
            public string result { get; set; }
            public string description { get; set; }
            public string message { get; set; }
            public string resultComment { get; set; }
            public string res { get; set; }
        }

        #endregion


        #region buyReturn
        public class BuyReturnRequest
        {
            public string transactionId { get; set; }
            //date – дата транзакции(необязательный параметр);
            public string date { get; set; }
            //cashierName – данные кассира(имя, фамилия);
            public string cashierName { get; set; }
            //items  –  блок с  перечнем товаров  к возврату, аналогичен  перечислению товаров в запросе buyNew;
            public Items items { get; set; }
            //commit   –  завершать ли  транзакцию.Необязательный параметр, может принимать значения ‘1’ или ‘0’. По умолчанию значение ‘0’.
            public string commit { get; set; }

        }
        #endregion


        #region buyNewrequest
        public class BuyNewRequest
        {
            //cardNum – номер карты(с ведущими нулями)
            public string cardNum { get; set; }
            // SHA256 хэш PIN-кода карты
            public string cardTrack2 { get; set; }
            public string phone { get; set; }
            //date – дата и время совершения транзакции     
            public string date { get; set; }
            //type – тип транзакции.Может принимать следующие значения:
            //1– покупка со скидкой
            //2– покупка с начислением бонусов
            //3– покупка со списанием бонусов
            //4– покупка с начислением и списанием
            //5- пополнение предоплаченной карты
            //6– покупка без карты (значение type по умолчанию)
            public string type { get; set; }            
            //public Items items { get; set; }
            //total – общая стоимость покупки.Передается в случае отсутствия блока items, либо  при предоставлении дополнительной скидки на стороне кассы.Значение поля total имеет приоритет над суммарной стоимостью позиций items.
            public string total { get; set; }
            //commit – завершать ли транзакцию.Необязательный параметр, может принимать значения ‘1’ или ‘0’. По умолчанию значение ‘0’. При значении ‘1’ транзакция завершается непосредственно при выполнении запроса, при значении ‘0’ для завершения требуется вызов buyCommit.
            public string commit { get; set; }
            public string cashierName { get; set; }
            //authCode – код подтверждения списания(при наличии), необязательный параметр
            public string authCode { get; set; }
            //options    -  дополнительные данные  транзакции(опции, тип доставки, и т.д.), варианты значений оговариваются отдельно.
            public string options { get; set; }
            //oddMoney - изменение счета "мелочь на карту", положительное значение для начисления, отрицательное для списания(опционально)
            public string oddMoney { get; set; }
            //coupons - перечисление купонов(опционально)
            public string coupons { get; set; }
            //items – блок перечисления позиций в чеке
            //public List<Item> items { get; set; } //= new List<Item>();
            public Items items { get; set; }
            //charge – сумма для снятия, положительное значение для покупки со снятием бонусов, для покупки со снятием с предоплаченного счета, (необязательный параметр)
            public string charge { get; set; }
            
        }

        public class Items
        {
            public List<Item> item { get; set; }
        }

        public class Item
        {
            //pos - номер позиции в чеке
            public string pos { get; set; }
            //SKU – артикул товара    
            public string SKU { get; set; }
            //price – цена за единицу товара(в копейках)
            public string price { get; set; }
            //qty – количество товара(штук/единиц объема/массы)
            public string qty { get; set; }
            //discount - скидка на позицию(в денежном выражении)
            public string discount { get; set; }
            public string productMarketingFlags { get; set; }            
        }
        #endregion
       


        private void _close__Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
