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
using Atol.Drivers10.Fptr;
using AtolConstants = Atol.Drivers10.Fptr.Constants;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Drawing.Imaging;
using System.IO;
using Newtonsoft.Json;


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
                    " path_for_web_service,unloading_period,last_date_download_bonus_clients," +
                    " system_taxation,version_fn," +
                    " id_acquirer_terminal,ip_address_acquiring_terminal," +//enable_cdn_markers
                    " webservice_authorize,printing_using_libraries,fn_serial_port,get_weight_automatically,scale_serial_port," +
                    " variant_connect_fn,fn_ipaddr,acquiring_bank,constant_conversion_to_kilograms,nds_ip,ip_adress_local_ch_z FROM constants";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    this.comboBox_nds_ip.SelectedIndex = Convert.ToInt16(reader["nds_ip"]);
                    this.nick_shop.Text = reader["nick_shop"].ToString();
                    this.cash_desk_number.Text = reader["cash_desk_number"].ToString();
                    m_cash_desk_number = Convert.ToInt32(reader["cash_desk_number"]);                   
                    this.path_for_web_service.Text = reader["path_for_web_service"].ToString();                    
                    this.unloading_period.Text = reader["unloading_period"].ToString();
                    this.txtB_last_date_download_bonus_clients.Text = (reader["last_date_download_bonus_clients"].ToString() == "" ? new DateTime(2000, 1, 1).ToString("dd-MM-yyyy") : Convert.ToDateTime(reader["last_date_download_bonus_clients"]).ToString("dd-MM-yyyy"));                    
                    this.comboBox_system_taxation.SelectedIndex = Convert.ToInt16(reader["system_taxation"]);                    
                    this.txtB_version_fn.Text = reader["version_fn"].ToString();                    
                    this.txtB_id_acquiring_terminal.Text = reader["id_acquirer_terminal"].ToString();
                    this.txtB_ip_address_acquiring_terminal.Text = reader["ip_address_acquiring_terminal"].ToString();                    
                    //this.checkBox_enable_cdn_markers.CheckState = (reader["enable_cdn_markers"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);                    
                    //this.checkBox_webservice_authorize.CheckState = (reader["webservice_authorize"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_printing_using_libraries.CheckState = (reader["printing_using_libraries"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                    this.checkBox_get_weight_automatically.CheckState = (reader["get_weight_automatically"].ToString().ToLower() == "false" ? CheckState.Unchecked : CheckState.Checked);
                   

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
                    this.txtB_ip_addr_lm_ch_z.Text = reader["ip_adress_local_ch_z"].ToString();
                    
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

            //if (currency.Text.Trim().Length == 0)
            //{
            //    MessageBox.Show(" Не заполнена валюта ", " Ошибка ввода !!!");
            //}


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

            //string print_m = (checkBox_print_m.CheckState == CheckState.Unchecked ? "false" : "true");
            //string enable_stock_processing_in_memory = (checkBox_enable_cdn_markers.CheckState == CheckState.Unchecked ? "false" : "true");
            //string self_service_kiosk = (checkBox_self_service_kiosk.CheckState == CheckState.Unchecked ? "false" : "true");
            //string enable_cdn_markers = (checkBox_enable_cdn_markers.CheckState == CheckState.Unchecked ? "false" : "true");
            //string version2_marking = (checkBox_version2_marking.CheckState == CheckState.Unchecked ? "false" : "true");
            //string webservice_authorize = (checkBox_webservice_authorize.CheckState == CheckState.Unchecked ? "false" : "true");
            //string static_guid_in_print = (checkBox_static_guid_in_print.CheckState == CheckState.Unchecked ? "false" : "true");
            string printing_using_libraries = (checkBox_printing_using_libraries.CheckState == CheckState.Unchecked ? "false" : "true");
            string get_weight_automatically = (checkBox_get_weight_automatically.CheckState == CheckState.Unchecked ? "false" : "true");
            //string do_not_prompt_marking_code = (checkBox_do_not_prompt_marking_code.CheckState == CheckState.Unchecked ? "false" : "true");

            string fn_serial_port = (comboBox_fn_port.Items.Count == 0 ? "" : (comboBox_fn_port.SelectedIndex == -1 ? "" : comboBox_fn_port.SelectedItem.ToString()));
            string scale_serial_port = (comboBox_scale_port.Items.Count == 0 ? "" : (comboBox_scale_port.SelectedIndex == -1 ? "" : comboBox_scale_port.SelectedItem.ToString()));
            //string acquiring_bank  = (comboBox_acquiring_bank.Items.Count == 0 ? "" : (comboBox_acquiring_bank.SelectedIndex == -1 ? "" : comboBox_acquiring_bank.SelectedItem.ToString()));
            
            string variant_connect_fn = comboBox_variant_connect_fn.SelectedIndex.ToString();
            string fn_ipaddr = txtB_fn_ipaddr.Text.Trim();
            string nds_ip = "";
            if (comboBox_nds_ip.SelectedItem != null)            
            {
                nds_ip = comboBox_nds_ip.SelectedIndex.ToString();                
            }
            else
            {
                nds_ip = "0";
            }


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
                    //"currency ='" + currency.Text + "'," +
                    "unloading_period =" + unloading_period.Text + "," +
                    //"print_m ='" + print_m + "'," +
                    "last_date_download_bonus_clients ='" + txtB_last_date_download_bonus_clients.Text + "'," +
                    "system_taxation = '" + comboBox_system_taxation.SelectedIndex.ToString() + "'," +
                    //"work_schema = '" + txtB_work_schema.Text + "'," +
                    "version_fn = " + txtB_version_fn.Text + "," +
                    //"enable_stock_processing_in_memory=" + enable_stock_processing_in_memory + "," +
                    "id_acquirer_terminal='" + txtB_id_acquiring_terminal.Text.ToString() + "'," +
                    "ip_address_acquiring_terminal='" + txtB_ip_address_acquiring_terminal.Text.ToString().Trim() + "'," +
                    //"self_service_kiosk=" + self_service_kiosk + "," +
                    //"enable_cdn_markers=" + enable_cdn_markers + "," +
                    //"version2_marking=" + version2_marking + "," +
                    //"webservice_authorize=" + webservice_authorize + "," +
                    //"static_guid_in_print=" + static_guid_in_print + "," +
                    "printing_using_libraries=" + printing_using_libraries + "," +
                    "fn_serial_port = '" + fn_serial_port + "'," +
                    "scale_serial_port = '" + scale_serial_port + "'," +
                    "get_weight_automatically=" + get_weight_automatically + "," +
                    "variant_connect_fn = " + variant_connect_fn + "," +
                    "fn_ipaddr='" + fn_ipaddr + "'" + "," +
                    "acquiring_bank= " + comboBox_acquiring_bank.SelectedIndex.ToString() + "," +
                    //"do_not_prompt_marking_code="+ do_not_prompt_marking_code+","+
                    "constant_conversion_to_kilograms=" + txtB_constant_conversion_to_kilograms.Text.Trim() + "," +
                    "nds_ip=" + nds_ip + "," +
                    "ip_adress_local_ch_z='" + txtB_ip_addr_lm_ch_z.Text+"'";

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                int resul_update = command.ExecuteNonQuery();
                if (resul_update == 0)
                {
                    query = "INSERT INTO constants(cash_desk_number," +
                        "nick_shop," +
                        //"use_debug," +
                        "path_for_web_service," +
                        //"currency," +
                        "unloading_period," +
                        "last_date_download_bonus_clients," +
                        //"envd,"+
                        //"print_m," +
                        "system_taxation," +
                        //"work_schema," +
                        "version_fn," +
                        //"enable_stock_processing_in_memory,"+
                        "id_acquirer_terminal," +
                        "ip_address_acquiring_terminal," +
                        //"self_service_kiosk," +
                        //"enable_cdn_markers," +
                        //"version2_marking,"+
                        //"webservice_authorize," +
                        //"static_guid_in_print," +
                        "printing_using_libraries," +
                        "fn_serial_port," +
                        "scale_serial_port," +
                        "get_weight_automatically,"+
                        "variant_connect_fn,"+
                        "fn_ipaddr,"+
                        "acquiring_bank,"+
                       // "do_not_prompt_marking_code,"+
                        "constant_conversion_to_kilograms,"+
                        "nds_ip,"+
                        "ip_adress_local_ch_z) VALUES(" +
                        cash_desk_number.Text + ",'" +
                        nick_shop.Text + "'," +
                        //get_use_debug() + ",'" +
                        path_for_web_service.Text + "','" +
                       // currency.Text + "','" +
                        unloading_period.Text + "','" +
                        txtB_last_date_download_bonus_clients.Text + "','" +
                        //print_m + "','" +
                        comboBox_system_taxation.SelectedIndex.ToString() + "','" +
                        //txtB_work_schema.Text + "','" +
                        txtB_version_fn.Text + "','" +
                        //enable_stock_processing_in_memory+"','"+
                        txtB_id_acquiring_terminal.Text.ToString() + "','" +
                        txtB_ip_address_acquiring_terminal.ToString().Trim() + "','" +
                        //self_service_kiosk+","+
                        //enable_cdn_markers + "," +
                        //version2_marking+","+
                        //webservice_authorize + "," +
                        //static_guid_in_print+","+
                        printing_using_libraries + ",'" +
                        fn_serial_port+"','"+      //comboBox_fn_port.SelectedItem.ToString() + "','" +
                        scale_serial_port + "'," +
                        get_weight_automatically + ","+
                        variant_connect_fn+",'"+
                        fn_ipaddr+"',"+
                        comboBox_acquiring_bank.SelectedIndex.ToString()+","+
                        //do_not_prompt_marking_code +","+
                        txtB_constant_conversion_to_kilograms.Text.Trim()+","+
                        nds_ip+",'"+
                        txtB_ip_addr_lm_ch_z.Text+"'";

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
            if (comboBox_system_taxation.SelectedIndex == 1)
            {
                //checkBox_version2_marking.Checked = true;
                //checkBox_enable_stock_processing_in_memory.Checked = true;
                //txtB_version_fn.Text = "2";
                if (comboBox_nds_ip.SelectedIndex != 0)
                {
                    MessageBox.Show("Ставка НДС для налогообложения по ип будет установлена в значение Без НДС.");
                    comboBox_nds_ip.SelectedIndex = 0;
                }
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





        public void PrintCouponCorrectly(string filePath)
        {
            using (var original = new Bitmap(filePath))
            {
                // 1. Масштабируем с сохранением пропорций
                var scaled = ScaleImage(original, 384);

                // 2. Конвертируем в монохромный буфер
                byte[] pixelBuffer = ConvertCouponToBuffer(scaled);

                // 3. Настраиваем печать
                IFptr fptr = MainStaticClass.FPTR;
                if (!fptr.isOpened()) fptr.open();

                fptr.setParam(AtolConstants.LIBFPTR_PARAM_PIXEL_BUFFER, pixelBuffer);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_WIDTH, 384);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_HEIGHT, scaled.Height);
                fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);

                fptr.printPixelBuffer();
            }
        }


        private byte[] ConvertToPrinterFormat(Bitmap bitmap)
        {
            // Размер буфера: количество пикселей / 8 (округляем вверх)
            int bufferSize = (bitmap.Width * bitmap.Height + 7) / 8;
            byte[] buffer = new byte[bufferSize];

            // Используем LockBits для быстрого доступа
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                int bytesPerPixel = 4; // Format32bppArgb
                byte[] pixelData = new byte[bmpData.Stride * bmpData.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelData, 0, pixelData.Length);

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        // Получаем цвет пикселя
                        int index = y * bmpData.Stride + x * bytesPerPixel;
                        byte r = pixelData[index + 2];
                        byte g = pixelData[index + 1];
                        byte b = pixelData[index];

                        // Преобразуем в черно-белое
                        bool isBlack = (r * 0.299 + g * 0.587 + b * 0.114) < 128;

                        if (isBlack)
                        {
                            // Упаковываем пиксели построчно
                            int pixelIndex = y * bitmap.Width + x;
                            int bytePos = pixelIndex / 8;
                            int bitPos = 7 - (pixelIndex % 8);
                            buffer[bytePos] |= (byte)(1 << bitPos);
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return buffer;
        }

        private Bitmap ScaleImage(Bitmap original, int targetWidth)
        {
            // Сохраняем пропорции
            double ratio = (double)original.Height / original.Width;
            int targetHeight = (int)(targetWidth * ratio);

            var scaled = new Bitmap(targetWidth, targetHeight);

            using (var graphics = Graphics.FromImage(scaled))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(original, 0, 0, targetWidth, targetHeight);
            }

            return scaled;
        }

        private byte[] ConvertToMonochromeSafe(Bitmap bitmap)
        {
            // Высота должна быть кратна 8 для правильного выравнивания
            int height = (bitmap.Height % 8 == 0) ? bitmap.Height : ((bitmap.Height / 8) + 1) * 8;
            byte[] buffer = new byte[(384 * height) / 8];

            // Используем LockBits без unsafe
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                // Копируем данные в массив
                byte[] pixelData = new byte[bitmapData.Stride * bitmapData.Height];
                System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, pixelData, 0, pixelData.Length);

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < 384; x++)
                    {
                        int index = y * bitmapData.Stride + x * 4;
                        byte r = pixelData[index + 2];
                        byte g = pixelData[index + 1];
                        byte b = pixelData[index];

                        bool isBlack = (r * 0.299 + g * 0.587 + b * 0.114) < 100;

                        if (isBlack)
                        {
                            // Правильная упаковка: 8 вертикальных пикселей в один байт
                            int bytePos = (x * height + y) / 8;
                            int bitPos = y % 8;
                            buffer[bytePos] |= (byte)(1 << bitPos);
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return buffer;
        }

        private byte[] ConvertCouponToBuffer(Bitmap bitmap)
        {
            // Размер буфера: (ширина * высота + 7) / 8
            int bufferSize = (384 * bitmap.Height + 7) / 8;
            byte[] buffer = new byte[bufferSize];

            using (var tempBitmap = new Bitmap(bitmap))
            {
                // Улучшаем качество текста
                tempBitmap.SetResolution(203, 203); // 203 DPI - стандарт для чеков

                for (int y = 0; y < tempBitmap.Height; y++)
                {
                    for (int x = 0; x < 384; x++)
                    {
                        Color pixel = tempBitmap.GetPixel(x, y);

                        // Улучшенное преобразование в черно-белое
                        bool isBlack = (pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114) < 150;

                        if (isBlack)
                        {
                            // Правильная упаковка пикселей для принтера
                            int bytePos = (y * 384 + x) / 8;
                            int bitPos = 7 - (x % 8);
                            buffer[bytePos] |= (byte)(1 << bitPos);
                        }
                    }
                }
            }

            return buffer;
        }


        private void button1_Click(object sender, EventArgs e)
        { 

            //PrintCouponCorrectly(Application.StartupPath + "\\Pictures2\\temp_picture.png");
            //PrintCoupon();

            //var imageBytes = System.IO.File.ReadAllBytes(Application.StartupPath + "\\Pictures2\\temp_picture.png");
            //byte[] pixelBuffer = ConvertToPixelBuffer(Application.StartupPath + "\\Pictures2\\temp_picture.png");
            //PrintPngCorrectly(Application.StartupPath + "\\Pictures2\\temp_picture.png");

            //// Настройка драйвера
            IFptr fptr = MainStaticClass.FPTR;
            if (!fptr.isOpened())
            {
                fptr.open();
            }

            fptr.setParam(AtolConstants.LIBFPTR_PARAM_FILENAME, Application.StartupPath + "\\Pictures2\\temp_picture.png");
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_SCALE_PERCENT, 50.0);
            fptr.uploadPictureMemory();

            uint pictureNumber = fptr.getParamInt(AtolConstants.LIBFPTR_PARAM_PICTURE_NUMBER);
            MessageBox.Show(pictureNumber.ToString());

            fptr.setParam(AtolConstants.LIBFPTR_PARAM_PICTURE_NUMBER, pictureNumber);
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
            fptr.printPictureByNumber();

            fptr.clearPictures();

            return;
            


            //// Критически важные параметры:
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_PIXEL_BUFFER, pixelBuffer);
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_WIDTH, 200);
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_DATA_TYPE, 0); // RAW-данные
            ////fptr.setParam(AtolConstants.LIBFPTR_PARAM_PIXEL_BUFFER_LINE_SIZE, (printerWidth + 7) / 8);

            //// Печать
            //fptr.printPixelBuffer();


            string hex_string = "";

            using (NpgsqlConnection conn = MainStaticClass.NpgsqlConn())
            {
                try
                {
                    conn.Open();
                    //foreach (var item in numDocsAction)
                    //{
                    string query = "SELECT picture	FROM public.action_header WHERE num_doc=65434";
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    hex_string = command.ExecuteScalar().ToString();
                    //hex_string = hex_string.Substring(2, hex_string.Length - 2);
                    //print_picture(hex_string);
                    //}
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show($"Ошибка базы данных: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Общие ошибки
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
            }

            //IFptr fptr = MainStaticClass.FPTR;
            if (!fptr.isOpened())
            {
                fptr.open();
            }

            
            byte[] byteArray = Convert.FromBase64String(hex_string);         




            fptr.setParam(AtolConstants.LIBFPTR_PARAM_ALIGNMENT, AtolConstants.LIBFPTR_ALIGNMENT_CENTER);
            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_SCALE_PERCENT, 150);

            // Преобразование шестнадцатеричной строки в массив байтов
            //byte[] byteArray = HexStringToByteArray(hex_string);
            //byte[] byteArray = Convert.FromBase64String(hex_string);
            MessageBox.Show(byteArray.Length.ToString());

            // Запись массива байтов в новый файл
            //string outputFilePath = Application.StartupPath + "temp_picture.png";
            string outputFilePath = Application.StartupPath + "\\Pictures2\\temp_picture.png";
            System.IO.File.WriteAllBytes(outputFilePath, byteArray);

            //fptr.setParam(AtolConstants.LIBFPTR_PARAM_FILENAME, "C:\\2025-05-13_10-30.png");
            fptr.setParam(AtolConstants.LIBFPTR_PARAM_FILENAME, outputFilePath);
            fptr.printPicture();
            if (fptr.errorCode() != 0)
            {
                MessageBox.Show("При выводе картинки на печать произошла ошибка  " + fptr.errorDescription());
            }
        }


        public class BlockedCis
        {
            public int serverDocCount { get; set; }
            public int localDocCount { get; set; }
        }

        public class BlockedGtin
        {
            public int serverDocCount { get; set; }
            public int localDocCount { get; set; }
        }

        public class ReplicationStatus
        {
            public BlockedGtin blocked_gtin { get; set; }
            public BlockedCis blocked_cis { get; set; }
        }

        public class StatusLmChZ
        {
            public string serviceUrl { get; set; }
            public bool requiresDownload { get; set; }
            public ReplicationStatus replicationStatus { get; set; }
            public string operationMode { get; set; }
            public long lastUpdate { get; set; }
            public long lastSync { get; set; }
            public string inn { get; set; }
            public string dbVersion { get; set; }
            public bool isGreyGtin { get; set; }
            public string inst { get; set; }
            public string version { get; set; }
            public string status { get; set; }
            public string name { get; set; }
        }


        public bool check_lm_ch_z()
        {
            bool result_check = false;
            //AnswerCheckMark answer_check_mark = null;
            //string error_description = "";
            // Параметры
            //string server = "192.168.2.50";
            //mark_str = process_marking_code(mark_str);
            string server = txtB_ip_addr_lm_ch_z.Text;// MainStaticClass.GetIpAddrLmChZ;// "192.168.2.50";
            int port = 5995;
            //string cis = mark_str;//"0104640043469202215Y1a67"; // ваш КИ
            string username = "admin";
            string password = "admin";
            //string xClientId = MainStaticClass.FiscalDriveNumber;//."8710000100123456"; // опционально
                                                                 //string xClientId = MainStaticClass.CDN_Token;// "8710000100123456"; // опционально


            // URL-кодирование КИ (RFC 3986)
            //string encodedCis = Uri.EscapeDataString(cis);
            string url = $"http://{server}:{port}/api/v1/status";

            // Создание запроса
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Accept = "application/json";

            // Basic Auth
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            request.Headers["Authorization"] = $"Basic {credentials}";

            // Опциональный заголовок
            //if (!string.IsNullOrEmpty(xClientId))
            //{
            //    request.Headers["X-ClientId"] = xClientId;
            //}

            try
            {
                // Синхронный вызов — поток заблокируется до получения ответа
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseBody = reader.ReadToEnd();                    
                    StatusLmChZ statusLm = JsonConvert.DeserializeObject<StatusLmChZ>(responseBody);
                    MessageBox.Show("Статус сервиса: " + statusLm.status.ToUpper(),"Проверка статуса ЛМ ЧЗ");
                    
                }
            }
            catch (WebException ex)
            {
                // Обработка ошибок (404, 500, таймаут и т.д.)
                if (ex.Response is HttpWebResponse errorResponse)
                {
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        MessageBox.Show($"Ошибка HTTP при проверке статуса лм чз {(int)errorResponse.StatusCode}: {errorText}");
                    }
                }
                else
                {
                    MessageBox.Show($"Сетевая ошибка при проверке статуса лм чз: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Неизвестная ошибка при проверке статуса лм чз: {ex.Message}");
            }

            return result_check;
        }

        private void btт_status_Click(object sender, EventArgs e)
        {
            try
            {
                check_lm_ch_z();
            }
            catch (Exception ex)
            {
                MessageBox.Show("При проверке статуса лм чз произошла ошибка " + ex.Message);
            }
        }
    }
}
