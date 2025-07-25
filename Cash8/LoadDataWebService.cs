﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Npgsql;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Threading;

namespace Cash8
{
    public partial class LoadDataWebService : Form
    {

        public LoadDataWebService()
        {
            InitializeComponent();
            if ((DateTime.Now - last_date_download_tovars()).TotalDays > 100)
            {
                btn_update_only.Enabled = false;
            }
        }


        private void check_temp_tables()
        {

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            conn.Open();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = conn;
            command.CommandText = "select COUNT(*) from information_schema.tables where table_schema='public' and table_name='tovar2'";
            if (Convert.ToInt16(command.ExecuteScalar()) == 0)
            {
                command.CommandText = "CREATE TABLE tovar2(code bigint NOT NULL,name character(100) NOT NULL,  retail_price numeric(10,2) ,purchase_price numeric(10,2) ,its_deleted numeric(1) ,nds integer,its_certificate smallint,percent_bonus numeric(8,2), tnved character varying(10),its_marked smallint,its_excise smallint,cdn_check boolean,fractional boolean NOT NULL DEFAULT false, refusal_of_marking boolean NOT NULL DEFAULT false) WITH (OIDS=FALSE);ALTER TABLE tovar2 OWNER TO postgres;CREATE UNIQUE INDEX _tovar2_code_  ON tovar2  USING btree  (code);";
            }
            else
            {
                command.CommandText = "DROP TABLE tovar2;CREATE TABLE tovar2(code bigint NOT NULL,name character(100) NOT NULL,  retail_price numeric(10,2) ,purchase_price numeric(10,2) ,its_deleted numeric(1),nds integer,its_certificate smallint,percent_bonus numeric(8,2), tnved character varying(10),its_marked smallint,its_excise smallint,cdn_check boolean,fractional boolean NOT NULL DEFAULT false, refusal_of_marking boolean NOT NULL DEFAULT false) WITH (OIDS=FALSE);ALTER TABLE tovar2 OWNER TO postgres;CREATE UNIQUE INDEX _tovar2_code_  ON tovar2  USING btree  (code);";
            }
            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {

            }
        }


        /// <summary>
        /// Влозвращает дату последней синхронизации 
        /// бонусных клиентов
        /// </summary>
        /// <returns></returns>
        public static DateTime last_date_download_tovars()
        {
            DateTime result = new DateTime(2000, 1, 1);

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT tovar FROM date_sync";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object query_result = command.ExecuteScalar();
                if (query_result != null)
                {
                    result = Convert.ToDateTime(query_result);
                }
                conn.Close();
            }
            catch (NpgsqlException)
            {

            }
            catch (Exception)
            {

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
        /// Влозвращает дату последней синхронизации 
        /// бонусных клиентов
        /// </summary>
        /// <returns></returns>
        private DateTime last_date_download_bonus_clients()
        {
            DateTime result = new DateTime(2000, 1, 1);

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT last_date_download_bonus_clients FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object query_result = command.ExecuteScalar();
                if (query_result != null)
                {
                    result = Convert.ToDateTime(query_result);
                }
                conn.Close();
            }
            catch (NpgsqlException)
            {

            }
            catch (Exception)
            {

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

        private void set_last_date_download_bonus_clients()
        {

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "UPDATE constants SET last_date_download_bonus_clients='" + DateTime.Now.Date.ToString("yyy-MM-dd HH:mm:ss") + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при установке даты последнего успешного обновления " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при установке даты последнего успешного обновления " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();

                }
            }
        }

        public class Client
        {
            public string code { get; set; }
            public string phone { get; set; }
            public string name { get; set; }
            public string holiday { get; set; }
            public string use_blocked { get; set; }
            public string reason_for_blocking { get; set; }
            public string notify_security { get; set; }
            public string datetime_update { get; set; }
        }
        public class Clients
        {
            public List<Client> list_clients { get; set; }
        }



        private bool get_load_bonus_clients_on_portions_new(bool show_message)
        {
            bool result = false;


            if (!MainStaticClass.service_is_worker())
            {
                if (show_message)
                {
                    MessageBox.Show("Веб сервис недоступен");
                }
                return result;
            }

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 60000;

            //Получить параметра для запроса на сервер 
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            if (nick_shop.Trim().Length == 0)
            {
                if (show_message)
                {
                    MessageBox.Show(" Не удалось получить название магазина ");
                }
                return result;
            }

            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (code_shop.Trim().Length == 0)
            {
                if (show_message)
                {
                    MessageBox.Show(" Не удалось получить код магазина ");
                }
                return result;
            }
            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
            DateTime dt = last_date_download_bonus_clients();

            string data = CryptorEngine.Encrypt(nick_shop + "|" + dt.Ticks.ToString() + "|" + code_shop, true, key);

            string result_query = "-1";
            try
            {
                //result_query = ds.GetDiscountClientsV8DateTime_NEW(nick_shop, data, MainStaticClass.GetWorkSchema.ToString());
                result_query = ds.GetDiscountClientsV8DateTime_NEW(nick_shop, data, "4");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (result_query == "-1")
            {
                if (show_message)
                {
                    MessageBox.Show("При обработке запроса на сервере произошли ошибки");
                }
                return result;
            }
            string result_query_decrypt = CryptorEngine.Decrypt(result_query, true, key);


            Clients clients = JsonConvert.DeserializeObject<Clients>(result_query_decrypt);

            //string[] delimiters = new string[] { "|" };

            //string[] insert_query = result_query_decrypt.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            //if (insert_query.Length == 0)
            //{
            //    return false;
            //}
            if (clients.list_clients.Count == 0)
            {
                return false;
            }

            progressBar1.Maximum = clients.list_clients.Count;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            NpgsqlConnection conn = null;
            NpgsqlTransaction tran = null;
            string query = "";

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                tran = conn.BeginTransaction();
                NpgsqlCommand command = null;
                //delimiters = new string[] { "," };
                int rowsaffected = 0;
                string local_last_date_download_bonus_clients = "";

                foreach (Client client in clients.list_clients)
                {
                    //string[] str1 = str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    //query=" DELETE FROM clients WHERE code="+str1[0]+";";
                    //query += "INSERT INTO clients(code,name, sum, date_of_birth,discount_types_code,its_work)VALUES(" + str + ")";

                    query = "UPDATE clients SET " +//code='" + client.code + "'," +
                        " phone='" + client.phone + "'," +
                        " name='" + client.name + "'," +
                        " date_of_birth='" + client.holiday + "'," +
                        //" discount_types_code=" + str1[4] + "," +
                        " its_work='" + client.use_blocked + "'," +
                        //" phone=" + client.str1[7] + "," +
                        //" attribute=" + str1[8] + "," +
                        //" bonus_is_on=" + str1[9] + "," +
                        " reason_for_blocking='" + client.reason_for_blocking + "'," +
                        " notify_security='" + client.notify_security + "' " +
                        " WHERE code='" + client.code + "';";

                    local_last_date_download_bonus_clients = client.datetime_update;

                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    rowsaffected = command.ExecuteNonQuery();
                    if (rowsaffected == 0)
                    {
                        query = "INSERT INTO clients(code,phone,name, date_of_birth,its_work,reason_for_blocking,notify_security)VALUES('" +
                            client.code + "','" +
                            client.phone + "','" +
                            client.name + "','" +
                            client.holiday + "','" +
                            client.use_blocked + "','" +
                            client.reason_for_blocking + "','" +
                            client.notify_security + "')";
                        command = new NpgsqlCommand(query, conn);
                        command.Transaction = tran;
                        command.ExecuteNonQuery();
                    }

                    progressBar1.Value++;
                    if (progressBar1.Value % 1000 == 0)
                    {
                        this.Refresh();
                        this.Update();
                        progressBar1.Refresh();
                        progressBar1.Update();
                    }
                }

                query = "UPDATE constants SET last_date_download_bonus_clients='" + local_last_date_download_bonus_clients + "'";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                command.ExecuteNonQuery();

                tran.Commit();
                conn.Close();
                result = true;
                //set_last_date_download_bonus_clients(); теперь делается для каждой строки
                //if (show_message)
                //{
                //    MessageBox.Show("Загрузка успешно завершена");
                //}
            }
            catch (NpgsqlException ex)
            {
                if (show_message)
                {
                    MessageBox.Show(query);
                    MessageBox.Show(ex.Message, "Ошибка при импорте данных ");
                }
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            catch (Exception ex)
            {
                if (show_message)
                {
                    MessageBox.Show(query);
                    MessageBox.Show(ex.Message, "Ошибка при импорте данных " + ex.Message);
                }
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            if (progressBar1.Maximum < 50000)
            {
                result = false;
            }

            return result;
        }


        private bool get_load_bonus_clients_on_portions(bool show_message)
        {
            bool result = false;


            if (!MainStaticClass.service_is_worker())
            {
                if (show_message)
                {
                    MessageBox.Show("Веб сервис недоступен");
                }
                return result;
            }

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 60000;

            //Получить параметра для запроса на сервер 
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            if (nick_shop.Trim().Length == 0)
            {
                if (show_message)
                {
                    MessageBox.Show(" Не удалось получить название магазина ");
                }
                return result;
            }

            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (code_shop.Trim().Length == 0)
            {
                if (show_message)
                {
                    MessageBox.Show(" Не удалось получить код магазина ");
                }
                return result;
            }
            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
            DateTime dt = last_date_download_bonus_clients();

            string data = CryptorEngine.Encrypt(nick_shop + "|" + dt.Ticks.ToString() + "|" + code_shop, true, key);

            string result_query = "-1";
            try
            {
                result_query = ds.GetDiscountClientsV8DateTime_NEW(nick_shop, data,MainStaticClass.GetWorkSchema.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (result_query == "-1")
            {
                if (show_message)
                {
                    MessageBox.Show("При обработке запроса на сервере произошли ошибки");
                }
                return result;
            }
            string result_query_decrypt = CryptorEngine.Decrypt(result_query, true, key);

            string[] delimiters = new string[] { "|" };

            string[] insert_query = result_query_decrypt.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            if (insert_query.Length == 0)
            {
                return false;
            }

            progressBar1.Maximum = insert_query.Length;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;

            NpgsqlConnection conn = null;
            NpgsqlTransaction tran = null;
            string query = "";

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                tran = conn.BeginTransaction();
                NpgsqlCommand command = null;
                delimiters = new string[] { "," };
                int rowsaffected = 0;
                string local_last_date_download_bonus_clients = "";

                foreach (string str in insert_query)
                {
                    string[] str1 = str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    //query=" DELETE FROM clients WHERE code="+str1[0]+";";
                    //query += "INSERT INTO clients(code,name, sum, date_of_birth,discount_types_code,its_work)VALUES(" + str + ")";

                    query = "UPDATE clients SET code=" + str1[0] + "," +
                        " name=" + str1[1] + "," +
                        " sum=" + str1[2] + "," +
                        " date_of_birth=" + str1[3] + "," +
                        //" discount_types_code=" + str1[4] + "," +
                        " its_work=" + str1[5] + "," +
                        " phone=" + str1[7] + "," +
                        " attribute=" + str1[8] + "," +
                        " bonus_is_on=" + str1[9] + "," +
                        " reason_for_blocking='"+str1[10]+"',"+
                        " notify_security=" + str1[11] +
                        " WHERE code=" + str1[0] + ";";

                    local_last_date_download_bonus_clients = str1[6];

                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    rowsaffected = command.ExecuteNonQuery();
                    if (rowsaffected == 0)
                    {
                        //query = "INSERT INTO clients(code,name, sum, date_of_birth,discount_types_code,its_work,phone,attribute,bonus_is_on,reason_for_blocking,notify_security)VALUES(" +
                        query = "INSERT INTO clients(code,name, sum, date_of_birth,its_work,phone,attribute,bonus_is_on,reason_for_blocking,notify_security)VALUES(" +
                            str1[0] + "," +
                            str1[1] + "," +
                            str1[2] + "," +
                            str1[3] + "," +
                            //str1[4] + "," +
                            str1[5] + "," +
                            str1[7] + "," +
                            str1[8] + "," +
                            str1[9] + "," +
                            str1[10]+ "," +
                            str1[11]+")";
                        command = new NpgsqlCommand(query, conn);
                        command.Transaction = tran;
                        command.ExecuteNonQuery();
                    }

                    progressBar1.Value++;
                    if (progressBar1.Value % 1000 == 0)
                    {
                        this.Refresh();
                        this.Update();
                        progressBar1.Refresh();
                        progressBar1.Update();
                    }
                }

                query = "UPDATE constants SET last_date_download_bonus_clients=" + local_last_date_download_bonus_clients;
                command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                command.ExecuteNonQuery();

                tran.Commit();
                conn.Close();
                result = true;
                //set_last_date_download_bonus_clients(); теперь делается для каждой строки
                //if (show_message)
                //{
                //    MessageBox.Show("Загрузка успешно завершена");
                //}
            }
            catch (NpgsqlException ex)
            {                
                if (show_message)
                {
                    MessageBox.Show(query);
                    MessageBox.Show(ex.Message, "Ошибка при импорте данных ");
                }
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            catch (Exception ex)
            {
                if (show_message)
                {
                    MessageBox.Show(query);
                    MessageBox.Show(ex.Message, "Ошибка при импорте данных "+ex.Message);
                }
                if (tran != null)
                {
                    tran.Rollback();
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            if (progressBar1.Maximum < 50000)
            {
                result = false;
            }

            return result;

        }


        public void load_bonus_clients(bool show_message)
        {
            bool loaded = true;

            while (loaded)
            {
                //if (MainStaticClass.GetWorkSchema != 4)
                //{
                //    loaded = get_load_bonus_clients_on_portions(show_message);
                //}
                //else
                //{
                loaded = get_load_bonus_clients_on_portions_new(show_message);
                //}
            }
        }           
        

        /// <summary>        
        /// Класс данных для отправки на кассу        
        /// </summary>
        public class LoadPacketData:IDisposable 
        {
            public int Threshold { get; set; }
            public List<Tovar> ListTovar { get; set; }
            public List<Barcode> ListBarcode { get; set; }
            public List<ActionHeader> ListActionHeader { get; set; }
            public List<ActionTable> ListActionTable { get; set; }
            public List<Characteristic> ListCharacteristic { get; set; }
            public List<Sertificate> ListSertificate { get; set; }
            public List<PromoText> ListPromoText { get; set; }
            public List<ActionClients> ListActionClients { get; set; }
            public bool PacketIsFull { get; set; }//true если пакет заполннен до конца
            public bool Exchange { get; set; }//true если идет обмен
            public string Exception { get; set; }//true если идет обмен
            public string TokenMark { get; set; }

            void IDisposable.Dispose()
            {
                //throw new NotImplementedException();
            }
        }

        public class Tovar
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string RetailPrice { get; set; }
            public string ItsDeleted { get; set; }
            public string Nds { get; set; }
            public string ItsCertificate { get; set; }
            public string PercentBonus { get; set; }
            public string TnVed { get; set; }
            public string ItsMarked { get; set; }
            public string ItsExcise { get; set; }
            public string CdnCheck { get; set; }
            public string Fractional { get; set; }
            public string RefusalOfMarking { get; set; }
        }

        public class Barcode
        {
            public string BarCode { get; set; }
            public string TovarCode { get; set; }
        }
        public class ActionHeader
        {
            public string DateStarted { get; set; }
            public string DateEnd { get; set; }
            public string NumDoc { get; set; }
            public string Tip { get; set; }
            public string Barcode { get; set; }
            public string Persent { get; set; }
            public string sum { get; set; }
            public string sum1 { get; set; }
            public string Comment { get; set; }
            //public string CodeTovar { get; set; }
            public string Marker { get; set; }
            public string ActionByDiscount { get; set; }
            public string TimeStart { get; set; }
            public string TimeEnd { get; set; }
            public string BonusPromotion { get; set; }
            public string WithOldPromotion { get; set; }
            public string Monday { get; set; }
            public string Tuesday { get; set; }
            public string Wednesday { get; set; }
            public string Thursday { get; set; }
            public string Friday { get; set; }
            public string Saturday { get; set; }
            public string Sunday { get; set; }
            public string PromoCode { get; set; }
            public string SumBonus { get; set; }
            public string ExecutionOrder { get; set; }
            public string GiftPrice { get; set; }
            public string Kind { get; set; }
            public string Picture { get; set; }

        }
        public class ActionTable
        {
            public string NumDoc { get; set; }
            public string NumList { get; set; }
            public string CodeTovar { get; set; }
            public string Price { get; set; }
        }
        public class Characteristic
        {
            public string CodeTovar { get; set; }
            public string Name { get; set; }
            public string Guid { get; set; }
            public string RetailPrice { get; set; }
        }
        public class Sertificate
        {
            public string Code { get; set; }
            public string CodeTovar { get; set; }
            public string Rating { get; set; }
            public string IsActive { get; set; }
        }
        public class PromoText
        {
            public string AdvertisementText { get; set; }
            public string NumStr { get; set; }
            public string Picture { get; set; }
        }
        public class ActionClients
        {
            public string NumDoc { get; set; }
            public string CodeClient { get; set; }
        }

        private void download_bonus_clients_Click(object sender, EventArgs e)
        {
            load_bonus_clients(true);            
        }
        
        private void btn_write_to_database_Click(object sender, EventArgs e)
        {

            if (!File.Exists(Application.StartupPath + "/LoadCashData.txt"))
            {
                MessageBox.Show("Файл " + (Application.StartupPath + "/LoadCashData.txt") + " не найден ");
                return;
            }
            NpgsqlConnection conn = null;
            NpgsqlTransaction tran = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                tran = conn.BeginTransaction();
                NpgsqlCommand command = null;
                using (StreamReader sr = new StreamReader(Application.StartupPath + "/LoadCashData.txt", Encoding.UTF8))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        command = new NpgsqlCommand(line, conn);
                        command.Transaction = tran;
                        command.ExecuteNonQuery();
                    }
                }
                tran.Commit();
                conn.Close();
                //Запись об успешной 
                MessageBox.Show("Загрузка в БД успешно завершена");
            }
            catch (NpgsqlException ex)
            {
                string error = ex.Message;
                MessageBox.Show(error, "Ошибка при импорте данных");
                //MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при импорте данных");
                //MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
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
        
        private string DecompressString(Byte[] value)
        {
            string resultString = string.Empty;
            if (value != null && value.Length > 0)
            {
                using (MemoryStream stream = new MemoryStream(value))
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                using (StreamReader reader = new StreamReader(zip))
                {
                    resultString = reader.ReadToEnd();
                }
            }
            return resultString;
        }

        public class QueryPacketData:IDisposable
        {
            public string Version { get; set; }
            public string NickShop { get; set; }
            public string CodeShop { get; set; }
            public string LastDateDownloadTovar { get; set; }
            public string NumCash { get; set; }

            void IDisposable.Dispose()
            {
                
            }
        }



        private LoadPacketData getLoadPacketDataFull(string nick_shop, string data_encrypt, string key)
        {
            LoadPacketData loadPacketData = new LoadPacketData();
            loadPacketData.PacketIsFull = false;

            string result_query = "";
            string decrypt_data = "";
            try
            {
                using (Cash8.DS.DS ds = MainStaticClass.get_ds())
                {
                    ds.Timeout = 60000;
                    //if (MainStaticClass.GetWorkSchema == 2)
                    //{
                    //    ds.Url = "http://10.21.200.21/DiscountSystem/Ds.asmx"; //"http://localhost:50520/DS.asmx";
                    //}
                    byte[] result_query_byte = ds.GetDataForCasheV8Jason(nick_shop, data_encrypt,MainStaticClass.GetWorkSchema.ToString());                   
                    result_query = DecompressString(result_query_byte);
                    decrypt_data = CryptorEngine.Decrypt(result_query, true, key);
                    loadPacketData = JsonConvert.DeserializeObject<LoadPacketData>(decrypt_data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                loadPacketData.PacketIsFull = false;
            }
            return loadPacketData;
        }


        private LoadPacketData getLoadPacketDataUpdateOnly(string nick_shop, string data_encrypt, string key)
        {
            LoadPacketData loadPacketData = new LoadPacketData();
            loadPacketData.PacketIsFull = false;

            string result_query = "";
            string decrypt_data = "";
            try
            {
                using (Cash8.DS.DS ds = MainStaticClass.get_ds())
                {
                    ds.Timeout = 60000;
                    byte[] result_query_byte = ds.GetDataForCasheV8JasonUpdateOnly(nick_shop, data_encrypt,MainStaticClass.GetWorkSchema.ToString());
                    result_query = DecompressString(result_query_byte);
                    decrypt_data = CryptorEngine.Decrypt(result_query, true, key);
                    loadPacketData = JsonConvert.DeserializeObject<LoadPacketData>(decrypt_data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                loadPacketData.PacketIsFull = false;
            }
            return loadPacketData;
        }


        /// <summary>
        /// ///////////////////////////
        /// </summary>
        /// <param name="barcodes"></param>

        public void InsertBarcodes(List<Barcode> barcodes, NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            int maxDegreeOfParallelism = 20; // Максимальное количество одновременно выполняемых задач
            SemaphoreSlim semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            int totalTasks = barcodes.Count; // Общее количество задач
            int completedTasks = 0;

            var tasks = new List<Task>();

            foreach (var barcode in barcodes)
            {
                // Ожидаем, пока освободится слот для новой задачи
                semaphore.Wait();

                // Запускаем задачу
                Task task = Task.Run(() =>
                {
                    try
                    {
                        InsertBarcode(barcode, conn, tran); // Вставляем штрихкод
                    }
                    finally
                    {
                        // Освобождаем слот после завершения задачи
                        semaphore.Release();

                        // Увеличиваем счетчик завершенных задач
                        Interlocked.Increment(ref completedTasks);
                        Console.WriteLine($"Выполнено {completedTasks} из {totalTasks}");
                    }
                });

                tasks.Add(task);
            }

            // Ожидаем завершения всех задач
            Task.WaitAll(tasks.ToArray());
        }

        private void InsertBarcode(Barcode barcode, NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            using (var cmd = new NpgsqlCommand(
                "INSERT INTO barcode(tovar_code, barcode) VALUES (@tovarCode, @barcode)", conn, tran))
            {
                cmd.Parameters.AddWithValue("tovarCode", barcode.TovarCode);
                cmd.Parameters.AddWithValue("barcode", barcode.BarCode);
                cmd.ExecuteNonQuery();
            }
        }

        private void HandleError(long tovarCode, string barcode, Exception ex)
        {            
            MessageBox.Show($"Ошибка при вставке штрихкода. Код товара: {tovarCode}, Штрихкод: {barcode}. Подробности: {ex.Message}","Ошибка при загрузке данных");
        }

        public void UpdateOrInsertRow(Tovar tovar, NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            // Пытаемся обновить строку
            string updateSql = @"
        UPDATE tovar 
        SET name = @name, 
            retail_price = @retailPrice, 
            its_deleted = @itsDeleted, 
            nds = @nds, 
            its_certificate = @itsCertificate, 
            percent_bonus = @percentBonus, 
            tnved = @tnved, 
            its_marked = @itsMarked, 
            its_excise = @itsExcise, 
            cdn_check = @cdnCheck, 
            fractional = @fractional, 
            refusal_of_marking = @refusalOfMarking 
        WHERE code = @code";

            using (var updateCmd = new NpgsqlCommand(updateSql, conn, tran))
            {
                updateCmd.Parameters.AddWithValue("name", tovar.Name);
                updateCmd.Parameters.AddWithValue("retailPrice", tovar.RetailPrice);
                updateCmd.Parameters.AddWithValue("itsDeleted", tovar.ItsDeleted);
                updateCmd.Parameters.AddWithValue("nds", tovar.Nds);
                updateCmd.Parameters.AddWithValue("itsCertificate", tovar.ItsCertificate);
                updateCmd.Parameters.AddWithValue("percentBonus", tovar.PercentBonus);
                updateCmd.Parameters.AddWithValue("tnved", tovar.TnVed);
                updateCmd.Parameters.AddWithValue("itsMarked", tovar.ItsMarked);
                updateCmd.Parameters.AddWithValue("itsExcise", tovar.ItsExcise);
                updateCmd.Parameters.AddWithValue("cdnCheck", tovar.CdnCheck);
                updateCmd.Parameters.AddWithValue("fractional", tovar.Fractional);
                updateCmd.Parameters.AddWithValue("refusalOfMarking", tovar.RefusalOfMarking);
                updateCmd.Parameters.AddWithValue("code", tovar.Code);

                int rowsUpdated = updateCmd.ExecuteNonQuery();

                // Если ни одна строка не была обновлена, выполняем INSERT
                if (rowsUpdated == 0)
                {
                    string insertSql = @"
                INSERT INTO tovar(
                    code, name, retail_price, its_deleted, nds, its_certificate, 
                    percent_bonus, tnved, its_marked, its_excise, cdn_check, 
                    fractional, refusal_of_marking
                ) VALUES (
                    @code, @name, @retailPrice, @itsDeleted, @nds, @itsCertificate, 
                    @percentBonus, @tnved, @itsMarked, @itsExcise, @cdnCheck, 
                    @fractional, @refusalOfMarking
                )";

                    using (var insertCmd = new NpgsqlCommand(insertSql, conn, tran))
                    {
                        insertCmd.Parameters.AddWithValue("code", tovar.Code);
                        insertCmd.Parameters.AddWithValue("name", tovar.Name);
                        insertCmd.Parameters.AddWithValue("retailPrice", tovar.RetailPrice);
                        insertCmd.Parameters.AddWithValue("itsDeleted", tovar.ItsDeleted);
                        insertCmd.Parameters.AddWithValue("nds", tovar.Nds);
                        insertCmd.Parameters.AddWithValue("itsCertificate", tovar.ItsCertificate);
                        insertCmd.Parameters.AddWithValue("percentBonus", tovar.PercentBonus);
                        insertCmd.Parameters.AddWithValue("tnved", tovar.TnVed);
                        insertCmd.Parameters.AddWithValue("itsMarked", tovar.ItsMarked);
                        insertCmd.Parameters.AddWithValue("itsExcise", tovar.ItsExcise);
                        insertCmd.Parameters.AddWithValue("cdnCheck", tovar.CdnCheck);
                        insertCmd.Parameters.AddWithValue("fractional", tovar.Fractional);
                        insertCmd.Parameters.AddWithValue("refusalOfMarking", tovar.RefusalOfMarking);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UpdateOrInsertTovars(List<Tovar> tovars, NpgsqlConnection conn, NpgsqlTransaction tran)
        {
            int maxDegreeOfParallelism = 20; // Максимальное количество одновременно выполняемых задач
            SemaphoreSlim semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            int totalTasks = tovars.Count; // Общее количество задач
            int completedTasks = 0;

            var tasks = new List<Task>();

            foreach (var tovar in tovars)
            {
                // Ожидаем, пока освободится слот для новой задачи
                semaphore.Wait();

                // Запускаем задачу
                Task task = Task.Run(() =>
                {
                    try
                    {
                        UpdateOrInsertRow(tovar, conn, tran); // Используем метод с UPDATE/INSERT
                    }
                    finally
                    {
                        // Освобождаем слот после завершения задачи
                        semaphore.Release();

                        // Увеличиваем счетчик завершенных задач
                        Interlocked.Increment(ref completedTasks);
                        Console.WriteLine($"Выполнено {completedTasks} из {totalTasks}");
                    }
                });

                tasks.Add(task);
            }

            // Ожидаем завершения всех задач
            Task.WaitAll(tasks.ToArray());
        }

        public void new_load_fast()
        {
            //btn_new_load.Enabled = false;
            if (!MainStaticClass.service_is_worker())
            {
                MessageBox.Show("Веб сервис недоступен");
                return;
            }

            check_temp_tables();

            //Получить параметра для запроса на сервер 
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            if (nick_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить название магазина ");
                return;
            }

            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (code_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить код магазина ");
                return;
            }
            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
            string data_encrypt = "";
            using (QueryPacketData queryPacketData = new QueryPacketData())
            {
                queryPacketData.NickShop = nick_shop;
                queryPacketData.CodeShop = code_shop;
                queryPacketData.LastDateDownloadTovar = last_date_download_tovars().ToString("dd-MM-yyyy");
                queryPacketData.NumCash = MainStaticClass.CashDeskNumber.ToString();
                queryPacketData.Version = MainStaticClass.version().Replace(".", "");
                string data = JsonConvert.SerializeObject(queryPacketData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                data_encrypt = CryptorEngine.Encrypt(data, true, key);
            }

            List<string> queries = new List<string>();//Список запросов                                          
            using (LoadPacketData loadPacketData = getLoadPacketDataFull(nick_shop, data_encrypt, key))
            {
                if (!loadPacketData.PacketIsFull)
                {
                    MessageBox.Show(loadPacketData.Exception + "\r\n Неудачная попытка получения данных");
                    return;
                }
                if (loadPacketData.Exchange)
                {
                    MessageBox.Show("Пакет данных получен во время обновления данных на сервере, загрузка прервана");
                    return;
                }

                queries.Add("Delete from action_table");
                queries.Add("Delete from action_header");
                queries.Add("Delete from advertisement");
                queries.Add("UPDATE constants SET threshold=" + loadPacketData.Threshold.ToString());
                queries.Add("UPDATE constants SET cdn_token='" + loadPacketData.TokenMark.ToString() + "'");


                if (loadPacketData.ListPromoText != null)
                {
                    if (loadPacketData.ListPromoText.Count > 0)
                    {
                        foreach (PromoText promoText in loadPacketData.ListPromoText)
                        {
                            queries.Add("INSERT INTO advertisement(advertisement_text,num_str)VALUES ('" + promoText.AdvertisementText + "'," + promoText.NumStr + ")");
                        }
                        loadPacketData.ListPromoText.Clear();
                        loadPacketData.ListPromoText = null;
                    }
                }
                if (loadPacketData.ListTovar.Count > 0)
                {
                    foreach (Tovar tovar in loadPacketData.ListTovar)
                    {
                        queries.Add("INSERT INTO tovar2(code,name,retail_price,its_deleted,nds,its_certificate,percent_bonus,tnved,its_marked,its_excise,cdn_check,fractional,refusal_of_marking) VALUES(" +
                                                        tovar.Code + ",'" +
                                                        tovar.Name + "'," +
                                                        tovar.RetailPrice + "," +
                                                        tovar.ItsDeleted + "," +
                                                        tovar.Nds + "," +
                                                        tovar.ItsCertificate + "," +
                                                        tovar.PercentBonus + ",'" +
                                                        tovar.TnVed + "'," +
                                                        tovar.ItsMarked + "," +
                                                        tovar.ItsExcise + "," +
                                                        tovar.CdnCheck + "," +
                                                        tovar.Fractional + "," +
                                                        tovar.RefusalOfMarking + ");");
                    }
                    loadPacketData.ListTovar.Clear();
                    loadPacketData.ListTovar = null;
                }

                queries.Add("UPDATE tovar SET its_deleted=1,retail_price=0;");
                queries.Add("INSERT INTO tovar SELECT F.code, F.name, F.retail_price, F.its_deleted, F.nds, F.its_certificate, F.percent_bonus, F.tnved,F.its_marked,F.its_excise,F.cdn_check,F.fractional,F.refusal_of_marking FROM(SELECT tovar2.code AS code, tovar.code AS code2, tovar2.name, tovar2.retail_price, tovar2.its_deleted, tovar2.nds, tovar2.its_certificate, tovar2.percent_bonus, tovar2.tnved,tovar2.its_marked,tovar2.its_excise,tovar2.cdn_check,tovar2.fractional,tovar2.refusal_of_marking  FROM tovar2 left join tovar on tovar2.code = tovar.code)AS F WHERE code2 ISNULL;");
                queries.Add("UPDATE tovar SET name = tovar2.name,retail_price = tovar2.retail_price, its_deleted=tovar2.its_deleted,nds=tovar2.nds,its_certificate = tovar2.its_certificate,percent_bonus = tovar2.percent_bonus,tnved = tovar2.tnved,its_marked = tovar2.its_marked,its_excise=tovar2.its_excise,cdn_check = tovar2.cdn_check,fractional=tovar2.fractional,refusal_of_marking=tovar2.refusal_of_marking FROM tovar2 where tovar.code=tovar2.code;");
                queries.Add("DELETE FROM barcode;");
                if (loadPacketData.ListBarcode.Count > 0)
                {
                    foreach (Barcode barcode in loadPacketData.ListBarcode)
                    {
                        queries.Add("INSERT INTO barcode(tovar_code,barcode) VALUES(" + barcode.TovarCode + ",'" + barcode.BarCode + "')");
                    }
                    loadPacketData.ListBarcode.Clear();
                    loadPacketData.ListBarcode = null;

                }
                if (loadPacketData.ListCharacteristic != null)
                {
                    if (loadPacketData.ListCharacteristic.Count > 0)
                    {
                        queries.Add("DELETE FROM characteristic");
                        foreach (Characteristic characteristic in loadPacketData.ListCharacteristic)
                        {
                            queries.Add("INSERT INTO characteristic(tovar_code, guid, name, retail_price_characteristic) VALUES(" +
                                characteristic.CodeTovar + ",'" +
                                characteristic.Guid + "','" +
                                characteristic.Name + "'," +
                                characteristic.RetailPrice + ")");
                        }
                        loadPacketData.ListCharacteristic.Clear();
                        loadPacketData.ListCharacteristic = null;
                    }
                }

                queries.Add("DELETE FROM sertificates");

                if (loadPacketData.ListSertificate.Count > 0)
                {
                    foreach (Sertificate sertificate in loadPacketData.ListSertificate)
                    {
                        queries.Add(" INSERT INTO sertificates(code, code_tovar, rating, is_active)VALUES (" +
                            sertificate.Code + "," +
                            sertificate.CodeTovar + "," +
                            sertificate.Rating + "," +
                            sertificate.IsActive + ")");
                    }
                    loadPacketData.ListSertificate.Clear();
                    loadPacketData.ListSertificate = null;
                }


                if (loadPacketData.ListActionHeader.Count > 0)
                {
                    foreach (ActionHeader actionHeader in loadPacketData.ListActionHeader)
                    {
                        queries.Add("INSERT INTO action_header(date_started,date_end,num_doc,tip,barcode,persent,sum,comment,marker,action_by_discount,time_start,time_end," +
                        " bonus_promotion, with_old_promotion, monday, tuesday, wednesday, thursday, friday, saturday, sunday, promo_code, sum_bonus,execution_order,gift_price,kind,sum1)VALUES ('" +
                        actionHeader.DateStarted + "','" +
                        actionHeader.DateEnd + "'," +
                        actionHeader.NumDoc + "," +
                        actionHeader.Tip + ",'" +
                        actionHeader.Barcode + "'," +
                        actionHeader.Persent + "," +
                        actionHeader.sum + ",'" +
                        actionHeader.Comment + "'," +
                        //actionHeader.CodeTovar + "," +
                        actionHeader.Marker + "," +
                        actionHeader.ActionByDiscount + "," +
                        actionHeader.TimeStart + "," +
                        actionHeader.TimeEnd + "," +
                        actionHeader.BonusPromotion + "," +
                        actionHeader.WithOldPromotion + "," +
                        actionHeader.Monday + "," +
                        actionHeader.Tuesday + "," +
                        actionHeader.Wednesday + "," +
                        actionHeader.Thursday + "," +
                        actionHeader.Friday + "," +
                        actionHeader.Saturday + "," +
                        actionHeader.Sunday + "," +
                        actionHeader.PromoCode + "," +
                        actionHeader.SumBonus + "," +
                        actionHeader.ExecutionOrder + "," +
                        actionHeader.GiftPrice + "," +
                        actionHeader.Kind + "," +
                        actionHeader.sum1 + ")");
                    }
                    if (loadPacketData.ListActionTable.Count > 0)
                    {
                        foreach (ActionTable actionTable in loadPacketData.ListActionTable)
                        {
                            queries.Add("INSERT INTO action_table(num_doc, num_list, code_tovar, price)VALUES(" +
                                actionTable.NumDoc + "," +
                                actionTable.NumList + "," +
                                actionTable.CodeTovar + "," +
                                actionTable.Price + ")");
                        }
                    }
                    loadPacketData.ListActionHeader.Clear();
                    loadPacketData.ListActionTable.Clear();
                    loadPacketData.ListActionHeader = null;
                    loadPacketData.ListActionTable = null;
                }
                else
                {
                    MessageBox.Show("Нет данных по акциям");
                }

                queries.Add("Delete from action_clients");

                if (loadPacketData.ListActionClients.Count > 0)
                {
                    foreach (ActionClients actionClients in loadPacketData.ListActionClients)
                    {
                        queries.Add("INSERT INTO action_clients(num_doc, code_client) VALUES(" +
                            actionClients.NumDoc + "," +
                            actionClients.CodeClient + ")");
                    }
                    loadPacketData.ListActionClients.Clear();
                    loadPacketData.ListActionClients = null;
                }
                ;
            }

            //queries.Add("UPDATE date_sync SET tovar='" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")+"'");
            //queries.Add("INSERT INTO date_sync(tovar) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')");

            NpgsqlConnection conn = null;
            NpgsqlTransaction tran = null;
            string s = "";
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                tran = conn.BeginTransaction();
                NpgsqlCommand command = null;
                foreach (string str in queries)
                {
                    s = str;
                    command = new NpgsqlCommand(str, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }
                //Обновление даты последнего обновления 
                string query = "UPDATE date_sync SET tovar = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                if (command.ExecuteNonQuery() == 0)
                {
                    query = "INSERT INTO date_sync(tovar) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }

                queries.Clear();
                queries = null;
                tran.Commit();
                if (!MainStaticClass.SendResultGetData())
                {
                    MessageBox.Show("Не удалось отправить информацию об успешной загрузке");
                    MainStaticClass.write_event_in_log("Не удалось отправить информацию об успешной загрузке ", "Загрузка данных", "0");
                }
                conn.Close();
                command.Dispose();
                command = null;
                tran = null;
                MessageBox.Show("Загрузка успешно завершена");
                if (CheckFirstLoadData())
                {
                    MessageBox.Show(" Это была первая загрузка данных, для применения новых параметров программа будет закрыта");
                    Application.Exit();
                }
            }
            catch (NpgsqlException ex)
            {
                string error = ex.Message;
                MessageBox.Show(error, "Ошибка при импорте данных");
                MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при импорте данных");
                MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
                }

            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            //btn_new_load.Enabled = true;
        }

        // Вспомогательный метод для выполнения SQL-запросов в транзакции
        private void ExecuteNonQuery(NpgsqlTransaction tran, string query)
        {
            using (var cmd = new NpgsqlCommand(query, tran.Connection, tran))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private void new_load()
        {
            //btn_new_load.Enabled = false;
            if (!MainStaticClass.service_is_worker())
            {
                MessageBox.Show("Веб сервис недоступен");
                return;
            }

            check_temp_tables();

            //Получить параметра для запроса на сервер 
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            if (nick_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить название магазина ");
                return;
            }

            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (code_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить код магазина ");
                return;
            }
            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
            string data_encrypt = "";
            using (QueryPacketData queryPacketData = new QueryPacketData())
            {
                queryPacketData.NickShop = nick_shop;
                queryPacketData.CodeShop = code_shop;
                queryPacketData.LastDateDownloadTovar = last_date_download_tovars().ToString("dd-MM-yyyy");
                queryPacketData.NumCash = MainStaticClass.CashDeskNumber.ToString();
                queryPacketData.Version = MainStaticClass.version().Replace(".", "");
                string data = JsonConvert.SerializeObject(queryPacketData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                data_encrypt = CryptorEngine.Encrypt(data, true, key);
            }

            List<string> queries = new List<string>();//Список запросов                                          
            using (LoadPacketData loadPacketData = getLoadPacketDataFull(nick_shop, data_encrypt, key))
            {
                if (!loadPacketData.PacketIsFull)
                {
                    MessageBox.Show(loadPacketData.Exception + "\r\n Неудачная попытка получения данных");
                    return;
                }
                if (loadPacketData.Exchange)
                {
                    MessageBox.Show("Пакет данных получен во время обновления данных на сервере, загрузка прервана");
                    return;
                }

                queries.Add("Delete from action_table");
                queries.Add("Delete from action_header");
                queries.Add("Delete from advertisement");
                //queries.Add("UPDATE constants SET threshold=" + loadPacketData.Threshold.ToString());
                queries.Add("UPDATE constants SET cdn_token='" + loadPacketData.TokenMark.ToString() + "'");


                if (loadPacketData.ListPromoText != null)
                {
                    if (loadPacketData.ListPromoText.Count > 0)
                    {
                        foreach (PromoText promoText in loadPacketData.ListPromoText)
                        {
                            queries.Add("INSERT INTO advertisement(advertisement_text,num_str,picture)VALUES ('" + promoText.AdvertisementText + "'," + promoText.NumStr + ",'"+promoText.Picture+"')");
                        }
                        loadPacketData.ListPromoText.Clear();
                        loadPacketData.ListPromoText = null;
                    }
                }
                if (loadPacketData.ListTovar.Count > 0)
                {
                    foreach (Tovar tovar in loadPacketData.ListTovar)
                    {
                        queries.Add("INSERT INTO tovar2(code,name,retail_price,its_deleted,nds,its_certificate,percent_bonus,tnved,its_marked,its_excise,cdn_check,fractional,refusal_of_marking) VALUES(" +
                                                        tovar.Code + ",'" +
                                                        tovar.Name + "'," +
                                                        tovar.RetailPrice + "," +
                                                        tovar.ItsDeleted + "," +
                                                        tovar.Nds + "," +
                                                        tovar.ItsCertificate + "," +
                                                        tovar.PercentBonus + ",'" +
                                                        tovar.TnVed + "'," +
                                                        tovar.ItsMarked + "," +
                                                        tovar.ItsExcise + "," +
                                                        tovar.CdnCheck + "," +
                                                        tovar.Fractional + "," +
                                                        tovar.RefusalOfMarking+");");
                    }
                    loadPacketData.ListTovar.Clear();
                    loadPacketData.ListTovar = null;
                }

                queries.Add("UPDATE tovar SET its_deleted=1,retail_price=0;");
                queries.Add("INSERT INTO tovar SELECT F.code, F.name, F.retail_price, F.its_deleted, F.nds, F.its_certificate, F.percent_bonus, F.tnved,F.its_marked,F.its_excise,F.cdn_check,F.fractional,F.refusal_of_marking FROM(SELECT tovar2.code AS code, tovar.code AS code2, tovar2.name, tovar2.retail_price, tovar2.its_deleted, tovar2.nds, tovar2.its_certificate, tovar2.percent_bonus, tovar2.tnved,tovar2.its_marked,tovar2.its_excise,tovar2.cdn_check,tovar2.fractional,tovar2.refusal_of_marking  FROM tovar2 left join tovar on tovar2.code = tovar.code)AS F WHERE code2 ISNULL;");
                queries.Add("UPDATE tovar SET name = tovar2.name,retail_price = tovar2.retail_price, its_deleted=tovar2.its_deleted,nds=tovar2.nds,its_certificate = tovar2.its_certificate,percent_bonus = tovar2.percent_bonus,tnved = tovar2.tnved,its_marked = tovar2.its_marked,its_excise=tovar2.its_excise,cdn_check = tovar2.cdn_check,fractional=tovar2.fractional,refusal_of_marking=tovar2.refusal_of_marking FROM tovar2 where tovar.code=tovar2.code;");
                queries.Add("DELETE FROM barcode;");
                if (loadPacketData.ListBarcode.Count > 0)
                {
                    foreach (Barcode barcode in loadPacketData.ListBarcode)
                    {
                        queries.Add("INSERT INTO barcode(tovar_code,barcode) VALUES(" + barcode.TovarCode + ",'" + barcode.BarCode + "')");
                    }
                    loadPacketData.ListBarcode.Clear();
                    loadPacketData.ListBarcode = null;

                }
                if (loadPacketData.ListCharacteristic != null)
                {
                    if (loadPacketData.ListCharacteristic.Count > 0)
                    {
                        queries.Add("DELETE FROM characteristic");
                        foreach (Characteristic characteristic in loadPacketData.ListCharacteristic)
                        {
                            queries.Add("INSERT INTO characteristic(tovar_code, guid, name, retail_price_characteristic) VALUES(" +
                                characteristic.CodeTovar + ",'" +
                                characteristic.Guid + "','" +
                                characteristic.Name + "'," +
                                characteristic.RetailPrice + ")");
                        }
                        loadPacketData.ListCharacteristic.Clear();
                        loadPacketData.ListCharacteristic = null;
                    }
                }

                queries.Add("DELETE FROM sertificates");

                if (loadPacketData.ListSertificate.Count > 0)
                {
                    foreach (Sertificate sertificate in loadPacketData.ListSertificate)
                    {
                        queries.Add(" INSERT INTO sertificates(code, code_tovar, rating, is_active)VALUES (" +
                            sertificate.Code + "," +
                            sertificate.CodeTovar + "," +
                            sertificate.Rating + "," +
                            sertificate.IsActive + ")");
                    }
                    loadPacketData.ListSertificate.Clear();
                    loadPacketData.ListSertificate = null;
                }


                if (loadPacketData.ListActionHeader.Count > 0)
                {
                    foreach (ActionHeader actionHeader in loadPacketData.ListActionHeader)
                    {
                        queries.Add("INSERT INTO action_header(date_started,date_end,num_doc,tip,barcode,persent,sum,comment,marker,action_by_discount,time_start,time_end," +
                        " bonus_promotion, with_old_promotion, monday, tuesday, wednesday, thursday, friday, saturday, sunday, promo_code, sum_bonus,execution_order,gift_price,kind,sum1,picture)VALUES ('" +
                        actionHeader.DateStarted + "','" +
                        actionHeader.DateEnd + "'," +
                        actionHeader.NumDoc + "," +
                        actionHeader.Tip + ",'" +
                        actionHeader.Barcode + "'," +
                        actionHeader.Persent + "," +
                        actionHeader.sum + ",'" +
                        actionHeader.Comment + "'," +
                        //actionHeader.CodeTovar + "," +
                        actionHeader.Marker + "," +
                        actionHeader.ActionByDiscount + "," +
                        actionHeader.TimeStart + "," +
                        actionHeader.TimeEnd + "," +
                        actionHeader.BonusPromotion + "," +
                        actionHeader.WithOldPromotion + "," +
                        actionHeader.Monday + "," +
                        actionHeader.Tuesday + "," +
                        actionHeader.Wednesday + "," +
                        actionHeader.Thursday + "," +
                        actionHeader.Friday + "," +
                        actionHeader.Saturday + "," +
                        actionHeader.Sunday + "," +
                        actionHeader.PromoCode + "," +
                        actionHeader.SumBonus + "," +
                        actionHeader.ExecutionOrder + "," +
                        actionHeader.GiftPrice + "," +
                        actionHeader.Kind + "," +
                        actionHeader.sum1 + ",'"+
                        actionHeader.Picture+"')");
                    }
                    if (loadPacketData.ListActionTable.Count > 0)
                    {
                        foreach (ActionTable actionTable in loadPacketData.ListActionTable)
                        {
                            queries.Add("INSERT INTO action_table(num_doc, num_list, code_tovar, price)VALUES(" +
                                actionTable.NumDoc + "," +
                                actionTable.NumList + "," +
                                actionTable.CodeTovar + "," +
                                actionTable.Price + ")");
                        }
                    }
                    loadPacketData.ListActionHeader.Clear();
                    loadPacketData.ListActionTable.Clear();
                    loadPacketData.ListActionHeader = null;
                    loadPacketData.ListActionTable = null;
                }
                else
                {
                    MessageBox.Show("Нет данных по акциям");
                }

                queries.Add("Delete from action_clients");

                if (loadPacketData.ListActionClients.Count > 0)
                {
                    foreach (ActionClients actionClients in loadPacketData.ListActionClients)
                    {
                        queries.Add("INSERT INTO action_clients(num_doc, code_client) VALUES(" +
                            actionClients.NumDoc + "," +
                            actionClients.CodeClient + ")");
                    }
                    loadPacketData.ListActionClients.Clear();
                    loadPacketData.ListActionClients = null;
                }
                ;
            }

            //queries.Add("UPDATE date_sync SET tovar='" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")+"'");
            //queries.Add("INSERT INTO date_sync(tovar) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')");

            NpgsqlConnection conn = null;
            NpgsqlTransaction tran = null;
            string s = "";
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                tran = conn.BeginTransaction();
                NpgsqlCommand command = null;
                foreach (string str in queries)
                {
                    s = str;
                    command = new NpgsqlCommand(str, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }
                //Обновление даты последнего обновления 
                string query = "UPDATE date_sync SET tovar = '" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = tran;
                if (command.ExecuteNonQuery() == 0)
                {
                    query = "INSERT INTO date_sync(tovar) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')";
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }

                queries.Clear();
                queries = null;
                tran.Commit();
                if (!MainStaticClass.SendResultGetData())
                {
                    MessageBox.Show("Не удалось отправить информацию об успешной загрузке");
                    MainStaticClass.write_event_in_log("Не удалось отправить информацию об успешной загрузке ", "Загрузка данных", "0");
                }
                conn.Close();
                command.Dispose();
                command = null;
                tran = null;
                MessageBox.Show("Загрузка успешно завершена");
                if (CheckFirstLoadData())
                {
                    MessageBox.Show(" Это была первая загрузка данных, для применения новых параметров программа будет закрыта");
                    Application.Exit();
                }
            }
            catch (NpgsqlException ex)
            {
                string error = ex.Message;
                MessageBox.Show(error, "Ошибка при импорте данных");
                MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при импорте данных");
                MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
                }

            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            //btn_new_load.Enabled = true;
        }

        private bool CheckFirstLoadData()
        {
            bool result = false;

            try
            {
                using (var conn = MainStaticClass.NpgsqlConn())
                {
                    conn.Open();
                    string query = "SELECT tovar FROM public.date_sync";
                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        object resultQuery = command.ExecuteScalar();
                        if (resultQuery != null && DateTime.TryParse(resultQuery.ToString(), out DateTime date))
                        {
                            if (date < new DateTime(2001, 1, 1))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is NpgsqlException || ex is InvalidOperationException || ex is FormatException)
            {
                MessageBox.Show("Произошла ошибка при определении первой загрузки: " + ex.Message);
            }

            return result;
        }




        private void btn_new_load_Click(object sender, EventArgs e)
        {            
            InventoryManager.ClearDictionaryProductData();
            LoadActionDataInMemory.AllActionData1 = null;
            LoadActionDataInMemory.AllActionData2 = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            new_load();
            InventoryManager.FillDictionaryProductDataAsync(); //товары и цены
            Task.Run(() => InventoryManager.DictionaryPriceGiftAction);//цены для подарков в акциях

            //btn_new_load.Enabled = false;
            //if (!MainStaticClass.service_is_worker())
            //{
            //    MessageBox.Show("Веб сервис недоступен");
            //    return;
            //}

            //check_temp_tables();

            ////Получить параметра для запроса на сервер 
            //string nick_shop = MainStaticClass.Nick_Shop.Trim();
            //if (nick_shop.Trim().Length == 0)
            //{
            //    MessageBox.Show(" Не удалось получить название магазина ");
            //    return;
            //}

            //string code_shop = MainStaticClass.Code_Shop.Trim();
            //if (code_shop.Trim().Length == 0)
            //{
            //    MessageBox.Show(" Не удалось получить код магазина ");
            //    return;
            //}
            //string count_day = CryptorEngine.get_count_day();
            //string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
            //string data_encrypt = "";
            //using (QueryPacketData queryPacketData = new QueryPacketData())
            //{
            //    queryPacketData.NickShop = nick_shop;
            //    queryPacketData.CodeShop = code_shop;
            //    queryPacketData.LastDateDownloadTovar = last_date_download_tovars().ToString("dd-MM-yyyy");
            //    queryPacketData.NumCash = MainStaticClass.CashDeskNumber.ToString();
            //    queryPacketData.Version = MainStaticClass.version().Replace(".", "");
            //    string data = JsonConvert.SerializeObject(queryPacketData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //    data_encrypt = CryptorEngine.Encrypt(data, true, key);
            //}

            //List<string> queries = new List<string>();//Список запросов                                          
            //using (LoadPacketData loadPacketData = getLoadPacketDataFull(nick_shop, data_encrypt, key))
            //{                
            //    if (!loadPacketData.PacketIsFull)
            //    {
            //        MessageBox.Show(loadPacketData.Exception+"\r\n Неудачная попытка получения данных");
            //        return;
            //    }                
            //    if (loadPacketData.Exchange)
            //    {
            //        MessageBox.Show("Пакет данных получен во время обновления данных на сервере, загрузка прервана");
            //        return;
            //    }                

            //    queries.Add("Delete from action_table");
            //    queries.Add("Delete from action_header");
            //    queries.Add("Delete from advertisement");
            //    queries.Add("UPDATE constants SET threshold="+loadPacketData.Threshold.ToString());
            //    queries.Add("UPDATE constants SET cdn_token='" + loadPacketData.TokenMark.ToString()+"'");                


            //    if (loadPacketData.ListPromoText != null)
            //    {
            //        if (loadPacketData.ListPromoText.Count > 0)
            //        {
            //            foreach (PromoText promoText in loadPacketData.ListPromoText)
            //            {
            //                queries.Add("INSERT INTO advertisement(advertisement_text,num_str)VALUES ('" + promoText.AdvertisementText + "'," + promoText.NumStr + ")");
            //            }
            //            loadPacketData.ListPromoText.Clear();
            //            loadPacketData.ListPromoText = null;
            //        }
            //    }
            //    if (loadPacketData.ListTovar.Count > 0)
            //    {
            //        foreach (Tovar tovar in loadPacketData.ListTovar)
            //        {
            //            queries.Add("INSERT INTO tovar2(code,name,retail_price,its_deleted,nds,its_certificate,percent_bonus,tnved,its_marked,its_excise,cdn_check,fractional) VALUES(" +
            //                                            tovar.Code + ",'" +
            //                                            tovar.Name + "'," +
            //                                            tovar.RetailPrice + "," +
            //                                            tovar.ItsDeleted + "," +
            //                                            tovar.Nds + "," +
            //                                            tovar.ItsCertificate + "," +
            //                                            tovar.PercentBonus + ",'" +
            //                                            tovar.TnVed +"',"+ 
            //                                            tovar.ItsMarked+","+
            //                                            tovar.ItsExcise+","+
            //                                            tovar.CdnCheck+","+
            //                                            tovar.Fractional+")");
            //        }
            //        loadPacketData.ListTovar.Clear();
            //        loadPacketData.ListTovar = null;
            //    }

            //    queries.Add("UPDATE tovar SET its_deleted=1,retail_price=0;");
            //    queries.Add("INSERT INTO tovar SELECT F.code, F.name, F.retail_price, F.its_deleted, F.nds, F.its_certificate, F.percent_bonus, F.tnved,F.its_marked,F.its_excise,F.cdn_check,F.fractional FROM(SELECT tovar2.code AS code, tovar.code AS code2, tovar2.name, tovar2.retail_price, tovar2.its_deleted, tovar2.nds, tovar2.its_certificate, tovar2.percent_bonus, tovar2.tnved,tovar2.its_marked,tovar2.its_excise,tovar2.cdn_check,tovar2.fractional  FROM tovar2 left join tovar on tovar2.code = tovar.code)AS F WHERE code2 ISNULL;");
            //    queries.Add("UPDATE tovar SET name = tovar2.name,retail_price = tovar2.retail_price, its_deleted=tovar2.its_deleted,nds=tovar2.nds,its_certificate = tovar2.its_certificate,percent_bonus = tovar2.percent_bonus,tnved = tovar2.tnved,its_marked = tovar2.its_marked,its_excise=tovar2.its_excise,cdn_check = tovar2.cdn_check,fractional=tovar2.fractional FROM tovar2 where tovar.code=tovar2.code;");
            //    queries.Add("DELETE FROM barcode;");
            //    if (loadPacketData.ListBarcode.Count > 0)
            //    {
            //        foreach (Barcode barcode in loadPacketData.ListBarcode)
            //        {
            //            queries.Add("INSERT INTO barcode(tovar_code,barcode) VALUES(" + barcode.TovarCode + ",'" + barcode.BarCode + "')");
            //        }
            //        loadPacketData.ListBarcode.Clear();
            //        loadPacketData.ListBarcode = null;

            //    }
            //    if (loadPacketData.ListCharacteristic != null)
            //    {
            //        if (loadPacketData.ListCharacteristic.Count > 0)
            //        {
            //            queries.Add("DELETE FROM characteristic");
            //            foreach (Characteristic characteristic in loadPacketData.ListCharacteristic)
            //            {
            //                queries.Add("INSERT INTO characteristic(tovar_code, guid, name, retail_price_characteristic) VALUES(" +
            //                    characteristic.CodeTovar + ",'" +
            //                    characteristic.Guid + "','" +
            //                    characteristic.Name + "'," +
            //                    characteristic.RetailPrice + ")");
            //            }
            //            loadPacketData.ListCharacteristic.Clear();
            //            loadPacketData.ListCharacteristic = null;
            //        }
            //    }

            //    queries.Add("DELETE FROM sertificates");

            //    if (loadPacketData.ListSertificate.Count > 0)
            //    {                    
            //        foreach (Sertificate sertificate in loadPacketData.ListSertificate)
            //        {
            //            queries.Add(" INSERT INTO sertificates(code, code_tovar, rating, is_active)VALUES (" +
            //                sertificate.Code + "," +
            //                sertificate.CodeTovar + "," +
            //                sertificate.Rating + "," +
            //                sertificate.IsActive + ")");
            //        }
            //        loadPacketData.ListSertificate.Clear();
            //        loadPacketData.ListSertificate = null;
            //    }


            //    if (loadPacketData.ListActionHeader.Count > 0)
            //    {
            //        foreach (ActionHeader actionHeader in loadPacketData.ListActionHeader)
            //        {
            //            queries.Add("INSERT INTO action_header(date_started,date_end,num_doc,tip,barcode,persent,sum,comment,marker,action_by_discount,time_start,time_end," +                        
            //            " bonus_promotion, with_old_promotion, monday, tuesday, wednesday, thursday, friday, saturday, sunday, promo_code, sum_bonus,execution_order,gift_price,kind,sum1)VALUES ('" +
            //            actionHeader.DateStarted + "','" +
            //            actionHeader.DateEnd + "'," +
            //            actionHeader.NumDoc + "," +
            //            actionHeader.Tip + ",'" +
            //            actionHeader.Barcode + "'," +
            //            actionHeader.Persent + "," +
            //            actionHeader.sum + ",'" +
            //            actionHeader.Comment + "'," +
            //            //actionHeader.CodeTovar + "," +
            //            actionHeader.Marker + "," +
            //            actionHeader.ActionByDiscount + "," +
            //            actionHeader.TimeStart + "," +
            //            actionHeader.TimeEnd + "," +
            //            actionHeader.BonusPromotion + "," +
            //            actionHeader.WithOldPromotion + "," +
            //            actionHeader.Monday + "," +
            //            actionHeader.Tuesday + "," +
            //            actionHeader.Wednesday + "," +
            //            actionHeader.Thursday + "," +
            //            actionHeader.Friday + "," +
            //            actionHeader.Saturday + "," +
            //            actionHeader.Sunday + "," +
            //            actionHeader.PromoCode + "," +
            //            actionHeader.SumBonus +","+
            //            actionHeader.ExecutionOrder + ","+
            //            actionHeader.GiftPrice+","+
            //            actionHeader.Kind+ ","+
            //            actionHeader.sum1+")");
            //        }
            //        if (loadPacketData.ListActionTable.Count > 0)
            //        {
            //            foreach (ActionTable actionTable in loadPacketData.ListActionTable)
            //            {
            //                queries.Add("INSERT INTO action_table(num_doc, num_list, code_tovar, price)VALUES(" +
            //                    actionTable.NumDoc + "," +
            //                    actionTable.NumList + "," +
            //                    actionTable.CodeTovar + "," +
            //                    actionTable.Price + ")");
            //            }
            //        }
            //        loadPacketData.ListActionHeader.Clear();
            //        loadPacketData.ListActionTable.Clear();
            //        loadPacketData.ListActionHeader = null;
            //        loadPacketData.ListActionTable = null;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Нет данных по акциям");
            //    }

            //    queries.Add("Delete from action_clients");

            //    if (loadPacketData.ListActionClients.Count > 0)
            //    {
            //        foreach (ActionClients actionClients in loadPacketData.ListActionClients)
            //        {
            //            queries.Add("INSERT INTO action_clients(num_doc, code_client) VALUES(" +
            //                actionClients.NumDoc + "," +
            //                actionClients.CodeClient + ")");
            //        }
            //        loadPacketData.ListActionClients.Clear();
            //        loadPacketData.ListActionClients = null;
            //    }
            //    ;
            //}

            ////queries.Add("UPDATE date_sync SET tovar='" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd")+"'");
            ////queries.Add("INSERT INTO date_sync(tovar) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')");

            //NpgsqlConnection conn = null;
            //NpgsqlTransaction tran = null;
            //string s = "";            
            //try
            //{
            //    conn = MainStaticClass.NpgsqlConn();
            //    conn.Open();
            //    tran = conn.BeginTransaction();
            //    NpgsqlCommand command = null;
            //    foreach (string str in queries)
            //    {
            //        s = str;
            //        command = new NpgsqlCommand(str, conn);
            //        command.Transaction = tran;
            //        command.ExecuteNonQuery();
            //    }
            //    //Обновление даты последнего обновления 
            //    string query = "UPDATE date_sync SET tovar = '" + DateTime.Now.ToString("yyyy-MM-dd")+"'";
            //    command = new NpgsqlCommand(query, conn);
            //    command.Transaction = tran;
            //    if (command.ExecuteNonQuery() == 0)
            //    {
            //        query = "INSERT INTO date_sync(tovar) VALUES('" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + "')";
            //        command = new NpgsqlCommand(query, conn);
            //        command.Transaction = tran;
            //        command.ExecuteNonQuery();
            //    }

            //    queries.Clear();
            //    queries = null;
            //    tran.Commit();
            //    if (!MainStaticClass.SendResultGetData())
            //    {
            //        MessageBox.Show("Не удалось отправить информацию об успешной загрузке");
            //        MainStaticClass.write_event_in_log("Не удалось отправить информацию об успешной загрузке ", "Загрузка данных", "0");
            //    }                
            //    conn.Close();
            //    command.Dispose();
            //    command = null;
            //    tran = null;
            //    MessageBox.Show("Загрузка успешно завершена");                
            //}
            //catch (NpgsqlException ex)
            //{
            //    string error = ex.Message;
            //    MessageBox.Show(error, "Ошибка при импорте данных");
            //    MessageBox.Show(s);
            //    if (tran != null)
            //    {
            //        tran.Rollback();
            //    }

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Ошибка при импорте данных");
            //    MessageBox.Show(s);
            //    if (tran != null)
            //    {                    
            //        tran.Rollback();
            //    }

            //}
            //finally
            //{
            //    if (conn.State == ConnectionState.Open)
            //    {
            //        conn.Close();
            //        conn.Dispose();
            //        conn = null;
            //    }
            //}
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            //GC.WaitForPendingFinalizers();

            //btn_new_load.Enabled = true;
        }


        public class ResultGetData
        {            
            public string Successfully { get; set; }
            public string Shop { get; set; }
            public string NumCash { get; set; }
            public string Version { get; set; }              
        }


        /// <summary>
        /// Загрузка только измененнных данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_update_only_Click(object sender, EventArgs e)
        {            
            if (!MainStaticClass.service_is_worker())
            {
                MessageBox.Show("Веб сервис недоступен");
                return;
            }

            check_temp_tables();

            //Получить параметра для запроса на сервер 
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            if (nick_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить название магазина ");
                return;
            }

            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (code_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить код магазина ");
                return;
            }
            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
            string data_encrypt = "";
            using (QueryPacketData queryPacketData = new QueryPacketData())
            {
                queryPacketData.NickShop = nick_shop;
                queryPacketData.CodeShop = code_shop;
                queryPacketData.LastDateDownloadTovar = last_date_download_tovars().ToString("dd-MM-yyyy");
                queryPacketData.Version = MainStaticClass.version().Replace(".", "");
                string data = JsonConvert.SerializeObject(queryPacketData, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                data_encrypt = CryptorEngine.Encrypt(data, true, key);
            }

            List<string> queries = new List<string>();//Список запросов                              
            try
            {
                using (LoadPacketData loadPacketData = getLoadPacketDataUpdateOnly(nick_shop, data_encrypt, key))
                {
                    if (!loadPacketData.PacketIsFull)
                    {
                        MessageBox.Show("Неудачная попытка получения данных");
                        return;
                    }
                    if (loadPacketData.Exchange)
                    {
                        MessageBox.Show("Пакет данных получен во время обновления данных на сервере, загрузка прервана");
                        return;
                    }
                    queries.Add("Delete from advertisement");

                    if (loadPacketData.ListPromoText != null)
                    {
                        if (loadPacketData.ListPromoText.Count > 0)
                        {                            
                            foreach (PromoText promoText in loadPacketData.ListPromoText)
                            {
                                queries.Add("INSERT INTO advertisement(advertisement_text,num_str,picture)VALUES ('" + promoText.AdvertisementText + "'," + promoText.NumStr + ",'"+promoText.Picture+"')");
                            }
                            loadPacketData.ListPromoText.Clear();
                            loadPacketData.ListPromoText = null;
                        }
                    }
                    if (loadPacketData.ListTovar.Count > 0)
                    {
                        foreach (Tovar tovar in loadPacketData.ListTovar)
                        {
                            queries.Add("INSERT INTO tovar2(code,name,retail_price,its_deleted,nds,its_certificate,percent_bonus,tnved,its_marked) VALUES(" +
                                                            tovar.Code + ",'" +
                                                            tovar.Name + "'," +
                                                            tovar.RetailPrice + "," +
                                                            tovar.ItsDeleted + "," +
                                                            tovar.Nds + "," +
                                                            tovar.ItsCertificate + "," +
                                                            tovar.PercentBonus + ",'" +
                                                            //tovar.TnVed + "')");
                                                            tovar.TnVed + "'," +
                                                            tovar.ItsMarked + ")");
                        }
                        loadPacketData.ListTovar.Clear();
                        loadPacketData.ListTovar = null;
                    }

                    //queries.Add("INSERT INTO tovar SELECT F.code, F.name, F.retail_price, F.its_deleted, F.nds, F.its_certificate, F.percent_bonus, F.tnved FROM(SELECT tovar2.code AS code, tovar.code AS code2, tovar2.name, tovar2.retail_price, tovar2.its_deleted, tovar2.nds, tovar2.its_certificate, tovar2.percent_bonus, tovar2.tnved  FROM tovar2 left join tovar on tovar2.code = tovar.code)AS F WHERE code2 ISNULL");
                    //queries.Add("UPDATE tovar SET name = tovar2.name,retail_price = tovar2.retail_price, its_deleted=tovar2.its_deleted,nds=tovar2.nds,its_certificate = tovar2.its_certificate,percent_bonus = tovar2.percent_bonus,tnved = tovar2.tnved FROM tovar2 where tovar.code=tovar2.code");                    
                    queries.Add("INSERT INTO tovar SELECT F.code, F.name, F.retail_price, F.its_deleted, F.nds, F.its_certificate, F.percent_bonus, F.tnved,F.its_marked FROM(SELECT tovar2.code AS code, tovar.code AS code2, tovar2.name, tovar2.retail_price, tovar2.its_deleted, tovar2.nds, tovar2.its_certificate, tovar2.percent_bonus, tovar2.tnved,tovar2.its_marked  FROM tovar2 left join tovar on tovar2.code = tovar.code)AS F WHERE code2 ISNULL");
                    queries.Add("UPDATE tovar SET name = tovar2.name,retail_price = tovar2.retail_price, its_deleted=tovar2.its_deleted,nds=tovar2.nds,its_certificate = tovar2.its_certificate,percent_bonus = tovar2.percent_bonus,tnved = tovar2.tnved,its_marked = tovar2.its_marked FROM tovar2 where tovar.code=tovar2.code");
                    if (loadPacketData.ListBarcode.Count > 0)
                    {
                        foreach (Barcode barcode in loadPacketData.ListBarcode)
                        {
                            queries.Add("DELETE FROM barcode WHERE tovar_code=" + barcode.TovarCode);                            
                        }
                        foreach (Barcode barcode in loadPacketData.ListBarcode)
                        {
                            queries.Add("INSERT INTO barcode(tovar_code,barcode) VALUES(" + barcode.TovarCode + ",'" + barcode.BarCode + "')");
                        }
                        loadPacketData.ListBarcode.Clear();
                    }
                    if (loadPacketData.ListCharacteristic != null)
                    {
                        if (loadPacketData.ListCharacteristic.Count > 0)
                        {
                            foreach (Characteristic characteristic in loadPacketData.ListCharacteristic)
                            {
                                queries.Add("DELETE FROM characteristic WHERE tovar_code=" + characteristic.CodeTovar);
                            }
                            foreach (Characteristic characteristic in loadPacketData.ListCharacteristic)
                            {                                
                                queries.Add("INSERT INTO characteristic(tovar_code, guid, name, retail_price_characteristic) VALUES(" +
                                    characteristic.CodeTovar + ",'" +
                                    characteristic.Guid + "','" +
                                    characteristic.Name + "'," +
                                    characteristic.RetailPrice + ")");
                            }
                            loadPacketData.ListCharacteristic.Clear();
                            loadPacketData.ListCharacteristic = null;
                        }
                    }

                    if (loadPacketData.ListSertificate.Count > 0)
                    {
                        foreach (Sertificate sertificate in loadPacketData.ListSertificate)
                        {
                            queries.Add("DELETE FROM sertificates WHERE code=" + sertificate.Code);
                            queries.Add(" INSERT INTO sertificates(code, code_tovar, rating, is_active)VALUES (" +
                                sertificate.Code + "," +
                                sertificate.CodeTovar + "," +
                                sertificate.Rating + "," +
                                sertificate.IsActive + ")");
                        }
                        loadPacketData.ListSertificate.Clear();
                        loadPacketData.ListSertificate = null;
                    }


                    if (loadPacketData.ListActionHeader.Count > 0)
                    {

                        foreach (ActionHeader actionHeader in loadPacketData.ListActionHeader)
                        {
                            queries.Add("DELETE FROM action_header WHERE num_doc=" + actionHeader.NumDoc);
                            queries.Add("DELETE FROM action_table WHERE num_doc=" + actionHeader.NumDoc);
                        }

                        foreach (ActionHeader actionHeader in loadPacketData.ListActionHeader)
                        {
                            queries.Add("INSERT INTO action_header(date_started,date_end,num_doc,tip,barcode,persent,sum,comment,marker,action_by_discount,time_start,time_end," +
                            " bonus_promotion, with_old_promotion, monday, tuesday, wednesday, thursday, friday, saturday, sunday, promo_code, sum_bonus,execution_order)VALUES ('" +
                            actionHeader.DateStarted + "','" +
                            actionHeader.DateEnd + "'," +
                            actionHeader.NumDoc + "," +
                            actionHeader.Tip + ",'" +
                            actionHeader.Barcode + "'," +
                            actionHeader.Persent + "," +
                            actionHeader.sum + ",'" +
                            actionHeader.Comment + "'," +
                            //actionHeader.CodeTovar + "," +
                            actionHeader.Marker + "," +
                            actionHeader.ActionByDiscount + "," +
                            actionHeader.TimeStart + "," +
                            actionHeader.TimeEnd + "," +
                            actionHeader.BonusPromotion + "," +
                            actionHeader.WithOldPromotion + "," +
                            actionHeader.Monday + "," +
                            actionHeader.Tuesday + "," +
                            actionHeader.Wednesday + "," +
                            actionHeader.Thursday + "," +
                            actionHeader.Friday + "," +
                            actionHeader.Saturday + "," +
                            actionHeader.Sunday + "," +
                            actionHeader.PromoCode + "," +
                            //actionHeader.SumBonus + ")");
                            actionHeader.SumBonus + "," +
                            actionHeader.ExecutionOrder + ")");
                        }
                        if (loadPacketData.ListActionTable.Count > 0)
                        {
                            foreach (ActionTable actionTable in loadPacketData.ListActionTable)
                            {
                                queries.Add("INSERT INTO action_table(num_doc, num_list, code_tovar, price)VALUES(" +
                                    actionTable.NumDoc + "," +
                                    actionTable.NumList + "," +
                                      //actionTable.CodeTovar + ")");
                                    actionTable.CodeTovar + "," +
                                    actionTable.Price + ")");
                            }
                        }
                        loadPacketData.ListActionHeader.Clear();
                        loadPacketData.ListActionTable.Clear();
                        loadPacketData.ListActionHeader = null;
                        loadPacketData.ListActionTable = null;
                    }
                    else
                    {
                        MessageBox.Show("Нет данных по акциям");
                    }

                    queries.Add("Delete from action_clients");

                    if (loadPacketData.ListActionClients.Count > 0)
                    {
                        foreach (ActionClients actionClients in loadPacketData.ListActionClients)
                        {
                            queries.Add("INSERT INTO action_clients(num_doc, code_client) VALUES(" +
                                actionClients.NumDoc + "," +
                                actionClients.CodeClient + ")");
                        }
                        loadPacketData.ListActionClients.Clear();
                        loadPacketData.ListActionClients = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных " + ex.Message);
                return;
            }

            //return;

            //queries.Add("UPDATE date_sync SET tovar='" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "'");
            queries.Add("INSERT INTO date_sync(tovar) VALUES('" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "')");
            NpgsqlConnection conn = null;
            NpgsqlTransaction tran = null;
            string s = "";
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                tran = conn.BeginTransaction();
                NpgsqlCommand command = null;
                foreach (string str in queries)
                {
                    s = str;
                    command = new NpgsqlCommand(str, conn);
                    command.Transaction = tran;
                    command.ExecuteNonQuery();
                }
                queries.Clear();
                queries = null;
                tran.Commit();
                MessageBox.Show("Загрузка успешно завершена");
                conn.Close();
                command.Dispose();
                command = null;
                tran = null;
            }
            catch (NpgsqlException ex)
            {
                string error = ex.Message;
                MessageBox.Show(error, "Ошибка при импорте данных");
                MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при импорте данных");
                MessageBox.Show(s);
                if (tran != null)
                {
                    tran.Rollback();
                }

            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

        }

        private void btn_new_load_fast_Click(object sender, EventArgs e)
        {
            DateTime start = DateTime.Now;
            new_load_fast();
            DateTime finish = DateTime.Now;
            MessageBox.Show((finish - start).Seconds.ToString());
        }
    }
}



