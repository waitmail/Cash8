using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace Cash8
{
    public partial class InputeCodeClient : Form
    {
        //public string barcode = "";
        public Cash8.Cash_check cc = null;

        public InputeCodeClient()
        {
            InitializeComponent();
            this.txtB_inpute_code_client.KeyPress += new KeyPressEventHandler(txtB_inpute_code_client_KeyPress);
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {              
                this.Close();
            }
        }


        private void txtB_inpute_code_client_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 43)
            {
                if (!(Char.IsDigit(e.KeyChar)))
                {
                    if (e.KeyChar != (char)Keys.Back)
                    {
                        e.Handled = true;
                    }
                }
            }

            if (e.KeyChar == (char)Keys.Enter)
            {                
                
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                bool thisclose = true;
                try
                {
                    conn.Open();
                    string query = "SELECT code FROM bonus_cards WHERE pin='" + this.txtB_inpute_code_client.Text.Trim() + "'";
                    this.txtB_inpute_code_client.Text = "";
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    object result_query = command.ExecuteScalar();
                    if (result_query == null)
                    {
                        MessageBox.Show("Бонусная карта с введенным кодом не найдена");
                        return;
                    }
                    string code = result_query.ToString();
                    if (code.Trim() == "")
                    {
                        MessageBox.Show("Бонусная карта не найдена");
                        txtB_inpute_code_client.Text = "";
                        thisclose = false;
                    }
                    else
                    {
                        //ПРЕДВАРИТЕЛЬНАЯ ПРОВЕРКА ТЧ НА НАЛИЧИЕ БОНУСНОЙ КАРТЫ
                        bool finded = false;
                        foreach (ListViewItem lvi in cc.listView1.Items)
                        {
                            if (lvi.SubItems[0].Text.Trim() == "3")
                            {
                                finded = true;
                                break;
                            }
                        }
                        if (finded)
                        {
                            MessageBox.Show("в Т.Ч. Уже есть бонусная карта ");
                            return;
                        }
                        //ДОБАВЛЕНИЕ БОНУСНОЙ КАРТЫ !!!
                        cc.client.Text = code;
                        cc.client.Tag  = code;

                        query = "SELECT code,name, retail_price FROM tovar WHERE code = 3 and its_deleted=0";
                        command = new NpgsqlCommand(query, conn);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            finded = true;//Это значит, что запись найдена  
                            ListViewItem item = new ListViewItem(reader[0].ToString());
                            item.Tag = reader[0].ToString();
                            item.SubItems.Add(reader[1].ToString());
                            item.SubItems.Add("");//Характеристика
                            item.SubItems.Add("1");
                            item.SubItems.Add(reader[2].ToString());//Цена                            
                            item.SubItems.Add(reader[2].ToString());//Сумма
                            item.SubItems.Add(reader[2].ToString());//Цена со скидкой
                            item.SubItems.Add(reader[2].ToString());//Сумма со кидкой
                            item.SubItems.Add("0");
                            item.SubItems.Add("0");
                            item.SubItems.Add("0");
                            item.SubItems.Add("0");
                            item.SubItems.Add("0");
                            item.SubItems.Add("0");
                            cc.listView1.Items.Add(item);
                            break;
                        }
                        if (!finded)//товар дисконтная карта не найден и не добавлен в ТЧ 
                        {
                            return;
                        }
                    }
                    conn.Close();
                    
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(" Ошибки при записи нового кода клиента  " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" Ошибки при записи нового кода клиента  " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                if (thisclose)
                {
                    this.Close();
                }
            }
        }

    }
}
