using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Npgsql;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Threading;
using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace Cash8
{




    class MainStaticClass
    {
        //public static Cash_check cc = null;
        //public static 

        //public static string url = "http://192.168.0.220:16732/requests";
        //public static string url = "http://localhost:16732/requests";
        //public static string url = "http://127.0.0.1:16732/requests";
        public static string url = "http://" + get_ip_adress() + ":16732/requests";
        //public static string url = "http://192.168.0.96:16732/requests";

        public static string shablon = "{uuid,\"request\": [body]}\"";

        private static bool fiscal_print;
        //private static int bonus_treshold = 0;
        //public static ListView listview_print;
        //public static double sum_print;

        private static byte[] EncryptedSymmetricKey = { 214, 46, 220, 83, 160, 73, 40, 39, 201, 155, 19, 202, 3, 11, 191, 178, 56, 74, 90, 36, 248, 103, 18, 144, 170, 163, 145, 87, 54, 61, 34, 220 };
        private static byte[] EncryptedSymmetricIV = { 207, 137, 149, 173, 14, 92, 120, 206, 222, 158, 28, 40, 24, 30, 16, 175 };
        private static string ipAdrServer = null;
        private static string dataBaseName = null;
        private static string portServer = null;
        private static string postgresUser = null;
        private static string passwordPostgres = null;
        static DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        //private static string codebase = null;
        private static string codekey = null;
        //private static bool databaseIsCentral;
        private static Int16 cashDeskNumber = 0;
        private static Int16 code_right_of_user = 0;
        private static Main main = null;
        private static string nick_shop = "";
        private static string code_shop = "";
        private static string cash_operator = "";
        //private static string cash_operator_nick = "";//Пока не используется Фио кассира 
        public static string cash_operator_inn { get; set; }

        private static string cash_operator_client_code = "";
        private static bool result_fiscal_print;

        private static DateTime last_answer_barcode_scaner;
        private static ArrayList forms = new ArrayList();

        private static string pass_promo = "";

        private static bool first_fogin_admin = false;

        //private static Int16 fiscal_num_port = -1;
        //private static string fiscal_type_port = "";
        private static string firma = "";
        private static string inn = "";
        //private static int use_trassir = -1;
        //private static string ip_addr_trassir = "";
        //private static int ip_port_trassir = -1;
        private static string path_for_web_service = "";
        //private static int show_before_payment_window = -1;
        //private static int start_sum_opt_price = -1;
        private static bool use_envd = false;
        private static int system_taxation = 0;
        private static DateTime last_send_last_successful_sending;
        private static DateTime last_write_check;
        private static DateTime min_date_work = new DateTime(2021, 1, 1);

        //private static bool use_text_print;
        //private static int width_of_symbols;
        //private static string barcode = "";
        //private static bool use_usb_to_com_barcode_scaner;

        //public enum TypeAction {ip,poi};
        //{
        //enum Types_of_actions { }
        //}

        private static string barcode = "";

        public static bool continue_to_read_the_data_from_a_port = false;


        //private static int BonusTreshold
        //{
        //    get
        //    {
        //        if (bonus_treshold == 0)
        //        {
        //            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //            try
        //            {
        //                conn.Open();
        //                string query = "SELECT threshold  FROM constants";
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                bonus_treshold = Convert.ToInt32(command.ExecuteScalar());
        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message);
        //            }
        //            finally
        //            {
        //                if (conn.State == ConnectionState.Open)
        //                {
        //                    conn.Close();
        //                }
        //            }
        //        }
        //        return bonus_treshold;
        //    }
        //}


        public static bool validate_cash_sum_non_cash_sum_on_return(int id_sale, decimal cash_summ,decimal non_cash_sum)
        {
            bool result = true;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = " SELECT SUM(d_c.cash_money)AS cash_money,SUM(d_c.non_cash_money)AS non_cash_money FROM" +
                              " (SELECT cash_money, non_cash_money FROM checks_header where document_number = "+ id_sale.ToString()+
                              "  AND checks_header.check_type = 0 AND checks_header.its_deleted = 0 "+
                              " AND checks_header.date_time_write BETWEEN '" + 
                              DateTime.Now.AddDays(-14).Date.ToString("dd-MM-yyyy") + "' AND  '" + DateTime.Now.AddDays(1).ToString("dd-MM-yyyy") + "'" +
                              " UNION ALL " +
                              " SELECT - coalesce(SUM(cash_money),0), - coalesce(SUM(non_cash_money),0) FROM checks_header where id_sale = " + id_sale.ToString()+
                              " AND checks_header.check_type = 1 AND checks_header.its_deleted = 0 "+
                              " AND checks_header.date_time_write BETWEEN '" + 
                              DateTime.Now.AddDays(-14).Date.ToString("dd-MM-yyyy") + "' AND  '" + DateTime.Now.AddDays(1).ToString("dd-MM-yyyy") + "') AS d_c ";//--delta_calculations

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();                
                while (reader.Read())
                {
                    if (Convert.ToDecimal(reader["cash_money"]) < cash_summ)
                    {
                        result = false;
                        MessageBox.Show("Вы можете вернуть наличными не более "+ Convert.ToDecimal(reader["cash_money"]).ToString());
                    }
                    if (Convert.ToDecimal(reader["non_cash_money"]) < non_cash_sum)
                    {
                        result = false;
                        MessageBox.Show("Вы можете вернуть по безналу не более " + Convert.ToDecimal(reader["non_cash_money"]).ToString());
                    }
                }

            }
            catch (NpgsqlException ex)
            {
                result = false;
                MessageBox.Show("Ошибка при определении корректности суммы возврата по видам оплаты " + ex.Message);
            }
            catch(Exception ex)
            {
                result = false;
                MessageBox.Show("Ошибка при определении корректности суммы возврата по видам оплаты " + ex.Message);
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
    

        public static DateTime GetMinDateWork
        {
            get
            {
                return min_date_work;
            }
        }

        //public static bool check_amount_exceeds_threshold(Decimal check_amount)
        //{
        //    bool result = false;

        //    if (MainStaticClass.BonusTreshold > 0)
        //    {
        //        if (check_amount >= MainStaticClass.BonusTreshold)
        //        {
        //            result = true;
        //        }
        //    }

        //    return result;
        //}

        public static int ckeck_failed_input_phone_on_client(string client_code)
        {
            int result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM failed_input_phone where client_code='" + client_code + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt16(command.ExecuteScalar());
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при определении количества попыток ввода неправильного номера телефона");
                result = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при определении количества попыток ввода неправильного номера телефона");
                result = -1;
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


        public static DateTime Last_Write_Check
        {
            get
            {
                return last_write_check;
            }
            set
            {
                last_write_check = value;
            }
        }

        public static DateTime Last_Send_Last_Successful_Sending
        {
            get
            {
                return last_send_last_successful_sending;
            }
            set
            {
                last_send_last_successful_sending = value;
            }
        }

        public static bool get_exists_internet()
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send("8.8.8.8");                
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {

            }

            return pingable;
        }


        public static Cash8.FiscallPrintJason.RootObject get_ofd_exchange_status()
        {

            Cash8.FiscallPrintJason.RootObject result = null;

            try
            {
                result = FiscallPrintJason.execute_operator_type("ofdExchangeStatus");
                //if (result != null)
                //{
                //    if (result.results[0].status == "ready")//Задание выполнено успешно 
                //    {
                //        //string s = result.results[0].result.status.notSentFirstDocDateTime.ToString("yyyy-MM-dd HH:mm:ss");
                //        //result.results[0].result.status
                //        //Invoke(new set_message_on_ofd_exchange_status(set_txtB_ofd_exchange_status), new object[] { s });
                //    }
                //    else
                //    {
                //        //string s = result.results[0].status + " | " + result.results[0].errorDescription;
                //        //Invoke(new set_message_on_ofd_exchange_status(set_txtB_ofd_exchange_status), new object[] { s });
                //    }
                //}
                //else
                //{
                //    //Invoke(new set_message_on_ofd_exchange_status(set_txtB_ofd_exchange_status), new object[] { "Общая ошибка" });
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }



        public static string get_ip_adress()
        {
            // Получение имени компьютера.
            String host = System.Net.Dns.GetHostName();
            // Получение ip-адреса.
            System.Net.IPAddress ip = System.Net.Dns.GetHostByName(host).AddressList[0];
            return ip.ToString();
        }


        public static void delete_old_checks(DateTime date)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT Max(document_number)  FROM checks_header where date_time_start<'" + date.ToString("yyyy.MM.dd") + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar();
                if (result_query.ToString() != "")
                {
                    query = "DELETE FROM checks_header where document_number<=" + Convert.ToInt64(result_query).ToString()+ " AND is_sent=1";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    //query = "DELETE FROM checks_table LEFT JOIN checks_header ON checks_table.document_number = checks_header.document_number  where document_number<=" + Convert.ToInt64(result_query).ToString()+ " AND is_sent = 1";
                    //query = "DELETE FROM checks_table ct  USING checks_header ch Where ct.document_number = ch.document_number  AND ct.document_number <=" + Convert.ToInt64(result_query).ToString() + " AND ch.is_sent = 1";
                    query = "DELETE FROM checks_table Where document_number <=" + Convert.ToInt64(result_query).ToString();
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                command.Dispose();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибка при удалении документов с датой до " + date.ToString("yyyy.MM.dd") + " " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        public static string get_string_message_for_trassir(string event_type, string operation_id, string cashier, string date, string time,
                    string position, string quantity, string price, string barcode, string article, string location, string text)
        {
            string result = "";

            result = "<?xml version=" + '"' + "1.0" + '"' + " encoding=" + '"' + "windows-1251" + '"' + "?>" + "\r\n";
            result += "<transaction>" + "\r\n";
            result += "<event_type>" + event_type + "</event_type>" + "\r\n";
            result += "<operation_id>" + operation_id + "</operation_id>" + "\r\n";

            if (cashier != "")
            {
                result += "<cashier>" + cashier + "</cashier>" + "\r\n";
            }
            result += "<date>" + date + "</date>" + "\r\n";
            result += "<time>" + time + "</time>" + "\r\n";
            if (position != "")
            {
                result += "<position>" + position + "</position>" + "\r\n";
            }
            if (quantity != "")
            {
                result += "<quantity>" + quantity + "</quantity>" + "\r\n";
            }
            if (price != "")
            {
                result += "<price>" + price + "</price>" + "\r\n";
            }
            if (barcode != "")
            {
                result += "<barcode>" + barcode + "</barcode>" + "\r\n";
            }
            if (article != "")
            {
                result += "<article>" + article + "</article>" + "\r\n";
            }
            if (location != "")
            {
                result += "<location>" + location + "</location>" + "\r\n";
            }
            if (text != "")
            {
                result += "<text>" + text + "</text>" + "\r\n";
            }

            result += "</transaction>";

            return result;
        }


        /*
        public static void write_event_dssl_in_log(
            string event_type,
            string operation_id,
            string cashier,
            DateTime date,           
            TimeSpan time,
            int position,
            int quantity,
            decimal price,
            string barcode,
            string article,
            string location,//Номер кассы
            string text)//Ошибка

            
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "INSERT INTO dssl_log(event_type,operation_id, cashier, date, time, position,quantity, price, barcode, article, location, text)"+
                    " VALUES (@event_type,@operation_id,@cashier,@date,@time,@position,@quantity,@price,@barcode,@article,@location,@text);";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);

                NpgsqlParameter _event_type = new NpgsqlParameter("event_type", DbType.String);
                _event_type.Value = event_type;

                NpgsqlParameter _operation_id = new NpgsqlParameter("operation_id", DbType.String);
                _operation_id.Value = operation_id;

                NpgsqlParameter _cashier = new NpgsqlParameter("cashier", DbType.String);
                _cashier.Value = cashier;

                NpgsqlParameter _date = new NpgsqlParameter("date", DbType.Date);
                _date.Value = date;

                NpgsqlParameter _time = new NpgsqlParameter("time", DbType.Time);
                _time.Value = time;

                NpgsqlParameter _position = new NpgsqlParameter("position", DbType.Int32);
                _position.Value = position;

                NpgsqlParameter _quantity = new NpgsqlParameter("quantity", DbType.Int32);
                _quantity.Value = quantity;

                NpgsqlParameter _price = new NpgsqlParameter("price", DbType.Decimal);
                _price.Value = price;

                NpgsqlParameter _barcode = new NpgsqlParameter("barcode", DbType.String);
                _barcode.Value = barcode;

                NpgsqlParameter _article = new NpgsqlParameter("article", DbType.String);
                _article.Value = article;

                NpgsqlParameter _location = new NpgsqlParameter("location", DbType.Int16);
                _location.Value = location;

                NpgsqlParameter _text = new NpgsqlParameter("text", DbType.String);
                _text.Value = text;

                command.Parameters.Add(_event_type);
                command.Parameters.Add(_operation_id);
                command.Parameters.Add(_cashier);
                command.Parameters.Add(_date);
                command.Parameters.Add(_time);
                command.Parameters.Add(_position);
                command.Parameters.Add(_quantity);
                command.Parameters.Add(_price);
                command.Parameters.Add(_barcode);
                command.Parameters.Add(_article);
                command.Parameters.Add(_location);
                command.Parameters.Add(_text);

                

                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибка при записи логов dssl "+ex.Message + " | " + ex.Detail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибка при записи логов dssl " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
         */



        public static bool Use_Envd
        {
            get
            {
                return use_envd;
            }
            set
            {
                use_envd = value;
            }
        }

        public static int SystemTaxation
        {
            get
            {
                return system_taxation;
            }
            set
            {
                system_taxation = value;
            }
        }
        //

        //public static int Start_sum_opt_price
        //{
        //    get
        //    {
        //        if (start_sum_opt_price == -1)
        //        {
        //            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //            try
        //            {
        //                conn.Open();
        //                string query = "SELECT start_sum_opt_price FROM constants";
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                object result_query = command.ExecuteScalar();
        //                if (result_query.ToString() != "")
        //                {
        //                    start_sum_opt_price = Convert.ToInt32(result_query);
        //                }
        //                else
        //                {
        //                    start_sum_opt_price = 0;
        //                }
        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show("Ошибка при получении суммы включения оптового прайса ");
        //                start_sum_opt_price = 0;
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show("Ошибка при получении суммы включения оптового прайса ");
        //                start_sum_opt_price = 0;
        //            }
        //            finally
        //            {
        //                if (conn.State == ConnectionState.Open)
        //                {
        //                    conn.Close();
        //                }
        //            }
        //        }
        //        return start_sum_opt_price;
        //    }
        //}


        //public static int Show_Before_Payment_Window
        //{
        //    get
        //    {
        //        if (show_before_payment_window == -1)
        //        {

        //            NpgsqlConnection conn = null;
        //            string query = "SELECT show_before_payment_window  FROM constants;";
        //            try
        //            {
        //                conn = MainStaticClass.NpgsqlConn();
        //                conn.Open();
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                object result_query = command.ExecuteScalar();
        //                if (result_query.ToString() != "")
        //                {
        //                    show_before_payment_window = Convert.ToInt16(result_query);
        //                }

        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при получении признака показа промежуточного окна");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "Ошибка при получении признака показа промежуточного окна");
        //            }
        //            if (conn.State == ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }

        //        }

        //        return show_before_payment_window; 
        //    }
        //}

        //public static int Use_Trassir
        //{
        //    get
        //    {
        //        if (use_trassir == -1)
        //        {
        //            NpgsqlConnection conn = null;
        //            string query = "SELECT use_trassir  FROM constants;";
        //            try
        //            {
        //                conn = MainStaticClass.NpgsqlConn();
        //                conn.Open();
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                object result_query = command.ExecuteScalar();
        //                if (result_query.ToString()!="")
        //                {
        //                    use_trassir = Convert.ToInt16(result_query);
        //                }

        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при получении признака работы трассира");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "Ошибка при получении признака работы трассира");
        //            }
        //            if (conn.State == ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }
        //        }
        //        return use_trassir;
        //    }
        //}


        //public static string Ip_Addr_Trassir
        //{

        //    get
        //    {
        //        if (ip_addr_trassir == "")
        //        {
        //            NpgsqlConnection conn = null;
        //            string query = "SELECT ip_addr_trassir  FROM constants;";
        //            try
        //            {
        //                conn = MainStaticClass.NpgsqlConn();
        //                conn.Open();
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                ip_addr_trassir = command.ExecuteScalar().ToString();
        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при получении ip адреса трассира");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "Ошибка при получении ip адреса трассира");
        //            }
        //            finally
        //            {
        //                if (conn.State == ConnectionState.Open)
        //                {
        //                    conn.Close();
        //                }
        //            }
        //        }
        //        return ip_addr_trassir.Replace(",",".");
        //    }
        //    set
        //    {
        //        ip_addr_trassir = value;
        //    }

        //}

        //public static int Ip_Port_Trassir
        //{

        //    get
        //    {
        //        if (ip_port_trassir == -1)
        //        {
        //            NpgsqlConnection conn = null;
        //            string query = "SELECT ip_port_trassir  FROM constants;";
        //            try
        //            {
        //                conn = MainStaticClass.NpgsqlConn();
        //                conn.Open();
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                ip_port_trassir = Convert.ToInt16(command.ExecuteScalar());
        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при получении порта отправки сообщений для трассира");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "Ошибка при получении порта отправки сообщений для трассира");
        //            }
        //            finally
        //            {
        //                if (conn.State == ConnectionState.Open)
        //                {
        //                    conn.Close();
        //                }
        //            }
        //        }
        //        return ip_port_trassir;
        //    }

        //}



        //public static Int16 Fiscal_Num_Port
        //{
        //    get
        //    {
        //        if (fiscal_num_port == -1)
        //        {
        //            NpgsqlConnection conn = null;
        //            string query = "SELECT fiscal_num_port  FROM constants;";
        //            try
        //            {
        //                conn = MainStaticClass.NpgsqlConn();
        //                conn.Open();
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                fiscal_num_port = Convert.ToInt16(command.ExecuteScalar());
        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при получении параметров фискального принтера");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "Ошибка при получении параметров фискального принтера");
        //            }
        //            if (conn.State == ConnectionState.Open)
        //            {
        //                conn.Close();
        //            }

        //            return fiscal_num_port;
        //        }
        //        else
        //        {
        //            return fiscal_num_port;
        //        }

        //    }
        //    set
        //    {
        //        fiscal_num_port = value;
        //    }
        //}

        //public static string Fiscal_Type_Port
        //{
        //    get
        //    {
        //        if (fiscal_type_port == "")
        //        {
        //            NpgsqlConnection conn = null;
        //            string query = "SELECT fiscal_type_port  FROM constants;";
        //            try
        //            {
        //                conn = MainStaticClass.NpgsqlConn();
        //                conn.Open();
        //                NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //                fiscal_type_port = command.ExecuteScalar().ToString();
        //                conn.Close();
        //            }
        //            catch (NpgsqlException ex)
        //            {
        //                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при получении параметров фискального принтера");
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(ex.Message, "Ошибка при получении параметров фискального принтера");
        //            }

        //            return fiscal_type_port;

        //        }
        //        else
        //        {
        //            return fiscal_type_port;
        //        }
        //    }
        //    set
        //    {
        //        fiscal_type_port = value;
        //    }
        //}




        public static bool First_Login_Admin
        {
            get
            {
                return first_fogin_admin;
            }
            set
            {
                first_fogin_admin = value;
            }
        }

        /// <summary>
        /// Обновить статус в документе данные по накопленному бонусу отправлены успешно
        /// 
        /// </summary>
        public static bool update_status_sent_bonus(string num_doc)
        {

            bool result = true;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " UPDATE checks_header SET discount_it_is_sent=true WHERE document_number=" + num_doc;
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                //MyMessageBox mmb = new MyMessageBox(ex.Message, "Попытка обновить статус документа отправлено для документа " + num_doc);
                result = false;
            }
            catch (Exception ex)
            {
                //MyMessageBox mmb = new MyMessageBox(ex.Message, "Попытка обновить статус документа отправлено для документа " + num_doc);
                //mmb.ShowDialog();
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
        /// получает строкове 
        /// представление текущей валюты
        /// </summary>
        /// <returns></returns>
        public static string get_currency()
        {

            string result = "";

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT currency  FROM constants;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToString(command.ExecuteScalar());
                conn.Close();

            }
            catch (NpgsqlException)
            {
                MyMessageBox mmb = new MyMessageBox("Ошибка при получении валюты", "Ошибка при получении валюты");
                mmb.ShowDialog();
            }
            catch (Exception)
            {
                MyMessageBox mmb = new MyMessageBox("Ошибка при получении валюты", "Ошибка при получении валюты");
                mmb.ShowDialog();
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

        public static string version()
        {
            return System.Windows.Forms.Application.ProductVersion;
        }



        private static string get_device_info()
        {
            string string_get_device_info = string.Empty;

            try
            {
                Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("getDeviceInfo");
                if (result != null)
                {
                    if (result.results[0].status == "ready")//Задание выполнено успешно 
                    {
                        string_get_device_info = JsonConvert.SerializeObject(result.results[0].result.deviceInfo, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        //string_get_device_info = result.results[0].result.deviceInfo.ToString();
                    }
                    else
                    {
                        MessageBox.Show(" Ошибка !!! " + result.results[0].status + " | " + result.results[0].errorDescription);
                    }
                }
                else
                {
                    MessageBox.Show("Общая ошибка");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("getDeviceInfo" + ex.Message);
            }

            return string_get_device_info;
        }

        public class ResultGetData
        {
            public string Successfully { get; set; }
            public string Shop { get; set; }
            public string NumCash { get; set; }
            public string Version { get; set; }
            public string OSVersion { get; set; }
            public string DeviceInfo { get; set; }
        }

        public static bool SendResultGetData()
        {
            bool result = true;

            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + nick_shop.Trim();
            string data_encrypt = "";
            ResultGetData resultGetData = new ResultGetData();
            resultGetData.Successfully = "Successfully";
            resultGetData.Version = MainStaticClass.version().Replace(".", "");
            resultGetData.NumCash = MainStaticClass.CashDeskNumber.ToString();
            resultGetData.OSVersion = Environment.OSVersion.VersionString;
            //Запросим информацию про фискальный регистратор
            resultGetData.DeviceInfo = get_device_info();
            string data = JsonConvert.SerializeObject(resultGetData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            data_encrypt = CryptorEngine.Encrypt(data, true, key);
            using (Cash8.DS.DS ds = MainStaticClass.get_ds())
            {
                ds.Timeout = 60000;
                try
                {
                    ds.GetDataForCasheV8Successfully(nick_shop, data_encrypt);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }


        public static bool SendOnlineStatus()
        {
            bool result = true;

            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + nick_shop.Trim();
            string data_encrypt = "";
            ResultGetData resultGetData = new ResultGetData();
            resultGetData.Successfully = "Successfully";
            resultGetData.Version = MainStaticClass.version().Replace(".", "");
            resultGetData.NumCash = MainStaticClass.CashDeskNumber.ToString();
            string data = JsonConvert.SerializeObject(resultGetData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            data_encrypt = CryptorEngine.Encrypt(data, true, key);
            using (Cash8.DS.DS ds = MainStaticClass.get_ds())
            {
                ds.Timeout = 60000;
                try
                {
                    ds.OnlineCasheV8Successfully(nick_shop, data_encrypt);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public static string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static int check_new_shema_autenticate()
        {
            int result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT COUNT(*)FROM information_schema.columns where table_name='users' and column_name='rights'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt16(command.ExecuteScalar());
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошика при определении схемы " + ex.Message);
                result = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошика при определении схемы " + ex.Message);
                result = -1;
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
        /// получение признака ведения учета в 2 валютах
        /// </summary>
        /// <returns></returns>
        public static bool get_account_two_currencies()
        {
            bool result = false;


            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT two_currencies  FROM constants;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToBoolean(command.ExecuteScalar());
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MyMessageBox mmb = new MyMessageBox("Ошибка при получении валюты", "Ошибка при получении валюты");
                mmb.ShowDialog();
            }
            catch (Exception ex)
            {
                MyMessageBox mmb = new MyMessageBox("Ошибка при получении валюты", "Ошибка при получении валюты");
                mmb.ShowDialog();
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


        public static Cash8.DS.DS get_ds()
        {
            Cash8.DS.DS ds = null;
            ds = new Cash8.DS.DS();
            //ds.Proxy = MainStaticClass.CreateWebProxyWithCredentials("http://proxy.sd2.com.ua:3128", "softupdate", "271828", "Basic", "sd2.com.ua");
            try
            {
                ds.Url = MainStaticClass.PathForWebService;//.get_path_for_web_service();
            }
            catch
            {
                ds.Url = "http://ch.sd2.com.ua/DiscountSystem/Ds.asmx";//.get_path_for_web_service();
            }

            

            return ds;
        }
        
        
        private static DateTime  get_datetime_on_server()
        {
            DateTime result = new DateTime(1,1,1);

            if (!MainStaticClass.service_is_worker())
            {
                return result;
            }

            try
            {
                Cash8.DS.DS ds = MainStaticClass.get_ds();
                ds.Timeout = 15000;
                result = ds.GetDateTimeServer();
            }
            catch (Exception)
            {
 
            }

            return result;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int get_documents_not_out()
        {
            int result = 0;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM checks_header WHERE is_sent = 0";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException)
            {
                result = -1;
            }
            catch (Exception)
            {
                result = -1;
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
        /// Количество документов которые находятся
        /// за диапазоном разрешенных дат
        /// </summary>
        /// <returns></returns>
        public static int get_documents_out_of_the_range_of_dates()
        {            
            int result = 0;

            DateTime result_query_datetime_on_server = get_datetime_on_server();
            if (result_query_datetime_on_server == new DateTime(1, 1, 1))
            {
                result = -1;
            }
            else
            {
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM checks_header WHERE (date_time_write<@start_data OR date_time_write>@current_data) AND is_sent = 0";
                    NpgsqlParameter start_data = new NpgsqlParameter("start_data",result_query_datetime_on_server.AddDays(-31));
                    NpgsqlParameter current_data = new NpgsqlParameter("current_data",result_query_datetime_on_server.AddHours(2));
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    command.Parameters.Add(start_data);
                    command.Parameters.Add(current_data);
                    result = Convert.ToInt32(command.ExecuteScalar());
                    conn.Close();
                }
                catch (NpgsqlException)
                {
                    result = -2;
                }
                catch (Exception)
                {
                    result = -2;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
 
                }                
            }                         
            
            return result;
        }

        public static int get_unloading_interval()
        {
            int result = 0;

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT  unloading_period  FROM constants;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            return result;
        }

        public static bool service_is_worker()
        {
            bool result = true;

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 15000;
            try
            {
                result = ds.ServiceIsWorker();
            }
            catch
            {
                result = false;
            }


            return result;
        }


       /* public static void send_data_trassir(string data)
        {
            IPAddress ipAddr = IPAddress.Parse(MainStaticClass.Ip_Addr_Trassir.Replace(",","."));

            try
            {
                // Создаем UdpClient
                UdpClient udpClient = new UdpClient();

                // Соединяемся с удаленным хостом
                udpClient.Connect(ipAddr, Convert.ToInt32(MainStaticClass.Ip_Port_Trassir));

                // Отправка простого сообщения
                Encoding encoding = Encoding.GetEncoding(1251);

                byte[] bytes = encoding.GetBytes(data);
                udpClient.Send(bytes, bytes.Length);

                // Закрываем соединение
                udpClient.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }*/

        //public static void send_data_trassir(string data)
        //{          

        //    try
        //    {              

        //        var client = new TcpClient();
        //        var result = client.BeginConnect(MainStaticClass.Ip_Addr_Trassir, MainStaticClass.Ip_Port_Trassir, null, null);

        //        result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(10));
        //        if (!client.Connected)
        //        {
        //            return;//throw new Exception("Failed to connect.");
        //        }

        //        // we have connected
        //        client.EndConnect(result);
        //        Encoding encoding = Encoding.GetEncoding(1251);                
        //        Byte[] byte_data = encoding.GetBytes(data);
        //        // Get a client stream for reading and writing.
        //        NetworkStream stream = client.GetStream();
        //        // Send the message to the connected TcpServer.  
        //        stream.WriteTimeout = 500;
        //        stream.Write(byte_data, 0, byte_data.Length);
        //        client.Close();
        //        stream.Dispose();
        //    }
        //    catch (SocketException)
        //    {
        //       // MessageBox.Show("trassir " + ex.Message);
        //    }
        //    catch (Exception)
        //    {
        //       // MessageBox.Show("tressir " + ex.Message);
        //    }
        //}


        public static string PathForWebService
        {
            get
            {

                if (path_for_web_service == "")
                {
                    path_for_web_service = get_path_for_web_service();
                }
                return path_for_web_service;
            }

        }

        /// <summary>
        /// Возвращает путь к веб сервису дисконта
        /// </summary>
        /// <returns></returns>
        private static string get_path_for_web_service()
        {
            string result = "";

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = " SELECT path_for_web_service  FROM constants ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result = reader[0].ToString();
                }
                reader.Close();
                reader.Dispose();
                command.Dispose();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MyMessageBox mmb = new MyMessageBox(ex.Message, "Получение пути веб сервиса дисконта");
                mmb.ShowDialog();                
            }
            catch (Exception ex)
            {
                MyMessageBox mmb = new MyMessageBox(ex.Message, "Получение пути веб сервиса дисконта");
                mmb.ShowDialog();                                
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


        public static bool two_currencies()
        {
            bool result = false;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                string query = "SELECT two_currencies  FROM constants;";
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToBoolean(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        /// Если пароль заполнен, значит бонусная программа для этой кассы включена
        /// </summary>
        public static string PassPromo
        {
            get
            {

                if (pass_promo == "")
                {


                    NpgsqlConnection conn = null;

                    try
                    {
                        conn = MainStaticClass.NpgsqlConn();
                        conn.Open();
                        string query = "SELECT pass_promo  FROM constants;";
                        NpgsqlCommand command = new NpgsqlCommand(query, conn);
                        pass_promo = command.ExecuteScalar().ToString().Trim();
                        conn.Close();
                    }
                    catch (NpgsqlException ex)
                    {
                        MessageBox.Show(ex.Message, " Ошибка при определении включения бонусов  ");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, " Ошибка при определении включения бонусов ");
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }

                    return pass_promo;

                }
                else
                {
                    return pass_promo;
                }               
            }
            

        }

        /// <summary>
        /// Возвращает путь к папке обмена с главным компом
        /// 
        /// </summary>
        /// <returns></returns>
        public static string get_change_path_for_main_computer()
        {
            string result = "";

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT change_path_for_main_computer  FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = command.ExecuteScalar().ToString().Trim();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, " Получение номера принтера ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " Получение номера принтера ");
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


        public static decimal get_rate()
        {
            decimal result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " SELECT rate  FROM constants;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar();
                if (result_query != null)
                {
                    result = Convert.ToDecimal(result_query);
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MyMessageBox mmb = new MyMessageBox(ex.Message, "Получение курса");
                mmb.ShowDialog();
            }
            catch (Exception ex)
            {
                MyMessageBox mmb = new MyMessageBox(ex.Message, "Получение курса");
                mmb.ShowDialog();
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




        public static float Font_list_view()
        {
            float result = 0;
            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string select_query = "SELECT size_font_listview FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(select_query, conn);
                result = Convert.ToSingle(command.ExecuteScalar());
                conn.Close();
            }
            catch
            {

            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            if (result == 0)
            {
                result = 12;
            }
            return result;
        }

        public static void add_window(object form)
        {
            if (!forms.Contains(form))
            {
                forms.Add(form);
            }
        }

        public static bool Result_Fiscal_Print
        {
            get
            {
                return result_fiscal_print;
            }
            set
            {
                result_fiscal_print = value;
            }
        }



        /// <summary>
        /// Получает индекс принтера
        /// в любом случае возвращает 0
        /// </summary>
        /// <returns></returns>
        public static int get_num_text_pinter()
        {
            int result = 0;

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT num_text_printer  FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                string result_query = command.ExecuteScalar().ToString().Trim();
                conn.Close();
                if (result_query.Length > 0)
                {
                    result = Convert.ToInt16(result_query);
                }
                if (result > PrinterSettings.InstalledPrinters.Count - 1)
                {
                    result = PrinterSettings.InstalledPrinters.Count - 1;
                    MessageBox.Show("В константах неверно указан принтер", "Получение номера принтера");
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, " Получение номера принтера ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " Получение номера принтера ");
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


        //

        public static void delete_all_events_in_log(DateTime date)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "DELETE FROM logs WHERE time_event <= '" + date.ToString("yyyy.MM.dd") + "'";
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public static void delete_events_in_log(string document_number)
        {
            //NpgsqlConnection conn = null;
            //NpgsqlCommand command = null;
            //try
            //{
            //    conn = MainStaticClass.NpgsqlConn();
            //    conn.Open();
            //    string query = "DELETE FROM logs WHERE document_number = '" + document_number + "'";
            //    command = new NpgsqlCommand(query, conn);
            //    command.ExecuteNonQuery();
            //    conn.Close();

            //}
            //catch (NpgsqlException ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    if (conn.State == ConnectionState.Open)
            //    {
            //        conn.Close();
            //    }
            //}
        }

        //public static string read_last_sell_guid()
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    string result = "";
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();                
        //        string query = "SELECT guid FROM last_guid;";
        //        command = new NpgsqlCommand(query, conn);                
        //        object result_query=command.ExecuteScalar();
        //        if(result_query!=null)
        //        {
        //            result = result_query.ToString();                
        //        }
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message + " | " + ex.Detail);
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


        //public static void write_last_sell_guid(string guid,string num_doc)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    NpgsqlTransaction trans = null;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        trans = conn.BeginTransaction();
        //        string query = "DELETE FROM last_guid;";
        //        command = new NpgsqlCommand(query, conn);
        //        command.Transaction = trans;
        //        command.ExecuteNonQuery();
        //        query = "INSERT INTO last_guid(guid,num_doc)VALUES ('" + guid + "',"+num_doc+");";
        //        command = new NpgsqlCommand(query, conn);
        //        command.Transaction = trans;
        //        command.ExecuteNonQuery();
        //        trans.Commit();
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message + " | " + ex.Detail);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        //write_last_sell_guid

        public static void write_document_wil_be_printed(string document_number)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "INSERT INTO document_wil_be_printed(document_number)VALUES ("+document_number+");";
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        public static void delete_document_wil_be_printed(string document_number)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;            
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "DELETE FROM document_wil_be_printed WHERE document_number=" + document_number + ";";
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }            
        }

        /// <summary>
        /// Получаем признак печатать лир букву m при печати 
        /// маркированного товара
        /// </summary>
        /// <returns></returns>
        public static bool get_print_m()
        {
            bool result = true;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT  print_m	FROM constants;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToBoolean(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибка при получении флага print_m"+ex.Message);
                result = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(" Ошибка при получении флага print_m" + ex.Message);
                result = false;
            }

            return result;

        }

        public static int get_document_wil_be_printed(string document_number)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            int result = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT COUNT(document_number) FROM document_wil_be_printed WHERE document_number=" + document_number + ";";
                command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt16(command.ExecuteScalar());
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
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





        public static void write_event_in_log(string description, string metadata, string document_number)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "INSERT INTO logs(time_event,description,metadata,document_number) VALUES('" + DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "','" + description + "','" + metadata + "','" + document_number + "')";
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }




        public static void remove_window(object form)
        {
            forms.Remove(form);
        }

        /*Эта функция возвращает true если такая форма уже открыта
         * иначе ложь 
         */
        public static bool exist_form(object form)
        {
            return forms.Contains(form);
        }





        public static bool Fiscal_Print
        {
            get
            {
                return fiscal_print;
            }
            set
            {
                fiscal_print = value;
            }
        }

        public static DateTime Last_Answer_Barcode_Scaner
        {
            get
            {
                return last_answer_barcode_scaner;
            }
            set
            {
                last_answer_barcode_scaner = value;
            }
        }

        public static string NumberDecimalSeparator()
        {
            return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        }

        public static string Barcode
        {
            get
            {
                return barcode;
            }
            set
            {
                lock (barcode)
                {
                    barcode = value;
                }
                //if (cc != null)
                //{
                //    cc.find_barcode_or_code_in_tovar(value);
                //}
            }
        }

        public static string Name_Com_Port
        {
            get
            {
                string result = "";
                NpgsqlConnection conn = null;
                try
                {
                    conn = MainStaticClass.NpgsqlConn();
                    conn.Open();
                    string query = "select name_com_port FROM constants";
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    result = Convert.ToString(command.ExecuteScalar());
                    conn.Close();
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(ex.Message, " Ошибка при работе с базой данных");
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
        }

        public static bool Use_Usb_to_Com_Barcode_Scaner
        {
            get
            {
                bool result = false;
                //NpgsqlConnection conn = null;
                //try
                //{
                //    conn = MainStaticClass.NpgsqlConn();
                //    conn.Open();
                //    string query = "select use_usb_to_com_barcode_scaner FROM constants";
                //    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                //    result = Convert.ToBoolean(command.ExecuteScalar());
                //    conn.Close();
                //}
                //catch (NpgsqlException ex)
                //{
                //    MessageBox.Show(ex.Message + " | " + ex.Detail, " Ошибка при работе с базой данных");
                //}
                //finally
                //{
                //    if (conn.State == ConnectionState.Open)
                //    {
                //        conn.Close();
                //    }
                //}
                return result;
            }

        }


        //public static string Barcode
        //{
        //    get
        //    {
        //        return barcode;
        //    }
        //    set
        //    {
        //        barcode = value;
        //    }
        //}


        public static bool Use_Fiscall_Print
        {
            get
            {
                bool result = true;
                //NpgsqlConnection conn = null;
                //try
                //{
                //    conn = MainStaticClass.NpgsqlConn();
                //    conn.Open();
                //    string query = "select use_fiscal_print FROM constants";
                //    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                //    result = Convert.ToBoolean(command.ExecuteScalar());
                //    conn.Close();
                //}
                //catch (NpgsqlException ex)
                //{
                //    MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при работе с базой данных");
                //}
                //finally
                //{
                //    if (conn.State == ConnectionState.Open)
                //    {
                //        conn.Close();
                //    }
                //}
                return result;
            }
        }




        public static bool Use_Text_Print
        {
            get
            {
                bool result = false;
                //NpgsqlConnection conn = null;
                //try
                //{
                //    conn = MainStaticClass.NpgsqlConn();
                //    conn.Open();
                //    string query = "select use_text_print FROM constants";
                //    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                //    result = Convert.ToBoolean(command.ExecuteScalar());
                //    conn.Close();
                //}
                //catch (NpgsqlException ex)
                //{
                //    MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при работе с базой данных");
                //}
                //finally
                //{
                //    if (conn.State == ConnectionState.Open)
                //    {
                //        conn.Close();
                //    }
                //}
                return result;
            }
        }

        public static int Width_Of_Symbols
        {
            get
            {
                int result = 0;
                NpgsqlConnection conn = null;
                try
                {
                    conn = MainStaticClass.NpgsqlConn();
                    conn.Open();
                    string query = "select width_of_symbols FROM constants";
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());
                    conn.Close();
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при работе с базой данных");
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
        }



        public static string Cash_Operator_Client_Code
        {
            get
            {
                return cash_operator_client_code;
            }
            set
            {
                cash_operator_client_code = value;
            }
        }

        public static string Cash_Operator
        {
            get
            {
                return cash_operator;
            }
            set
            {
                cash_operator = value;
            }
        }



        public static string Code_Shop
        {
            get
            {
                if (code_shop == "")
                {
                    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                    try
                    {
                        conn.Open();
                        string queryString = "SELECT code_shop FROM constants";
                        NpgsqlCommand command = new NpgsqlCommand(queryString, conn);
                        code_shop = command.ExecuteScalar().ToString().Trim();
                        conn.Close();
                    }
                    catch (NpgsqlException ex)
                    {
                        MessageBox.Show("Ошибка при получении названия магазина" + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
                
                return code_shop;
            }
        }




        public static string Nick_Shop
        {
            get
            {
                if (nick_shop == "")
                {
                    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                    try
                    {
                        conn.Open();
                        string queryString = "SELECT nick_shop FROM constants";
                        NpgsqlCommand command = new NpgsqlCommand(queryString, conn);
                        nick_shop = (command.ExecuteScalar()).ToString().Trim();
                        conn.Close();
                    }
                    catch (NpgsqlException ex)
                    {
                        MessageBox.Show("Ошибка при получении названия магазина" + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
                return nick_shop;
            }
        }


        public static Main Main
        {
            get
            {
                return main;
            }
            set
            {
                main = value;
            }
        }

        //private static NpgsqlConnection npgsqlconn = null;

        public static NpgsqlConnection NpgsqlConn()
        {
            return new NpgsqlConnection("Server=" + Cash8.MainStaticClass.ipAdrServer + ";Port=" + Cash8.MainStaticClass.portServer + ";User Id=" + Cash8.MainStaticClass.postgresUser + ";Password=" + Cash8.MainStaticClass.PasswordPostgres + ";Database=" + Cash8.MainStaticClass.DataBaseName + ";CommandTimeout=300;Pooling=false");
            //return new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=postgres;Password=a123456789;Database=Cash_Place;CommandTimeout=60;");
            //return new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id=postgres;Password=1;Database=Cash_Place;CommandTimeout=60;");

        }
        public static Int16 Code_right_of_user
        {
            get
            {
                return code_right_of_user;
            }
            set
            {
                code_right_of_user = value;
            }

        }
        public static int CashDeskNumber
        {
            get
            {
                if (cashDeskNumber == 0)
                {
                    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                    try
                    {
                        conn.Open();
                        string queryString = "SELECT cash_desk_number FROM constants";
                        NpgsqlCommand command = new NpgsqlCommand(queryString, conn);
                        cashDeskNumber = Convert.ToInt16(command.ExecuteScalar());
                        conn.Close();
                    }
                    catch (NpgsqlException ex)
                    {
                        MessageBox.Show("Ошибка при получении номера кассы" + ex.Message);
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }                
                
                return cashDeskNumber;
            }
        }


        public static string Firma
        {
            get
            {
                //if (cashDeskNumber == 0)
                //{
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string queryString = "SELECT firma FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(queryString, conn);
                //Int16 rez = 0;
                try
                {
                    firma = Convert.ToString(command.ExecuteScalar());
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(" Ошибка при получении фирмы " + ex.Message);
                }
                conn.Close();
                //if (rez != 0)
                //{
                //    cashDeskNumber = rez;
                //}
                //}
                return firma;
            }
        }

        public static string INN
        {
            get
            {
                //if (cashDeskNumber == 0)
                //{
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string queryString = "SELECT inn FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(queryString, conn);
                //Int16 rez = 0;
                try
                {
                    inn = Convert.ToString(command.ExecuteScalar());
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(" Ошибка при получении инн " + ex.Message);
                }
                conn.Close();
                //if (rez != 0)
                //{
                //    cashDeskNumber = rez;
                //}
                //}
                return inn;
            }
        }


        public static string CodeKey
        {
            get
            {
                if (codekey == null)
                {
                    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                    conn.Open();
                    object rez = null;
                    string queryString = "SELECT guidhash FROM urbd";
                    NpgsqlCommand command = new NpgsqlCommand(queryString, conn);
                    try
                    {
                        rez = command.ExecuteScalar();
                    }
                    catch
                    { }
                    conn.Close();
                    if (rez != null)
                    {
                        codekey = rez.ToString();
                    }
                }
                return codekey;
            }
        }
        //public static string CodeBase
        //{
        //    get
        //    {
        //        if (codebase == null)
        //        {
        //            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //            conn.Open();
        //            object rez = null;
        //            string queryString = "SELECT numbase FROM urbd";
        //            NpgsqlCommand command = new NpgsqlCommand(queryString, conn);
        //            try
        //            {
        //                rez = command.ExecuteScalar();
        //            }
        //            catch
        //            { }
        //            conn.Close();
        //            if (rez != null)
        //            {
        //                codebase = rez.ToString();
        //            }
        //        }
        //        return codebase;
        //    }
        //}
        public static string IPAdrServer
        {
            get
            {
                return ipAdrServer;
            }
            set
            {
                ipAdrServer = value;
            }
        }
        public static string DataBaseName
        {
            get
            {
                return dataBaseName;
            }
            set
            {
                dataBaseName = value;
            }
        }
        public static string PortServer
        {
            get
            {
                return portServer;
            }
            set
            {
                portServer = value;
            }
        }
        public static string PostgresUser
        {
            get
            {
                return postgresUser;
            }
            set
            {
                postgresUser = value;
            }
        }
        public static string PasswordPostgres
        {
            get
            {
                return passwordPostgres;
            }
            set
            {
                passwordPostgres = value;
            }
        }
        public static void EncryptData(String outName, String data)
        {
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);
            //string roundtrip = "";
            //Encoding ascii = Encoding.Default;
            Encoding ascii = Encoding.UTF8;

            byte[] bin = ascii.GetBytes(data);//This is intermediate storage for the encryption.
            int totlen = bin.Length;    //This is the total length of the input file.
            SymmetricAlgorithm rijn = SymmetricAlgorithm.Create(); //Creates the default implementation, which is RijndaelManaged.         
            CryptoStream encStream = new CryptoStream(fout, rijn.CreateEncryptor(EncryptedSymmetricKey, EncryptedSymmetricIV), CryptoStreamMode.Write);
            encStream.Write(bin, 0, totlen);
            encStream.Close();
            fout.Close();
        }
        public static void loadConfig(string fileConfig)
        {
            StringReader sr = MainStaticClass.DecryptData(fileConfig);
            string line = ""; int etap = 0;
            while ((line = sr.ReadLine()) != null)
            {
                if (line == "[ip адрес сервера]")
                {
                    etap = 1;
                    continue;
                }
                if (line == "[имя базы данных]")
                {
                    etap = 2;
                    continue;
                }
                if (line == "[порт сервера]")
                {
                    etap = 3;
                    continue;
                }
                if (line == "[пароль postgres]")
                {
                    etap = 4;
                    continue;
                }
                if (line == "[пользователь postgres]")
                {
                    etap = 5;
                    continue;
                }

                if (etap == 1)
                {
                    Cash8.MainStaticClass.IPAdrServer = line;
                    //this.Text += " | Server = " + line;
                    etap = 0;
                }
                if (etap == 2)
                {
                    Cash8.MainStaticClass.DataBaseName = line;
                    //this.Text += " | DataBase = " + line;
                    etap = 0;
                }
                if (etap == 3)
                {
                    Cash8.MainStaticClass.PortServer = line;
                    //this.Text += " | DataBase = " + line;
                    etap = 0;
                }
                if (etap == 4)
                {
                    Cash8.MainStaticClass.PasswordPostgres = line;
                    //this.Text += " | DataBase = " + line;
                    etap = 0;
                }
                if (etap == 5)
                {
                    Cash8.MainStaticClass.PostgresUser = line;
                    //this.Text += " | DataBase = " + line;
                    etap = 0;
                }
            }
        }
        public static StringReader DecryptData(String inName)
        {
            string roundtrip = "";
            FileStream fin = new FileStream(inName, FileMode.Open, FileAccess.Read);
            SymmetricAlgorithm rijn = SymmetricAlgorithm.Create(); //Creates the default implementation, which is RijndaelManaged.         
            CryptoStream encStream = new CryptoStream(fin, rijn.CreateDecryptor(EncryptedSymmetricKey, EncryptedSymmetricIV), CryptoStreamMode.Read);
            //            Encoding ascii = Encoding.Default;
            Encoding ascii = Encoding.UTF8;
            byte[] bin = new byte[100];      //This is intermediate storage for the encryption.
            int len = 1;                     //This is the number of bytes to be written at a time.
            while (len != 0)
            {
                len = encStream.Read(bin, 0, 100);
                roundtrip = roundtrip + ascii.GetString(bin, 0, len);
            }
            fin.Close();
            StringReader sTrR = new StringReader(roundtrip);
            return sTrR;
        }

        public static bool exists_update_prorgam()
        {
            bool result = false;



            return result; 
        }


        //public static void fiscall_print()
        //{
        //    Mini_FP_6 mini = new Mini_FP_6();            
        //    System.Threading.Thread t = new System.Threading.Thread(delegate() { mini.fiscall_print(MainStaticClass.listview_print,MainStaticClass.sum_print); });
        //    t.Start();
        //    t.Join();
        //}

        //public static void test_print()
        //{
        //    //Test_Mini_FP_6 myPrint = new Test_Mini_FP_6();
        //    //Thread t = new Thread(new ThreadStart(myPrint.Test));
        //    //t.Start();
        //    //t.Join();
        //}


    }
}
