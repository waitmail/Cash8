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
    public partial class Actions : Form
    {
        public Actions()
        {
            InitializeComponent();
            this.Load += new EventHandler(Actions_Load);
        }

        private void load_actions()
        {
            NpgsqlConnection conn = null;
            NpgsqlCommand command = null;
            try
            {
                conn = MainStaticClass.NpgsqlConn();
                conn.Open();
                string query = "SELECT action_header.date_started, action_header.date_end, action_header.num_doc, action_header.tip, action_header.barcode, action_header.persent, action_header.sum,action_header.comment,tovar.name   FROM action_header left join tovar on action_header.code_tovar=tovar.code ;";
                command = new NpgsqlCommand(query, conn);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ListViewItem lvi = new ListViewItem(reader.GetInt32(2).ToString());
                    lvi.Tag = reader.GetInt32(2).ToString();
                    lvi.SubItems.Add(reader.GetDateTime(0).ToString());
                    lvi.SubItems.Add(reader.GetDateTime(1).ToString());
                    lvi.SubItems.Add(reader.GetInt16(3).ToString());
                    lvi.SubItems.Add(reader.GetString(4).ToString());
                    lvi.SubItems.Add(reader.GetDecimal(5).ToString());
                    lvi.SubItems.Add(reader.GetDecimal(6).ToString());
                    lvi.SubItems.Add(reader.GetString(7).ToString());
                    //lvi.SubItems.Add(reader.GetString(8).ToString());
                    lvi.SubItems.Add(reader[8].ToString());
                    listView_doc.Items.Add(lvi);
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

        }

        private void Actions_Load(object sender, EventArgs e)
        {
            //Главный список
            listView_doc.View = View.Details;

            // Allow the user to rearrange columns.
            listView_doc.AllowColumnReorder = true;

            // Select the item and subitems when selection is made.
            listView_doc.FullRowSelect = true;

            // Display grid lines.
            listView_doc.GridLines = true;
            listView_doc.Columns.Add("Номер", 50, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Дата начала", 100, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Дата окончания", 100, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Тип акции", 100, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Штрихкод", 100, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Процент", 100, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Сумма", 100, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Коментарий", 100, HorizontalAlignment.Left);
            listView_doc.Columns.Add("Товар", 100, HorizontalAlignment.Left);
            load_actions();
        }
    }
}
