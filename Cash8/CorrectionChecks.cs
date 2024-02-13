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
    public partial class CorrectionChecks : Form
    {
        private DataTable dt = new DataTable();
        public CorrectionChecks()
        {
            InitializeComponent();
            this.Load += CorrectionChecks_Load;
        }

        private void CorrectionChecks_Load(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();
            DataColumn document_number = new DataColumn();
            document_number.DataType = System.Type.GetType("System.Double");
            document_number.ColumnName = "document_number"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);                       
            document_number.ReadOnly = true;
            dt.Columns.Add(document_number);

            DataColumn date_time_write = new DataColumn();
            date_time_write.DataType = System.Type.GetType("System.String");
            date_time_write.ColumnName = "date_time_write"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);                       
            date_time_write.ReadOnly = true;
            dt.Columns.Add(date_time_write);

            DataColumn cash_money = new DataColumn();
            cash_money.DataType = System.Type.GetType("System.Double");
            cash_money.ColumnName = "cash_money"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);                       
            cash_money.ReadOnly = true;
            dt.Columns.Add(cash_money);

            DataColumn non_cash_money = new DataColumn();
            non_cash_money.DataType = System.Type.GetType("System.Double");
            non_cash_money.ColumnName = "non_cash_money"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);                       
            non_cash_money.ReadOnly = true;
            dt.Columns.Add(non_cash_money);

            DataColumn sertificate_money = new DataColumn();
            sertificate_money.DataType = System.Type.GetType("System.Double");
            sertificate_money.ColumnName = "sertificate_money"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);                       
            sertificate_money.ReadOnly = true;
            dt.Columns.Add(sertificate_money);

            DataColumn correction = new DataColumn();
            correction.DataType = typeof(bool);
            correction.ColumnName = "correction"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);                       
            //correction.ReadOnly = true;
            dt.Columns.Add(correction);

            DataColumn modified = new DataColumn();
            modified.DataType = typeof(bool);
            modified.ColumnName = "modified"; //listView1.Columns.Add("Код", 100, HorizontalAlignment.Left);                       
            //modified.ReadOnly = true;
            dt.Columns.Add(modified);           

        }

        private void btn_fill_checks_Click(object sender, EventArgs e)
        {
            loaddocuments();
        }

        public void loaddocuments()
        {
            //listView1.Items.Clear();
            dt.Rows.Clear();
            NpgsqlConnection conn = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                //string myQuery = "SELECT checks_header.date_time_write,clients.name,checks_header.cash,checks_header.remainder  FROM checks_header left join clients ON checks_header.client=clients.code WHERE checks_header.date_time_write BETWEEN '" + data_start.Text + "' and '" + data_finish.Text + "' order by checks_header.date_time_write  ";
                string myQuery = "SELECT checks_header.its_deleted," +
                    "checks_header.date_time_write," +
                    "clients.name," +
                    "checks_header.cash," +
                    "checks_header.remainder," +
                    "checks_header.comment," +
                    "checks_header.cash_money," +
                    "checks_header.non_cash_money," +
                    "checks_header.sertificate_money," +
                    "checks_header.its_print," +
                    "checks_header.check_type,checks_header.document_number,checks_header.its_print_p  " +
                    " FROM checks_header left join clients ON checks_header.client=clients.code " +
                    " WHERE checks_header.date_time_write BETWEEN '" + dateTimePicker1.Value.ToString("yyy-MM-dd") + " 00:00:00" + "' and '" + dateTimePicker1.Value.AddDays(1).ToString("yyy-MM-dd") + " 00:00:00" 
                    + "' AND its_deleted=0 AND check_type = 0 order by checks_header.date_time_write  ";
                //if (checkBox_show_3_last_checks.CheckState == CheckState.Checked)
                //{
                //    myQuery += " desc limit 3 ";
                //}
                ////MessageBox.Show(myQuery);
                NpgsqlCommand command = new NpgsqlCommand(myQuery, conn);
                NpgsqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DataRow row =  dt.NewRow();
                    row["document_number"] = reader["document_number"];
                    row["cash_money"] = reader["cash_money"];
                    row["non_cash_money"] = reader["non_cash_money"];
                    row["sertificate_money"] = reader["sertificate_money"];
                    row["date_time_write"] = reader["date_time_write"];
                    row["correction"] = true;
                    dt.Rows.Add(row);                   
                }
                reader.Close();
                conn.Close();
                command.Dispose();
                DataSet ds = new DataSet();

                dataGridView1.DataSource = dt;
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

            //listView1.Focus();
            //this.listView1.Select();
            //if (checkBox_show_3_last_checks.CheckState == CheckState.Checked)
            //{
            //    if (listView1.Items.Count > 0)
            //    {
            //        this.listView1.Items[0].Selected = true;
            //        this.listView1.Items[0].Focused = true;
            //        this.listView1.EnsureVisible(0);
            //    }
            //}
            //else
            //{
            //    if (listView1.Items.Count > 0)
            //    {
            //        this.listView1.Items[this.listView1.Items.Count - 1].Selected = true;
            //        this.listView1.Items[this.listView1.Items.Count - 1].Focused = true;
            //        this.listView1.EnsureVisible(this.listView1.Items.Count - 1);
            //    }
            //}
        }

        private void create_check(string num_doc)
        {
            Cash_check cash_Check = new Cash_check();
            cash_Check.Cash_check_Load(null, null);
            cash_Check.external_fix = true;
            //cash_Check.check_type.Items.Add("Продажа");
            //cash_Check.check_type.Items.Add("Возврат");
            //cash_Check.check_type.Items.Add("КоррекцияПродажи");            
            cash_Check.check_type.SelectedIndex = 2;
            cash_Check.txtB_num_sales.Text = num_doc;
            cash_Check.tax_order = txtB_tax_order.Text;
            //cash_Check.num_cash.Tag = MainStaticClass.CashDeskNumber.ToString();
            //cash_Check.user.Tag = MainStaticClass.Cash_Operator_Client_Code;
            //cash_Check.it_is_paid(string pay, string sum_doc, string remainder, string pay_bonus_many, bool last_rewrite, string cash_money, string non_cash_money, string sertificate_money)

            cash_Check.fill_on_sales();

            string sertificate_money = cash_Check.txtB_sertificate_money.Text;
            string non_cash_money = cash_Check.txtB_non_cash_money.Text;
            string cash_money = cash_Check.txtB_cash_money.Text;
            
            string sum_doc = cash_Check.calculation_of_the_sum_of_the_document().ToString().Replace(",", ".");
            cash_Check.it_is_paid(sum_doc     , sum_doc, "0", "0", true, cash_money, non_cash_money, sertificate_money);
                                    
            //PrintingUsingLibraries printing = new PrintingUsingLibraries();
            //printing.print_sell_2_or_return_sell(cash_Check);

            //cash_Check.Show();
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToBoolean(row["correction"]))
                {
                    create_check(row["document_number"].ToString());
                    row["correction"] = false;
                    row["modified"] = true;
                }
            }
        }

        private void btn_enable_Click(object sender, EventArgs e)
        {
            if (txtB_password.Text.Trim() == "123698745")
            {
                btn_print.Enabled       = true;
                txtB_tax_order.Enabled  = true;
                dataGridView1.Enabled   = true;
                dateTimePicker1.Enabled = true;
                btn_fill_checks.Enabled = true;
            }
            else
            {
                MessageBox.Show("Вы ввели неправильный пароль ", "Ошибка при вводе пароля");
                txtB_password.Text = "";
            }
        }

        private void btn_check_all_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dt.Rows)
            {
                row["correction"] = true;
            }
        }

        private void btn_uncheck_all_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dt.Rows)
            {
                row["correction"] = false;
            }
        }
    }
}
