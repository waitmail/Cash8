using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Npgsql;
using System.Drawing.Printing;

namespace Cash8
{
    public partial class Constants : Form
    {
        private int m_cash_desk_number = 0;
        //private System.IO.Ports.SerialPort mySerial;

        public Constants()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.unloading_period.KeyPress += new KeyPressEventHandler(unloading_period_KeyPress);
            this.cash_desk_number.KeyPress += new KeyPressEventHandler(cash_desk_number_KeyPress);            
            this.Load += new EventHandler(Constants_Load);            
        }


        private void txtB_sum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                if ((e.KeyChar != (char)Keys.Delete) || (e.KeyChar != (char)Keys.Back))
                {
                    e.Handled = true;
                }
            }            
        }

        private void txtB_ip_addr_trassir_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                if ((e.KeyChar != (char)Keys.Delete) || (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
            }
        }

        private void inn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void unloading_period_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        
        //private void size_font_listview_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (!(Char.IsDigit(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
        //    {
        //        e.Handled = true;
        //    }
        //}

        

        private void width_of_symbols_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.')))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
            //else
            //{
            //    if (e.KeyChar == '0')
            //    {
            //        e.Handled = true;
            //    }
            //}
        }


        private void Constants_Load(object sender, EventArgs e)
        {
            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT nick_shop,cash_desk_number,use_debug,code_shop,"+
                    " path_for_web_service,currency,unloading_period,last_date_download_bonus_clients,"+
                    " envd,pass_promo,print_m,system_taxation,work_schema,version_fn,enable_stock_processing_in_memory," +
                    " id_acquirer_terminal,ip_address_acquiring_terminal,self_service_kiosk "+
                    " FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    this.nick_shop.Text = reader["nick_shop"].ToString();
                    this.cash_desk_number.Text = reader["cash_desk_number"].ToString();
                    m_cash_desk_number = Convert.ToInt32(reader["cash_desk_number"]);
                    if (Convert.ToBoolean(reader["use_debug"]))
                    {
                        this.use_debug.CheckState = CheckState.Checked;
                    }
                    //this.code_shop.Text = reader["code_shop"].ToString();
                    this.path_for_web_service.Text = reader["path_for_web_service"].ToString();
                    this.currency.Text = reader["currency"].ToString();                    
                    this.unloading_period.Text = reader["unloading_period"].ToString();                    
                    this.txtB_last_date_download_bonus_clients.Text = (reader["last_date_download_bonus_clients"].ToString() == "" ? new DateTime(2000, 1, 1).ToString("dd-MM-yyyy") : Convert.ToDateTime(reader["last_date_download_bonus_clients"]).ToString("dd-MM-yyyy"));
                    //this.checkBox_envd.CheckState = (reader["envd"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_print_m.CheckState = (reader["print_m"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    //this.checkBox_osn_usnIncomeOutcome.CheckState = (reader["usn_income_out_come"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.comboBox_system_taxation.SelectedIndex = Convert.ToInt16(reader["system_taxation"]);
                    this.txtB_work_schema.Text = reader["work_schema"].ToString();
                    this.txtB_version_fn.Text = reader["version_fn"].ToString();
                    this.checkBox_enable_stock_processing_in_memory.CheckState= (reader["enable_stock_processing_in_memory"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.txtB_id_acquiring_terminal.Text= reader["id_acquirer_terminal"].ToString();
                    this.txtB_ip_address_acquiring_terminal.Text= reader["ip_address_acquiring_terminal"].ToString();
                    this.checkBox_self_service_kiosk.CheckState = (reader["self_service_kiosk"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                }
                reader.Close();                
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }


        void cash_desk_number_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)) && !((e.KeyChar == '.')))
            {
                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
            //else
            //{
            //    if (e.KeyChar == '0')
            //    {
            //        e.Handled = true; 
            //    }
            //}

        }


        private void _close__Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private bool check_exists()
        {
            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT COUNT(*) FROM checks_header WHERE checks_header.date_time_write BETWEEN '" + DateTime.Now.Date +
                  "' and '" + DateTime.Now.Date.AddDays(1) + "'";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt32(command.ExecuteScalar()) != 0)
                {
                    conn.Close();
                    return true;
                }
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

            return false;
        }


        private void write_Click(object sender, EventArgs e)
        {
            if (this.cash_desk_number.Text.Trim().Length == 0)
            {
                MessageBox.Show("Не заполнен номер кассы");
                return;
            }
            if (this.nick_shop.Text.Trim().Length == 0)
            {
                MessageBox.Show("Не заполнен код магазина");
                return;
            }

            //if (main_change_path.Text.Trim().Length == 0)
            //{
            //    MessageBox.Show("Не заполнен путь к папке обмена");
            //    return;
            //}


            if (m_cash_desk_number != 0)
            {
                if (Convert.ToInt16(this.cash_desk_number.Text) != m_cash_desk_number)
                {
                    if (check_exists())
                    {
                        MessageBox.Show("За сегодня существуют чеки , номер кассы изменить невозможно");
                        return;
                    }
                }
            }
            
            if (currency.Text.Trim().Length == 0)
            {
                MessageBox.Show(" Не заполнена валюта ", " Ошибка ввода !!!");
            }


            if (unloading_period.Text.Trim().Length == 0)
            {
                unloading_period.Text = "0";
            }
            else
            {
                if (Convert.ToInt32(unloading_period.Text) != 0)
                {
                    if (Convert.ToInt32(unloading_period.Text) > 10)
                    {
                        MessageBox.Show(" Период выгрузки может быть раным нулю или быть в диапазоне 1-10 ");
                        return;
                    }
                }
            }
                        
            string print_m = (checkBox_print_m.CheckState == CheckState.Unchecked ? "false" : "true");
            string enable_stock_processing_in_memory = (checkBox_enable_stock_processing_in_memory.CheckState == CheckState.Unchecked ? "false" : "true");
            string self_service_kiosk = (checkBox_self_service_kiosk.CheckState == CheckState.Unchecked ? "false" : "true");
            string one_monitors_connected = (checkBox_one_monitors_connected.CheckState == CheckState.Unchecked ? "false" : "true");

            try
            {
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlTransaction tran = conn.BeginTransaction();
                string query = "UPDATE constants SET " +
                    "cash_desk_number =" + cash_desk_number.Text + "," +
                    "nick_shop ='" + nick_shop.Text + "'," +
                    "use_debug =" + get_use_debug() + "," +
                    "path_for_web_service ='" + path_for_web_service.Text + "'," +
                    "currency ='" + currency.Text + "'," +
                    "unloading_period =" + unloading_period.Text + "," +
                    "print_m ='" + print_m + "'," +
                    "last_date_download_bonus_clients ='" + txtB_last_date_download_bonus_clients.Text + "'," +
                    "system_taxation = '" + comboBox_system_taxation.SelectedIndex.ToString() + "'," +
                    "work_schema = '" + txtB_work_schema.Text+"',"+
                    "version_fn = "+txtB_version_fn.Text+","+
                    "enable_stock_processing_in_memory="+ enable_stock_processing_in_memory+","+
                    "id_acquirer_terminal='"+txtB_id_acquiring_terminal.Text.ToString()+"',"+
                    "ip_address_acquiring_terminal='" + txtB_ip_address_acquiring_terminal.Text.ToString().Trim() + "'," +
                    "self_service_kiosk=" + self_service_kiosk+","+
                    "one_monitors_connected="+ one_monitors_connected;                    
       
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                int resul_update = command.ExecuteNonQuery();
                if (resul_update == 0)
                {
                    query = "INSERT INTO constants(cash_desk_number," +
                        "nick_shop," +
                        "use_debug," +                        
                        "path_for_web_service," +
                        "currency," +
                        "unloading_period," +
                        "last_date_download_bonus_clients," +
                        //"envd,"+
                        "print_m,"+
                        "system_taxation,"+
                        "work_schema,"+
                        "version_fn,"+
                        "enable_stock_processing_in_memory,"+
                        "id_acquirer_terminal,"+
                        "ip_address_acquiring_terminal,"+
                        "self_service_kiosk,"+
                        "one_monitors_connected) VALUES(" +
                        cash_desk_number.Text + ",'" +
                        nick_shop.Text + "'," +
                        get_use_debug() + ",'" +                        
                        path_for_web_service.Text + "','" +
                        currency.Text + "','" +
                        unloading_period.Text + "','" +
                        txtB_last_date_download_bonus_clients.Text + "','" +                       
                        print_m+"','"+
                        comboBox_system_taxation.SelectedIndex.ToString()+ "','"+
                        txtB_work_schema.Text+"','"+
                        txtB_version_fn.Text+"','"+
                        enable_stock_processing_in_memory+"','"+
                        txtB_id_acquiring_terminal.Text.ToString()+"','"+
                        txtB_ip_address_acquiring_terminal.ToString().Trim()+"','"+
                        self_service_kiosk+","+
                        one_monitors_connected+")";

                    command = new NpgsqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                tran.Commit();
                conn.Close();
                this.Close();
                MessageBox.Show(" Для применения новых параметров программа будет закрыта");
                Application.Exit();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
              
        private string get_use_debug()
        {
            if (use_debug.CheckState == CheckState.Checked)
            {
                return true.ToString();
            }
            else
            {
                return false.ToString();
            }
        }
    }
}
