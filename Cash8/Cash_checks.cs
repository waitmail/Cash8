using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using Npgsql;
using System.Threading.Tasks;



namespace Cash8
{
    public partial class Cash_checks : Form
    {
        //private Nomenklatura tovar;
        public delegate void set_message_on_txtB_not_unloaded_docs(string message);
        //public delegate void set_color_on_have_internet(Color color);
        //public delegate void set_message_on_ofd_exchange_status(string message);
        private bool new_document = false;
        private string[] print_data;
        private int count_pages = 0;
        private System.Timers.Timer timer = new System.Timers.Timer(60000);
        private DateTime timer_execute = DateTime.Now;
        private ToolTip toolTip = new ToolTip();

        public Cash_checks()
        {
            InitializeComponent();
            //this.listView1.Font = new System.Drawing.Font("Microsoft Sans Serif", MainStaticClass.Font_list_view(), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.KeyPreview = true;
            this.Load+=new EventHandler(Cash_checks_Load);
        }       

        //protected override void OnMinimumSizeChanged(EventArgs e)
        //{
        //    base.OnMinimumSizeChanged(e);
        //}

        private bool all_is_filled()
        {
            bool result = true;
            //NpgsqlConnection conn = null;
            //try
            //{
            try
            {
                if (MainStaticClass.Nick_Shop.Trim().Length == 0)
                {
                    MessageBox.Show("Не заполнен код магазина");
                    return false;
                }
                if (MainStaticClass.CashDeskNumber == 0)
                {
                    MessageBox.Show("Номер кассы не может быть ноль");
                    return false;
                }
                if (MainStaticClass.Cash_Operator.Trim().Length == 0)
                {
                    MessageBox.Show("Не заполнен Кассир");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" all_is_filled "+ex.Message);
            }
            
            return result;
        }

        private void to_change_the_document_status()
        {
            NpgsqlConnection conn = null;
            string myQuery = "";
            string update_value = "";
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                if (listView1.Items[listView1.SelectedIndices[0]].Text == "0")
                {
                    update_value = "1";
                }
                else
                {
                    update_value = "0";
                }
                myQuery = "UPDATE checks_header   SET its_deleted=" + update_value + " where date_time_write = '" + listView1.Items[listView1.SelectedIndices[0]].SubItems[1].Text + "'";
                NpgsqlCommand command = new NpgsqlCommand(myQuery, conn);
                object result = command.ExecuteScalar();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при работе с базой данных");
            }
            finally
            {
                conn.Close();
            }
        }


        private string get_num_doc_on_date_time_write(string date_time_write)
        {
            string result = "";

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT document_number FROM checks_header WHERE date_time_write='" + date_time_write + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                result = command.ExecuteScalar().ToString();
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("get_num_doc_on_date_time_write "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("get_num_doc_on_date_time_write "+ex.Message);
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
        
        private bool this_check_the_last(string datetime)
        {
            bool result = false;
            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT date_time_write  FROM checks_header order by date_time_write desc";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                //object result_query = command.ExecuteScalar();
                if (datetime.Trim() == Convert.ToDateTime(command.ExecuteScalar()).ToString("yyyy-MM-dd HH:mm:ss").Trim())
                {
                    result = true;
                }
                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("this_check_the_last "+ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(" this_check_the_last "+ex.Message);
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


        protected override void OnKeyDown(KeyEventArgs e)
        {
            //base.OnKeyDown(e);
            if (e.KeyCode == Keys.F4 && e.Alt)
            {
                e.Handled = true;
            }            
            //if ((e.KeyCode == Keys.Insert)||(e.KeyCode == Keys.F7))
            if (e.KeyCode == Keys.Insert)
            {
                if (DateTime.Now <= MainStaticClass.GetMinDateWork)
                {
                    MessageBox.Show(" У ВАС УСТАНОВЛЕНА НЕПРАВИЛЬНАЯ ДАТА НА КОМПЬЮТЕРЕ !!! ДАЛЬНЕЙШАЯ РАБОТА С ЧЕКАМИ НЕВОЗМОЖНА !!!");
                    return;
                }
                if (MainStaticClass.GetDoNotPromptMarkingCode == 0)
                {
                    if (MainStaticClass.CashDeskNumber != 9)
                    {
                        bool restart = false; bool errors = false;
                        MainStaticClass.check_version_fn(ref restart, ref errors);
                        if (errors)
                        {
                            return;
                        }
                        if (restart)
                        {
                            MessageBox.Show("У вас неверно была установлена версия ФН,НЕОБХОДИМ ПЕРЕЗАПУСК КАССОВОЙ ПРОГРАММЫ !!!");
                            this.Close();
                        }
                    }
                    if (MainStaticClass.SystemTaxation == 0)
                    {
                        MessageBox.Show("У вас не заполнена система налогообложения!\r\nСоздание и печать чеков невозможна!\r\nОБРАЩАЙТЕСЬ В БУХГАЛТЕРИЮ!");
                        return;
                    }
                }

                //Проверка на заполненность обяз реквизитов
                if (all_is_filled())
                {
                    if (new_document)
                    {
                        return;
                    }                    
                    
                        if (txtB_cashier.Text.Trim().Length == 0)
                        {
                            MessageBox.Show("Не заполнен кассир");
                            return;
                        }

                    MainStaticClass.validate_date_time_with_fn(15);

                    new_document = true;
                    Cash_check doc = new Cash_check();
                    doc.cashier = txtB_cashier.Text;
                    doc.ShowDialog();
                    doc.Dispose();
                    new_document = false;                    
                    loaddocuments();
                    //SendDataOnSalesPortions sdsp = new SendDataOnSalesPortions();
                    //sdsp.send_sales_data_Click(null, null);
                    //sdsp.Dispose();
                }                
            }
            if (e.KeyCode == Keys.F10)
            {                
                FPTK22 fptk22 = new FPTK22();
                fptk22.ShowDialog();
            }          
            if ((e.KeyCode == Keys.F12)||(e.KeyCode==Keys.D))
            {                
                if (new_document)
                {
                    return;
                }
                this.Close();
                MainStaticClass.Main.menuStrip.Items.Clear();
                MainStaticClass.Main.start_interface_switching();
            }
            if (listView1.Focused)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (listView1.SelectedItems.Count == 0)
                    {
                        return;
                    }

                    Cash_check doc = new Cash_check();
                    doc.itsnew = false;
                    doc.date_time_write = listView1.Items[listView1.SelectedIndices[0]].SubItems[1].Text;
                    doc.ShowDialog();
                    loaddocuments();
                }
            }
        }

        /// <summary>
        /// Проверка необходима ли отправка с этой кассы
        /// </summary>
        private void check_send_document()
        {

        }

        private void Cash_checks_Load(object sender, System.EventArgs e)
        {

            if (listView1.Columns.Count == 6)
            {
                return;
            }

            if (System.IO.File.Exists(Application.StartupPath + "\\Pictures\\ExistUpdateProgramm.jpg"))
            {
                pictureBox_get_update_program.Load(Application.StartupPath + "\\Pictures\\ExistUpdateProgramm.jpg");
            }

            int result = MainStaticClass.get_unloading_interval();
            if (result != 0)
            {                
                timer.Start();
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
                //timer_Elapsed(null, null);
                //get_status_send_document();
            }

            this.num_cash.Text = "КАССА № " + MainStaticClass.CashDeskNumber.ToString();
            this.txtB_cashier.Text = MainStaticClass.Cash_Operator;

            //this.data_start.Text = monthCalendar1.SelectionStart.ToString();
            //this.data_finish.Text = monthCalendar1.SelectionStart.AddSeconds(86399).ToString();
            // Set the view to show details.
            listView1.View = View.Details;

            // Allow the user to rearrange columns.
            listView1.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            listView1.FullRowSelect = true;

            // Display grid lines.
            listView1.GridLines = true;
            listView1.Columns.Clear();
            listView1.Columns.Add("Статус", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("Дата", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Клиент", 180, HorizontalAlignment.Left);
            listView1.Columns.Add("Сумма", 100, HorizontalAlignment.Right);
            listView1.Columns.Add("Сдача", 100, HorizontalAlignment.Right);
            listView1.Columns.Add("Тип чека ", 100, HorizontalAlignment.Right);
            listView1.Columns.Add("Коментарий", 50, HorizontalAlignment.Right);
            listView1.Columns.Add("Номер", 100, HorizontalAlignment.Right);
            
            //this.WindowState = FormWindowState.Normal;
            this.WindowState = FormWindowState.Maximized;

            if (MainStaticClass.Code_right_of_user == 2)
            {
                label2.Enabled = false;
                dateTimePicker1.Enabled = false;
                fill.Enabled = false;            
                label2.Visible = false;
                this.FormBorderStyle = FormBorderStyle.None;
            }
            
            this.Top = 0;
            this.Left = 0;
            this.Size = new System.Drawing.Size(SystemInformation.PrimaryMonitorSize.Width, SystemInformation.PrimaryMonitorSize.Height);
            this.TopMost = true;

            loaddocuments();

            if (MainStaticClass.Code_right_of_user != 1)
            {
                checkBox_show_3_last_checks.Enabled = false;
            }

            if (MainStaticClass.Code_right_of_user != 1)
            {
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
            }
            
            if (MainStaticClass.Code_right_of_user == 1)
            {                
                fill.BackColor= System.Drawing.Color.FromArgb(255, 255, 192);
            }

            toolTip.ToolTipTitle = " Когда флажок выбран ";
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.IsBalloon = true; // Для отображения подсказки в виде "баллона"

            toolTip.SetToolTip(this.checkBox_show_3_last_checks, "Отображать последние 3 чека");
        }

        protected override void OnClosed(EventArgs e)
        {
            this.timer.Stop();
            this.timer.Dispose();
        }

        public void set_text_on_txtB_not_unloaded_docs(string message)
        {
            txtB_not_unloaded_docs.Text = message;
        }


        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Thread potok = new Thread(get_exists_internet);
            //potok.Start();            
            //Thread potok1 = new Thread(get_ofd_exchange_status);
            //potok1.Start();


            //string result = "";

            //string documents_not_out = MainStaticClass.get_documents_not_out().ToString();

            //if (documents_not_out == "-1")
            //{
            //    result = " Произошли ошибки при получении кол-ва неотправленных документов, ";
            //}
            //else
            //{
            //    result = " Не отправлено документов "+documents_not_out+",";
            //}

            //int documents_out_of_the_range_of_dates = MainStaticClass.get_documents_out_of_the_range_of_dates();

            //if (documents_out_of_the_range_of_dates == -1)
            //{
            //    result += " Не удалось получить дату с сервера "; 
            //}
            //else if (documents_out_of_the_range_of_dates == -2)
            //{
            //    result += " Не удалось получить количество документов вне диапазона ";
            //}
            //else if (documents_out_of_the_range_of_dates > 0)
            //{
            //    result += " За диапазоном находится " + documents_out_of_the_range_of_dates.ToString();
            //}

            //result += "  "+DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            //Invoke(new set_message_on_txtB_not_unloaded_docs(set_text_on_txtB_not_unloaded_docs), new object[] { result });
            if (DateTime.Now > timer_execute.AddSeconds(59))
            {
                timer.Stop();
                get_status_send_document();
                timer.Start();
                timer_execute = DateTime.Now;
                if (MainStaticClass.CheckNewVersionProgramm())
                {
                    pictureBox_get_update_program.Visible = true;
                }
            }

        }
        
        private void get_status_send_document()
        {
            string result = "";

            string documents_not_out = MainStaticClass.get_documents_not_out().ToString();

            if (documents_not_out == "-1")
            {
                result = " Произошли ошибки при получении кол-ва неотправленных документов, ";
            }
            else
            {
                result = " Не отправлено документов " + documents_not_out + ",";
            }

            int documents_out_of_the_range_of_dates = MainStaticClass.get_documents_out_of_the_range_of_dates();

            if (documents_out_of_the_range_of_dates == -1)
            {
                result += " Не удалось получить дату с сервера ";
            }
            else if (documents_out_of_the_range_of_dates == -2)
            {
                result += " Не удалось получить количество документов вне диапазона ";
            }
            else if (documents_out_of_the_range_of_dates > 0)
            {
                result += " За диапазоном находится " + documents_out_of_the_range_of_dates.ToString();
            }

            result += "  " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            Invoke(new set_message_on_txtB_not_unloaded_docs(set_text_on_txtB_not_unloaded_docs), new object[] { result });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
              

        protected override void OnClosing(CancelEventArgs e)
        {
            MainStaticClass.remove_window(this.GetType().FullName);
        }

        public void loaddocuments()
        {
            listView1.Items.Clear();
            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //string myQuery = "SELECT checks_header.date_time_write,clients.name,checks_header.cash,checks_header.remainder  FROM checks_header left join clients ON checks_header.client=clients.code WHERE checks_header.date_time_write BETWEEN '" + data_start.Text + "' and '" + data_finish.Text + "' order by checks_header.date_time_write  ";
                string myQuery = "SELECT checks_header.its_deleted,"+
                    " checks_header.date_time_write,"+
                    " clients.name,"+
                    " checks_header.cash,"+
                    " checks_header.remainder,"+
                    " checks_header.comment,"+                    
                    " checks_header.its_print,"+
                    " checks_header.check_type,checks_header.document_number,checks_header.its_print_p  FROM checks_header "+
                    " LEFT JOIN clients ON checks_header.client = clients.code " +
                    //" LEFT JOIN clients ON checks_header.client = CASE WHEN LEFT(checks_header.client, 1) = '9' THEN clients.phone ELSE clients.code END  "+
                    " WHERE checks_header.date_time_write BETWEEN '" + dateTimePicker1.Value.ToString("yyy-MM-dd") + " 00:00:00" + "' and '" + 
                    dateTimePicker1.Value.AddDays(1).ToString("yyy-MM-dd") + " 00:00:00" + "' AND its_deleted<2 order by checks_header.date_time_write  ";
                if (checkBox_show_3_last_checks.CheckState == CheckState.Checked)
                {
                    myQuery += " desc limit 3 ";
                }
                //MessageBox.Show(myQuery);
                NpgsqlCommand command = new NpgsqlCommand(myQuery, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    //ListViewItem lvi = new ListViewItem(reader.GetDateTime(0).ToString("yyy-MM-dd HH:mm:ss"));
                    ListViewItem lvi = new ListViewItem(reader.GetDecimal(0).ToString());
                    lvi.SubItems.Add(reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss"));
                    lvi.SubItems.Add(reader[2].ToString());
                    lvi.SubItems.Add(reader.GetDecimal(3).ToString());
                    lvi.SubItems.Add(reader.GetDecimal(4).ToString());
                    if (reader["check_type"].ToString() == "0")
                    {
                        lvi.SubItems.Add("Продажа");
                    }
                    else if (reader["check_type"].ToString() == "1")
                    {
                        lvi.SubItems.Add("Возврат");
                    }
                    else if (reader["check_type"].ToString() == "2")
                    {
                        lvi.SubItems.Add("Коррекция прихода");
                    }

                    lvi.SubItems.Add(reader.GetString(5));

                    if (MainStaticClass.Use_Fiscall_Print)
                    {
                        if (!Convert.IsDBNull(reader[6]) && !Convert.IsDBNull(reader[9]))
                        {
                            if ((!reader.GetBoolean(6)) || (!reader.GetBoolean(9)))
                            {
                                if (reader.GetDecimal(0) == 0)
                                {
                                    //lvi.UseItemStyleForSubItems = false;
                                    lvi.BackColor = Color.Pink;
                                    lvi.Font = new System.Drawing.Font("Microsoft Sans Serif", 18, System.Drawing.FontStyle.Underline);
                                    //lvi.SubItems[1].Font = new Font(lvi.SubItems[1].Font, lvi.SubItems[1].Font.Style | FontStyle.Bold);
                                    //lvi.SubItems[2].Font = new Font(lvi.SubItems[2].Font, lvi.SubItems[2].Font.Style | FontStyle.Bold);
                                    //lvi.SubItems[3].Font = new Font(lvi.SubItems[3].Font, lvi.SubItems[3].Font.Style | FontStyle.Bold);
                                }
                            }
                        }
                        else
                        {
                            if (reader.GetDecimal(0) == 0)
                            {                                
                                lvi.BackColor = Color.Pink;
                                lvi.Font = new System.Drawing.Font("Microsoft Sans Serif", 18, System.Drawing.FontStyle.Underline);                                
                            }
                        }
                    }
                    lvi.SubItems.Add(reader["document_number"].ToString());
                    if (reader.GetDecimal(0) == 1)//Это удаленный чек
                    {
                        lvi.Font = new System.Drawing.Font("Microsoft Sans Serif", 18, System.Drawing.FontStyle.Strikeout);
                    }
                    listView1.Items.Add(lvi);
                }
                reader.Close();
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" loaddocuments "+ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            listView1.Focus();
            this.listView1.Select();
            if (checkBox_show_3_last_checks.CheckState == CheckState.Checked)
            {
                if (listView1.Items.Count > 0)
                {
                    this.listView1.Items[0].Selected = true;
                    this.listView1.Items[0].Focused = true;
                    this.listView1.EnsureVisible(0);
                }
            }
            else
            {
                if (listView1.Items.Count > 0)
                {
                    this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                    this.listView1.Items[this.listView1.Items.Count - 1].Focused = true;
                    this.listView1.EnsureVisible(this.listView1.Items.Count - 1);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            loaddocuments();
        }

        private void print_control_check()
        {

            StringBuilder print_string = new StringBuilder();

            int width_of_symbols = MainStaticClass.Width_Of_Symbols;

            if (width_of_symbols == 0)
            {
                MessageBox.Show("Не задана ширина печати, печать выполнена не будет ", "Ошибка настройки программы");
                return;
            }
            
            print_string.Append("ЧИСТЫЙ ДОМ - Магазин низких цен №1" + "\r\n");            
            print_string.Append("бытовая химия - косметика - парфюмерия" + "\r\n");            
            print_string.Append("---------------------------------------" + "\r\n");
            
            print_string.Append(MainStaticClass.Nick_Shop + " К №" + MainStaticClass.CashDeskNumber + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + MainStaticClass.Cash_Operator + "\r\n");

            string number = "";

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                string query = "SELECT MAX(document_number)  FROM checks_header  where date_time_write<'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                number = command.ExecuteScalar().ToString();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(" Ошибка при получении номера "+ex.Message);
                number = "Ошибка при получении номера";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении номера "+ex.Message);
                number = "Ошибка при получении номера";
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            print_string.Append("Предыдущий чек № " + number + "\r\n");


            //if (DialogResult.OK == MessageBox.Show("Печатать чек ?", "Печатать чек ?", MessageBoxButtons.OKCancel))
            //{
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(pd_PrintPage);
            //pd.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[0];                            
            pd.PrinterSettings.PrinterName = PrinterSettings.InstalledPrinters[MainStaticClass.get_num_text_pinter()];
            count_pages = -1;
            string TFF = print_string.ToString();
            TFF = TFF.Replace("\r\n", "|");
            print_data = TFF.Split('|');
            pd.Print();
            //}


        }

        void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (count_pages < print_data.Length - 1)
            {
                count_pages++;
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
            }
            e.Graphics.DrawString(print_data[count_pages].ToString(), Font, new SolidBrush(Color.Black), new RectangleF(5, 5, 400, 120000));
        }

        //private void close_day_Click(object sender, EventArgs e)
        //{

        //    if (MessageBox.Show("Вы действительно хотите закрыть день ?", "Закрытие дня ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
        //    {
        //        MessageBox.Show("Закрытие дня отменено", "Закрытие дня ");
        //        return;
        //    }

        //    SendDataOnSales sd = new SendDataOnSales();
        //    sd.ShowDialog();


        //    NpgsqlConnection conn = null;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        string query = "DELETE FROM last_unload_date;INSERT INTO last_unload_date(closing_of_day)VALUES('" + DateTime.Now.Date.ToString("yyy-MM-dd HH:mm:ss") + "');";
        //        NpgsqlCommand command = new NpgsqlCommand(query, conn);
        //        command.ExecuteNonQuery();
        //        conn.Close();
        //        //Unload unload_data = new Unload();
        //        //StringBuilder sb=new StringBuilder();
        //        //sb.Append(DateTime.Now.Date.ToString("yyy-MM-dd")+"\r\n");
        //        //unload_data.unload_cheks_for_day_new(DateTime.Now.Date, sb);
        //        //unload_data.Dispose();
        //        MessageBox.Show("Закрытие дня успешно выполнено");

        //        //Напечатать контрольный чек если не используется фискальный принтер

        //        if (!MainStaticClass.Use_Fiscall_Print)
        //        {
        //            print_control_check();
        //        }

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

        

        private void fill_Click(object sender, EventArgs e)
        {
            loaddocuments();
        }

        //private void send_data_on_sales_Click(object sender, EventArgs e)
        //{
        //    SendDataOnSales sd = new SendDataOnSales();
        //    sd.ShowDialog();
        //}

        private void btn_update_status_send_Click(object sender, EventArgs e)
        {
            MainStaticClass.Last_Write_Check = DateTime.Now;            
            Task.Factory.StartNew(() => execute_send_data());            
            //get_status_send_document();
        }

        private void execute_send_data()
        {
            var parentForm = this.MdiParent as Main;
            parentForm.timer_send_data_Elapsed(null, null);
            get_status_send_document();
        }

        private void checkBox_show_3_last_checks_CheckedChanged(object sender, EventArgs e)
        {
            loaddocuments();
        }

        private void btn_new_check_Click(object sender, EventArgs e)
        {
            KeyEventArgs e_key = new KeyEventArgs(Keys.Insert);
            OnKeyDown(e_key);
        }

        private void btn_check_actions_Click(object sender, EventArgs e)
        {
            CheckActions checkActions = new CheckActions();
            checkActions.ShowDialog();
        }

        private void pictureBox_get_update_program_Click(object sender, EventArgs e)
        {
            LoadProgramFromInternet loadProgramFromInternet = new LoadProgramFromInternet();
            loadProgramFromInternet.ShowDialog();
        }
    }
}
