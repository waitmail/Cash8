using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Npgsql;
using Newtonsoft.Json;
using System.Diagnostics;



namespace Cash8
{
    public partial class Main : Form
    {
        public MenuStrip menuStrip = new System.Windows.Forms.MenuStrip();
        //public delegate void load_bonus_clients();


        //Thread printThread;
        //private System.Timers.Timer timer = new System.Timers.Timer();//Таймер для печати фискального принтера
        private System.Timers.Timer timer_send_data = new System.Timers.Timer();//Таймер для печати фискального принтера


        private void get_users()
        {
            //MessageBox.Show("1");
            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 10000;
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
            string encrypt_string = CryptorEngine.Encrypt(nick_shop+"|"+code_shop, true, key);
           
            string answer="";
            try
            {
                //MessageBox.Show("2");
                answer = ds.GetUsers(MainStaticClass.Nick_Shop, encrypt_string,MainStaticClass.GetWorkSchema.ToString());
                //MessageBox.Show("3");
            }
            catch
            {
            }
            if (answer == "")
            {
                return;
            }

            

            string decrypt_string = CryptorEngine.Decrypt(answer, true, key);

            string[] delimiters = new string[] { "||" };
            string[] s = decrypt_string.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction trans = null;
           // int rezult_query = 0;
            //MessageBox.Show("4");
            try
            {
                conn.Open();
                trans = conn.BeginTransaction();
                string query = "UPDATE users SET rights=13";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();
                foreach (string str in s)
                {
                    delimiters = new string[] { "|" };
                    string[] settings = str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    //rezult_query = check_insert_or_update_user(s2, conn, trans, command);
                    //if (rezult_query == -1)
                    //{
                    //    break;
                    //}
                    query = "SELECT COUNT(*) FROM users where code=" + settings[0];
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = trans;
                    if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                    {
                        query = "INSERT INTO users(" +
                            " code," +
                            " name," +
                            " rights," +
                            " shop," +
                            " password_m," +
                            " password_b," +
                            " inn " +
                            ")VALUES (" +
                            settings[0] + ",'" +
                            settings[1] + "'," +
                            settings[2] + ",'" +
                            settings[3] + "','" +
                            settings[4].Replace(" ", "") + "','" +
                            settings[5].Replace(" ", "") + "','"+
                            settings[6] + "')";
                    }
                    else
                    {
                        query = "UPDATE users  SET " +
                            " name='" + settings[1] + "'," +
                         " rights=" + settings[2] + "," +
                         " shop='" + settings[3] + "'," +
                         " password_m='" + settings[4].Replace(" ", "") + "'," +
                         " password_b='" + settings[5].Replace(" ", "") + "', " +
                         " inn='" + settings[6].Replace(" ", "") + "' " +                         
                         " WHERE code=" + settings[0];
                    }
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                }
                //if (rezult_query != -1)
                //{
                //    if (trans != null)
                //    {
                trans.Commit();
                conn.Close();
                //    }
                //}
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Произошли ошибки sql при обновлении пользователей " + ex.Message);
                trans.Rollback();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли общие ошибки при обновлении пользователей " + ex.Message);
                trans.Rollback();
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            //MessageBox.Show("100");
        }


        private int check_insert_or_update_user(string[] settings, NpgsqlConnection conn, NpgsqlTransaction trans,NpgsqlCommand command)
        {
            int result = 1;
            //NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                //conn.Open();
                string query = "SELECT COUNT(*) FROM users where code=" + settings[0];
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {
                    query = "INSERT INTO users(" +
                        " code," +
                        " name," +
                        " rights," +
                        " shop," +
                        " password_m," +
                        " password_b" +
                        ")VALUES (" +
                        settings[0] + ",'" +
                        settings[1] + "'," +
                        settings[2] + ",'" +
                        settings[3] + "','" +
                        settings[4].Replace(" ", "") + "','" +
                        settings[5].Replace(" ", "") + "')";
                }
                else
                {
                    query = "UPDATE users  SET " +
                        " name='" + settings[1] + "'," +
                     " rights=" + settings[2] + "," +
                     " shop='" + settings[3] + "'," +
                     " password_m='" + settings[4].Replace(" ", "") + "'," +
                     " password_b='" + settings[5].Replace(" ", "") + "' " +
                     " WHERE code=" + settings[0];
                }
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Произошла ошибка при обновлении пользователей "+ex.Message);
                result = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении пользователей "+ex.Message);
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



        public Main()
        {             
            this.WindowState = FormWindowState.Maximized;
            this.KeyPreview = true;
            this.Load += new EventHandler(Main_Load);
        }



        private void getShiftStatus()
        {            
            try
            {
                Cash8.FiscallPrintJason.RootObject result = FiscallPrintJason.execute_operator_type("getShiftStatus");
                if (result != null)
                {
                    if (result.results[0].status == "ready")//Задание выполнено успешно 
                    {
                        if (result.results[0].result.shiftStatus.state != "closed")
                        {
                            if ((DateTime.Now - result.results[0].result.shiftStatus.expiredTime).TotalHours > 0)
                            {
                                MessageBox.Show(" Период открытой смены превысил 24 часа !!!\r\n СНИМИТЕ Z-ОТЧЁТ. ЕСЛИ СОМНЕВАЕТЕСЬ В ЧЁМ-ТО, ТО ВСЁ РАВНО СНИМИТЕ Z-ОТЧЁТ");
                            }
                        }
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
                MessageBox.Show("getShiftStatus" + ex.Message);
            }
            
 
        }

        private void UploadTempCodeClients()
        {
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

            StringBuilder sb = new StringBuilder();

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " SELECT old_code_client, new_code_client, date_time_change  FROM temp_code_clients;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    sb.Append("'" + reader["old_code_client"].ToString() + "','" + reader["new_code_client"].ToString().Trim() + "','" + nick_shop + "','" + reader.GetDateTime(2).ToString("dd-MM-yyyy HH:mm:ss") + "'" + "|");
                }
                reader.Close();
                reader.Dispose();
                //conn.Close();

                if (!MainStaticClass.service_is_worker())
                {
                    MessageBox.Show("Веб сервис недоступен");
                    return;
                }
                Cash8.DS.DS ds = MainStaticClass.get_ds();
                ds.Timeout = 20000;

                //Получить параметра для запроса на сервер                                 
                string count_day = CryptorEngine.get_count_day();
                string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
                string encrypt_string = CryptorEngine.Encrypt(sb.ToString(), true, key);
                string answer = ds.UploadCodeClients(nick_shop, encrypt_string,MainStaticClass.GetWorkSchema.ToString());
                if (answer == "1")
                {
                    query = "DELETE FROM temp_code_clients";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
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
        }



        public class ChangeStatusClients : IDisposable
        {
            public string NickShop { get; set; }
            public string NumCash { get; set; }
            public List<ChangeStatusClient> ListChangeStatusClient { get; set; }

            void IDisposable.Dispose()
            {

            }
        }

        public class ChangeStatusClient
        {
            public string Client { get; set; }
            public string DateTimeChangeStatus { get; set; }
            public string new_phone_number { get; set; }

        }
                
        private void UploadChangeStatusClients()
        {
            ChangeStatusClients changeStatusClients = new ChangeStatusClients();
            changeStatusClients.NickShop = MainStaticClass.Nick_Shop;
            changeStatusClients.NumCash = MainStaticClass.CashDeskNumber.ToString();            
            changeStatusClients.ListChangeStatusClient = new List<ChangeStatusClient>();
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT client, date_change,new_phone_number FROM public.client_with_changed_status_to_send";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ChangeStatusClient changeStatusClient = new ChangeStatusClient();
                    changeStatusClient.Client = reader["client"].ToString();
                    changeStatusClient.DateTimeChangeStatus = Convert.ToDateTime(reader["date_change"]).ToString("dd-MM-yyyy HH:mm:ss");
                    changeStatusClient.new_phone_number = reader["new_phone_number"].ToString();
                    changeStatusClients.ListChangeStatusClient.Add(changeStatusClient);
                }
                reader.Close();                

                if (!MainStaticClass.service_is_worker())
                {
                    MessageBox.Show("Веб сервис недоступен");
                    return;
                }
                Cash8.DS.DS ds = MainStaticClass.get_ds();
                ds.Timeout = 20000;

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
                string data = JsonConvert.SerializeObject(changeStatusClients , Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                string encrypt_string = CryptorEngine.Encrypt(data, true, key);

                string answer = ds.UploadChangeStatusClients(nick_shop,encrypt_string,MainStaticClass.GetWorkSchema.ToString());
                if (answer == "1")
                {
                    query = "DELETE FROM client_with_changed_status_to_send";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();                   
                }                
                else
                {
                    MessageBox.Show("Произошли ошибки на сервере при передаче статусов клиентов");
                }
                command.Dispose();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при отправке покупателей с измененным статусом "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при отправке покупателей с измененным статусом " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public class PhoneClient
        {
            public string NumPhone { get; set; }
            public string ClientCode { get; set; }
        }
        
        public class PhonesClients : IDisposable
        {
            public string Version { get; set; }
            public string NickShop { get; set; }
            public string CodeShop { get; set; }
            public List<PhoneClient> ListPhoneClient { get; set; }

            void IDisposable.Dispose()
            {

            }
        }

        /// <summary>
        /// Уменьшенное количестов в чеке
        /// или удаленная строка
        /// </summary>
        public class DeletedItem
        {
            public string num_doc { get; set; }
            public string num_cash { get; set; }
            public string date_time_start { get; set; }
            public string date_time_action { get; set; }
            public string tovar { get; set; }
            public string quantity { get; set; }
            public string type_of_operation { get; set; }
        }

        public class DeletedItems : IDisposable
        {
            public string Version { get; set; }
            public string NickShop { get; set; }
            public string CodeShop { get; set; }
            public List<DeletedItem> ListDeletedItem { get; set; }

            void IDisposable.Dispose()
            {

            }
        }

        private void UploadDeletedItems()
        {
            DeletedItems deletedItems = new DeletedItems();
            deletedItems.CodeShop     = MainStaticClass.Code_Shop;
            deletedItems.NickShop     = MainStaticClass.Nick_Shop;
            deletedItems.ListDeletedItem = new List<DeletedItem>();
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT num_doc, num_cash, date_time_start, date_time_action, tovar, quantity, type_of_operation FROM deleted_items;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DeletedItem deletedItem = new DeletedItem();
                    deletedItem.num_doc = reader["num_doc"].ToString();
                    deletedItem.num_cash = reader["num_cash"].ToString();
                    deletedItem.date_time_start = reader["date_time_start"].ToString();
                    deletedItem.date_time_action = reader["date_time_action"].ToString();
                    deletedItem.tovar = reader["tovar"].ToString();
                    deletedItem.quantity = reader["quantity"].ToString();
                    deletedItem.type_of_operation = reader["type_of_operation"].ToString();
                    deletedItems.ListDeletedItem.Add(deletedItem);
                }
                reader.Close();
                reader.Dispose();

                if (deletedItems.ListDeletedItem.Count == 0)
                {
                    return;
                }

                if (!MainStaticClass.service_is_worker())
                {
                    MessageBox.Show("Веб сервис недоступен");
                    return;
                }
                Cash8.DS.DS ds = MainStaticClass.get_ds();
                ds.Timeout = 20000;

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
                string data = JsonConvert.SerializeObject(deletedItems, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                string encrypt_string = CryptorEngine.Encrypt(data, true, key);
                string answer = ds.UploadDeletedItems(nick_shop, encrypt_string,MainStaticClass.GetWorkSchema.ToString());
                if (answer == "1")
                {
                    query = "DELETE FROM deleted_items";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("Произошли ошибки при передаче удаленных строк");
                }
                command.Dispose();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при передаче удаленных строк " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }


        }


        private void UploadPhoneClients()
        {
            //StringBuilder sb = new StringBuilder();
            PhonesClients phonesClients = new PhonesClients();
            phonesClients.CodeShop = MainStaticClass.Code_Shop;
            phonesClients.NickShop = MainStaticClass.Nick_Shop;
            phonesClients.ListPhoneClient = new List<PhoneClient>();

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " SELECT barcode, phone  FROM temp_phone_clients; ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    PhoneClient phoneClient = new PhoneClient();
                    phoneClient.NumPhone = reader["phone"].ToString().Trim();
                    phoneClient.ClientCode = reader["barcode"].ToString();
                    phonesClients.ListPhoneClient.Add(phoneClient);
                }
                reader.Close();
                reader.Dispose();

                if (phonesClients.ListPhoneClient.Count == 0)
                {
                    return;
                }

                if (!MainStaticClass.service_is_worker())
                {
                    MessageBox.Show("Веб сервис недоступен");
                    return;
                }
                Cash8.DS.DS ds = MainStaticClass.get_ds();
                ds.Timeout = 20000;

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
                string data = JsonConvert.SerializeObject(phonesClients, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                string encrypt_string = CryptorEngine.Encrypt(data, true, key);
                string answer = ds.UploadPhoneClients(nick_shop, encrypt_string,MainStaticClass.GetWorkSchema.ToString());
                if (answer == "1")
                {
                    query = "DELETE FROM temp_phone_clients";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                else
                {
                    MessageBox.Show("Произошли ошибки на сервере при передаче телефонов клиентов");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при передаче телефонов клиентов " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
                
        private void timer_send_data_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MainStaticClass.SendOnlineStatus();
            if (MainStaticClass.Last_Write_Check > MainStaticClass.Last_Send_Last_Successful_Sending)
            {
                SendDataOnSalesPortions sdsp = new SendDataOnSalesPortions();
                sdsp.send_sales_data_Click(null, null);
                sdsp.Dispose();

                if (MainStaticClass.PassPromo != "")
                {
                    if (MainStaticClass.GetWorkSchema == 1)
                    {
                        //SentDataOnBonus sentDataOnBonus = new SentDataOnBonus();
                        //sentDataOnBonus.run_in_the_background = true;
                        //sentDataOnBonus.sent_Click(null, null);
                        //sentDataOnBonus.Dispose();
                    }
                    else if (MainStaticClass.GetWorkSchema == 2)
                    {
                        SentDataOnBonusEva sentDataOnBonusEva = new SentDataOnBonusEva();
                        sentDataOnBonusEva.run_in_the_background = true;
                        sentDataOnBonusEva.sent_Click(null, null);
                        sentDataOnBonusEva.Dispose();
                    }
                }
            }
        }


        //private void check_and_update_npgsql()
        //{
        //    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(Application.StartupPath+"/Npgsql.dll");
        //    int cash_version = int.Parse(myFileVersionInfo.FileVersion.Replace(".",""));
        //    if (cash_version == 20100)//Старая версия Npgsql 
        //    {
        //        if (!Directory.Exists(Application.StartupPath + "/PreviousNpgsql"))
        //        {
        //            Directory.CreateDirectory(Application.StartupPath + "/PreviousNpgsql");
        //        }
        //        if (!Directory.Exists(Application.StartupPath + "/UpdateNpgsql"))
        //        {
        //            Directory.CreateDirectory(Application.StartupPath + "/UpdateNpgsql");
        //        }


        //        if (!MainStaticClass.service_is_worker())
        //        {
        //            return;
        //        }

        //        Cash8.DS.DS ds = MainStaticClass.get_ds();
        //        ds.Timeout = 50000;

        //        //Получить параметра для запроса на сервер 
        //        string nick_shop = MainStaticClass.Nick_Shop.Trim();
        //        if (nick_shop.Trim().Length == 0)
        //        {
        //            return;
        //        }

        //        string code_shop = MainStaticClass.Code_Shop.Trim();
        //        if (code_shop.Trim().Length == 0)
        //        {
        //            return;
        //        }

        //        string count_day = CryptorEngine.get_count_day();

        //        string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
        //        //старая версия
        //        //string my_version = version.Substring(0, 2) + "-" + version.Substring(2, 2) + "-" + version.Substring(4, 4);                

        //        string data = code_shop.Trim() + "|" + code_shop.Trim();
        //        byte[] result_web_query = new byte[0];
        //        try
        //        {
        //            result_web_query = ds.GetNpgsqlNew(nick_shop, CryptorEngine.Encrypt(data, true, key));
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            return;
        //        }

        //        File.WriteAllBytes(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll", result_web_query);

        //        File.Copy(Application.StartupPath + "/Npgsql.dll", Application.StartupPath + "/PreviousNpgsql/Npgsql.dll", true);
        //        //                File.Copy(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll", Application.StartupPath + "/Npgsql.dll", true);

        //        //MessageBox.Show(" Библиотека Npgsql.dll успешно обновлена");
        //        Application.Exit();
        //    }

        //}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //if (MainStaticClass.GetWorkSchema == 1)
            //{
            if (e.KeyCode == Keys.F12)
            {
                this.menuStrip.Items.Clear();
                MainStaticClass.Main.start_interface_switching();
            }
            //}
            //else
            //{
            //    if (e.KeyCode == Keys.D)
            //    {
            //        this.menuStrip.Items.Clear();
            //        MainStaticClass.Main.start_interface_switching();
            //    }
            //}             
        }
        
        private void параметрыБазыДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingConnect sc = new SettingConnect();
            sc.MdiParent = this;
            sc.Show();
        }


        public void start_interface_switching()
        {
            Interface_switching isw = new Interface_switching();
            isw.ShowDialog();
        }

        private void append_column(string query)
        {

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (NpgsqlException)
            {
                //MessageBox.Show(ex.Message,query);
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message, query);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        void Main_Load(object sender, System.EventArgs e)
        {           

            if (File.Exists(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll"))
            {
                if (File.ReadAllBytes(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll").Length > 0)
                {
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(Application.StartupPath + "/Npgsql.dll");
                    int cash_version = int.Parse(myFileVersionInfo.FileVersion.Replace(".", ""));
                    if (cash_version == 20100)//Старая версия Npgsql 
                    {
                        File.Copy(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll", Application.StartupPath + "/Npgsql.dll", true);
                    }
                }
            }
            
            MainStaticClass.Main = this;
            this.IsMdiContainer = true;
            MainStaticClass.Last_Send_Last_Successful_Sending = DateTime.Now;
            MainStaticClass.Last_Write_Check = DateTime.Now.AddSeconds(1);

            //string verssssion = new Version(System.Windows.Forms.Application.ProductVersion).ToString();

            if (File.Exists(Application.StartupPath + "/Setting.gaa") == false)
            {
                MessageBox.Show("Не обнаружен файл " + " Setting.gaa " + " с параметрами для программы " + Application.StartupPath);
            }
            else
            {
                Cash8.MainStaticClass.loadConfig(Application.StartupPath + "/Setting.gaa");
                Text += "   " + Cash8.MainStaticClass.CashDeskNumber;
                Text += " | " + Cash8.MainStaticClass.Nick_Shop;
                Text += " | " + Cash8.MainStaticClass.version();
                check_add_field();//gaa
            }            
            update_unloading_period();
            int result = MainStaticClass.get_unloading_interval();
            if (result != 0)
            {
                timer_send_data.Interval = result * 60000;
                //MessageBox.Show(timer_send_data.Interval.ToString());
                timer_send_data.Start();
                timer_send_data.Elapsed += new System.Timers.ElapsedEventHandler(timer_send_data_Elapsed);
                //timer_send_data_Elapsed(null, null);//при старте сделать выгрузку               при отсутствмм связи программа вешается

                

                //Thread t2 = new Thread(load_bonus_cards);
                //t2.IsBackground = true;
                //t2.Start();               
            }
            
            LoadProgramFromInternet lpfi = new LoadProgramFromInternet();
            lpfi.show_phone = true;
            lpfi.check_new_version_programm();            
            bool new_version_of_the_program_exist = lpfi.new_version_of_the_program;
            lpfi.Dispose();
            
            if (new_version_of_the_program_exist)
            {
                обновлениеПрограммыToolStripMenuItem_Click(null, null);
            }

            Thread t = new Thread(load_bonus_clients);
            t.IsBackground = true;
            t.Start();

            if (MainStaticClass.GetWorkSchema == 1)//Это условие будет работать только для ЧД
            {
                //Thread t = new Thread(load_bonus_clients);
                //t.IsBackground = true;
                //t.Start();

                UploadPhoneClients();
                UploadChangeStatusClients();
                check_failed_input_phone();
            }

            MainStaticClass.delete_old_checks(MainStaticClass.GetMinDateWork);
            get_users();
            //MainStaticClass.Use_Envd = check_envd();
            //if (DateTime.Now > new DateTime(2021, 1, 1) && (MainStaticClass.Use_Envd))
            //{
            //    MessageBox.Show("Схема ЕНВД в 1 января 2021 года не работает, необходимо это исправить");
            //    Constants constants = new Constants();
            //    constants.ShowDialog();                
            //    this.Close();
            //    return;
            //}

            MainStaticClass.SystemTaxation = check_system_taxation();
            MainStaticClass.delete_all_events_in_log(MainStaticClass.GetMinDateWork);
            if (MainStaticClass.Use_Fiscall_Print)
            {
                getShiftStatus();
            }
            
            //if (MainStaticClass.PassPromo == "")//Пароля нет надо его запросить
            //{
            get_login_and_pass_on_bonus_programm();
            //}            
            //check_and_update_npgsql();
            UploadDeletedItems();//передача удаленных строк и строк с изменением количества вниз

            if (MainStaticClass.Nick_Shop == "A01")//Для отладки нового механизма пока что сделаю такую заплатку
            {
               MainStaticClass.UseOldProcessiingActions = false;
            }

            this.menuStrip.Items.Clear();
            MainStaticClass.Main.start_interface_switching();
        }


        private void get_login_and_pass_on_bonus_programm()
        {
            if (!MainStaticClass.service_is_worker())
            {
                //MessageBox.Show("Веб сервис недоступен");
                return;
            }
            
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
            string key = nick_shop.Trim() + count_day.Trim() + nick_shop.Trim();
            LoginPassPromo passPromo = new LoginPassPromo();
            passPromo.CashDeskNumber = MainStaticClass.CashDeskNumber.ToString();
            passPromo.PassPromoForCashDeskNumber = "";

            string data = JsonConvert.SerializeObject(passPromo, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string data_encrypt = CryptorEngine.Encrypt(data, true, key);
            try
            {
                using (Cash8.DS.DS ds = MainStaticClass.get_ds())
                {
                    ds.Timeout = 10000;
                    string result_web_query = ds.GetParametersOnBonusProgram(nick_shop, data_encrypt,MainStaticClass.GetWorkSchema.ToString());                    
                    string decrypt_data = CryptorEngine.Decrypt(result_web_query, true, key);
                    if (decrypt_data != "-1")
                    {
                        passPromo = JsonConvert.DeserializeObject<LoginPassPromo>(decrypt_data);
                        if (passPromo.PassPromoForCashDeskNumber != "")
                        {
                            update_login_and_pass_promo(passPromo.LoginPromoForCashDeskNumber,passPromo.PassPromoForCashDeskNumber);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);                
            }
        }


        private void update_login_and_pass_promo(string login_promo,string pass_promo)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "UPDATE constants SET pass_promo='" + pass_promo+"', login_promo = '"+login_promo+"'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                command.Dispose();
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
        }
        
        public class LoginPassPromo
        {
            public string PassPromoForCashDeskNumber { get; set; }
            public string CashDeskNumber { get; set; }
            public string LoginPromoForCashDeskNumber { get; set; }
        }

        /// <summary>
        /// Проверка таблицы failed_input_phone
        /// при старте программы, если это сегодня первый старт 
        /// и документов продажи еще нет, тогда очищаем таблицу
        /// </summary>
        private void check_failed_input_phone()
        {
             NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

             try
             {
                 conn.Open();
                 string query = "SELECT COUNT(*) FROM checks_header where date_time_start between '" + DateTime.Now.ToString("dd-MM-yyyy") + " 00:00:00' AND  '" + DateTime.Now.ToString("dd-MM-yyyy") + " 23:59:59'";
                 NpgsqlCommand command = new NpgsqlCommand(query, conn);
                 if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                 {
                     query = "DELETE FROM failed_input_phone";
                     command = new NpgsqlCommand(query, conn);
                     command.ExecuteNonQuery();
                 }
                 conn.Close();
                 command.Dispose();
             }
             catch (NpgsqlException ex)
             {
                 MessageBox.Show("Ошибка при очистке счетчика ошибочно введенных номеров телефонов" + ex.Message);
             }
             catch (Exception ex)
             {
                 MessageBox.Show("Ошибка при очистке счетчика ошибочно введенных номеров телефонов" + ex.Message);
             }
             finally
             {
                 if (conn.State == ConnectionState.Open)
                 {
                     conn.Close();
                 }
             }
        }


        private void update_unloading_period()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " UPDATE constants SET unloading_period = 4 where unloading_period > 0 ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteScalar();
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при проеверке / установке значения периода выгрузки "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проеверке / установке значения периода выгрузки " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        //private void check_and_chage_data_type_inn()
        //{
        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    NpgsqlTransaction tran = null;
        //    try
        //    {
        //        conn.Open();
        //        tran = conn.BeginTransaction();
        //        string query = "SELECT data_type FROM information_schema.columns where table_name = 'users' AND column_name = 'inn'";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        if (command.ExecuteScalar().ToString().Trim() != "character varying")//старый тип колонки в бд, меняем на новый
        //        {
        //            query = "ALTER TABLE users DROP COLUMN inn";
        //            command = new NpgsqlCommand(query, conn);
        //            command.Transaction = tran;
        //            command.ExecuteNonQuery();
        //            query = "ALTER TABLE users ADD COLUMN inn character varying(12);";
        //            command = new NpgsqlCommand(query, conn);
        //            command.Transaction = tran;
        //            command.ExecuteNonQuery();
        //        }
        //        tran.Commit();
        //        conn.Close();
        //        command.Dispose();
        //    }
        //    catch
        //    {
        //        if (tran != null)
        //        {
        //            tran.Rollback();
        //        }
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}


        private bool check_envd()
        {
            bool result = false;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT envd FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToBoolean(command.ExecuteScalar());
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка sql "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            
            return result;
        }
        
        private int check_system_taxation()
        {
            int result = 0;

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT system_taxation FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = Convert.ToInt16(command.ExecuteScalar());
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка sql check_system_taxation " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Общая ошибка check_system_taxation " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }

            return result;
        }

        private void check_add_field()
        {            
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = " SELECT execute_addcolumn  FROM constants;";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object result_query = command.ExecuteScalar();
                if (result_query.ToString() == "")
                {
                    SettingConnect sc = new SettingConnect();
                    sc.add_field_Click(null, null);
                    sc.Dispose();

                    query = " UPDATE constants SET execute_addcolumn = 1 ";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                else
                {
                    if (Convert.ToInt16(result_query) == 2)
                    {
                        SettingConnect sc = new SettingConnect();
                        sc.add_field_Click(null, null);
                        sc.Dispose();

                        query = " UPDATE constants SET execute_addcolumn = 1 ";
                        command = new NpgsqlCommand(query, conn);
                        command.ExecuteNonQuery();
                    } 
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибка при добавлении полей в БД "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибка при добавлении полей в БД " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        //void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{      

        //    timer.Stop();
        //    timer = null;
        //    //SetForegroundWindow(this);
        //    //this.Focus();            
        //    //MainStaticClass.Main.start_interface_switching();            
        //}

        private void load_bonus_clients()
        {
            LoadDataWebService ld = new LoadDataWebService();
            ld.load_bonus_clients(false);
            ld.Dispose();
        }

        //private void load_bonus_cards()
        //{
        //    LoadDataWebService ld = new LoadDataWebService();
        //    ld.load_bonus_cards(false);
        //    ld.Dispose();
        //}


        public void show_Cash_checks()
        {
            MainStaticClass.Code_right_of_user = 2;
            Cash_checks cc = new Cash_checks();
            cc.FormBorderStyle = FormBorderStyle.None;
            cc.WindowState = FormWindowState.Maximized;
            cc.MdiParent = MainStaticClass.Main;
            cc.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            /*SendDataOnSales sd = new SendDataOnSales();
            sd.show_messages = false;
            sd.send_sales_data_Click(null, null);
            sd.Dispose();*/
            
            timer_send_data.Stop();
            //SendDataOnSalesPortions sdsp = new SendDataOnSalesPortions();
            //sdsp.send_sales_data_Click(null, null);
            //sdsp.Dispose();
            timer_send_data_Elapsed(null, null);
            UploadPhoneClients();
            UploadDeletedItems();
        }


        private void загрузкаДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В этой версии не работает");  
            /*LoadData ld = new LoadData();
            ld.MdiParent = this;
            ld.Show();*/
            
            //MessageBox.Show("В этой версии не работает");
        }

        //private void штрихкодыToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //((Cash.Barcode)(this.MdiChildren[0])).Text
        //    Barcode bc = new Barcode();
        //    bc.MdiParent = this;
        //    bc.Show();
        //}

        //private void контрагентыToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Clients cl = new Clients();
        //    cl.MdiParent = this;
        //    cl.Show();
        //}

        //private void пользователиToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Users us = new Users();
        //    us.MdiParent = this;
        //    us.Show();
        //}

        //private void праваToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //}

        //private void типыДисконтаToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Discount_types dtypes = new Discount_types();
        //    dtypes.MdiParent = this;
        //    dtypes.Show();
        //}

        private void кассовыеЧекиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cash_checks ccheks = new Cash_checks();
            ccheks.MdiParent = this;
            ccheks.WindowState = FormWindowState.Maximized;
            ccheks.FormBorderStyle = FormBorderStyle.Sizable;
            ccheks.TopMost = true;
            if (!MainStaticClass.exist_form(ccheks.GetType().FullName))
            {
                MainStaticClass.add_window(ccheks.GetType().FullName);
                ccheks.Show();
            }
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    //if (MainStaticClass.Use_Fiscall_Print)
        //    //{
        //    //    //if (File.Exists("/var/lock/LCK..ttyS0"))
        //    //    //{                    
        //    //    //    ProcessStartInfo startInfo = new ProcessStartInfo();
        //    //    //    startInfo.FileName = "rm";
        //    //    //    startInfo.Arguments = "/var/lock/LCK..ttyS0";
        //    //    //    Process.Start(startInfo); 
        //    //    //}
        //    //    //Work_FPTK22 fiscall = new Work_FPTK22();               
        //    //}
        //}

        //private void быстрыеТоварыToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Fast_tovar ft = new Fast_tovar();
        //    ft.MdiParent = this;
        //    ft.Show();
        //}

        private void константыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Constants cons = new Constants();
            cons.ShowDialog();
        }


        /*
         * Эта функция вызыывается потому 
         * что надо убрать создание меню
         */
        public void InitializeComponent1()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            //this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.справочникиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.номенклатураToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.штрихкодыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.контрагентыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.пользователиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.праваToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.типыДисконтаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.быстрыеТоварыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.константыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.закрытиеДняToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.фискальныйПринтерToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.журналыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.кассовыеЧекиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.акцииToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.служебныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.параметрыБазыДанныхToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузкаДанныхToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выгрузкаДанныхToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выгрузкаДанныхПоБонусамToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выгрузкаПродажToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.загрузкаДанныхЧерезИнтернетToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.обновлениеПрограммыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.оПрограммеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.справочникиToolStripMenuItem,
            this.журналыToolStripMenuItem,
            this.служебныеToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // справочникиToolStripMenuItem
            // 
            this.справочникиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            //this.номенклатураToolStripMenuItem,
            //this.штрихкодыToolStripMenuItem,
            //this.контрагентыToolStripMenuItem,
            //this.пользователиToolStripMenuItem,
            //this.праваToolStripMenuItem,
            //this.типыДисконтаToolStripMenuItem,
            //this.быстрыеТоварыToolStripMenuItem,
            this.константыToolStripMenuItem,
            //this.закрытиеДняToolStripMenuItem,
            this.фискальныйПринтерToolStripMenuItem,
            this.выходToolStripMenuItem});
            this.справочникиToolStripMenuItem.Name = "справочникиToolStripMenuItem";
            resources.ApplyResources(this.справочникиToolStripMenuItem, "справочникиToolStripMenuItem");
            // 
            // номенклатураToolStripMenuItem
            // 
            //this.номенклатураToolStripMenuItem.Name = "номенклатураToolStripMenuItem";
            //resources.ApplyResources(this.номенклатураToolStripMenuItem, "номенклатураToolStripMenuItem");
            //this.номенклатураToolStripMenuItem.Click += new System.EventHandler(this.номенклатураToolStripMenuItem_Click);
            // 
            // штрихкодыToolStripMenuItem
            // 
            //this.штрихкодыToolStripMenuItem.Name = "штрихкодыToolStripMenuItem";
            //resources.ApplyResources(this.штрихкодыToolStripMenuItem, "штрихкодыToolStripMenuItem");
            //this.штрихкодыToolStripMenuItem.Click += new System.EventHandler(this.штрихкодыToolStripMenuItem_Click);
            // 
            // контрагентыToolStripMenuItem
            // 
            //this.контрагентыToolStripMenuItem.Name = "контрагентыToolStripMenuItem";
            //resources.ApplyResources(this.контрагентыToolStripMenuItem, "контрагентыToolStripMenuItem");
            //this.контрагентыToolStripMenuItem.Click += new System.EventHandler(this.контрагентыToolStripMenuItem_Click);
            // 
            // пользователиToolStripMenuItem
            // 
            //this.пользователиToolStripMenuItem.Name = "пользователиToolStripMenuItem";
            //resources.ApplyResources(this.пользователиToolStripMenuItem, "пользователиToolStripMenuItem");
            //this.пользователиToolStripMenuItem.Click += new System.EventHandler(this.пользователиToolStripMenuItem_Click);
            // 
            // праваToolStripMenuItem
            // 
            //this.праваToolStripMenuItem.Name = "праваToolStripMenuItem";
            //resources.ApplyResources(this.праваToolStripMenuItem, "праваToolStripMenuItem");
            //this.праваToolStripMenuItem.Click += new System.EventHandler(this.праваToolStripMenuItem_Click);
            // 
            // типыДисконтаToolStripMenuItem
            // 
            //this.типыДисконтаToolStripMenuItem.Name = "типыДисконтаToolStripMenuItem";
            //resources.ApplyResources(this.типыДисконтаToolStripMenuItem, "типыДисконтаToolStripMenuItem");
            //this.типыДисконтаToolStripMenuItem.Click += new System.EventHandler(this.типыДисконтаToolStripMenuItem_Click);
            // 
            // быстрыеТоварыToolStripMenuItem
            // 
            //this.быстрыеТоварыToolStripMenuItem.Name = "быстрыеТоварыToolStripMenuItem";
            //resources.ApplyResources(this.быстрыеТоварыToolStripMenuItem, "быстрыеТоварыToolStripMenuItem");
            //this.быстрыеТоварыToolStripMenuItem.Click += new System.EventHandler(this.быстрыеТоварыToolStripMenuItem_Click);
            // 
            // константыToolStripMenuItem
            // 
            this.константыToolStripMenuItem.Name = "константыToolStripMenuItem";
            resources.ApplyResources(this.константыToolStripMenuItem, "константыToolStripMenuItem");
            this.константыToolStripMenuItem.Click += new System.EventHandler(this.константыToolStripMenuItem_Click);
            // 
            // закрытиеДняToolStripMenuItem
            // 
            //this.закрытиеДняToolStripMenuItem.Name = "закрытиеДняToolStripMenuItem";
            //resources.ApplyResources(this.закрытиеДняToolStripMenuItem, "закрытиеДняToolStripMenuItem");
            //this.закрытиеДняToolStripMenuItem.Click += new System.EventHandler(this.закрытиеДняToolStripMenuItem_Click);
            // 
            // фискальныйПринтерToolStripMenuItem
            // 
            this.фискальныйПринтерToolStripMenuItem.Name = "фискальныйПринтерToolStripMenuItem";
            resources.ApplyResources(this.фискальныйПринтерToolStripMenuItem, "фискальныйПринтерToolStripMenuItem");
            this.фискальныйПринтерToolStripMenuItem.Click += new System.EventHandler(this.фискальныйПринтерToolStripMenuItem_Click);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            resources.ApplyResources(this.выходToolStripMenuItem, "выходToolStripMenuItem");
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // журналыToolStripMenuItem
            // 
            this.журналыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.кассовыеЧекиToolStripMenuItem,
            this.акцииToolStripMenuItem});
            this.журналыToolStripMenuItem.Name = "журналыToolStripMenuItem";
            resources.ApplyResources(this.журналыToolStripMenuItem, "журналыToolStripMenuItem");
            // 
            // кассовыеЧекиToolStripMenuItem
            // 
            this.кассовыеЧекиToolStripMenuItem.Name = "кассовыеЧекиToolStripMenuItem";
            resources.ApplyResources(this.кассовыеЧекиToolStripMenuItem, "кассовыеЧекиToolStripMenuItem");
            this.кассовыеЧекиToolStripMenuItem.Click += new System.EventHandler(this.кассовыеЧекиToolStripMenuItem_Click);
            // 
            // акцииToolStripMenuItem
            // 
            this.акцииToolStripMenuItem.Name = "акцииToolStripMenuItem";
            resources.ApplyResources(this.акцииToolStripMenuItem, "акцииToolStripMenuItem");
            this.акцииToolStripMenuItem.Click += new System.EventHandler(this.акцииToolStripMenuItem_Click);
            // 
            // служебныеToolStripMenuItem
            // 
            this.служебныеToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.параметрыБазыДанныхToolStripMenuItem,
            //this.загрузкаДанныхToolStripMenuItem,
            //this.выгрузкаДанныхToolStripMenuItem,
            this.выгрузкаДанныхПоБонусамToolStripMenuItem,
            //this.выгрузкаПродажToolStripMenuItem,
            this.загрузкаДанныхЧерезИнтернетToolStripMenuItem,
            this.обновлениеПрограммыToolStripMenuItem,
            this.оПрограммеToolStripMenuItem});
            this.служебныеToolStripMenuItem.Name = "служебныеToolStripMenuItem";
            resources.ApplyResources(this.служебныеToolStripMenuItem, "служебныеToolStripMenuItem");
            // 
            // параметрыБазыДанныхToolStripMenuItem
            // 
            this.параметрыБазыДанныхToolStripMenuItem.Name = "параметрыБазыДанныхToolStripMenuItem";
            resources.ApplyResources(this.параметрыБазыДанныхToolStripMenuItem, "параметрыБазыДанныхToolStripMenuItem");
            this.параметрыБазыДанныхToolStripMenuItem.Click += new System.EventHandler(this.параметрыБазыДанныхToolStripMenuItem_Click);
            // 
            // загрузкаДанныхToolStripMenuItem
            // 
            this.загрузкаДанныхToolStripMenuItem.Name = "загрузкаДанныхToolStripMenuItem";
            resources.ApplyResources(this.загрузкаДанныхToolStripMenuItem, "загрузкаДанныхToolStripMenuItem");
            this.загрузкаДанныхToolStripMenuItem.Click += new System.EventHandler(this.загрузкаДанныхToolStripMenuItem_Click);
            // 
            // выгрузкаДанныхToolStripMenuItem
            // 
            //this.выгрузкаДанныхToolStripMenuItem.Name = "выгрузкаДанныхToolStripMenuItem";
            //resources.ApplyResources(this.выгрузкаДанныхToolStripMenuItem, "выгрузкаДанныхToolStripMenuItem");
            //this.выгрузкаДанныхToolStripMenuItem.Click += new System.EventHandler(this.выгрузкаДанныхToolStripMenuItem_Click);
            // 
            // выгрузкаДанныхПоБонусамToolStripMenuItem
            // 
            this.выгрузкаДанныхПоБонусамToolStripMenuItem.Name = "выгрузкаДанныхПоБонусамToolStripMenuItem";
            resources.ApplyResources(this.выгрузкаДанныхПоБонусамToolStripMenuItem, "выгрузкаДанныхПоБонусамToolStripMenuItem");
            this.выгрузкаДанныхПоБонусамToolStripMenuItem.Click += new System.EventHandler(this.выгрузкаДанныхПоБонусамToolStripMenuItem_Click);
            // 
            // выгрузкаПродажToolStripMenuItem
            // 
            //this.выгрузкаПродажToolStripMenuItem.Name = "выгрузкаПродажToolStripMenuItem";
            //resources.ApplyResources(this.выгрузкаПродажToolStripMenuItem, "выгрузкаПродажToolStripMenuItem");
            //this.выгрузкаПродажToolStripMenuItem.Click += new System.EventHandler(this.выгрузкаПродажToolStripMenuItem_Click);
            // 
            // загрузкаДанныхЧерезИнтернетToolStripMenuItem
            // 
            this.загрузкаДанныхЧерезИнтернетToolStripMenuItem.Name = "загрузкаДанныхЧерезИнтернетToolStripMenuItem";
            resources.ApplyResources(this.загрузкаДанныхЧерезИнтернетToolStripMenuItem, "загрузкаДанныхЧерезИнтернетToolStripMenuItem");
            this.загрузкаДанныхЧерезИнтернетToolStripMenuItem.Click += new System.EventHandler(this.загрузкаДанныхЧерезИнтернетToolStripMenuItem_Click);            
            // 
            // оПрограммеToolStripMenuItem
            // 
            this.оПрограммеToolStripMenuItem.Name = "оПрограммеToolStripMenuItem";
            resources.ApplyResources(this.оПрограммеToolStripMenuItem, "оПрограммеToolStripMenuItem");
            this.оПрограммеToolStripMenuItem.Click += new System.EventHandler(this.оПрограммеToolStripMenuItem_Click);
            // 
            // обновлениеПрограммыToolStripMenuItem
            // 
            this.обновлениеПрограммыToolStripMenuItem.Name = "обновлениеПрограммыToolStripMenuItem";
            resources.ApplyResources(this.обновлениеПрограммыToolStripMenuItem, "обновлениеПрограммыToolStripMenuItem");
            this.обновлениеПрограммыToolStripMenuItem.Click += new System.EventHandler(this.обновлениеПрограммыToolStripMenuItem_Click);
            // 
            // Main
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Main_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void выгрузкаДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Unload un = new Unload();
            un.ShowDialog();
             */
            MessageBox.Show("В этой версии не работает");  
        }

        //private void спискиАкционныхТоваровToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Lists_actions_tovar list = new Lists_actions_tovar();
        //    list.ShowDialog();
        //}

        private void акцииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Actions actions = new Actions();
            actions.ShowDialog();
        }

        //private void закрытиеДняToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    Сlosing_Day cd = new Сlosing_Day();
        //    cd.ShowDialog();
        //}

        private void фискальныйПринтерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Cash_check doc = new Cash_check();
            //doc.ShowDialog();
            //new_document = false;
            //loaddocuments();
            //Mini_FP_6 mini = new Mini_FP_6();
            //mini.ShowDialog();            
            FPTK22 fptk = new FPTK22();
            fptk.ShowDialog();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            MessageBox.Show("Номер версии программы " +Application.ProductVersion+"\r\n"+ "Номер версии ОС "+Environment.OSVersion.VersionString);            
        }

        //private void выгрузкаПродажToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    SendDataOnSales sd = new SendDataOnSales();
        //    sd.ShowDialog();
        //}

        private void выгрузкаДанныхПоБонусамToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MainStaticClass.GetWorkSchema == 1)
            {
                SentDataOnBonus sdb = new SentDataOnBonus();
                sdb.ShowDialog();
            }
            else if (MainStaticClass.GetWorkSchema == 2)
            {
                SentDataOnBonusEva sentDataOnBonusEva = new SentDataOnBonusEva();
                sentDataOnBonusEva.ShowDialog();
            }
            
            //MessageBox.Show("В этой версии не работает");
        } 
        


        private void загрузкаДанныхЧерезИнтернетToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDataWebService ld = new LoadDataWebService();
            ld.ShowDialog();
            ld.Dispose();
            ld = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }

        private void обновлениеПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadProgramFromInternet lpfi = new LoadProgramFromInternet();
            DialogResult dlg_rez = lpfi.ShowDialog();
            lpfi.Dispose();
            if (dlg_rez == DialogResult.Yes)
            {
                this.Close();
                //this.Dispose();
                Application.Exit();
            }
        }

        private void текущаяДатаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
        }

        //private void загрузкаДанныхПоИнтернетуToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UnloadDataToTrassir udtr = new UnloadDataToTrassir();
        //    udtr.ShowDialog();
        //}
        
        //private void InitializeComponent()
        //{
        //    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
        //    this.SuspendLayout();
        //    // 
        //    // Main
        //    // 
        //    resources.ApplyResources(this, "$this");
        //    this.IsMdiContainer = true;
        //    this.Name = "Main";
        //    this.ResumeLayout(false);
        //}       
    }
}
