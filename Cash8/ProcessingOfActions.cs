﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using System.Linq;
using Npgsql;




namespace Cash8
{
    public partial class ProcessingOfActions
    {

        public DataTable dt = new DataTable();
        public DataTable dt_copy = new DataTable();//эта таблица необходима для временного хранения строк которые позднее будут добавлены в осносную базу в тот момент когда идет перебор строк основной таблицы в нее добавлять строки нельзя
        //private DataTable dt_gift = new DataTable();
        public string client_code = "";
        public int action_num_doc = 0;
        public ArrayList action_barcode_list = new ArrayList();//Доступ из формы ввода акционного штрихкода 
        public bool inpun_action_barcode = false;//Доступ из формы ввода акционного штрихкода
        public bool have_action = false;
        public decimal discount = 0;
        public bool show_messages = false;
        //public Cash_check cc = null;

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
                    sb.Append("INSERT INTO checks_table_temp(tovar)VALUES (" + row["tovar_code"].ToString() + ");");
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
            quantity.DataType = System.Type.GetType("System.Double");
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


            foreach (ListViewItem lvi in listView1.Items)
            {
                DataRow row = dt.NewRow();
                row["tovar_code"]                  = lvi.SubItems[0].Text;
                row["tovar_name"]                  = lvi.SubItems[1].Text;
                row["characteristic_code"]         = lvi.SubItems[2].Tag.ToString();
                row["characteristic_name"]         = lvi.SubItems[2].Text;
                row["quantity"]                    = lvi.SubItems[3].Text;
                row["price"]                       = lvi.SubItems[4].Text;
                row["price_at_discount"]           = lvi.SubItems[5].Text;
                row["sum_full"]                    = lvi.SubItems[6].Text;
                row["sum_at_discount"]             = lvi.SubItems[7].Text;
                row["action"]                      = lvi.SubItems[8].Text;
                row["gift"]                        = lvi.SubItems[9].Text;
                row["action2"]                     = lvi.SubItems[10].Text;
                row["bonus_reg"]                   = lvi.SubItems[11].Text;
                row["bonus_action"]                = lvi.SubItems[12].Text;
                row["bonus_action_b"]              = lvi.SubItems[13].Text;
                row["marking"]                     = lvi.SubItems[14].Text;

                dt.Rows.Add(row);
            }

            return dt;
        }

        ///// <summary>
        ///// Создаем таблицу значений в которую помещаем данные листвью
        ///// обработка акций далее будет происходить над этим новым объектом
        ///// </summary>
        ///// <param name="dt"></param>
        ///// <returns></returns>
        //public DataTable[] to_process_actions(DataTable dt)
        //{
        //    to_define_the_action_dt(false);//вызываем обработку акций
        //    DataTable[] dt_tables = new DataTable[2];
        //    dt_tables[0] = dt;
        //    //dt_tables[1] = dt_gift;

        //    return dt_tables;
        //}


        /// <summary>
        /// Подсчет суммы документа
        /// 
        /// </summary>
        /// <returns></returns>
        private decimal calculation_of_the_sum_of_the_document_dt()
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
        public void find_barcode_or_code_in_tovar_dt(string barcode)
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
        public void to_define_the_action_dt()
        {

            if (!check_and_create_checks_table_temp())
            {
                return;
            }

            //total_seconnds = 0;
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
            short tip_action;// = 0;            
            decimal persent = 0;
            Int32 num_doc = 0;
            string comment = "";
            short marker = 0;
            decimal sum = 0;
            decimal sum1 = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker,execution_order,sum1 FROM action_header " +
                    " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
                    " AND " + count_minutes.ToString() + " between time_start AND time_end  AND tip<>10 AND kind=0 AND num_doc in(" +
                    " SELECT DISTINCT action_table.num_doc FROM checks_table_temp " +
                    " LEFT JOIN action_table ON checks_table_temp.tovar = action_table.code_tovar)  order by execution_order asc, tip asc ";

                command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //listView1.Focus();
                    if (reader.GetString(6).Trim().Length != 0)
                    {
                        continue;
                    }
                                        
                    tip_action = Convert.ToInt16(reader["tip"]);
                    persent = Convert.ToDecimal(reader["persent"]);
                    num_doc = Convert.ToInt32(reader["num_doc"]);
                    comment = reader["comment"].ToString().Trim();
                    marker = Convert.ToInt16(reader["marker"]);
                    sum = Convert.ToDecimal(reader["sum"]);
                    sum1 = Convert.ToDecimal(reader["sum1"]);
                    /* Обработать акцию по типу 1
                    * первый тип это скидка на конкретный товар
                    * если есть процент скидки то дается скидка 
                    * иначе выдается сообщение о подарке*/
                    if (tip_action == 1)
                    {
                        //start_action = DateTime.Now;
                        if (persent != 0)
                        {
                            //action_1_dt(num_doc, persent, comment);//Дать скидку на эту позицию  
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_1_dt(num_doc, persent, comment);
                            }
                            else
                            {
                                action_1_dt(num_doc, persent, comment, LoadActionDataInMemory.AllActionData1);
                            }
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                            // action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            }
                            else
                            {
                                action_1_dt(num_doc, comment, marker, show_messages, LoadActionDataInMemory.AllActionData1); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 2)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {
                            if (LoadActionDataInMemory.AllActionData2 == null)
                            {
                                action_2_dt(num_doc, persent, comment);
                            }
                            else
                            {
                                action_2_dt(num_doc, persent, comment, LoadActionDataInMemory.AllActionData2);
                            }
                    }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                            //action_2_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                            if (LoadActionDataInMemory.AllActionData2 == null)
                            {
                                action_2_dt(num_doc, comment, show_messages);
                            }
                            else
                            {
                                action_2_dt(num_doc, comment, show_messages, LoadActionDataInMemory.AllActionData2);
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 3)
                    {
                        //start_action = DateTime.Now;                        
                        if (persent != 0)
                        {
                            //action_3_dt(num_doc, persent, sum, comment);//Дать скидку на все позиции из списка позицию                                                 
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_3_dt(num_doc, persent, sum, comment);
                            }
                            else
                            {
                                action_3_dt(num_doc, persent, sum, comment, LoadActionDataInMemory.AllActionData1);
                            }
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                                //action_3_dt(num_doc, comment, sum, marker,show_messages); //Сообщить о подарке                           
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_3_dt(num_doc, comment, sum, marker, show_messages);
                            }
                            else
                            {
                                action_3_dt(num_doc, comment, sum, marker, show_messages, LoadActionDataInMemory.AllActionData1);
                            }
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 4)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_4_dt(num_doc, persent, sum, comment);//Дать скидку на все позиции из списка позицию                                                 
                            }
                            else
                            {
                                action_4_dt(num_doc, persent, sum, comment, LoadActionDataInMemory.AllActionData1);//Дать скидку на все позиции из списка позицию                                                 
                            }
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{

                            //}
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_4_dt(num_doc, comment, sum, show_messages);
                            }
                            else
                            {
                                action_4_dt(num_doc, comment, sum, show_messages, LoadActionDataInMemory.AllActionData1);
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 6)
                    {           //Номер документа  //Сообщение о подарке //Сумма в данном случае шаг акции
                        //start_action = DateTime.Now;
                        if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                        {
                            action_6_dt(num_doc, comment, sum, marker);
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 8)
                    {
                        //start_action = DateTime.Now;
                        if (persent != 0)
                        {
                            if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            {
                                action_8_dt(num_doc, persent, sum,comment);//Дать скидку на все позиции из списка позицию                                                 
                            }
                        }
                        else
                        {
                            if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            {
                                action_8_dt(num_doc, comment, sum, marker);
                            }
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

                        //if (reader.GetDecimal(2) != 0)
                        if (persent != 0)
                        {
                            action_1_dt(num_doc, persent, comment);
                        }
                        else
                        {
                            //action_1_dt(reader.GetInt32(1), reader.GetString(3), reader.GetInt16(7), reader.GetInt32(4)); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    //else if (tip_action == 10)
                    //{
                    //    if (sum <= calculation_of_the_sum_of_the_document_dt())
                    //    {
                    //        //MessageBox.Show(reader[3].ToString());
                    //        action_num_doc = num_doc; //Convert.ToInt32(reader["num_doc"].ToString());
                    //    }
                    //}
                    else if (tip_action == 12)
                    {

                        action_12_dt(num_doc, persent,sum,sum1);
                    }
                    else
                    {
                        MessageBox.Show("Неопознанный тип акции в документе  № " + reader["num_doc"].ToString(), " Обработка акций ");
                    }
                }
                reader.Close();

                if (show_messages)
                {
                    query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker,execution_order FROM action_header " +
                     " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
                     " AND " + count_minutes.ToString() + " between time_start AND time_end AND bonus_promotion=0 " +
                     " AND barcode='' AND tip=10 AND num_doc in(" +//AND tip<>10 
                     " SELECT DISTINCT action_table.num_doc FROM checks_table_temp " +
                     " LEFT JOIN action_table ON checks_table_temp.tovar = action_table.code_tovar) order by execution_order asc, tip asc";//date_started asc,, tip desc

                    command = new NpgsqlCommand(query, conn);
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        if (Convert.ToDecimal(reader["sum"]) <= action_10_dt(Convert.ToInt32(reader["num_doc"])))
                        {
                            int multiplicity = (int)(calculation_of_the_sum_of_the_document_dt() / action_10_dt(Convert.ToInt32(reader["num_doc"])));
                            MessageBox.Show("Крастность " + multiplicity.ToString() + " " + reader["comment"].ToString());
                            action_num_doc = Convert.ToInt32(reader["num_doc"]);
                        }
                    }

                    reader.Close();
                }
                conn.Close();
                command.Dispose();
                if (show_messages)
                {
                    checked_action_10_dt();//Отдельная проверка поскольку может не быть товарной части, а все акции выше проверяются именно на вхождение товаров документа в таб части акционных документов
                }

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
                    conn.Dispose();
                }
            }
            //MessageBox.Show(total_seconnds.ToString());
        }
        
        /// <summary>
        /// Это сработка акций по группе клиентов
        /// 
        /// </summary>
        public void to_define_the_action_personal_dt(string code_client)
        {

            if (!check_and_create_checks_table_temp())
            {
                return;
            }

            //total_seconnds = 0;
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            short tip_action;// = 0;
            decimal persent = 0;
            Int32 num_doc = 0;
            string comment = "";
            short marker = 0;
            decimal sum = 0;
            Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker,execution_order FROM action_header " +
                string query = "SELECT tip,num_doc,persent,comment,sum,barcode,marker,execution_order FROM action_header " +
                    " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
                    " AND " + count_minutes.ToString() + " between time_start AND time_end  AND kind=2 AND num_doc in(" +
                    " (SELECT DISTINCT action_table.num_doc FROM checks_table_temp " +
                    " LEFT JOIN action_table ON checks_table_temp.tovar = action_table.code_tovar "+
                    " WHERE  action_table.num_doc in(SELECT num_doc	FROM action_clients where code_client='" + code_client+ "')))  order by execution_order asc, tip asc ";

                command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //listView1.Focus();
                    if (reader["barcode"].ToString().Trim().Length != 0)
                    {
                        continue;
                    }
                                        
                    tip_action = Convert.ToInt16(reader["tip"]);                    
                    persent = Convert.ToDecimal(reader["persent"]);
                    num_doc = Convert.ToInt32(reader["num_doc"]);
                    comment = reader["comment"].ToString();
                    marker = Convert.ToInt16(reader["marker"]);
                    sum = Convert.ToDecimal(reader["sum"]);
                    /* Обработать акцию по типу 1
                    * первый тип это скидка на конкретный товар
                    * если есть процент скидки то дается скидка 
                    * иначе выдается сообщение о подарке*/
                    if (tip_action == 1)
                    {
                        //start_action = DateTime.Now;
                        if (persent != 0)
                        {
                            //action_1_dt(num_doc, persent, comment);//Дать скидку на эту позицию  
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_1_dt(num_doc, persent, comment);
                            }
                            else
                            {
                                action_1_dt(num_doc, persent, comment, LoadActionDataInMemory.AllActionData1);
                            }
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                            // action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            }
                            else
                            {
                                action_1_dt(num_doc, comment, marker, show_messages, LoadActionDataInMemory.AllActionData1); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 2)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {
                            if (LoadActionDataInMemory.AllActionData2 == null)
                            {
                                action_2_dt(num_doc, persent, comment);
                            }
                            else
                            {
                                action_2_dt(num_doc, persent, comment, LoadActionDataInMemory.AllActionData2);
                            }
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                         
                            //action_2_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок                          
                            //}
                            if (LoadActionDataInMemory.AllActionData2 == null)
                            {
                                action_2_dt(num_doc, comment, show_messages);
                            }
                            else
                            {
                                action_2_dt(num_doc, comment, show_messages, LoadActionDataInMemory.AllActionData2);
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 3)
                    {
                        //start_action = DateTime.Now;

                        //action_2(reader.GetInt32(1));
                        if (persent != 0)
                        {                                                                        
                            action_3_dt(num_doc, persent, sum, comment);//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                                
                                action_3_dt(num_doc, comment, sum, marker, show_messages); //Сообщить о подарке                           
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 4)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {                                                                        
                            action_4_dt(num_doc, persent, sum,comment);//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                            
                                action_4_dt(num_doc, comment, sum, show_messages);
                            //}
                        }                    
                    }
                    else
                    {
                        MessageBox.Show("Неопознанный тип акции в документе  № " + reader["num_doc"].ToString(), " Обработка акций ");
                    }
                }
                reader.Close();
                                
                conn.Close();
                command.Dispose();                

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
                    conn.Dispose();
                }
            }
            //MessageBox.Show(total_seconnds.ToString());
        }
        /// <summary>
        /// Для Евы напоминание о подарке
        /// После сработки всех акций 
        /// 1.Сначала в табличной части проверяем товары которые не участвовали в акциях
        /// 2.Формируем список акций 2 типа(скидочные) которые в периоде действия и в них максимальный номер списка=2
        /// 3.Последовательно проверяем свободные товары на вхождение во 2-й список этих акций подокументно и выводим первые 10 поизций 1 списка в сообщение 
        /// </summary>
        /// <param name="num_doc"></param>
        private void load_list1_action2_dt()
        {
            string list_code_tovar = "";
            
            foreach (DataRow row in dt.Rows)
            {
                //index++;
                if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
                {
                    continue;
                }

                list_code_tovar += row["tovar_code"].ToString() + ",";
            }

            if (list_code_tovar == "")
            {
                return;
            }
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT action_header.num_doc FROM action_header " +
                    " LEFT JOIN action_table ON action_header.num_doc = action_table.num_doc " +
                    " WHERE tip = 2 AND  '" + DateTime.Now.ToString("dd-MM-yyyy") + "' between date_started AND date_end " +
                    " AND action_table.code_tovar in (" + list_code_tovar.Substring(0, list_code_tovar.Length - 1) +
                    " ) AND action_table.num_list = 2 GROUP BY action_header.num_doc HAVING MAX(action_table.num_list)=2 ";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    show_list1_dt(reader[0].ToString());
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// вывести сообщение по 1 списку в акции 2 типа 
        /// </summary>
        /// <param name="num_doc"></param>
        private void show_list1_dt(string num_doc)
        {
            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            try
            {
                conn.Open();
                string query = "SELECT tovar.name FROM public.action_table LEFT JOIN tovar ON action_table.code_tovar = tovar.code where num_doc=" + num_doc + " and num_list= 1 limit 10";
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                string result = "";
                while (reader.Read())
                {
                    result += reader[0].ToString().Trim() + "\r\n";
                }
                if (result != "")
                {
                    //MessageBox.Show(result,"Для сработки акции необходимо добавить любую позицию ИЗ ... ");
                    MyMessageBox myMessageBox = new MyMessageBox();
                    myMessageBox.text_message.Text = result;
                    myMessageBox.Text = " Для сработки акции необходимо добавить любую позицию ИЗ ... ";
                    myMessageBox.text_message.TextAlign = HorizontalAlignment.Left;
                    myMessageBox.ShowDialog();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подсказке по 2 акции" + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// обработка акций вызывается в двух режимах
        /// 1. Без окна вызова ввода штрихкода Предварительный рассчет
        /// 2. С вызовом всех дополнительных окон, окончательный рассчет
        /// </summary>
        /// <param name="show_query_window_barcode"></param>
        public void to_define_the_action_dt(string barcode)
        {

            if (!check_and_create_checks_table_temp())
            {
                return;
            }

            //total_seconnds = 0;
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            short tip_action;// = 0;
            decimal persent = 0;
            Int32 num_doc = 0;
            string comment = "";
            short marker = 0;
            decimal sum = 0;

            Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker,action_by_discount FROM action_header WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end AND barcode='" + barcode + "' AND kind = 1";
                string query = "SELECT tip,num_doc,persent,comment,sum,barcode,marker,action_by_discount FROM action_header WHERE '" +
                    DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end AND barcode='" + barcode + "' AND kind = 1";

                command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {                 

                    tip_action = Convert.ToInt16(reader["tip"]);
                    persent = Convert.ToDecimal(reader["persent"]);
                    num_doc = Convert.ToInt32(reader["num_doc"]);
                    comment = reader["comment"].ToString();
                    marker = Convert.ToInt16(reader["marker"]);
                    sum = Convert.ToDecimal(reader["sum"]);
                    /* Обработать акцию по типу 1
                    * первый тип это скидка на конкретный товар
                    * если есть процент скидки то дается скидка 
                    * иначе выдается сообщение о подарке*/
                    if (tip_action == 1)
                    {
                        //start_action = DateTime.Now;
                        if (persent != 0)
                        {
                            //action_1_dt(num_doc, persent, comment);//Дать скидку на эту позицию  
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_1_dt(num_doc, persent, comment);
                            }
                            else
                            {
                                action_1_dt(num_doc, persent, comment, LoadActionDataInMemory.AllActionData1);
                            }
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                            // action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                            if (LoadActionDataInMemory.AllActionData1 == null)
                            {
                                action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            }
                            else
                            {
                                action_1_dt(num_doc, comment, marker, show_messages, LoadActionDataInMemory.AllActionData1); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 2)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {
                            if (LoadActionDataInMemory.AllActionData2 == null)
                            {
                                action_2_dt(num_doc, persent, comment);
                            }
                            else
                            {
                                action_2_dt(num_doc, persent, comment, LoadActionDataInMemory.AllActionData2);
                            }
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                         
                            //action_2_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                            if (LoadActionDataInMemory.AllActionData2 == null)
                            {
                                action_2_dt(num_doc, comment, show_messages);
                            }
                            else
                            {
                                action_2_dt(num_doc, comment, show_messages, LoadActionDataInMemory.AllActionData2);
                            }
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 3)
                    {
                        //start_action = DateTime.Now;

                        //action_2(reader.GetInt32(1));
                        if (persent != 0)
                        {                                                                        
                            action_3_dt(num_doc, persent, sum, comment);//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                                
                                action_3_dt(num_doc, comment, sum, marker, show_messages); //Сообщить о подарке                           
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 4)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {                                                                        
                            action_4_dt(num_doc, persent, sum,comment);//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                            
                                action_4_dt(num_doc, comment, sum,show_messages);
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 5)
                    {                        
                        action_5_dt(num_doc, sum,comment);
                    }                    
                    else
                    {
                        MessageBox.Show("Неопознанный тип акции в документе  № " + reader[1].ToString(), " Обработка акций ");
                    }
                }
                reader.Close();
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

        /*Пометка товарных позиций которые участвовали в акции
        * для того чтобы они не участвоствовали в следующих акциях
        */
        private void marked_action_tovar_dt(DataTable dtCopy, int num_doc,string comment)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                foreach (DataRow row in dtCopy.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
                    {
                        continue;
                    }
                    string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"].ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    Int16 result = Convert.ToInt16(command.ExecuteScalar());
                    if (result == 1)
                    {
                        row["action2"] = num_doc.ToString();
                        if (dt.Columns.Contains("promo_description"))
                        {
                            row["promo_description"] = comment;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "пометка товарных позиций участвующих в акции");
            }

            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /*Пометка товарных позиций которые участвовали в акции
       * для того чтобы они не участвоствовали в следующих акциях
       */
        private void marked_action_tovar_dt(int num_doc, string comment)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
                    {
                        continue;
                    }
                    string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"].ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    Int16 result = Convert.ToInt16(command.ExecuteScalar());
                    if (result == 1)
                    {
                        row["action2"] = num_doc.ToString();
                        if (dt.Columns.Contains("promo_description"))
                        {
                            row["promo_description"] = comment;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "пометка товарных позиций участвующих в акции");
            }

            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// Пометка товарных позиций, которые участвовали в акции,
        /// чтобы они не участвовали в следующих акциях.
        /// </summary>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="actionPricesByDoc">Словарь с данными о товарах и их ценах по документам.</param>
        private void marked_action_tovar_dt(int num_doc, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            try
            {
                // Проверяем, есть ли данные для текущего документа в словаре
                if (actionPricesByDoc.ContainsKey(num_doc))
                {
                    // Получаем словарь с товарами для текущего документа
                    var tovarPrices = actionPricesByDoc[num_doc];

                    // Проходим по всем строкам в DataTable (предполагается, что dt — это DataTable)
                    foreach (DataRow row in dt.Rows)
                    {
                        // Пропускаем товары, которые уже участвовали в акции
                        if (Convert.ToInt32(row["action2"]) > 0)
                        {
                            continue;
                        }

                        // Получаем код товара из строки
                        long tovarCode = Convert.ToInt64(row["tovar_code"]);

                        // Проверяем, есть ли товар в словаре для текущего документа
                        if (tovarPrices.ContainsKey(tovarCode))
                        {
                            // Если товар участвовал в акции, помечаем его
                            row["action2"] = num_doc.ToString();

                            // Добавляем комментарий, если есть соответствующая колонка
                            if (dt.Columns.Contains("promo_description"))
                            {
                                row["promo_description"] = comment;
                            }
                        }
                    }
                }
                else
                {
                    // Если данных для текущего документа нет, можно вывести предупреждение
                    MessageBox.Show($"Данные для документа {num_doc} отсутствуют в словаре.");
                    MainStaticClass.WriteRecordErrorLog("Данные для документа {num_doc} отсутствуют в словаре.", "marked_action_tovar_dt(int num_doc, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)", num_doc, MainStaticClass.CashDeskNumber, "Отметка позиций уже участовавших в акции");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show(ex.Message, "Ошибка при пометке товарных позиций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "marked_action_tovar_dt(int num_doc, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)", num_doc, MainStaticClass.CashDeskNumber, "Отметка позиций уже участовавших в акции");
            }
        }

        /// <summary>
        /// Пометка товарных позиций, которые участвовали в акции,
        /// чтобы они не участвовали в следующих акциях.
        /// </summary>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="actionPricesByDoc">Словарь с данными о товарах и их ценах по документам.</param>
        private void marked_action_tovar_dt(DataTable dtCopy,int num_doc, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            try
            {
                // Проверяем, есть ли данные для текущего документа в словаре
                if (actionPricesByDoc.ContainsKey(num_doc))
                {
                    // Получаем словарь с товарами для текущего документа
                    var tovarPrices = actionPricesByDoc[num_doc];

                    // Проходим по всем строкам в DataTable (предполагается, что dt — это DataTable)
                    foreach (DataRow row in dtCopy.Rows)
                    {
                        // Пропускаем товары, которые уже участвовали в акции
                        if (Convert.ToInt32(row["action2"]) > 0)
                        {
                            continue;
                        }

                        // Получаем код товара из строки
                        long tovarCode = Convert.ToInt64(row["tovar_code"]);

                        // Проверяем, есть ли товар в словаре для текущего документа
                        if (tovarPrices.ContainsKey(tovarCode))
                        {
                            // Если товар участвовал в акции, помечаем его
                            row["action2"] = num_doc.ToString();

                            // Добавляем комментарий, если есть соответствующая колонка
                            if (dt.Columns.Contains("promo_description"))
                            {
                                row["promo_description"] = comment;
                            }
                        }
                    }
                }
                else
                {
                    // Если данных для текущего документа нет, можно вывести предупреждение
                    MessageBox.Show($"Данные для документа {num_doc} отсутствуют в словаре.");
                    MainStaticClass.WriteRecordErrorLog("Данные для документа {num_doc} отсутствуют в словаре.", "marked_action_tovar_dt(int num_doc, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)", num_doc, MainStaticClass.CashDeskNumber, "Отметка позиций уже участовавших в акции");
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show(ex.Message, "Ошибка при пометке товарных позиций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "marked_action_tovar_dt(int num_doc, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)", num_doc, MainStaticClass.CashDeskNumber, "Отметка позиций уже участовавших в акции");
            }
        }




        /// <summary>
        /// Возвращает true если в табличной части условия акций есть строки и false
        /// если строк нет
        /// </summary>
        /// <param name="num_action_doc"></param>
        /// <returns></returns>
        private bool checked_action_table_rows(int num_action_doc)
        {
            bool result = false;

            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();

                string query = "SELECT COUNT(*) FROM action_table WHERE num_doc=" + num_action_doc.ToString();
                command = new NpgsqlCommand(query, conn);
                int result_query = Convert.ToInt32(command.ExecuteScalar());
                if (result_query > 0)
                {
                    result = true;
                }
                conn.Close();
                command.Dispose();
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
            return result;
        }


        private void checked_action_10_dt()
        {

            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Int64 count_minutes = Convert.ToInt64((DateTime.Now - DateTime.Now.Date).TotalMinutes);
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();

                string query = "SELECT tip,num_doc,persent,comment,code_tovar,sum,barcode,marker FROM action_header " +
                    " WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end " +
                    " AND " + count_minutes.ToString() + " between time_start AND time_end and tip = 10";
                command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (checked_action_table_rows(Convert.ToInt32(reader[1].ToString())))
                    {
                        continue;
                    }

                    if (reader.GetDecimal(5) <= calculation_of_the_sum_of_the_document_dt())// action_10(Convert.ToInt32(reader["num_doc"]))
                    {
                        int multiplicity = (int)(calculation_of_the_sum_of_the_document_dt() / reader.GetDecimal(5));
                        MessageBox.Show("Кратность " + multiplicity.ToString() + " " + reader[3].ToString());
                        action_num_doc = Convert.ToInt32(reader[1].ToString());
                    }
                }
                reader.Close();
                conn.Close();
                command.Dispose();

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


        /// <summary>
        /// Это 10 акция для тех акционных документов у которых есть строки и 
        /// здесь происходит проверка по сумме на вхождение строк документа чек
        /// в строки акционного документа
        /// 
        /// </summary>
        /// <param name="num_doc"></param>
        /// <returns></returns>
        private Decimal action_10_dt(int num_doc)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Decimal result = 0;
            //bool is_found = false;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "";
                foreach (DataRow row in dt.Rows)
                {
                    query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"].ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    if (Convert.ToInt16(command.ExecuteScalar()) == 1)//вхождение найдено 
                    {
                        result += Convert.ToDecimal(row["sum_at_discount"].ToString());
                        //is_found = true;
                    }
                }
                conn.Close();
                command.Dispose();
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

            //if (is_found)
            //{
            //    result = calculation_of_the_sum_of_the_document(); 
            //}

            return result;
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
        public void find_barcode_or_code_in_tovar_action_dt(string barcode, int count, bool sum_null, int num_doc,int mode,DataTable dtCopy=null)
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
                DataRow row = null;
                while (reader.Read())
                {
                    if (dtCopy == null)
                    {
                        if (mode == 0)
                        {
                            row = dt.NewRow();
                        }
                        else
                        {
                            row = dt_copy.NewRow();
                        }
                    }
                    else
                    {
                        row = dtCopy.NewRow();
                    }

                    row["tovar_code"] = reader.GetInt64(0).ToString();//ListViewItem lvi = new ListViewItem(reader.GetInt32(0).ToString());                    
                    row["tovar_name"] = reader.GetString(1);//Наименование
                    row["characteristic_code"] = "";
                    row["characteristic_name"] = "";                    
                    row["quantity"] = count;//Количество
                    row["price"] = reader.GetDecimal(2);//Цена
                    string retail_price = get_price_action(num_doc);
                    if (retail_price != "")
                    {
                        //row["price_at_discount"] = reader.GetDecimal(2);//Цена соскидкой    
                        row["price_at_discount"] = Decimal.Parse(retail_price);
                    }
                        
                    //if (sum_null)//это пережиток
                    //{
                    //    row["sum_full"] = 0;// reader.GetDecimal(2);//Цена
                    //    row["sum_at_discount"] = 0;// reader.GetDecimal(2);//Цена соскидкой                            
                    //}
                    //else
                    //{
                        row["sum_full"] = Convert.ToDecimal(row["price"]) * Convert.ToDecimal(row["quantity"]);// lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[2].Text) * Convert.ToDecimal(lvi.SubItems[3].Text)).ToString());//Сумма
                        row["sum_at_discount"] = Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]); //lvi.SubItems.Add((Convert.ToDecimal(lvi.SubItems[2].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString()); //Сумма со скидкой                        
                    //}
                    row["action"] = 0;// lvi.SubItems.Add("0");
                    row["gift"] = num_doc;
                    row["action2"] = num_doc;
                    row["bonus_reg"] = 0;
                    row["bonus_action"] = 0;
                    row["bonus_action_b"] = 0;
                    row["marking"] = "0";
                    if (mode == 0)
                    {
                        dt.Rows.Add(row);
                    }
                    else
                    {
                        dt_copy.Rows.Add(row);
                    }
                    there_are_goods = true;                    
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

        private DialogResult show_query_window_barcode(int call_type, int count, int num_doc, int mode)
        {
            Input_action_barcode ib = new Input_action_barcode();
            ib.count = count;
            ib.caller2 = this;
            ib.call_type = call_type;
            ib.num_doc = num_doc;
            ib.mode = mode;
            DialogResult dr = ib.ShowDialog();
            ib.Dispose();
            return dr;
        }

        private DialogResult show_query_window_barcode(int call_type, int count, int num_doc, int mode,DataTable dtCopy)
        {
            Input_action_barcode ib = new Input_action_barcode();
            ib.count = count;
            ib.caller2 = this;
            ib.call_type = call_type;
            ib.num_doc = num_doc;
            ib.mode = mode;
            ib.dtCopy = dtCopy;
            DialogResult dr = ib.ShowDialog();
            ib.Dispose();
            return dr;
        }


        ///// <summary>
        ///// Показывает окно запроса штрих-кода.
        ///// </summary>
        //private DialogResult ShowQueryWindowBarcode(int type, int quantity, int num_doc, int additionalParam)
        //{
        //    // Логика отображения окна
        //    return DialogResult.OK; // Заглушка, замените на реальную логику
        //}


        /// <summary>                
        ///Обработать акцию по типу 1
        ///первый тип это скидка на конкретный товар
        ///если есть процент скидки то дается скидка 
        ///иначе выдается сообщение о подарке
        ///Здесь скидка
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        //private void action_1_dt(int num_doc, decimal persent, string comment)//Мой старый метод
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    double result = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "";
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }

        //            query = "SELECT price FROM action_table WHERE code_tovar=" + row["tovar_code"].ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            object result_query = command.ExecuteScalar();
        //            command.Dispose();
        //            if (result_query != null)
        //            {
        //                if (Convert.ToDecimal(result_query) == 0)
        //                {
        //                    //have_action = true;//Признак того что в документе есть сработка по акции                        
        //                    row["price_at_discount"] = Math.Round(Convert.ToDouble(Convert.ToDouble(row["price"]) - Convert.ToDouble(row["price"]) * Convert.ToDouble(persent) / 100), 2,MidpointRounding.ToEven);//Цена со скидкой                                    
        //                    row["sum_full"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price"]),2,MidpointRounding.ToEven);
        //                    row["sum_at_discount"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price_at_discount"]),2,MidpointRounding.ToEven);
        //                    row["action"] = num_doc.ToString(); //Номер акционного документа                        
        //                    row["action2"] = num_doc.ToString();//Тип акции 
        //                    if (dt.Columns.Contains("promo_description"))
        //                    {
        //                        row["promo_description"] = comment;
        //                    }
        //                }
        //                else
        //                {
        //                    row["price_at_discount"] = result_query.ToString();//Цена со скидкой                                    
        //                    row["sum_full"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price"]),2,MidpointRounding.ToEven);
        //                    row["sum_at_discount"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price_at_discount"]),2,MidpointRounding.ToEven);
        //                    row["action"] = num_doc.ToString(); //Номер акционного документа                        
        //                    row["action2"] = num_doc.ToString();//Тип акции         
        //                    if (dt.Columns.Contains("promo_description"))
        //                    {
        //                        row["promo_description"] = comment;
        //                    }
        //                }
        //            }
        //        }
        //        conn.Close();                
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message + " | " + ex.Detail);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 1 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            conn.Dispose();
        //        }
        //    }
        //}


        /// <summary>                
        ///Обработать акцию по типу 1
        ///первый тип это скидка на конкретный товар
        ///если есть процент скидки то дается скидка 
        ///иначе выдается сообщение о подарке
        ///Здесь скидка чтение с диска
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        /// <param name="comment"></param>
        private void action_1_dt(int num_doc, decimal percent, string comment)
        {
            DataTable dtCopy = null; // Копия DataTable

            try
            {
                // Создаем копию DataTable перед началом обработки
                dtCopy = dt.Copy();

                using (var conn = MainStaticClass.NpgsqlConn())
                {
                    conn.Open();

                    var query = "SELECT code_tovar, price FROM action_table WHERE num_doc = @num_doc";
                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@num_doc", num_doc);
                        using (var reader = command.ExecuteReader())
                        {
                            var actionPrices = new Dictionary<long, decimal>();
                            while (reader.Read())
                            {
                                actionPrices[reader.GetInt64(0)] = reader.GetDecimal(1);
                            }

                            // Обрабатываем строки копии DataTable
                            ProcessRows(dtCopy, actionPrices, num_doc, percent, comment);

                            // Если все прошло успешно, заменяем оригинальный DataTable на измененную копию
                            dt = dtCopy;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка базы данных");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при обработке 1 типа акций");
            }
            finally
            {
                // Если произошла ошибка, dt остается неизменным
            }
        }


        private void ProcessRows(DataTable dtCopy, Dictionary<long, decimal> actionPrices, int num_doc, decimal percent, string comment)
        {
            foreach (DataRow row in dtCopy.Rows)
            {
                if (Convert.ToInt32(row["action2"]) > 0)
                {
                    continue;
                }

                long tovarCode = Convert.ToInt64(row["tovar_code"]);
                if (actionPrices.TryGetValue(tovarCode, out var price))
                {
                    double priceAtDiscount;
                    if (price == 0)
                    {
                        priceAtDiscount = Math.Round(Convert.ToDouble(row["price"]) * (1 - (double)percent / 100), 2, MidpointRounding.ToEven);
                    }
                    else
                    {
                        priceAtDiscount = Convert.ToDouble(price);
                    }

                    row["price_at_discount"] = priceAtDiscount;
                    row["sum_full"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price"]), 2, MidpointRounding.ToEven);
                    row["sum_at_discount"] = Math.Round(Convert.ToDouble(row["quantity"]) * priceAtDiscount, 2, MidpointRounding.ToEven);
                    row["action"] = num_doc.ToString();
                    row["action2"] = num_doc.ToString();

                    if (dtCopy.Columns.Contains("promo_description"))
                    {
                        row["promo_description"] = comment;
                    }
                }
            }
        }


        /// <summary>
        /// Обрабатывает акции типа "1" - скидка на товары.
        /// </summary>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="percent">Процент скидки.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="actionPricesByDoc">Словарь, где ключ - номер документа, значение - словарь с товарами и их ценами по акции.</param>
        private void action_1_dt(int num_doc, decimal percent, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            DataTable dtCopy = null; // Копия DataTable

            try
            {
                // Создаем копию DataTable перед началом обработки
                dtCopy = dt.Copy();

                // Проверяем, есть ли данные для текущего документа
                if (!actionPricesByDoc.ContainsKey(num_doc))
                {
                    MessageBox.Show($"Данные для документа {num_doc} не найдены.", "Обработка акций 1 типа");
                    MainStaticClass.WriteRecordErrorLog($"Данные для документа {num_doc} не найдены.", "action_1_dt", num_doc, MainStaticClass.CashDeskNumber, "Обработка акций 1 типа скидка чтение с диска, номер документа здесь это номер ак. док.");
                    return;
                }

                // Получаем словарь с ценами для текущего документа
                var actionPrices = actionPricesByDoc[num_doc];

                // Проходим по всем строкам в копии DataTable
                foreach (DataRow row in dtCopy.Rows)
                {
                    // Пропускаем товары, уже участвовавшие в акциях
                    if (Convert.ToInt32(row["action2"]) > 0)
                    {
                        continue;
                    }

                    // Получаем код товара
                    long tovarCode = Convert.ToInt64(row["tovar_code"]);

                    // Проверяем, есть ли товар в словаре акций
                    if (actionPrices.TryGetValue(tovarCode, out var price))
                    {
                        // Вычисляем цену со скидкой
                        double priceAtDiscount;
                        if (price == 0)
                        {
                            // Если цена в акции не указана, применяем процент скидки
                            priceAtDiscount = Math.Round(Convert.ToDouble(row["price"]) * (1 - (double)percent / 100), 2, MidpointRounding.ToEven);
                        }
                        else
                        {
                            // Используем цену из акции
                            priceAtDiscount = Convert.ToDouble(price);
                        }

                        // Обновляем данные в строке
                        row["price_at_discount"] = priceAtDiscount;
                        row["sum_full"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price"]), 2, MidpointRounding.ToEven);
                        row["sum_at_discount"] = Math.Round(Convert.ToDouble(row["quantity"]) * priceAtDiscount, 2, MidpointRounding.ToEven);
                        row["action"] = num_doc.ToString();
                        row["action2"] = num_doc.ToString();

                        // Добавляем комментарий, если колонка существует
                        if (dtCopy.Columns.Contains("promo_description"))
                        {
                            row["promo_description"] = comment;
                        }
                    }
                }

                // Если все прошло успешно, заменяем оригинальный DataTable на измененную копию
                dt = dtCopy;
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show(ex.Message, "Ошибка при обработке 1 типа акций");
            }
            finally
            {
                // Если произошла ошибка, dt остается неизменным
            }
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
        //private void action_1_dt(int num_doc, string comment, int marker, long code_tovar)        
        //private void action_1_dt(int num_doc, string comment, int marker, bool show_messages)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    Int16 result = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "";
        //        dt_copy = dt.Clone();

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
        //            {
        //                continue;
        //            }
        //            query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"].ToString() + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            result = Convert.ToInt16(command.ExecuteScalar());
        //            if (result > 0)
        //            {
        //                have_action = true;//Признак того что в документе есть сработка по акции                        

        //                row["gift"] = num_doc.ToString();//Тип акции                                                 
        //                if (show_messages)
        //                {
        //                    MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок " + comment);                            
        //                    if (marker == 1)
        //                    {
        //                        show_query_window_barcode(2, 1, num_doc, 1);
        //                    }
        //                }                     
        //            }
        //        }
        //        foreach (DataRow row in dt_copy.Rows)
        //        {
        //            dt.ImportRow(row);
        //        }
        //        conn.Close();
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message + " | " + ex.Detail);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 1 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            //// conn.Dispose();
        //        }
        //    }
        //}

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
        /// <param name="show_messages"></param>
        private void action_1_dt(int num_doc, string comment, int marker, bool show_messages)
        {
            DataTable dtCopy = null; // Копия DataTable

            try
            {
                // Создаем копию DataTable перед началом обработки
                dtCopy = dt.Copy();

                using (var conn = MainStaticClass.NpgsqlConn())
                {
                    conn.Open();

                    // Загружаем все товары, участвующие в акции, одним запросом
                    var actionItems = LoadActionItems(conn, num_doc);

                    // Обрабатываем строки копии DataTable
                    ProcessRows(dtCopy, actionItems, num_doc, comment, marker, show_messages);

                    // Если все прошло успешно, заменяем оригинальный DataTable на измененную копию
                    dt = dtCopy;
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка базы данных");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_1_dt", num_doc, MainStaticClass.CashDeskNumber, "Обработка акций 1 типа подарок чтение с диска, номер документа здесь это номер ак. док.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при обработке акции 1 типа");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_1_dt", num_doc, MainStaticClass.CashDeskNumber, "Обработка акций 1 типа подарок чтение с диска, номер документа здесь это номер ак. док.");
            }
            finally
            {
                // Если произошла ошибка, dt остается неизменным
            }
        }

        private HashSet<long> LoadActionItems(NpgsqlConnection conn, int num_doc)
        {
            var actionItems = new HashSet<long>();

            string query = "SELECT code_tovar FROM action_table WHERE num_doc = @num_doc";
            using (var command = new NpgsqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@num_doc", num_doc);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actionItems.Add(reader.GetInt64(0));
                    }
                }
            }

            return actionItems;
        }

        /// <summary>
        /// Обрабатывает акцию по типу 1 с использованием предварительно загруженных данных.
        /// </summary>
        /// <param name="num_doc">Номер документа.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="marker">Маркер для дополнительной логики.</param>
        /// <param name="show_messages">Флаг показа сообщений.</param>
        /// <param name="actionPricesByDoc">Словарь с данными о товарах и их ценах.</param>
        private void action_1_dt(int num_doc, string comment, int marker, bool show_messages, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            DataTable dtCopy = null; // Копия DataTable

            try
            {
                // Создаем копию DataTable перед началом обработки
                dtCopy = dt.Copy();

                // Проверяем, есть ли данные для текущего документа
                if (!actionPricesByDoc.ContainsKey(num_doc))
                {
                    MessageBox.Show($"Данные для документа {num_doc} не найдены.", "Обработка акций 1 типа");
                    return;
                }

                // Получаем товары, участвующие в акции
                var actionItems = new HashSet<long>(actionPricesByDoc[num_doc].Keys); // Используем конструктор HashSet<T>

                // Обрабатываем строки копии DataTable
                ProcessRows(dtCopy, actionItems, num_doc, comment, marker, show_messages);

                // Если все прошло успешно, заменяем оригинальный DataTable на измененную копию
                dt = dtCopy;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при обработке акции 1 типа");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_1_dt", num_doc, MainStaticClass.CashDeskNumber, "Обработка акций 1 типа подарок со словарем, номер документа здесь это номер ак. док.");
            }
            finally
            {
                // Если произошла ошибка, dt остается неизменным
            }
        }


        /// <summary>
        /// Обрабатывает строки DataTable и применяет акцию.
        /// </summary>
        /// <param name="dtCopy">Копия DataTable для обработки.</param>
        /// <param name="actionItems">Список товаров, участвующих в акции.</param>
        /// <param name="num_doc">Номер документа.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="marker">Маркер для дополнительной логики.</param>
        /// <param name="show_messages">Флаг показа сообщений.</param>
        private void ProcessRows(DataTable dtCopy, HashSet<long> actionItems, int num_doc, string comment, int marker, bool show_messages)
        {
            foreach (DataRow row in dtCopy.Rows)
            {
                if (Convert.ToInt32(row["action2"]) > 0) continue; // Пропускаем товары, уже участвовавшие в акциях

                long tovarCode = Convert.ToInt64(row["tovar_code"]);
                if (actionItems.Contains(tovarCode))
                {
                    have_action = true; // Признак срабатывания акции
                    row["gift"] = num_doc.ToString(); // Тип акции

                    if (show_messages)
                    {
                        MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок " + comment);

                        if (marker == 1)
                        {
                            var result = show_query_window_barcode(2, 1, num_doc, 1,dtCopy);
                            //if (result == DialogResult.OK)
                            //{
                            //    // Дополнительная логика, если требуется
                            //}
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Загружает товары, участвующие в акции.
        /// </summary>
        private HashSet<long> LoadActionItems(NpgsqlConnection conn, NpgsqlTransaction transaction, int num_doc)
        {
            var actionItems = new HashSet<long>();

            string query = "SELECT code_tovar FROM action_table WHERE num_doc = @num_doc";
            using (var command = new NpgsqlCommand(query, conn, transaction))
            {
                command.Parameters.AddWithValue("@num_doc", num_doc);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        actionItems.Add(reader.GetInt64(0));
                    }
                }
            }

            return actionItems;
        }

        ///// <summary>
        ///// Обрабатывает строки DataTable и применяет акцию.
        ///// </summary>
        //private void ProcessRows(DataTable dtCopy, HashSet<long> actionItems, int num_doc, string comment, int marker, bool show_messages)
        //{
        //    foreach (DataRow row in dtCopy.Rows)
        //    {
        //        if (Convert.ToInt32(row["action2"]) > 0) continue; // Пропускаем товары, уже участвовавшие в акциях

        //        long tovarCode = Convert.ToInt64(row["tovar_code"]);
        //        if (actionItems.Contains(tovarCode))
        //        {
        //            have_action = true; // Признак срабатывания акции
        //            row["gift"] = num_doc.ToString(); // Тип акции

        //            if (show_messages)
        //            {
        //                MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок " + comment);

        //                if (marker == 1)
        //                {
        //                    var result = show_query_window_barcode(2, 1, num_doc, 1);
        //                    //if (result == DialogResult.OK)
        //                    //{
        //                    //    // Дополнительная логика, если требуется
        //                    //}
        //                }
        //            }
        //        }
        //    }
        //}



        #region Ации 2 типа 
        /// </summary>
        /// Здесь акция с предварительно 
        /// загруженными данными в память
        /// <param name="num_doc"></param>
        /// <param name="percent"></param>
        /// <param name="comment"></param>
        /////Обработать акцию по 2 типу
        /////это значит в документе должен быть товар
        /////по вхождению в акционный список 
        /////Здесь дается скидка на кратное количество позиций из 1-го списка
        ///// </summary>
        ///// <param name="num_doc"></param>
        ///// <param name="persent"></param>
        private void action_2_dt(int num_doc, decimal percent, string comment,
                         Dictionary<int, LoadActionDataInMemory.ActionDataContainer> allActionData2)
        {
            DataTable dtCopy = null; // Копия DataTable

            try
            {
                // Создаем копию DataTable перед началом обработки
                dtCopy = dt.Copy();

                if (!allActionData2.ContainsKey(num_doc))
                {
                    MessageBox.Show($"Данные для документа {num_doc} не найдены.", "Обработка акций 2 типа");
                    MainStaticClass.WriteRecordErrorLog($"Данные для документа {num_doc} не найдены.", "action_2_dt скидка", Convert.ToInt16(num_doc), MainStaticClass.CashDeskNumber, "Обработка акций 2 типа с предварительно загруженным словарем");
                    return;
                }

                var container = allActionData2[num_doc];

                // Создаем копии словарей для работы с текущим документом
                var listItems = new Dictionary<int, List<long>>(container.ListItems);
                var listQuantities = new Dictionary<int, int>(container.ListQuantities);

                // Обрабатываем данные
                ProcessActionData(dtCopy, num_doc, percent, comment, listItems, listQuantities);

                // Если все прошло успешно, заменяем оригинальный DataTable на измененную копию
                dt = dtCopy;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при обработке 2 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_2_dt скидка", Convert.ToInt16(num_doc), MainStaticClass.CashDeskNumber, "Обработка акций 2 типа с предварительно загруженным словарем");
            }
        }


        /// </summary>
        /// Здесь акция данные читаются с диска                
        /////Обработать акцию по 2 типу
        /////это значит в документе должен быть товар
        /////по вхождению в акционный список 
        /////Здесь дается скидка на кратное количество позиций из 1-го списка
        ///// </summary>
        ///// <param name="num_doc"></param>
        ///// <param name="persent"></param>
        private void action_2_dt(int num_doc, decimal percent, string comment)
        {
            DataTable dtCopy = null; // Копия DataTable

            try
            {
                // Создаем копию DataTable перед началом обработки
                dtCopy = dt.Copy();

                Dictionary<int, List<long>> listItems = new Dictionary<int, List<long>>();
                Dictionary<int, int> listQuantities = new Dictionary<int, int>();

                using (var conn = MainStaticClass.NpgsqlConn())
                {
                    conn.Open();

                    // Загружаем данные из action_table
                    string query = @"
                SELECT num_list, code_tovar 
                FROM action_table 
                WHERE num_doc = @num_doc 
                ORDER BY num_list, code_tovar";

                    using (var command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@num_doc", num_doc);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int num_list = reader.GetInt32(0);
                                long code_tovar = reader.GetInt64(1);

                                if (!listItems.ContainsKey(num_list))
                                {
                                    listItems[num_list] = new List<long>();
                                }
                                listItems[num_list].Add(code_tovar);

                                if (!listQuantities.ContainsKey(num_list))
                                {
                                    listQuantities[num_list] = 0;
                                }
                            }
                        }
                    }
                }

                // Обрабатываем данные
                ProcessActionData(dtCopy, num_doc, percent, comment, listItems, listQuantities);

                // Если все прошло успешно, заменяем оригинальный DataTable на измененную копию
                dt = dtCopy;
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail, "Ошибка при обработке 2 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_2_dt скидка", Convert.ToInt16(num_doc), MainStaticClass.CashDeskNumber, "Обработка акций 2 типа чтение с диска ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при обработке 2 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_2_dt скидка", Convert.ToInt16(num_doc), MainStaticClass.CashDeskNumber, "Обработка акций 2 типа чтение с диска");
            }
        }

        private void ProcessActionData(DataTable dtCopy, int num_doc, decimal percent, string comment,
                              Dictionary<int, List<long>> listItems,
                              Dictionary<int, int> listQuantities)
        {
            if (!listItems.ContainsKey(1))
            {
                MessageBox.Show("Первый список товаров отсутствует.", "Обработка акций 2 типа");
                MainStaticClass.WriteRecordErrorLog("Первый список товаров отсутствует.", "action_2_dt скидка", Convert.ToInt16(num_doc), MainStaticClass.CashDeskNumber, "Обработка акций 2 типа общий метод для чтения с диска и словаря");
                return;
            }

            // Очищаем значения listQuantities перед подсчетом
            foreach (var key in listQuantities.Keys.ToList())
            {
                listQuantities[key] = 0;
            }

            Dictionary<long, int> firstListItems = new Dictionary<long, int>();
            int min_quantity = int.MaxValue;

            try
            {
                // Инициализируем firstListItems
                foreach (var code_tovar in listItems[1])
                {
                    firstListItems[code_tovar] = 0;
                }

                // Анализируем dtCopy для подсчета количества товаров из каждого списка
                foreach (DataRow row in dtCopy.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0) continue;

                    long tovar_code = Convert.ToInt64(row["tovar_code"]);
                    int quantity_of_pieces = Convert.ToInt32(row["quantity"]);

                    // Проверяем, к какому списку принадлежит товар
                    foreach (var num_list in listQuantities.Keys.ToList())
                    {
                        if (listItems.ContainsKey(num_list) && listItems[num_list].Contains(tovar_code))
                        {
                            listQuantities[num_list] += quantity_of_pieces;
                        }
                    }

                    // Обновляем количество товаров из первого списка
                    if (firstListItems.ContainsKey(tovar_code))
                    {
                        firstListItems[tovar_code] += quantity_of_pieces;
                    }
                }

                // Находим минимальное количество для применения скидки
                if (listQuantities.Any())
                {
                    min_quantity = listQuantities.Values.Min();
                }

                // Применяем скидку к товарам из первого списка
                ApplyDiscountsToEligibleItems(dtCopy, num_doc, percent, min_quantity, firstListItems);

                // Помечаем товары, участвовавшие в акции
                marked_action_tovar_dt(dtCopy, num_doc, comment);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при обработке 2 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_2_dt скидка", Convert.ToInt16(num_doc), MainStaticClass.CashDeskNumber, "Обработка акций 2 типа общий метод для чтения с диска и словаря");
            }
        }

        private void ApplyDiscountsToEligibleItems(DataTable dtCopy, int num_doc, decimal percent, int min_quantity, Dictionary<long, int> firstListItems)
        {
            DataRow newRow = null;

            foreach (DataRow row in dtCopy.Rows)
            {
                if (Convert.ToInt32(row["action2"]) > 0) continue;

                long tovar_code = Convert.ToInt64(row["tovar_code"]);
                int quantity_of_pieces = Convert.ToInt32(row["quantity"]);

                if (firstListItems.ContainsKey(tovar_code) && firstListItems[tovar_code] >= min_quantity)
                {
                    int discountedQuantity = Math.Min(quantity_of_pieces, min_quantity);

                    if (discountedQuantity > 0)
                    {
                        if (quantity_of_pieces <= discountedQuantity)
                        {
                            ApplyDiscountToRow(row, percent, num_doc);
                        }
                        else
                        {
                            newRow = CreateNewRow(dtCopy, row, discountedQuantity, percent, num_doc);
                            row["quantity"] = Convert.ToInt32(row["quantity"]) - discountedQuantity;
                            row["sum_at_discount"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price_at_discount"]), 2, MidpointRounding.ToEven);
                        }

                        firstListItems[tovar_code] -= discountedQuantity;

                        if (firstListItems[tovar_code] <= 0)
                        {
                            firstListItems.Remove(tovar_code);
                        }
                    }
                }
            }

            if (newRow != null)
            {
                dtCopy.Rows.Add(newRow);
            }
        }

        private void ApplyDiscountToRow(DataRow row, decimal percent, int num_doc)
        {
            double price = Convert.ToDouble(row["price"]);
            double discountPrice = Math.Round(price - price * (double)percent / 100, 2, MidpointRounding.ToEven);

            row["price_at_discount"] = discountPrice;
            row["sum_full"] = Math.Round(Convert.ToDouble(row["quantity"]) * price, 2, MidpointRounding.ToEven);
            row["sum_at_discount"] = Convert.ToDouble(row["quantity"]) * discountPrice;
            row["action"] = num_doc.ToString();
            row["action2"] = num_doc.ToString();
        }

        private DataRow CreateNewRow(DataTable dtCopy, DataRow originalRow, int quantity, decimal percent, int num_doc)
        {
            DataRow newRow = dtCopy.NewRow();
            newRow.ItemArray = originalRow.ItemArray;
            newRow["quantity"] = quantity;
            ApplyDiscountToRow(newRow, percent, num_doc);
            return newRow;
        }

        //private void marked_action_tovar_dt(DataTable dtCopy, int num_doc, string comment)
        //{
        //    // Логика для пометки товаров, участвовавших в акции
        //    foreach (DataRow row in dtCopy.Rows)
        //    {
        //        if (dtCopy.Columns.Contains("promo_description"))
        //        {
        //            row["promo_description"] = comment;
        //        }
        //    }
        //}



        #endregion


        /// <summary>
        /// Пометка товарных позиций, которые участвовали в акции,
        /// чтобы они не участвовали в следующих акциях.
        /// </summary>
        /// <param name="dtCopy">DataTable с товарами.</param>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="listQuantities">Словарь с данными о товарах и их количестве по документам.</param>
        /// <param name="show_messages">Флаг, указывающий, нужно ли показывать сообщения об ошибках.</param>
        private void marked_action_tovar_dt(DataTable dtCopy, int num_doc, string comment, Dictionary<int, int> listQuantities, bool show_messages)
        {
            try
            {
                // Проходим по всем строкам в DataTable
                foreach (DataRow row in dtCopy.Rows)
                {
                    // Пропускаем товары, которые уже участвовали в акции
                    if (Convert.ToInt32(row["action2"]) > 0)
                    {
                        continue;
                    }

                    // Получаем код товара из строки
                    int tovarCode = Convert.ToInt32(row["tovar_code"]);

                    // Проверяем, есть ли товар в словаре для текущего документа
                    if (listQuantities.ContainsKey(tovarCode))
                    {
                        // Если товар участвовал в акции, помечаем его
                        row["action2"] = num_doc.ToString();

                        // Добавляем комментарий, если есть соответствующая колонка
                        if (dtCopy.Columns.Contains("promo_description"))
                        {
                            row["promo_description"] = comment;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                if (show_messages)
                {
                    MessageBox.Show(ex.Message, "Пометка товарных позиций участвующих в акции");                    
                }

                // Логирование ошибки
                MainStaticClass.WriteRecordErrorLog(ex.Message, "marked_action_tovar_dt(DataTable dtCopy, int num_doc, string comment)", num_doc, MainStaticClass.CashDeskNumber, "Пометка товарных позиций участвующих в акции");
            }
        }


        ///// <summary>
        /////Обработать акцию по 2 типу
        /////это значит в документе должен быть товар
        /////по вхождению в акционный список 
        /////Здесь дается скидка на кратное количество позиций из 1-го списка
        ///// </summary>
        ///// <param name="num_doc"></param>
        ///// <param name="persent"></param>
        //private void action_2_dt(int num_doc, decimal percent, string comment)
        //{
        //    NpgsqlConnection conn = null;
        //    Dictionary<int, int> listQuantities = new Dictionary<int, int>(); // Количество товаров в каждом списке
        //    Dictionary<long, int> firstListItems = new Dictionary<long, int>(); // Товары из первого списка
        //    int min_quantity = int.MaxValue; // Минимальное количество для применения скидки

        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        // Получаем все списки акций и товары из первого списка
        //        string query = @"
        //    SELECT num_list, code_tovar 
        //    FROM action_table 
        //    WHERE num_doc = @num_doc 
        //    ORDER BY num_list, code_tovar";
        //        using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
        //        {
        //            command.Parameters.AddWithValue("@num_doc", num_doc);
        //            using (NpgsqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    int num_list = reader.GetInt32(0);
        //                    long code_tovar = reader.GetInt64(1);

        //                    // Заполняем словарь для первого списка
        //                    if (num_list == 1)
        //                    {
        //                        firstListItems[code_tovar] = 0; // Инициализируем счетчик
        //                    }

        //                    // Инициализируем счетчики для каждого списка
        //                    if (!listQuantities.ContainsKey(num_list))
        //                    {
        //                        listQuantities[num_list] = 0;
        //                    }
        //                }
        //            }
        //        }

        //        // Анализируем dt для подсчета количества товаров из каждого списка
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            if (Convert.ToInt32(row["action2"]) > 0) continue; // Пропускаем товары, уже участвовавшие в акциях

        //            long tovar_code = Convert.ToInt64(row["tovar_code"]);
        //            int quantity_of_pieces = Convert.ToInt32(row["quantity"]);

        //            // Проверяем, к какому списку принадлежит товар
        //            foreach (var num_list in listQuantities.Keys.ToList())
        //            {
        //                if (IsTovarInList(conn, num_doc, num_list, tovar_code))
        //                {
        //                    listQuantities[num_list] += quantity_of_pieces;
        //                }
        //            }

        //            // Обновляем количество товаров из первого списка
        //            if (firstListItems.ContainsKey(tovar_code))
        //            {
        //                firstListItems[tovar_code] += quantity_of_pieces;
        //            }
        //        }

        //        // Находим минимальное количество для применения скидки
        //        min_quantity = listQuantities.Values.Min();

        //        // Применяем скидку к товарам из первого списка
        //        ApplyDiscounts(num_doc, percent, min_quantity, firstListItems);

        //        // Помечаем товары, участвовавшие в акции
        //        marked_action_tovar_dt(num_doc, comment);
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message + " | " + ex.Detail);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Ошибка при обработке 2 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn != null && conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }
        //}

        //private bool IsTovarInList(NpgsqlConnection conn, int num_doc, int num_list, long tovar_code)
        //{
        //    string query = @"
        //SELECT COUNT(*) 
        //FROM action_table 
        //WHERE num_doc = @num_doc 
        //  AND num_list = @num_list 
        //  AND code_tovar = @code_tovar";
        //    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
        //    {
        //        command.Parameters.AddWithValue("@num_doc", num_doc);
        //        command.Parameters.AddWithValue("@num_list", num_list);
        //        command.Parameters.AddWithValue("@code_tovar", tovar_code);
        //        return Convert.ToInt32(command.ExecuteScalar()) > 0;
        //    }
        //}

        //private void ApplyDiscounts(int num_doc, decimal percent, int min_quantity, Dictionary<long, int> firstListItems)
        //{
        //    DataRow newRow = null;

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        if (Convert.ToInt32(row["action2"]) > 0) continue; // Пропускаем товары, уже участвовавшие в акциях

        //        long tovar_code = Convert.ToInt64(row["tovar_code"]);

        //        // Применяем скидку только к товарам из первого списка
        //        if (!firstListItems.ContainsKey(tovar_code)) continue;

        //        int quantity_of_pieces = Convert.ToInt32(row["quantity"]);

        //        // Вычисляем количество товаров, к которым можно применить скидку
        //        int discountedQuantity = Math.Min(quantity_of_pieces, min_quantity);

        //        if (discountedQuantity > 0)
        //        {
        //            if (quantity_of_pieces <= discountedQuantity)
        //            {
        //                // Применяем скидку ко всей строке
        //                ApplyDiscountToRow(row, percent, num_doc);
        //            }
        //            else
        //            {
        //                // Создаем новую строку для частичного количества
        //                newRow = CreateNewRow(row, discountedQuantity, percent, num_doc);
        //                row["quantity"] = Convert.ToInt32(row["quantity"]) - discountedQuantity;
        //                row["sum_at_discount"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price_at_discount"]), 2, MidpointRounding.ToEven);
        //            }
        //        }
        //    }

        //    if (newRow != null)
        //    {
        //        dt.Rows.Add(newRow);
        //    }
        //}

        //private void ApplyDiscountToRow(DataRow row, decimal percent, int num_doc)
        //{
        //    double price = Convert.ToDouble(row["price"]);
        //    double discountPrice = Math.Round(price - price * (double)percent / 100, 2, MidpointRounding.ToEven);

        //    row["price_at_discount"] = discountPrice;
        //    row["sum_full"] = Math.Round(Convert.ToDouble(row["quantity"]) * price, 2, MidpointRounding.ToEven);
        //    row["sum_at_discount"] = Convert.ToDouble(row["quantity"]) * discountPrice;
        //    row["action"] = num_doc.ToString();
        //    row["action2"] = num_doc.ToString();
        //}

        //private DataRow CreateNewRow(DataRow originalRow, int quantity, decimal percent, int num_doc)
        //{
        //    DataRow newRow = dt.NewRow();
        //    newRow.ItemArray = originalRow.ItemArray;
        //    newRow["quantity"] = quantity;
        //    ApplyDiscountToRow(newRow, percent, num_doc);
        //    return newRow;
        //}




        /// <summary>
        ///Обработать акцию по 2 типу
        ///это значит в документе должен быть товар
        ///по вхождению в акционный список 
        ///Здесь дается скидка на кратное количество позиций из 1-го списка
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        //private void action_2_dt(int num_doc, decimal persent, string comment)//Оригинальный метод
        //{
        //    /*В этой переменной запомнится позиция которая первой входит в первый список акции
        //    * на него будет дана скидка, необходимо скопировать эту позицию в конец списка 
        //    * и дать не на него скидку
        //    */
        //    int first_string_actions = 1000000;
        //    //int num_list=0;

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    //object result = null;
        //    int quantity_of_pieces = 0;//Количество штук товара в строке
        //    int min_quantity = 1000000000;
        //    int num_list = 0;
        //    int num_pos = 0;
        //    ArrayList ar = new ArrayList();

        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
        //        string query = "SELECT COUNT(*) from(SELECT DISTINCT num_list FROM action_table where num_doc=" + num_doc + ") as foo";
        //        command = new NpgsqlCommand(query, conn);
        //        int[] action_list = new int[Convert.ToInt16(command.ExecuteScalar())];
        //        int x = 0;
        //        while (x < action_list.Length)
        //        {
        //            action_list[x] = 0;
        //            x++;
        //        }

        //        int index = -1;
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            index++;
        //            if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
        //            {
        //                continue;
        //            }
        //            quantity_of_pieces = Convert.ToInt16(row["quantity"]);
        //            while (quantity_of_pieces > 0)
        //            {
        //                query = "SELECT num_list FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
        //                command = new NpgsqlCommand(query, conn);
        //                NpgsqlDataReader reader = command.ExecuteReader();
        //                min_quantity = 1000000000;
        //                num_list = 0;
        //                int marker_min_amount = 1000000;
        //                while (reader.Read())
        //                {
        //                    min_quantity = Math.Min(min_quantity, action_list[reader.GetInt32(0) - 1]);
        //                    if (min_quantity < marker_min_amount)
        //                    {
        //                        num_list = reader.GetInt32(0);
        //                        marker_min_amount = min_quantity;
        //                    }
        //                    if ((first_string_actions == 1000000) && (reader.GetInt32(0) == 1))
        //                    {
        //                        first_string_actions = index;// lvi.Index; //Запомним номер строки товара на который будем давать скидку
        //                    }
        //                }
        //                if (num_list != 0)
        //                {
        //                    action_list[num_list - 1] += 1;
        //                    num_pos = index;// lvi.Index;
        //                }
        //                if (num_list == 1)
        //                {
        //                    //ar.Add(lvi.Tag.ToString());
        //                    ar.Add(row["tovar_code"].ToString());
        //                }
        //                quantity_of_pieces--;
        //            }
        //        }
        //        //                conn.Close();
        //        bool the_action_has_worked = false;
        //        x = 0;
        //        min_quantity = 1000000000;

        //        while (x < action_list.Length)
        //        {
        //            if (action_list[x] == 0)
        //            {
        //                the_action_has_worked = false;
        //                break;
        //            }
        //            else
        //            {
        //                //Здесь надо получить кратность подарка
        //                min_quantity = Math.Min(min_quantity, action_list[x]);
        //                the_action_has_worked = true;
        //            }
        //            x++;
        //        }
        //        if (the_action_has_worked)
        //        {
        //            DataRow row2 = null;
        //            //foreach (ListViewItem lvi in listView1.Items)
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                if (min_quantity > 0)
        //                {
        //                    have_action = true;//Признак того что в документе есть сработка по акции
        //                    //Сначала получим количество,если больше кратного количества наборов то копируем строку,
        //                    //а в исходной уменьшаем количество на количества наборов и пересчитываем суммы                            
        //                    if (ar.IndexOf(row["tovar_code"].ToString(), 0) == -1)
        //                    {
        //                        continue;
        //                    }
        //                    quantity_of_pieces = Convert.ToInt16(row["quantity"]);
        //                    if (quantity_of_pieces <= min_quantity)
        //                    {
        //                        row["price_at_discount"] = Math.Round(Convert.ToDouble(row["price"]) - Convert.ToDouble(row["price"]) * Convert.ToDouble(persent) / 100, 2, MidpointRounding.ToEven);//Цена со скидкой                                            
        //                        row["sum_full"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price"]), 2, MidpointRounding.ToEven);
        //                        row["sum_at_discount"] = Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price_at_discount"]);
        //                        row["action"] = num_doc.ToString(); //Номер акционного документа 
        //                        row["action2"] = num_doc.ToString(); //Номер акционного документа 
        //                        min_quantity = min_quantity - quantity_of_pieces;
        //                    }
        //                    if ((quantity_of_pieces > min_quantity) && (min_quantity > 0))
        //                    {
        //                        row["quantity"] = Convert.ToDouble(row["quantity"]) - min_quantity;
        //                        row["sum_at_discount"] = Math.Round(Convert.ToDouble(row["quantity"]) * Convert.ToDouble(row["price_at_discount"]), 2, MidpointRounding.ToEven);

        //                        //Добавляем новую строку с количеством min_quantity 
        //                        row2 = dt.NewRow();
        //                        row2.ItemArray = row.ItemArray;
        //                        row2["quantity"] = min_quantity;
        //                        row2["price_at_discount"] = Math.Round(Convert.ToDouble(row2["price"]) - Convert.ToDouble(row2["price"]) * Convert.ToDouble(persent) / 100, 2);//Цена со скидкой                                            
        //                        row2["sum_at_discount"] = Math.Round(Convert.ToDouble(row2["quantity"]) * Convert.ToDouble(row2["price_at_discount"]), 2, MidpointRounding.ToEven);
        //                        row2["action"] = num_doc.ToString(); //Номер акционного документа 
        //                        row2["action2"] = num_doc.ToString(); //Номер акционного документа 
        //                        //dt.Rows.Add(row2);

        //                        //row["action"] = num_doc.ToString(); //Номер акционного документа 


        //                        //calculation_of_the_sum_of_the_document_dt()
        //                        // calculate_on_string(lvi);

        //                        ////Добавляем строку с акционным товаром 
        //                        //ListViewItem lvi_new = new ListViewItem(lvi.Tag.ToString());
        //                        //lvi_new.Tag = lvi.Tag;
        //                        //x = 0;
        //                        //while (x < lvi.SubItems.Count - 1)
        //                        //{
        //                        //    lvi_new.SubItems.Add(lvi.SubItems[x + 1].Text);
        //                        //    x++;
        //                        //}

        //                        //lvi_new.SubItems[2].Text = lvi.SubItems[2].Text;
        //                        //lvi_new.SubItems[2].Tag = lvi.SubItems[2].Tag;
        //                        //lvi_new.SubItems[3].Text = min_quantity.ToString();
        //                        //lvi_new.SubItems[5].Text = (Math.Round(Convert.ToDecimal(lvi.SubItems[4].Text) - Convert.ToDecimal(lvi.SubItems[4].Text) * persent / 100, 2)).ToString();//Цена со скидкой            
        //                        //lvi_new.SubItems[6].Text = ((Convert.ToDecimal(lvi_new.SubItems[3].Text) * Convert.ToDecimal(lvi_new.SubItems[4].Text)).ToString());
        //                        //lvi_new.SubItems[7].Text = ((Convert.ToDecimal(lvi_new.SubItems[3].Text) * Convert.ToDecimal(lvi_new.SubItems[5].Text)).ToString());
        //                        //lvi_new.SubItems[8].Text = num_doc.ToString(); //Номер акционного документа
        //                        //lvi_new.SubItems[10].Text = num_doc.ToString(); //Номер акционного документа
        //                        ////*****************************************************************************
        //                        //lvi_new.SubItems[11].Text = "0";
        //                        //lvi_new.SubItems[12].Text = "0";
        //                        //lvi_new.SubItems[13].Text = "0";
        //                        //lvi_new.SubItems[14].Text = "0";
        //                        ////*****************************************************************************
        //                        //listView1.Items.Add(lvi_new);
        //                        //SendDataToCustomerScreen(1, 0, 1);
        //                        //min_quantity = 0;


        //                    }
        //                }
        //            }
        //            if (row2 != null)
        //            {
        //                dt.Rows.Add(row2);
        //            }

        //            /*акция сработала
        //         * надо отметить все товарные позиции 
        //         * чтобы они не участвовали в других акциях 
        //         */
        //            if (the_action_has_worked)
        //            {
        //                marked_action_tovar_dt(num_doc, comment);
        //            }
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message + " | " + ex.Detail);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 2 типа акций");
        //    }

        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}

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

        /*
       * Обработать акцию по 2 типу
       * это значит в документе должен быть товар 
       * по вхождению в акционный список 
         * 
       * Здесь выдается сообщение о подарке
         * 
       * Списки товаров могут быть абсолютно одинаковыми, а могут и отличатся
       * 
       */
        //private void action_2_dt(int num_doc, string comment, int marker, long code_tovar)
        //private void action_2_dt(int num_doc, string comment, int marker, bool show_messages)
        //{
        //    //Про скидку убрано ибо в тексте как выяснилось не используется
        //    ///*В этой переменной запомнится позиция которая первой входит в первый список акции 
        //    //* на него будет дана скидка, необходимо скопировать эту позицию в конец списка 
        //    //* и дать не на него скидку
        //    //*/
        //    ////int first_string_actions = 1000000;
        //    //int num_list=0;

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    //object result = null;
        //    int quantity_of_pieces = 0;//Количество штук товара в строке
        //    int min_quantity = 1000000000;
        //    int num_list = 0;
        //    int num_pos = 0;
        //    ArrayList ar = new ArrayList();

        //    //Int16 result = 0;

        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 

        //        string query = "SELECT COUNT(*) from(SELECT DISTINCT num_list FROM action_table where num_doc=" + num_doc + ") as foo";
        //        command = new NpgsqlCommand(query, conn);
        //        int[] action_list = new int[Convert.ToInt16(command.ExecuteScalar())];
        //        int x = 0;
        //        while (x < action_list.Length)
        //        {
        //            action_list[x] = 0;
        //            x++;
        //        }

        //        int index = -1;
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            index++;
        //            if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
        //            {
        //                continue;
        //            }
        //            quantity_of_pieces = Convert.ToInt16(row["quantity"]);
        //            while (quantity_of_pieces > 0)
        //            {
        //                query = "SELECT num_list FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
        //                command = new NpgsqlCommand(query, conn);
        //                NpgsqlDataReader reader = command.ExecuteReader();
        //                min_quantity = 1000000000;
        //                num_list = 0;
        //                int marker_min_amount = 1000000;
        //                while (reader.Read())
        //                {
        //                    min_quantity = Math.Min(min_quantity, action_list[reader.GetInt32(0) - 1]);
        //                    if (min_quantity < marker_min_amount)
        //                    {
        //                        num_list = reader.GetInt32(0);
        //                        marker_min_amount = min_quantity;
        //                    }
        //                    //if ((first_string_actions == 1000000) && (reader.GetInt32(0) == 1))
        //                    //{
        //                    //    first_string_actions = index; //Запомним номер строки товара на который будем давать скидку
        //                    //}
        //                }
        //                if (num_list != 0)
        //                {
        //                    action_list[num_list - 1] += 1;
        //                    num_pos = index;
        //                }
        //                if (num_list == 1)
        //                {
        //                    ar.Add(row["tovar_code"].ToString());
        //                }
        //                quantity_of_pieces--;
        //            }
        //        }

        //        bool the_action_has_worked = false;
        //        x = 0;
        //        min_quantity = 1000000000;

        //        while (x < action_list.Length)
        //        {
        //            if (action_list[x] == 0)
        //            {
        //                the_action_has_worked = false;
        //                break;
        //            }
        //            else
        //            {
        //                //Здесь надо получить кратность подарка
        //                min_quantity = Math.Min(min_quantity, action_list[x]);
        //                the_action_has_worked = true;
        //            }
        //            x++;
        //        }
        //        if (the_action_has_worked)
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции 
        //            if (show_messages)
        //            {
        //                MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок количестве " + min_quantity.ToString() + " шт. " + comment);
        //                DialogResult dr = DialogResult.Cancel;
        //                if (marker == 1)
        //                {
        //                    dr = show_query_window_barcode(2, min_quantity, num_doc, 0);
        //                }
        //            }

        //            //if (dr != DialogResult.Cancel)
        //            //{
        //            //    find_barcode_or_code_in_tovar_dt(code_tovar.ToString());
        //            //}

        //            /*акция сработала
        //            * надо отметить все товарные позиции 
        //            * чтобы они не участвовали в других акциях 
        //            */

        //            if (the_action_has_worked)
        //            {
        //                marked_action_tovar_dt(num_doc,comment);
        //            }
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message + " | " + ex.Detail);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 2 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}




        /*
  * Обработать акцию по 2 типу
  * это значит в документе должен быть товар 
  * по вхождению в акционный список 
    * 
  * Здесь выдается сообщение о подарке*/
        private void action_2_dt(int num_doc, string comment, bool show_messages, Dictionary<int, LoadActionDataInMemory.ActionDataContainer> allActionData2)
   {
       if (!allActionData2.ContainsKey(num_doc))
       {
           MessageBox.Show($"Данные для документа {num_doc} не найдены.", "Обработка акций 2 типа");
                MainStaticClass.WriteRecordErrorLog($"Данные для документа {num_doc} не найдены.", "action_2_dt(int num_doc, string comment, bool show_messages, Dictionary<int, LoadActionDataInMemory.ActionDataContainer> allActionData2)", num_doc, MainStaticClass.CashDeskNumber, "Акции 2 типа выдается сообщение о подарке");
                return;
       }

       var container = allActionData2[num_doc];

       // Создаем копии словарей для работы с текущим документом
       var listItems = new Dictionary<int, List<long>>(container.ListItems);
       var listQuantities = new Dictionary<int, int>(container.ListQuantities);

       // Обрабатываем данные для подарков
       ProcessGifts(num_doc, comment, listItems, listQuantities, show_messages);
   }


        /*
      * Обработать акцию по 2 типу
      * это значит в документе должен быть товар 
      * по вхождению в акционный список 
        * 
      * Здесь выдается сообщение о подарке*/
  private void action_2_dt(int num_doc, string comment, bool show_messages)
  {
      Dictionary<int, List<long>> listItems = new Dictionary<int, List<long>>();
      Dictionary<int, int> listQuantities = new Dictionary<int, int>();

      try
      {
          using (var conn = MainStaticClass.NpgsqlConn())
          {
              conn.Open();

              // Загружаем данные из action_table
              string query = @"
          SELECT num_list, code_tovar 
          FROM action_table 
          WHERE num_doc = @num_doc 
          ORDER BY num_list, code_tovar";

              using (var command = new NpgsqlCommand(query, conn))
              {
                  command.Parameters.AddWithValue("@num_doc", num_doc);
                  using (var reader = command.ExecuteReader())
                  {
                      while (reader.Read())
                      {
                          int num_list = reader.GetInt32(0);
                          long code_tovar = reader.GetInt64(1);

                          if (!listItems.ContainsKey(num_list))
                          {
                              listItems[num_list] = new List<long>();
                          }
                          listItems[num_list].Add(code_tovar);

                          if (!listQuantities.ContainsKey(num_list))
                          {
                              listQuantities[num_list] = 0;
                          }
                      }
                  }
              }
          }

          // Обрабатываем данные для подарков
          ProcessGifts(num_doc, comment, listItems, listQuantities, show_messages);
      }
      catch (NpgsqlException ex)
      {
          MessageBox.Show(ex.Message, "Ошибка при обработке 2 типа акций");
          MainStaticClass.WriteRecordErrorLog(ex.Message, "action_2_dt(int num_doc, string comment, bool show_messages, Dictionary<int, LoadActionDataInMemory.ActionDataContainer> allActionData2)", num_doc, MainStaticClass.CashDeskNumber, "Акции 2 типа выдается сообщение о подарке");
      }
      catch (Exception ex)
      {
          MessageBox.Show(ex.Message, "Ошибка при обработке 2 типа акций");
          MainStaticClass.WriteRecordErrorLog(ex.Message, "action_2_dt(int num_doc, string comment, bool show_messages, Dictionary<int, LoadActionDataInMemory.ActionDataContainer> allActionData2)", num_doc, MainStaticClass.CashDeskNumber, "Акции 2 типа выдается сообщение о подарке");
      }
  }


  /** Обработать акцию по 2 типу
* это значит в документе должен быть товар 
* по вхождению в акционный список       
* Здесь выдается сообщение о подарке*/
        private void ProcessGifts(int num_doc, string comment,
                          Dictionary<int, List<long>> listItems,
                          Dictionary<int, int> listQuantities, bool show_messages)
        {
            // Создаем копию DataTable для работы
            DataTable dtCopy = dt.Copy();

            if (!listItems.ContainsKey(1))
            {
                MessageBox.Show("Первый список товаров отсутствует.", "Обработка акций 2 типа");
                MainStaticClass.WriteRecordErrorLog("Первый список товаров отсутствует.", @"ProcessGifts(int num_doc, string comment,
              Dictionary<int, List<long>> listItems,
              Dictionary<int, int> listQuantities, bool show_messages)", num_doc, MainStaticClass.CashDeskNumber, "Акции 2 типа выдается сообщение о подарке");
                return;
            }

            // Очищаем значения listQuantities перед подсчетом
            foreach (var key in listQuantities.Keys.ToList())
            {
                listQuantities[key] = 0;
            }

            Dictionary<long, int> firstListItems = new Dictionary<long, int>();

            try
            {
                // Инициализируем firstListItems
                foreach (var code_tovar in listItems[1])
                {
                    firstListItems[code_tovar] = 0;
                }

                // Анализируем dtCopy для подсчета количества товаров из каждого списка
                foreach (DataRow row in dtCopy.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0) continue;

                    long tovar_code = Convert.ToInt64(row["tovar_code"]);
                    int quantity_of_pieces = Convert.ToInt32(row["quantity"]);

                    // Проверяем, к какому списку принадлежит товар
                    foreach (var num_list in listQuantities.Keys.ToList())
                    {
                        if (listItems.ContainsKey(num_list) && listItems[num_list].Contains(tovar_code))
                        {
                            listQuantities[num_list] += quantity_of_pieces;
                        }
                    }

                    // Обновляем количество товаров из первого списка
                    if (firstListItems.ContainsKey(tovar_code))
                    {
                        firstListItems[tovar_code] += quantity_of_pieces;
                    }
                }

                int giftCount = 0;
                // Находим минимальное количество для применения подарков
                if (listQuantities.Any())
                {
                    giftCount = listQuantities.Values.Min();
                }

                // Выводим сообщение о количестве подарков
                if (giftCount > 0)
                {
                    if (show_messages)
                    {
                        MessageBox.Show($"Сработала акция, НЕОБХОДИМО выдать подарок количестве {giftCount} шт.  {comment}", "Акция 2 типа: Подарки");
                    }
                }

                // Помечаем товары, участвовавшие в акции (работаем с копией)
                if (LoadActionDataInMemory.AllActionData1 == null)
                {
                    marked_action_tovar_dt(dtCopy, num_doc, comment);
                }
                else
                {
                    marked_action_tovar_dt(dtCopy, num_doc, comment, LoadActionDataInMemory.AllActionData1);
                }

                // Если ошибок не произошло, применяем изменения к оригинальной таблице
                dt = dtCopy;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при обработке 2 типа акций");
                MainStaticClass.WriteRecordErrorLog("Ошибка при обработке акции.", @"ProcessGifts(int num_doc, string comment,
              Dictionary<int, List<long>> listItems,
              Dictionary<int, int> listQuantities, bool show_messages)", num_doc, MainStaticClass.CashDeskNumber, "Акции 2 типа выдается сообщение о подарке");
            }
        }


        /*Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
        * тогда дается скидка на те позиции которые перечисляются в условии акции
        */
        //private void action_3_dt(int num_doc, decimal persent, decimal sum, string comment)
        //{
        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    decimal sum_on_doc = 0;//сумма документа без скидок 
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
        //            {
        //                continue;
        //            }
        //            string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {
        //                sum_on_doc += Convert.ToDecimal(row["sum_at_discount"]);
        //            }
        //        }
        //        //Сумма документа без скидки больше или равна той что в условии ации
        //        //значит акция сработала
        //        if (sum_on_doc >= sum)
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции

        //            foreach (DataRow row in dt.Rows)
        //            {
        //                if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
        //                {
        //                    continue;
        //                }
        //                string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
        //                command = new NpgsqlCommand(query, conn);
        //                if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //                {
        //                    row["price_at_discount"] = (Math.Round(Convert.ToDecimal(row["price"]) - Convert.ToDecimal(row["price"]) * persent / 100, 2)).ToString();//Цена со скидкой            
        //                    row["sum_full"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString());
        //                    row["sum_at_discount"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString());
        //                    row["action"] = num_doc.ToString();
        //                    row["action2"] = num_doc.ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //        }
        //    }

        //}


        /// <summary>
        /// Эта акция срабатывает, когда сумма без скидки в документе >= сумме акции.
        /// Тогда дается скидка на те позиции, которые перечисляются в условии акции.
        /// </summary>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="percent">Процент скидки.</param>
        /// <param name="sum">Сумма акции.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="actionPricesByDoc">Словарь с данными из базы данных.</param>
        private void action_3_dt(int num_doc, decimal percent, decimal sum, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            // Создаем копию DataTable для работы
            DataTable dtCopy = dt.Copy();

            try
            {
                // Получаем коды товаров, участвующих в акции, из словаря
                var tovarCodesInAction = GetTovarCodesInActionFromDictionary(actionPricesByDoc, num_doc);

                // Вычисляем общую сумму документа без скидок
                decimal sumOnDoc = CalculateTotalSumWithoutDiscount(dtCopy, tovarCodesInAction);

                // Проверяем условия акции
                if (CheckActionConditions(sumOnDoc, sum))
                {
                    have_action = true; // Признак того, что в документе есть сработка по акции

                    // Применяем скидку к товарам
                    ApplyDiscountToTovars(dtCopy, tovarCodesInAction, num_doc, percent);

                    // Если все успешно, применяем изменения к исходной DataTable
                    dt = dtCopy;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_3_dt(int num_doc, decimal percent, decimal sum, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)", num_doc, MainStaticClass.CashDeskNumber, "Акции 3 типа скидка");
            }
        }

        /// <summary>
        /// Получает коды товаров, участвующих в акции, из словаря.
        /// </summary>
        /// <param name="actionPricesByDoc">Словарь с данными из базы данных.</param>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <returns>Набор кодов товаров.</returns>
        private HashSet<long> GetTovarCodesInActionFromDictionary(Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc, int num_doc)
        {
            if (actionPricesByDoc.ContainsKey(num_doc))
            {
                return new HashSet<long>(actionPricesByDoc[num_doc].Keys);
            }
            return new HashSet<long>();
        }

        /// <summary>
        /// Эта акция срабатывает, когда сумма без скидки в документе >= сумме акции.
        /// Тогда выдается сообщение о подарке.
        /// </summary>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="sum">Сумма акции.</param>
        /// <param name="marker">Маркер для дополнительной логики.</param>
        /// <param name="show_messages">Флаг, указывающий, нужно ли показывать сообщения.</param>
        /// <param name="actionPricesByDoc">Словарь с данными из базы данных.</param>
        private void action_3_dt(int num_doc, string comment, decimal sum, int marker, bool show_messages, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            // Создаем копию DataTable для работы
            DataTable dtCopy = dt.Copy();

            try
            {
                // Получаем коды товаров, участвующих в акции, из словаря
                var tovarCodesInAction = GetTovarCodesInActionFromDictionary(actionPricesByDoc, num_doc);

                // Обработка данных в копии DataTable
                decimal totalSumWithoutDiscount;
                int index;
                CalculateTotalSum(dtCopy, tovarCodesInAction, out totalSumWithoutDiscount, out index);

                if (CheckActionConditions(totalSumWithoutDiscount, sum))
                {
                    // Применяем изменения к копии DataTable
                    dtCopy.Rows[index]["gift"] = num_doc.ToString();

                    if (show_messages)
                    {
                        MessageBox.Show(comment, " АКЦИЯ !!!");
                        if (marker == 1)
                        {
                            //var dr = show_query_window_barcode(2, 1, num_doc, 0);
                            show_query_window_barcode(2, 1, num_doc, 0, dtCopy);
                        }
                    }

                    // Логика для отметки товаров (если нужно)
                    //marked_action_tovar_dt(num_doc, comment);
                    marked_action_tovar_dt(dtCopy,num_doc, comment);
                }

                // Если все успешно, применяем изменения к исходной DataTable
                dt = dtCopy;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_3_dt(int num_doc, string comment, decimal sum, int marker, bool show_messages, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)", num_doc, MainStaticClass.CashDeskNumber, "Акции 3 типа сообщение о подарке");
            }
        }


        /// <summary>
        /// Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
        /// тогда дается скидка на те позиции которые перечисляются в условии акции
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="percent"></param>
        /// <param name="sum"></param>
        /// <param name="comment"></param>
        private void action_3_dt(int num_doc, decimal percent, decimal sum, string comment)
        {
            // Создаем копию DataTable для работы
            DataTable dtCopy = dt.Copy();

            try
            {
                using (var conn = MainStaticClass.NpgsqlConn())
                {
                    conn.Open();

                    // Получаем коды товаров, участвующих в акции
                    var tovarCodesInAction = GetTovarCodesInAction(conn, num_doc);

                    // Вычисляем общую сумму документа без скидок
                    decimal sumOnDoc = CalculateTotalSumWithoutDiscount(dtCopy, tovarCodesInAction);

                    // Проверяем условия акции
                    if (CheckActionConditions(sumOnDoc, sum))
                    {
                        have_action = true; // Признак того, что в документе есть сработка по акции

                        // Применяем скидку к товарам
                        ApplyDiscountToTovars(dtCopy, tovarCodesInAction, num_doc, percent);

                        // Если все успешно, применяем изменения к исходной DataTable
                        dt = dtCopy;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                //Logger.LogError(ex, "Ошибка при работе с базой данных");
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_3_dt(int num_doc, decimal percent, decimal sum, string comment)", num_doc, MainStaticClass.CashDeskNumber, "Акции 3 типа скидка");
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, "Ошибка при обработке акции");
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_3_dt(int num_doc, decimal percent, decimal sum, string comment)", num_doc, MainStaticClass.CashDeskNumber, "Акции 3 типа скидка");
            }
        }

        private HashSet<long> GetTovarCodesInAction(NpgsqlConnection conn, int num_doc)
        {
            var tovarCodesInAction = new HashSet<long>();
            string query = "SELECT code_tovar FROM action_table WHERE num_doc = @num_doc";

            using (var command = new NpgsqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@num_doc", num_doc);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tovarCodesInAction.Add(reader.GetInt64(0)); // Используем GetInt64 для long
                }
            }

            return tovarCodesInAction;
        }

        private decimal CalculateTotalSumWithoutDiscount(DataTable dtCopy, HashSet<long> tovarCodesInAction)
        {
            decimal sumOnDoc = 0;

            foreach (DataRow row in dtCopy.Rows)
            {
                if (Convert.ToInt32(row["action2"]) > 0)
                {
                    continue;
                }

                if (tovarCodesInAction.Contains(Convert.ToInt64(row["tovar_code"]))) // Используем Convert.ToInt64 для long
                {
                    sumOnDoc += Convert.ToDecimal(row["sum_at_discount"]);
                }
            }

            return sumOnDoc;
        }

        private bool CheckActionConditions(decimal sumOnDoc, decimal sum)
        {
            return sumOnDoc >= sum;
        }

        private void ApplyDiscountToTovars(DataTable dtCopy, HashSet<long> tovarCodesInAction, int num_doc, decimal percent)
        {
            foreach (DataRow row in dtCopy.Rows)
            {
                if (Convert.ToInt32(row["action2"]) > 0)
                {
                    continue;
                }

                if (tovarCodesInAction.Contains(Convert.ToInt64(row["tovar_code"]))) // Используем Convert.ToInt64 для long
                {
                    decimal price = Convert.ToDecimal(row["price"]);
                    decimal priceAtDiscount = Math.Round(price - price * percent / 100, 2);
                    decimal quantity = Convert.ToDecimal(row["quantity"]);

                    row["price_at_discount"] = priceAtDiscount.ToString();
                    row["sum_full"] = (quantity * price).ToString();
                    row["sum_at_discount"] = (quantity * priceAtDiscount).ToString();
                    row["action"] = num_doc.ToString();
                    row["action2"] = num_doc.ToString();
                }
            }
        }
        /*Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
         * тогда выдается сообщение о подарке
         */
        //private void action_3_dt(int num_doc, string comment, decimal sum, int marker,bool show_messages)
        //{

        //    NpgsqlConnection conn = null;
        //    NpgsqlCommand command = null;
        //    decimal sum_on_doc = 0;//сумма документа без скидок 
        //    int index = 0;
        //    try
        //    {
        //        conn = MainStaticClass.NpgsqlConn();
        //        conn.Open();

        //        foreach (DataRow row in dt.Rows)
        //        {
        //            if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
        //            {
        //                continue;
        //            }
        //            string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
        //            command = new NpgsqlCommand(query, conn);
        //            if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //            {
        //                sum_on_doc += Convert.ToDecimal(row["sum_at_discount"]);
        //                index = dt.Rows.IndexOf(row);
        //            }
        //        }
        //        //Сумма документа без скидки больше или равна той что в условии ации
        //        //значит акция сработала
        //        if (sum_on_doc >= sum)
        //        {
        //            have_action = true;//Признак того что в документе есть сработка по акции                    
        //            dt.Rows[index]["gift"] = num_doc.ToString();//Тип акции                    
        //            if (show_messages)
        //            {
        //                MessageBox.Show(comment, " АКЦИЯ !!!");
        //            }
        //            DialogResult dr = DialogResult.Cancel;
        //            if (show_messages)
        //            {
        //                if (marker == 1)
        //                {
        //                    dr = show_query_window_barcode(2, 1, num_doc,0);
        //                }
        //            }
        //            /*акция сработала
        //            * надо отметить все товарные позиции 
        //            * чтобы они не участвовали в других акциях 
        //            */
        //            marked_action_tovar_dt(num_doc,comment);
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
        //    }
        //    finally
        //    {
        //        if (conn.State == ConnectionState.Open)
        //        {
        //            conn.Close();
        //            // conn.Dispose();
        //        }
        //    }
        //}

        /// <summary>
        ///   Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
        ///   тогда выдается сообщение о подарке
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="comment"></param>
        /// <param name="sum"></param>
        /// <param name="marker"></param>
        /// <param name="show_messages"></param>
        private void action_3_dt(int num_doc, string comment, decimal sum, int marker, bool show_messages)
        {
            // Создаем копию DataTable для работы
            DataTable dtCopy = dt.Copy();

            try
            {
                using (var conn = MainStaticClass.NpgsqlConn())
                {
                    conn.Open();

                    // Чтение данных из базы
                    var tovarCodesInAction = GetTovarCodesInAction(conn, num_doc);

                    // Обработка данных в копии DataTable
                    decimal totalSumWithoutDiscount;
                    int index;
                    CalculateTotalSum(dtCopy, tovarCodesInAction, out totalSumWithoutDiscount, out index);

                    if (CheckActionConditions(totalSumWithoutDiscount, sum))
                    {
                        // Применяем изменения к копии DataTable
                        dtCopy.Rows[index]["gift"] = num_doc.ToString();

                        if (show_messages)
                        {
                            MessageBox.Show(comment, " АКЦИЯ !!!");
                            if (marker == 1)
                            {
                                var dr = show_query_window_barcode(2, 1, num_doc, 0);
                            }
                        }

                        // Логика для отметки товаров (если нужно)
                        //MarkActionTovar(conn, num_doc, comment);
                        marked_action_tovar_dt(num_doc, comment);
                    }

                    // Если все успешно, применяем изменения к исходной DataTable
                    dt = dtCopy;
                }
            }
            catch (NpgsqlException ex)
            {
                // В случае ошибки при чтении из базы, dt остается неизменной
                //Logger.LogError(ex, "Ошибка при работе с базой данных");
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_3_dt(int num_doc, string comment, decimal sum, int marker, bool show_messages)", num_doc, MainStaticClass.CashDeskNumber, "Акции 3 типа сообщение о подарке");
            }
            catch (Exception ex)
            {
                // В случае любой другой ошибки, dt остается неизменной
                //Logger.LogError(ex, "Ошибка при обработке акции");
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_3_dt(int num_doc, string comment, decimal sum, int marker, bool show_messages)", num_doc, MainStaticClass.CashDeskNumber, "Акции 3 типа сообщение о подарке");
            }
        }

     

        private void CalculateTotalSum(DataTable dtCopy, HashSet<long> tovarCodesInAction, out decimal totalSumWithoutDiscount, out int index)
        {
            totalSumWithoutDiscount = 0;
            index = 0;

            foreach (DataRow row in dtCopy.Rows)
            {
                if (Convert.ToInt32(row["action2"]) > 0)
                {
                    continue;
                }

                if (tovarCodesInAction.Contains(Convert.ToInt32(row["tovar_code"])))
                {
                    totalSumWithoutDiscount += Convert.ToDecimal(row["sum_at_discount"]);
                    index = dtCopy.Rows.IndexOf(row);
                }
            }
        }

      

        /// <summary>
        /// Создание временной таблицы для 4 типа акций
        /// </summary>
        /// <returns></returns>
        private bool create_temp_tovar_table_4()
        {
            bool result = true;

            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();

                string query = "select COUNT(*) from information_schema.tables 		where table_schema='public' 	and table_name='tovar_action'	";
                command = new NpgsqlCommand(query, conn);

                if (Convert.ToInt16(command.ExecuteScalar()) == 0)
                {
                    query = "CREATE TABLE tovar_action(  code bigint NOT NULL,  retail_price numeric(10,2) NOT NULL,  quantity integer, retail_price_discount numeric(10,2) ,characteristic_name character varying(100),characteristic_guid character varying(36) )WITH (  OIDS=FALSE);ALTER TABLE tovar_action  OWNER TO postgres;";
                }
                else
                {
                    query = "DROP TABLE tovar_action;CREATE TABLE tovar_action(  code bigint NOT NULL,  retail_price numeric(10,2) NOT NULL,  quantity integer, retail_price_discount numeric(10,2) ,characteristic_name character varying(100),characteristic_guid character varying(36) )WITH (  OIDS=FALSE);ALTER TABLE tovar_action  OWNER TO postgres;";
                }
                command = new NpgsqlCommand(query, conn);
                command.ExecuteNonQuery();
                //                conn.Close();
                command.Dispose();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, " Ошибка при создании временной таблицы ");
                result = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " Ошибка при создании временной таблицы ");
                result = false;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }

            return result;
        }

        /*Эта акция срабатывает когда количество товаров 
        * в документе >= сумме(количество) товаров в акции
        * тогда дается скидка на кратное количество товара
        * на самый дешевый товар из участвующих в акции 
        * 
        */
        private void action_4_dt(int num_doc, decimal persent, decimal sum, string comment)
        {

            if (!create_temp_tovar_table_4())
            {
                return;
            }

            DataTable dt2 = dt.Copy();
            dt2.Rows.Clear();
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            decimal quantity_on_doc = 0;//количество позиций в документе            
            ListView clon = new ListView();
            StringBuilder query = new StringBuilder();
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query_string = "";

                //int index = -1;
                foreach (DataRow row in dt.Rows)
                {
                    //index++;
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
                    {
                        DataRow row2 = dt2.NewRow();
                        row2.ItemArray = row.ItemArray;
                        dt2.Rows.Add(row2);
                        continue;
                    }
                    query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query_string, conn);

                    if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                    {

                        for (int i = 0; i < Convert.ToInt32(row["quantity"]); i++)
                        {
                            query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid)VALUES(" +
                                row["tovar_code"].ToString() + "," +
                                row["price"].ToString().Replace(",", ".") + "," +
                               "1,'" +
                               row["characteristic_name"].ToString() + "','" +
                               row["characteristic_code"].ToString() + "');");
                        }
                        quantity_on_doc += Convert.ToDecimal(row["quantity"]);
                    }
                    else//Не участвует в акции убираем пока в сторонку
                    {
                        DataRow row2 = dt2.NewRow();
                        row2.ItemArray = row.ItemArray;
                        dt2.Rows.Add(row2);
                    }
                }


                if (quantity_on_doc >= sum)//Есть вхождение в акцию
                {
                    have_action = true;//Признак того что в документе есть сработка по акции                    
                    dt.Rows.Clear();
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        DataRow row = dt.NewRow();
                        row.ItemArray = row2.ItemArray;
                        dt.Rows.Add(row);
                    }

                    command = new NpgsqlCommand(query.ToString(), conn);//устанавливаем акционные позиции во временную таблицу
                    command.ExecuteNonQuery();
                    query.Append("DELETE FROM tovar_action;");//Очищаем таблицу акционных товаров 
                    //иначе результат задваивается ранее эта строка была закомментирована и при 2 товарах по 1 шт. учавстсующих в акции
                    //работала неверно

                    int multiplication_factor = (int)(quantity_on_doc / sum);
                    query_string = " SELECT code, retail_price, quantity,characteristic_name,characteristic_guid FROM tovar_action ORDER BY retail_price desc";//запросим товары отсортированные по цене, теперь еще и с характеристиками
                    command = new NpgsqlCommand(query_string, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();

                    Decimal sum_on_string = sum;

                    int num_records = 1;

                    while (reader.Read())
                    {
                        if (multiplication_factor > 0)
                        {
                            if ((decimal)num_records / sum == Math.Round(num_records / sum, 0, MidpointRounding.AwayFromZero))//(sum_on_string == sum) && 
                            {
                                query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid,retail_price_discount)VALUES(" +
                                  reader[0].ToString() + "," +
                                  reader[1].ToString().Replace(",", ".") + "," + //Цена
                                  "1,'" +
                                  reader[3].ToString() + "','" +
                                  reader[4].ToString() + "'," +
                                  (Math.Round(Convert.ToDecimal(reader[1].ToString()) - Convert.ToDecimal(reader[1].ToString()) * persent / 100, 2)).ToString().Replace(",", ".") + //Цена со скидкой            
                                  ");");
                                multiplication_factor--;
                            }
                            else
                            {
                                query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid,retail_price_discount)VALUES(" +
                                                         reader[0].ToString() + "," +
                                                         reader[1].ToString().Replace(",", ".") + "," +
                                                         "1,'" +
                                                        reader[3].ToString() + "','" +
                                                        reader[4].ToString() + "'," +
                                                        reader[1].ToString().Replace(",", ".") + ");");
                            }

                        }
                        else
                        {
                            query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid,retail_price_discount)VALUES(" +
                                                        reader[0].ToString() + "," +
                                                        reader[1].ToString().Replace(",", ".") + "," +
                                                        "1,'" +
                                                       reader[3].ToString() + "','" +
                                                       reader[4].ToString() + "'," +
                                                       reader[1].ToString().Replace(",", ".") + ");");
                        }
                        num_records++;
                    }

                    command = new NpgsqlCommand(query.ToString(), conn);
                    command.ExecuteNonQuery();

                    query_string = " SELECT tovar_action.code,tovar.name, tovar_action.retail_price,tovar_action.retail_price_discount ," +
                    " SUM(quantity),characteristic_name,characteristic_guid FROM tovar_action " +
                    " LEFT JOIN tovar ON tovar_action.code=tovar.code " +
                    " GROUP BY tovar_action.code,tovar.name, tovar.retail_price,tovar_action.retail_price,characteristic_name,characteristic_guid," +
                    " retail_price_discount";

                    command = new NpgsqlCommand(query_string, conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["tovar_code"] = reader[0].ToString();
                        row["tovar_name"] = reader[1].ToString().Trim();
                        row["characteristic_name"] = reader[5].ToString();
                        row["characteristic_code"] = reader[6].ToString();
                        row["quantity"] = reader[4].ToString().Trim();
                        row["price"] = reader.GetDecimal(2).ToString();
                        row["price_at_discount"] = reader.GetDecimal(3).ToString();
                        row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
                        row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
                        if (Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"]))
                        {
                            row["action"] = num_doc.ToString();
                        }
                        else
                        {
                            row["action"] = "0";
                        }
                        row["gift"] = "0";
                        row["action2"] = num_doc.ToString();
                        row["bonus_reg"] = 0;
                        row["bonus_action"] = 0;
                        row["bonus_action_b"] = 0;
                        row["marking"] = "0";
                        dt.Rows.Add(row);
                        //SendDataToCustomerScreen(1, 0);
                    }

                    /*акция сработала
             * надо отметить все товарные позиции 
             * чтобы они не участвовали в других акциях 
             */
                    marked_action_tovar_dt(num_doc, comment);
                }

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " Ошибка при обработке 4 типа акций ");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// в документе >= сумме(количество) товаров в акции  
        /// тогда дается скидка на кратное количество товара
        /// на самый дешевый товар из участвующих в акции 
        /// здесь метод без обращения к бд
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="percent"></param>
        /// <param name="sum"></param>
        /// <param name="comment"></param>
        /// <param name="actionPricesByDoc"></param>
        private void action_4_dt(int num_doc,
                                decimal percent,
                                decimal sum,
                                string comment,
                                Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            // Проверка на целочисленность sum
            if (sum != Math.Floor(sum))
                throw new ArgumentException("Параметр 'sum' должен быть целым числом");

            // Сохраняем оригинальные данные для возможного отката
            DataTable originalDt = dt.Copy();
            DataTable tempDt = dt.Clone();

            try
            {
                // 1. Переносим строки, не участвующие в акции
                foreach (DataRow row in originalDt.Rows)
                {
                    if (row.Field<int>("action2") > 0 ||
                        !IsTovarInAction(actionPricesByDoc, num_doc, (long)row.Field<double>("tovar_code")))
                    {
                        tempDt.ImportRow(row);
                    }
                }

                // 2. Подготовка данных для обработки
                var items = new List<ItemData>();
                foreach (DataRow row in originalDt.Rows)
                {
                    if (row.Field<int>("action2") > 0) continue;

                    long tovarCode = (long)row.Field<double>("tovar_code"); // Преобразуем double в long
                    if (!IsTovarInAction(actionPricesByDoc, num_doc, tovarCode)) continue;

                    items.Add(new ItemData
                    {
                        Code = row.Field<double>("tovar_code"),
                        TovarName = row.Field<string>("tovar_name"), // Сохраняем наименование товара
                        CharName = row.Field<string>("characteristic_name"),
                        CharGuid = row.Field<string>("characteristic_code"),
                        Price = row.Field<decimal>("price"),
                        Quantity = row.Field<double>("quantity")
                    });
                }

                // 3. Обработка и группировка
                var processedItems = ProcessItems(items, num_doc, percent, (int)sum); // Преобразуем sum в int

                // 4. Заполнение таблицы
                dt.BeginLoadData();
                try
                {
                    dt.Clear();

                    // Добавляем обработанные элементы
                    foreach (var group in processedItems)
                    {
                        DataRow newRow = dt.NewRow();
                        FillDataRow(newRow, group, num_doc);
                        dt.Rows.Add(newRow);
                    }

                    // Добавляем неизмененные строки
                    foreach (DataRow row in tempDt.Rows)
                    {
                        dt.ImportRow(row);
                    }
                }
                finally
                {
                    dt.EndLoadData();
                }

                // 5. Отмечаем товары, участвовавшие в акции
                marked_action_tovar_dt(dt,num_doc, comment, actionPricesByDoc);
            }
            catch (Exception ex)
            {
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_4_dt(int num_doc, decimal percent, decimal sum, string comment, Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)", num_doc, MainStaticClass.CashDeskNumber, "Акция 4 типа без обращения к бд");
                // Восстановление данных при ошибке
                dt.Clear();
                foreach (DataRow row in originalDt.Rows)
                {
                    dt.ImportRow(row);
                }
                throw;
            }
            finally
            {
                originalDt.Dispose();
                tempDt.Dispose();
            }
        }


       

        // Вспомогательные классы
        private class ItemData
        {
            public double Code { get; set; }
            public string TovarName { get; set; } // Наименование товара
            public string CharName { get; set; }  // Название характеристики
            public string CharGuid { get; set; }  // Идентификатор характеристики
            public decimal Price { get; set; }    // Цена товара
            public double Quantity { get; set; }  // Количество товара
        }

        private class GroupedItem
        {
            public double Code { get; set; }
            public string TovarName { get; set; } // Наименование товара
            public string CharName { get; set; }  // Название характеристики
            public string CharGuid { get; set; }  // Идентификатор характеристики
            public decimal Price { get; set; }    // Цена товара
            public decimal Discount { get; set; } // Цена со скидкой
            public double Count { get; set; }     // Количество товара
            public decimal SumFull { get; set; }  // Сумма без скидки
            public decimal SumDiscount { get; set; } // Сумма со скидкой
            public int Action { get; set; }       // Флаг акции
            public int Gift { get; set; }         //флаг подарка     
        }

        private List<GroupedItem> ProcessItems(List<ItemData> items, int num_doc, decimal percent, int sum)
        {
            var flatItems = new List<ItemData>();
            foreach (var item in items)
            {
                // Проверка на целочисленность Quantity
                if (item.Quantity != Math.Floor(item.Quantity))
                    throw new ArgumentException("Количество должно быть целым числом");

                int quantity = (int)item.Quantity;

                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(new ItemData
                    {
                        Code = item.Code,
                        TovarName = item.TovarName, // Сохраняем наименование товара
                        CharName = item.CharName,
                        CharGuid = item.CharGuid,
                        Price = item.Price,
                        Quantity = 1.0
                    });
                }
            }

            // Сортировка по цене (от меньшего к большему)
            flatItems.Sort((a, b) => a.Price.CompareTo(b.Price));

            var result = new List<GroupedItem>();
            for (int i = 0; i < flatItems.Count; i++)
            {
                var item = flatItems[i];
                bool applyDiscount = (i % sum) == 0 && flatItems.Count >= sum; // Скидка применяется только при наличии достаточного количества товаров

                decimal discount = applyDiscount
                    ? Math.Round(item.Price * (1 - percent / 100m), 2)
                    : item.Price;

                // Создаем новый GroupedItem для каждого товара
                var groupedItem = new GroupedItem
                {
                    Code = item.Code,
                    TovarName = item.TovarName, // Сохраняем наименование товара
                    CharName = item.CharName,
                    CharGuid = item.CharGuid,
                    Price = item.Price,
                    Discount = discount,
                    Action = applyDiscount ? num_doc : 0,
                    Count = 1.0,
                    SumFull = item.Price,
                    SumDiscount = discount
                };

                result.Add(groupedItem);
            }

            return result;
        }
        private void FillDataRow(DataRow row, GroupedItem item, int num_doc)
        {
            row["tovar_code"] = item.Code;
            row["tovar_name"] = item.TovarName ?? string.Empty; // Заполняем наименование товара
            row["characteristic_name"] = item.CharName ?? string.Empty;
            row["characteristic_code"] = item.CharGuid ?? string.Empty;
            row["quantity"] = item.Count;
            row["price"] = item.Price;
            row["price_at_discount"] = item.Discount;
            row["sum_full"] = item.SumFull;
            row["sum_at_discount"] = item.SumDiscount;
            row["action"] = item.Action;
            row["action2"] = num_doc;
            row["gift"] = 0;
            row["bonus_reg"] = 0m;
            row["bonus_action"] = 0m;
            row["bonus_action_b"] = 0m;
            row["marking"] = "0";
        }

        private bool IsTovarInAction(
            Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc,
            int num_doc,
            long tovarCode)
        {
            return actionPricesByDoc.TryGetValue(num_doc, out var docPrices)
                   && docPrices.ContainsKey(tovarCode);
        }


        ///// <summary>
        ///// Эта акция срабатывает, когда количество товаров в документе >= сумме (количеству) товаров в акции.
        ///// Тогда дается скидка на кратное количество товара на самый дешевый товар из участвующих в акции.
        ///// </summary>
        ///// <param name="num_doc">Номер документа акции.</param>
        ///// <param name="percent">Процент скидки.</param>
        ///// <param name="sum">Сумма (количество) товаров в акции.</param>
        ///// <param name="comment">Комментарий к акции.</param>
        //private void action_4_dt(int num_doc, decimal percent, decimal sum, string comment)
        //{
        //    if (!create_temp_tovar_table_4())
        //    {
        //        return;
        //    }

        //    DataTable dt2 = dt.Copy();
        //    dt2.Rows.Clear();
        //    decimal quantity_on_doc = 0; // Количество позиций в документе
        //    StringBuilder query = new StringBuilder();

        //    try
        //    {
        //        using (var conn = MainStaticClass.NpgsqlConn())
        //        {
        //            conn.Open();

        //            // Шаг 1: Фильтруем товары и рассчитываем общее количество
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                if (Convert.ToInt32(row["action2"]) > 0) // Пропускаем товары, уже участвовавшие в акции
        //                {
        //                    dt2.Rows.Add(row.ItemArray);
        //                    continue;
        //                }

        //                if (IsTovarInAction(conn, num_doc, row["tovar_code"].ToString()))
        //                {
        //                    // Добавляем товары во временную таблицу
        //                    AddTovarToTempTable(query, row, Convert.ToInt32(row["quantity"]));
        //                    quantity_on_doc += Convert.ToDecimal(row["quantity"]);
        //                }
        //                else
        //                {
        //                    // Товары, не участвующие в акции, добавляем в dt2
        //                    dt2.Rows.Add(row.ItemArray);
        //                }
        //            }

        //            // Шаг 2: Проверяем условие акции
        //            if (quantity_on_doc >= sum)
        //            {
        //                have_action = true; // Признак того, что в документе есть сработка по акции

        //                // Шаг 3: Очищаем dt и добавляем строки из dt2
        //                dt.Rows.Clear();
        //                foreach (DataRow row2 in dt2.Rows)
        //                {
        //                    dt.Rows.Add(row2.ItemArray);
        //                }

        //                // Шаг 4: Выполняем запросы к временной таблице
        //                ExecuteTempTableQueries(conn, query, num_doc, percent, quantity_on_doc, sum);

        //                // Шаг 5: Обновляем DataTable с учетом скидок
        //                UpdateDataTableWithDiscounts(conn, num_doc);

        //                // Шаг 6: Отмечаем товары, участвовавшие в акции
        //                marked_action_tovar_dt(num_doc, comment);
        //            }
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        //Logger.LogError(ex, "Ошибка при работе с базой данных");
        //        MessageBox.Show("Произошла ошибка при работе с базой данных."+ex.Message, " Ошибка при обработке 4 типа акций ");
        //        MainStaticClass.WriteRecordErrorLog(ex.Message, "action_4_dt(int num_doc, decimal percent, decimal sum, string comment)", num_doc, MainStaticClass.CashDeskNumber, "Обработка акций 4 типа,скидка");
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logger.LogError(ex, "Ошибка при обработке акции");
        //        MessageBox.Show("Произошла ошибка при обработке акции."+ex.Message, " Ошибка при обработке 4 типа акций ");
        //        MainStaticClass.WriteRecordErrorLog(ex.Message, "action_4_dt(int num_doc, decimal percent, decimal sum, string comment)", num_doc, MainStaticClass.CashDeskNumber, "Обработка акций 4 типа,скидка");
        //    }
        //}

        ///// <summary>
        ///// Проверяет, участвует ли товар в акции.
        ///// </summary>
        //private bool IsTovarInAction(NpgsqlConnection conn, int num_doc, string tovar_code)
        //{
        //    string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar = @code_tovar AND num_doc = @num_doc";
        //    using (var command = new NpgsqlCommand(query, conn))
        //    {
        //        command.Parameters.AddWithValue("@code_tovar", tovar_code);
        //        command.Parameters.AddWithValue("@num_doc", num_doc);
        //        return Convert.ToInt16(command.ExecuteScalar()) != 0;
        //    }
        //}

        ///// <summary>
        ///// Добавляет товары во временную таблицу.
        ///// </summary>
        //private void AddTovarToTempTable(StringBuilder query, DataRow row, int quantity)
        //{
        //    for (int i = 0; i < quantity; i++)
        //    {
        //        query.Append("INSERT INTO tovar_action(code, retail_price, quantity, characteristic_name, characteristic_guid) VALUES (")
        //            .Append(row["tovar_code"].ToString()).Append(",")
        //            .Append(row["price"].ToString().Replace(",", ".")).Append(",")
        //            .Append("1,'")
        //            .Append(row["characteristic_name"].ToString()).Append("','")
        //            .Append(row["characteristic_code"].ToString()).Append("');");
        //    }
        //}

        ///// <summary>
        ///// Выполняет запросы к временной таблице.
        ///// </summary>
        //private void ExecuteTempTableQueries(NpgsqlConnection conn, StringBuilder query, int num_doc, decimal percent, decimal quantity_on_doc, decimal sum)
        //{
        //    using (var command = new NpgsqlCommand(query.ToString(), conn))
        //    {
        //        command.ExecuteNonQuery();
        //    }

        //    query.Clear();
        //    query.Append("DELETE FROM tovar_action;"); // Очищаем таблицу акционных товаров

        //    int multiplication_factor = (int)(quantity_on_doc / sum);
        //    string query_string = "SELECT code, retail_price, quantity, characteristic_name, characteristic_guid FROM tovar_action ORDER BY retail_price DESC";
        //    using (var command = new NpgsqlCommand(query_string, conn))
        //    using (var reader = command.ExecuteReader())
        //    {
        //        int num_records = 1;
        //        while (reader.Read())
        //        {
        //            decimal retail_price = reader.GetDecimal(1);
        //            decimal discounted_price = retail_price;

        //            if (multiplication_factor > 0 && num_records % sum == 0)
        //            {
        //                discounted_price = Math.Round(retail_price - retail_price * percent / 100, 2);
        //                multiplication_factor--;
        //            }

        //            query.Append("INSERT INTO tovar_action(code, retail_price, quantity, characteristic_name, characteristic_guid, retail_price_discount) VALUES (")
        //                .Append(reader[0].ToString()).Append(",")
        //                .Append(retail_price.ToString().Replace(",", ".")).Append(",")
        //                .Append("1,'")
        //                .Append(reader[3].ToString()).Append("','")
        //                .Append(reader[4].ToString()).Append("',")
        //                .Append(discounted_price.ToString().Replace(",", ".")).Append(");");

        //            num_records++;
        //        }
        //    }

        //    using (var command = new NpgsqlCommand(query.ToString(), conn))
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //}

        ///// <summary>
        ///// Обновляет DataTable с учетом скидок.
        ///// </summary>
        //private void UpdateDataTableWithDiscounts(NpgsqlConnection conn, int num_doc)
        //{
        //    string query_string = @"
        //SELECT tovar_action.code, tovar.name, tovar_action.retail_price, tovar_action.retail_price_discount, 
        //       SUM(quantity), characteristic_name, characteristic_guid 
        //FROM tovar_action 
        //LEFT JOIN tovar ON tovar_action.code = tovar.code 
        //GROUP BY tovar_action.code, tovar.name, tovar.retail_price, tovar_action.retail_price, characteristic_name, characteristic_guid, retail_price_discount";

        //    using (var command = new NpgsqlCommand(query_string, conn))
        //    using (var reader = command.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            DataRow row = dt.NewRow();
        //            row["tovar_code"] = reader[0].ToString();
        //            row["tovar_name"] = reader[1].ToString().Trim();
        //            row["characteristic_name"] = reader[5].ToString();
        //            row["characteristic_code"] = reader[6].ToString();
        //            row["quantity"] = reader[4].ToString().Trim();
        //            row["price"] = reader.GetDecimal(2).ToString();
        //            row["price_at_discount"] = reader.GetDecimal(3).ToString();
        //            row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
        //            row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
        //            row["action"] = (Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"])) ? num_doc.ToString() : "0";
        //            row["gift"] = "0";
        //            row["action2"] = num_doc.ToString();
        //            row["bonus_reg"] = 0;
        //            row["bonus_action"] = 0;
        //            row["bonus_action_b"] = 0;
        //            row["marking"] = "0";
        //            dt.Rows.Add(row);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Эта акция срабатывает, когда количество товаров в документе >= сумме (количеству) товаров в акции.
        ///// Тогда дается скидка на кратное количество товара на самый дешевый товар из участвующих в акции.
        ///// </summary>
        ///// <param name="num_doc">Номер документа акции.</param>
        ///// <param name="percent">Процент скидки.</param>
        ///// <param name="sum">Сумма (количество) товаров в акции.</param>
        ///// <param name="comment">Комментарий к акции.</param>
        //private void action_4_dt(int num_doc, decimal percent, decimal sum, string comment)
        //{
        //    if (!create_temp_tovar_table_4())
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        using (var conn = MainStaticClass.NpgsqlConn())
        //        {
        //            conn.Open();

        //            // Шаг 1: Собираем товары, участвующие в акции
        //            var tovarCodesInAction = GetTovarCodesInAction(conn, num_doc);

        //            // Шаг 2: Фильтруем строки DataTable для товаров, участвующих в акции
        //            var eligibleRows = new List<DataRow>();
        //            decimal totalQuantity = 0;

        //            foreach (DataRow row in dt.Rows)
        //            {
        //                if (Convert.ToInt32(row["action2"]) > 0) // Пропускаем товары, уже участвовавшие в акции
        //                {
        //                    continue;
        //                }

        //                long tovarCode = Convert.ToInt64(row["tovar_code"]);
        //                if (tovarCodesInAction.Contains(tovarCode))
        //                {
        //                    eligibleRows.Add(row);
        //                    totalQuantity += Convert.ToDecimal(row["quantity"]); // Суммируем общее количество
        //                }
        //            }

        //            // Шаг 3: Проверяем условие акции
        //            if (totalQuantity < sum)
        //            {
        //                // Если общее количество меньше требуемого, ничего не делаем
        //                return;
        //            }

        //            have_action = true; // Признак того, что в документе есть сработка по акции

        //            // Шаг 4: Рассчитываем количество товаров для скидки
        //            int multiplicationFactor = (int)(totalQuantity / sum); // Количество кратных групп
        //            int discountQuantity = (int)sum * multiplicationFactor; // Общее количество товаров со скидкой

        //            // Шаг 5: Находим самый дешевый товар среди участвующих
        //            var cheapestRow = eligibleRows.OrderBy(row => Convert.ToDecimal(row["price"])).First();
        //            decimal originalPrice = Convert.ToDecimal(cheapestRow["price"]);
        //            decimal discountedPrice = Math.Round(originalPrice - originalPrice * percent / 100, 2);

        //            // Шаг 6: Определяем, сколько единиц самого дешевого товара можно "зачесть" под скидку
        //            int quantityOfCheapest = Convert.ToInt32(cheapestRow["quantity"]);

        //            if (quantityOfCheapest <= discountQuantity)
        //            {
        //                // Если всего самого дешевого товара достаточно для скидки
        //                ApplyDiscountToRow(cheapestRow, discountedPrice, originalPrice, quantityOfCheapest, num_doc);
        //                discountQuantity -= quantityOfCheapest; // Уменьшаем оставшееся количество для скидки
        //            }
        //            else
        //            {
        //                // Если самого дешевого товара больше, чем нужно для скидки
        //                SplitRowWithDiscount(cheapestRow, discountedPrice, originalPrice, sum, percent, num_doc);
        //                discountQuantity -= (int)sum; // Уменьшаем оставшееся количество для скидки
        //            }

        //            // Шаг 7: Применяем скидку к остальным товарам, если необходимо
        //            foreach (DataRow row in eligibleRows)
        //            {
        //                if (discountQuantity <= 0) break; // Если скидка уже полностью применена

        //                int rowQuantity = Convert.ToInt32(row["quantity"]);
        //                if (rowQuantity <= discountQuantity)
        //                {
        //                    ApplyDiscountToRow(row, discountedPrice, originalPrice, rowQuantity, num_doc);
        //                    discountQuantity -= rowQuantity; // Уменьшаем оставшееся количество для скидки
        //                }
        //                else
        //                {
        //                    SplitRowWithDiscount(row, discountedPrice, originalPrice, sum, percent, num_doc);
        //                    discountQuantity -= (int)sum; // Уменьшаем оставшееся количество для скидки
        //                }
        //            }

        //            // Шаг 8: Отмечаем товары, участвовавшие в акции
        //            marked_action_tovar_dt(num_doc, comment);
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        //Logger.LogError(ex, "Ошибка при работе с базой данных");
        //        MessageBox.Show("Произошла ошибка при работе с базой данных.");
        //    }
        //    catch (Exception ex)
        //    {
        //        //Logger.LogError(ex, "Ошибка при обработке акции");
        //        MessageBox.Show("Произошла ошибка при обработке акции.");
        //    }
        //}

        ///// <summary>
        ///// Применяет скидку к строке полностью.
        ///// </summary>
        //private void ApplyDiscountToRow(DataRow row, decimal discountedPrice, decimal originalPrice, int quantity, int num_doc)
        //{
        //    row["price_at_discount"] = discountedPrice.ToString();
        //    row["sum_full"] = (quantity * originalPrice).ToString();
        //    row["sum_at_discount"] = (quantity * discountedPrice).ToString();
        //    row["action"] = num_doc.ToString();
        //    row["action2"] = num_doc.ToString();
        //}

        ///// <summary>
        ///// Разбивает строку на две части: одну со скидкой, другую без скидки.
        ///// </summary>
        //private void SplitRowWithDiscount(DataRow row, decimal discountedPrice, decimal originalPrice, decimal sum, decimal percent, int num_doc)
        //{
        //    int rowQuantity = Convert.ToInt32(row["quantity"]);
        //    int discountedUnits = (int)sum; // Количество единиц, на которые дается скидка
        //    int remainingUnits = rowQuantity - discountedUnits; // Оставшееся количество без скидки

        //    // Создаем новую строку для оставшихся единиц без скидки
        //    DataRow newRow = dt.NewRow();
        //    newRow.ItemArray = row.ItemArray;
        //    newRow["quantity"] = remainingUnits.ToString();
        //    newRow["price_at_discount"] = originalPrice.ToString();
        //    newRow["sum_full"] = (remainingUnits * originalPrice).ToString();
        //    newRow["sum_at_discount"] = (remainingUnits * originalPrice).ToString();
        //    newRow["action"] = "0"; // Без скидки
        //    newRow["action2"] = "0"; // Без скидки
        //    dt.Rows.Add(newRow);

        //    // Обновляем текущую строку для единиц со скидкой
        //    row["quantity"] = discountedUnits.ToString();
        //    row["price_at_discount"] = discountedPrice.ToString();
        //    row["sum_full"] = (discountedUnits * originalPrice).ToString();
        //    row["sum_at_discount"] = (discountedUnits * discountedPrice).ToString();
        //    row["action"] = num_doc.ToString();
        //    row["action2"] = num_doc.ToString();
        //}

        //private void Action4Dt(int numDoc, decimal percent, decimal sum, string comment)
        //{
        //    if (!create_temp_tovar_table_4())
        //    {
        //        return;
        //    }

        //    DataTable dt2 = dt.Copy();
        //    dt2.Rows.Clear();
        //    decimal quantityOnDoc = 0;
        //    StringBuilder query = new StringBuilder();

        //    try
        //    {
        //        using (var conn = MainStaticClass.NpgsqlConn())
        //        {
        //            conn.Open();

        //            foreach (DataRow row in dt.Rows)
        //            {
        //                if (Convert.ToInt32(row["action2"]) > 0)
        //                {
        //                    dt2.ImportRow(row);
        //                    continue;
        //                }

        //                string queryString = $"SELECT COUNT(*) FROM action_table WHERE code_tovar={row["tovar_code"]} AND num_doc={numDoc}";
        //                using (var command = new NpgsqlCommand(queryString, conn))
        //                {
        //                    if (Convert.ToInt16(command.ExecuteScalar()) != 0)
        //                    {
        //                        for (int i = 0; i < Convert.ToInt32(row["quantity"]); i++)
        //                        {
        //                            query.AppendLine($"INSERT INTO tovar_action(code, retail_price, quantity, characteristic_name, characteristic_guid) VALUES({row["tovar_code"]}, {row["price"].ToString().Replace(",", ".")}, 1, '{row["characteristic_name"]}', '{row["characteristic_code"]}');");
        //                        }
        //                        quantityOnDoc += Convert.ToDecimal(row["quantity"]);
        //                    }
        //                    else
        //                    {
        //                        dt2.ImportRow(row);
        //                    }
        //                }
        //            }

        //            if (quantityOnDoc >= sum)
        //            {
        //                have_action = true;
        //                dt.Rows.Clear();
        //                dt.Merge(dt2);

        //                using (var command = new NpgsqlCommand(query.ToString(), conn))
        //                {
        //                    command.ExecuteNonQuery();
        //                }

        //                query.Clear();
        //                query.AppendLine("DELETE FROM tovar_action;");

        //                int multiplicationFactor = (int)(quantityOnDoc / sum);
        //                string selectQuery = "SELECT code, retail_price, quantity, characteristic_name, characteristic_guid FROM tovar_action ORDER BY retail_price DESC";
        //                using (var command = new NpgsqlCommand(selectQuery, conn))
        //                using (var reader = command.ExecuteReader())
        //                {
        //                    int numRecords = 1;
        //                    while (reader.Read())
        //                    {
        //                        decimal retailPrice = reader.GetDecimal(1);
        //                        decimal retailPriceDiscount = retailPrice;

        //                        if (multiplicationFactor > 0 && numRecords % sum == 0)
        //                        {
        //                            retailPriceDiscount = Math.Round(retailPrice - retailPrice * percent / 100, 2);
        //                            multiplicationFactor--;
        //                        }

        //                        query.AppendLine($"INSERT INTO tovar_action(code, retail_price, quantity, characteristic_name, characteristic_guid, retail_price_discount) VALUES({reader["code"]}, {retailPrice.ToString().Replace(",", ".")}, 1, '{reader["characteristic_name"]}', '{reader["characteristic_guid"]}', {retailPriceDiscount.ToString().Replace(",", ".")});");
        //                        numRecords++;
        //                    }
        //                }

        //                using (var command = new NpgsqlCommand(query.ToString(), conn))
        //                {
        //                    command.ExecuteNonQuery();
        //                }

        //                string finalQuery = @"
        //                                    SELECT tovar_action.code, tovar.name, tovar_action.retail_price, tovar_action.retail_price_discount, SUM(quantity), characteristic_name, characteristic_guid 
        //                                    FROM tovar_action 
        //                                    LEFT JOIN tovar ON tovar_action.code = tovar.code 
        //                                    GROUP BY tovar_action.code, tovar.name, tovar.retail_price, tovar_action.retail_price, characteristic_name, characteristic_guid, retail_price_discount";

        //                using (var command = new NpgsqlCommand(finalQuery, conn))
        //                using (var reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        DataRow row = dt.NewRow();
        //                        row["tovar_code"] = reader["code"];
        //                        row["tovar_name"] = reader["name"].ToString().Trim();
        //                        row["characteristic_name"] = reader["characteristic_name"];
        //                        row["characteristic_code"] = reader["characteristic_guid"];
        //                        row["quantity"] = reader["sum"].ToString().Trim();
        //                        row["price"] = reader.GetDecimal(2).ToString();
        //                        row["price_at_discount"] = reader.GetDecimal(3).ToString();
        //                        row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
        //                        row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
        //                        row["action"] = Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"]) ? numDoc.ToString() : "0";
        //                        row["gift"] = "0";
        //                        row["action2"] = numDoc.ToString();
        //                        row["bonus_reg"] = 0;
        //                        row["bonus_action"] = 0;
        //                        row["bonus_action_b"] = 0;
        //                        row["marking"] = "0";
        //                        dt.Rows.Add(row);
        //                    }
        //                }

        //                marked_action_tovar_dt(numDoc, comment);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Ошибка при обработке 4 типа акций");
        //    }
        //}

        /// <summary>
        /// Возвращает цену подарка
        /// </summary>
        /// <param name="num_doc"></param>
        /// <returns></returns>
        private string get_price_action(int num_doc)
        {
            string result = "";

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

            try
            {

                conn.Open();
                string query = "SELECT action_header.tip, action_header.gift_price, action_header.marker  FROM action_header where action_header.num_doc=" + num_doc.ToString();
                NpgsqlCommand command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if ((Convert.ToInt16(reader["tip"]) == 1) || (Convert.ToInt16(reader["tip"]) == 2) || (Convert.ToInt16(reader["tip"]) == 3) || (Convert.ToInt16(reader["tip"]) == 4) || (Convert.ToInt16(reader["tip"]) == 5) || (Convert.ToInt16(reader["tip"]) == 6) || (Convert.ToInt16(reader["tip"]) == 8))
                    {
                        //if (Convert.ToInt16(reader["marker"]) == 1)//запрашивать подарок
                        //{
                        result = reader["gift_price"].ToString();//получить розничную цену подарка
                        //}
                    }
                }
                reader.Close();
                conn.Close();
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

            return result;
        }

        /*Эта акция срабатывает когда количество товаров в документе >= сумме(количество) товаров в акции
        * тогда выдается сообщение о подарке
        * самый дешевый товар из документа дается в подарок кратное число единиц 
        * и еще добавляется некий товар из акционного документа         
        */
        //private void action_4(int num_doc, string comment, decimal sum, long code_tovar)
        private void action_4_dt(int num_doc, string comment, decimal sum,bool show_messages)
        {
            if (!create_temp_tovar_table_4()) //Создать временную таблицу для акционного товара
            {
                return;
            }

            DataTable dt2 = dt.Copy();
            dt2.Rows.Clear();
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            decimal quantity_on_doc = 0;//количество позиций в документе            
            ListView clon = new ListView();
            StringBuilder query = new StringBuilder();
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query_string = "";

                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
                    {
                        DataRow row2 = dt2.NewRow();
                        row2.ItemArray = row.ItemArray;
                        dt2.Rows.Add(row2);
                        continue;
                    }
                    query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query_string, conn);
                    if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                    {

                        for (int i = 0; i < Convert.ToInt32(row["quantity"]); i++)
                        {
                            query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid)VALUES(" +
                                row["tovar_code"].ToString() + "," +
                                row["price"].ToString().Replace(",", ".") + "," +
                               "1,'" +
                               row["characteristic_name"].ToString() + "','" +
                               row["characteristic_code"].ToString() + "');");
                        }
                        quantity_on_doc += Convert.ToDecimal(row["quantity"]);
                    }
                    else//Не участвует в акции убираем пока в сторону
                    {
                        DataRow row2 = dt2.NewRow();
                        row2.ItemArray = row.ItemArray;
                        dt2.Rows.Add(row2);
                    }
                }

                if (quantity_on_doc >= sum)//Есть вхождение в акцию
                {
                    have_action = true;//Признак того что в документе есть сработка по акции                    
                    dt.Rows.Clear();
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        DataRow row = dt.NewRow();
                        row.ItemArray = row2.ItemArray;
                        dt.Rows.Add(row);
                    }
                    command = new NpgsqlCommand(query.ToString(), conn);//устанавливаем акционные позиции во временную таблицу
                    command.ExecuteNonQuery();
                    query.Append("DELETE FROM tovar_action;");//Очищаем таблицу акционных товаров 
                    //иначе результат задваивается ранее эта строка была закомментирована и при 2 товарах по 1 шт. учавстсующих в акции
                    //работала неверно
                    int multiplication_factor = (int)(quantity_on_doc / sum);
                    //query_string = " SELECT code, retail_price, quantity,characteristic_name,characteristic_guid FROM tovar_action ORDER BY retail_price desc";//запросим товары отсортированные по цене, теперь еще и с характеристиками
                    query_string = " SELECT tovar_action.code,tovar.name, tovar_action.retail_price,tovar_action.retail_price,tovar_action.quantity,tovar_action.characteristic_name,tovar_action.characteristic_guid " +
                       " FROM tovar_action LEFT JOIN tovar ON tovar_action.code=tovar.code " +
                       //" LEFT JOIN characteristic ON tovar_action.characteristic_guid = characteristic.guid " +
                       " order by tovar_action.retail_price desc ";
                    command = new NpgsqlCommand(query_string, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    Decimal sum_on_string = sum;

                    int num_records = 1;
                    while (reader.Read())
                    {
                        if (multiplication_factor > 0)//Это количество позиций которые будут выданы в качестве подарка
                        {
                            if ((decimal)num_records / sum == Math.Round(num_records / sum, 0, MidpointRounding.AwayFromZero))
                            {
                                DataRow row = dt.NewRow();
                                row["tovar_code"] = reader[0].ToString();
                                row["tovar_name"] = reader[1].ToString().Trim();
                                row["characteristic_name"] = reader[5].ToString();
                                row["characteristic_code"] = reader[6].ToString();
                                row["quantity"] = 1;
                                row["price"] = reader.GetDecimal(2).ToString();
                                row["price_at_discount"] = get_price_action(num_doc);
                                row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
                                row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
                                //if (Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"]))
                                //{
                                //    row["action"] = num_doc.ToString();
                                //}
                                //else
                                //{
                                    row["action"] = "0";
                                //}
                                //row["gift"] = "0";
                                row["gift"] = num_doc.ToString();
                                row["action2"] = num_doc.ToString();
                                row["bonus_reg"] = 0;
                                row["bonus_action"] = 0;
                                row["bonus_action_b"] = 0;
                                row["marking"] = "0";
                                dt.Rows.Add(row);
                                multiplication_factor--;
                            }
                            else
                            {
                                DataRow row = dt.NewRow();
                                row["tovar_code"] = reader[0].ToString();
                                row["tovar_name"] = reader[1].ToString().Trim();
                                row["characteristic_name"] = reader[5].ToString();
                                row["characteristic_code"] = reader[6].ToString();
                                row["quantity"] = 1;
                                row["price"] = reader.GetDecimal(2).ToString();
                                row["price_at_discount"] = reader.GetDecimal(3).ToString();
                                row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
                                row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
                                if (Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"]))
                                {
                                    row["action"] = num_doc.ToString();
                                }
                                else
                                {
                                    row["action"] = "0";
                                }
                                row["gift"] = "0";
                                //row["gift"] = num_doc.ToString();
                                row["action2"] = num_doc.ToString();
                                row["bonus_reg"] = 0;
                                row["bonus_action"] = 0;
                                row["bonus_action_b"] = 0;
                                row["marking"] = "0";
                                dt.Rows.Add(row);
                            }
                        }
                        else
                        {
                            DataRow row = dt.NewRow();
                            row["tovar_code"] = reader[0].ToString();
                            row["tovar_name"] = reader[1].ToString().Trim();
                            row["characteristic_name"] = reader[3].ToString();
                            row["characteristic_code"] = reader[4].ToString();
                            row["quantity"] = 1;
                            row["price"] = reader.GetDecimal(2).ToString();
                            row["price_at_discount"] = reader.GetDecimal(3).ToString();
                            row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
                            row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
                            if (Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"]))
                            {
                                row["action"] = num_doc.ToString();
                            }
                            else
                            {
                                row["action"] = "0";
                            }
                            row["gift"] = "0";
                            //row["gift"] = num_doc.ToString();
                            row["action2"] = num_doc.ToString();
                            row["bonus_reg"] = 0;
                            row["bonus_action"] = 0;
                            row["bonus_action_b"] = 0;
                            row["marking"] = "0";
                            dt.Rows.Add(row);
                        }
                        num_records++;
                    }
                    /*акция сработала
             * надо отметить все товарные позиции 
             * чтобы они не участвовали в других акциях 
             */
                    marked_action_tovar_dt(num_doc,comment);
                }
                roll_up_dt();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 4 типа акций");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }


        /// <summary>
        /// Эта акция срабатывает, когда количество товаров в документе >= сумме (количество) товаров в акции.
        /// Тогда выдается сообщение о подарке. Самый дешевый товар из документа дается в подарок кратное число единиц,
        /// и еще добавляется некий товар из акционного документа. Метод работает без обращения к базе данных.
        /// </summary>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="comment">Комментарий к акции.</param>
        /// <param name="sum">Количество товаров, необходимое для срабатывания акции.</param>
        /// <param name="show_messages">Флаг, указывающий, нужно ли показывать сообщения.</param>
        /// <param name="actionPricesByDoc">Словарь с ценами товаров по документам акций.</param>
        private void action_4_dt(int num_doc,
                         string comment,
                         decimal sum,
                         bool show_messages,
                         Dictionary<int, Dictionary<long, decimal>> actionPricesByDoc)
        {
            if (sum != Math.Floor(sum))
                throw new ArgumentException("Параметр 'sum' должен быть целым числом");

            // Создаем копию исходной таблицы
            DataTable originalDt = dt.Copy();
            DataTable tempDt = dt.Clone(); // Временная таблица для обработки данных

            try
            {
                // Фильтрация строк, которые не участвуют в акции
                foreach (DataRow row in originalDt.Rows)
                {
                    if (row.Field<int>("action2") > 0 ||
                        !IsTovarInAction(actionPricesByDoc, num_doc, (long)row.Field<double>("tovar_code")))
                    {
                        tempDt.ImportRow(row);
                    }
                }

                // Сбор данных о товарах, участвующих в акции
                var items = new List<ItemData>();
                foreach (DataRow row in originalDt.Rows)
                {
                    if (row.Field<int>("action2") > 0) continue;

                    long tovarCode = (long)row.Field<double>("tovar_code");
                    if (!IsTovarInAction(actionPricesByDoc, num_doc, tovarCode)) continue;

                    items.Add(new ItemData
                    {
                        Code = row.Field<double>("tovar_code"),
                        TovarName = row.Field<string>("tovar_name"),
                        CharName = row.Field<string>("characteristic_name") ?? string.Empty,
                        CharGuid = row.Field<string>("characteristic_code") ?? string.Empty,
                        Price = row.Field<decimal>("price"),
                        Quantity = row.Field<double>("quantity")                      //Convert.ToDouble(row["quantity"])
                    });
                }

                // Обработка товаров для определения подарков
                var processedItems = ProcessItems(items, num_doc, (int)sum);

                // Создаем временную таблицу для новых данных
                DataTable newDt = dt.Clone();
                newDt.BeginLoadData();

                try
                {
                    // Добавление обработанных товаров во временную таблицу
                    foreach (var group in processedItems)
                    {
                        DataRow newRow = newDt.NewRow();
                        FillDataRowGift(newRow, group, num_doc);
                        newDt.Rows.Add(newRow);
                    }

                    // Добавление отфильтрованных товаров во временную таблицу
                    foreach (DataRow row in tempDt.Rows)
                    {
                        newDt.ImportRow(row);
                    }
                }
                finally
                {
                    newDt.EndLoadData();
                }

                // Если всё прошло успешно, заменяем оригинальную таблицу новой
                dt.Clear();
                foreach (DataRow row in newDt.Rows)
                {
                    dt.ImportRow(row);
                }

                // Помечаем товары, участвующие в акции
                marked_action_tovar_dt(dt,num_doc, comment, actionPricesByDoc);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                MainStaticClass.WriteRecordErrorLog(ex.Message, "action_4_dt", num_doc, MainStaticClass.CashDeskNumber, "Акция 4 типа без обращения к бд");

                // Восстановление исходного состояния таблицы
                dt.Clear();
                foreach (DataRow row in originalDt.Rows)
                {
                    dt.ImportRow(row);
                }

                throw; // Повторно выбрасываем исключение
            }
            finally
            {
                // Освобождение ресурсов
                originalDt.Dispose();
                tempDt.Dispose();
            }
        }

        /// <summary>
        /// Заполняет строку DataRow данными из GroupedItem.
        /// </summary>
        /// <param name="row">Строка DataRow для заполнения.</param>
        /// <param name="item">Группированный товар.</param>
        /// <param name="num_doc">Номер документа акции.</param>
        private void FillDataRowGift(DataRow row, GroupedItem item, int num_doc)
        {
            row["tovar_code"] = item.Code;
            row["tovar_name"] = item.TovarName ?? string.Empty; // Заполняем наименование товара
            row["characteristic_name"] = item.CharName ?? string.Empty;
            row["characteristic_code"] = item.CharGuid ?? string.Empty;
            row["quantity"] = item.Count;
            row["price"] = item.Price;
            row["price_at_discount"] = item.Discount;
            row["sum_full"] = item.SumFull;
            row["sum_at_discount"] = item.SumDiscount;
            row["action"] = 0; // Указываем, что это акция
            row["gift"] = item.Gift; // Указываем, является ли товар подарком
            row["action2"] = num_doc; // Номер акции
            row["bonus_reg"] = 0m; // Бонусы (по умолчанию 0)
            row["bonus_action"] = 0m; // Бонусы акции (по умолчанию 0)
            row["bonus_action_b"] = 0m; // Дополнительные бонусы акции (по умолчанию 0)
            row["marking"] = "0"; // Маркировка товара (по умолчанию "0")
        }

        /// <summary>
        /// Обрабатывает список товаров, группирует их и определяет подарки.
        /// </summary>
        /// <param name="items">Список товаров для обработки.</param>
        /// <param name="num_doc">Номер документа акции.</param>
        /// <param name="sum">Количество товаров, необходимое для срабатывания акции.</param>
        /// <returns>Список сгруппированных товаров.</returns>
        private List<GroupedItem> ProcessItems(List<ItemData> items, int num_doc, int sum)
        {
            var flatItems = new List<ItemData>();
            foreach (var item in items)
            {
                // Проверка, что количество товара является целым числом
                if (item.Quantity != Math.Floor(item.Quantity))
                    throw new ArgumentException("Quantity must be integer value");

                int quantity = (int)item.Quantity;

                // Разбиваем товар на отдельные единицы
                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(new ItemData
                    {
                        Code = item.Code,
                        TovarName = item.TovarName,
                        CharName = item.CharName,
                        CharGuid = item.CharGuid,
                        Price = item.Price,
                        Quantity = 1.0
                    });
                }
            }

            // Сортировка товаров по цене (от меньшего к большему)
            flatItems.Sort((a, b) => a.Price.CompareTo(b.Price));

            var groups = new Dictionary<string, GroupedItem>();
            for (int i = 0; i < flatItems.Count; i++)
            {
                var item = flatItems[i];
                // Определение, является ли текущий товар подарком
                bool isGift = (i % sum) == 0 && flatItems.Count >= sum; // Подарок выдается на каждую sum-ую позицию, начиная с первой, если товаров достаточно

                // Расчет цены: для подарков используется цена из акции, для остальных — цена товара
                decimal price = isGift
                    ? Convert.ToDecimal(get_price_action(num_doc)) // Используем цену подарка из акции
                    : Convert.ToDecimal(item.Price); // Используем обычную цену товара

                // Создание ключа для группировки, включая признак подарка
                string key = $"{item.Code}|{item.CharName}|{item.CharGuid}|{item.Price}|{price}|{isGift}";

                if (!groups.TryGetValue(key, out GroupedItem group))
                {
                    // Создание новой группы, если она не существует
                    group = new GroupedItem
                    {
                        Code = item.Code,
                        TovarName = item.TovarName,
                        CharName = item.CharName,
                        CharGuid = item.CharGuid,
                        Price = item.Price,
                        Discount = price, // Используем цену подарка или обычную цену
                        Action = 0,
                        Gift = isGift ? num_doc : 0, // Указываем, является ли группа подарком
                        Count = 0.0,
                        SumFull = 0.0m,
                        SumDiscount = 0.0m
                    };
                    groups[key] = group;
                }

                // Обновление данных группы
                group.Count += 1.0;
                group.SumFull += Convert.ToDecimal(item.Price);
                group.SumDiscount += price;
            }

            // Возвращаем список сгруппированных товаров
            return groups.Values.ToList();
        }

        /*Эта акция работает только по предъявлению 
        * акционного купона то есть по штрихкоду
        * если в чеке есть товары из списка. в случае, если сумма этих товаров в чеке, меньше суммы, 
        * на которую даётся скидка, разница покупателю не возвращается, а цены товаров ставятся равными 0.01грн.; 
        * скидка выдаётся при предъявлении специального купона с ШК. в случае, если в чеке вообще нет товаров, 
        * на которые должна выдаваться скидка, то выдаётся предупреждение и купон не используется. 
        * в одном чеке может использоваться только один акционный купон!!!
        * имеется ввиду что сколько раз не предъявляй купон скидка не накапливается
        */
        private void action_5_dt(int num_doc, decimal sum,string comment)
        {
            // 1 сНачала надо проверить сработку акции
            //bool the_action_has_worked = false;
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
                    query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());

                    if (result == 1)//Сработала акция
                    {
                        have_action = true;//Признак того что в документе есть сработка по акции

                        decimal the_sum_without_a_discount = Convert.ToDecimal(row["sum_full"]);
                        if (sum >= the_sum_without_a_discount)
                        {
                            sum = sum - the_sum_without_a_discount;
                            row["price_at_discount"] = (Convert.ToDecimal(1 / 100)).ToString();
                            row["sum_full"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString());
                            row["sum_at_discount"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString());
                            row["action"] = num_doc.ToString(); //Номер акционного документа
                            row["action2"] = num_doc.ToString(); //Номер акционного документа                           
                        }
                        else
                        {
                            //Здесь сначала получаем сумму путем вычитания оставшейся суммы скидки 
                            //от суммы без скидки, а только затем получаем цену со скидкой
                            row["sum_at_discount"] = Convert.ToDecimal(the_sum_without_a_discount - sum).ToString();//Сумма со скидкой
                            row["price_at_discount"] = Math.Round(Convert.ToDecimal(row["sum_at_discount"]) / Convert.ToDecimal(row["quantity"]), 2).ToString();
                            row["action"] = num_doc.ToString(); //Номер акционного документа
                            row["action2"] = num_doc.ToString(); //Номер акционного документа
                            sum = 0; break; //Поскольку сумма скидки закончилась прерываем цикл
                        }
                    }
                }
                //                conn.Close();
                /*Помечаем позиции которые 
                 * остались не помеченными 
                 * при сработке акции                 
                 */
                marked_action_tovar_dt(num_doc,comment);

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 5 типа акций");
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

        /*новый тип акции (6): за каждые ***руб. внутри чека (товаров из определённого списка) выдаётся сообщение типа 
        * "акция такая-то сработала *** раз, выдайте *** подарков" 
        * (в реальной акции, которая должна быть, будут выдаваться стикера, которые наклеиваются на купон, когда человек 
        * собирает 10 стикеров на этом купоне, то может обменять этот купон с наклеенными стикерами на подарочный комплект,
        * состав может быть разный, контролировать будет кассир)
        * 
        */
        private void action_6_dt(int num_doc, string comment, decimal sum, Int32 marker)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            Int16 result = 0;
            decimal action_sum_of_this_document = 0;
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
                    query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());
                    if (result == 1)//Акция сработала
                    {
                        have_action = true;//Признак того что в документе есть сработка по акции

                        action_sum_of_this_document += Convert.ToDecimal(row["sum_full"]);
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при работе с базой данных");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 6 типа акций");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            int quantity_of_gifts = (int)(action_sum_of_this_document / sum);
            //decimal quantity_of_gifts = Math.Round(action_sum_of_this_document / sum,0);

            if (quantity_of_gifts > 0)//значит акция сработала
            {
                if (show_messages)
                {
                    MessageBox.Show(comment + " количество подарков = " + quantity_of_gifts.ToString() + " шт. ", " АКЦИЯ !!!");
                }
                /*акция сработала
             * надо отметить все товарные позиции 
             * чтобы они не участвовали в других акциях 
             */
                if (marker == 1)
                {
                    for (int i = 0; i < quantity_of_gifts; i++)
                    {
                        show_query_window_barcode(2, 1, num_doc,0);
                    }
                }
                marked_action_tovar_dt(num_doc,comment);
            }
            roll_up_dt();
        }

        /*новый тип акции (7). для фиксирования выданных подарков по акции (6)
         * человек приносит купон и выбирает определённые товары с полки (согласно условиям акции),
         * кассир пробивает эти товары (подарки) в отдельный чек и проводит ШК купона по сканеру.
         * в чеке обнуляются цены на эти товары и добавляется строка типа "ПОДАРОК 0.01грн" - 4шт. 
         * (количество равно количеству товаров-подарков). кассир сам будет контролировать товарный состав чека, точнее, чтобы количество 
         * подарков было такое, которое позволяет купон со стикерами, если вдруг в чеке будут обычные товары, то их цена не должна меняться 
         * или можно выдать предупреждение, что для этого типа акции наличие в чеке не акционных товаров недопустимо.
         */
        private void action_7(int num_doc, long code_tovar)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            int gift = 0;
            Int16 result = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
                string query = "";
                foreach (DataRow row in dt.Rows)
                {
                    query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());
                    if (result > 0)
                    {
                        have_action = true;//Признак того что в документе есть сработка по акции

                        //lvi.SubItems[4].Text = ((decimal)1 / 100).ToString();
                        row["sum_full"] = "0";
                        row["sum_at_discount"] = "0"; //((Convert.ToDecimal(lvi.SubItems[2].Text) * Convert.ToDecimal(lvi.SubItems[4].Text)).ToString());
                        row["action"] = num_doc.ToString(); //Номер акционного документа
                        row["action"] = num_doc.ToString(); //Номер акционного документа
                        gift += Convert.ToInt32(row["quantity"]);
                    }
                    //else
                    //{
                    //    MessageBox.Show("Обнаружен неакционный товар " + row["code"]);
                    //}
                }
                find_barcode_or_code_in_tovar_action_dt(code_tovar.ToString(), gift, false, num_doc,0);
                //                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при работе с базой данных");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 7 типа акций");
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

        /// <summary>
        /// При покупке указанного количества 
        /// товаров покупатель может получить 
        /// указанную скидку на эти товары 
        /// либо указанный подарок,
        /// здесь дается скидка на все позиции 
        /// из списка
        ///  
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        /// <param name="sum"></param>
        private void action_8_dt(int num_doc, decimal persent, decimal sum,string comment)
        {

            if (!create_temp_tovar_table_4())
            {
                return;
            }

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlCommand command = null;
            string query_string = "";
            StringBuilder query = new StringBuilder();
            decimal quantity_on_doc = 0;
            DataTable dt2 = dt.Copy();
            dt2.Rows.Clear();


            try
            {

                conn.Open();
                //ListView clon = new ListView();
                //int total_quantity = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
                    {
                        DataRow row2 = dt2.NewRow();
                        row2.ItemArray = row.ItemArray;
                        dt2.Rows.Add(row2);
                        continue;
                    }
                    
                    query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query_string, conn);

                    if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                    {

                        for (int i = 0; i < Convert.ToInt32(row["quantity"]); i++)
                        {
                            query.Append("INSERT INTO tovar_action(code, retail_price, quantity,characteristic_name,characteristic_guid)VALUES(" +
                                row["tovar_code"].ToString() + "," +
                                row["price"].ToString().Replace(",", ".") + "," +
                               "1,'" +
                               row["characteristic_name"].ToString() + "','" +
                               row["characteristic_code"].ToString() + "');");
                        }
                        quantity_on_doc += Convert.ToDecimal(row["quantity"]);
                    }
                    else//Не участвует в акции убираем пока в сторонку
                    {
                        DataRow row2 = dt2.NewRow();
                        row2.ItemArray = row.ItemArray;
                        dt2.Rows.Add(row2);
                    }
                }


                if (quantity_on_doc >= sum)//Есть вхождение в акцию
                {
                    have_action = true;//Признак того что в документе есть сработка по акции                    
                    dt.Rows.Clear();
                    foreach (DataRow row2 in dt2.Rows)
                    {
                        DataRow row = dt.NewRow();
                        row.ItemArray = row2.ItemArray;
                        dt.Rows.Add(row);
                    }
                    //have_action = true;//Признак того что в документе есть сработка по акции                    
                    
                    command = new NpgsqlCommand(query.ToString(), conn);//устанавливаем акционные позиции во временную таблицу
                    command.ExecuteNonQuery();
                    //query = new StringBuilder();
                    //query.Append("DELETE FROM tovar_action;");//Очищаем таблицу акционных товаров 
                    //иначе результат задваивается ранее эта строка была закомментирована и при 2 товарах по 1 шт. учавстсующих в акции
                    //работала неверно

                    //int multiplication_factor = (int)(quantity_on_doc / sum)-1;
                    //query_string = " SELECT code, retail_price, quantity FROM tovar_action ORDER BY retail_price ";//запросим товары отсортированные по цене
                    query_string = " SELECT tovar_action.code,tovar.name, tovar.retail_price,tovar_action.retail_price, quantity FROM tovar_action LEFT JOIN tovar ON tovar_action.code=tovar.code ";//запросим товары отсортированные по цене
                    command = new NpgsqlCommand(query_string, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    decimal _sum_ = sum;
                    while (reader.Read())
                    {

                        //if ((multiplication_factor > 0) || (_sum_ > 0))
                        //{
                        //    if ((_sum_ == 0) && (multiplication_factor > 0))
                        //    {
                        //        _sum_ = sum;
                        //        multiplication_factor--;
                        //    }
                        //    _sum_ -= 1;

                        DataRow row = dt.NewRow();
                        row["tovar_code"] = reader[0].ToString();
                        row["tovar_name"] = reader[1].ToString().Trim();
                        row["characteristic_name"] = "";// reader[5].ToString();
                        row["characteristic_code"] = "";// reader[6].ToString();
                        row["quantity"] = reader[4].ToString().Trim();
                        row["price"] = reader.GetDecimal(2).ToString();
                        row["price_at_discount"] = Math.Round(reader.GetDecimal(2) - reader.GetDecimal(2) * persent / 100, 2).ToString();// get_price_action(num_doc);
                        row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
                        row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
                        if (Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"]))
                        {
                            row["action"] = num_doc.ToString();
                        }
                        else
                        {
                            row["action"] = "0";
                        }
                        row["gift"] = "0";
                        row["action2"] = num_doc.ToString();
                        row["bonus_reg"] = 0;
                        row["bonus_action"] = 0;
                        row["bonus_action_b"] = 0;
                        row["marking"] = "0";
                        dt.Rows.Add(row);
                        //multiplication_factor--;

                        //}
                        //else
                        //{
                        //    DataRow row = dt.NewRow();
                        //    row["tovar_code"] = reader[0].ToString();
                        //    row["tovar_name"] = reader[1].ToString().Trim();
                        //    row["characteristic_name"] = "";// reader[5].ToString();
                        //    row["characteristic_code"] = "";// reader[6].ToString();
                        //    row["quantity"] = reader[4].ToString().Trim();
                        //    row["price"] = reader.GetDecimal(2).ToString();
                        //    row["price_at_discount"] = reader.GetDecimal(2).ToString();
                        //    row["sum_full"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString();
                        //    row["sum_at_discount"] = (Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString();
                        //    if (Convert.ToDecimal(row["price"]) != Convert.ToDecimal(row["price_at_discount"]))
                        //    {
                        //        row["action"] = num_doc.ToString();
                        //    }
                        //    else
                        //    {
                        //        row["action"] = "0";
                        //    }
                        //    row["gift"] = "0";
                        //    row["action2"] = num_doc.ToString();
                        //    row["bonus_reg"] = 0;
                        //    row["bonus_action"] = 0;
                        //    row["bonus_action_b"] = 0;
                        //    row["marking"] = "0";
                        //    dt.Rows.Add(row);
                        //    multiplication_factor--;
                        //}
                    }
                    /*акция сработала
             * надо отметить все товарные позиции 
             * чтобы они не участвовали в других акциях 
             */
                    marked_action_tovar_dt(num_doc,comment);
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, " 8 акция ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " 8 акция ");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    // conn.Dispose();
                }
            }
            roll_up_dt();
        }

        /// <summary>
        /// При покупке указанного количества 
        /// товаров покупатель может получить 
        /// указанную скидку на эти товары 
        /// либо указанный подарок,
        /// здесь выдается подарок
        /// из списка
        /// если маркер =1 и заполнен код товара то будет выдан запрос
        /// на ввод кода или штрихкода и ему будет проставлена цена 
        /// кода товара
        /// Проверено 23.05.2019
        ///  
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        /// <param name="sum"></param>
        private void action_8_dt(int num_doc, string comment, decimal sum, Int32 marker)
        {

            if (!create_temp_tovar_table_4())
            {
                return;
            }

            NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
            NpgsqlCommand command = null;
            string query_string = "";
            //StringBuilder query = new StringBuilder();
            decimal quantity_on_doc = 0;

            try
            {

                conn.Open();
                //ListView clon = new ListView();
                //int total_quantity = 0;
                //foreach (ListViewItem lvi in listView1.Items)
                //{
                //    if (Convert.ToInt32(lvi.SubItems[10].Text.Trim()) > 0)//Этот товар уже участвовал в акции значит его пропускаем
                //    {
                //        continue;
                //    }
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
                    {
                        continue;
                    }

                    query_string = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query_string, conn);

                    if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                    {
                        quantity_on_doc += Convert.ToDecimal(row["quantity"]);
                    }
                }

                if (quantity_on_doc >= sum)//Есть вхождение в акцию
                {
                    have_action = true;//Признак того что в документе есть сработка по акции
                    int multiplication_factor = (int)(quantity_on_doc / sum);
                    MessageBox.Show(comment.Trim() + " количество подарков = " + multiplication_factor.ToString() + " шт. ", " АКЦИЯ !!!");
                    if (marker == 1)
                    {
                        for (int i = 0; i < multiplication_factor; i++)
                        {
                            show_query_window_barcode(2, 1, num_doc,0);
                        }
                    }
                    marked_action_tovar_dt(num_doc,comment);
                }
                roll_up_dt();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message, " 8 акция ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " 8 акция ");
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

        /// <summary>
        /// Здесь мы проверяем сумму по 1 списку тваров,
        /// если товар имеет вхождение в список мы 
        /// получаем сумму
        /// </summary>
        /// <param name="num_doc"></param>
        /// <param name="persent"></param>
        /// <param name="sum"></param>
        /// <param name="sum2"></param>
        private void action_12_dt(int num_doc, decimal persent, decimal sum,decimal sum1)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;            
            decimal fact1 = 0;//фактическая сумма в чеке по товарам из 1-го списка
            decimal fact2 = 0;//фактическая сумма в чеке по товарам из 2-го списка
            int count_list = 0;//количество списков в условии акций
            
            DataTable table_list1 = new DataTable();

            DataColumn tovar_code = new DataColumn();
            tovar_code.DataType = System.Type.GetType("System.Double");
            tovar_code.ColumnName = "tovar_code";
            table_list1.Columns.Add(tovar_code);

            DataColumn sum_at_discount = new DataColumn();
            sum_at_discount.DataType = System.Type.GetType("System.Double");
            sum_at_discount.ColumnName = "sum_at_discount";
            table_list1.Columns.Add(sum_at_discount);

            DataTable table_list2 = new DataTable();

            tovar_code = new DataColumn();
            tovar_code.DataType = System.Type.GetType("System.Double");
            tovar_code.ColumnName = "tovar_code";
            table_list2.Columns.Add(tovar_code);

            sum_at_discount = new DataColumn();
            sum_at_discount.DataType = System.Type.GetType("System.Double");
            sum_at_discount.ColumnName = "sum_at_discount";
            table_list2.Columns.Add(sum_at_discount);

            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();                
                
                string query = "DROP TABLE IF EXISTS table12;CREATE TEMP TABLE table12(tovar_code bigint,sum_at_a_discount numeric(12, 2));";

                //Поскольку документ не записан еще найти строки которые могут участвовать в акции можно только последовательным перебором 
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
                    {
                        continue;
                    }
                    query += "INSERT INTO table12(tovar_code,sum_at_a_discount)VALUES(" + row["tovar_code"].ToString() + "," + row["sum_at_discount"].ToString().Replace(",", ".") + ");";
                }

                command = new NpgsqlCommand(query, conn);                
                command.ExecuteNonQuery();

                query = " SELECT MAX(num_list)AS count_list FROM action_table WHERE num_doc =" + num_doc + ";" +

                    " Select coalesce(num_list,2)AS num_list,SUM(sum_at_a_discount) AS sum_at_a_discount" +
                    " FROM(SELECT code_tovar, coalesce(num_list, 2) AS num_list, num_doc" +
                    " FROM action_table WHERE action_table.num_doc =" + num_doc + ") AS Action12" +
                    " FULL JOIN  table12 ON tovar_code = code_tovar" +
                    " GROUP BY coalesce(num_list, 2);" +

                    " Select coalesce(CASE WHEN Action12.num_list = 1 THEN" +
                    " tovar_code::varchar(255)" +
                    " WHEN Action12.num_list = 2 OR Action12.num_list = NULL THEN" +
                    " 'num_list2'" +
                    " END,'0') AS code_action, coalesce(num_list, 2)AS num_list, sum_at_a_discount" +
                    " FROM(SELECT code_tovar, coalesce(num_list, 2) AS num_list, num_doc" +
                    " FROM action_table WHERE action_table.num_doc =" + num_doc + ") AS Action12" +
                    " FULL JOIN  table12 ON tovar_code = code_tovar;" +

                    " Select coalesce(CASE WHEN Action12.num_list = 1 THEN" +
                    " tovar_code::varchar(255)" +
                    " WHEN Action12.num_list = 2 OR Action12.num_list = NULL THEN" +
                    " 'num_list2'" +
                    " END,'0') AS code_action, coalesce(num_list, 2), sum_at_a_discount" +
                    " FROM(SELECT code_tovar, coalesce(num_list, 2) AS num_list, num_doc" +
                    " FROM action_table WHERE action_table.num_doc =" + num_doc + ") AS Action12" +
                    " LEFT JOIN  table12 ON tovar_code = code_tovar; ";

                command = new NpgsqlCommand(query, conn);
                //command.Transaction = tran;
                NpgsqlDataReader reader = command.ExecuteReader();
                Int16 num_query = 1;
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (num_query == 1)
                        {
                            count_list = Convert.ToUInt16(reader["count_list"]);
                        }
                        else if (num_query == 2)
                        {
                            if (Convert.ToInt16(reader["num_list"]) == 1)
                            {
                                fact1 = Convert.ToDecimal(reader["sum_at_a_discount"]);
                            }
                            else if (Convert.ToInt16(reader["num_list"]) == 2)
                            {
                                fact2 = Convert.ToDecimal(reader["sum_at_a_discount"]);
                            }
                        }
                        else if (num_query == 3)
                        {
                            if (Convert.ToInt16(reader["num_list"]) == 1)
                            {
                                DataRow new_row = table_list1.NewRow();
                                new_row["tovar_code"] = Convert.ToInt64(reader["code_action"]);
                                new_row["sum_at_discount"] = Convert.ToInt64(reader["sum_at_a_discount"]);
                                table_list1.Rows.Add(new_row);
                            }
                            else if (Convert.ToInt16(reader["num_list"]) == 2)
                            {
                                DataRow new_row = table_list2.NewRow();
                                new_row["tovar_code"] = Convert.ToInt64(reader["code_action"]);
                                new_row["sum_at_discount"] = Convert.ToInt64(reader["sum_at_a_discount"]);
                                table_list2.Rows.Add(new_row);
                            }
                        }
                    }
                    reader.NextResult();
                    num_query++;
                }

                int multiplicity = (int)(fact2 / sum1);//это кратность суммы скидки, а именно скидка дана будет товарам на сумму multiplicity*sum

                if ((multiplicity > 0) && (fact1 > 0))//Выполнились условия для сработки акции
                {
                    decimal action_sum = multiplicity * sum;
                    foreach (DataRow row_list1 in table_list1.Rows)
                    {
                        if (action_sum == 0)
                        {
                            break;
                        }
                        foreach (DataRow row_dt in dt.Rows)
                        {
                            if (action_sum == 0)
                            {
                                break;
                            }
                            if (Convert.ToDouble(row_list1["tovar_code"]) == Convert.ToDouble(row_dt["tovar_code"]))//на эту строку необходимо дать скидку но проверить сумму 
                            {
                                if (action_sum >= Convert.ToDecimal(row_dt["sum_at_discount"]))//сумма в строке меньше чем сумма на которую должна распространиться скидка
                                {
                                    action_sum = action_sum - Convert.ToDecimal(row_dt["sum_at_discount"]);
                                    row_dt["price_at_discount"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(row_dt["price"]) - Convert.ToDecimal(row_dt["price"]) * persent / 100), 2);//Цена со скидкой                                                                        
                                    row_dt["sum_at_discount"] = Convert.ToDecimal(row_dt["quantity"]) * Convert.ToDecimal(row_dt["price_at_discount"]);//сумма со скидкой                                    
                                    row_dt["action"] = num_doc.ToString(); //Номер акционного документа                        
                                    row_dt["action2"] = num_doc.ToString();//Тип акции 
                                }
                                else if (action_sum < Convert.ToDecimal(row_dt["sum_at_discount"]))//сумма в строке больше чем сумма на которую должна распространиться скидка, значит необходимо дать скидку на какое то число товаров
                                {
                                    int required_quantity = (int)(action_sum / Convert.ToDecimal(row_dt["price"]));//это то количество товара на которе будет дана скидка, строка разделится надвое

                                    row_dt["quantity"] = Convert.ToInt32(row_dt["quantity"]) - Convert.ToInt32(required_quantity);
                                    row_dt["sum_full"] = (Convert.ToDecimal(row_dt["quantity"]) * Convert.ToDecimal(row_dt["price"])).ToString();
                                    row_dt["sum_at_discount"] = (Convert.ToDecimal(row_dt["quantity"]) * Convert.ToDecimal(row_dt["price_at_discount"])).ToString();

                                    DataRow row = dt.NewRow();

                                    row["tovar_code"] = Convert.ToInt64(row_dt["tovar_code"]);
                                    row["tovar_name"] = row_dt["tovar_name"].ToString();
                                    row["characteristic_code"] = row_dt["characteristic_code"].ToString();
                                    row["characteristic_name"] = row_dt["characteristic_name"].ToString();
                                    row["quantity"] = required_quantity;
                                    row["price"] = Convert.ToDecimal(row_dt["price"]);
                                    row["price_at_discount"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(row["price"]) - Convert.ToDecimal(row["price"]) * persent / 100), 2);//Цена со скидкой                                                                        
                                    row["sum_full"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"]);
                                    row["sum_at_discount"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"]);
                                    row["action"] = num_doc.ToString(); //Номер акционного документа                        
                                    row["gift"] = "0";
                                    row["action2"] = num_doc.ToString();//Тип акции 
                                    row["bonus_reg"] = 0;
                                    row["bonus_action"] = 0;
                                    row["bonus_action_b"] = 0;
                                    row["marking"] = "0";

                                    dt.Rows.Add(row);

                                    action_sum = 0;
                                    break;
                                }
                            }
                        }
                    }
                    if (count_list == 2)
                    {
                        foreach (DataRow row_list2 in table_list2.Rows)//отметить позиции 2 списка как участвовавшие в акции
                        {
                            foreach (DataRow row_dt in dt.Rows)
                            {
                                if (Convert.ToDouble(row_list2["tovar_code"]) == Convert.ToDouble(row_dt["tovar_code"]))//на эту строку необходимо дать скидку но проверить сумму 
                                {
                                    row_dt["action2"] = num_doc.ToString();//Тип акции 
                                }
                            }
                        }
                    }
                    else if (count_list == 1)//отметить позиции 2 списка как участвовавшие в акции, в данном варианте 2 список это весь товарный состав
                    {
                        foreach (DataRow row_dt in dt.Rows)
                        {
                            if (row_dt["action2"].ToString() != num_doc.ToString())
                            {
                                row_dt["action2"] = num_doc.ToString();//Тип акции 
                            }
                        }
                    }
                }                  
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message + " | " + ex.Detail + " ошибка при обработке 12 типа акций ");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 12 типа акций");
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


        private void roll_up_dt()
        {
            //return;
            string query = "";
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            NpgsqlTransaction trans = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                trans = conn.BeginTransaction();
                query = "DROP TABLE IF EXISTS roll_up_temp;"+
                    " CREATE TEMP TABLE roll_up_temp "+
                    " (code_tovar bigint,"+
                    " name_tovar character varying(200) COLLATE pg_catalog.default,"+
                    " characteristic_guid character varying(36) COLLATE pg_catalog.default,"+
                    " characteristic_name character varying(200) COLLATE pg_catalog.default,"+
                    " quantity numeric(10, 3)," +
                    " price numeric(10, 2),"+
                    " price_at_a_discount numeric(10,2),"+
                    " sum numeric(10,2),"+
                    " sum_at_a_discount numeric(10,2),"+
                    " action_num_doc integer,"+
                    " action_num_doc1 integer,"+
                    " action_num_doc2 integer,"+
                    " item_marker character varying(200) COLLATE pg_catalog.default);";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                command.ExecuteNonQuery();

                foreach (DataRow row in dt.Rows)
                {
                    query = "INSERT INTO roll_up_temp(code_tovar," +
                                                            " name_tovar, " +
                                                            "characteristic_guid, " +
                                                            "characteristic_name, " +
                                                            "quantity, " +
                                                            "price, " +
                                                            "price_at_a_discount, " +
                                                            "sum, " +
                                                            "sum_at_a_discount, " +
                                                            "action_num_doc, " +
                                                            "action_num_doc1, " +
                                                            "action_num_doc2, " +
                                                            "item_marker)VALUES(" +
                                                             row["tovar_code"] + ",'" +
                                                             row["tovar_name"] + "','" +
                                                             row["characteristic_code"] + "','" +
                                                             row["characteristic_name"] + "'," +
                                                             row["quantity"].ToString().Replace(",",".") + "," +
                                                             row["price"].ToString().Replace(",", ".") + "," +
                                                             row["price_at_discount"].ToString().Replace(",", ".") + "," +
                                                             row["sum_full"].ToString().Replace(",", ".") + "," +
                                                             row["sum_at_discount"].ToString().Replace(",", ".") + "," +
                                                             row["action"] + "," +
                                                             row["gift"] + "," +
                                                             row["action2"] + ",'" +
                                                             row["marking"].ToString().Replace("'", "vasya2021") + "')";                    
                                                            
                    command = new NpgsqlCommand(query, conn);
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                }
                query = "SELECT code_tovar, name_tovar, characteristic_guid, characteristic_name, SUM(quantity) AS quantity, price," +
                    " price_at_a_discount, SUM(sum), SUM(sum_at_a_discount), action_num_doc, action_num_doc1, action_num_doc2, item_marker" +
                    " FROM roll_up_temp    GROUP BY code_tovar, name_tovar, characteristic_guid, characteristic_name, price," +
                    " price_at_a_discount, action_num_doc, action_num_doc1, action_num_doc2, item_marker;";
                command = new NpgsqlCommand(query, conn);
                command.Transaction = trans;
                NpgsqlDataReader reader = command.ExecuteReader();
                dt.Rows.Clear();
                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    row["tovar_code"] = reader.GetInt64(0);
                    row["tovar_name"] = reader[1].ToString().Trim();
                    row["characteristic_code"] = reader[2].ToString();
                    row["characteristic_name"] = reader[3].ToString();
                    row["quantity"] = reader.GetDecimal(4);
                    row["price"] = reader.GetDecimal(5);
                    row["price_at_discount"] = reader.GetDecimal(6);
                    row["sum_full"] = reader.GetDecimal(7);
                    row["sum_at_discount"] = reader.GetDecimal(8);
                    row["action"] = reader.GetInt32(9);
                    row["gift"] = reader.GetInt32(10);
                    row["action2"] = reader.GetInt32(11);
                    row["bonus_reg"] = 0;
                    row["bonus_action"] = 0;
                    row["bonus_action_b"] = 0;
                    row["marking"] = reader[12].ToString().Replace("vasya2021", "'");

                    dt.Rows.Add(row);                    
                }
                reader.Close();
                conn.Close();
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
    }
}

