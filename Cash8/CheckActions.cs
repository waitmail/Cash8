using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Npgsql;
using System.Collections;


namespace Cash8
{
    public partial class CheckActions : Form
    {
        private DataTable dt1 = null;
        private DataTable dt2 = null;
        public ArrayList action_barcode_list = new ArrayList();//Доступ из формы ввода акционного штрихкода 

        public CheckActions()
        {
            InitializeComponent();
            this.txtB_input_code_or_barcode.KeyPress += TxtB_input_code_or_barcode_KeyPress;
            dt1 = create_dt(1);
            dt2 = create_dt(2);

           
            this.Load += CheckActions_Load;
            this.Resize += CheckActions_Resize;
            txtB_client_code.KeyPress += TxtB_client_code_KeyPress;
            dataGridView_tovar_execute.DataSourceChanged += DataGridView_tovar_execute_DataSourceChanged;
        }

        private void DataGridView_tovar_execute_DataSourceChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView_tovar_execute.Rows)
            {
                if (Convert.ToInt32(row.Cells["action2"].Value) != 0)
                {
                    dataGridView_tovar_execute.Rows[row.Index].DefaultCellStyle.BackColor = Color.LightYellow;
                }
                else
                {
                    dataGridView_tovar_execute.Rows[row.Index].DefaultCellStyle.BackColor = Color.White;
                }
            }
        }       

        private void TxtB_client_code_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                bool find_phone = false;
                if ((txtB_client_code.Text.Trim().Length != 10) && (txtB_client_code.Text.Trim().Length != 13))
                {
                    MessageBox.Show("Код клиента имеет неправильную длину");
                    return;
                }
                string query = "";
                if (txtB_client_code.Text.Trim().Length == 10)
                {
                    if (txtB_client_code.Text.Trim().Substring(0, 1) == "9")//это номер телефона
                    {
                        query = "SELECT code,name FROM clients where phone='" + txtB_client_code.Text.Trim() + "'";
                    }
                }
                if (query == "")
                {
                    query = "SELECT code,name FROM clients where code='" + txtB_client_code.Text.Trim() + "'";
                }

                NpgsqlConnection conn = MainStaticClass.NpgsqlConn();
                try
                {
                    conn.Open();
                    NpgsqlCommand command = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    bool find_card = false;
                    while (reader.Read())
                    {
                        find_card = true;
                        txtB_client.Tag = reader["code"].ToString();
                        txtB_client.Text = reader["name"].ToString();
                        break;
                    }
                    if (!find_card)
                    {
                        foreach (DataRow row in dt1.Rows)
                        {
                            row["price_at_discount"] = Convert.ToDecimal(row["price"]);
                        }
                    }
                    else
                    {
                        if (find_phone)
                        {
                            MessageBox.Show("Клиент с "+(find_phone ? "кодом" : " номером телефона ") + txtB_client_code.Text.Trim()+ "не найден ");
                        }
                        foreach (DataRow row in dt1.Rows)
                        {
                            row["price_at_discount"] = Convert.ToDecimal(row["price"]) - Convert.ToDecimal(row["price"]) * Convert.ToDecimal(0.05);
                        }
                    }
                    txtB_client_code.Text = "";
                    btn_check_actions_Click();
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show("Произошли ошибки при поиске клиента " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошли ошибки при поиске клиента " + ex.Message);
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

        private void CheckActions_Resize(object sender, EventArgs e)
        {
            dataGridView_tovar.Size = new Size(this.Width - 20, (this.Height - 80) / 2);
            dataGridView_tovar_execute.Size = new Size(this.Width - 20, (this.Height - 180) / 2);
            dataGridView_tovar_execute.Location = new Point(10, dataGridView_tovar_execute.Height + 130);
        }

        private void CheckActions_Load(object sender, EventArgs e)
        {

            // Создаем словарь с соответствием имен колонок и HeaderText
            Dictionary<string, string> columnHeaders = new Dictionary<string, string>
            {
                {"tovar_code", "Код"},
                {"tovar_name", "Намиенование"},
                { "quantity", "К-во"},
                { "price", "Цена б.с."},
                { "price_at_discount", "Цена"},
                { "sum_full", "Сумма б.с."},
                { "sum_at_discount", "Сумма"},
                { "action", "Акция"},
                { "gift", "Подарок"},
                { "action2", "Уч. в акции"},
                { "marking", "Марк./сертиф."},
                { "promo_description", "Акция"}
                
            };

            this.dataGridView_tovar.DataSource = dt1;
            this.dataGridView_tovar_execute.DataSource = dt2;

            foreach (KeyValuePair<string, string> entry in columnHeaders)
            {
                if (dataGridView_tovar.Columns.Contains(entry.Key))
                {
                    dataGridView_tovar.Columns[entry.Key].HeaderText = entry.Value;
                }
                if (dataGridView_tovar_execute.Columns.Contains(entry.Key))
                {
                    dataGridView_tovar_execute.Columns[entry.Key].HeaderText = entry.Value;
                }
            }
            foreach (DataGridViewColumn col in dataGridView_tovar.Columns)
            {
                if (col.ValueType != typeof(string))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }
            foreach (DataGridViewColumn col in dataGridView_tovar_execute.Columns)
            {
                if (col.ValueType != typeof(string))
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            // Устанавливаем автоматическое изменение высоты строк
            dataGridView_tovar.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            // Устанавливаем перенос текста по словам для конкретной колонки
            dataGridView_tovar.Columns["tovar_name"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            // Устанавливаем перенос текста по словам для конкретной колонки
            dataGridView_tovar.Columns["tovar_name"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView_tovar.Columns["tovar_name"].Width = 200;

            // Устанавливаем автоматическое изменение высоты строк
            dataGridView_tovar_execute.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            // Устанавливаем перенос текста по словам для конкретной колонки
            dataGridView_tovar_execute.Columns["tovar_name"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            // Устанавливаем перенос текста по словам для конкретной колонки
            dataGridView_tovar.Columns["tovar_name"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView_tovar_execute.Columns["tovar_name"].Width = 200;

            foreach (DataGridViewColumn col in dataGridView_tovar.Columns)
            {
                if (col.Name != "tovar_name")
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }

            foreach (DataGridViewColumn col in dataGridView_tovar_execute.Columns)
            {
                if (col.Name != "tovar_name")
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }


                // Устанавливаем автоматическое изменение высоты строк для конкретной колонки
                //dataGridView_tovar_execute.Columns["tovar_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Устанавливаем автоматическое изменение высоты строк для конкретной колонки
                //dataGridView_tovar_execute.Columns["tovar_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Устанавливаем автоматическое изменение высоты строк для конкретной колонки
                //dataGridView_tovar.Columns["tovar_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            }

        public DataTable create_dt(int variant)
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
            characteristic_code.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(characteristic_code);

            DataColumn characteristic_name = new DataColumn();
            characteristic_name.DataType = System.Type.GetType("System.String");
            characteristic_name.ColumnName = "characteristic_name"; //listView1.Columns.Add("Характеристика", 400, HorizontalAlignment.Left);
            characteristic_name.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(characteristic_name);

            DataColumn quantity = new DataColumn();
            quantity.DataType = System.Type.GetType("System.Decimal");
            quantity.ColumnName = "quantity"; //listView1.Columns.Add("Количество", 50, HorizontalAlignment.Right);
            dt.Columns.Add(quantity);

            DataColumn price = new DataColumn();
            price.DataType = System.Type.GetType("System.Decimal");
            price.ColumnName = "price"; //listView1.Columns.Add("Цена", 50, HorizontalAlignment.Right);
            price.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(price);

            DataColumn price_at_discount = new DataColumn();
            price_at_discount.DataType = System.Type.GetType("System.Decimal");
            price_at_discount.ColumnName = "price_at_discount"; //listView1.Columns.Add("Цена со скидкой", 50, HorizontalAlignment.Right);
            dt.Columns.Add(price_at_discount);

            DataColumn sum_full = new DataColumn();
            sum_full.DataType = System.Type.GetType("System.Decimal");
            sum_full.ColumnName = "sum_full"; //listView1.Columns.Add("Сумма", 50, HorizontalAlignment.Right);
            sum_full.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(sum_full);

            DataColumn sum_at_discount = new DataColumn();
            sum_at_discount.DataType = System.Type.GetType("System.Decimal");
            sum_at_discount.ColumnName = "sum_at_discount"; //listView1.Columns.Add("Сумма со скидкой", 50, HorizontalAlignment.Right);
            dt.Columns.Add(sum_at_discount);

            DataColumn action = new DataColumn();
            action.DataType = System.Type.GetType("System.Int32");
            action.ColumnName = "action"; //listView1.Columns.Add("Акция", 50, HorizontalAlignment.Right);
            action.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(action);

            DataColumn gift = new DataColumn();
            gift.DataType = System.Type.GetType("System.Int32");
            gift.ColumnName = "gift"; //listView1.Columns.Add("Подарок", 50, HorizontalAlignment.Right);
            gift.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(gift);

            DataColumn action2 = new DataColumn();
            action2.DataType = System.Type.GetType("System.Int32");
            action2.ColumnName = "action2"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            if (variant == 1)
            {
                action2.ColumnMapping = MappingType.Hidden;
            }
            dt.Columns.Add(action2);

            DataColumn bonus_reg = new DataColumn();
            bonus_reg.DataType = System.Type.GetType("System.Int32");
            bonus_reg.ColumnName = "bonus_reg"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            bonus_reg.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(bonus_reg);

            DataColumn bonus_action = new DataColumn();
            bonus_action.DataType = System.Type.GetType("System.Int32");
            bonus_action.ColumnName = "bonus_action"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            bonus_action.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(bonus_action);

            DataColumn bonus_action_b = new DataColumn();
            bonus_action_b.DataType = System.Type.GetType("System.Int32");
            bonus_action_b.ColumnName = "bonus_action_b"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            bonus_action_b.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(bonus_action_b);

            DataColumn marking = new DataColumn();
            marking.DataType = System.Type.GetType("System.String");
            marking.ColumnName = "marking"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            marking.ColumnMapping = MappingType.Hidden;
            dt.Columns.Add(marking);
            
            DataColumn promo_description = new DataColumn();
            promo_description.DataType = System.Type.GetType("System.String");
            promo_description.ColumnName = "promo_description"; //listView1.Columns.Add("Акция2", 50, HorizontalAlignment.Right);
            if (variant == 1)
            {
                promo_description.ColumnMapping = MappingType.Hidden;
            }
            dt.Columns.Add(promo_description);
            
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
                
                NpgsqlDataReader reader = command.ExecuteReader();
                
                bool ТоварНайден = false;

                while (reader.Read())
                {
                    ТоварНайден = true;
                    DataRow row = null;

                    DataRow[] найденныеСтроки = dt1.Select("tovar_code = '" + reader["code"].ToString() + "'");

                    if (найденныеСтроки.Length > 0)
                    {
                        найденныеСтроки[0]["quantity"] = Convert.ToInt32(найденныеСтроки[0]["quantity"]) + 1;
                        row = найденныеСтроки[0];
                        row["sum_full"] = Convert.ToDecimal(row["price"]) * Convert.ToDecimal(row["quantity"]);
                        row["sum_at_discount"] = Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]);
                    }
                    else
                    {
                        row = dt1.NewRow();
                        row["tovar_code"] = reader["code"].ToString().Trim();
                        row["tovar_name"] = reader["name"].ToString().Trim();
                        row["quantity"] = 1;
                        row["price"] = reader["retail_price"].ToString();
                        if (txtB_client.Tag != null)
                        {
                            if ((txtB_client.Tag.ToString() != ""))
                            {
                                row["price_at_discount"] = Convert.ToDecimal(row["price"]) * Convert.ToDecimal(0.05);
                            }
                        }
                        else
                        {
                            row["price_at_discount"] = Convert.ToDecimal(row["price"]);
                        }
                        row["sum_full"] = Convert.ToDecimal(row["price"]) * Convert.ToDecimal(row["quantity"]);
                        row["sum_at_discount"] = Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]);

                        row["action"] = 0;// Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]);
                        row["gift"] = 0;// Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]);
                        row["action2"] = 0;// Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]);
                        //row["sum_at_discount"] = Convert.ToDecimal(row["price_at_discount"]) * Convert.ToDecimal(row["quantity"]);
                        dt1.Rows.Add(row);
                    }

                    btn_check_actions_Click();
                }
                if (!ТоварНайден)
                {
                    Tovar_Not_Found t_n_f = new Tovar_Not_Found();
                    t_n_f.ShowDialog();
                    t_n_f.Dispose();
                }
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

        public void btn_check_actions_Click()
        {
            to_define_the_action_dt(true);
        }

        /// <summary>
        /// Здесь происходит обработка по всем 
        /// регулярным акциям течто со штрихкодом
        /// и те что без штрихкода        
        /// </summary>
        /// <param name="show_messages"></param>
        /// <returns></returns>
        private void to_define_the_action_dt(bool show_messages)
        {            
            ProcessingOfActions processingOfActions = new ProcessingOfActions();
            processingOfActions.dt = dt1.Copy();// processingOfActions.create_dt(listView1);
            processingOfActions.show_messages = show_messages;
            //MainStaticClass.write_event_in_log(" Попытка обработать акции по штрихкодам ", "Документ чек", numdoc.ToString());
            foreach (string barcode in action_barcode_list)
            {
                processingOfActions.to_define_the_action_dt(barcode);
            }

            if (txtB_client.Tag != null)
            {
                processingOfActions.to_define_the_action_personal_dt(this.txtB_client.Tag.ToString());
            }

            processingOfActions.to_define_the_action_dt();
            dt2 = processingOfActions.dt.Copy();
            dataGridView_tovar_execute.DataSource = dt2;           
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                show_query_window_barcode(1, 0, 0);
            }
        }
        
        //private string get_promo_description(int promo_um_doc)
        //{
        //    string result = "";
        //    NpgsqlConnection conn = MainStaticClass.NpgsqlConn();

        //    try
        //    {
        //        conn.Open();

        //    }
        //    catch (NpgsqlException ex)
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {

        //    }

        //    return result;
        //}

        /*
        * Когда по акции должен быть выдан подарок
        * иногда вводится штрихкод, то есть подарок
        * зараннее может быть не определен
        * 1.Вызов для ввода акционного штрихкода который определяет собой акцию
        * 2.Вызов для ввода штрихкода по которму будет найден подарок по акции с нулевой ценой
        * 
        */
        private DialogResult show_query_window_barcode(int call_type, int count, int num_doc)
        {
            Input_action_barcode ib = new Input_action_barcode();
            ib.count = count;
            ib.caller3 = this;
            ib.call_type = call_type;
            ib.num_doc = num_doc;
            DialogResult dr = ib.ShowDialog();
            ib.Dispose();
            return dr;
        }

        public bool check_action(string barcode)
        {
            if (barcode.Trim().Length == 0)
            {
                return false;
            }
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            int count_action = 0;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "";
                if (barcode.Trim().Length > 4)
                {
                    query = "SELECT COUNT(*) FROM action_header WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end AND barcode='" + barcode + "'";
                }
                else
                {
                    query = "SELECT COUNT(*) FROM action_header WHERE '" + DateTime.Now.Date.ToString("yyy-MM-dd") + "' between date_started AND date_end AND promo_code='" + barcode + "'";
                }
                command = new NpgsqlCommand(query, conn);
                count_action = Convert.ToInt32(command.ExecuteScalar());
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Ошибка при работе с базой данных " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            if (count_action > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
