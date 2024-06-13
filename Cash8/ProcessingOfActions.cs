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
                    " AND " + count_minutes.ToString() + " between time_start AND time_end  AND kind=0 AND num_doc in(" +
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
                            action_1_dt(num_doc, persent, comment);//Дать скидку на эту позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                                action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 2)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {
                            action_2_dt(num_doc, persent, comment);
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                                action_2_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());

                    }
                    else if (tip_action == 3)
                    {
                        //start_action = DateTime.Now;                        
                        if (persent != 0)
                        {
                            action_3_dt(num_doc, persent, sum, comment);//Дать скидку на все позиции из списка позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{
                                action_3_dt(num_doc, comment, sum, marker,show_messages); //Сообщить о подарке                           
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
                    else if (tip_action == 10)
                    {
                        if (sum <= calculation_of_the_sum_of_the_document_dt())
                        {
                            //MessageBox.Show(reader[3].ToString());
                            action_num_doc = num_doc; Convert.ToInt32(reader["num_doc"].ToString());
                        }
                    }
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
                            action_1_dt(num_doc, persent, comment);//Дать скидку на эту позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                                             
                                action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 2)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {                            
                            action_2_dt(num_doc, persent, comment);
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                         
                                action_2_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок                          
                            //}
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
                                                            
                            action_1_dt(num_doc, persent, comment);//Дать скидку на эту позицию                                                 
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                                                    
                                action_1_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
                        }
                        //write_time_execution(reader[1].ToString(), tip_action.ToString());
                    }
                    else if (tip_action == 2)
                    {
                        //start_action = DateTime.Now;

                        if (persent != 0)
                        {                            
                            action_2_dt(num_doc, persent, comment);
                        }
                        else
                        {
                            //if (show_messages)//В этой акции в любом случае всплывающие окна, в предварительном рассчете она не будет участвовать
                            //{                         
                                action_2_dt(num_doc, comment, marker, show_messages); //Сообщить о подарке, а так же добавить товар в подарок если указан код товара                          
                            //}
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
        private void marked_action_tovar_dt(int num_doc,string comment)
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
        public void find_barcode_or_code_in_tovar_action_dt(string barcode, int count, bool sum_null, int num_doc,int mode)
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
                    if (mode == 0)
                    {
                        row = dt.NewRow();
                    }
                    else
                    {
                        row = dt_copy.NewRow();
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
        private void action_1_dt(int num_doc, string comment, int marker, bool show_messages)
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
                dt_copy = dt.Clone();

                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем
                    {
                        continue;
                    }
                    query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"].ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    result = Convert.ToInt16(command.ExecuteScalar());
                    if (result > 0)
                    {
                        have_action = true;//Признак того что в документе есть сработка по акции                        

                        row["gift"] = num_doc.ToString();//Тип акции                                                 
                        if (show_messages)
                        {
                            MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок " + comment);
                            DialogResult dr = DialogResult.Cancel;
                            if (marker == 1)
                            {
                                dr = show_query_window_barcode(2, 1, num_doc, 1);
                            }
                        }
                        //if (dr != DialogResult.Cancel)
                        //{
                        //    if (code_tovar != 0)
                        //    {
                        //        find_barcode_or_code_in_tovar_dt(code_tovar.ToString());
                        //    }
                        //}
                    }
                }
                foreach (DataRow row in dt_copy.Rows)
                {
                    dt.ImportRow(row);
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
        private void action_1_dt(int num_doc, decimal persent, string comment)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            double result = 0;
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

                    query = "SELECT price FROM action_table WHERE code_tovar=" + row["tovar_code"].ToString() + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    object result_query = command.ExecuteScalar();
                    command.Dispose();
                    if (result_query != null)
                    {
                        if (Convert.ToDecimal(result_query) == 0)
                        {
                            //have_action = true;//Признак того что в документе есть сработка по акции                        
                            row["price_at_discount"] = Math.Round(Convert.ToDecimal(Convert.ToDecimal(row["price"]) - Convert.ToDecimal(row["price"]) * persent / 100), 2);//Цена со скидкой                                    
                            row["sum_full"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"]);
                            row["sum_at_discount"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"]);
                            row["action"] = num_doc.ToString(); //Номер акционного документа                        
                            row["action2"] = num_doc.ToString();//Тип акции 
                            if (dt.Columns.Contains("promo_description"))
                            {
                                row["promo_description"] = comment;
                            }
                        }
                        else
                        {
                            row["price_at_discount"] = result_query.ToString();//Цена со скидкой                                    
                            row["sum_full"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"]);
                            row["sum_at_discount"] = Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"]);
                            row["action"] = num_doc.ToString(); //Номер акционного документа                        
                            row["action2"] = num_doc.ToString();//Тип акции         
                            if (dt.Columns.Contains("promo_description"))
                            {
                                row["promo_description"] = comment;
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
        private void action_2_dt(int num_doc, decimal persent, string comment)
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
                        query = "SELECT num_list FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
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
                            ar.Add(row["tovar_code"].ToString());
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
                    DataRow row2 = null;
                    //foreach (ListViewItem lvi in listView1.Items)
                    foreach (DataRow row in dt.Rows)
                    {
                        if (min_quantity > 0)
                        {
                            have_action = true;//Признак того что в документе есть сработка по акции
                            //Сначала получим количество,если больше кратного количества наборов то копируем строку,
                            //а в исходной уменьшаем количество на количества наборов и пересчитываем суммы                            
                            if (ar.IndexOf(row["tovar_code"].ToString(), 0) == -1)
                            {
                                continue;
                            }
                            quantity_of_pieces = Convert.ToInt16(row["quantity"]);
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
                                row["quantity"] = Convert.ToInt32(row["quantity"]) - min_quantity;
                                row["sum_at_discount"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString());
                                
                                //Добавляем новую строку с количеством min_quantity 
                                row2 = dt.NewRow();
                                row2.ItemArray = row.ItemArray;
                                row2["quantity"] = min_quantity;
                                row2["price_at_discount"] = Math.Round(Convert.ToDecimal(row2["price"]) - Convert.ToDecimal(row2["price"]) * persent / 100, 2);//Цена со скидкой                                            
                                row2["sum_at_discount"] = ((Convert.ToDecimal(row2["quantity"]) * Convert.ToDecimal(row2["price_at_discount"])).ToString());
                                row2["action"] = num_doc.ToString(); //Номер акционного документа 
                                row2["action"] = num_doc.ToString(); //Номер акционного документа 
                                //dt.Rows.Add(row2);

                                //row["action"] = num_doc.ToString(); //Номер акционного документа 


                                //calculation_of_the_sum_of_the_document_dt()
                                // calculate_on_string(lvi);

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
                                ////*****************************************************************************
                                //lvi_new.SubItems[11].Text = "0";
                                //lvi_new.SubItems[12].Text = "0";
                                //lvi_new.SubItems[13].Text = "0";
                                //lvi_new.SubItems[14].Text = "0";
                                ////*****************************************************************************
                                //listView1.Items.Add(lvi_new);
                                //SendDataToCustomerScreen(1, 0, 1);
                                //min_quantity = 0;


                            }
                        }
                    }
                    if (row2 != null)
                    {
                        dt.Rows.Add(row2);
                    }

                    /*акция сработала
                 * надо отметить все товарные позиции 
                 * чтобы они не участвовали в других акциях 
                 */
                    if (the_action_has_worked)
                    {
                        marked_action_tovar_dt(num_doc,comment);
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
        private void action_2_dt(int num_doc, string comment, int marker, bool show_messages)
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
                        query = "SELECT num_list FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
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
                                first_string_actions = index; //Запомним номер строки товара на который будем давать скидку
                            }
                        }
                        if (num_list != 0)
                        {
                            action_list[num_list - 1] += 1;
                            num_pos = index;
                        }
                        if (num_list == 1)
                        {
                            ar.Add(row["tovar_code"].ToString());
                        }
                        quantity_of_pieces--;
                    }
                }

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
                    have_action = true;//Признак того что в документе есть сработка по акции 
                    if (show_messages)
                    {
                        MessageBox.Show("Сработала акция, НЕОБХОДИМО выдать подарок количестве " + min_quantity.ToString() + " шт. " + comment);
                        DialogResult dr = DialogResult.Cancel;
                        if (marker == 1)
                        {
                            dr = show_query_window_barcode(2, min_quantity, num_doc, 0);
                        }
                    }

                    //if (dr != DialogResult.Cancel)
                    //{
                    //    find_barcode_or_code_in_tovar_dt(code_tovar.ToString());
                    //}

                    /*акция сработала
                    * надо отметить все товарные позиции 
                    * чтобы они не участвовали в других акциях 
                    */

                    if (the_action_has_worked)
                    {
                        marked_action_tovar_dt(num_doc,comment);
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

        /*Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
        * тогда дается скидка на те позиции которые перечисляются в условии акции
        */
        private void action_3_dt(int num_doc, decimal persent, decimal sum, string comment)
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            decimal sum_on_doc = 0;//сумма документа без скидок 
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
                    string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                    {
                        sum_on_doc += Convert.ToDecimal(row["sum_at_discount"]);
                    }
                }
                //Сумма документа без скидки больше или равна той что в условии ации
                //значит акция сработала
                if (sum_on_doc >= sum)
                {
                    have_action = true;//Признак того что в документе есть сработка по акции

                    foreach (DataRow row in dt.Rows)
                    {
                        if (Convert.ToInt32(row["action2"]) > 0)//Этот товар уже участвовал в акции значит его пропускаем                  
                        {
                            continue;
                        }
                        string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                        command = new NpgsqlCommand(query, conn);
                        if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                        {
                            row["price_at_discount"] = (Math.Round(Convert.ToDecimal(row["price"]) - Convert.ToDecimal(row["price"]) * persent / 100, 2)).ToString();//Цена со скидкой            
                            row["sum_full"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price"])).ToString());
                            row["sum_at_discount"] = ((Convert.ToDecimal(row["quantity"]) * Convert.ToDecimal(row["price_at_discount"])).ToString());
                            row["action"] = num_doc.ToString();
                            row["action2"] = num_doc.ToString();
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
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        /*Эта акция срабатывает когда сумма без скидки в документе >= сумме акции
         * тогда выдается сообщение о подарке
         */
        private void action_3_dt(int num_doc, string comment, decimal sum, int marker,bool show_messages)
        {

            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            decimal sum_on_doc = 0;//сумма документа без скидок 
            int index = 0;
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
                    string query = "SELECT COUNT(*) FROM action_table WHERE code_tovar=" + row["tovar_code"] + " AND num_doc=" + num_doc.ToString();
                    command = new NpgsqlCommand(query, conn);
                    if (Convert.ToInt16(command.ExecuteScalar()) != 0)
                    {
                        sum_on_doc += Convert.ToDecimal(row["sum_at_discount"]);
                        index = dt.Rows.IndexOf(row);
                    }
                }
                //Сумма документа без скидки больше или равна той что в условии ации
                //значит акция сработала
                if (sum_on_doc >= sum)
                {
                    have_action = true;//Признак того что в документе есть сработка по акции                    
                    dt.Rows[index]["gift"] = num_doc.ToString();//Тип акции                    
                    if (show_messages)
                    {
                        MessageBox.Show(comment, " АКЦИЯ !!!");
                    }
                    DialogResult dr = DialogResult.Cancel;
                    if (show_messages)
                    {
                        if (marker == 1)
                        {
                            dr = show_query_window_barcode(2, 1, num_doc,0);
                        }
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
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка при обработке 3 типа акций");
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
        private void action_4_dt(int num_doc, decimal persent, decimal sum,string comment)
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
                    marked_action_tovar_dt(num_doc,comment);
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
                ListView clon = new ListView();
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
                    " quantity integer,"+
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
                                                             row["quantity"] + "," +
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
                    row["quantity"] = reader.GetInt64(4);
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

