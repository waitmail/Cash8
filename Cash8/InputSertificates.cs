﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Cash8
{
    public partial class InputSertificates : Form
    {

        public Pay pay=null;
        public bool closed_normally=false;
        public InputSertificates()
        {
            InitializeComponent();
            this.input_sertificate.KeyDown += new KeyEventHandler(input_sertificate_KeyDown);
            this.listView_sertificates.KeyDown += new KeyEventHandler(listView_sertificates_KeyDown);
            this.Load += new EventHandler(InputSertificates_Load); this.KeyPreview = true;
            this.FormClosing += InputSertificates_FormClosing;
        }

        private void InputSertificates_FormClosing(object sender, FormClosingEventArgs e)
        {            
            if (!closed_normally)
            {
                e.Cancel = true;
                MessageBox.Show("Так нельзя закрывать окно ввода");
            }
        }

        private void listView_sertificates_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listView_sertificates.Items.Count > 0)
                {
                    if (listView_sertificates.SelectedIndices.Count > 0)
                    {
                        //ListViewItem lvi = listView_sertificates.SelectedItems[0];
                        listView_sertificates.Items.RemoveAt(listView_sertificates.SelectedIndices[0]);
                        //if (pay.listView_sertificates.Items.Count > 0)
                        //{                           
                        //    //проверим есть ли такой сертификат в сертификатах оплаты
                        //    foreach (ListViewItem _lvi_ in pay.listView_sertificates.Items)
                        //    {
                        //        if (lvi.SubItems[0].Text == lvi.SubItems[0].Text)
                        //        {
                        //            pay.listView_sertificates.Items.Remove(lvi);
                        //            //MessageBox.Show(" Удален добавленный сертификат ");
                        //            break;
                        //        }
                        //    }
                        //}
                    }
                }
            }            
        }

        private void InputSertificates_Load(object sender, EventArgs e)
        {
            // Set the view to show details.
            listView_sertificates.View = View.Details;

            listView_sertificates.AllowColumnReorder = false;

            // Select the item and subitems when selection is made.
            listView_sertificates.FullRowSelect = true;

            // Display grid lines.
            listView_sertificates.GridLines = true;
            listView_sertificates.MultiSelect = false;
                        
            listView_sertificates.Columns.Add("Код", 100, HorizontalAlignment.Right);
            listView_sertificates.Columns.Add("Сертификат", 400, HorizontalAlignment.Left);                        
            listView_sertificates.Columns.Add("Сумма", 100, HorizontalAlignment.Right);
            listView_sertificates.Columns.Add("Штрихкод", 100, HorizontalAlignment.Left);
            listView_sertificates.Items.Clear();

            if (pay.listView_sertificates.Items.Count > 0)
            {
                foreach (ListViewItem lvi in pay.listView_sertificates.Items)
                {
                    //listView_sertificates.Items.Add((ListViewItem)lvi.Clone());//в Линуксе не работает                     
                    find_sertificate_on_code(lvi.SubItems[3].Text.Trim());
                }
            }
        }


        private void find_sertificate_on_code(string code)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {
                conn.Open();
                string query = "";
                if (code.Length > 6)
                {                    
                    query = "select tovar.code AS tovar_code,tovar.name AS tovar_name ,tovar.retail_price AS retail_price " +
                        " FROM  barcode left join tovar ON barcode.tovar_code=tovar.code " +
                    //" WHERE barcode='" + input_sertificate.Text + "' AND its_deleted=0  AND retail_price<>0 ";
                    " WHERE barcode='" + code + "' AND its_deleted=0  AND retail_price<>0 AND its_certificate=1 ";
                }
                else
                {
                    query = "select tovar.code AS tovar_code,tovar.name  AS tovar_name,tovar.retail_price  AS retail_price " +
                        " FROM tovar where tovar.its_deleted=0 AND retail_price<>0 " +
                        //" AND tovar.code='" + input_sertificate.Text + "'";
                    " AND tovar.code='" + code + "' AND its_certificate=1";                 
                }
                input_sertificate.Text = "";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                bool have = false;
                while (reader.Read())
                {
                    have = true;
                    //проверка есть ли такой уже сертификат
                    bool exist = false;
                    if (listView_sertificates.Items.Count > 0)
                    {
                        for (int i = 0; i < listView_sertificates.Items.Count; i++)
                        {
                            //if (listView_sertificates.Items[i].Tag.ToString().Trim() == reader["tovar_code"].ToString().Trim())
                            if (listView_sertificates.Items[i].SubItems[3].Text.Trim() == code)
                            {
                                MessageBox.Show("Сертификат с номером " + code + " уже выбран в строках ");
                                exist = true;
                                break;                                
                            }
                        }
                    }
                    if (!exist)
                    {
                        ListViewItem lvi = new ListViewItem(reader["tovar_code"].ToString());
                        lvi.Tag = reader["tovar_code"].ToString();
                        lvi.SubItems.Add(reader["tovar_name"].ToString());  // Наименование 
                        lvi.SubItems.Add(reader["retail_price"].ToString());//Сумма
                        lvi.SubItems.Add(code);//Сумма
                        listView_sertificates.Items.Add(lvi);
                        listView_sertificates.Focus();
                        listView_sertificates.Items[listView_sertificates.Items.Count - 1].Focused = true;
                        listView_sertificates.Items[listView_sertificates.Items.Count - 1].Selected = true;
                    }
                }
                reader.Close();
                command.Dispose();
                conn.Close();
                if (!have)
                {
                    MessageBox.Show(" Сертификат с номером "+ code+" не найден ");
                }
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


        private void input_sertificate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                find_sertificate_on_code(input_sertificate.Text); 
            }
           
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                input_sertificate.Focus();
            }
            else if (e.KeyCode == Keys.F12)
            {
                if (button_commit.Enabled)
                {
                    button_commit_Click(null, null);
                }
            }
            else if (e.KeyCode == Keys.F5)
            {
                //button_cancel_Click(null, null);
            }
        }



        private bool check_sertificate_active(string sertificate_code)
        {
            bool result = true;
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;

            // Запуск функции с параметром в новом потоке            
            Task<bool> task = Task.Factory.StartNew(() => check_sertificate_active1(sertificate_code));

            try
            {
                // Ожидание результата функции в течение 5 секунд
                bool isCompletedSuccessfully = task.Wait(TimeSpan.FromSeconds(5));

                if (isCompletedSuccessfully)
                {
                    // Если задача завершена успешно, получаем результат
                    MainStaticClass.write_event_in_log("Удачное завершение check_sertificate_active " + sertificate_code, "Документ чек", "0");
                    result = task.Result;
                    //Console.WriteLine("Результат функции: " + result);
                }
                else
                {
                    // Если результат не был получен в течение 5 секунд
                    //Console.WriteLine("Функция не завершила выполнение в отведённое время.");
                    result = false;
                    MainStaticClass.write_event_in_log("Произошли ошибка при check_sertificate_active " + sertificate_code + " Timeout ", "Документ чек", "0");
                    cts.Cancel(); // Отправка запроса на отмену задачи
                    MessageBox.Show("Внешний таймаут при проверке активности сертификата ", "check_sertificate_active");
                }
            }
            catch (AggregateException ae)
            {
                //Обработка исключений, которые могли быть выброшены во время выполнения функции
                foreach (var e in ae.InnerExceptions)
                {
                    //Console.WriteLine("Исключение: " + e.Message);
                    MainStaticClass.write_event_in_log("Произошли ошибка при check_sertificate_active " + sertificate_code + " " + e.Message, "Документ чек", "0");
                }
                result = false;
            }

            return result;
        }


        private bool check_sertificate_active1(string sertificate_code)
        {
            bool result = true;

            Cash8.DS.DS ds = MainStaticClass.get_ds();
            ds.Timeout = 5000;
           
            //Получить параметр для запроса на сервер 
            string nick_shop = MainStaticClass.Nick_Shop.Trim();
            if (nick_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить название магазина ");
                return false;
            }
            string code_shop = MainStaticClass.Code_Shop.Trim();
            if (code_shop.Trim().Length == 0)
            {
                MessageBox.Show(" Не удалось получить код магазина ");
                return false;
            }
            string count_day = CryptorEngine.get_count_day();
            string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
//            string sertificate_code = get_tovar_code(barcode);
            string encrypt_data = CryptorEngine.Encrypt(sertificate_code, true, key);
            string status = "-1";
            try
            {
                status = ds.GetStatusSertificat(MainStaticClass.Nick_Shop, encrypt_data,MainStaticClass.GetWorkSchema.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (status == "-1")
            {
                MessageBox.Show("Произошли ошибки при работе с сертификатами");
                return false;
            }
            else
            {
                string decrypt_data = CryptorEngine.Decrypt(status, true, key);
                if (decrypt_data != "1")
                {
                    MessageBox.Show("Сертификат номер " + sertificate_code + " не активирован");
                    result = false;
                }
            }           
            
            return result;
        }

        /// <summary>
        /// Проверить все сертификаты на то , что они активированы
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_commit_Click(object sender, EventArgs e)
        {
            bool result_check = true;
            foreach (ListViewItem lvi in listView_sertificates.Items)
            {
                //if (!check_sertificate_active(lvi.Tag.ToString()))
                if (!check_sertificate_active(lvi.SubItems[3].Text))
                {
                    result_check = false;
                    break;
                }
            }
            if (!result_check)
            {
                return;
            }

            decimal summ_sertificates = 0;
            pay.listView_sertificates.Items.Clear();
            foreach (ListViewItem lvi in listView_sertificates.Items)
            {
                //ListViewItem item = (ListViewItem)lvi.Clone();
                pay.listView_sertificates.Items.Add((ListViewItem)lvi.Clone());
                summ_sertificates += decimal.Parse(lvi.SubItems[2].Text);
            }

            //if (Convert.ToDecimal(pay.pay_sum.Text) < summ_sertificates + Convert.ToDecimal(pay.non_cash_sum.Text))
            //{
            //    MessageBox.Show("Сумма сертификатов + сумма по карте оплаты превышает сумму чека ");
            //    return;
            //}

            pay.sertificates_sum.Text = summ_sertificates.ToString();
            closed_normally = true;
            this.Close();
        }
    }
}
