using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;

namespace Cash8
{
    public partial class CheckActions : Form
    {
        DataTable dt1 = null;
        DataTable dt2 = null;

        public CheckActions()
        {
            InitializeComponent();
            this.txtB_input_code_or_barcode.KeyPress += TxtB_input_code_or_barcode_KeyPress;
            dt1 = create_dt();
            dt2 = create_dt();
            this.Load += CheckActions_Load;            
        }

        private void CheckActions_Load(object sender, EventArgs e)
        {
            this.dataGridView_tovar.DataSource = dt1;
            this.dataGridView_tovar.DataSource = dt2;
        }

        public DataTable create_dt()
        {

            DataTable dt = new DataTable();
            DataColumn tovar_code = new DataColumn();
            tovar_code.DataType = System.Type.GetType("System.Double");
            tovar_code.ColumnName = "tovar_code"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);           
            dt.Columns.Add(tovar_code);

            DataColumn tovar_name = new DataColumn();
            tovar_name.DataType = System.Type.GetType("System.String");
            tovar_name.ColumnName = "tovar_name"; //listView1.Columns.Add("Товар", 400, HorizontalAlignment.Left);
            dt.Columns.Add(tovar_name);

            DataColumn characteristic_code = new DataColumn();
            characteristic_code.DataType = System.Type.GetType("System.String");
            characteristic_code.ColumnName = "characteristic_code"; //listView1.Columns.Add("Характеристика", 400, HorizontalAlignment.Left);
            dt.Columns.Add(characteristic_code);

            DataColumn characteristic_name = new DataColumn();
            characteristic_name.DataType = System.Type.GetType("System.String");
            characteristic_name.ColumnName = "characteristic_name"; //listView1.Columns.Add("Характеристика", 400, HorizontalAlignment.Left);
            dt.Columns.Add(characteristic_name);

            DataColumn quantity = new DataColumn();
            quantity.DataType = System.Type.GetType("System.Decimal");
            quantity.ColumnName = "quantity"; //listView1.Columns.Add("Количество", 50, HorizontalAlignment.Right);
            dt.Columns.Add(quantity);

            DataColumn price = new DataColumn();
            price.DataType = System.Type.GetType("System.Decimal");
            price.ColumnName = "price"; //listView1.Columns.Add("Цена", 50, HorizontalAlignment.Right);
            dt.Columns.Add(price);

            DataColumn price_at_discount = new DataColumn();
            price_at_discount.DataType = System.Type.GetType("System.Decimal");
            price_at_discount.ColumnName = "price_at_discount"; //listView1.Columns.Add("Цена со скидкой", 50, HorizontalAlignment.Right);
            dt.Columns.Add(price_at_discount);

            DataColumn sum_full = new DataColumn();
            sum_full.DataType = System.Type.GetType("System.Decimal");
            sum_full.ColumnName = "sum_full"; //listView1.Columns.Add("Сумма", 50, HorizontalAlignment.Right);
            dt.Columns.Add(sum_full);

            DataColumn sum_at_discount = new DataColumn();
            sum_at_discount.DataType = System.Type.GetType("System.Decimal");
            sum_at_discount.ColumnName = "sum_at_discount"; //listView1.Columns.Add("Сумма со скидкой", 50, HorizontalAlignment.Right);
            dt.Columns.Add(sum_at_discount);

            DataColumn action = new DataColumn();
            action.DataType = System.Type.GetType("System.Int32");
            action.ColumnName = "action"; //listView1.Columns.Add("Акция", 50, HorizontalAlignment.Right);
            dt.Columns.Add(action);

            DataColumn gift = new DataColumn();
            gift.DataType = System.Type.GetType("System.Int32");
            gift.ColumnName = "gift"; //listView1.Columns.Add("Подарок", 50, HorizontalAlignment.Right);
            dt.Columns.Add(gift);

            DataColumn action2 = new DataColumn();
            action2.DataType = System.Type.GetType("System.Int32");
            action2.ColumnName = "action2"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            dt.Columns.Add(action2);

            DataColumn bonus_reg = new DataColumn();
            bonus_reg.DataType = System.Type.GetType("System.Int32");
            bonus_reg.ColumnName = "bonus_reg"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            dt.Columns.Add(bonus_reg);

            DataColumn bonus_action = new DataColumn();
            bonus_action.DataType = System.Type.GetType("System.Int32");
            bonus_action.ColumnName = "bonus_action"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            dt.Columns.Add(bonus_action);

            DataColumn bonus_action_b = new DataColumn();
            bonus_action_b.DataType = System.Type.GetType("System.Int32");
            bonus_action_b.ColumnName = "bonus_action_b"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            dt.Columns.Add(bonus_action_b);

            DataColumn marking = new DataColumn();
            marking.DataType = System.Type.GetType("System.String");
            marking.ColumnName = "marking"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            dt.Columns.Add(marking);


            //foreach (ListViewItem lvi in listView1.Items)
            //{
            //    DataRow row = dt.NewRow();
            //    row["tovar_code"] = lvi.SubItems[0].Text;
            //    row["tovar_name"] = lvi.SubItems[1].Text;
            //    row["characteristic_code"] = lvi.SubItems[2].Tag.ToString();
            //    row["characteristic_name"] = lvi.SubItems[2].Text;
            //    row["quantity"] = lvi.SubItems[3].Text;
            //    row["price"] = lvi.SubItems[4].Text;
            //    row["price_at_discount"] = lvi.SubItems[5].Text;
            //    row["sum_full"] = lvi.SubItems[6].Text;
            //    row["sum_at_discount"] = lvi.SubItems[7].Text;
            //    row["action"] = lvi.SubItems[8].Text;
            //    row["gift"] = lvi.SubItems[9].Text;
            //    row["action2"] = lvi.SubItems[10].Text;
            //    row["bonus_reg"] = lvi.SubItems[11].Text;
            //    row["bonus_action"] = lvi.SubItems[12].Text;
            //    row["bonus_action_b"] = lvi.SubItems[13].Text;
            //    row["marking"] = lvi.SubItems[14].Text;

            //    dt.Rows.Add(row);
            //}

            return dt;
        }


        /*Поиск товара по штрихкоду
        * и добвление его в табличную часть
        * стандартное добавление товара
        */
        public void find_barcode_or_code_in_tovar(string barcode)
        {                   

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                
                if (barcode.Length > 6)
                {
                    //command.CommandText = "select tovar.code,tovar.name,tovar.retail_price from  barcode left join tovar ON barcode.tovar_code=tovar.code where barcode='" + inputbarcode.Text + "' ";
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price,characteristic.name,characteristic.guid,characteristic.retail_price_characteristic,tovar.its_certificate,tovar.its_marked,tovar.cdn_check,tovar.fractional " +
                        " from  barcode left join tovar ON barcode.tovar_code=tovar.code " +
                    " left join characteristic ON tovar.code = characteristic.tovar_code " +
                    " where barcode='" + barcode + "' AND its_deleted=0  AND (retail_price<>0 OR characteristic.retail_price_characteristic<>0)";
                }
                else
                {                    
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price, characteristic.name,characteristic.guid,characteristic.retail_price_characteristic,tovar.its_certificate,tovar.its_marked,tovar.cdn_check,tovar.fractional " +
                        " FROM tovar left join characteristic  ON tovar.code = characteristic.tovar_code where tovar.its_deleted=0 AND tovar.its_certificate=0 AND  (retail_price<>0 OR characteristic.retail_price_characteristic<>0) " +
                        " AND tovar.code='" + barcode + "'";
                }

                //    int its_certificate = 0;
                //    int its_marked = 0;
                    NpgsqlDataReader reader = command.ExecuteReader();
                
                //    bool find_sertificate = false;
                
                    bool fractional = false;
                


                    while (reader.Read())
                    {
                //        cdn_check = Convert.ToBoolean(reader["cdn_check"]);
                        fractional = Convert.ToBoolean(reader["fractional"]);
                //        //Сначала добавляем в предварительный список listView2 для того чтобы дать выбор если таких товаров будет несколько
                //        //ListViewItem lvi = new ListViewItem(reader[1].ToString());

                //        //if (reader[3].ToString().Trim() == "3")
                //        //{
                //        //    its_bonus_card = true;
                //        //}
                //        ListViewItem lvi = new ListViewItem(reader[3].ToString().Trim());//Внутренний код товара
                //        //lvi.Tag = reader.GetInt32(0);//Внутренний код товара
                //        //lvi.SubItems.Add(reader[1].ToString().Trim());//Наименование
                //        select_tovar.Text = reader[1].ToString().Trim();
                //        select_tovar.Tag = reader.GetInt64(0).ToString();
                //        //tovar_code = reader.GetInt64(0).ToString();

                //        //lvi.SubItems.Add(reader[3].ToString().Trim());//Характеристика
                //        lvi.Tag = reader[4].ToString().Trim();//GUID характеристики

                //        lvi.SubItems.Add(reader.GetDecimal(2).ToString());//Цена
                //        if (reader[5].ToString() != "")
                //        {
                //            lvi.SubItems[1].Text = reader.GetDecimal(5).ToString();
                //        }
                //        its_certificate = Convert.ToInt16(reader["its_certificate"]);
                //        //its_marked = reader["its_marked"].ToString().Length > 13 ? 1 : 0;
                //        its_marked = Convert.ToInt16(reader["its_marked"]);
                //        listView2.Items.Add(lvi);

                //        //Надо проверить может уже сертификат есть в чеке      
                //        if (its_certificate == 1)
                //        {
                //            foreach (ListViewItem _lvi_ in listView1.Items)
                //            {
                //                //if (_lvi_.SubItems[0].Text == reader.GetInt64(0).ToString())
                //                if (_lvi_.SubItems[14].Text == barcode)
                //                {
                //                    find_sertificate = true;
                //                    break;
                //                }
                //            }
                //        }
                //        //КОНЕЦ Надо проверить может уже сертификат есть в чеке                    
                //    }

                //    if (find_sertificate)
                //    {
                //        MessageBox.Show("Этот сертификат уже добавлен в чек");
                //        return;
                //    }

                //    //if (its_bonus_card)
                //    //{
                //    //    MessageBox.Show("Этот бонусная карта и добавлена в чек может быть только по нажатию на F9 ");
                //    //    return;
                //    //}

                //    //Проверка по сертификату
                //    if (its_certificate == 1)
                //    {
                //        if (!check_sertificate_for_sales(barcode))
                //        {
                //            return;
                //        }
                //        Cash8.DS.DS ds = MainStaticClass.get_ds();
                //        ds.Timeout = 60000;
                //        //Получить параметр для запроса на сервер 
                //        string nick_shop = MainStaticClass.Nick_Shop.Trim();
                //        if (nick_shop.Trim().Length == 0)
                //        {
                //            MessageBox.Show(" Не удалось получить название магазина ");
                //            return;
                //        }
                //        string code_shop = MainStaticClass.Code_Shop.Trim();
                //        if (code_shop.Trim().Length == 0)
                //        {
                //            MessageBox.Show(" Не удалось получить код магазина ");
                //            return;
                //        }
                //        string count_day = CryptorEngine.get_count_day();
                //        string key = nick_shop.Trim() + count_day.Trim() + code_shop.Trim();
                //        //string sertificate_code = get_tovar_code(barcode);
                //        string sertificate_code = barcode;
                //        string encrypt_data = CryptorEngine.Encrypt(sertificate_code, true, key);
                //        string status = "";
                //        try
                //        {
                //            status = ds.GetStatusSertificat(MainStaticClass.Nick_Shop, encrypt_data, MainStaticClass.GetWorkSchema.ToString());
                //        }
                //        catch (Exception ex)
                //        {
                //            MessageBox.Show(" Произошли ошибки при работе с сертификатами " + ex.Message);
                //            return;
                //        }
                //        if (status == "-1")
                //        {
                //            MessageBox.Show("Произошли ошибки при работе с сертификатами");
                //            return;
                //        }
                //        else
                //        {
                //            string decrypt_data = CryptorEngine.Decrypt(status, true, key);
                //            if (decrypt_data == "1")
                //            {
                //                MessageBox.Show("Сертификат уже активирован");
                //                return;
                //            }
                //        }

                //    }
                //    else if (its_certificate == 2)//Это продажа бонусной карты проверяем, что в шапке нет другой карты
                //    {
                //        if (check_availability_card_sale())
                //        {
                //            MessageBox.Show("В строках чека уже есть бонусная карта на продажу");
                //            return;
                //        }
                //        //if (get_status_promo_card(barcode) != 1)
                //        //{
                //        //    MessageBox.Show("Данная бонусная карта уже активирована и повторно продана быть не может");
                //        //    return;
                //        //}
                //        if (client.Tag != null)
                //        {
                //            if (client.Tag.ToString() != barcode)
                //            {
                //                MessageBox.Show("В чеке уже выбран клиент с другой бонусной2 картой, продажа бонусной карты в этом чеке невозможна");
                //                return;
                //            }
                //            if (card_state != 1)//Проверяем статус карты он должен быть 1 т.е. не активирована
                //            {
                //                MessageBox.Show("Эта карта имеет неверный статус в процессиноговом центре и продана быть не может ");
                //                return;
                //            }
                //        }
                //    }

                //    //Подсчет суммы по документу
                //    if (listView2.Items.Count == 1)//1 товар найден
                //    {
                //        ListViewItem lvi = null;
                //        if ((its_marked == 0) && (its_certificate == 0) && ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3)))
                //        {
                //            lvi = exist_tovar_in_listView(listView1, Convert.ToInt64(select_tovar.Tag), listView2.Items[0].Tag);
                //        }
                //        if (lvi == null)
                //        {
                //            //select_tovar.Tag.ToString()
                //            lvi = new ListViewItem(select_tovar.Tag.ToString());
                //            lvi.Tag = select_tovar.Tag.ToString();
                //            lvi.SubItems.Add(select_tovar.Text);//Наименование
                //            lvi.SubItems.Add(listView2.Items[0].Text);//Характеиристика                         

                //            if (listView2.Items[0].Tag == null)
                //            {
                //                lvi.SubItems[2].Tag = "";  // listView2.Items[0].Tag;//GUID характеристики   
                //            }
                //            else
                //            {
                //                lvi.SubItems[2].Tag = listView2.Items[0].Tag;//GUID характеристики   
                //            }

                //            lvi.SubItems.Add("1");
                //            lvi.SubItems.Add(listView2.Items[0].SubItems[1].Text);//Цена 
                //                                                                  //Проверка на сертификат               
                //            if (this.its_certificate(select_tovar.Tag.ToString()) != "1")
                //            {
                //                lvi.SubItems.Add(Math.Round(Convert.ToDouble(lvi.SubItems[4].Text) - Convert.ToDouble(lvi.SubItems[4].Text) * Discount, 2).ToString());//Цена со скидкой
                //            }
                //            else
                //            {
                //                lvi.SubItems.Add(Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text), 2).ToString());//Цена со скидкой 
                //            }
                //            lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма
                //            lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString()); //Сумма со скидкой                        
                //            lvi.SubItems.Add("0"); //Номер акционного документа скидка
                //            lvi.SubItems.Add("0"); //Номер акционного документа подарок
                //            lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть
                //            lvi.SubItems.Add("0");//Бонус
                //            lvi.SubItems.Add("0");//Бонус1
                //            lvi.SubItems.Add("0");//Бонус2
                //            if (its_certificate == 0)
                //            {
                //                lvi.SubItems.Add("0");//Маркер
                //            }
                //            else if (its_certificate == 1)
                //            {
                //                lvi.SubItems.Add(barcode);//Это сертификат и при продаже мы добавляем его штрихкод 
                //            }
                //            //listView1.Items.Add(lvi);
                //            //listView1.Select();
                //            //listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                //            //update_record_last_tovar(listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text, listView1.Items[this.listView1.Items.Count - 1].SubItems[3].Text);
                //            bool error = false;
                //            int code_marking_error = 0;
                //            bool cdn_vrifyed = false;
                //            string mark_str = "";
                //            //bool its_marked = (check_sign_marker_code(select_tovar.Tag.ToString()) > 0);
                //            if (its_marked > 0)
                //            {
                //                if (!Console.CapsLock)
                //                {
                //                    if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
                //                    {
                //                        this.qr_code = "";
                //                        Input_action_barcode input_Action_Barcode = new Input_action_barcode();
                //                        input_Action_Barcode.call_type = 6;
                //                        input_Action_Barcode.caller = this;
                //                        if (DialogResult.Cancel == input_Action_Barcode.ShowDialog())
                //                        {
                //                            error = true;
                //                        }
                //                        if (this.qr_code != "")//Был введен qr код необходимо его внести в чек
                //                        {

                //                            if (this.qr_code.ToUpper().Substring(0, 4).IndexOf("HTTP") != -1)
                //                            {
                //                                error = true;
                //                                MessageBox.Show("Считан не верный qr код");
                //                                MainStaticClass.write_event_in_log("HTTP не верный qr код  " + barcode, "Документ чек", numdoc.ToString());
                //                                this.qr_code = "";

                //                            }
                //                            else
                //                            {
                //                                //здесь проверка на известные ошибки \u001d                                        
                //                                int num_pos = this.qr_code.IndexOf("\\");
                //                                if (num_pos > 0)
                //                                {
                //                                    if (this.qr_code.Substring(num_pos + 1, 5) == "u001d")//необходимо из строки вырезать этот символ
                //                                    {
                //                                        this.qr_code = this.qr_code.Substring(0, num_pos) + this.qr_code.Substring(num_pos + 1 + 5, this.qr_code.Length - (num_pos + 1 + 5));
                //                                        num_pos = this.qr_code.IndexOf("\\");
                //                                        if (num_pos > 0)
                //                                        {
                //                                            this.qr_code = this.qr_code.Substring(0, num_pos) + this.qr_code.Substring(num_pos + 1 + 5, this.qr_code.Length - (num_pos + 1 + 5));
                //                                        }
                //                                    }
                //                                }

                //                                if (this.qr_code != "")
                //                                {
                //                                    if (this.qr_code.ToUpper().Substring(0, 4).IndexOf("HTTP") != -1)
                //                                    {
                //                                        error = true;
                //                                        MessageBox.Show("Считан не верный код маркировки");
                //                                        MainStaticClass.write_event_in_log("HTTP не верный код маркировки  " + this.qr_code, "Документ чек", numdoc.ToString());
                //                                    }
                //                                }

                //                                //перед тем как добавить qr код в чек необходимо его проверить
                //                                //string mark_str = "";
                //                                string mark_str_cdn = "";
                //                                if (this.qr_code.Trim().Length > 13)
                //                                {
                //                                    if (!qr_code_lenght.Contains(this.qr_code.Trim().Length))
                //                                    {
                //                                        MessageBox.Show(qr_code + "\r\n Ваш код маркировки имеет длину " + qr_code.Length.ToString() + " символов при этом он не входит в допустимый диапазок ", "Проверка qr-кода на допустимую длину");
                //                                        //MessageBox.Show("Длина введенного qr-кода не входит в диапазон допустимых ! \r\n ТОВАР В ЧЕК ДОБАВЛЕНЕ НЕ БУДЕТ!!! ", "Проверки введенного qr кода");
                //                                        MainStaticClass.write_event_in_log("Длина введенного кода маркировки не входит в диапазон допустимых, он имеет длину " + qr_code.Length.ToString(), "Документ чек", numdoc.ToString());
                //                                        error = true;
                //                                        return;
                //                                    }
                //                                }
                //                                if (MainStaticClass.Version2Marking == 1)
                //                                {
                //                                    WortWithMarkingV3 markingV3 = new WortWithMarkingV3();
                //                                    mark_str = this.qr_code.Trim();
                //                                    mark_str = add_gs1(mark_str);
                //                                    bool result_check_cdn = false;
                //                                    bool timeout_check_cdn = false;//таймаут при проверке по cdn

                //                                    //*******************************************************************************************************************************
                //                                    foreach (ListViewItem listViewItem4 in this.listView1.Items)
                //                                    {
                //                                        if (listViewItem4.SubItems[14].Text == mark_str)
                //                                        {
                //                                            MessageBox.Show("Номенклатура с введенным кодом маркировки который вы пытались добавить уже существует в чеке. \r\n Номенклатура не будет добавлена.");
                //                                            error = true;
                //                                            break;
                //                                        }
                //                                    }
                //                                    if (cdn_check)
                //                                    {
                //                                        if (MainStaticClass.CashDeskNumber != 9 && MainStaticClass.EnableCdnMarkers == 1)
                //                                        {
                //                                            if (MainStaticClass.CDN_Token == "")
                //                                            {
                //                                                MessageBox.Show("В этой кассе не заполнен CDN токен, \r\n ПРОДАЖА ДАННОГО ТОВАРА НЕВОЗМОЖНА ! ", "Проверка CDN");
                //                                                return;
                //                                            }
                //                                            CDN cdn = new CDN();
                //                                            List<string> codes = new List<string>();
                //                                            mark_str_cdn = mark_str.Replace("\u001d", @"\u001d");
                //                                            codes.Add(mark_str_cdn);
                //                                            mark_str_cdn = mark_str_cdn.Replace("'", "\'");
                //                                            Dictionary<string, string> d_tovar = new Dictionary<string, string>();
                //                                            d_tovar[lvi.SubItems[1].Text] = lvi.SubItems[0].Text;
                //                                            result_check_cdn = cdn.check_marker_code(codes, mark_str, this.numdoc, ref request, mark_str_cdn, d_tovar, ref timeout_check_cdn);
                //                                            if ((!result_check_cdn) && (!timeout_check_cdn))//не прошел проверку и таймаута не было 
                //                                            {
                //                                                return;
                //                                            }
                //                                            else
                //                                            {
                //                                                cdn_vrifyed = true;
                //                                            }
                //                                        }
                //                                    }
                //                                    // && MainStaticClass.PrintingUsingLibraries==0
                //                                    //if (!cdn_vrifyed)
                //                                    //{
                //                                    byte[] textAsBytes = Encoding.Default.GetBytes(mark_str);
                //                                    string imc = Convert.ToBase64String(textAsBytes);

                //                                    if (MainStaticClass.PrintingUsingLibraries == 0)
                //                                    {
                //                                        if (!cdn_vrifyed)
                //                                        {
                //                                            WortWithMarkingV3.Root root = markingV3.beginMarkingCodeValidation("auto", imc, "itemPieceSold", 1, "piece", 0, false);
                //                                            if (root.results[0].errorCode != 0)
                //                                            {
                //                                                markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                //                                                code_marking_error = root.results[0].errorCode;
                //                                                if ((code_marking_error != 421) && (code_marking_error != 402))
                //                                                {
                //                                                    MainStaticClass.write_event_in_log("beginMarkingCodeValidation " + root.results[0].errorDescription + " " + code_marking_error, "Документ", numdoc.ToString());
                //                                                    error = true;//не прошли проверку товар не добавляем в чек
                //                                                }
                //                                            }
                //                                            else
                //                                            {
                //                                                root = markingV3.getMarkingCodeValidationStatus();
                //                                                if (root.results[0].result != null)
                //                                                {
                //                                                    if (root.results[0].result.driverError != null)
                //                                                    {
                //                                                        if (root.results[0].result.driverError.code != 0)
                //                                                        {
                //                                                            markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                //                                                            code_marking_error = root.results[0].result.driverError.code;
                //                                                            //if (code_marking_error != 0)
                //                                                            //{
                //                                                            if ((code_marking_error != 421) && (code_marking_error != 402))
                //                                                            {
                //                                                                //markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                //                                                                MessageBox.Show("getMarkingCodeValidationStatus " + root.results[0].result.driverError.description, "Ошибка при начале проверки кода маркировки");
                //                                                                error = true;//не прошли проверку товар не добавляем в чек
                //                                                            }
                //                                                        }
                //                                                        else//проверяем статус и если все хорошо отправляем принять 
                //                                                        {
                //                                                            //if (root.results[0].result.ready)
                //                                                            //{
                //                                                            if (root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckFlag &&
                //                                                                root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckResult &&
                //                                                                root.results[0].result.onlineValidation.itemInfoCheckResult.imcStatusInfo &&
                //                                                                root.results[0].result.onlineValidation.itemInfoCheckResult.imcEstimatedStatusCorrect)
                //                                                            {
                //                                                                //Все признаки успех, значит M+
                //                                                                markingV3.acceptMarkingCode();
                //                                                                MainStaticClass.write_event_in_log("acceptMarkingCode  " + lvi.SubItems[0].Text, "Документ чек", numdoc.ToString());
                //                                                            }
                //                                                            else// сообщим детально об ошибке 
                //                                                            {
                //                                                                if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckFlag)
                //                                                                {
                //                                                                    MessageBox.Show("Код маркировки не был проверен ФН и(или) ОИСМП");
                //                                                                }
                //                                                                if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcCheckResult)
                //                                                                {
                //                                                                    MessageBox.Show("Результат проверки КП КМ отрицательный или код маркировки не был проверен");
                //                                                                }
                //                                                                if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcStatusInfo)
                //                                                                {
                //                                                                    MessageBox.Show("Сведения о статусе товара от ОИСМП не получены");
                //                                                                }
                //                                                                if (!root.results[0].result.onlineValidation.itemInfoCheckResult.imcEstimatedStatusCorrect)
                //                                                                {
                //                                                                    MessageBox.Show("От ОИСМП получены сведения, что планируемый статус товара некорректен или сведения о статусе товара от ОИСМП не получены");
                //                                                                }

                //                                                                error = true;
                //                                                                markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                //                                                            }
                //                                                        }
                //                                                    }
                //                                                    else
                //                                                    {
                //                                                        MainStaticClass.write_event_in_log("root.results[0].result.driverError == null  " + lvi.SubItems[0].Text, "Документ чек", numdoc.ToString());
                //                                                        markingV3.cancelMarkingCodeValidation();//прерываем валидацию
                //                                                        code_marking_error = 421;
                //                                                    }
                //                                                }
                //                                                else
                //                                                {
                //                                                    error = true;
                //                                                }
                //                                            }
                //                                        }
                //                                    }
                //                                    else
                //                                    {
                //                                        PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                //                                        if (!printingUsingLibraries.check_marking_code(imc, this.numdoc.ToString(), ref this.cdn_markers_result_check))
                //                                        {
                //                                            error = true;
                //                                        }
                //                                    }
                //                                    //}
                //                                }
                //                                else
                //                                {
                //                                    foreach (ListViewItem listViewItem4 in this.listView1.Items)
                //                                    {
                //                                        if (listViewItem4.SubItems[14].Text == this.qr_code)
                //                                        {
                //                                            error = true;
                //                                            MessageBox.Show("Номенклатура с введенным кодом маркировки который вы пытались добавить уже существует в чеке. \r\n Номенклатура не будет добавлена.");
                //                                            break;
                //                                        }
                //                                    }
                //                                }
                //                                if (error)
                //                                {
                //                                    return;
                //                                }
                //                                if (mark_str != "")
                //                                {
                //                                    lvi.SubItems[14].Text = mark_str;//добавим в чек qr код                                        
                //                                }
                //                                else
                //                                {
                //                                    lvi.SubItems[14].Text = this.qr_code;//добавим в чек qr код                                        
                //                                }
                //                                //}
                //                                //else
                //                                //{
                //                                //    MessageBox.Show("Введен неверный код маркировки, попробуйте еще раз. Номенклатура не будет добавлена.");
                //                                //    error = true;
                //                                //    //Не добавляем позицию в чек
                //                                //}
                //                                this.qr_code = "";//обнулим переменную
                //                            }
                //                        }
                //                    }
                //                    else
                //                    {
                //                        MessageBox.Show("У вас нажата клавиша Shift, ввод кода маркировки невозможен.Номенклатура не будет добавлена.");
                //                        //Не добавляем позицию в чек
                //                        error = true;
                //                    }
                //                }
                //                else
                //                {
                //                    MessageBox.Show("У вас нажата клавиша CapsLock, ввод кода маркировки невозможен.Номенклатура не будет добавлена.");
                //                    //Не добавляем позицию в чек
                //                    error = true;

                //                }

                //                if ((!error) || ((code_marking_error == 402) || (code_marking_error == 421) || cdn_vrifyed))//Если с qr кодом все хорошо тогда добавляем позицию иначе не добавляем 
                //                {
                //                    listView1.Items.Add(lvi);

                //                    MainStaticClass.write_event_in_log("Товар добавлен " + barcode, "Документ чек", numdoc.ToString());
                //                    if (cdn_vrifyed && MainStaticClass.PrintingUsingLibraries == 0)
                //                    {
                //                        km_adding_to_buffer_index(listView1.Items.Count - 1);
                //                        MainStaticClass.write_event_in_log("cdn_vrifyed = " + cdn_vrifyed + ", на фискальном регистраторе не проверяем ", "Документ чек", numdoc.ToString());
                //                    }
                //                }
                //                else
                //                {
                //                    MainStaticClass.write_event_in_log("Отказ от ввода qr кода, товар не добавлен", "Документ чек", numdoc.ToString());
                //                    last_tovar.Text = barcode;
                //                    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                //                    t_n_f.textBox1.Text = "Код маркировки не прошел проверку";
                //                    t_n_f.textBox1.Font = new Font("Microsoft Sans Serif", 22);
                //                    t_n_f.label1.Text = " Код ошибки code_marking_error = " + code_marking_error.ToString();
                //                    t_n_f.ShowDialog();
                //                    t_n_f.Dispose();
                //                    return;
                //                }
                //            }
                //            else
                //            {
                //                listView1.Items.Add(lvi);
                //            }

                //            if (fractional)
                //            {
                //                listView1.Focus();
                //                listView1.Select();
                //                listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                //                listView1.Items[this.listView1.Items.Count - 1].Focused = true;
                //                SendKeys.Send("Enter");

                //                //listView1.Invoke((MethodInvoker)(() => listView1_ItemActivate(listView1, new EventArgs())));
                //                show_quantity_control();
                //            }


                //            //if ((!error) || ((code_marking_error == 402) || (code_marking_error == 421) || cdn_vrifyed))//Если с qr кодом все хорошо тогда добавляем позицию иначе не добавляем 
                //            //{
                //            //    MainStaticClass.write_event_in_log("Товар добавлен " + barcode, "Документ чек", numdoc.ToString());                            
                //            //    //if ((code_marking_error == 402) || (code_marking_error == 421))//это говорит о том что включена новая схема маркировки и мы имеет проблемы с интернетом
                //            //    //{
                //            //        //km_adding_to_buffer(listView1.Items.Count - 1);//принудительно добавляем в буфер последнюю строку с маркировкой 
                //            //        MainStaticClass.write_event_in_log("cdn_vrifyed = " + cdn_vrifyed + ", на фискальном регистраторе не проверяем ", "Документ чек", numdoc.ToString());
                //            //        if (MainStaticClass.PrintingUsingLibraries == 0)
                //            //        {
                //            //            listView1.Items.Add(lvi);
                //            //            km_adding_to_buffer_index(listView1.Items.Count - 1);                                    
                //            //        }
                //            //        else
                //            //        {
                //            //            //km_adding_to_buffer
                //            //            PrintingUsingLibraries printingUsingLibraries = new PrintingUsingLibraries();
                //            //            if (!printingUsingLibraries.km_adding_to_buffer(mark_str, this.numdoc.ToString()))
                //            //            {
                //            //                error = true;
                //            //            }
                //            //            else
                //            //            {
                //            //                listView1.Items.Add(lvi);
                //            //            }
                //            //        }                                
                //            //    //}
                //            //}
                //            //else
                //            //{
                //            //    MainStaticClass.write_event_in_log("Отказ от ввода qr кода, товар не добавлен", "Документ чек", numdoc.ToString());
                //            //    last_tovar.Text = barcode;
                //            //    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                //            //    t_n_f.textBox1.Text = "Код маркировки не прошел проверку";
                //            //    t_n_f.textBox1.Font = new Font("Microsoft Sans Serif", 22);
                //            //    t_n_f.label1.Text = " Код ошибки code_marking_error = " + code_marking_error.ToString();
                //            //    t_n_f.ShowDialog();
                //            //    t_n_f.Dispose();
                //            //    return;
                //            //}
                //            SendDataToCustomerScreen(1, 0, 1);
                //            if ((MainStaticClass.GetWorkSchema == 1) || (MainStaticClass.GetWorkSchema == 3))
                //            {
                //                if (!fractional)
                //                {
                //                    listView1.Select();
                //                    listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                //                }
                //                //inputbarcode.Focus();//01.08.2022 Теперь здесь для Визы автоматом переност фокуса
                //            }
                //            else if (MainStaticClass.GetWorkSchema == 2)
                //            {
                //                //inputbarcode.Focus();
                //            }

                //            update_record_last_tovar(listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text, listView1.Items[this.listView1.Items.Count - 1].SubItems[3].Text);

                //            //if (MainStaticClass.Use_Trassir > 0)
                //            //{
                //            //    string s = MainStaticClass.get_string_message_for_trassir("POSNG_POSITION_ADD", numdoc.ToString(), MainStaticClass.Cash_Operator, DateTime.Now.Date.ToString("MM'/'dd'/'yyyy"), DateTime.Now.ToString("HH:mm:ss"), (listView1.Items.Count - 1).ToString(), "1", listView2.Items[0].SubItems[1].Text, barcode, "", MainStaticClass.CashDeskNumber.ToString(), select_tovar.Text + " "+listView2.Items[0].Text);
                //            //    MainStaticClass.send_data_trassir(s);
                //            //}                        
                //        }
                //        else
                //        {
                //            if (!fractional)
                //            {
                //                lvi.SubItems[3].Text = (Convert.ToDecimal(lvi.SubItems[3].Text) + 1).ToString();
                //                calculate_on_string(lvi);
                //                lvi.Selected = true;
                //                listView1.Select();
                //            }
                //            else
                //            {
                //                listView1.Focus();
                //                listView1.Select();
                //                listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                //                listView1.Items[this.listView1.Items.Count - 1].Focused = true;
                //                show_quantity_control();
                //            }
                //            update_record_last_tovar(lvi.SubItems[1].Text, lvi.SubItems[4].Text);
                //        }
                //        if (!fractional)
                //        {
                //            inputbarcode.Focus();
                //        }
                //        calculation_of_the_sum_of_the_document();
                //    }
                //    else if (listView2.Items.Count > 1)//Найденных товаров больше одного необходимо показать список выбра пользователю
                //    {
                //        this.panel2.Visible = true;
                //        this.panel2.BringToFront();
                //        this.listView2.Visible = true;
                //        listView2.Select();
                //        listView2.Items[0].Selected = true;
                //        listView2.Items[0].Focused = true;
                //    }
                //    else if (listView2.Items.Count == 0)
                //    {
                //        //MessageBox.Show("Не найден");
                //        //stop_com_barcode_scaner();
                //        last_tovar.Text = barcode;
                //        Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                //        t_n_f.ShowDialog();
                //        t_n_f.Dispose();
                //        //start_com_barcode_scaner();
                    }

                //    reader.Close();
                    //                 conn.Close();                
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show("find_barcode_or_code_in_tovar " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("find_barcode_or_code_in_tovar " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        // conn.Dispose();
                    }
                }            
            }

        private void TxtB_input_code_or_barcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.txtB_input_code_or_barcode.Text = this.txtB_input_code_or_barcode.Text.Replace("\r\n", "");
            if (e.KeyChar == 13)
            {
                if (this.txtB_input_code_or_barcode.Text.Length == 0)//||(this.inputbarcode.Text=="\r\n"))//тут еще проверка на минимальность символов
                {
                    //MessageBox.Show("Штрихкод не найден");
                    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                    t_n_f.ShowDialog();
                    t_n_f.Dispose();
                    return;
                }
                find_barcode_or_code_in_tovar(this.txtB_input_code_or_barcode.Text);
                //if (listView1.Items.Count > 0)
                //{
                //    btn_inpute_phone_client.Enabled = false;
                //}
                txtB_input_code_or_barcode.Text = "";              
                return;
            }

            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != (char)Keys.Back)
                {

                    e.Handled = true;
                }
            }

        }
    }
}
