using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using System.Threading;

namespace Cash8
{
    public partial class Interface_switching : Form
    {
        //Main parent_form = null;
        bool result_execute_enter = false;
        Thread workerThread = null;
        System.Windows.Forms.Timer timer = null;
        //System.Windows.Forms.Timer input_barcode_timer = null;
        private Read_Data_From_Com_Port rd = null;
        public int caller_type = 0;
        public Cash_check cc = null;
        public bool not_change_Cash_Operator = false;




        public Interface_switching()
        {
            this.TopMost = true;
            InitializeComponent();
            this.Load += new EventHandler(Interface_switching_Load);
            //input_barcode_timer = new System.Windows.Forms.Timer();
            //input_barcode_timer.Interval = 700;
            //this.input_barcode_timer.Tick += new EventHandler(input_barcode_timer_Tick);
            this.input_barcode.MouseUp += new MouseEventHandler(input_barcode_MouseUp);            
        }

        /* protected override void OnKeyUp(KeyEventArgs e)
         {
             if ((e.KeyCode==Keys.V) && (e.Modifiers == Keys.Control))
             {
                 this.input_barcode.Enabled = true;
                 this.input_barcode.Text="";
                  if (!MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                 {
                     this.input_barcode.Enabled = true;
                 }
             }
             {
                /* if (!MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                 {
                     this.input_barcode.Enabled = true;
                 }
             }            
         }*/

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
            {
                e.Handled = true;
            }
            if (e.Modifiers == Keys.Shift)
            {
                e.Handled = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                if (input_barcode.Text.Trim().Length == 0)
                {
                    //MainStaticClass.write_event_dssl_in_log("POSNG_CASHIER_LOGIN_BEGIN", MainStaticClass.Cash_Operator_Client_Code, DateTime.Now, 0);
                    this.Close();
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.input_barcode.Focus();
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
            }
            /* else if ((e.KeyCode==Keys.V) && (e.Modifiers == Keys.Control))
             {
                 this.input_barcode.Text="";
                 this.input_barcode.Enabled = false;
				
             }*/
        }

        void input_barcode_MouseUp(object sender, MouseEventArgs e)
        {
            input_barcode.Text = "";
        }

        //private void input_barcode_timer_Tick(object sender, EventArgs e)
        //{
        //    input_barcode_timer.Stop();
        //    execute_enter(input_barcode.Text);
        //}

        #region com_barcode_scaner


        protected override void OnClosing(CancelEventArgs e)
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {

                this.timer.Stop();
                this.timer = null;
                MainStaticClass.continue_to_read_the_data_from_a_port = false;
                if (rd.mySerial != null)
                    if (rd.mySerial.IsOpen)
                        rd.mySerial.Close();
                workerThread.Join();
                rd = null;
                GC.Collect();
            }
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            //Проверка работоспособности потока слушающего com port
            //if ((DateTime.Now - MainStaticClass.Last_Answer_Barcode_Scaner).Seconds > 2)
            //{
            //    restart_com_barcode_scaner();
            //}
            if (MainStaticClass.Barcode.Length > 0)
            {
                //if (MainStaticClass.Barcode.Length == 10)//10 символов считано из com порта
                //{
                stop_com_barcode_scaner();
                execute_enter(MainStaticClass.Barcode);
                if (!result_execute_enter)
                {
                    start_com_barcode_scaner();
                }
                MainStaticClass.Barcode = "";
            }
        }

        private void first_start_com_barcode_scaner()
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                MainStaticClass.continue_to_read_the_data_from_a_port = true;
                rd = new Read_Data_From_Com_Port();
                workerThread = new Thread(rd.to_read_the_data_from_a_port);
                workerThread.Start();
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 200;
                timer.Start();
                timer.Tick += new EventHandler(timer_Tick);
            }
        }

        private void start_com_barcode_scaner()
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                MainStaticClass.continue_to_read_the_data_from_a_port = true;
                rd = new Read_Data_From_Com_Port();
                workerThread = new Thread(rd.to_read_the_data_from_a_port);
                workerThread.Start();
                timer.Start();
            }
        }

        private void stop_com_barcode_scaner()
        {
            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                timer.Stop();
                MainStaticClass.continue_to_read_the_data_from_a_port = false;//изменяем условие выхода из цикла                 
                if (rd.mySerial != null)
                    if (rd.mySerial.IsOpen)
                        rd.mySerial.Close();//закрываем COM порт если он открыт                                        
                /*try
                {
                    workerThread.Abort();
                }
                catch
                {

                }*/
                workerThread.Join();//прекращаем действие потока            
            }
        }

        //private void restart_com_barcode_scaner()
        //{
        //    if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
        //    {
        //        MainStaticClass.continue_to_read_the_data_from_a_port = false;//изменяем условие выхода из цикла                 
        //        if (rd.mySerial != null)
        //            if (rd.mySerial.IsOpen)
        //                rd.mySerial.Close();//закрываем COM порт если он открыт                                        
        //        workerThread.Join();//прекращаем действие потока                
        //        timer.Stop();//Останавливаем таймер чтобы не мельтешил сейчас своими сработками
        //        Thread.Sleep(500);//останавливаем текущий поток пауза пусть все завершится
        //        //Теперь стартуем прослушку по новой
        //        MainStaticClass.continue_to_read_the_data_from_a_port = true;
        //        rd = new Read_Data_From_Com_Port();
        //        workerThread = new Thread(rd.to_read_the_data_from_a_port);
        //        workerThread.Start();
        //        timer.Start();
        //    }
        //}

        #endregion


        private void Interface_switching_Load(object sender, EventArgs e)
        {

            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                first_start_com_barcode_scaner();
                //this.input_barcode.Enabled = false;
            }

        }


        /// <summary>
        /// Возвращает ид Роли
        /// </summary>
        /// <param name="user_code"></param>
        /// <returns></returns>
        private int find_user_role(string user_code)
        {
            int rezult = 0;

            try
            {
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //string query = "SELECT users._right_,nick FROM users left join clients on users.client_code=clients.code where clients.barcode='" + user_code + "'";
                string query = "SELECT users._right_,nick,client_code FROM users left join clients on users.client_code=clients.code where clients.code='" + user_code + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rezult = Convert.ToInt32(reader.GetInt16(0));
                    if (!not_change_Cash_Operator)
                    {
                        MainStaticClass.Cash_Operator = reader.GetString(1);
                        MainStaticClass.Cash_Operator_Client_Code = reader.GetString(2);
                    }
                }
                //object result = command.ExecuteScalar();
                //if (result != null)
                //{
                //    rezult = Convert.ToInt32(result);
                //}
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return rezult;
        }


       

        /// <summary>
        /// Возвращает ид Роли
        /// </summary>
        /// <param name="user_code"></param>
        /// <returns></returns>
        private int find_user_role_new(string password)
        {
            int rezult = 0;
                        
            string password_Md5Hash = MainStaticClass.getMd5Hash(password).ToUpper();

            try
            {
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT rights,name,code,inn FROM users where password_m='" + password_Md5Hash.Trim() + "' or password_b='" + password_Md5Hash.Trim() + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rezult = Convert.ToInt32(reader.GetInt16(0));
                    if (!not_change_Cash_Operator)
                    {
                        MainStaticClass.Cash_Operator = reader["name"].ToString().Trim();
                        MainStaticClass.Cash_Operator_Client_Code = reader["code"].ToString();
                        MainStaticClass.cash_operator_inn = reader["inn"].ToString();
                    }
                }                
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (MainStaticClass.Cash_Operator.Trim().ToUpper() == "К9")
            {
                //MessageBox.Show("                                                                              "+MainStaticClass.CashDeskNumber.ToString());
                //MessageBox.Show("                                                                              "+MainStaticClass.get_unloading_interval().ToString());

                if ((MainStaticClass.CashDeskNumber != 9)||(MainStaticClass.get_unloading_interval()!=0))//Это пользователь для центрального компьютера 9 касса, пользователь не может зайти если стоит период синхронизации отличный от нуля или если номер кассы не 9
                {
                    rezult = 0;
                    MainStaticClass.Cash_Operator = "";
                    MainStaticClass.Cash_Operator_Client_Code = "";
                }
            }

            return rezult;
        }

        private int count_users()
        {

            int rezult = 0;

            try
            {
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT COUNT(*)FROM users ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    rezult = Convert.ToInt32(result);
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            return rezult;
        }


        private void execute_enter(string barcode)
        {         

            fail_autorize.Text = "";
            int result = -1;

            if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
            {
                this.input_barcode.Text = barcode;
            }
            //Проверка наличия таблицы если не найдена то это первый запуск

            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                command.CommandText = "select COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='users'	";
                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {

                    //if (caller_type == 3)
                    //{

                    //    return;
                    //}
                    //else
                    //{
                    this.Close();
                    MainStaticClass.Code_right_of_user = 1;
                    MainStaticClass.Main.InitializeComponent1();
                    conn.Close();
                    return;
                    //}
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                conn.Close();
            }

            if ((count_users() == 0) && (input_barcode.Text.Trim() == "1"))
            {//Пользователей еще нет, это первый вход 
                this.Close();
                MainStaticClass.Code_right_of_user = 1;
                MainStaticClass.Main.InitializeComponent1();
            }
            else
            {

                //if (barcode.Length < 10)
                //{
                //    MessageBox.Show("Неверный код входа");
                //    return;
                //}

                //Если версия программы < 18.12.2017 тогда вызываем старую процедуру 
                //иначе если версия = 18.12.2017 тогда вызываем промежуточную
                //иначе если версия > 18.12.2017 тогда вызываем новую
                //string cash_version = MainStaticClass.version();
                //int year = int.Parse("" + cash_version.Substring(6, 5).Replace(".", ""));
                //int month = int.Parse("" + cash_version.Substring(3, 2).Replace(".", ""));
                //int day = int.Parse("" + cash_version.Substring(0, 2));
                //DateTime date_version_program = new DateTime(year, month, day);
                //if (new DateTime(2017, 12, 18) <= date_version_program)
                //{
                    //проверим схему бд она старая или уже новая 
                    //if (MainStaticClass.check_new_shema_autenticate() == 0)
                    //{
                    //    result = find_user_role(input_barcode.Text);
                    //}

                    if (MainStaticClass.check_new_shema_autenticate() == 1)
                    {
                        result = find_user_role_new(input_barcode.Text);
                    }
                    else
                    {
                        MessageBox.Show(" Из за произошедших ошибок авторизация невозможна ");
                        result = 0;
                    }
                //}
                //else
                //{
                //    //result = find_user_role(input_barcode.Text);
                //    result = find_user_role_new(input_barcode.Text);
                //}
            }
            if (result == 1)
            {
                if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                {
                    result_execute_enter = true;
                    stop_com_barcode_scaner();
                }
                MainStaticClass.First_Login_Admin = true;
                this.Close();

                if ((caller_type == 3) && (cc != null))//Это авторизация на удаление чека
                {
                    cc.enable_delete = true;
                    return;
                }

                MainStaticClass.Code_right_of_user = 1;
                MainStaticClass.Main.InitializeComponent1();
                //MainStaticClass.write_event_dssl_in_log("POSNG_ADMIN_LOGIN","", MainStaticClass.Cash_Operator,DateTime.Now.Date, DateTime.Now.TimeOfDay,0,0,0,"","",MainStaticClass.CashDeskNumber.ToString(),"");
                //if (MainStaticClass.Use_Trassir > 0)
                //{
                //    string s = MainStaticClass.get_string_message_for_trassir("POSNG_ADMIN_LOGIN", "", MainStaticClass.Cash_Operator, DateTime.Now.Date.ToString("dd'/'MM'/'yyyy"), DateTime.Now.ToString("HH:mm:ss"), "", "", "", "", "", MainStaticClass.CashDeskNumber.ToString(), "");
                //    MainStaticClass.send_data_trassir(s);
                //}       
            }
            else if (result == 2)
            {
                if (MainStaticClass.Use_Usb_to_Com_Barcode_Scaner)
                {
                    result_execute_enter = true;
                    stop_com_barcode_scaner();
                }
                if (!MainStaticClass.First_Login_Admin)
                {
                    MessageBox.Show(" Первая регистрация должна с правами администратора ");
                    return;
                }
                this.Close();
                MainStaticClass.Main.show_Cash_checks();
                //MainStaticClass.write_event_dssl_in_log("POSNG_CASHIER_LOGIN", "", MainStaticClass.Cash_Operator, DateTime.Now.Date, DateTime.Now.TimeOfDay, 0, 0, 0, "", "", MainStaticClass.CashDeskNumber.ToString(), "");
                //MainStaticClass.write_event_dssl_in_log("POSNG_CASHIER_LOGIN", MainStaticClass.Cash_Operator_Client_Code, DateTime.Now, 0);
            }
            else if (result == 13)
            {
                fail_autorize.Text = " У вас нет прав для входа в программу ";
                this.input_barcode.Focus();
            }
            else if (result == 0)
            {

                fail_autorize.Text = "Неудачная попытка авторизации";
                this.input_barcode.Focus();
            }

            this.input_barcode.Text = "";
        }


        private void input_barcode_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
            {
                e.Handled = true;
            }

            //if (!e.Handled)
            //{
            //    if (input_barcode.Text.Trim().Length == 1)
            //    {
            //        input_barcode_timer.Start();
            //    }
            //}

            if (e.KeyChar == 13)
            {
                execute_enter(this.input_barcode.Text);
            }
            //    execute_enter(input_barcode.Text);

            //    //Проверка наличия таблицы если не найдена то это первый запуск

            //    NpgsqlConnection conn = null;
            //    try
            //    {
            //        conn = MainStaticClass.NpgsqlConn();
            //        conn.Open();
            //        NpgsqlCommand command = new NpgsqlCommand();
            //        command.Connection = conn;
            //        command.CommandText = "select COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='users'	";
            //        if (Convert.ToInt16(command.ExecuteScalar()) == 0)
            //        {
            //            this.Close();
            //            MainStaticClass.Code_right_of_user = 1;
            //            MainStaticClass.Main.InitializeComponent1();
            //            conn.Close();
            //            return;
            //        }
            //    }
            //    catch (NpgsqlException ex)
            //    {
            //        MessageBox.Show(ex.Message + " | " + ex.Detail);
            //        conn.Close();                    
            //    }


            //    if ((count_users() == 0) && (input_barcode.Text.Trim() == "1"))
            //    {//Пользователей еще нет, это первый вход 
            //        this.Close();
            //        MainStaticClass.Code_right_of_user = 1;
            //        MainStaticClass.Main.InitializeComponent1();
            //    }
            //    else
            //    {
            //        result = find_user_role(input_barcode.Text);
            //    }
            //}
            //if (result == 1)
            //{
            //    this.Close();
            //    MainStaticClass.Code_right_of_user = 1;
            //    MainStaticClass.Main.InitializeComponent1();
            //}
            //else if (result == 2)
            //{
            //    this.Close();
            //    MainStaticClass.Main.show_Cash_checks();
            //}
            //else if (result == 0)
            //{
            //    MessageBox.Show("не найден");
            //    this.input_barcode.Text = "";
            //}
            //else if (e.KeyChar == 27)
            //{                
            //    this.Close();
            //}
        }

    }
}
