using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Npgsql;
using System.Diagnostics;

namespace Cash8
{
    public partial class LoadProgramFromInternet : Form
    {
        private string version = "";
        public bool new_version_of_the_program = false;
        public bool show_phone = false;

        public LoadProgramFromInternet()
        {
            InitializeComponent();
            this.Shown += new EventHandler(LoadProgramFromInternet_Shown);
        }
        
        public void check_new_version_programm()
        {
            if (!MainStaticClass.service_is_worker())
            {
                return;
            }

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 1000;

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
            string data = code_shop.Trim() + "|" + MainStaticClass.version() + "|" + code_shop.Trim();
            string result_web_query = "";                        

            try
            {
                result_web_query = ds.ExistsUpdateProrgam(nick_shop, CryptorEngine.Encrypt(data, true, key),MainStaticClass.GetWorkSchema.ToString());
            }
            catch (Exception ex)
            {                
                MessageBox.Show("Ошибка при получении версии программы на сервере " + ex.Message);
                return;
            }

            if (result_web_query == "")
            {
                label_update.Text = "Не удалось проверить версию программы на сервере";
            }
            else// (result_web_query != "")
            {
                result_web_query = CryptorEngine.Decrypt(result_web_query, true, key);

                if (MainStaticClass.version() == result_web_query)
                {
                    label_update.Text = " У вас установлена самая последняя версия программы ";
                }
                else
                {
                    //это старое решение по контролю версий
                    version = result_web_query;
                    //result_web_query = result_web_query.Replace(".", "");
                    //string my_version = result_web_query.Substring(0, 2) + "-" + result_web_query.Substring(2, 2) + "-" + result_web_query.Substring(4, 4);
                    //label_update.Text = "Есть обновление программы от " + my_version;
                    //btn_download.Enabled = true;
                    //new_version_of_the_program = true;

                    //это новое решение по контролю версий
                    //здесь наверное надо установить проверку на больше меньше по версиям 
                    Int64 local_version = Convert.ToInt64(MainStaticClass.version().Replace(".", ""));
                    Int64 remote_version = Convert.ToInt64(result_web_query.Replace(".", ""));
                    if(remote_version>local_version)
                    {
                        label_update.Text = "Есть обновление программы " + result_web_query;
                        btn_download.Enabled = true;
                        new_version_of_the_program = true;
                        //Принудительно вызываем обновление версии программы                        
                        if (!show_phone)
                        {
                            btn_download_Click(null, null);
                        }
                    }                    
                }
            } 
        }


        private void LoadProgramFromInternet_Shown(object sender, EventArgs e)
        {
            if (!show_phone)
            {
                check_new_version_programm();
            }
        }

        private void check_and_update_npgsql()
        {
            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(Application.StartupPath + "/Npgsql.dll");
            int cash_version = int.Parse(myFileVersionInfo.FileVersion.Replace(".", ""));
            if (cash_version == 20100)//Старая версия Npgsql 
            {
                if (!Directory.Exists(Application.StartupPath + "/PreviousNpgsql"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "/PreviousNpgsql");
                }

                if (!Directory.Exists(Application.StartupPath + "/UpdateNpgsql"))
                {
                    Directory.CreateDirectory(Application.StartupPath + "/UpdateNpgsql");
                }


                if (!MainStaticClass.service_is_worker())
                {
                    return;
                }

                Cash8.DS.DS ds = MainStaticClass.get_ds();
                ds.Timeout = 50000;

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
                //старая версия
                //string my_version = version.Substring(0, 2) + "-" + version.Substring(2, 2) + "-" + version.Substring(4, 4);                

                string data = code_shop.Trim() + "|" + code_shop.Trim();
                byte[] result_web_query = new byte[0];
                try
                {
                    result_web_query = ds.GetNpgsqlNew(nick_shop, CryptorEngine.Encrypt(data, true, key),MainStaticClass.GetWorkSchema.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }

                File.WriteAllBytes(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll", result_web_query);

                try
                {
                    File.Copy(Application.StartupPath + "/Npgsql.dll", Application.StartupPath + "/PreviousNpgsql/Npgsql.dll", true);
                    if (File.ReadAllBytes(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll").Length > 0)
                    {
                        File.Copy(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll", Application.StartupPath + "/Npgsql.dll", true);
                        MessageBox.Show(" Библиотека Npgsql.dll успешно обновлена ");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                //                File.Copy(Application.StartupPath + "/UpdateNpgsql/Npgsql.dll", Application.StartupPath + "/Npgsql.dll", true);

                
                //Application.Exit();
            }
            //MessageBox.Show("check_and_update_npgsql");

        }

        private void btn_download_Click(object sender, EventArgs e)
        {

            //check_and_update_npgsql();

            btn_close.Enabled = false;
            if (!MainStaticClass.service_is_worker())
            {
                return;
            }

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            //if (MainStaticClass.GetWorkSchema == 2)
            //{
            //    ds.Url = "http://10.21.200.21/DiscountSystem/Ds.asmx";
            //}
            ds.Timeout = 10000;

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
            //старая версия
            //string my_version = version.Substring(0, 2) + "-" + version.Substring(2, 2) + "-" + version.Substring(4, 4);
            string my_version = version;

            string data = code_shop.Trim() + "|" + version + "|" + code_shop.Trim();
            byte[] result_web_query = new byte[0];            
            try
            {
                result_web_query = ds.GetUpdateProgram(nick_shop, CryptorEngine.Encrypt(data, true, key),MainStaticClass.GetWorkSchema.ToString());
            }
            catch (Exception ex)
            {                
                return;
            }
            

            if (result_web_query.Length > 10)
            {
                try
                {
                    if (!Directory.Exists(Application.StartupPath + "/Update"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "/Update");
                    }

                    File.WriteAllBytes(Application.StartupPath + "/Update/Cash.exe", result_web_query);

                    if (!Directory.Exists(Application.StartupPath + "/Previous"))
                    {
                        Directory.CreateDirectory(Application.StartupPath + "/Previous");
                    }

                    File.Copy(Application.StartupPath + "/Cash.exe", Application.StartupPath + "/Previous/Cash.exe", true);
                    //File.Copy(Application.StartupPath + "/Update/Cash.exe", Application.StartupPath + "/Cash.exe", true); При переходе на wine это не будет работать.
                    //update_execute_addcolumn();
                    //*********************************************************************** update_execute_addcolumn()
                    //NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

                    //try
                    //{
                    //    conn.Open();
                    //    string query = " UPDATE constants SET execute_addcolumn = 2 ";
                    //    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    //    command.ExecuteNonQuery();
                    //    conn.Close();
                    //}
                    //catch (NpgsqlException ex)
                    //{
                    //    MessageBox.Show(" Ошибка при обновлении значения константы " + ex.Message);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(" Ошибка при обновлении значения константы " + ex.Message);
                    //}
                    //finally
                    //{
                    //    if (conn.State == ConnectionState.Open)
                    //    {
                    //        conn.Close();
                    //    }
                    //}
                    //*********************************************************************** КОНЕЦ update_execute_addcolumn()
                    MessageBox.Show(" Обновление успешно загружено, теперь необходимо перезапустить программу ");
                    this.DialogResult = DialogResult.Yes;
                    this.Close();
                    //this.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" При загрузке произошли ошибки " + ex.Message);
                }
            }

            btn_close.Enabled = true;
        }

        private void update_execute_addcolumn()
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = " UPDATE constants SET execute_addcolumn = 2 ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибка при обновлении значения константы "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" Ошибка при обновлении значения константы " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                } 
            }
        }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }        
    }
}
