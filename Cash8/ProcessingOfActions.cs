using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using Npgsql;

namespace Cash8
{
    public partial class ProcessingOfActions
    {

        public DataTable dt = new DataTable();
        //private DataTable dt_gift = new DataTable();
        public string client_code = "";
        public int action_num_doc = 0;
        public ArrayList action_barcode_list = new ArrayList();//Доступ из формы ввода акционного штрихкода 
        public bool inpun_action_barcode = false;//Доступ из формы ввода акционного штрихкода
        public bool have_action = false;
        public decimal discount = 0;

        public ProcessingOfActions()
        {

        }

        private bool actions_birthday()
        {

            bool result = false;

            if (client_code == "")
            {
                return result;
            }

            NpgsqlConnection conn = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT COUNT(*)FROM clients WHERE code='" + client_code + "' AND date_part('month',date_of_birth)=" + DateTime.Now.Date.Month +
                    " AND  date_part('day',date_of_birth) BETWEEN " + DateTime.Now.Date.AddDays(-1).Day.ToString() + " AND " + DateTime.Now.Date.AddDays(1).Day.ToString() +
                    " AND date_of_birth<>'01.01.1900'";

                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                {
                    result = true;
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail, "Акция день рождения");
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Акция день рождения");
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }

            return result;
        }


        private bool check_and_create_checks_table_temp()
        {
            bool result = true;
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {

                conn.Open();
                //string query = "select COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='checks_table_temp'";
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;

                command.CommandText = "select COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='checks_table_temp'	";
                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {
                    command.CommandText = "CREATE TABLE checks_table_temp( tovar integer)WITH (  OIDS=FALSE);ALTER TABLE checks_table_temp  OWNER TO postgres;";

                }
                else
                {
                    command.CommandText = "DROP TABLE checks_table_temp;CREATE TABLE checks_table_temp( tovar integer)WITH (  OIDS=FALSE);ALTER TABLE checks_table_temp  OWNER TO postgres;";
                }

                command.ExecuteNonQuery();

                StringBuilder sb = new StringBuilder();
                /*foreach (ListViewItem lvi in listView1.Items)
                {
                    sb.Append("INSERT INTO checks_table_temp(tovar)VALUES (" + lvi.SubItems[0].Text + ");");
                }*/
                foreach (DataRow row in dt.Rows)
                {
                    sb.Append("INSERT INTO checks_table_temp(tovar)VALUES (" + row["code"].ToString() + ");");
                }

                command.CommandText = sb.ToString();
                command.ExecuteNonQuery();

                conn.Close();

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                result = false;

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


        public DataTable create_dt(ListView listView1)
        {

            DataTable dt = new DataTable();
            DataColumn tovar_code = new DataColumn();
            tovar_code.DataType = System.Type.GetType("System.Int32");
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
            quantity.DataType = System.Type.GetType("System.Int32");
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

            foreach (ListViewItem lvi in listView1.Items)
            {
                DataRow row = dt.NewRow();
                row["tovar_code"] = lvi.SubItems[0].Text;
                row["tovar_name"] = lvi.SubItems[1].Text;
                row["characteristic_code"] = lvi.SubItems[2].Tag.ToString();
                row["characteristic_name"] = lvi.SubItems[2].Text;
                row["quantity"] = lvi.SubItems[3].Text;
                row["price"] = lvi.SubItems[4].Text;
                row["price_at_discount"] = lvi.SubItems[5].Text;
                row["sum_full"] = lvi.SubItems[6].Text;
                row["sum_at_discount"] = lvi.SubItems[7].Text;
                row["action"] = lvi.SubItems[8].Text;
                row["gift"] = lvi.SubItems[9].Text;
                row["action2"] = lvi.SubItems[10].Text;
                dt.Rows.Add(row);
            }

            return dt;
        }

        /// <summary>
        /// Создаем таблицу значений в которую помещаем данные листвью
        /// обработка акций далее будет происходить над этим новым объектом
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable[] to_process_actions(DataTable dt)
        {
            DataTable[] dt_tables = new DataTable[2];
            dt_tables[0] = dt;
            //dt_tables[1] = dt_gift;

            return dt_tables;
        }


        /// <summary>
        /// Подсчет суммы документа
        /// 
        /// </summary>
        /// <returns></returns>
        private decimal calculation_of_the_sum_of_the_document()
        {
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
            {
                total += Convert.ToDecimal(row["sum_at_discount"]);
            }
            /* foreach (DataRow row in dt_gift.Rows)
             {
                 total += Convert.ToDecimal(row["sum_at_discount"]);
             }*/
            return total;
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
                    command.CommandText = "select tovar.code AS tovar_code,tovar.name AS tovar_name,tovar.retail_price AS retail_price,characteristic.name AS characteristic_name,characteristic.guid AS characteristic_guid," +
                        " characteristic.retail_price_characteristic AS retail_price_characteristic ,tovar.its_certificate AS tovar_its_certificate" +
                        " from  barcode left join tovar ON barcode.tovar_code=tovar.code " +
                    " left join characteristic ON tovar.code = characteristic.tovar_code " +
                    " where barcode='" + barcode + "' AND its_deleted=0  AND (retail_price<>0 OR characteristic.retail_price_characteristic<>0) AND tovar.its_certificate=0";
                }
                else
                {
                    command.CommandText = "select tovar.code AS tovar_code,tovar.name AS tovar_name,tovar.retail_price AS retail_price, characteristic.name AS characteristic_name,characteristic.guid AS characteristic_guid," +
                        " characteristic.retail_price_characteristic AS retail_price_characteristic,tovar.its_certificate AS tovar_its_certificate " +
                        " FROM tovar left join characteristic  ON tovar.code = characteristic.tovar_code where tovar.its_deleted=0 AND (retail_price<>0 OR characteristic.retail_price_characteristic<>0) " +
                        " AND tovar.code='" + barcode + "' AND tovar.its_certificate=0";
                }

                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {

                    bool new_row = false;
                    DataRow[] findRow;
                    string expression = "code=" + reader[1].ToString(); // sql подобный запрос
                    findRow = dt.Select(expression);
                    DataRow row = null;
                    if (findRow.Length > 0)
                    {
                        row = findRow[0];
                    }
                    else
                    {
                        row = dt.NewRow();
                        new_row = true;
                    }

                    row["tovar_code"] = reader["tovar_code"].ToString();
                    row["tovar_name"] = reader["tovar_name"].ToString();
                    row["characteristic_code"] = reader["characteristic_guid"].ToString();
                    row["characteristic_name"] = reader["characteristic_name"].ToString();

                    row["quantity"] = "1";// lvi.SubItems[3].Text;Количество 1 без кратности
                    row["price"] = reader["retail_price"];
                    row["price_at_discount"] = reader["retail_price"]; //lvi.SubItems[5].Text; //?
                    row["sum_full"] = Convert.ToDecimal(row["sum_full"]) + Convert.ToDecimal(row["price"]) * Convert.ToDecimal(row["quantity"]); //lvi.SubItems[6].Text;
                    row["sum_at_discount"] = Convert.ToDecimal(row["sum_at_discount"]) + Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]);
                    if (new_row)
                    {
                        dt.Rows.Add(row);
                    }                  
                }


                ////Подсчет суммы по документу
                //if (listView2.Items.Count == 1)//1 товар найден
                //{
                //    //Проверим не сертификат ли это 


                //    ListViewItem lvi = exist_tovar_in_listView(listView1, Convert.ToInt32(select_tovar.Tag), listView2.Items[0].Tag);
                //    if (lvi == null)
                //    {
                //        //select_tovar.Tag.ToString()
                //        lvi = new ListViewItem(select_tovar.Tag.ToString());
                //        lvi.Tag = select_tovar.Tag.ToString();
                //        lvi.SubItems.Add(select_tovar.Text);//Наименование
                //        lvi.SubItems.Add(listView2.Items[0].Text);//Характеиристика                         

                //        if (listView2.Items[0].Tag == null)
                //        {
                //            lvi.SubItems[2].Tag = "";  // listView2.Items[0].Tag;//GUID характеристики   
                //        }
                //        else
                //        {
                //            lvi.SubItems[2].Tag = listView2.Items[0].Tag;//GUID характеристики   
                //        }

                //        lvi.SubItems.Add("1");
                //        lvi.SubItems.Add(listView2.Items[0].SubItems[1].Text);//Цена 
                //        //Проверка на сертификат               
                //        if (this.its_certificate(select_tovar.Tag.ToString()) != "1")
                //        {
                //            lvi.SubItems.Add(Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text) - Convert.ToDecimal(lvi.SubItems[4].Text) * Discount, 2).ToString());//Цена со скидкой
                //        }
                //        else
                //        {
                //            lvi.SubItems.Add(Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text), 2).ToString());//Цена со скидкой 
                //        }
                //        lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());//Сумма
                //        lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[3].Text) * Convert.ToDecimal(lvi.SubItems[5].Text)).ToString()); //Сумма со скидкой                        
                //        lvi.SubItems.Add("0"); //Номер акционного документа скидка
                //        lvi.SubItems.Add("0"); //Номер акционного документа подарок
                //        lvi.SubItems.Add("0"); //Номер акционного документа дополнительное поле пометка что участвовало в акции, но скидка может быть

                //        listView1.Items.Add(lvi);
                //        listView1.Select();
                //        listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                //        update_record_last_tovar(listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text, listView1.Items[this.listView1.Items.Count - 1].SubItems[3].Text);
                //        if (MainStaticClass.Use_Trassir > 0)
                //        {
                //            string s = MainStaticClass.get_string_message_for_trassir("POSNG_POSITION_ADD", numdoc.ToString(), MainStaticClass.Cash_Operator, DateTime.Now.Date.ToString("MM'/'dd'/'yyyy"), DateTime.Now.ToString("HH:mm:ss"), (listView1.Items.Count - 1).ToString(), "1", listView2.Items[0].SubItems[1].Text, barcode, "", MainStaticClass.CashDeskNumber.ToString(), select_tovar.Text + " " + listView2.Items[0].Text);
                //            MainStaticClass.send_data_trassir(s);
                //        }
                //    }
                //    else
                //    {
                //        lvi.SubItems[3].Text = (Convert.ToInt64(lvi.SubItems[3].Text) + 1).ToString();
                //        //lvi.SubItems[4].Text = listView2.Items[0].SubItems[2].Text;//Цена
                //        calculate_on_string(lvi);
                //        lvi.Selected = true;
                //        listView1.Select();
                //        update_record_last_tovar(lvi.SubItems[1].Text, lvi.SubItems[4].Text);

                //    }
                //    calculation_of_the_sum_of_the_document();
                //    //listView1.Select();
                //    //listView1.Items[this.listView1.Items.Count - 1].Selected = true;

                //    //this.last_tovar.Text = listView1.Items[this.listView1.Items.Count - 1].SubItems[1].Text;
                //    //this.last_cena.Text = listView1.Items[this.listView1.Items.Count - 1].SubItems[3].Text;
                //}
                //else if (listView2.Items.Count > 1)//Найденных товаров больше одного необходимо показать список выбра пользователю
                //{

                //    // this.listView2.Width = SystemInformation.PrimaryMonitorSize.Width;
                //    this.panel2.Visible = true;
                //    this.panel2.BringToFront();
                //    this.listView2.Visible = true;
                //    listView2.Select();
                //    listView2.Items[0].Selected = true;
                //    listView2.Items[0].Focused = true;

                //    //listView2.Focus();
                //}
                //else if (listView2.Items.Count == 0)
                //{
                //    //MessageBox.Show("Не найден");
                //    //stop_com_barcode_scaner();
                //    last_tovar.Text = barcode;
                //    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                //    t_n_f.ShowDialog();
                //    t_n_f.Dispose();
                //    //start_com_barcode_scaner();
                //}

                reader.Close();
                //                 conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }

            //write_new_document("0", "0", "0", "0", false, "0", "0", "0");
        }
        
       
        /// <summary>
        /// обработка акций вызывается в двух режимах
        /// 1. Без окна вызова ввода штрихкода Предварительный рассчет
        /// 2. С вызовом всех дополнительных окон, окончательный рассчет
        /// </summary>
        /// <param name="show_query_window_barcode"></param>
        private void to_define_the_action_dt(bool show_query_window_barcode)
        {

            if (!check_and_create_checks_table_temp())
            {
                return;
            }

            //total_seconnds = 0;
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            short tip_action;// = 0;
            Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker FROM action_header " +
                //    " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end "+
                //    //" AND " + count_minutes.ToString() + " between time_start AND time_end  
                //    " AND num_doc in(" +
                //    " SELECT DISTINCT action_table.num_doc FROM checks_table_temp " +
                //    " LEFT JOIN action_table ON checks_table_temp.tovar = action_table.code_tovar) order by date_started ";

                string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker FROM action_header " +
                    " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
                    " AND " + count_minutes.ToString() + " between time_start AND time_end  AND num_doc in(" +
                    " SELECT DISTINCT action_table.num_doc FROM checks_table_temp " +
                    " LEFT JOIN action_table ON checks_table_temp.tovar = action_table.code_tovar) order by date_started ";



                command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //listView1.Focus();
                    if (reader.GetString(6).Trim().Length != 0)
                    {
                        continue;
                    }
                    
                    tip_action = reader.GetInt16(0);
                    /* Обработать акцию по типу 1
                    * первый тип это скидка на конкретный товар
                    * если есть процент скидки то дается скидка 
                    * иначе выдается сообщение о подарке*/
                    if (tip_action == 1)
                    {
                        //start_action = DateTime.Now;
                        if (reader.GetDecimal(2) != 0)
                        {
                            action_1_dt(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
                        }
                        else
                        {
                            if (!show_query_window_barcode)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            {
                                action_1_dt(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt32(4)); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 2)
                    {
                        //start_action = DateTime.Now;

                        if (reader.GetDecimal(2) != 0)
                        {
                            action_2_dt(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
                        }
                        else
                        {
                            if (!show_query_window_barcode)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            {
                                //action_2_dt(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt32(4)); //Сообщить о подарке                           
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 3)
                    {
                        //start_action = DateTime.Now;

                        //action_2(reader.GetInt32(1));
                        if (reader.GetDecimal(2) != 0)
                        {
                            //action_3(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //action_3(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt16(7)); //Сообщить о подарке                           
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 4)
                    {
                        //start_action = DateTime.Now;

                        if (reader.GetDecimal(2) != 0)
                        {
                            //action_4(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //action_4(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt32(4));
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 6)
                    {           //Номер документа  //Сообщение о подарке //Сумма в данном случае шаг акции
                        //start_action = DateTime.Now;
                        //action_6(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt16(7));
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 8)
                    {
                        //start_action = DateTime.Now;
                        if (reader.GetDecimal(2) != 0)
                        {
                            //action_8(reader.GetInt32(1), reader.GetDecimal(2), reader.GetDecimal(5));//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //action_8(reader.GetInt32(1), reader.GetString(3), reader.GetDecimal(5), reader.GetInt32(4), reader.GetInt16(7));
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 9)//Акция работает в день рождения владельца дисконтной карты
                    {
                        //start_action = DateTime.Now;
                        if (!actions_birthday())
                        {
                            //write_time_execution("проверка на день рождения", tip_action.ToString());
                            continue;
                        }

                        if (reader.GetDecimal(2) != 0)
                        {
                            //action_1(reader.GetInt32(1), reader.GetDecimal(2));//Дать скидку на эту позицию                                                 
                        }
                        else
                        {
                            //action_1(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt32(4)); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 10)
                    {
                        if (reader.GetDecimal(5) <= calculation_of_the_sum_of_the_document())
                        {
                            MessageBox.Show(reader[3].ToString());
                            action_num_doc = Convert.ToInt32(reader[1].ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неопознанный тип акции в документе  № " + reader[1].ToString(), " Обработка акций ");
                    }
                }
                reader.Close();
                //                conn.Close();

                //recalculate_all();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке акций");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }
            //MessageBox.Show(total_seconnds.ToString());
        }

        /*Поиск товара по штрихкоду
        * и добвление его в табличную часть
        * это подарочный товар
        * добавляется по нулевой цене
        * barcode это код или штрихкод товара
        * count это количество позиций
        * sum_null если true тогда сумма и сумма со скидкой 0 иначе как обычный товар
        * это для акции 
        */
        public void find_barcode_or_code_in_tovar_action(string barcode, int count, bool sum_null, int num_doc)
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
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price from  barcode left join tovar ON barcode.tovar_code=tovar.code where barcode='" + barcode + "' AND tovar.its_deleted=0 ";
                }
                else
                {
                    command.CommandText = "select tovar.code,tovar.name,tovar.retail_price from  tovar where tovar.code='" + barcode + "' AND tovar.its_deleted=0 ";
                }

                NpgsqlDataReader reader = command.ExecuteReader();

                bool there_are_goods = false;//Флаг для понимания есть ли акционный товар

                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    row["tovar_code"] = reader.GetInt32(0).ToString();//ListViewItem lvi = new ListViewItem(reader.GetInt32(0).ToString());                    
                    row["tovar_name"] = reader.GetString(1);//Наименование
                    //lvi.SubItems.Add("");//Характеристика
                    row["quantity"] = count;//Количество
                    row["price"] = reader.GetDecimal(2);//Цена
                    row["price_at_discount"] = reader.GetDecimal(2);//Цена соскидкой    
                    if (sum_null)
                    {
                        row["sum_full"] = 0;// reader.GetDecimal(2);//Цена
                        row["sum_at_discount"] = 0;// reader.GetDecimal(2);//Цена соскидкой                            
                    }
                    else
                    {
                        row["sum_full"] = Convert.ToDecimal(row["price"]) * Convert.ToDecimal(row["quantity"]);// lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[2].Text) * Convert.ToDecimal(lvi.SubItems[3].Text)).ToString());//Сумма
                        row["sum_at_discount"] = Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]); //lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[2].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString()); //Сумма со скидкой                        
                    }
                    row["action"] = 0;// lvi.SubItems.Add("0");
                    row["gift"] = num_doc;
                    row["action2"] = num_doc;
                    there_are_goods = true;
                    //this.listView1.Select();
                    //this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
                }

                reader.Close();
                conn.Close();
                if (!there_are_goods)
                {
                    MessageBox.Show("ВНИМАНИЕ ПОДАРОК НЕ НАЙДЕН !!! СООБЩИТЕ АДМИНИСТРАТОРУ !!! ", "ОШИБКА ПРИ РАБОТЕ С АКЦИЯМИ");
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }

            }
        }


        private DialogResult show_query_window_barcode(int call_type, int count, int num_doc)
        {
            Input_action_barcode ib = new Input_action_barcode();
            ib.count = count;
            //ib.caller = this;
            ib.call_type = call_type;
            ib.num_doc = num_doc;
            DialogResult dr = ib.ShowDialog();
            ib.Dispose();
            return dr;
        }
                
        /// <summary>
        ///Обработать акцию по типу 1
        ///первый тип это скидка на конкретный товар
        ///если есть процент скидки то дается скидка 
        ///иначе выдается сообщение о подарке
        ///здесь сообщение о подарке
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="comment"></param>
        /// <param name="marker"></param>
        /// <param name="code_tovar"></param>
        private void action_1_dt(int num_doc, string comment, int marker, int code_tovar)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Int16 result = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
                string query = "";
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
                    {
                        continue;
                    }
                    query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["code"].ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());
                    if (result > 0)
                    {

                        //have_action = true;//Признак того что в документе есть сработка по акции                        
                        row["gift"] = num_doc.ToString();//Тип акции                                                 
                        MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок " + comment);
                        DialogResult dr = DialogResult.Cancel;
                        if (marker == 1)
                        {
                            dr = show_query_window_barcode(2, 1, num_doc);
                        }
                        if (dr != DialogResult.Cancel)
                        {
                            if (code_tovar != 0)
                            {
                                find_barcode_or_code_in_tovar(code_tovar.ToString());

                                //listView1.Items[listView1.Items.Count - 1].SubItems[8].Text = num_doc.ToString(); //Номер акционного документа 
                                //listView1.Items[listView1.Items.Count - 1].SubItems[9].Text = num_doc.ToString(); //Номер акционного документа 
                                //listView1.Items[listView1.Items.Count - 1].SubItems[10].Text = num_doc.ToString(); //Номер акционного документа 
                                ////Переносим в dt                                
                                //ListViewItem lvi = listView1.Items[listView1.Items.Count - 1];
                                //move_lvi_to_row(lvi);
                                //Конец Переносим в dt
                            }
                        }

                    }
                }
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 1 типа акций");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    //// conn.Dispose();
                }
            }
        }

        /// <summary>                
        ///Обработать акцию по типу 1
        ///первый тип это скидка на конкретный товар
        ///если есть процент скидки то дается скидка 
        ///иначе выдается сообщение о подарке
        ///Здесь скидка
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        private void action_1_dt(int num_doc, decimal persent)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Int16 result = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
                string query = "";
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
                    {
                        continue;
                    }
                    query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["code"].ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());
                    if (result > 0)
                    {
                        //have_action = true;//Признак того что в документе есть сработка по акции                        
                        row["price_at_discount"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(row["price_at_discount"]) - Convert.ToDecimal(row["price_at_discount"]) * persent / 100), 2);//Цена со скидкой                                    
                        row["sum_full"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"]);
                        row["sum_at_discount"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"]);
                        row["action"] = num_doc.ToString(); //Номер акционного документа                        
                        row["action2"] = num_doc.ToString();//Тип акции                        
                    }
                }
                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 1 типа акций");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
               
        /// <summary>
        ///Обработать акцию по 2 типу
        ///это значит в документе должен быть товар
        ///по вхождению в акционный список 
        ///Здесь дается скидка на кратное количество позиций из 1-го списка
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        private void action_2_dt(int num_doc, decimal persent)
        {
            /*В этой переменной запомнится позиция которая первой входит в первый список акции
            * на него будет дана скидка, необходимо скопировать эту позицию в конец списка 
            * и дать не на него скидку
            */
            int first_string_actions = 1000000;
            //int num_list=0;

            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            //object result = null;
            int quantity_of_pieces = 0;//Количество штук товара в строке
            int min_quantity = 1000000000;
            int num_list = 0;
            int num_pos = 0;
            ArrayList ar = new ArrayList();

            //Int16 result = 0;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();

                //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 

                string query = "SELECT COUNT(*) from(SELECT DISTINCT num_list FROM action_table where num_doc=" + num_doc + ") as foo";
                command = new NpgsqlCommand(query, conn);
                int[] action_list = new int[Convert.ToInt16(command.ExecuteScalar())];
                int x = 0;
                while (x < action_list.Length)
                {
                    action_list[x] = 0;
                    x++;
                }

                int index = -1;
                foreach (DataRow row in dt.Rows)
                {
                    index++;
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
                    {
                        continue;
                    }
                    quantity_of_pieces = Convert.ToInt16(row["quantity"]);
                    while (quantity_of_pieces > 0)
                    {
                        query = "SELECT num_list FROM action_table WHERE code_tovar=" + row["code"] + " AND num_doc=" + num_doc.ToString();
                        command = new NpgsqlCommand(query, conn);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        min_quantity = 1000000000;
                        num_list = 0;
                        int marker_min_amount = 1000000;
                        while (reader.Read())
                        {
                            min_quantity = Math.Min(min_quantity, action_list[reader.GetInt32(0) - 1]);
                            if (min_quantity < marker_min_amount)
                            {
                                num_list = reader.GetInt32(0);
                                marker_min_amount = min_quantity;
                            }
                            if ((first_string_actions == 1000000) && (reader.GetInt32(0) == 1))
                            {
                                first_string_actions = index;// lvi.Index; //Запомним номер строки товара на который будем давать скидку
                            }
                        }
                        if (num_list != 0)
                        {
                            action_list[num_list - 1] += 1;
                            num_pos = index;// lvi.Index;
                        }
                        if (num_list == 1)
                        {
                            //ar.Add(lvi.Tag.ToString());
                            ar.Add(row["code"].ToString());
                        }
                        quantity_of_pieces--;
                    }
                }
                //                conn.Close();
                bool the_action_has_worked = false;
                x = 0;
                min_quantity = 1000000000;

                while (x < action_list.Length)
                {
                    if (action_list[x] == 0)
                    {
                        the_action_has_worked = false;
                        break;
                    }
                    else
                    {
                        //Здесь надо получить кратность подарка
                        min_quantity = Math.Min(min_quantity, action_list[x]);
                        the_action_has_worked = true;
                    }
                    x++;
                }
                if (the_action_has_worked)
                {
                    //foreach (ListViewItem lvi in listView1.Items)
                    foreach (DataRow row in dt.Rows)
                    {
                        if (min_quantity > 0)
                        {
                            have_action = true;//Признак того что в документе есть сработка по акции

                            //Сначала получим количество,если больше кратного количества наборов то копируем строку,
                            //а в исходной уменьшаем количество на количества наборов и пересчитываем суммы
                            //
                            //if (ar.IndexOf(row["code"].lvi.Tag.ToString(), 0) == -1)
                            if (ar.IndexOf(row["code"].ToString(), 0) == -1)
                            {
                                continue;
                            }
                            //ListViewItem lvi = listView1.Items[first_string_actions];
                            quantity_of_pieces = Convert.ToInt16(row["quantity"]);
                            //int quantity_action_pieces = Math.Min(quantity_of_pieces, min_quantity);
                            if (quantity_of_pieces <= min_quantity)
                            {
                                row["price_at_discount"] = Math.Round(Convert.ToDecimal(row["price"]) - Convert.ToDecimal(row["price"]) * persent / 100, 2);//Цена со скидкой                                            
                                row["sum_full"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"]);
                                row["sum_at_discount"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString());
                                row["action"] = num_doc.ToString(); //Номер акционного документа 
                                min_quantity = min_quantity - quantity_of_pieces;
                            }
                            if ((quantity_of_pieces > min_quantity) && (min_quantity > 0))
                            {
                                //lvi.SubItems[3].Text = (Convert.ToInt32(lvi.SubItems[3].Text) - min_quantity).ToString();
                                row["quantity"] = Convert.ToInt32(row["quantity"]) - min_quantity;
                                //lvi.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа 
                                row["action"] = num_doc.ToString(); //Номер акционного документа 
                                //calculate_on_string_row(row);
                                ////Добавляем строку с акционным товаром 

                                //ListViewItem lvi_new = new ListViewItem(lvi.Tag.ToString());
                                //lvi_new.Tag = lvi.Tag;
                                //x = 0;
                                //while (x < lvi.SubItems.Count - 1)
                                //{
                                //    lvi_new.SubItems.Add(lvi.SubItems[x + 1].Text);
                                //    x++;
                                //}

                                //lvi_new.SubItems[2].Text = lvi.SubItems[2].Text;
                                //lvi_new.SubItems[2].Tag = lvi.SubItems[2].Tag;
                                //lvi_new.SubItems[3].Text = min_quantity.ToString();
                                //lvi_new.SubItems[5].Text = (Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text) - Convert.ToDecimal(lvi.SubItems[4].Text) * persent / 100, 2)).ToString();//Цена со скидкой            
                                //lvi_new.SubItems[6].Text = ((Convert.ToDecimal(lvi_new.SubItems[3].Text) * Convert.ToDecimal(lvi_new.SubItems[4].Text)).ToString());
                                //lvi_new.SubItems[7].Text = ((Convert.ToDecimal(lvi_new.SubItems[3].Text) * Convert.ToDecimal(lvi_new.SubItems[5].Text)).ToString());
                                //lvi_new.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
                                //lvi_new.SubItems[10].Text = num_doc.ToString(); //Номер акционного документа

                                //listView1.Items.Add(lvi_new);
                                //min_quantity = 0;
                            }
                        }
                    }

                    /*акция сработала
                 * надо отметить все товарные позиции 
                 * чтобы они не участвовали в других акциях 
                 */
                    if (the_action_has_worked)
                    {
                        //marked_action_tovar(num_doc);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 2 типа акций");
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

       // /*
       //* Когда по акции должен быть выдан подарок
       //* иногда вводится штрихкод, то есть подарок
       //* зараннее может быть не определен
       //* 1.Вызов для ввода акционного штрихкода который определяет собой акцию
       //* 2.Вызов для ввода штрихкода по которму будет найден подарок по акции с нулевой ценой
       //* 
       //*/
       // private DialogResult show_query_window_barcode(int call_type, int count, int num_doc)
       // {
       //     Input_action_barcode ib = new Input_action_barcode();
       //     ib.count = count;
       //     ib.caller = this;
       //     ib.call_type = call_type;
       //     ib.num_doc = num_doc;
       //     DialogResult dr = ib.ShowDialog();
       //     ib.Dispose();
       //     return dr;
       // }

       // /*
       //* Обработать акцию по 2 типу
       //* это значит в документе должен быть товар 
       //* по вхождению в акционный список 
       //  * 
       //* Здесь выдается сообщение о подарке
       //  * 
       //* Списки товаров могут быть абсолютно одинаковыми, а могут и отличатся
       //* 
       //*/
       // private void action_2_dt(int num_doc, string comment, int marker, int code_tovar)
       // {
       //     NpgsqlConnection conn = null;
       //     NpgsqlCommand command = null;
       //     //object result = null;
       //     int quantity_of_pieces = 0;//Количество штук товара в строке
       //     int min_quantity = 1000000000;
       //     int num_list = 0;
       //     int num_pos = 0;
       //     //Int16 result = 0;
       //     try
       //     {
       //         conn = MainStaticClass.NpgsqlConn();
       //         conn.Open();
       //         //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 

       //         string query = "SELECT COUNT(*) from(SELECT DISTINCT num_list FROM action_table where num_doc=" + num_doc + ") as foo";
       //         command = new NpgsqlCommand(query, conn);
       //         int[] action_list = new int[Convert.ToInt16(command.ExecuteScalar())];
       //         int x = 0;
       //         while (x < action_list.Length)
       //         {
       //             action_list[x] = 0;
       //             x++;
       //         }

       //         foreach (ListViewItem lvi in listView1.Items)
       //         {
       //             if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
       //             {
       //                 continue;
       //             }
       //             quantity_of_pieces = Convert.ToInt16(lvi.SubItems[3].Text);
       //             while (quantity_of_pieces > 0)
       //             {
       //                 query = "SELECT num_list FROM action_table WHERE code_tovar=" + lvi.Tag.ToString() + " AND num_doc=" + num_doc.ToString();
       //                 command = new NpgsqlCommand(query, conn);
       //                 NpgsqlDataReader reader = command.ExecuteReader();
       //                 min_quantity = 1000000000;
       //                 num_list = 0;
       //                 int marker_min_amount = 1000000;
       //                 while (reader.Read())
       //                 {
       //                     min_quantity = Math.Min(min_quantity, action_list[reader.GetInt32(0) - 1]);
       //                     if (min_quantity < marker_min_amount)
       //                     {
       //                         num_list = reader.GetInt32(0);
       //                         marker_min_amount = min_quantity;
       //                     }
       //                 }
       //                 if (num_list != 0)
       //                 {
       //                     action_list[num_list - 1] += 1;
       //                     num_pos = lvi.Index;
       //                 }
       //                 quantity_of_pieces--;
       //             }
       //         }
       //         //                conn.Close();
       //         bool the_action_has_worked = false;
       //         x = 0;
       //         min_quantity = 1000000000;

       //         while (x < action_list.Length)
       //         {
       //             if (action_list[x] == 0)
       //             {
       //                 the_action_has_worked = false;
       //                 break;
       //             }
       //             else
       //             {
       //                 //Здесь надо получить кратность подарка
       //                 min_quantity = Math.Min(min_quantity, action_list[x]);
       //                 the_action_has_worked = true;
       //             }
       //             x++;
       //         }
       //         if (the_action_has_worked)
       //         {
       //             have_action = true;//Признак того что в документе есть сработка по акции

       //             listView1.Items[num_pos].SubItems[9].Text = num_doc.ToString();
       //             MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок количестве " + min_quantity.ToString() + " шт. " + comment);
       //             DialogResult dr = DialogResult.Cancel;
       //             if (marker == 1)
       //             {
       //                 dr = show_query_window_barcode(2, min_quantity, num_doc);
       //             }

       //             if (dr != DialogResult.Cancel)
       //             {
       //                 find_barcode_or_code_in_tovar(code_tovar.ToString());
       //             }

       //             /*акция сработала
       //             * надо отметить все товарные позиции 
       //             * чтобы они не участвовали в других акциях 
       //             */

       //             if (the_action_has_worked)
       //             {
       //                 marked_action_tovar(num_doc);
       //             }

       //         }
       //     }
       //     catch (NpgsqlException ex)
       //     {
       //         MessageBox.Show(ex.Message + " | " + ex.Detail);
       //     }
       //     catch (Exception ex)
       //     {
       //         MessageBox.Show(ex.Message, "ошибка при обработке 2 типа акций");
       //     }
       //     finally
       //     {
       //         if (conn.State == ConnectionState.Open)
       //         {
       //             conn.Close();
       //             // conn.Dispose();
       //         }
       //     }
       // }


    }
}
