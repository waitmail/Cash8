﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using Npgsql;
using System.Drawing.Printing;
using Atol.Drivers10.Fptr;
using AtolConstants = Atol.Drivers10.Fptr.Constants;
using System.Linq;
using System.Text.RegularExpressions;

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
            txtB_constant_conversion_to_kilograms.KeyPress += TxtB_constant_conversion_to_kilograms_KeyPress;
            this.Load += new EventHandler(Constants_Load);
        }

        private void TxtB_constant_conversion_to_kilograms_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем только цифры, клавиши Backspace и Delete
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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

            //comboBox_fn_port
            comboBox_fn_port.Items.Clear();
            string[] sreial_ports = SerialPort.GetPortNames();
            foreach (string s in sreial_ports)
            {
                comboBox_fn_port.Items.Add(s);
            }

            comboBox_scale_port.Items.Clear();
            sreial_ports = SerialPort.GetPortNames();
            foreach (string s in sreial_ports)
            {
                comboBox_scale_port.Items.Add(s);
            }

            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT nick_shop,cash_desk_number,code_shop," +
                    " path_for_web_service,currency,unloading_period,last_date_download_bonus_clients," +
                    " envd,pass_promo,print_m,system_taxation,work_schema,version_fn,enable_stock_processing_in_memory," +
                    " id_acquirer_terminal,ip_address_acquiring_terminal,self_service_kiosk,enable_cdn_markers, " +
                    " webservice_authorize,printing_using_libraries,fn_serial_port,get_weight_automatically,scale_serial_port,"+
                    " variant_connect_fn,fn_ipaddr,acquiring_bank,do_not_prompt_marking_code,constant_conversion_to_kilograms FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    this.nick_shop.Text = reader["nick_shop"].ToString();
                    this.cash_desk_number.Text = reader["cash_desk_number"].ToString();
                    m_cash_desk_number = Convert.ToInt32(reader["cash_desk_number"]);
                    //if (Convert.ToBoolean(reader["use_debug"]))
                    //{
                    //    this.use_debug.CheckState = CheckState.Checked;
                    //}
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
                    //this.checkBox_enable_cdn_markers.CheckState= (reader["enable_stock_processing_in_memory"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.txtB_id_acquiring_terminal.Text = reader["id_acquirer_terminal"].ToString();
                    this.txtB_ip_address_acquiring_terminal.Text = reader["ip_address_acquiring_terminal"].ToString();
                    //this.checkBox_self_service_kiosk.CheckState = (reader["self_service_kiosk"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_enable_cdn_markers.CheckState = (reader["enable_cdn_markers"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    //this.checkBox_version2_marking.CheckState = (reader["version2_marking"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_webservice_authorize.CheckState = (reader["webservice_authorize"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_printing_using_libraries.CheckState = (reader["printing_using_libraries"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_get_weight_automatically.CheckState = (reader["get_weight_automatically"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_do_not_prompt_marking_code.CheckState = (reader["do_not_prompt_marking_code"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);

                    comboBox_variant_connect_fn.SelectedIndex = Convert.ToInt16(reader["variant_connect_fn"]);
                    this.txtB_constant_conversion_to_kilograms.Text = reader["constant_conversion_to_kilograms"].ToString();
                    txtB_fn_ipaddr.Text = reader["fn_ipaddr"].ToString();
                    int index = 0;
                    if (comboBox_variant_connect_fn.SelectedIndex == 0)
                    {
                        index = comboBox_fn_port.Items.IndexOf(reader["fn_serial_port"].ToString());
                        if (index == -1)
                        {
                            btn_trst_connection.Enabled = false;
                            if (this.checkBox_printing_using_libraries.CheckState == CheckState.Checked)
                            {
                                MessageBox.Show(" fn_serial_port в доступных не найден ", "ФР");
                            }
                        }
                        else
                        {
                            this.comboBox_fn_port.SelectedIndex = index;
                        }
                    }
                    else if (comboBox_variant_connect_fn.SelectedIndex == 1)
                    {
                        if (txtB_fn_ipaddr.Text.Trim().Length == 0)
                        {
                            btn_trst_connection.Enabled = false;
                            if (this.checkBox_printing_using_libraries.CheckState == CheckState.Checked)
                            {
                                MessageBox.Show(" ip адрес ФН не заполнен ", "ФР");
                            }
                        }
                    }                    

                    index = comboBox_scale_port.Items.IndexOf(reader["scale_serial_port"].ToString());
                    if (index == -1)
                    {
                        btn_get_weight.Enabled = false;
                        if (this.checkBox_get_weight_automatically.CheckState == CheckState.Checked)
                        {
                            MessageBox.Show(" scale_serial_port в доступных не найден ","Весы");
                        }
                    }
                    else
                    {
                        this.comboBox_scale_port.SelectedIndex = index;
                    }
                    this.comboBox_acquiring_bank.SelectedIndex = Convert.ToInt16(reader["acquiring_bank"]);
                }
                reader.Close();
                //if (nick_shop.Text.Trim() != "A01")
                //{
                //    checkBox_enable_stock_processing_in_memory.Enabled = false;
                //}
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


        private bool check_ip_addr()
        {
            bool result = true;

            string pattern = @"^(\d{1,3}(\.\d{1,3}){3}:\d+)?$";
            bool isValid = Regex.IsMatch(txtB_fn_ipaddr.Text, pattern) && !txtB_fn_ipaddr.Text.Contains(",");

            if (checkBox_printing_using_libraries.Checked && comboBox_variant_connect_fn.SelectedIndex == 1)
            {
                // Если условие истинно, строка не должна быть пустой
                isValid = isValid && !string.IsNullOrEmpty(txtB_fn_ipaddr.Text);
            }

            if (!isValid)
            {
                MessageBox.Show("Строка ип адрес:порт не соответствует формату!", "Проверка ввода ип адреса");
                result = false;
            }

            return result;
        }



        private void write_Click(object sender, EventArgs e)
        {
            if (!check_ip_addr())
            {
                return;
            }
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
            //string enable_stock_processing_in_memory = (checkBox_enable_cdn_markers.CheckState == CheckState.Unchecked ? "false" : "true");
            //string self_service_kiosk = (checkBox_self_service_kiosk.CheckState == CheckState.Unchecked ? "false" : "true");
            string enable_cdn_markers = (checkBox_enable_cdn_markers.CheckState == CheckState.Unchecked ? "false" : "true");
            //string version2_marking = (checkBox_version2_marking.CheckState == CheckState.Unchecked ? "false" : "true");
            string webservice_authorize = (checkBox_webservice_authorize.CheckState == CheckState.Unchecked ? "false" : "true");
            //string static_guid_in_print = (checkBox_static_guid_in_print.CheckState == CheckState.Unchecked ? "false" : "true");
            string printing_using_libraries = (checkBox_printing_using_libraries.CheckState == CheckState.Unchecked ? "false" : "true");
            string get_weight_automatically = (checkBox_get_weight_automatically.CheckState == CheckState.Unchecked ? "false" : "true");
            string do_not_prompt_marking_code = (checkBox_do_not_prompt_marking_code.CheckState == CheckState.Unchecked ? "false" : "true");

            string fn_serial_port = (comboBox_fn_port.Items.Count == 0 ? "" : (comboBox_fn_port.SelectedIndex == -1 ? "" : comboBox_fn_port.SelectedItem.ToString()));
            string scale_serial_port = (comboBox_scale_port.Items.Count == 0 ? "" : (comboBox_scale_port.SelectedIndex == -1 ? "" : comboBox_scale_port.SelectedItem.ToString()));
            //string acquiring_bank  = (comboBox_acquiring_bank.Items.Count == 0 ? "" : (comboBox_acquiring_bank.SelectedIndex == -1 ? "" : comboBox_acquiring_bank.SelectedItem.ToString()));
            
            string variant_connect_fn = comboBox_variant_connect_fn.SelectedIndex.ToString();
            string fn_ipaddr = txtB_fn_ipaddr.Text.Trim();

            try
            {
                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlTransaction tran = conn.BeginTransaction();
                string query = "UPDATE constants SET " +
                    "cash_desk_number =" + cash_desk_number.Text + "," +
                    "nick_shop ='" + nick_shop.Text + "'," +
                    //"use_debug =" + get_use_debug() + "," +
                    "path_for_web_service ='" + path_for_web_service.Text + "'," +
                    "currency ='" + currency.Text + "'," +
                    "unloading_period =" + unloading_period.Text + "," +
                    "print_m ='" + print_m + "'," +
                    "last_date_download_bonus_clients ='" + txtB_last_date_download_bonus_clients.Text + "'," +
                    "system_taxation = '" + comboBox_system_taxation.SelectedIndex.ToString() + "'," +
                    "work_schema = '" + txtB_work_schema.Text + "'," +
                    "version_fn = " + txtB_version_fn.Text + "," +
                    //"enable_stock_processing_in_memory=" + enable_stock_processing_in_memory + "," +
                    "id_acquirer_terminal='" + txtB_id_acquiring_terminal.Text.ToString() + "'," +
                    "ip_address_acquiring_terminal='" + txtB_ip_address_acquiring_terminal.Text.ToString().Trim() + "'," +
                    //"self_service_kiosk=" + self_service_kiosk + "," +
                    "enable_cdn_markers=" + enable_cdn_markers + "," +
                    //"version2_marking=" + version2_marking + "," +
                    "webservice_authorize=" + webservice_authorize + "," +
                    //"static_guid_in_print=" + static_guid_in_print + "," +
                    "printing_using_libraries=" + printing_using_libraries + "," +
                    "fn_serial_port = '" + fn_serial_port + "'," +
                    "scale_serial_port = '" + scale_serial_port + "'," +
                    "get_weight_automatically=" + get_weight_automatically+","+
                    "variant_connect_fn = " + variant_connect_fn+","+
                    "fn_ipaddr='"+ fn_ipaddr+"'"+","+
                    "acquiring_bank= "+comboBox_acquiring_bank.SelectedIndex.ToString()+","+
                    "do_not_prompt_marking_code="+ do_not_prompt_marking_code+","+
                    "constant_conversion_to_kilograms="+ txtB_constant_conversion_to_kilograms.Text.Trim();

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                int resul_update = command.ExecuteNonQuery();
                if (resul_update == 0)
                {
                    query = "INSERT INTO constants(cash_desk_number," +
                        "nick_shop," +
                        //"use_debug," +
                        "path_for_web_service," +
                        "currency," +
                        "unloading_period," +
                        "last_date_download_bonus_clients," +
                        //"envd,"+
                        "print_m," +
                        "system_taxation," +
                        "work_schema," +
                        "version_fn," +
                        //"enable_stock_processing_in_memory,"+
                        "id_acquirer_terminal," +
                        "ip_address_acquiring_terminal," +
                        "self_service_kiosk," +
                        "enable_cdn_markers," +
                        //"version2_marking,"+
                        "webservice_authorize," +
                        //"static_guid_in_print," +
                        "printing_using_libraries," +
                        "fn_serial_port," +
                        "scale_serial_port," +
                        "get_weight_automatically,"+
                        "variant_connect_fn,"+
                        "fn_ipaddr,"+
                        "acquiring_bank,"+
                        "do_not_prompt_marking_code,"+
                        "constant_conversion_to_kilograms) VALUES(" +
                        cash_desk_number.Text + ",'" +
                        nick_shop.Text + "'," +
                        //get_use_debug() + ",'" +
                        path_for_web_service.Text + "','" +
                        currency.Text + "','" +
                        unloading_period.Text + "','" +
                        txtB_last_date_download_bonus_clients.Text + "','" +
                        print_m + "','" +
                        comboBox_system_taxation.SelectedIndex.ToString() + "','" +
                        txtB_work_schema.Text + "','" +
                        txtB_version_fn.Text + "','" +
                        //enable_stock_processing_in_memory+"','"+
                        txtB_id_acquiring_terminal.Text.ToString() + "','" +
                        txtB_ip_address_acquiring_terminal.ToString().Trim() + "','" +
                        //self_service_kiosk+","+
                        enable_cdn_markers + "," +
                        //version2_marking+","+
                        webservice_authorize + "," +
                        //static_guid_in_print+","+
                        printing_using_libraries + ",'" +
                        fn_serial_port+"','"+      //comboBox_fn_port.SelectedItem.ToString() + "','" +
                        scale_serial_port + "'," +
                        get_weight_automatically + ","+
                        variant_connect_fn+",'"+
                        fn_ipaddr+"',"+
                        comboBox_acquiring_bank.SelectedIndex.ToString()+","+
                        do_not_prompt_marking_code +","+
                        txtB_constant_conversion_to_kilograms.Text.Trim()+")";

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

        //private string get_use_debug()
        //{
        //    if (use_debug.CheckState == CheckState.Checked)
        //    {
        //        return true.ToString();
        //    }
        //    else
        //    {
        //        return false.ToString();
        //    }
        //}

        private void comboBox_system_taxation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_system_taxation.SelectedIndex == 3)
            {
                //checkBox_version2_marking.Checked = true;
                //checkBox_enable_stock_processing_in_memory.Checked = true;
                txtB_version_fn.Text = "2";
            }
        }

        private void btn_trst_connection_Click(object sender, EventArgs e)
        {
            IFptr fptr = MainStaticClass.FPTR;
            if (!fptr.isOpened())
            {
                fptr.open();
            }
            if (fptr.printText() < 0)
            {
                MessageBox.Show(fptr.errorCode() + " " + fptr.errorDescription());
            }
            else
            {
                MessageBox.Show("Соединение успешно установлено!");
            }
        }

        private void btn_get_weight_Click(object sender, EventArgs e)
        {
            //bool error = true;
            //Dictionary<double, int> frequencyMap = new Dictionary<double, int>();
            //if (MainStaticClass.GetWeightAutomatically == 1)
            //{
            double weigt = 0;
            //int num = 0;
            //while (num<5)
            //{
            //    num++;
            weigt = MainStaticClass.GetWeight();
            //    if (frequencyMap.ContainsKey(weigt))
            //    {
            //        frequencyMap[weigt]++;
            //    }
            //    else
            //    {
            //        frequencyMap[weigt] = 1;
            //    }
            //}                
            //weigt = frequencyMap.Where(pair => pair.Key > 0) // Фильтруем, оставляя только числа больше нуля
            //    .OrderByDescending(pair => pair.Value) // Сортируем по убыванию частоты
            //    .FirstOrDefault().Key; // Берем первый элемент или значение по умолчанию, если таких нет
            MessageBox.Show(weigt.ToString());//+ (error ? " \r\nОшибка чтения " : "\r\nВес получен").ToString()                
        }

        private void comboBox_variant_connect_fn_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_variant_connect_fn.SelectedIndex == 0)
            {
                //MessageBox.Show("0");
                txtB_fn_ipaddr.Enabled = false;
                comboBox_fn_port.Enabled = true;
            }
            else if (comboBox_variant_connect_fn.SelectedIndex == 1)
            {
                //MessageBox.Show("1");
                txtB_fn_ipaddr.Enabled =true;
                comboBox_fn_port.Enabled = false;
            }
        }
    }
}
