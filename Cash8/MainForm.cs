using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Npgsql;
using Newtonsoft.Json;
using System.Transactions;
using System.Threading.Tasks;


namespace Cash8
{
    public partial class Main : Form
    {
        public MenuStrip menuStrip = new System.Windows.Forms.MenuStrip();        
        private System.Timers.Timer timer_send_data = new System.Timers.Timer();

        public class Users
        {
            public List<User> list_users { get; set; }
        }

        public class User
        {
            public string shop { get; set; }
            public string user_id { get; set; }
            public string name { get; set; }
            public string rights { get; set; }
            public string password_m { get; set; }
            public string password_b { get; set; }
        }


        private void get_users()
        {            
            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 3000;            
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
            string encrypt_string = CryptorEngine.Encrypt(nick_shop + "|" + code_shop, true, key);

            string answer = "";
            try
            {                
                answer = ds.GetUsers(MainStaticClass.Nick_Shop, encrypt_string, "4");             
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при при получении пользователей от веб сервиса " + ex.Message+".","Синхронизация пользователей");
            }
            if (answer == "")
            {
                return;
            }

            string decrypt_string = CryptorEngine.Decrypt(answer, true, key);
            Users users = JsonConvert.DeserializeObject<Users>(decrypt_string);           
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction trans = null;
            
            try
            {
                conn.Open();
                trans = conn.BeginTransaction();
                string query = "UPDATE users SET rights=13";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();
                foreach (User user in users.list_users)
                {                   
                    query = "DELETE FROM public.users	WHERE inn='" + user.user_id + "';";
                   
                    query += "INSERT INTO users(" +
                        " code," +
                        " name," +
                        " rights," +
                        " shop," +
                        " password_m," +
                        " password_b," +
                        " inn " +
                        ")VALUES ('" +
                        user.user_id + "','" +
                        user.name + "'," +
                        user.rights + ",'" +
                        user.shop + "','" +
                        user.password_m + "','" +
                        user.password_b + "','" +
                        user.user_id + "')";
                  
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                }
              
                trans.Commit();
                conn.Close();
              
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
        }


        private int check_insert_or_update_user(string[] settings, NpgsqlConnection conn, NpgsqlTransaction trans, NpgsqlCommand command)
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
                MessageBox.Show(" Произошла ошибка при обновлении пользователей " + ex.Message);
                result = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при обновлении пользователей " + ex.Message);
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
            if (MainStaticClass.PrintingUsingLibraries == 0)
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
                                    //MessageBox.Show(" Период открытой смены превысил 24 часа !!!\r\n СНИМИТЕ Z-ОТЧЁТ. ЕСЛИ СОМНЕВАЕТЕСЬ В ЧЁМ-ТО, ТО ВСЁ РАВНО СНИМИТЕ Z-ОТЧЁТ");
                                    MessageBox.Show(" Период открытой смены превысил 24 часа!\r\nСмена будет закрыта автоматически!\r\n" +
                                                    "В ИТ отдел звонить не надо, если хотите кому нибудь позвонить по этому вопросу, звоните в бухгалтерию");
                                    FPTK22 fPTK22 = new FPTK22();
                                    fPTK22.z_report_Click(null, null);
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
            else
            {
                PrintingUsingLibraries printing = new PrintingUsingLibraries();
                printing.getShiftStatus();
            }

        }

        //private void getShiftStatusLibraries()
        //{
        //    PrintingUsingLibraries printing = new PrintingUsingLibraries();
        //    printing.getShiftStatus();         
        //}

        //private void UploadTempCodeClients()
        //{
        //    string nick_shop = MainStaticClass.Nick_Shop.Trim();
        //    if (nick_shop.Trim().Length == 0)
        //    {
        //        MessageBox.Show(" Не удалось получить название магазина ");
        //        return;
        //    }

        //    string code_shop = MainStaticClass.Code_Shop.Trim();
        //    if (code_shop.Trim().Length == 0)
        //    {
        //        MessageBox.Show(" Не удалось получить код магазина ");
        //        return;
        //    }

        //    StringBuilder sb = new StringBuilder();

        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

        //    try
        //    {
        //        conn.Open();
        //        string query = " SELECT old_code_client, new_code_client, date_time_change  FROM temp_code_clients;";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        NpgsqlDataReader reader = command.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            sb.Append("'" + reader["old_code_client"].ToString() + "','" + reader["new_code_client"].ToString().Trim() + "','" + nick_shop + "','" + reader.GetDateTime(2).ToString("dd-MM-yyyy HH:mm:ss") + "'" + "|");
        //        }
        //        reader.Close();
        //        reader.Dispose();
        //        //conn.Close();

        //        if (!MainStaticClass.service_is_worker())
        //        {
        //            MessageBox.Show("Веб сервис недоступен");
        //            return;
        //        }
        //        Cash8.DS.DS ds = MainStaticClass.get_ds();
        //        ds.Timeout = 20000;

        //        //Получить параметра для запроса на сервер                                 
        //        string count_day = CryptorEngine.get_count_day();
        //        string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
        //        string encrypt_string = CryptorEngine.Encrypt(sb.ToString(), true, key);
        //        string answer = ds.UploadCodeClients(nick_shop, encrypt_string,MainStaticClass.GetWorkSchema.ToString());
        //        if (answer == "1")
        //        {
        //            query = "DELETE FROM temp_code_clients";
        //            command = new NpgsqlCommand(query, conn);
        //            command.ExecuteNonQuery();
        //            command.Dispose();
        //        }
        //        conn.Close();
        //    }
        //    catch
        //    {

        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}



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
                string data = JsonConvert.SerializeObject(changeStatusClients, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                string encrypt_string = CryptorEngine.Encrypt(data, true, key);

                string answer = ds.UploadChangeStatusClients(nick_shop, encrypt_string, MainStaticClass.GetWorkSchema.ToString());
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
                MessageBox.Show("Ошибка при отправке покупателей с измененным статусом " + ex.Message);
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
            public string guid { get; set; }
            public string autor { get; set; }
            public string reason { get; set; }
        }

        public class DeletedItems : IDisposable
        {
            public string Version { get; set; }
            public string NickShop { get; set; }
            public string CodeShop { get; set; }
            //public string Guid { get; set; }
            public List<DeletedItem> ListDeletedItem { get; set; }

            void IDisposable.Dispose()
            {

            }
        }

        /// <summary>
        /// Запрашивает на промежуточном сервере 
        /// необходимость проверки в CDN 
        /// для данного магазина и управляет этим состоянием на кассе
        /// </summary>
        private void get_web_tovar_check_cdn()
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

            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
            string encrypt_string = CryptorEngine.Encrypt(code_shop, true, key);

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 200000;

            try
            {

                string answer_crypt = ds.GetTovarCheckCDN(nick_shop, encrypt_string, MainStaticClass.GetWorkSchema.ToString());
                string decrypt_data = CryptorEngine.Decrypt(answer_crypt, true, key);
                if (decrypt_data != "-1")
                {
                    if (decrypt_data != MainStaticClass.EnableCdnMarkers.ToString())
                    {
                        string updateSql = "UPDATE constants SET enable_cdn_markers =" + (decrypt_data == "1" ? true : false);
                        UpdateDatabaseAndVariable(updateSql, decrypt_data);
                    }
                }
            }
            catch
            {

            }
        }


        public void UpdateDatabaseAndVariable(string updateSql, string decrypt_data)
        {
            using (TransactionScope scope = new TransactionScope())
            {

                using (NpgsqlConnection conn = MainStaticClass.NpgsqlConn())
                {
                    conn.Open();
                    NpgsqlCommand command = new NpgsqlCommand(updateSql, conn);

                    // Изменение в базе данных
                    int rowwaffected = command.ExecuteNonQuery();

                    // Изменение переменной программы
                    MainStaticClass.EnableCdnMarkers = -1;

                    // Проверка условий и завершение транзакции
                    if (rowwaffected > 0)
                    {
                        scope.Complete();
                    }
                }
            }
        }



        private void UploadDeletedItems()
        {
            DeletedItems deletedItems = new DeletedItems();
            deletedItems.CodeShop = MainStaticClass.Code_Shop;
            deletedItems.NickShop = MainStaticClass.Nick_Shop;
            deletedItems.ListDeletedItem = new List<DeletedItem>();
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "SELECT num_doc, num_cash, date_time_start, date_time_action, tovar, quantity, type_of_operation,guid,reason FROM deleted_items;";
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
                    deletedItem.guid = reader["guid"].ToString();
                    deletedItem.autor = MainStaticClass.CashOperatorInn;
                    deletedItem.reason = reader["reason"].ToString();
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
                    //MessageBox.Show("Веб сервис недоступен");
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
                string answer = ds.UploadDeletedItems(nick_shop, encrypt_string, MainStaticClass.GetWorkSchema.ToString());
                if (answer == "1")
                {
                    query = "DELETE FROM deleted_items";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                else
                {
                    //MessageBox.Show("Произошли ошибки при передаче удаленных строк");
                    MainStaticClass.WriteRecordErrorLog("Произошли ошибки при передаче удаленных строк", "UploadDeletedItems", 0, MainStaticClass.CashDeskNumber, "не удалось передать информацию об удаленных строках");
                }
                command.Dispose();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошли ошибки при передаче удаленных строк " + ex.Message);
                MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "Не удалось передать информацию об удаленных строках");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private void check_files_and_folders()
        {
            try
            {
                string folderPathPictures = Application.StartupPath + "\\Pictures2";
                if (!Directory.Exists(folderPathPictures))
                {
                    Directory.CreateDirectory(folderPathPictures);
                }
                string fileExistUpdateProgrammPictures = Application.StartupPath + "\\Pictures2\\ExistUpdateProgramm.jpg";
                if (!File.Exists(fileExistUpdateProgrammPictures))
                {
                    get_file_for_web_service("Pictures2\\ExistUpdateProgramm.jpg");
                }
            }
            catch (Exception ex)
            {
                MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "Проверка/создание файлов и папок");
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
                //string answer = ds.UploadPhoneClients(nick_shop, encrypt_string,MainStaticClass.GetWorkSchema.ToString());
                string answer = ds.UploadPhoneClients(nick_shop, encrypt_string, "4");
                if (answer == "1")
                {
                    query = "DELETE FROM temp_phone_clients";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
                else
                {
                    //MessageBox.Show("Произошли ошибки на сервере при передаче телефонов клиентов");
                    MainStaticClass.WriteRecordErrorLog("Произошли ошибки на сервере при передаче телефонов клиентов", "UploadPhoneClients", 0, MainStaticClass.CashDeskNumber, "не удалось передать информацию о телефонах клиентов");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Произошли ошибки при передаче телефонов клиентов " + ex.Message);
                MainStaticClass.WriteRecordErrorLog(ex,0, MainStaticClass.CashDeskNumber, "не удалось передать информацию о телефонах клиентов");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void timer_send_data_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            MainStaticClass.SendOnlineStatus();
            if (MainStaticClass.Last_Write_Check > MainStaticClass.Last_Send_Last_Successful_Sending)
            {
                SendDataOnSalesPortions sdsp = new SendDataOnSalesPortions();
                sdsp.send_sales_data_Click(null, null);
                sdsp.Dispose();
                UploadDeletedItems();
                get_web_tovar_check_cdn();
                send_cdn_logs();
                //UploadErrorsLogAsync();
                UploadErrorsLog();
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
            //MessageBox.Show(Math.Round(Convert.ToDouble(0.479) * Convert.ToDouble(897.17), 2, MidpointRounding.ToEven).ToString());
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

        public class CdnLogs
        {
            public List<CdnLog> ListCdnLog { get; set; }
        }

        public class CdnLog
        {
            //public string Shop { get; set; }
            public string NumCash { get; set; }
            public string CdnAnswer { get; set; }
            public string DateShop { get; set; }
            public string NumDoc { get; set; }
            public string Mark { get; set; }
            public string Status { get; set; }
        }

        private void send_cdn_logs()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                string query = "SELECT num_cash, date, cdn_answer, numdoc, is_sent, mark,status FROM cdn_log WHERE is_sent=0;";
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                CdnLogs logs = new CdnLogs();
                logs.ListCdnLog = new List<CdnLog>();
                while (reader.Read())
                {
                    CdnLog log = new CdnLog();
                    log.CdnAnswer = reader["cdn_answer"].ToString();
                    log.Mark = reader["mark"].ToString();
                    log.NumCash = MainStaticClass.CashDeskNumber.ToString();
                    log.NumDoc = reader["numdoc"].ToString();
                    log.DateShop = Convert.ToDateTime(reader["date"]).ToString("dd-MM-yyyy HH:mm:ss");
                    log.Status = reader["status"].ToString();
                    logs.ListCdnLog.Add(log);
                }
                if (logs.ListCdnLog.Count > 0)
                {
                    Cash8.DS.DS ds = MainStaticClass.get_ds();
                    ds.Timeout = 180000;

                    //Получить параметра для запроса на сервер 
                    string nick_shop = MainStaticClass.Nick_Shop.Trim();
                    if (nick_shop.Trim().Length == 0)
                    {
                        return;
                    }
                    string code_shop = MainStaticClass.Code_Shop.Trim();
                    if (code_shop.Trim().Length == 0)
                    {
                        return;
                    }
                    string count_day = CryptorEngine.get_count_day();
                    string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
                    bool result_web_quey = false;
                    string data = JsonConvert.SerializeObject(logs, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    string data_crypt = CryptorEngine.Encrypt(data, true, key);

                    result_web_quey = ds.UploadCDNLogsPortionJason(nick_shop, data_crypt, MainStaticClass.GetWorkSchema.ToString());

                    if (result_web_quey)
                    {
                        foreach (CdnLog log in logs.ListCdnLog)
                        {
                            query = "UPDATE cdn_log SET is_sent = 1 WHERE date='" + log.DateShop + "';";
                            command = new NpgsqlCommand(query, conn);
                            command.ExecuteNonQuery();
                        }
                    }
                }
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
        }

        private void guid_to_lover()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {                
                string query = "SELECT code_shop	FROM public.constants;";
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                string code_shop = "";
                while (reader.Read())
                {
                    code_shop = reader["code_shop"].ToString();
                }
                if (code_shop != "")
                {
                    code_shop = code_shop.ToLower();
                    query = "UPDATE constants SET code_shop='" + code_shop + "'";
                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при изменение реквизита " + ex.Message);
                MainStaticClass.WriteRecordErrorLog(ex,0, MainStaticClass.CashDeskNumber, "Изменение гуин в НРЕГ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при изменение реквизита " + ex.Message);
                MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "Изменение гуин в НРЕГ");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }



        private async void Main_Load(object sender, System.EventArgs e)
        {
            

            //if (File.Exists(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll"))
            //{
            //    if (File.ReadAllBytes(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll").Length > 0)
            //    {
            //        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(Application.StartupPath + "/Npgsql.dll");
            //        int cash_version = int.Parse(myFileVersionInfo.FileVersion.Replace(".", ""));
            //        if (cash_version == 20100)//Старая версия Npgsql 
            //        {
            //            File.Copy(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll", Application.StartupPath + "/Npgsql.dll", true);
            //        }
            //    }
            //}



            MainStaticClass.Main = this;
            this.IsMdiContainer = true;
            MainStaticClass.Last_Send_Last_Successful_Sending = DateTime.Now;
            MainStaticClass.Last_Write_Check = DateTime.Now.AddSeconds(1);

            //string verssssion = new Version(System.Windows.Forms.Application.ProductVersion).ToString();

            if (File.Exists(Application.StartupPath + "/Setting.gaa") == false)
            {
                MessageBox.Show("Не обнаружен файл " + " Setting.gaa " + " с параметрами для программы " + Application.StartupPath + " дальнейшая работа невозможна ");
                return;
            }
            else
            {
                Cash8.MainStaticClass.loadConfig(Application.StartupPath + "/Setting.gaa");
            }

            if (DateTime.Now > new DateTime(2024, 12, 16, 0, 0, 0))
            {
                guid_to_lover();
            }

            MainStaticClass.write_event_in_log("Перед проверкой обновления в интернет", " Старт программы ", "0");
            LoadProgramFromInternet lpfi = new LoadProgramFromInternet();
            lpfi.show_phone = true;
            lpfi.check_new_version_programm();
            //bool new_version_of_the_program_exist = lpfi.new_version_of_the_program;
            if (lpfi.new_version_of_the_program)
            {
                обновлениеПрограммыToolStripMenuItem_Click(null, null);
            }
            lpfi.Dispose();
            MainStaticClass.write_event_in_log("Проверка обновления в интернет завершена", " Старт программы ", "0");

            if (MainStaticClass.exist_table_name("constants"))
            {
                //MessageBox.Show("1");
                Task.Run(() => get_pdb());
                check_add_field();
                InventoryManager.FillDictionaryProductDataAsync();
                Task.Run(() => InventoryManager.DictionaryPriceGiftAction); ;
                //MessageBox.Show("2");

                MainStaticClass.write_event_in_log(" Старт программы ", "проверка таблицы констант", "0");
                Text += "Касса   " + Cash8.MainStaticClass.CashDeskNumber;
                Text += " | " + Cash8.MainStaticClass.Nick_Shop;
                Text += " | " + Cash8.MainStaticClass.version();
                Text += " | " + Cash8.LoadDataWebService.last_date_download_tovars().ToString("yyyy-MM-dd hh:mm:ss");


                update_unloading_period();
                int result = MainStaticClass.get_unloading_interval();
                if (result != 0)
                {
                    timer_send_data.Interval = result * 60000;
                    timer_send_data.Start();
                    timer_send_data.Elapsed += new System.Timers.ElapsedEventHandler(timer_send_data_Elapsed);
                }
                //MessageBox.Show("3");
                UploadPhoneClients();
                check_failed_input_phone();

                //MainStaticClass.write_event_in_log("Перед получением данных по пользователям", " Старт программы ", "0");
                get_users();
                //MainStaticClass.write_event_in_log("После получения данных по пользователям", " Старт программы ", "0");

                //MainStaticClass.write_event_in_log("Перед проверкой системы налогообложения", " Старт программы ", "0");
                MainStaticClass.SystemTaxation = check_system_taxation();
                //MainStaticClass.write_event_in_log("После проверки системы налогообложения", " Старт программы ", "0");

                MainStaticClass.delete_old_checks(MainStaticClass.GetMinDateWork);
                MainStaticClass.delete_all_events_in_log(MainStaticClass.GetMinDateWorkLogs);
                //MessageBox.Show("4");
                if (MainStaticClass.Use_Fiscall_Print)
                {
                    getShiftStatus();
                }
                //MessageBox.Show("5");

                //if (MainStaticClass.GetDoNotPromptMarkingCode == 0)
                //{
                if (MainStaticClass.CashDeskNumber != 9)
                {
                    MainStaticClass.validate_date_time_with_fn(10);
                    if (MainStaticClass.SystemTaxation == 0)
                    {
                        MessageBox.Show("У вас не заполнена система налогообложения!\r\nСоздание и печать чеков невозможна!\r\nОБРАЩАЙТЕСЬ В БУХГАЛТЕРИЮ!");
                    }
                    bool restart = false; bool error = false;

                    MainStaticClass.check_version_fn(ref restart, ref error);
                    if (!error)
                    {
                        if (restart)
                        {
                            MessageBox.Show("У вас неверно была установлена версия ФН, необходим перезапуск программы");
                            this.Close();
                        }
                    }
                }
                //}
            }
            else
            {
                MessageBox.Show("В этой бд нет таблицы constatnts,необходимо создать таблицы бд");
            }
            this.menuStrip.Items.Clear();
            MainStaticClass.Main.start_interface_switching();

            if (MainStaticClass.GetVersionFn == 2)
            {
                MainStaticClass.Version2Marking = 1;
            }
            else
            {
                MainStaticClass.Version2Marking = 0;
            }
            if (MainStaticClass.CashDeskNumber != 9)//&& MainStaticClass.EnableCdnMarkers == 1
            {
                Task.Run(() => load_bonus_clients());
                if (MainStaticClass.CDN_Token == "")
                {
                    MessageBox.Show("В этой кассе не заполнен CDN токен, \r\n ПРОДАЖА МАРКИРОВАННОГО ТОВАРА ОГРАНИЧЕНА/НЕВОЗМОЖНА!", "Проверка CDN");
                }
                else
                {
                    LoadCdnWithStartAsync();                    
                }
            }
            if (MainStaticClass.PrintingUsingLibraries == 1)
            {
                PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                printingUsingLibraries.CheckTaxationTypes();
            }

            check_files_and_folders();
            //MessageBox.Show("10");

        }

       



        //private void load_cdn_with_start()
        //{
        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    CancellationToken token = cts.Token;

        //    // Запуск функции с параметром в новом потоке            
        //    Task task = Task.Factory.StartNew(() => get_cdn_with_start());
        //    try
        //    {
        //        // Ожидание результата функции в течение 5 секунд
        //        bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(60));

        //        if (!isCompletedSuccessfully)
        //        {
        //            cts.Cancel();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void get_cdn_with_start()
        {
            CDN.CDN_List list = MainStaticClass.CDN_List;
        }

        private async Task LoadCdnWithStartAsync()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            try
            {
                // Запуск функции с параметром в новом потоке
                Task task = Task.Run(() => get_cdn_with_start(), token);

                // Ожидание результата функции в течение 60 секунд
                bool isCompletedSuccessfully = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(60), token)) == task;

                if (!isCompletedSuccessfully)
                {
                    cts.Cancel();
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
                MessageBox.Show($"При загрузке CDN произошла ошибка: {ex.Message}");
            }
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
                //throw new Exception("Тевтоый атас проверка");
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
                 MainStaticClass.WriteRecordErrorLog(ex,0,Convert.ToInt16(MainStaticClass.CashDeskNumber),@"MainForms Проверка таблицы failed_input_phone
        при старте программы, если это сегодня первый старт
        и документов продажи еще нет, тогда очищаем таблицу");
             }
             catch (Exception ex)
             {
                 MessageBox.Show("Ошибка при очистке счетчика ошибочно введенных номеров телефонов" + ex.Message);                
                MainStaticClass.WriteRecordErrorLog(ex, 0, Convert.ToInt16(MainStaticClass.CashDeskNumber), @"MainForms Проверка таблицы failed_input_phone
        при старте программы, если это сегодня первый старт
        и документов продажи еще нет, тогда очищаем таблицу");
            }
             finally
             {
                 if (conn.State == ConnectionState.Open)
                 {
                     conn.Close();
                 }
             }
        }
        

        public class RecordsErrorLog
        {
            public string Shop { get; set; }
            public short CashDeskNumber { get; set; }
            public List<RecordErrorLog> ErrorLogs { get; set; } = new List<RecordErrorLog>();
        }

        public class RecordErrorLog
        {            
            public string ErrorMessage { get; set; }
            public string MethodName { get; set; }
            public long NumDoc { get; set; }            
            public string Description { get; set; }
            public DateTime DateTimeRecord { get; set; }
        }

        //private void UploadErrorsLog()
        //{
        //    RecordsErrorLog recordsErrorLog = new RecordsErrorLog();
        //    using (var connection = MainStaticClass.NpgsqlConn())
        //    {
        //        connection.Open();

        //        // SQL-запрос для выборки данных из таблицы log_errors
        //        string query = "SELECT error_message, date_time_record, num_doc, cash_desk_number, method_name, description FROM public.log_errors";

        //        using (var command = new NpgsqlCommand(query, connection))
        //        using (var reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                var logError = new RecordErrorLog
        //                {
        //                    ErrorMessage = reader["error_message"].ToString().Trim(),
        //                    DateTimeRecord = reader.GetDateTime(reader.GetOrdinal("date_time_record")),
        //                    NumDoc = reader.GetInt64(reader.GetOrdinal("num_doc")),
        //                    CashDeskNumber = reader.GetInt16(reader.GetOrdinal("cash_desk_number")),
        //                    MethodName = reader["method_name"].ToString().Trim(),
        //                    Description = reader["description"].ToString().Trim()
        //                };

        //                recordsErrorLog.ErrorLogs.Add(logError);
        //            }
        //        }
        //    }

        //    if (recordsErrorLog.ErrorLogs.Count > 0)
        //    {                
        //        Cash8.DS.DS ds = MainStaticClass.get_ds();
        //        ds.Timeout = 180000;

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
        //        bool result_web_quey = false;
        //        string data = JsonConvert.SerializeObject(recordsErrorLog, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //        string data_crypt = CryptorEngine.Encrypt(data, true, key);
        //        try
        //        {
        //            result_web_quey = ds.UploadErrorLogPortionJason(nick_shop, data_crypt, MainStaticClass.GetWorkSchema.ToString());
        //            if (result_web_quey)
        //            {
        //                using (var connection = MainStaticClass.NpgsqlConn())
        //                {
        //                    connection.Open();
        //                    foreach (RecordErrorLog recordErrorLog in recordsErrorLog.ErrorLogs)
        //                    {
        //                        string query = "DELETE FROM public.log_errors WHERE date_time_record='" + recordErrorLog.DateTimeRecord.ToString() + "'";
        //                        NpgsqlCommand command  = new NpgsqlCommand(query, connection);
        //                        command.ExecuteNonQuery();
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            //write_error(ex.Message);
        //        }
        //    }
        //}

        //private async Task UploadErrorsLogAsync()
        //{
        //    var recordsErrorLog = await ReadErrorLogsFromDatabaseAsync();
        //    if (recordsErrorLog.ErrorLogs.Count > 0)
        //    {
        //        bool uploadResult = await UploadErrorLogsToServerAsync(recordsErrorLog);
        //        if (uploadResult)
        //        {
        //            await DeleteErrorLogsFromDatabaseAsync(recordsErrorLog);
        //        }
        //    }
        //}

        //private async Task<RecordsErrorLog> ReadErrorLogsFromDatabaseAsync()
        //{
        //    RecordsErrorLog recordsErrorLog = new RecordsErrorLog();
        //    recordsErrorLog.Shop = MainStaticClass.Nick_Shop;
        //    recordsErrorLog.CashDeskNumber = Convert.ToInt16(MainStaticClass.CashDeskNumber);

        //    using (var connection = MainStaticClass.NpgsqlConn())
        //    {
        //        await connection.OpenAsync();
        //        string query = "SELECT error_message, date_time_record, num_doc, method_name, description FROM public.log_errors";
        //        using (var command = new NpgsqlCommand(query, connection))
        //        using (var reader = await command.ExecuteReaderAsync())
        //        {
        //            while (await reader.ReadAsync())
        //            {
        //                var logError = new RecordErrorLog
        //                {
        //                    ErrorMessage = reader["error_message"].ToString().Trim(),
        //                    DateTimeRecord = reader.GetDateTime(reader.GetOrdinal("date_time_record")),
        //                    NumDoc = reader.GetInt64(reader.GetOrdinal("num_doc")),                            
        //                    MethodName = reader["method_name"].ToString().Trim(),
        //                    Description = reader["description"].ToString().Trim()
        //                };                        
        //                recordsErrorLog.ErrorLogs.Add(logError);
        //            }
        //        }
        //    }
        //    return recordsErrorLog;
        //}

        //private async Task<bool> UploadErrorLogsToServerAsync(RecordsErrorLog recordsErrorLog)
        //{
        //    string nick_shop = MainStaticClass.Nick_Shop.Trim();
        //    string code_shop = MainStaticClass.Code_Shop.Trim();
        //    if (string.IsNullOrEmpty(nick_shop) || string.IsNullOrEmpty(code_shop))
        //    {
        //        return false;
        //    }

        //    string count_day = CryptorEngine.get_count_day();
        //    string key = nick_shop + count_day + code_shop;
        //    string data = JsonConvert.SerializeObject(recordsErrorLog, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //    string data_crypt = CryptorEngine.Encrypt(data, true, key);

        //    Cash8.DS.DS ds = MainStaticClass.get_ds();
        //    ds.Timeout = 18000;
        //    try
        //    {
        //        return await Task.Run(() =>
        //            ds.UploadErrorLogPortionJson(nick_shop, data_crypt, MainStaticClass.GetWorkSchema.ToString())
        //        );
        //    }
        //    catch (Exception)
        //    {                
        //        return false;
        //    }
        //}

        //private async Task DeleteErrorLogsFromDatabaseAsync(RecordsErrorLog recordsErrorLog)
        //{
        //    using (var connection = MainStaticClass.NpgsqlConn())
        //    {
        //        await connection.OpenAsync();
        //        foreach (var recordErrorLog in recordsErrorLog.ErrorLogs)
        //        {
        //            string query = "DELETE FROM public.log_errors WHERE date_time_record = @DateTimeRecord";
        //            using (var command = new NpgsqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@DateTimeRecord", recordErrorLog.DateTimeRecord);
        //                await command.ExecuteNonQueryAsync();
        //            }
        //        }
        //    }
        //}


        private void get_pdb()
        {
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (string.IsNullOrEmpty(nick_shop) || string.IsNullOrEmpty(code_shop))
            {
                MainStaticClass.WriteRecordErrorLog("Не удалось получить ник или код магазина", "get_pdb()",0, MainStaticClass.CashDeskNumber, "Обновление pdb файла");
                return ;
            }

            string filePath = Application.StartupPath+ "\\Cash8.pdb";
            DateTime lastWriteTime = new DateTime(2000, 1, 1);
            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                lastWriteTime = fileInfo.LastWriteTime;
            }
            

            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop + count_day + code_shop;

            string data = JsonConvert.SerializeObject(lastWriteTime, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string data_crypt = CryptorEngine.Encrypt(data, true, key);

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 18000;
            try
            {
                byte[] pdb=ds.GetPDP(nick_shop, data_crypt, MainStaticClass.GetWorkSchema.ToString());
                if (pdb.Length != 0)
                {
                    File.WriteAllBytes(Application.StartupPath + "\\Cash8.pdb", pdb);
                }
            }
            catch (Exception ex)
            {
                MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "не удалось загрузить pdb файл");
                //return false;
            }
        }

        private void get_file_for_web_service(string filename)
        {
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (string.IsNullOrEmpty(nick_shop) || string.IsNullOrEmpty(code_shop))
            {
                MainStaticClass.WriteRecordErrorLog("Не удалось получить ник или код магазина", "get_pdb()", 0, MainStaticClass.CashDeskNumber, "Обновление pdb файла");
                return;
            }

            //string filePath = Application.StartupPath + "\\Cash8.pdb";
            //DateTime lastWriteTime = new DateTime(2000, 1, 1);
            //if (File.Exists(filePath))
            //{
            //    FileInfo fileInfo = new FileInfo(filePath);
            //    lastWriteTime = fileInfo.LastWriteTime;
            //}


            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop + count_day + code_shop;

            string data = JsonConvert.SerializeObject(filename, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string data_crypt = CryptorEngine.Encrypt(data, true, key);

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 18000;
            try
            {
                byte[] file = ds.GetPDP(nick_shop, data_crypt, MainStaticClass.GetWorkSchema.ToString());
                if (file.Length != 0)
                {
                    File.WriteAllBytes(Application.StartupPath + "\\" + filename, file);
                }
            }
            catch (Exception ex)
            {
                MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "не удалось загрузить pdb файл");
                //return false;
            }
        }



        private void UploadErrorsLog()
        {
            try
            {
                var recordsErrorLog = ReadErrorLogsFromDatabase();
                if (recordsErrorLog.ErrorLogs.Count > 0)
                {
                    bool uploadResult = UploadErrorLogsToServer(recordsErrorLog);
                    if (uploadResult)
                    {
                        DeleteErrorLogsFromDatabase(recordsErrorLog);
                    }
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку или предпринимаем другие действия по обработке исключения
                MainStaticClass.WriteRecordErrorLog(ex,0, MainStaticClass.CashDeskNumber, "Произошла ошибка при загрузке логов ошибок");
            }
        }

        private RecordsErrorLog ReadErrorLogsFromDatabase()
        {
            RecordsErrorLog recordsErrorLog = new RecordsErrorLog();
            recordsErrorLog.Shop = MainStaticClass.Nick_Shop;
            recordsErrorLog.CashDeskNumber = Convert.ToInt16(MainStaticClass.CashDeskNumber);

            using (var connection = MainStaticClass.NpgsqlConn())
            {
                connection.Open();
                string query = "SELECT error_message, date_time_record, num_doc, method_name, description FROM public.errors_log";
                using (var command = new NpgsqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var logError = new RecordErrorLog
                        {
                            ErrorMessage = reader["error_message"].ToString().Trim(),
                            DateTimeRecord = reader.GetDateTime(reader.GetOrdinal("date_time_record")),
                            NumDoc = reader.GetInt64(reader.GetOrdinal("num_doc")),
                            MethodName = reader["method_name"].ToString().Trim(),
                            Description = reader["description"].ToString().Trim()
                        };
                        recordsErrorLog.ErrorLogs.Add(logError);
                    }
                }
            }
            return recordsErrorLog;
        }

        private bool UploadErrorLogsToServer(RecordsErrorLog recordsErrorLog)
        {
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (string.IsNullOrEmpty(nick_shop) || string.IsNullOrEmpty(code_shop))
            {
                return false;
            }

            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop + count_day + code_shop;
            string data = JsonConvert.SerializeObject(recordsErrorLog, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            string data_crypt = CryptorEngine.Encrypt(data, true, key);

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 18000;
            try
            {
                return ds.UploadErrorLogPortionJson(nick_shop, data_crypt, MainStaticClass.GetWorkSchema.ToString());
            }
            catch (Exception ex)
            {
                MainStaticClass.WriteRecordErrorLog(ex, 0, MainStaticClass.CashDeskNumber, "не удалось передать информацию об ошибках в программе");
                return false;
            }
        }

        private void DeleteErrorLogsFromDatabase(RecordsErrorLog recordsErrorLog)
        {
            using (var connection = MainStaticClass.NpgsqlConn())
            {
                connection.Open();
                foreach (var recordErrorLog in recordsErrorLog.ErrorLogs)
                {
                    string query = "DELETE FROM public.errors_log WHERE date_time_record = @DateTimeRecord";
                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DateTimeRecord", recordErrorLog.DateTimeRecord);
                        command.ExecuteNonQuery();
                    }
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

        /// <summary>
        /// Исправление старого типа автор
        /// в колонке
        /// </summary>
        private void check_correct_type_column()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlTransaction tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                string query = "SELECT data_type FROM information_schema.columns where table_name = 'errors_log' AND column_name = 'error_message'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (command.ExecuteScalar().ToString().Trim() != "text")//старый тип колонки в бд, меняем на новый
                {
                    SettingConnect sc = new SettingConnect();
                    sc.add_field_Click(null, null);
                    sc.Dispose();
                    this.Close();
                }
                tran.Commit();
                conn.Close();
                command.Dispose();
            }
            catch
            {
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

        private void check_exists_column()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT EXISTS(SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND table_name = 'tovar' AND column_name = 'refusal_of_marking'); ";                
                
                 NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (!Convert.ToBoolean(command.ExecuteScalar())) //не нашли такой колонки   
                {
                    //check_add_field();
                    SettingConnect sc = new SettingConnect();
                    sc.add_field_Click(null, null);
                    sc.Dispose();
                    this.Close();
                }
                conn.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "check_add_field");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        private void check_exists_table()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {                
                conn.Open();                
                string query = "SELECT EXISTS(SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND table_name = 'errors_log'); ";

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (!Convert.ToBoolean(command.ExecuteScalar())) //не нашли такой таблицы   
                {
                    //check_add_field();
                    SettingConnect sc = new SettingConnect();
                    sc.add_field_Click(null, null);
                    sc.Dispose();
                    this.Close();
                }
                conn.Close();
                command.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "check_add_field");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }


        ///// <summary>
        ///// Исправление старого типа автор
        ///// в колонке
        ///// </summary>
        private void check_add_field()
        {
            check_correct_type_column();
            //check_exists_table();
            check_exists_column();            
        }




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



        //private void check_add_field()
        //{            
        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
        //    try
        //    {
        //        conn.Open();
        //        string query = " SELECT execute_addcolumn  FROM constants;";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        object result_query = command.ExecuteScalar();
        //        if (result_query.ToString() == "")
        //        {
        //            SettingConnect sc = new SettingConnect();
        //            sc.add_field_Click(null, null);
        //            sc.Dispose();

        //            query = " UPDATE constants SET execute_addcolumn = 1 ";
        //            command = new NpgsqlCommand(query, conn);
        //            command.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            if (Convert.ToInt16(result_query) == 2)
        //            {
        //                SettingConnect sc = new SettingConnect();
        //                sc.add_field_Click(null, null);
        //                sc.Dispose();

        //                query = " UPDATE constants SET execute_addcolumn = 1 ";
        //                command = new NpgsqlCommand(query, conn);
        //                command.ExecuteNonQuery();
        //            } 
        //        }
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(" Ошибка при добавлении полей в БД "+ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(" Ошибка при добавлении полей в БД " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

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
            //UploadDeletedItems();
            if (MainStaticClass.PrintingUsingLibraries == 1)
            {
                Atol.Drivers10.Fptr.IFptr fptr = MainStaticClass.FPTR;
                if (fptr.isOpened())
                {
                    fptr.close();
                }
                fptr.destroy();
            }
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
            this.корректировочныеЧекиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.проверкаАкцийToolStripMenuItem        = new System.Windows.Forms.ToolStripMenuItem();
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
            //this.выгрузкаДанныхПоБонусамToolStripMenuItem,
            this.проверкаАкцийToolStripMenuItem,
            this.корректировочныеЧекиToolStripMenuItem,
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
            // проверкаАкцийToolStripMenuItem
            // 
            this.проверкаАкцийToolStripMenuItem.Name = "проверкаАкцийToolStripMenuItem";
            //this.проверкаАкцийToolStripMenuItem.Text = "Проверка Акций";
            resources.ApplyResources(this.проверкаАкцийToolStripMenuItem, "проверкаАкцийToolStripMenuItem");
            this.проверкаАкцийToolStripMenuItem.Click += new System.EventHandler(this.проверкаАкцийToolStripMenuItem_Click);
            // 
            // выгрузкаДанныхПоБонусамToolStripMenuItem
            // 
            //this.выгрузкаДанныхПоБонусамToolStripMenuItem.Name = "выгрузкаДанныхПоБонусамToolStripMenuItem";
            //resources.ApplyResources(this.выгрузкаДанныхПоБонусамToolStripMenuItem, "выгрузкаДанныхПоБонусамToolStripMenuItem");
            //this.выгрузкаДанныхПоБонусамToolStripMenuItem.Click += new System.EventHandler(this.выгрузкаДанныхПоБонусамToolStripMenuItem_Click);
            // 
            // выгрузкаПродажToolStripMenuItem
            // 
            this.корректировочныеЧекиToolStripMenuItem.Name = "корректировочныеЧекиToolStripMenuItem";
            resources.ApplyResources(this.корректировочныеЧекиToolStripMenuItem, "корректировочныеЧекиToolStripMenuItem");
            this.корректировочныеЧекиToolStripMenuItem.Click += КорректировочныеЧекиToolStripMenuItem_Click;// new System.EventHandler(this.выгрузкаПродажToolStripMenuItem_Click);
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

        private void КорректировочныеЧекиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CorrectionChecks checks = new CorrectionChecks();            
            //checks.FormBorderStyle = FormBorderStyle.None;
            checks.WindowState = FormWindowState.Maximized;
            checks.MdiParent = MainStaticClass.Main;
            checks.Show();
        }

        private void проверкаАкцийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckActions checkActions = new CheckActions();
            checkActions.ShowDialog();
            //CorrectionChecks checks = new CorrectionChecks();
            ////checks.FormBorderStyle = FormBorderStyle.None;
            //checks.WindowState = FormWindowState.Maximized;
            //checks.MdiParent = MainStaticClass.Main;
            //checks.Show();
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

        //private void выгрузкаДанныхПоБонусамToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    if (MainStaticClass.GetWorkSchema == 1)
        //    {
        //        SentDataOnBonus sdb = new SentDataOnBonus();
        //        sdb.ShowDialog();
        //    }
        //    else if (MainStaticClass.GetWorkSchema == 2)
        //    {
        //        SentDataOnBonusEva sentDataOnBonusEva = new SentDataOnBonusEva();
        //        sentDataOnBonusEva.ShowDialog();
        //    }
            
        //    //MessageBox.Show("В этой версии не работает");
        //} 
        


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
